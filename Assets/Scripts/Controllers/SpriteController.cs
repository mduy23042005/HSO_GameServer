using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.U2D.Animation;

public class SpriteController : MonoBehaviour, IUpdatable
{
    private SpriteResolver faceResolver;
    private Animator animator;
    private string lastCategory;
    private string lastLabel;
    private MovementPlayerController movementPlayerController;
    private Direction currentDirection;

    [Header("Chỉ định sprite nào của player sẽ bị thay thế")]
    [SerializeField] private List<SpriteLibrary> spriteLibraries;
    [SerializeField] private List<SpriteResolver> spriteResolvers;

    private ItemController listItem0;

    private SocketManager socketManager;

    // id của trang bị thực tế từ database
    private int weaponData = 0;
    private int helmetData = 0;
    private int armorData = 0;
    private int legArmorData = 0;
    private int hairData = 0;
    private int headData = 0;

    private void Awake()
    {
        faceResolver = spriteResolvers.FirstOrDefault(r => r.gameObject.name == "4_0_0");
        animator = GetComponent<Animator>();
        movementPlayerController = GetComponent<MovementPlayerController>();
        socketManager = GameManager.Instance.GetComponent<SocketManager>();
        listItem0 = ItemController.Instance;
        ReadCache();
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
        EquipmentResultPacket outfitData = new EquipmentResultPacket();

        if (EquipmentView.GetListEquipmentSlots().Count == 0)
        {
            byte[] data = socketManager.GetOutfitSpritesData();

            if (data == null || data.Length == 0)
                return;

            PacketReaderManager reader = new PacketReaderManager(data);

            outfitData.cmd = (EnumCmdCode)reader.ReadInt();
            outfitData.equipmentData = new List<EquipmentData>();

            int countOutfitSprite = reader.ReadInt();
            for (int i = 0; i < countOutfitSprite; i++)
            {
                outfitData.equipmentData.Add(new EquipmentData
                {
                    id = reader.ReadInt(),
                    idItem0_1 = reader.ReadInt(),
                    nameItem0_1 = reader.ReadString(),
                    category = reader.ReadInt(),
                    slotName = reader.ReadString()
                });
            }
        }
        else
        {
            outfitData.equipmentData = EquipmentView.GetListEquipmentSlots();
        }

        weaponData = outfitData.equipmentData[0].idItem0_1;
        helmetData = outfitData.equipmentData[1].idItem0_1;
        armorData = outfitData.equipmentData[2].idItem0_1;
        legArmorData = outfitData.equipmentData[3].idItem0_1;
        hairData = LogInView.GetHair();

        EquipLegArmor(legArmorData);
        EquipArmor(armorData);
        EquipHelmet(helmetData);
        EquipWeapon(weaponData);
        EquipHair(hairData);
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

    private async void ReadCache()
    {
        int idAccount = LogInView.GetIDAccount() ?? 0;
        EquipmentRequestPacket outfitSpritesRequestPacket = new EquipmentRequestPacket
        {
            cmd = EnumCmdCode.outfitSprites,
            idAccount = idAccount,
        };

        PacketWriterManager writer = new PacketWriterManager();
        writer.WriteInt((int)outfitSpritesRequestPacket.cmd);
        writer.WriteInt(outfitSpritesRequestPacket.idAccount);

        await socketManager.SendToServer(writer.ToArray());
    }

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
        if (state.IsName($"Stand"))
        {
            float[] moveChangeTimes = { 0f, 0.5f, 1f }; // Clip dài 0:40 giây, đổi frame ở 0 / 0.4, 0.2 / 0.4

            int frame = GetFrameByTime(t, moveChangeTimes);

            SetAllResolvers("Stand", $"Stand{direction}");
            faceResolver.SetCategoryAndLabel("Stand", $"Stand{direction}Frame{frame}");
        }
        // Move
        if (state.IsName("Move"))
        {
            float[] moveChangeTimes = { 0f, 0.5f, 1f }; // Clip dài 0:40 giây, đổi frame ở 0 / 0.2, 0.1 / 0.2

            int frame = GetFrameByTime(t, moveChangeTimes);

            SetAllResolvers("Move", $"Move{direction}Frame{frame}");
        }
        // Attack
        if (state.IsName("Atk"))
        {
            float[] moveChangeTimes = { 0f, 0.6667f, 1f }; // Clip dài 0:15 giây, đổi frame ở 0 / 0.15, 0.1 / 0.15

            int frame = GetFrameByTime(t, moveChangeTimes);

            SetAllResolvers("Atk", $"Atk{direction}Frame{frame}");
        }
        //Injured
        if (state.IsName("Injured"))
        {
            float[] moveChangeTimes = { 0f, 0.5f, 1f }; // Clip dài 0:20 giây, đổi frame ở 0 / 0.2, 0.1 / 0.2

            int frame = GetFrameByTime(t, moveChangeTimes);

            SetAllResolvers("Stand", $"Stand{direction}");
            faceResolver.SetCategoryAndLabel("Injured", $"Injured{direction}Frame{frame}");
        }
        // Die
        if (state.IsName("Die"))
        {
           SetAllResolvers("Die", $"DieFrame0");   
        }
    }

    private void SetAllResolvers(string category, string label)
    {
        if (category == lastCategory && label == lastLabel)
            return;

        lastCategory = category;
        lastLabel = label;

        foreach (var r in spriteResolvers)
        {
            if (r != null && r.spriteLibrary != null)
            {
                r.SetCategoryAndLabel(category, label);
                r.ResolveSpriteToSpriteRenderer();
            }
        }
    }

    public List<SpriteLibrary> GetListSpriteLibraries()
    {
        return spriteLibraries;
    }
}