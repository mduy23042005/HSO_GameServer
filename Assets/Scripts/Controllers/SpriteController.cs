using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.U2D.Animation;

public class SpriteController : MonoBehaviour, IUpdatable
{
    private List<SpriteResolver> resolvers;
    private Animator animator;
    private int lastFrame = -1;
    private string lastState = "";
    private MovementController movementController;
    private Direction currentDirection;
    private int currentFrame;

    [Header("Chỉ định sprite nào của player sẽ bị thay thế")]
    [SerializeField] private List<SpriteLibrary> spriteLibrary;

    private ItemController listItem0;

    private SocketManager socketManager;
    private PacketSerializeManager packetSerializeManager;

    // id của trang bị thực tế từ database
    private int weaponData = 0;
    private int helmetData = 0;
    private int armorData = 0;
    private int legArmorData = 0;
    private int hairData = 0;
    private int headData = 0;

    private void Awake()
    {
        resolvers = GetComponentsInChildren<SpriteResolver>().ToList();
        animator = GetComponent<Animator>();
        movementController = GetComponent<MovementController>();
        socketManager = GameManager.Instance.GetComponent<SocketManager>();
        packetSerializeManager = GameManager.Instance.GetComponent<PacketSerializeManager>();
        listItem0 = ItemController.Instance;
        ReadDatabase();
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
        List<EquipmentResultPacket> listOutfitSprites;

        if (EquipmentView.GetListEquipmentSlots().Count == 0)
        {
            string data = socketManager.GetOutfitSpritesData();

            if (string.IsNullOrEmpty(data))
                return;

            listOutfitSprites = packetSerializeManager.HandleReceivedPacket<List<EquipmentResultPacket>>(data);
        }
        else
        {
            listOutfitSprites = EquipmentView.GetListEquipmentSlots();
        }

        weaponData = listOutfitSprites[0].idItem0_1;
        helmetData = listOutfitSprites[1].idItem0_1;
        armorData = listOutfitSprites[2].idItem0_1;
        legArmorData = listOutfitSprites[3].idItem0_1;
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
    public int GetCurrentFrame()
    {
        return currentFrame;
    }
    #endregion

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
        int idSchool = LogInView.GetIDSchool();

        switch (idSchool)
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

    private async void ReadDatabase()
    {
        int idAccount = LogInView.GetIDAccount() ?? 0;
        EquipmentRequestPacket sendOutfitSpritesRequestPacket = new EquipmentRequestPacket
        {
            cmd = "outfitSprites",
            idAccount = idAccount,
        };

        packetSerializeManager.HandleSentPacket(sendOutfitSpritesRequestPacket);
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
        t %= 1f;

        for (int i = 0; i < changeTimes.Length; i++)
        {
            if (t < changeTimes[i])
                return Mathf.Max(0, i - 1);
        }

        return changeTimes.Length - 1;
    }
    private void UpdateSprite()
    {
        if (animator == null) return;

        for (int i = 0; i < resolvers.Count; i++)
        {
            if (resolvers[i] == null)
                continue;
        }

        AnimatorStateInfo state = animator.GetCurrentAnimatorStateInfo(0);

        float h;
        float v;
        if (movementController.GetIsMovingToTarget())
        {
            h = movementController.GetMovement().x;
            v = movementController.GetMovement().y;
        }
        else
        {
            h = movementController.GetLastMovement().x;
            v = movementController.GetLastMovement().y;
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
        if (state.IsName("Stand"))
        {
            float t = state.normalizedTime % 1f;

            float[] moveChangeTimes = { 0.0f, 0.5f }; // Clip dài 0:40 giây, đổi frame ở 0 / 0.4, 0.2 / 0.4

            int frame = GetFrameByTime(t, moveChangeTimes);

            if (lastState != "Stand" + direction)
            {
                lastFrame = -1;
                lastState = "Stand" + direction;
                SetAllResolvers("Stand", $"Stand{direction}");
            }
            foreach (var r in resolvers)
            {
                if (r != null && r.spriteLibrary != null && r.gameObject.name == "4_0_0")
                {
                    r.SetCategoryAndLabel("Stand", $"Stand{direction}Frame{frame}");
                    currentFrame = frame;
                    r.ResolveSpriteToSpriteRenderer();
                }
            }
        }
        // Move
        if (state.IsName("Move"))
        {
            float t = state.normalizedTime % 1f;

            float[] moveChangeTimes = { 0.0f, 0.5f }; // Clip dài 0:40 giây, đổi frame ở 0 / 0.4, 0.2 / 0.4

            int frame = GetFrameByTime(t, moveChangeTimes);

            if (frame != lastFrame || lastState != "Move" + direction)
            {
                lastFrame = frame;
                lastState = "Move" + direction;
                SetAllResolvers("Move", $"Move{direction}Frame{frame}");
                currentFrame = frame;
            }
        }
        // Attack
        if (state.IsName("Atk"))
        {
            float t = state.normalizedTime % 1f;

            float[] moveChangeTimes = { 0.0f, 0.6667f }; // Clip dài 0:15 giây, đổi frame ở 0 / 0.15, 0.1 / 0.15

            int frame = GetFrameByTime(t, moveChangeTimes);

            if (frame != lastFrame || lastState != "Atk" + direction)
            {
                lastFrame = frame;
                lastState = "Atk" + direction;
                SetAllResolvers("Atk", $"Atk{direction}Frame{frame}");
                currentFrame = frame;
            }
        }
        //Injured
        if (state.IsName("Injured"))
        {
            float t = state.normalizedTime % 1f;

            float[] moveChangeTimes = { 0.0f, 0.5f }; // Clip dài 0:20 giây, đổi frame ở 0 / 0.2, 0.1 / 0.2

            int frame = GetFrameByTime(t, moveChangeTimes);

            if (frame != lastFrame || lastState != "Injured" + direction)
            {
                lastFrame = frame;
                lastState = "Injured" + direction;
                foreach (var r in resolvers)
                {
                    if (r != null && r.spriteLibrary != null && r.gameObject.name == "4_0_0")
                    {
                        r.SetCategoryAndLabel("Injured", $"Injured{direction}Frame{frame}");
                        currentFrame = frame;
                        r.ResolveSpriteToSpriteRenderer();
                    }
                }
            }
        }
        // Die
        if (state.IsName("Die"))
        {
            if (lastState != "Die")
            {
                lastFrame = -1;
                lastState = "Die";
                SetAllResolvers("Die", $"DieFrame0");
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