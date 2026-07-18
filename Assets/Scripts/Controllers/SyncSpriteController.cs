using System.Collections.Generic;
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

    private PlayerTransformData otherPlayerTransform = new PlayerTransformData();
    private PlayerStateData otherPlayerState = new PlayerStateData();

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
    private Label ConvertLabel(string label)
    {
        switch (label)
        {
            case "StandFront":
                return Label.StandFront;
            case "StandBack":
                return Label.StandBack;
            case "StandLeft":
                return Label.StandLeft;
            case "StandRight":
                return Label.StandRight;

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
                return Label.StandFront;
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

        otherPlayerTransform = serverTransform;
        otherPlayerState = serverState;

        hpBar.maxValue = serverData.maxHP;
        hpBar.value = serverData.hp;

        currentTileType = serverData.currentTile;

        UpdateSprite();
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
                spriteResolvers[i].SetCategoryAndLabel(((Category)otherPlayerState.partBodyTransforms[i].category).ToString(), ((Label)otherPlayerState.partBodyTransforms[i].label).ToString());
                spriteResolversInfos[i] = ((Category)otherPlayerState.partBodyTransforms[i].category, (Label)otherPlayerState.partBodyTransforms[i].label);
            }

            Vector3 currentPositionPartBody = spriteResolvers[i].transform.localPosition;
            if (currentPositionPartBody.x != otherPlayerState.partBodyTransforms[i].positionData.x 
                || currentPositionPartBody.y != otherPlayerState.partBodyTransforms[i].positionData.y 
                || currentPositionPartBody.z != otherPlayerState.partBodyTransforms[i].positionData.z)
            {
                currentPositionPartBody.x = otherPlayerState.partBodyTransforms[i].positionData.x;
                currentPositionPartBody.y = otherPlayerState.partBodyTransforms[i].positionData.y;
                currentPositionPartBody.z = otherPlayerState.partBodyTransforms[i].positionData.z;
                spriteResolvers[i].transform.localPosition = currentPositionPartBody;
            }

            Quaternion currentRotationPartBody = spriteResolvers[i].transform.localRotation;
            if (currentRotationPartBody.x != otherPlayerState.partBodyTransforms[i].rotationData.x 
                || currentRotationPartBody.y != otherPlayerState.partBodyTransforms[i].rotationData.y 
                || currentRotationPartBody.z != otherPlayerState.partBodyTransforms[i].rotationData.z)
            {
                spriteResolvers[i].transform.localRotation = Quaternion.Euler(otherPlayerState.partBodyTransforms[i].rotationData.x, otherPlayerState.partBodyTransforms[i].rotationData.y, otherPlayerState.partBodyTransforms[i].rotationData.z);
            }

            Vector3 currentScalePartBody = spriteResolvers[i].transform.localScale;
            if (currentScalePartBody.x != otherPlayerState.partBodyTransforms[i].scaleData.x 
                || currentScalePartBody.y != otherPlayerState.partBodyTransforms[i].scaleData.y 
                || currentScalePartBody.z != otherPlayerState.partBodyTransforms[i].scaleData.z)
            {
                currentScalePartBody.x = otherPlayerState.partBodyTransforms[i].scaleData.x;
                currentScalePartBody.y = otherPlayerState.partBodyTransforms[i].scaleData.y;
                currentScalePartBody.z = otherPlayerState.partBodyTransforms[i].scaleData.z;
                spriteResolvers[i].transform.localScale = currentScalePartBody;
            }

            Color currentColorPartBody = spriteRenderers[i].color;
            if (currentColorPartBody.a != otherPlayerState.partBodyTransforms[i].colorData.a)
            {
                currentColorPartBody.a = otherPlayerState.partBodyTransforms[i].colorData.a;
                spriteRenderers[i].color = currentColorPartBody;
            }
        }
    }
}