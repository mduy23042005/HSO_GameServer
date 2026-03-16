using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.U2D.Animation;

public class SyncSpriteController : MonoBehaviour, IUpdatable
{
    PlayerTransformData otherPlayerTransform = new PlayerTransformData();
    PlayerStateData otherPlayerState = new PlayerStateData();

    private List<SpriteResolver> resolvers;

    [Header("Chỉ định sprite nào của player sẽ bị thay thế")]
    [SerializeField] private List<SpriteLibrary> spriteLibrary;

    private ItemController listItem0;

    // id của trang bị thực tế từ database
    private int weaponData = 0;
    private int helmetData = 0;
    private int armorData = 0;
    private int legArmorData = 0;
    private int hairData = 0;

    private void Awake()
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

        otherPlayerTransform = serverTransform;
        otherPlayerState = serverState;

        UpdateSprite();
    }

    public void OnUpdate() { }
    public void OnLateUpdate() { }
    public void OnFixedUpdate()
    {
        Vector2 targetPos = new Vector2(otherPlayerTransform.positionData.x, otherPlayerTransform.positionData.y);
        transform.position = Vector2.MoveTowards(transform.position, targetPos, 6f * Time.fixedDeltaTime);

        Vector3 otherPlayerScale = transform.localScale;
        otherPlayerScale.x = otherPlayerTransform.scaleData.x;
        otherPlayerScale.y = otherPlayerTransform.scaleData.y;
        otherPlayerScale.z = otherPlayerTransform.scaleData.z;
        transform.localScale = otherPlayerScale;
    }

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
            spriteLibrary[5].gameObject.SetActive(false);
        }
        else
        {
            spriteLibrary[5].gameObject.SetActive(true);
        }
    }
    public void EquipHair(int id, int idSchool)
    {
        switch (idSchool)
        {
            case 1: //Chiến binh
                spriteLibrary[5].spriteLibraryAsset = listItem0.GetMaleHairLibrary(id).hairLibrariesAsset;
                break;

            case 2: //Sát thủ 
                spriteLibrary[5].spriteLibraryAsset = listItem0.GetMaleHairLibrary(id).hairLibrariesAsset;
                break;

            case 3: //Pháp sư
                spriteLibrary[5].spriteLibraryAsset = listItem0.GetFemaleHairLibrary(id).hairLibrariesAsset;
                break;

            case 4: //Xạ thủ 
                spriteLibrary[5].spriteLibraryAsset = listItem0.GetFemaleHairLibrary(id).hairLibrariesAsset;
                break;
        }
    }
    public void EquipWeapon(int id)
    {
        spriteLibrary[6].spriteLibraryAsset = listItem0.GetItem0(id).weapon.weaponFrontLibraries;
        spriteLibrary[7].spriteLibraryAsset = listItem0.GetItem0(id).weapon.weaponBackLibraries;
    }
    #endregion


    private void UpdateSprite()
    {
        for (int i = 0; i < otherPlayerState.partBodyTransforms.Count; i++)
        {
            Vector3 pos = resolvers[i].transform.localPosition;
            pos.x = otherPlayerState.partBodyTransforms[i].positionData.x;
            pos.y = otherPlayerState.partBodyTransforms[i].positionData.y;
            pos.z = otherPlayerState.partBodyTransforms[i].positionData.z;
            resolvers[i].transform.localPosition = pos;

            var rot = otherPlayerState.partBodyTransforms[i].rotationData;
            resolvers[i].transform.localRotation = Quaternion.Euler(rot.x, rot.y, rot.z);

            Vector3 scale = resolvers[i].transform.localScale;
            scale.x = otherPlayerState.partBodyTransforms[i].scaleData.x;
            scale.y = otherPlayerState.partBodyTransforms[i].scaleData.y;
            scale.z = otherPlayerState.partBodyTransforms[i].scaleData.z;
            resolvers[i].transform.localScale = scale;

            Renderer renderer = resolvers[i].GetComponent<Renderer>();
            Color color = renderer.material.color;
            color.r = otherPlayerState.partBodyTransforms[i].colorData.r;
            color.g = otherPlayerState.partBodyTransforms[i].colorData.g;
            color.b = otherPlayerState.partBodyTransforms[i].colorData.b;
            color.a = otherPlayerState.partBodyTransforms[i].colorData.a;
            renderer.material.color = color;

            SetAllResolvers(i, otherPlayerState.partBodyTransforms[i].category, otherPlayerState.partBodyTransforms[i].label);
        }
    }
    private void SetAllResolvers(int idResolver, string category, string label)
    {
        resolvers[idResolver].SetCategoryAndLabel(category, label);
        resolvers[idResolver].ResolveSpriteToSpriteRenderer();
    }
}