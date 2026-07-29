using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.U2D.Animation;
using UnityEngine.UI;

public class SyncSpriteController : MonoBehaviour, IUpdatable
{
    [SerializeField] private GameObject shadow;
    [SerializeField] private GameObject waterShadow;

    private bool isStandingInWater = false;
    private TileType currentTileType;

    private List<SpriteRenderer> spriteRenderers = new List<SpriteRenderer>();

    private PlayerData otherPlayerData = new PlayerData();
    private PlayerTransformData otherPlayerTransform = new PlayerTransformData();
    private PlayerStateData otherPlayerState = new PlayerStateData();
    private Dictionary<(int, int, Category, Label), (PositionData, RotationData, ScaleData, ColorData)> bodyDatas;

    [Header("Chỉ định sprite nào của player sẽ bị thay thế")]
    [SerializeField] private List<SpriteLibrary> spriteLibraries;
    [SerializeField] private List<SpriteResolver> spriteResolvers;

    private Dictionary<int, (Category, Label)> spriteResolversInfos = new Dictionary<int, (Category, Label)>();

    [SerializeField] private Slider hpBar;

    private ItemController listItem0;

    // id của trang bị thực tế từ database
    private int weaponData = 0;
    private int helmetData = 0;
    private int armorData = 0;
    private int legArmorData = 0;
    private int hairData = 0;

    private void Awake()
    {
        listItem0 = ItemController.Instance;

        if (waterShadow != null)
            waterShadow.SetActive(false);

        for (int i = 0; i < spriteResolvers.Count; i++)
        {
            SpriteRenderer spriteRenderer = spriteResolvers[i].GetComponent<SpriteRenderer>();
            if (spriteRenderer != null)
            {
                spriteRenderers.Add(spriteRenderer);
            }
        }
        bodyDatas = PlayerManager.bodyDatas;
        InitSpriteResolversInfos();
    }
    private Category ConvertCategory(string category)
    {
        switch (category)
        {
            case "Stand":
                return Category.Stand;

            case "Move":
                return Category.Move;

            case "Atk":
                return Category.Atk;

            case "Injured":
                return Category.Injured;

            case "Die":
                return Category.Die;

            default:
                return Category.Stand;
        }
    }
    private string ConvertCategory(Category category)
    {
        switch (category)
        {
            case Category.Stand:
                return "Stand";

            case Category.Move:
                return "Move";

            case Category.Atk:
                return "Atk";

            case Category.Injured:
                return "Injured";

            case Category.Die:
                return "Die";

            default:
                return "Stand";
        }
    }
    private Label ConvertLabel(string label)
    {
        switch (label)
        {
            case "StandFrontFrame0":
                return Label.StandFrontFrame0;
            case "StandFrontFrame1":
                return Label.StandFrontFrame1;

            case "MoveFrontFrame0":
                return Label.MoveFrontFrame0;
            case "MoveFrontFrame1":
                return Label.MoveFrontFrame1;

            case "MoveBackFrame0":
                return Label.MoveBackFrame0;
            case "MoveBackFrame1":
                return Label.MoveBackFrame1;

            case "MoveLeftFrame0":
                return Label.MoveLeftFrame0;
            case "MoveLeftFrame1":
                return Label.MoveLeftFrame1;

            case "MoveRightFrame0":
                return Label.MoveRightFrame0;
            case "MoveRightFrame1":
                return Label.MoveRightFrame1;

            case "AtkFrontFrame0":
                return Label.AtkFrontFrame0;
            case "AtkFrontFrame1":
                return Label.AtkFrontFrame1;

            case "AtkBackFrame0":
                return Label.AtkBackFrame0;
            case "AtkBackFrame1":
                return Label.AtkBackFrame1;

            case "AtkLeftFrame0":
                return Label.AtkLeftFrame0;
            case "AtkLeftFrame1":
                return Label.AtkLeftFrame1;

            case "AtkRightFrame0":
                return Label.AtkRightFrame0;
            case "AtkRightFrame1":
                return Label.AtkRightFrame1;

            case "InjuredFrontFrame0":
                return Label.InjuredFrontFrame0;
            case "InjuredFrontFrame1":
                return Label.InjuredFrontFrame1;

            case "InjuredBackFrame0":
                return Label.InjuredBackFrame0;
            case "InjuredBackFrame1":
                return Label.InjuredBackFrame1;

            case "InjuredLeftFrame0":
                return Label.InjuredLeftFrame0;
            case "InjuredLeftFrame1":
                return Label.InjuredLeftFrame1;

            case "InjuredRightFrame0":
                return Label.InjuredRightFrame0;
            case "InjuredRightFrame1":
                return Label.InjuredRightFrame1;

            case "DieFrame0":
                return Label.DieFrame0;

            default:
                return Label.StandFrontFrame0;
        }
    }
    private string ConvertLabel(Label label)
    {
        switch (label)
        {
            case Label.StandFrontFrame0:
                return "StandFrontFrame0";
            case Label.StandFrontFrame1:
                return "StandFrontFrame1";
            case Label.StandBackFrame0:
                return "StandBackFrame0";
            case Label.StandBackFrame1:
                return "StandBackFrame1";
            case Label.StandLeftFrame0:
                return "StandLeftFrame0";
            case Label.StandLeftFrame1:
                return "StandLeftFrame1";
            case Label.StandRightFrame0:
                return "StandRightFrame0";
            case Label.StandRightFrame1:
                return "StandRightFrame1";

            case Label.MoveFrontFrame0:
                return "MoveFrontFrame0";
            case Label.MoveFrontFrame1:
                return "MoveFrontFrame1";
            case Label.MoveBackFrame0:
                return "MoveBackFrame0";
            case Label.MoveBackFrame1:
                return "MoveBackFrame1";
            case Label.MoveLeftFrame0:
                return "MoveLeftFrame0";
            case Label.MoveLeftFrame1:
                return "MoveLeftFrame1";
            case Label.MoveRightFrame0:
                return "MoveRightFrame0";
            case Label.MoveRightFrame1:
                return "MoveRightFrame1";

            case Label.AtkFrontFrame0:
                return "AtkFrontFrame0";
            case Label.AtkFrontFrame1:
                return "AtkFrontFrame1";
            case Label.AtkBackFrame0:
                return "AtkBackFrame0";
            case Label.AtkBackFrame1:
                return "AtkBackFrame1";
            case Label.AtkLeftFrame0:
                return "AtkLeftFrame0";
            case Label.AtkLeftFrame1:
                return "AtkLeftFrame1";
            case Label.AtkRightFrame0:
                return "AtkRightFrame0";
            case Label.AtkRightFrame1:
                return "AtkRightFrame1";

            case Label.InjuredFrontFrame0:
                return "InjuredFrontFrame0";
            case Label.InjuredFrontFrame1:
                return "InjuredFrontFrame1";
            case Label.InjuredBackFrame0:
                return "InjuredBackFrame0";
            case Label.InjuredBackFrame1:
                return "InjuredBackFrame1";
            case Label.InjuredLeftFrame0:
                return "InjuredLeftFrame0";
            case Label.InjuredLeftFrame1:
                return "InjuredLeftFrame1";
            case Label.InjuredRightFrame0:
                return "InjuredRightFrame0";
            case Label.InjuredRightFrame1:
                return "InjuredRightFrame1";

            case Label.DieFrame0:
                return "DieFrame0";

            default:
                return "StandFrontFrame0";
        }
    }
    private void InitSpriteResolversInfos()
    {
        for (int i = 0; i < spriteResolvers.Count; i++)
        {
            var resolver = spriteResolvers[i];

            Category category;
            Label label;

            category = ConvertCategory(resolver.GetCategory());
            label = ConvertLabel(resolver.GetLabel());

            spriteResolversInfos[i] = (category, label);
        }
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

    public void ApplyServerData(PlayerData serverData, PlayerTransformData serverTransform, PlayerStateData serverState)
    {
        if (weaponData != serverData.weapon)
        {
            weaponData = serverData.weapon;
            EquipWeapon(weaponData);
        }
        if (helmetData != serverData.helmet)
        {
            helmetData = serverData.helmet;
            EquipHelmet(helmetData);
        }
        if (armorData != serverData.armor)
        {
            armorData = serverData.armor;
            EquipArmor(armorData);
        }
        if (legArmorData != serverData.legArmor)
        {
            legArmorData = serverData.legArmor;
            EquipLegArmor(legArmorData);
        }
        if (hairData != serverData.hair)
        {
            hairData = serverData.hair;
            EquipHair(hairData, serverData.idSchool);
        }

        otherPlayerData = serverData;
        otherPlayerTransform = serverTransform;
        otherPlayerState = serverState;

        if (hpBar.maxValue != serverData.maxHP)
            hpBar.maxValue = serverData.maxHP;
        if (hpBar.value != serverData.hp)
            hpBar.value = serverData.hp;
        if (currentTileType != serverData.currentTile)
            currentTileType = serverData.currentTile;
    }

    public void OnUpdate() 
    {
        Vector2 targetPos = new Vector2(otherPlayerTransform.positionData.x, otherPlayerTransform.positionData.y);
        transform.position = Vector2.MoveTowards(transform.position, targetPos, 6f * Time.deltaTime);

        Vector3 otherPlayerScale = transform.localScale;
        otherPlayerScale.x = otherPlayerTransform.scaleData.x;
        otherPlayerScale.y = otherPlayerTransform.scaleData.y;
        otherPlayerScale.z = otherPlayerTransform.scaleData.z;
        transform.localScale = otherPlayerScale;

        if (currentTileType == TileType.Water)
        {
            shadow.SetActive(false);
            waterShadow.SetActive(true);

            // chỉ chạy đúng 1 lần khi vừa xuống nước
            if (!isStandingInWater)
            {
                transform.position = new Vector3(transform.position.x, transform.position.y - 0.2f, transform.position.z);
                waterShadow.transform.position = new Vector3(transform.position.x, transform.position.y + 0.2f, transform.position.z);

                isStandingInWater = true;
            }
        }
        else
        {
            shadow.SetActive(true);
            waterShadow.SetActive(false);

            // chỉ chạy đúng 1 lần khi vừa lên bờ
            if (isStandingInWater)
            {
                transform.position = new Vector3(transform.position.x, transform.position.y + 0.2f, transform.position.z);
                waterShadow.transform.position = new Vector3(transform.position.x, transform.position.y - 0.2f, transform.position.z);

                isStandingInWater = false;
            }
        }

        UpdateSprite();
    }
    public void OnLateUpdate() { }
    public void OnFixedUpdate() { }

    #region Sửa sprite library sau khi equip item
    public void EquipLegArmor(int id)
    {
        spriteLibraries[0].spriteLibraryAsset = listItem0.GetItem0(id).legArmor.legArmorLibrariesAsset;
    }
    public void EquipArmor(int id)
    {
        spriteLibraries[1].spriteLibraryAsset = listItem0.GetItem0(id).armor.armorLibrariesAsset;
    }
    public void EquipHelmet(int id)
    {
        spriteLibraries[3].spriteLibraryAsset = listItem0.GetItem0(id).helmet.helmetLibrariesAsset;

        if (listItem0.GetItem0(id).helmet.isHiddenHair)
        {
            spriteLibraries[5].gameObject.SetActive(false);
        }
        else
        {
            spriteLibraries[5].gameObject.SetActive(true);
        }
    }
    public void EquipHair(int id, int idSchool)
    {
        switch (idSchool)
        {
            case 1: //Chiến binh
                spriteLibraries[5].spriteLibraryAsset = listItem0.GetMaleHairLibrary(id).hairLibrariesAsset;
                break;

            case 2: //Sát thủ 
                spriteLibraries[5].spriteLibraryAsset = listItem0.GetMaleHairLibrary(id).hairLibrariesAsset;
                break;

            case 3: //Pháp sư
                spriteLibraries[5].spriteLibraryAsset = listItem0.GetFemaleHairLibrary(id).hairLibrariesAsset;
                break;

            case 4: //Xạ thủ 
                spriteLibraries[5].spriteLibraryAsset = listItem0.GetFemaleHairLibrary(id).hairLibrariesAsset;
                break;
        }
    }
    public void EquipWeapon(int id)
    {
        spriteLibraries[6].spriteLibraryAsset = listItem0.GetItem0(id).weapon.weaponFrontLibraries;
        spriteLibraries[7].spriteLibraryAsset = listItem0.GetItem0(id).weapon.weaponBackLibraries;
    }
    #endregion

    private void UpdateSprite()
    {
        for (int i = 0; i < otherPlayerState.partBodyTransforms.Count; i++)
        {
            if (spriteResolversInfos[i].Item1 != otherPlayerState.partBodyTransforms[i].category || spriteResolversInfos[i].Item2 != otherPlayerState.partBodyTransforms[i].label)
            {
                string category = ConvertCategory(otherPlayerState.partBodyTransforms[i].category);
                string label = ConvertLabel(otherPlayerState.partBodyTransforms[i].label);
                spriteResolvers[i].SetCategoryAndLabel(category, label);
                spriteResolversInfos[i] = ((Category)otherPlayerState.partBodyTransforms[i].category, (Label)otherPlayerState.partBodyTransforms[i].label);

                if (bodyDatas.TryGetValue((otherPlayerData.idSchool, i, otherPlayerState.partBodyTransforms[i].category, otherPlayerState.partBodyTransforms[i].label), out var bodyData))
                {
                    Vector3 currentPositionPartBody = spriteResolvers[i].transform.localPosition;
                    currentPositionPartBody.x = bodyData.Item1.x;
                    currentPositionPartBody.y = bodyData.Item1.y;
                    currentPositionPartBody.z = bodyData.Item1.z;
                    spriteResolvers[i].transform.localPosition = currentPositionPartBody;

                    Quaternion currentRotationPartBody = spriteResolvers[i].transform.localRotation;
                    spriteResolvers[i].transform.localRotation = Quaternion.Euler(bodyData.Item2.x, bodyData.Item2.y, bodyData.Item2.z);

                    Vector3 currentScalePartBody = spriteResolvers[i].transform.localScale;
                    currentScalePartBody.x = bodyData.Item3.x;
                    currentScalePartBody.y = bodyData.Item3.y;
                    currentScalePartBody.z = bodyData.Item3.z;
                    spriteResolvers[i].transform.localScale = currentScalePartBody;

                    Color currentColorPartBody = spriteRenderers[i].color;
                    currentColorPartBody.a = bodyData.Item4.a;
                    spriteRenderers[i].color = currentColorPartBody;
                }
            }
        }
    }
}