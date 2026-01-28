using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.U2D.Animation;

public class SyncSpriteController : MonoBehaviour, IUpdatable
{
    private List<SpriteResolver> resolvers;
    private SyncDataPacket syncDataSprite;
    private string direction;

    [Header("Chỉ định sprite nào của player sẽ bị thay thế")]
    [SerializeField] private List<SpriteLibrary> spriteLibrary;

    private ItemController listItem0;

    // id của trang bị thực tế từ database
    private int weaponData = 0;
    private int helmetData = 0;
    private int armorData = 0;
    private int legArmorData = 0;
    private int hairData = 0;

    void Awake()
    {
        resolvers = GetComponentsInChildren<SpriteResolver>().ToList();
        listItem0 = ItemController.Instance;
    }

    private void OnEnable()
    {
        GameManager.Instance.Register(this);
    }
    private void OnDisable()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.Unregister(this);
        }
    }
    public void RegisterDontDestroyOnLoad()
    {
        GameManager.Instance.RegisterPersistent(this);
    }
    public void ApplyServerState(SyncDataPacket data)
    {
        syncDataSprite = data;

        if (weaponData != data.weapon)
        {
            weaponData = syncDataSprite.weapon;
            EquipWeapon(weaponData);
        }
        if (helmetData != data.helmet)
        {
            helmetData = syncDataSprite.helmet;
            EquipHelmet(helmetData);
        }
        if (armorData != data.armor)
        {
            armorData = syncDataSprite.armor;
            EquipArmor(armorData);
        }
        if (legArmorData != data.legArmor)
        {
            legArmorData = syncDataSprite.legArmor;
            EquipLegArmor(legArmorData);
        }
        if (hairData != data.hair)
        {
            hairData = syncDataSprite.hair;
            EquipHair(hairData);
        }
        UpdateSprite();
    }
    public void OnUpdate() { }
    public void OnLateUpdate() { }
    public void OnFixedUpdate() { }

    #region Sửa sprite library sau khi equip item
    public void EquipLegArmor(int id)
    {
        spriteLibrary[0].spriteLibraryAsset = listItem0.GetItem0(id).legArmor.legArmorLibrariesAsset;
    }
    public void EquipArmor(int id)
    {
        spriteLibrary[1].spriteLibraryAsset = listItem0.GetItem0(id).armor.armorLibrariesAsset;
    }
    public void EquipHelmet(int id)
    {
        spriteLibrary[3].spriteLibraryAsset = listItem0.GetItem0(id).helmet.helmetLibrariesAsset;

        if (listItem0.GetItem0(id).helmet.isHiddenHair)
        {
            spriteLibrary[4].gameObject.SetActive(false);
        }
        else
        {
            spriteLibrary[4].gameObject.SetActive(true);
        }
    }
    public void EquipHair(int id)
    {
        switch (syncDataSprite.idSchool)
        {
            case 1: //Chiến binh
                spriteLibrary[4].spriteLibraryAsset = listItem0.GetMaleHairLibrary(id).hairLibrariesAsset;
                break;

            case 2: //Sát thủ 
                spriteLibrary[4].spriteLibraryAsset = listItem0.GetMaleHairLibrary(id).hairLibrariesAsset;
                break;

            case 3: //Pháp sư
                spriteLibrary[4].spriteLibraryAsset = listItem0.GetFemaleHairLibrary(id).hairLibrariesAsset;
                break;

            case 4: //Xạ thủ 
                spriteLibrary[4].spriteLibraryAsset = listItem0.GetFemaleHairLibrary(id).hairLibrariesAsset;
                break;
        }
    }
    public void EquipWeapon(int id)
    {
        spriteLibrary[5].spriteLibraryAsset = listItem0.GetItem0(id).weapon.weaponFrontLibraries;
        spriteLibrary[6].spriteLibraryAsset = listItem0.GetItem0(id).weapon.weaponBackLibraries;
    }
    #endregion

    private void UpdateSprite()
    {
        Direction syncDirection = (Direction)syncDataSprite.direction;
        PlayerState syncState = (PlayerState)syncDataSprite.state;

        switch (syncDirection)
        {
            case Direction.Front:
                direction = "Front"; break;

            case Direction.Back:
                direction = "Back"; break;

            case Direction.Left:
                direction = "Left"; break;

            case Direction.Right:
                direction = "Right"; break;

            default:
                direction = "Front"; break;
        }
        switch (syncState)
        {
            case PlayerState.Stand:
                {
                    SetAllResolvers("Stand", $"Stand{direction}");
                    foreach (var r in resolvers)
                    {
                        if (r != null && r.spriteLibrary != null && r.gameObject.name == "4_0_0")
                        {
                            r.SetCategoryAndLabel("Stand", $"Stand{direction}Frame{syncDataSprite.frame}");
                            r.ResolveSpriteToSpriteRenderer();
                        }
                    }
                    break;
                }

            case PlayerState.Move:
                {
                    SetAllResolvers("Move", $"Move{direction}Frame{syncDataSprite.frame}");
                    break;
                }

            case PlayerState.Attack:
                {
                    SetAllResolvers("Atk", $"Atk{direction}Frame{syncDataSprite.frame}");
                    break;
                }

            case PlayerState.Injured:
                {
                    SetAllResolvers("Stand", $"Stand{direction}");
                    foreach (var r in resolvers)
                    {
                        if (r != null && r.spriteLibrary != null && r.gameObject.name == "4_0_0")
                        {
                            r.SetCategoryAndLabel("Injured", $"Injured{direction}Frame{syncDataSprite.frame}");
                            r.ResolveSpriteToSpriteRenderer();
                        }
                    }
                    break;
                }

            case PlayerState.Die:
                {
                    SetAllResolvers("Die", "DieFrame0");
                    break;
                }
        }
    }

    protected void SetAllResolvers(string category, string label)
    {
        foreach (var r in resolvers)
        {
            if (r != null && r.spriteLibrary != null)
            {
                r.SetCategoryAndLabel(category, label);
                r.ResolveSpriteToSpriteRenderer();
            }
        }
    }
}