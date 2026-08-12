using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.U2D.Animation;

public class SpritePlayerController : MonoBehaviour, IUpdatable
{
    private SpriteResolver faceResolver;
    private Animator animator;
    private string lastCategory;
    private string lastLabel;
    private MovementPlayerController movementPlayerController;
    private Direction currentDirection;
    private bool isInjured;
    private EquipmentResultPacket outfitData = new EquipmentResultPacket();

    [Header("Chỉ định sprite nào của player sẽ bị thay thế")]
    [SerializeField] private List<SpriteLibrary> spriteLibraries;
    [SerializeField] private List<SpriteResolver> spriteResolvers;

    private Dictionary<int, (Category, Label)> spriteResolversInfos = new Dictionary<int, (Category, Label)>();

    private ItemController listItem0;

    private float[] moveChangeTimesInStandClip = { 0f, 0.5f, 1f }; // Clip dài 0:40 giây, đổi frame ở 0 / 0.4, 0.2 / 0.4
    private float[] moveChangeTimesInMoveClip = { 0f, 0.5f, 1f }; // Clip dài 0:40 giây, đổi frame ở 0 / 0.2, 0.1 / 0.2
    private float[] moveChangeTimesInAtkClip = { 0f, 0.6667f, 1f }; // Clip dài 0:15 giây, đổi frame ở 0 / 0.15, 0.1 / 0.15

    // id của trang bị thực tế từ database
    private int weaponData = 0;
    private int helmetData = 0;
    private int armorData = 0;
    private int legArmorData = 0;
    private int hairData = 0;
    private int headData = 0;

    private int standStateHash;
    private int moveStateHash;
    private int atkStateHash;
    private int dieStateHash;

    private void Awake()
    {
        faceResolver = spriteResolvers.FirstOrDefault(r => r.gameObject.name == "4_0_0");
        animator = GetComponent<Animator>();
        movementPlayerController = GetComponent<MovementPlayerController>();
        listItem0 = ItemController.Instance;

        // hàm Animator.StringToHash() truyền vào ký tự theo cú pháp [tên layer].[tên state]
        // mục tiêu của khởi tạo các biến này là so sánh với state animator hiện tại (Animator state -> (int)state.fullPathHash)
        standStateHash = Animator.StringToHash("Base Layer.Stand");
        moveStateHash = Animator.StringToHash("Base Layer.Move");
        atkStateHash = Animator.StringToHash("Base Layer.Atk");
        dieStateHash = Animator.StringToHash("Base Layer.Die");

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
            case "StandFrontFrame0": 
                return Label.StandFrontFrame0;
            case "StandFrontFrame1": 
                return Label.StandFrontFrame1;
            case "StandBackFrame0":
                return Label.StandBackFrame0;
            case "StandBackFrame1":
                return Label.StandBackFrame1;
            case "StandLeftFrame0":
                return Label.StandLeftFrame0;
            case "StandLeftFrame1":
                return Label.StandLeftFrame1;
            case "StandRightFrame0":
                return Label.StandRightFrame0;
            case "StandRightFrame1":
                return Label.StandRightFrame1;

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
    public void OnUpdate()
    {
        if (EquipmentView.equipments == null)
        {
            return;
        }
        else
        {
            outfitData.equipmentData = EquipmentView.equipments;
        }

        if (weaponData != outfitData.equipmentData[0].idItem0_1)
        {
            weaponData = outfitData.equipmentData[0].idItem0_1;
            EquipWeapon(weaponData);
        }
        if (helmetData != outfitData.equipmentData[1].idItem0_1)
        {
            helmetData = outfitData.equipmentData[1].idItem0_1;
            EquipHelmet(helmetData);
        }
        if (armorData != outfitData.equipmentData[2].idItem0_1)
        {
            armorData = outfitData.equipmentData[2].idItem0_1;
            EquipArmor(armorData);
        }
        if (legArmorData != outfitData.equipmentData[3].idItem0_1)
        {
            legArmorData = outfitData.equipmentData[3].idItem0_1;
            EquipLegArmor(legArmorData);
        }
        if (hairData != LogInView.GetHair())
        {
            hairData = LogInView.GetHair();
            EquipHair(hairData);
        }
    }
    public void OnLateUpdate()
    {
        UpdateSprite();
    }
    public void OnFixedUpdate() { }

    #region Getter lấy dữ liệu để gửi đến server
    public int GetLegArmorData()
    {
        return legArmorData;
    }
    public int GetArmorData()
    {
        return armorData;
    }
    public int GetHelmetData()
    {
        return helmetData;
    }
    public int GetHairData()
    {
        return hairData;
    }
    public int GetWeaponData()
    {
        return weaponData;
    }
    public int GetHeadData()
    {
        return headData;
    }
    public Direction GetCurrentDirection()
    {
        return currentDirection;
    }
    public List<SpriteLibrary> GetListSpriteLibrary()
    {
        return spriteLibraries;
    }
    public Dictionary<int, (Category, Label)> GetSpriteResolversInfos()
    {
        return spriteResolversInfos;
    }
    #endregion

    #region Sửa sprite library sau khi equip item
    private void EquipLegArmor(int id)
    {
        spriteLibraries[0].spriteLibraryAsset = listItem0.GetItem0(id).legArmor.legArmorLibrariesAsset;
    }
    private void EquipArmor(int id)
    {
        spriteLibraries[1].spriteLibraryAsset = listItem0.GetItem0(id).armor.armorLibrariesAsset;
    }
    private void EquipHelmet(int id)
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
    private void EquipHair(int id)
    {
        int idSchool = LogInView.GetIDSchool();

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
    private void EquipWeapon(int id)
    {
        spriteLibraries[6].spriteLibraryAsset = listItem0.GetItem0(id).weapon.weaponFrontLibraries;
        spriteLibraries[7].spriteLibraryAsset = listItem0.GetItem0(id).weapon.weaponBackLibraries;
    }
    #endregion

    private string GetDirection(float h, float v)
    {
        if (Mathf.Abs(h) == 0 && Mathf.Abs(v) == 0)
            return "Front";

        if (Mathf.Abs(v) > 0.01f)
            return v > 0 ? "Back" : "Front";

        return h > 0 ? "Right" : "Left";
    }

    private int GetFrameByTime(float t, float[] changeTimes)
    {
        for (int i = 0; i < changeTimes.Length - 1; i++)
        {
            if (t >= changeTimes[i] && t < changeTimes[i + 1])
                return i;
        }

        return changeTimes.Length - 1;
    }
    private void UpdateSprite()
    {
        AnimatorStateInfo state = animator.GetCurrentAnimatorStateInfo(0);
        int fullPathHash = state.fullPathHash;

        float t = Mathf.Repeat(state.normalizedTime, 1f);

        float h;
        float v;
        if (movementPlayerController.GetIsMovingToTarget())
        {
            h = movementPlayerController.GetMovement().x;
            v = movementPlayerController.GetMovement().y;
        }
        else
        {
            h = movementPlayerController.GetLastMovement().x;
            v = movementPlayerController.GetLastMovement().y;
        }

        string direction = GetDirection(h, v);
        switch (direction)
        {
            case "Front":
                currentDirection = Direction.Front; break;
              
            case "Back":
                currentDirection = Direction.Back; break;

            case "Left":
                currentDirection = Direction.Left; break;

            case "Right":
                currentDirection = Direction.Right; break;
        }

        // Stand
        if (fullPathHash == standStateHash)
        {
            int frame = GetFrameByTime(t, moveChangeTimesInStandClip);

            SetAllResolvers("Stand", $"Stand{direction}Frame{frame}");
            if (!isInjured)
            {
                faceResolver.SetCategoryAndLabel("Stand", $"Stand{direction}Frame{frame}");
                spriteResolversInfos[4] = (ConvertCategory("Stand"), ConvertLabel($"Stand{direction}Frame{frame}"));
            }
        }
        // Move
        if (fullPathHash == moveStateHash)
        {
            int frame = GetFrameByTime(t, moveChangeTimesInMoveClip);

            SetAllResolvers("Move", $"Move{direction}Frame{frame}");
            if (!isInjured)
            {
                faceResolver.SetCategoryAndLabel("Move", $"Move{direction}Frame{frame}");
                spriteResolversInfos[4] = (ConvertCategory("Move"), ConvertLabel($"Move{direction}Frame{frame}"));
            }
        }
        // Attack
        if (fullPathHash == atkStateHash)
        {
            int frame = GetFrameByTime(t, moveChangeTimesInAtkClip);

            SetAllResolvers("Atk", $"Atk{direction}Frame{frame}");
            if (!isInjured)
            {
                faceResolver.SetCategoryAndLabel("Atk", $"Atk{direction}Frame{frame}");
                spriteResolversInfos[4] = (ConvertCategory("Atk"), ConvertLabel($"Atk{direction}Frame{frame}"));
            }
        }
        // Die
        if (fullPathHash == dieStateHash)
        {
            SetAllResolvers("Die", $"DieFrame0");
            if (!isInjured)
            {
                faceResolver.SetCategoryAndLabel("Die", $"DieFrame0");
                spriteResolversInfos[4] = (ConvertCategory("Die"), ConvertLabel($"DieFrame0"));
            }    
        }
    }
    private IEnumerator InjuredCoroutine()
    {
        yield return new WaitForSeconds(0.2f);

        isInjured = false;
    }
    public void UpdateInjuredSprite()
    {
        float h;
        float v;
        if (movementPlayerController.GetIsMovingToTarget())
        {
            h = movementPlayerController.GetMovement().x;
            v = movementPlayerController.GetMovement().y;
        }
        else
        {
            h = movementPlayerController.GetLastMovement().x;
            v = movementPlayerController.GetLastMovement().y;
        }

        string direction = GetDirection(h, v);
        switch (direction)
        {
            case "Front":
                currentDirection = Direction.Front; break;

            case "Back":
                currentDirection = Direction.Back; break;

            case "Left":
                currentDirection = Direction.Left; break;

            case "Right":
                currentDirection = Direction.Right; break;
        }

        StopCoroutine(nameof(InjuredCoroutine));

        isInjured = true;
        faceResolver.SetCategoryAndLabel("Injured", $"Injured{direction}Frame1");
        spriteResolversInfos[4] = (ConvertCategory("Injured"), ConvertLabel($"Injured{direction}Frame1"));
        StartCoroutine(InjuredCoroutine());
    }

    private void SetAllResolvers(string category, string label)
    {
        if (category == lastCategory && label == lastLabel)
            return;

        lastCategory = category;
        lastLabel = label;

        Category enumCategory = ConvertCategory(category);
        Label enumLabel = ConvertLabel(label);

        for (int i = 0; i < spriteResolvers.Count; i++)
        {
            if (i == 4) continue;

            var r = spriteResolvers[i];

            if (r != null && r.spriteLibrary != null)
            {
                r.SetCategoryAndLabel(category, label);
                r.ResolveSpriteToSpriteRenderer();
                   
                spriteResolversInfos[i] = (enumCategory, enumLabel);
            }
        }
    }
}