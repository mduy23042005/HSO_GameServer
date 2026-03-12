using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.U2D.Animation;

public class StateData
{
    public PlayerState state;
    public Direction direction;
    public float stateStartTime;
}
public class PositionData
{
    public float posX;
    public float posY;
    public float lastPosX;
    public float lastPosY;
}
public class SyncSpriteController : MonoBehaviour, IUpdatable
{
    private ConcurrentQueue<PlayerData> stateDataQueue = new ConcurrentQueue<PlayerData>();
    private Animator animator;

    private string direction;
    private string currentState;
    private string lastCategory;
    private string lastLabel;

    private StateData stateData;
    private PositionData positionData;

    private float lastAnimStartTime = -1f;

    private List<SpriteResolver> resolvers;
    private SpriteResolver faceResolver;

    [Header("Chỉ định sprite nào của player sẽ bị thay thế")]
    [SerializeField] private List<SpriteLibrary> spriteLibrary;

    private ItemController listItem0;

    // id của trang bị thực tế từ database
    private int weaponData = 0;
    private int helmetData = 0;
    private int armorData = 0;
    private int legArmorData = 0;
    private int hairData = 0;

    private Dictionary<string, float> clipLengths = new()
{
    {"StandFront",0.4f},
    {"StandBack",0.4f},
    {"StandLeft",0.4f},
    {"StandRight",0.4f},

    {"InjuredFront",0.4f},
    {"InjuredBack",0.4f},
    {"InjuredLeft",0.4f},
    {"InjuredRight",0.4f},

    {"MoveFront",0.2f},
    {"MoveBack",0.2f},
    {"MoveLeft",0.2f},
    {"MoveRight",0.2f},

    {"AtkFront",0.15f},
    {"AtkBack",0.15f},
    {"AtkLeft",0.15f},
    {"AtkRight",0.15f}
};

    private void Awake()
    {
        resolvers = GetComponentsInChildren<SpriteResolver>().ToList();
        faceResolver = resolvers.FirstOrDefault(r => r.gameObject.name == "4_0_0");
        listItem0 = ItemController.Instance;
        animator = GetComponentInChildren<Animator>();

        stateData = new StateData();
        positionData = new PositionData();

        stateData.stateStartTime = 1f;
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

    public void ApplyServerPlayerData(PlayerData data)
    {
        if (stateData.stateStartTime == data.stateStartTime && data.state == PlayerState.Attack)
            return;

        stateDataQueue.Enqueue(data);
    }
    private PlayerData DequeuePlayerData()
    {
        if (stateDataQueue.TryDequeue(out var data))
            return data;

        return null;
    }

    private void ApplyData(PlayerData data)
    {
        if (weaponData != data.weapon)
        {
            weaponData = data.weapon;
            EquipWeapon(weaponData);
        }
        if (helmetData != data.helmet)
        {
            helmetData = data.helmet;
            EquipHelmet(helmetData);
        }
        if (armorData != data.armor)
        {
            armorData = data.armor;
            EquipArmor(armorData);
        }
        if (legArmorData != data.legArmor)
        {
            legArmorData = data.legArmor;
            EquipLegArmor(legArmorData);
        }
        if (hairData != data.hair)
        {
            hairData = data.hair;
            EquipHair(hairData, data.idSchool);
        }

        positionData.posX = data.posX;
        positionData.posY = data.posY;
        positionData.lastPosX = data.lastPosX;
        positionData.lastPosY = data.lastPosY;

        stateData.state = data.state;
        stateData.direction = data.direction;
        stateData.stateStartTime = data.stateStartTime;
    }
    public void OnUpdate()
    {
        PlayerData data = DequeuePlayerData();

        if (data != null)
        {
            ApplyData(data);
        }

        string stateName = UpdateStateString(stateData);

        float clipLength = clipLengths[stateName];
        float elapsed = Time.time - stateData.stateStartTime;

        float normalizedTime = (elapsed / clipLength) % 1f;

        UpdateAnimation(stateName, normalizedTime);
    }
    public void OnLateUpdate() 
    {
        UpdateSprite();
    }
    public void OnFixedUpdate()
    {
        Vector2 targetPos = new Vector2(positionData.posX, positionData.posY);
        transform.position = Vector2.MoveTowards(transform.position, targetPos, 6f * Time.fixedDeltaTime);
    }

    private string UpdateStateString(StateData data)
    {
        switch (stateData.direction)
        {
            case Direction.Front:
                direction = "Front"; break;

            case Direction.Back:
                direction = "Back"; break;

            case Direction.Left:
                direction = "Left"; break;

            case Direction.Right:
                direction = "Right"; break;
        }
        switch (stateData.state)
        {
            case PlayerState.Stand:
                currentState = $"Stand"; break;
            case PlayerState.Move:
                currentState = $"Move"; break;
            case PlayerState.Attack:
                currentState = $"Atk"; break;
            case PlayerState.Injured:
                currentState = $"Injured"; break;
            case PlayerState.Die:
                currentState = $"Die"; break;
        }

        return $"{currentState}{direction}";
    }
    private void UpdateAnimation(string currentState, float normalizedTime)
    {
        if (lastAnimStartTime == stateData.stateStartTime)
            return;

        var state = animator.GetCurrentAnimatorStateInfo(0);
        if (state.IsName($"Atk{direction}") && state.normalizedTime < 1f)
            return;

        animator.Play(currentState, 0, normalizedTime);

        lastAnimStartTime = stateData.stateStartTime;
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
            spriteLibrary[4].gameObject.SetActive(false);
        }
        else
        {
            spriteLibrary[4].gameObject.SetActive(true);
        }
    }
    public void EquipHair(int id, int idSchool)
    {
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

    private int GetFrameByTime(float t, float[] changeTimes)
    {
        t = Mathf.Repeat(t, 1f);

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

        // Stand
        if (state.IsName($"Stand{direction}"))
        {
            float[] moveChangeTimes = { 0.0f, 0.5f, 1f }; // Clip dài 0:40 giây, đổi frame ở 0 / 0.4, 0.2 / 0.4

            int frame = GetFrameByTime(t, moveChangeTimes);

            SetAllResolvers("Stand", $"Stand{direction}");
            faceResolver.SetCategoryAndLabel("Stand", $"Stand{direction}Frame{frame}");
            return;
        }
        // Move
        if (state.IsName($"Move{direction}"))
        {
            float[] moveChangeTimes = { 0.0f, 0.5f, 1f }; // Clip dài 0:20 giây, đổi frame ở 0 / 0.2, 0.1 / 0.2

            int frame = GetFrameByTime(t, moveChangeTimes);

            SetAllResolvers("Move", $"Move{direction}Frame{frame}");
            return;
        }
        // Attack
        if (state.IsName($"Atk{direction}"))
        {
            float[] moveChangeTimes = { 0.0f, 0.6667f, 1f }; // Clip dài 0:15 giây, đổi frame ở 0 / 0.15, 0.1 / 0.15

            int frame = GetFrameByTime(t, moveChangeTimes);

            SetAllResolvers("Atk", $"Atk{direction}Frame{frame}");
            return;
        }
        //Injured
        if (state.IsName($"Injured{direction}"))
        {
            float[] moveChangeTimes = { 0.0f, 0.5f, 1f }; // Clip dài 0:20 giây, đổi frame ở 0 / 0.2, 0.1 / 0.2

            int frame = GetFrameByTime(t, moveChangeTimes);

            faceResolver.SetCategoryAndLabel("Injured", $"Injured{direction}Frame{frame}");
            return;
        }
        // Die
        if (state.IsName($"Die"))
        {
            SetAllResolvers("Die", $"DieFrame0");
            return;
        }
    }
    private void SetAllResolvers(string category, string label)
    {
        if (category == lastCategory && label == lastLabel)
            return;

        lastCategory = category;
        lastLabel = label;

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