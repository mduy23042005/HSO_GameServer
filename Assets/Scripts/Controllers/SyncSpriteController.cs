using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D.Animation;
using UnityEngine.UI;

public class SyncSpriteController : MonoBehaviour, IUpdatable
{
    private PlayerTransformData otherPlayerTransform = new PlayerTransformData();
    private PlayerStateData otherPlayerState = new PlayerStateData();

    [Header("Chỉ định sprite nào của player sẽ bị thay thế")]
    [SerializeField] private List<SpriteLibrary> spriteLibraries;
    [SerializeField] private List<SpriteResolver> spriteResolvers;

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
            Vector3 newPositionPartBody = spriteResolvers[i].transform.localPosition;
            Vector3 currentPositionPartBody = new Vector3(otherPlayerState.partBodyTransforms[i].positionData.x, otherPlayerState.partBodyTransforms[i].positionData.y, otherPlayerState.partBodyTransforms[i].positionData.z);
            if (newPositionPartBody != currentPositionPartBody)
            {
                newPositionPartBody.x = otherPlayerState.partBodyTransforms[i].positionData.x;
                newPositionPartBody.y = otherPlayerState.partBodyTransforms[i].positionData.y;
                newPositionPartBody.z = otherPlayerState.partBodyTransforms[i].positionData.z;
                spriteResolvers[i].transform.localPosition = newPositionPartBody;
            }

            var newRotationPartBody = Quaternion.Euler(otherPlayerState.partBodyTransforms[i].rotationData.x, otherPlayerState.partBodyTransforms[i].rotationData.y, otherPlayerState.partBodyTransforms[i].rotationData.z); ;
            Quaternion currentRotationPartBody = spriteResolvers[i].transform.localRotation;
            if (currentRotationPartBody != newRotationPartBody)
            {
                spriteResolvers[i].transform.localRotation = newRotationPartBody;
            }

            Vector3 newScalePartBody = spriteResolvers[i].transform.localScale;
            Vector3 currentScalePartBody = new Vector3(otherPlayerState.partBodyTransforms[i].scaleData.x, otherPlayerState.partBodyTransforms[i].scaleData.y, otherPlayerState.partBodyTransforms[i].scaleData.z);
            if (currentScalePartBody != newScalePartBody)
            {
                newScalePartBody.x = otherPlayerState.partBodyTransforms[i].scaleData.x;
                newScalePartBody.y = otherPlayerState.partBodyTransforms[i].scaleData.y;
                newScalePartBody.z = otherPlayerState.partBodyTransforms[i].scaleData.z;
                spriteResolvers[i].transform.localScale = newScalePartBody;
            }

            SpriteRenderer renderer = spriteResolvers[i].GetComponent<SpriteRenderer>();
            Color colorPartBody = renderer.color;
            colorPartBody.r = otherPlayerState.partBodyTransforms[i].colorData.r;
            colorPartBody.g = otherPlayerState.partBodyTransforms[i].colorData.g;
            colorPartBody.b = otherPlayerState.partBodyTransforms[i].colorData.b;
            colorPartBody.a = otherPlayerState.partBodyTransforms[i].colorData.a;
            renderer.material.color = colorPartBody;

            if (spriteResolvers[i].GetCategory() != otherPlayerState.partBodyTransforms[i].category || spriteResolvers[i].GetLabel() != otherPlayerState.partBodyTransforms[i].label)
            {
                spriteResolvers[i].SetCategoryAndLabel(otherPlayerState.partBodyTransforms[i].category, otherPlayerState.partBodyTransforms[i].label);
            }
        }
    }
}