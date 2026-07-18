using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour, IUpdatable
{
    [SerializeField] private List<GameObject> playerPrefab;

    // Key: (idSchool, PlayerState, Direction, frame)
    public static Dictionary<(int, PlayerState, Direction, int), List<PartBodyData>> bodyDatas;

    private List<PartBodyData> partBodyDatas;
    private PositionData positionData = new PositionData();
    private RotationData rotationData = new RotationData();
    private ScaleData scaleData = new ScaleData();
    private ColorData colorData = new ColorData();

    private GameObject player;

    private void Awake()
    {

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

    public void OnUpdate() 
    {
        if (player == null && LogInView.GetIDAccount() != 0)
            InitPlayer();
    }
    public void OnLateUpdate() { }
    public void OnFixedUpdate() { }
    public void RegisterDontDestroyOnLoad()
    {
        GameManager.Instance.RegisterPersistent(this);
    }

    //Test cache part body
    private PositionData GetPartBodyPositionData(float x, float y, float z)
    {
        positionData.x = x;
        positionData.y = y;
        positionData.z = z;
        return positionData;
    }
    private RotationData GetPartBodyRotationData(float x, float y, float z)
    {
        rotationData.x = x;
        rotationData.y = y;
        rotationData.z = z;
        return rotationData;
    }
    private ScaleData GetPartBodyScaleData(float x, float y, float z)
    {
        scaleData.x = x;
        scaleData.y = y;
        scaleData.z = z;
        return scaleData;
    }
    private ColorData GetPartBodyColorData(float r, float g, float b, float a)
    {
        colorData.r = r;
        colorData.g = g;
        colorData.b = b;
        colorData.a = a;
        return colorData;
    }
    private void InitPartBodyData()
    {
        // Key: (idSchool, PlayerState, Direction, frame)
        bodyDatas = new Dictionary<(int, PlayerState, Direction, int), List<PartBodyData>>();

        PartBodyData partBodyData = new PartBodyData();
        partBodyData.category = Category.Stand;
        partBodyData.label = Label.StandFront;
        partBodyData.positionData = GetPartBodyPositionData(0f, 0.27f, 0f);
        partBodyData.rotationData = GetPartBodyRotationData(0f, 0f, 0f);
        partBodyData.scaleData = GetPartBodyScaleData(1f, 1f, 1f);
        partBodyData.colorData = GetPartBodyColorData(1f, 1f, 1f, 1f);

        bodyDatas.Add((1, PlayerState.Stand, Direction.Front, 0), partBodyDatas);
    }
    public void InitPlayer()
    {
        int idSchool = LogInView.GetIDSchool();

        switch (idSchool)
        {
            case 1:
                player = Instantiate(playerPrefab[0], new Vector2(-9.5f, -4.5f), Quaternion.identity);
                break;
            case 2:
                player = Instantiate(playerPrefab[1], new Vector2(-9.5f, -4.5f), Quaternion.identity);
                break;
            case 3:
                player = Instantiate(playerPrefab[2], new Vector2(-9.5f, -4.5f), Quaternion.identity);
                break;
            case 4:
                player = Instantiate(playerPrefab[3], new Vector2(-9.5f, -4.5f), Quaternion.identity);
                break;
        }
    }

    public void DestroyPlayer()
    {
        if (player != null)
        {
            Destroy(player);
            player = null;
            LogInView.SetIDAccount(0);
        }
    }
}