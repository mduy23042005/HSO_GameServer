using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour, IUpdatable
{
    [SerializeField] private List<GameObject> playerPrefab;

    // Key: (idSchool, idPartBody, Category, Label)
    public static Dictionary<(int, int, Category, Label), (PositionData, RotationData, ScaleData, ColorData)> bodyDatas;

    private SocketManager socketManager;
    public static GameObject player;

    private void Awake()
    {
        socketManager = GameManager.Instance.GetComponent<SocketManager>();
        InitPartBodyData();
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
        PositionData positionData = new PositionData();
        positionData.x = x;
        positionData.y = y;
        positionData.z = z;
        return positionData;
    }
    private RotationData GetPartBodyRotationData(float x, float y, float z)
    {
        RotationData rotationData = new RotationData();
        rotationData.x = x;
        rotationData.y = y;
        rotationData.z = z;
        return rotationData;
    }
    private ScaleData GetPartBodyScaleData(float x, float y, float z)
    {
        ScaleData scaleData = new ScaleData();
        scaleData.x = x;
        scaleData.y = y;
        scaleData.z = z;
        return scaleData;
    }
    private ColorData GetPartBodyColorData(float r, float g, float b, float a)
    {
        ColorData colorData = new ColorData();
        colorData.r = r;
        colorData.g = g;
        colorData.b = b;
        colorData.a = a;
        return colorData;
    }

    private void InitPlayer()
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

    private void InitPartBodyData()
    {
        // Key: (idSchool, idPartBody, frame, Category, Label)
        bodyDatas = new Dictionary<(int, int, Category, Label), (PositionData, RotationData, ScaleData, ColorData)>();
        #region ChienBinh
        #region idSchool: 1, idPartBody: 0, Category: Stand, Label: StandFront
        bodyDatas.Add((1, 0, Category.Stand, Label.StandFrontFrame0),
            (GetPartBodyPositionData(0f, 0.27f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        bodyDatas.Add((1, 0, Category.Stand, Label.StandFrontFrame1), 
            (GetPartBodyPositionData(0f, 0.27f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        #endregion
        #region idSchool: 1, idPartBody: 1, Category: Stand, Label: StandFront
        bodyDatas.Add((1, 1, Category.Stand, Label.StandFrontFrame0), 
            (GetPartBodyPositionData(0.00974f, 0.689f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        bodyDatas.Add((1, 1, Category.Stand, Label.StandFrontFrame1), 
            (GetPartBodyPositionData(0.00974f, 0.719f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        #endregion
        #region idSchool: 1, idPartBody: 2, Category: Stand, Label: StandFront
        bodyDatas.Add((1, 2, Category.Stand, Label.StandFrontFrame0), 
            (GetPartBodyPositionData(0.01774f, 1.494f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        bodyDatas.Add((1, 2, Category.Stand, Label.StandFrontFrame1), 
            (GetPartBodyPositionData(0.01774f, 1.524f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        #endregion
        #region idSchool: 1, idPartBody: 3, Category: Stand, Label: StandFront
        bodyDatas.Add((1, 3, Category.Stand, Label.StandFrontFrame0), 
            (GetPartBodyPositionData(0.01974f, 1.832f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        bodyDatas.Add((1, 3, Category.Stand, Label.StandFrontFrame1), 
            (GetPartBodyPositionData(0.01974f, 1.862f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        #endregion
        #region idSchool: 1, idPartBody: 4, Category: Stand, Label: StandFront
        bodyDatas.Add((1, 4, Category.Stand, Label.StandFrontFrame0), 
            (GetPartBodyPositionData(0.01874f, 1.355f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        bodyDatas.Add((1, 4, Category.Stand, Label.StandFrontFrame1), 
            (GetPartBodyPositionData(0.01874f, 1.385f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        #endregion
        #region idSchool: 1, idPartBody: 5, Category: Stand, Label: StandFront
        bodyDatas.Add((1, 5, Category.Stand, Label.StandFrontFrame0), 
            (GetPartBodyPositionData(0.00174f, 1.8002f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        bodyDatas.Add((1, 5, Category.Stand, Label.StandFrontFrame1), 
            (GetPartBodyPositionData(0.00174f, 1.8302f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        #endregion
        #region idSchool: 1, idPartBody: 6, Category: Stand, Label: StandFront
        bodyDatas.Add((1, 6, Category.Stand, Label.StandFrontFrame0), 
            (GetPartBodyPositionData(-0.10379f, 0.879f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        bodyDatas.Add((1, 6, Category.Stand, Label.StandFrontFrame1), 
            (GetPartBodyPositionData(-0.10379f, 0.909f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        #endregion
        #region idSchool: 1, idPartBody: 7, Category: Stand, Label: StandFront
        bodyDatas.Add((1, 7, Category.Stand, Label.StandFrontFrame0), 
            (GetPartBodyPositionData(0f, 0f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 0f)));
        bodyDatas.Add((1, 7, Category.Stand, Label.StandFrontFrame1), 
            (GetPartBodyPositionData(0f, 0f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 0f)));
        #endregion
        #region idSchool: 1, idPartBody: 8, Category: Stand, Label: StandFront
        bodyDatas.Add((1, 8, Category.Stand, Label.StandFrontFrame0), 
            (GetPartBodyPositionData(0f, 0f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 0f)));
        bodyDatas.Add((1, 8, Category.Stand, Label.StandFrontFrame1), 
            (GetPartBodyPositionData(0f, 0f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 0f)));
        #endregion

        #region idSchool: 1, idPartBody: 0, Category: Stand, Label: StandBack
        bodyDatas.Add((1, 0, Category.Stand, Label.StandBackFrame0), 
            (GetPartBodyPositionData(0f, 0.27f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        bodyDatas.Add((1, 0, Category.Stand, Label.StandBackFrame1), 
            (GetPartBodyPositionData(0f, 0.27f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        #endregion
        #region idSchool: 1, idPartBody: 1, Category: Stand, Label: StandBack
        bodyDatas.Add((1, 1, Category.Stand, Label.StandBackFrame0), 
            (GetPartBodyPositionData(0.00974f, 0.734f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        bodyDatas.Add((1, 1, Category.Stand, Label.StandBackFrame1), 
            (GetPartBodyPositionData(0.00974f, 0.704f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        #endregion
        #region idSchool: 1, idPartBody: 2, Category: Stand, Label: StandBack
        bodyDatas.Add((1, 2, Category.Stand, Label.StandBackFrame0), 
            (GetPartBodyPositionData(0.01774f, 1.544f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        bodyDatas.Add((1, 2, Category.Stand, Label.StandBackFrame1), 
            (GetPartBodyPositionData(0.01774f, 1.514f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        #endregion
        #region idSchool: 1, idPartBody: 3, Category: Stand, Label: StandBack
        bodyDatas.Add((1, 3, Category.Stand, Label.StandBackFrame0), 
            (GetPartBodyPositionData(0.01974f, 1.514f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        bodyDatas.Add((1, 3, Category.Stand, Label.StandBackFrame1), 
            (GetPartBodyPositionData(0.01974f, 1.484f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        #endregion
        #region idSchool: 1, idPartBody: 4, Category: Stand, Label: StandBack
        bodyDatas.Add((1, 4, Category.Stand, Label.StandBackFrame0), 
            (GetPartBodyPositionData(0f, 0f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 0f)));
        bodyDatas.Add((1, 4, Category.Stand, Label.StandBackFrame1), 
            (GetPartBodyPositionData(0f, 0f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 0f)));
        #endregion
        #region idSchool: 1, idPartBody: 5, Category: Stand, Label: StandBack
        bodyDatas.Add((1, 5, Category.Stand, Label.StandBackFrame0),
            (GetPartBodyPositionData(0.00174f, 1.8502f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        bodyDatas.Add((1, 5, Category.Stand, Label.StandBackFrame1),
            (GetPartBodyPositionData(0.00174f, 1.8202f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        #endregion
        #region idSchool: 1, idPartBody: 6, Category: Stand, Label: StandBack
        bodyDatas.Add((1, 6, Category.Stand, Label.StandBackFrame0),
            (GetPartBodyPositionData(0f, 0f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 0f)));
        bodyDatas.Add((1, 6, Category.Stand, Label.StandBackFrame1),
            (GetPartBodyPositionData(0f, 0f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 0f)));
        #endregion
        #region idSchool: 1, idPartBody: 7, Category: Stand, Label: StandBack
        bodyDatas.Add((1, 7, Category.Stand, Label.StandBackFrame0),
            (GetPartBodyPositionData(0.11921f, 0.864f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        bodyDatas.Add((1, 7, Category.Stand, Label.StandBackFrame1),
            (GetPartBodyPositionData(0.11921f, 0.834f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        #endregion
        #region idSchool: 1, idPartBody: 8, Category: Stand, Label: StandBack
        bodyDatas.Add((1, 8, Category.Stand, Label.StandBackFrame0),
            (GetPartBodyPositionData(0f, 0f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 0f)));
        bodyDatas.Add((1, 8, Category.Stand, Label.StandBackFrame1),
            (GetPartBodyPositionData(0f, 0f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 0f)));
        #endregion

        #region idSchool: 1, idPartBody: 0, Category: Stand, Label: StandLeft
        bodyDatas.Add((1, 0, Category.Stand, Label.StandLeftFrame0),
            (GetPartBodyPositionData(0f, 0.274f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        bodyDatas.Add((1, 0, Category.Stand, Label.StandLeftFrame1),
            (GetPartBodyPositionData(0f, 0.274f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        #endregion
        #region idSchool: 1, idPartBody: 1, Category: Stand, Label: StandLeft
        bodyDatas.Add((1, 1, Category.Stand, Label.StandLeftFrame0),
            (GetPartBodyPositionData(0.00974f, 0.714f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        bodyDatas.Add((1, 1, Category.Stand, Label.StandLeftFrame1),
            (GetPartBodyPositionData(0.00974f, 0.684f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        #endregion
        #region idSchool: 1, idPartBody: 2, Category: Stand, Label: StandLeft
        bodyDatas.Add((1, 2, Category.Stand, Label.StandLeftFrame0),
            (GetPartBodyPositionData(0.01774f, 1.544f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        bodyDatas.Add((1, 2, Category.Stand, Label.StandLeftFrame1),
            (GetPartBodyPositionData(0.01774f, 1.514f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        #endregion
        #region idSchool: 1, idPartBody: 3, Category: Stand, Label: StandLeft
        bodyDatas.Add((1, 3, Category.Stand, Label.StandLeftFrame0),
            (GetPartBodyPositionData(0.01974f, 1.942f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        bodyDatas.Add((1, 3, Category.Stand, Label.StandLeftFrame1),
            (GetPartBodyPositionData(0.01974f, 1.912f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        #endregion
        #region idSchool: 1, idPartBody: 4, Category: Stand, Label: StandLeft
        bodyDatas.Add((1, 4, Category.Stand, Label.StandLeftFrame0),
            (GetPartBodyPositionData(-0.16879f, 1.425f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        bodyDatas.Add((1, 4, Category.Stand, Label.StandLeftFrame1),
            (GetPartBodyPositionData(-0.16879f, 1.395f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        #endregion
        #region idSchool: 1, idPartBody: 5, Category: Stand, Label: StandLeft
        bodyDatas.Add((1, 5, Category.Stand, Label.StandLeftFrame0),
            (GetPartBodyPositionData(0.00174f, 1.8502f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        bodyDatas.Add((1, 5, Category.Stand, Label.StandLeftFrame1),
            (GetPartBodyPositionData(0.00174f, 1.8202f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        #endregion
        #region idSchool: 1, idPartBody: 6, Category: Stand, Label: StandLeft
        bodyDatas.Add((1, 6, Category.Stand, Label.StandLeftFrame0),
            (GetPartBodyPositionData(-0.10379f, 0.979f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        bodyDatas.Add((1, 6, Category.Stand, Label.StandLeftFrame1),
            (GetPartBodyPositionData(-0.10379f, 0.949f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        #endregion
        #region idSchool: 1, idPartBody: 7, Category: Stand, Label: StandLeft
        bodyDatas.Add((1, 7, Category.Stand, Label.StandLeftFrame0),
            (GetPartBodyPositionData(0f, 0f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 0f)));
        bodyDatas.Add((1, 7, Category.Stand, Label.StandLeftFrame1),
            (GetPartBodyPositionData(0f, 0f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 0f)));
        #endregion
        #region idSchool: 1, idPartBody: 8, Category: Stand, Label: StandLeft
        bodyDatas.Add((1, 8, Category.Stand, Label.StandLeftFrame0),
            (GetPartBodyPositionData(0f, 0f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 0f)));
        bodyDatas.Add((1, 8, Category.Stand, Label.StandLeftFrame1),
            (GetPartBodyPositionData(0f, 0f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 0f)));
        #endregion

        #region idSchool: 1, idPartBody: 0, Category: Stand, Label: StandRight
        bodyDatas.Add((1, 0, Category.Stand, Label.StandRightFrame0),
            (GetPartBodyPositionData(0f, 0.274f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        bodyDatas.Add((1, 0, Category.Stand, Label.StandRightFrame1),
            (GetPartBodyPositionData(0f, 0.274f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        #endregion
        #region idSchool: 1, idPartBody: 1, Category: Stand, Label: StandRight
        bodyDatas.Add((1, 1, Category.Stand, Label.StandRightFrame0),
            (GetPartBodyPositionData(0.00974f, 0.714f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        bodyDatas.Add((1, 1, Category.Stand, Label.StandRightFrame1),
            (GetPartBodyPositionData(0.00974f, 0.684f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        #endregion
        #region idSchool: 1, idPartBody: 2, Category: Stand, Label: StandRight
        bodyDatas.Add((1, 2, Category.Stand, Label.StandRightFrame0),
            (GetPartBodyPositionData(0.01774f, 1.544f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        bodyDatas.Add((1, 2, Category.Stand, Label.StandRightFrame1),
            (GetPartBodyPositionData(0.01774f, 1.514f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        #endregion
        #region idSchool: 1, idPartBody: 3, Category: Stand, Label: StandRight
        bodyDatas.Add((1, 3, Category.Stand, Label.StandRightFrame0),
            (GetPartBodyPositionData(0.01974f, 1.942f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        bodyDatas.Add((1, 3, Category.Stand, Label.StandRightFrame1),
            (GetPartBodyPositionData(0.01974f, 1.912f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        #endregion
        #region idSchool: 1, idPartBody: 4, Category: Stand, Label: StandRight
        bodyDatas.Add((1, 4, Category.Stand, Label.StandRightFrame0),
            (GetPartBodyPositionData(-0.16879f, 1.425f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        bodyDatas.Add((1, 4, Category.Stand, Label.StandRightFrame1),
            (GetPartBodyPositionData(-0.16879f, 1.395f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        #endregion
        #region idSchool: 1, idPartBody: 5, Category: Stand, Label: StandRight
        bodyDatas.Add((1, 5, Category.Stand, Label.StandRightFrame0),
            (GetPartBodyPositionData(0.00174f, 1.8502f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        bodyDatas.Add((1, 5, Category.Stand, Label.StandRightFrame1),
            (GetPartBodyPositionData(0.00174f, 1.8202f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        #endregion
        #region idSchool: 1, idPartBody: 6, Category: Stand, Label: StandRight
        bodyDatas.Add((1, 6, Category.Stand, Label.StandRightFrame0),
            (GetPartBodyPositionData(-0.10379f, 0.979f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        bodyDatas.Add((1, 6, Category.Stand, Label.StandRightFrame1),
            (GetPartBodyPositionData(-0.10379f, 0.949f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        #endregion
        #region idSchool: 1, idPartBody: 7, Category: Stand, Label: StandRight
        bodyDatas.Add((1, 7, Category.Stand, Label.StandRightFrame0),
            (GetPartBodyPositionData(0f, 0f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 0f)));
        bodyDatas.Add((1, 7, Category.Stand, Label.StandRightFrame1),
            (GetPartBodyPositionData(0f, 0f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 0f)));
        #endregion
        #region idSchool: 1, idPartBody: 8, Category: Stand, Label: StandRight
        bodyDatas.Add((1, 8, Category.Stand, Label.StandRightFrame0),
            (GetPartBodyPositionData(0f, 0f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 0f)));
        bodyDatas.Add((1, 8, Category.Stand, Label.StandRightFrame1),
            (GetPartBodyPositionData(0f, 0f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 0f)));
        #endregion

        #region idSchool: 1, idPartBody: 0, Category: Move, Label: MoveFront
        bodyDatas.Add((1, 0, Category.Move, Label.MoveFrontFrame0),
            (GetPartBodyPositionData(0f, 0.27f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        bodyDatas.Add((1, 0, Category.Move, Label.MoveFrontFrame1),
            (GetPartBodyPositionData(0f, 0.27f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        #endregion
        #region idSchool: 1, idPartBody: 1, Category: Move, Label: MoveFront
        bodyDatas.Add((1, 1, Category.Move, Label.MoveFrontFrame0),
            (GetPartBodyPositionData(-0.01379f, 0.739f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        bodyDatas.Add((1, 1, Category.Move, Label.MoveFrontFrame1),
            (GetPartBodyPositionData(-0.01379f, 0.739f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        #endregion
        #region idSchool: 1, idPartBody: 2, Category: Move, Label: MoveFront
        bodyDatas.Add((1, 2, Category.Move, Label.MoveFrontFrame0),
            (GetPartBodyPositionData(0.01774f, 1.544f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        bodyDatas.Add((1, 2, Category.Move, Label.MoveFrontFrame1),
            (GetPartBodyPositionData(0.01774f, 1.544f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        #endregion
        #region idSchool: 1, idPartBody: 3, Category: Move, Label: MoveFront
        bodyDatas.Add((1, 3, Category.Move, Label.MoveFrontFrame0),
            (GetPartBodyPositionData(0.01974f, 1.883f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        bodyDatas.Add((1, 3, Category.Move, Label.MoveFrontFrame1),
            (GetPartBodyPositionData(0.01974f, 1.883f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        #endregion
        #region idSchool: 1, idPartBody: 4, Category: Move, Label: MoveFront
        bodyDatas.Add((1, 4, Category.Move, Label.MoveFrontFrame0),
            (GetPartBodyPositionData(0.01874f, 1.3972f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        bodyDatas.Add((1, 4, Category.Move, Label.MoveFrontFrame1),
            (GetPartBodyPositionData(0.01874f, 1.3972f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        #endregion
        #region idSchool: 1, idPartBody: 5, Category: Move, Label: MoveFront
        bodyDatas.Add((1, 5, Category.Move, Label.MoveFrontFrame0),
            (GetPartBodyPositionData(0.00174f, 1.8502f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        bodyDatas.Add((1, 5, Category.Move, Label.MoveFrontFrame1),
            (GetPartBodyPositionData(0.00174f, 1.8502f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        #endregion
        #region idSchool: 1, idPartBody: 6, Category: Move, Label: MoveFront
        bodyDatas.Add((1, 6, Category.Move, Label.MoveFrontFrame0),
            (GetPartBodyPositionData(-0.10379f, 0.929f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        bodyDatas.Add((1, 6, Category.Move, Label.MoveFrontFrame1),
            (GetPartBodyPositionData(-0.10379f, 0.929f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        #endregion
        #region idSchool: 1, idPartBody: 7, Category: Move, Label: MoveFront
        bodyDatas.Add((1, 7, Category.Move, Label.MoveFrontFrame0),
            (GetPartBodyPositionData(0f, 0f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 0f)));
        bodyDatas.Add((1, 7, Category.Move, Label.MoveFrontFrame1),
            (GetPartBodyPositionData(0f, 0f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 0f)));
        #endregion
        #region idSchool: 1, idPartBody: 8, Category: Move, Label: MoveFront
        bodyDatas.Add((1, 8, Category.Move, Label.MoveFrontFrame0),
            (GetPartBodyPositionData(0f, 0f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 0f)));
        bodyDatas.Add((1, 8, Category.Move, Label.MoveFrontFrame1),
            (GetPartBodyPositionData(0f, 0f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 0f)));
        #endregion

        #region idSchool: 1, idPartBody: 0, Category: Move, Label: MoveBack
        bodyDatas.Add((1, 0, Category.Move, Label.MoveBackFrame0),
            (GetPartBodyPositionData(0f, 0.27f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        bodyDatas.Add((1, 0, Category.Move, Label.MoveBackFrame1),
            (GetPartBodyPositionData(0f, 0.27f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        #endregion
        #region idSchool: 1, idPartBody: 1, Category: Move, Label: MoveBack
        bodyDatas.Add((1, 1, Category.Move, Label.MoveBackFrame0),
            (GetPartBodyPositionData(0.05021f, 0.699f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        bodyDatas.Add((1, 1, Category.Move, Label.MoveBackFrame1),
            (GetPartBodyPositionData(0.05021f, 0.699f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        #endregion
        #region idSchool: 1, idPartBody: 2, Category: Move, Label: MoveBack
        bodyDatas.Add((1, 2, Category.Move, Label.MoveBackFrame0),
            (GetPartBodyPositionData(0.01774f, 1.544f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        bodyDatas.Add((1, 2, Category.Move, Label.MoveBackFrame1),
            (GetPartBodyPositionData(0.01774f, 1.544f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        #endregion
        #region idSchool: 1, idPartBody: 3, Category: Move, Label: MoveBack
        bodyDatas.Add((1, 3, Category.Move, Label.MoveBackFrame0),
            (GetPartBodyPositionData(0.01974f, 1.514f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        bodyDatas.Add((1, 3, Category.Move, Label.MoveBackFrame1),
            (GetPartBodyPositionData(0.01974f, 1.514f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        #endregion
        #region idSchool: 1, idPartBody: 4, Category: Move, Label: MoveBack
        bodyDatas.Add((1, 4, Category.Move, Label.MoveBackFrame0),
            (GetPartBodyPositionData(0f, 0f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 0f)));
        bodyDatas.Add((1, 4, Category.Move, Label.MoveBackFrame1),
            (GetPartBodyPositionData(0f, 0f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 0f)));
        #endregion
        #region idSchool: 1, idPartBody: 5, Category: Move, Label: MoveBack
        bodyDatas.Add((1, 5, Category.Move, Label.MoveBackFrame0),
            (GetPartBodyPositionData(0.00174f, 1.8502f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        bodyDatas.Add((1, 5, Category.Move, Label.MoveBackFrame1),
            (GetPartBodyPositionData(0.00174f, 1.8202f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        #endregion
        #region idSchool: 1, idPartBody: 6, Category: Move, Label: MoveBack
        bodyDatas.Add((1, 6, Category.Move, Label.MoveBackFrame0),
            (GetPartBodyPositionData(0f, 0f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 0f)));
        bodyDatas.Add((1, 6, Category.Move, Label.MoveBackFrame1),
            (GetPartBodyPositionData(0f, 0f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 0f)));
        #endregion
        #region idSchool: 1, idPartBody: 7, Category: Move, Label: MoveBack
        bodyDatas.Add((1, 7, Category.Move, Label.MoveBackFrame0),
            (GetPartBodyPositionData(0.11921f, 0.864f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        bodyDatas.Add((1, 7, Category.Move, Label.MoveBackFrame1),
            (GetPartBodyPositionData(0.11921f, 0.864f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        #endregion
        #region idSchool: 1, idPartBody: 8, Category: Move, Label: MoveBack
        bodyDatas.Add((1, 8, Category.Move, Label.MoveBackFrame0),
            (GetPartBodyPositionData(0f, 0f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 0f)));
        bodyDatas.Add((1, 8, Category.Move, Label.MoveBackFrame1),
            (GetPartBodyPositionData(0f, 0f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 0f)));
        #endregion

        #region idSchool: 1, idPartBody: 0, Category: Move, Label: MoveLeft
        bodyDatas.Add((1, 0, Category.Move, Label.MoveLeftFrame0),
            (GetPartBodyPositionData(0f, 0.314f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        bodyDatas.Add((1, 0, Category.Move, Label.MoveLeftFrame1),
            (GetPartBodyPositionData(0f, 0.314f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        #endregion
        #region idSchool: 1, idPartBody: 1, Category: Move, Label: MoveLeft
        bodyDatas.Add((1, 1, Category.Move, Label.MoveLeftFrame0),
            (GetPartBodyPositionData(0f, 0.714f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        bodyDatas.Add((1, 1, Category.Move, Label.MoveLeftFrame1),
            (GetPartBodyPositionData(0.062f, 0.714f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        #endregion
        #region idSchool: 1, idPartBody: 2, Category: Move, Label: MoveLeft
        bodyDatas.Add((1, 2, Category.Move, Label.MoveLeftFrame0),
            (GetPartBodyPositionData(0.00953f, 1.544f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        bodyDatas.Add((1, 2, Category.Move, Label.MoveLeftFrame1),
            (GetPartBodyPositionData(0.00953f, 1.544f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        #endregion
        #region idSchool: 1, idPartBody: 3, Category: Move, Label: MoveLeft
        bodyDatas.Add((1, 3, Category.Move, Label.MoveLeftFrame0),
            (GetPartBodyPositionData(0.01153f, 1.942f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        bodyDatas.Add((1, 3, Category.Move, Label.MoveLeftFrame1),
            (GetPartBodyPositionData(0.01153f, 1.942f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        #endregion
        #region idSchool: 1, idPartBody: 4, Category: Move, Label: MoveLeft
        bodyDatas.Add((1, 4, Category.Move, Label.MoveLeftFrame0),
            (GetPartBodyPositionData(-0.141f, 1.425f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        bodyDatas.Add((1, 4, Category.Move, Label.MoveLeftFrame1),
            (GetPartBodyPositionData(-0.141f, 1.425f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        #endregion
        #region idSchool: 1, idPartBody: 5, Category: Move, Label: MoveLeft
        bodyDatas.Add((1, 5, Category.Move, Label.MoveLeftFrame0),
            (GetPartBodyPositionData(-0.00647f, 1.8502f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        bodyDatas.Add((1, 5, Category.Move, Label.MoveLeftFrame1),
            (GetPartBodyPositionData(-0.00647f, 1.8502f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        #endregion
        #region idSchool: 1, idPartBody: 6, Category: Move, Label: MoveLeft
        bodyDatas.Add((1, 6, Category.Move, Label.MoveLeftFrame0),
            (GetPartBodyPositionData(-0.112f, 0.979f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        bodyDatas.Add((1, 6, Category.Move, Label.MoveLeftFrame1),
            (GetPartBodyPositionData(-0.112f, 0.979f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        #endregion
        #region idSchool: 1, idPartBody: 7, Category: Move, Label: MoveLeft
        bodyDatas.Add((1, 7, Category.Move, Label.MoveLeftFrame0),
            (GetPartBodyPositionData(0f, 0f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 0f)));
        bodyDatas.Add((1, 7, Category.Move, Label.MoveLeftFrame1),
            (GetPartBodyPositionData(0f, 0f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 0f)));
        #endregion
        #region idSchool: 1, idPartBody: 8, Category: Move, Label: MoveLeft
        bodyDatas.Add((1, 8, Category.Move, Label.MoveLeftFrame0),
            (GetPartBodyPositionData(0f, 0f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 0f)));
        bodyDatas.Add((1, 8, Category.Move, Label.MoveLeftFrame1),
            (GetPartBodyPositionData(0f, 0f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 0f)));
        #endregion

        #region idSchool: 1, idPartBody: 0, Category: Move, Label: MoveRight
        bodyDatas.Add((1, 0, Category.Move, Label.MoveRightFrame0),
            (GetPartBodyPositionData(0f, 0.314f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        bodyDatas.Add((1, 0, Category.Move, Label.MoveRightFrame1),
            (GetPartBodyPositionData(0f, 0.314f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        #endregion
        #region idSchool: 1, idPartBody: 1, Category: Move, Label: MoveRight
        bodyDatas.Add((1, 1, Category.Move, Label.MoveRightFrame0),
            (GetPartBodyPositionData(0f, 0.714f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        bodyDatas.Add((1, 1, Category.Move, Label.MoveRightFrame1),
            (GetPartBodyPositionData(0.062f, 0.714f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        #endregion
        #region idSchool: 1, idPartBody: 2, Category: Move, Label: MoveRight
        bodyDatas.Add((1, 2, Category.Move, Label.MoveRightFrame0),
            (GetPartBodyPositionData(0.00953f, 1.544f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        bodyDatas.Add((1, 2, Category.Move, Label.MoveRightFrame1),
            (GetPartBodyPositionData(0.00953f, 1.544f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        #endregion
        #region idSchool: 1, idPartBody: 3, Category: Move, Label: MoveRight
        bodyDatas.Add((1, 3, Category.Move, Label.MoveRightFrame0),
            (GetPartBodyPositionData(0.01153f, 1.942f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        bodyDatas.Add((1, 3, Category.Move, Label.MoveRightFrame1),
            (GetPartBodyPositionData(0.01153f, 1.942f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        #endregion
        #region idSchool: 1, idPartBody: 4, Category: Move, Label: MoveRight
        bodyDatas.Add((1, 4, Category.Move, Label.MoveRightFrame0),
            (GetPartBodyPositionData(-0.141f, 1.425f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        bodyDatas.Add((1, 4, Category.Move, Label.MoveRightFrame1),
            (GetPartBodyPositionData(-0.141f, 1.425f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        #endregion
        #region idSchool: 1, idPartBody: 5, Category: Move, Label: MoveRight
        bodyDatas.Add((1, 5, Category.Move, Label.MoveRightFrame0),
            (GetPartBodyPositionData(-0.00647f, 1.8502f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        bodyDatas.Add((1, 5, Category.Move, Label.MoveRightFrame1),
            (GetPartBodyPositionData(-0.00647f, 1.8502f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        #endregion
        #region idSchool: 1, idPartBody: 6, Category: Move, Label: MoveRight
        bodyDatas.Add((1, 6, Category.Move, Label.MoveRightFrame0),
            (GetPartBodyPositionData(-0.112f, 0.979f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        bodyDatas.Add((1, 6, Category.Move, Label.MoveRightFrame1),
            (GetPartBodyPositionData(-0.112f, 0.979f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        #endregion
        #region idSchool: 1, idPartBody: 7, Category: Move, Label: MoveRight
        bodyDatas.Add((1, 7, Category.Move, Label.MoveRightFrame0),
            (GetPartBodyPositionData(0f, 0f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 0f)));
        bodyDatas.Add((1, 7, Category.Move, Label.MoveRightFrame1),
            (GetPartBodyPositionData(0f, 0f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 0f)));
        #endregion
        #region idSchool: 1, idPartBody: 8, Category: Move, Label: MoveRight
        bodyDatas.Add((1, 8, Category.Move, Label.MoveRightFrame0),
            (GetPartBodyPositionData(0f, 0f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 0f)));
        bodyDatas.Add((1, 8, Category.Move, Label.MoveRightFrame1),
            (GetPartBodyPositionData(0f, 0f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 0f)));
        #endregion

        #region idSchool: 1, idPartBody: 0, Category: Atk, Label: AtkFront
        bodyDatas.Add((1, 0, Category.Atk, Label.AtkFrontFrame0),
            (GetPartBodyPositionData(0f, 0.27f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        bodyDatas.Add((1, 0, Category.Atk, Label.AtkFrontFrame1),
            (GetPartBodyPositionData(0f, 0.27f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        #endregion
        #region idSchool: 1, idPartBody: 1, Category: Atk, Label: AtkFront
        bodyDatas.Add((1, 1, Category.Atk, Label.AtkFrontFrame0),
            (GetPartBodyPositionData(-0.12479f, 0.954f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        bodyDatas.Add((1, 1, Category.Atk, Label.AtkFrontFrame1),
            (GetPartBodyPositionData(0.00974f, 0.699f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        #endregion
        #region idSchool: 1, idPartBody: 2, Category: Atk, Label: AtkFront
        bodyDatas.Add((1, 2, Category.Atk, Label.AtkFrontFrame0),
            (GetPartBodyPositionData(0.01774f, 1.544f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        bodyDatas.Add((1, 2, Category.Atk, Label.AtkFrontFrame1),
            (GetPartBodyPositionData(0.01774f, 1.544f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        #endregion
        #region idSchool: 1, idPartBody: 3, Category: Atk, Label: AtkFront
        bodyDatas.Add((1, 3, Category.Atk, Label.AtkFrontFrame0),
            (GetPartBodyPositionData(0.01974f, 1.883f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        bodyDatas.Add((1, 3, Category.Atk, Label.AtkFrontFrame1),
            (GetPartBodyPositionData(0.01974f, 1.883f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        #endregion
        #region idSchool: 1, idPartBody: 4, Category: Atk, Label: AtkFront
        bodyDatas.Add((1, 4, Category.Atk, Label.AtkFrontFrame0),
            (GetPartBodyPositionData(0.01874f, 1.416f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        bodyDatas.Add((1, 4, Category.Atk, Label.AtkFrontFrame1),
            (GetPartBodyPositionData(0.01874f, 1.355f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        #endregion
        #region idSchool: 1, idPartBody: 5, Category: Atk, Label: AtkFront
        bodyDatas.Add((1, 5, Category.Atk, Label.AtkFrontFrame0),
            (GetPartBodyPositionData(0.00174f, 1.8502f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        bodyDatas.Add((1, 5, Category.Atk, Label.AtkFrontFrame1),
            (GetPartBodyPositionData(0.00174f, 1.8502f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        #endregion
        #region idSchool: 1, idPartBody: 6, Category: Atk, Label: AtkFront
        bodyDatas.Add((1, 6, Category.Atk, Label.AtkFrontFrame0),
            (GetPartBodyPositionData(-0.38879f, 1.114f, 0f), GetPartBodyRotationData(0f, 0f, 130f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        bodyDatas.Add((1, 6, Category.Atk, Label.AtkFrontFrame1),
            (GetPartBodyPositionData(0.01121f, 1.564f, 0f), GetPartBodyRotationData(0f, 0f, 90f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 0f)));
        #endregion
        #region idSchool: 1, idPartBody: 7, Category: Atk, Label: AtkFront
        bodyDatas.Add((1, 7, Category.Atk, Label.AtkFrontFrame0),
            (GetPartBodyPositionData(0.11921f, 0.864f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 0f)));
        bodyDatas.Add((1, 7, Category.Atk, Label.AtkFrontFrame1),
            (GetPartBodyPositionData(0.08121f, 0.514f, 0f), GetPartBodyRotationData(-180f, 0f, -140f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        #endregion
        #region idSchool: 1, idPartBody: 8, Category: Atk, Label: AtkFront
        bodyDatas.Add((1, 8, Category.Atk, Label.AtkFrontFrame0),
            (GetPartBodyPositionData(-0.73879f, 1.164f, 0f), GetPartBodyRotationData(0f, 180f, 85f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 0f)));
        bodyDatas.Add((1, 8, Category.Atk, Label.AtkFrontFrame1),
            (GetPartBodyPositionData(-0.63879f, -0.136f, 0f), GetPartBodyRotationData(40f, 180f, 66f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        #endregion

        #region idSchool: 1, idPartBody: 0, Category: Atk, Label: AtkBack
        bodyDatas.Add((1, 0, Category.Atk, Label.AtkBackFrame0),
            (GetPartBodyPositionData(0f, 0.289f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        bodyDatas.Add((1, 0, Category.Atk, Label.AtkBackFrame1),
            (GetPartBodyPositionData(0f, 0.289f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        #endregion
        #region idSchool: 1, idPartBody: 1, Category: Atk, Label: AtkBack
        bodyDatas.Add((1, 1, Category.Atk, Label.AtkBackFrame0),
            (GetPartBodyPositionData(0.12821f, 0.913f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        bodyDatas.Add((1, 1, Category.Atk, Label.AtkBackFrame1),
            (GetPartBodyPositionData(0.00221f, 0.864f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        #endregion
        #region idSchool: 1, idPartBody: 2, Category: Atk, Label: AtkBack
        bodyDatas.Add((1, 2, Category.Atk, Label.AtkBackFrame0),
            (GetPartBodyPositionData(0.002f, 1.544f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        bodyDatas.Add((1, 2, Category.Atk, Label.AtkBackFrame1),
            (GetPartBodyPositionData(0.002f, 1.544f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        #endregion
        #region idSchool: 1, idPartBody: 3, Category: Atk, Label: AtkBack
        bodyDatas.Add((1, 3, Category.Atk, Label.AtkBackFrame0),
            (GetPartBodyPositionData(0.004f, 1.514f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        bodyDatas.Add((1, 3, Category.Atk, Label.AtkBackFrame1),
            (GetPartBodyPositionData(0.004f, 1.514f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        #endregion
        #region idSchool: 1, idPartBody: 4, Category: Atk, Label: AtkBack
        bodyDatas.Add((1, 4, Category.Atk, Label.AtkBackFrame0),
            (GetPartBodyPositionData(0f, 0f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 0f)));
        bodyDatas.Add((1, 4, Category.Atk, Label.AtkBackFrame1),
            (GetPartBodyPositionData(0f, 0f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 0f)));
        #endregion
        #region idSchool: 1, idPartBody: 5, Category: Atk, Label: AtkBack
        bodyDatas.Add((1, 5, Category.Atk, Label.AtkBackFrame0),
            (GetPartBodyPositionData(-0.014f, 1.8502f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        bodyDatas.Add((1, 5, Category.Atk, Label.AtkBackFrame1),
            (GetPartBodyPositionData(-0.014f, 1.8502f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        #endregion
        #region idSchool: 1, idPartBody: 6, Category: Atk, Label: AtkBack
        bodyDatas.Add((1, 6, Category.Atk, Label.AtkBackFrame0),
            (GetPartBodyPositionData(-0.10379f, 0.929f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 0f)));
        bodyDatas.Add((1, 6, Category.Atk, Label.AtkBackFrame1),
            (GetPartBodyPositionData(-0.40453f, 1.004f, 0f), GetPartBodyRotationData(0f, 0f, 135f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        #endregion
        #region idSchool: 1, idPartBody: 7, Category: Atk, Label: AtkBack
        bodyDatas.Add((1, 7, Category.Atk, Label.AtkBackFrame0),
            (GetPartBodyPositionData(0.45426f, 1.014f, 0f), GetPartBodyRotationData(180f, 0f, -135f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        bodyDatas.Add((1, 7, Category.Atk, Label.AtkBackFrame1),
            (GetPartBodyPositionData(0.10347f, 0.604f, 0f), GetPartBodyRotationData(9f, 0f, 17f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 0f)));
        #endregion
        #region idSchool: 1, idPartBody: 8, Category: Atk, Label: AtkBack
        bodyDatas.Add((1, 8, Category.Atk, Label.AtkBackFrame0),
            (GetPartBodyPositionData(1.0112f, 0.664f, 0f), GetPartBodyRotationData(140f, 0f, 77f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 0f)));
        bodyDatas.Add((1, 8, Category.Atk, Label.AtkBackFrame1),
            (GetPartBodyPositionData(0.51121f, 1.864f, 0f), GetPartBodyRotationData(150f, 0f, 50f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        #endregion

        #region idSchool: 1, idPartBody: 0, Category: Atk, Label: AtkLeft
        bodyDatas.Add((1, 0, Category.Atk, Label.AtkLeftFrame0),
            (GetPartBodyPositionData(0f, 0.314f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        bodyDatas.Add((1, 0, Category.Atk, Label.AtkLeftFrame1),
            (GetPartBodyPositionData(0f, 0.314f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        #endregion
        #region idSchool: 1, idPartBody: 1, Category: Atk, Label: AtkLeft
        bodyDatas.Add((1, 1, Category.Atk, Label.AtkLeftFrame0),
            (GetPartBodyPositionData(0.01f, 0.854f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        bodyDatas.Add((1, 1, Category.Atk, Label.AtkLeftFrame1),
            (GetPartBodyPositionData(-0.03f, 0.764f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        #endregion
        #region idSchool: 1, idPartBody: 2, Category: Atk, Label: AtkLeft
        bodyDatas.Add((1, 2, Category.Atk, Label.AtkLeftFrame0),
            (GetPartBodyPositionData(0.04f, 1.544f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        bodyDatas.Add((1, 2, Category.Atk, Label.AtkLeftFrame1),
            (GetPartBodyPositionData(-0.03f, 1.544f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        #endregion
        #region idSchool: 1, idPartBody: 3, Category: Atk, Label: AtkLeft
        bodyDatas.Add((1, 3, Category.Atk, Label.AtkLeftFrame0),
            (GetPartBodyPositionData(0.07853f, 1.942f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        bodyDatas.Add((1, 3, Category.Atk, Label.AtkLeftFrame1),
            (GetPartBodyPositionData(-0.02807f, 1.942f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        #endregion
        #region idSchool: 1, idPartBody: 4, Category: Atk, Label: AtkLeft
        bodyDatas.Add((1, 4, Category.Atk, Label.AtkLeftFrame0),
            (GetPartBodyPositionData(-0.11f, 1.425f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        bodyDatas.Add((1, 4, Category.Atk, Label.AtkLeftFrame1),
            (GetPartBodyPositionData(-0.2166f, 1.364f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        #endregion
        #region idSchool: 1, idPartBody: 5, Category: Atk, Label: AtkLeft
        bodyDatas.Add((1, 5, Category.Atk, Label.AtkLeftFrame0),
            (GetPartBodyPositionData(0.06053f, 1.8502f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        bodyDatas.Add((1, 5, Category.Atk, Label.AtkLeftFrame1),
            (GetPartBodyPositionData(-0.04607f, 1.8502f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        #endregion
        #region idSchool: 1, idPartBody: 6, Category: Atk, Label: AtkLeft
        bodyDatas.Add((1, 6, Category.Atk, Label.AtkLeftFrame0),
            (GetPartBodyPositionData(-0.23f, 0.964f, 0f), GetPartBodyRotationData(0f, 0f, 90f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        bodyDatas.Add((1, 6, Category.Atk, Label.AtkLeftFrame1),
            (GetPartBodyPositionData(-0.13f, 1.414f, 0f), GetPartBodyRotationData(0f, 0f, 90f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 0f)));
        #endregion
        #region idSchool: 1, idPartBody: 7, Category: Atk, Label: AtkLeft
        bodyDatas.Add((1, 7, Category.Atk, Label.AtkLeftFrame0),
            (GetPartBodyPositionData(0.178f, 0.864f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 0f)));
        bodyDatas.Add((1, 7, Category.Atk, Label.AtkLeftFrame1),
            (GetPartBodyPositionData(-0.31f, 0.614f, 0f), GetPartBodyRotationData(180f, 0f, -50f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        #endregion
        #region idSchool: 1, idPartBody: 8, Category: Atk, Label: AtkLeft
        bodyDatas.Add((1, 8, Category.Atk, Label.AtkLeftFrame0),
            (GetPartBodyPositionData(-0.28879f, 1.764f, 0f), GetPartBodyRotationData(0f, 180f, 140f), GetPartBodyScaleData(0.7f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 0f)));
        bodyDatas.Add((1, 8, Category.Atk, Label.AtkLeftFrame1),
            (GetPartBodyPositionData(-0.98879f, 1.564f, 0f), GetPartBodyRotationData(180f, 0f, -50f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        #endregion

        #region idSchool: 1, idPartBody: 0, Category: Atk, Label: AtkRight
        bodyDatas.Add((1, 0, Category.Atk, Label.AtkRightFrame0),
            (GetPartBodyPositionData(0f, 0.314f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        bodyDatas.Add((1, 0, Category.Atk, Label.AtkRightFrame1),
            (GetPartBodyPositionData(0f, 0.314f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        #endregion
        #region idSchool: 1, idPartBody: 1, Category: Atk, Label: AtkRight
        bodyDatas.Add((1, 1, Category.Atk, Label.AtkRightFrame0),
            (GetPartBodyPositionData(0.01f, 0.854f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        bodyDatas.Add((1, 1, Category.Atk, Label.AtkRightFrame1),
            (GetPartBodyPositionData(-0.03f, 0.764f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        #endregion
        #region idSchool: 1, idPartBody: 2, Category: Atk, Label: AtkRight
        bodyDatas.Add((1, 2, Category.Atk, Label.AtkRightFrame0),
            (GetPartBodyPositionData(0.04f, 1.544f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        bodyDatas.Add((1, 2, Category.Atk, Label.AtkRightFrame1),
            (GetPartBodyPositionData(-0.03f, 1.544f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        #endregion
        #region idSchool: 1, idPartBody: 3, Category: Atk, Label: AtkRight
        bodyDatas.Add((1, 3, Category.Atk, Label.AtkRightFrame0),
            (GetPartBodyPositionData(0.07853f, 1.942f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        bodyDatas.Add((1, 3, Category.Atk, Label.AtkRightFrame1),
            (GetPartBodyPositionData(-0.02807f, 1.942f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        #endregion
        #region idSchool: 1, idPartBody: 4, Category: Atk, Label: AtkRight
        bodyDatas.Add((1, 4, Category.Atk, Label.AtkRightFrame0),
            (GetPartBodyPositionData(-0.11f, 1.425f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        bodyDatas.Add((1, 4, Category.Atk, Label.AtkRightFrame1),
            (GetPartBodyPositionData(-0.2166f, 1.364f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        #endregion
        #region idSchool: 1, idPartBody: 5, Category: Atk, Label: AtkRight
        bodyDatas.Add((1, 5, Category.Atk, Label.AtkRightFrame0),
            (GetPartBodyPositionData(0.06053f, 1.8502f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        bodyDatas.Add((1, 5, Category.Atk, Label.AtkRightFrame1),
            (GetPartBodyPositionData(-0.04607f, 1.8502f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        #endregion
        #region idSchool: 1, idPartBody: 6, Category: Atk, Label: AtkRight
        bodyDatas.Add((1, 6, Category.Atk, Label.AtkRightFrame0),
            (GetPartBodyPositionData(-0.23f, 0.964f, 0f), GetPartBodyRotationData(0f, 0f, 90f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        bodyDatas.Add((1, 6, Category.Atk, Label.AtkRightFrame1),
            (GetPartBodyPositionData(-0.13f, 1.414f, 0f), GetPartBodyRotationData(0f, 0f, 90f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 0f)));
        #endregion
        #region idSchool: 1, idPartBody: 7, Category: Atk, Label: AtkRight
        bodyDatas.Add((1, 7, Category.Atk, Label.AtkRightFrame0),
            (GetPartBodyPositionData(0.178f, 0.864f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 0f)));
        bodyDatas.Add((1, 7, Category.Atk, Label.AtkRightFrame1),
            (GetPartBodyPositionData(-0.31f, 0.614f, 0f), GetPartBodyRotationData(180f, 0f, -50f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        #endregion
        #region idSchool: 1, idPartBody: 8, Category: Atk, Label: AtkRight
        bodyDatas.Add((1, 8, Category.Atk, Label.AtkRightFrame0),
            (GetPartBodyPositionData(-0.28879f, 1.764f, 0f), GetPartBodyRotationData(0f, 180f, 140f), GetPartBodyScaleData(0.7f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 0f)));
        bodyDatas.Add((1, 8, Category.Atk, Label.AtkRightFrame1),
            (GetPartBodyPositionData(-0.98879f, 1.564f, 0f), GetPartBodyRotationData(180f, 0f, -50f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        #endregion

        #region idSchool: 1, idPartBody: 0, Category: Die, Label: DieFrame0
        bodyDatas.Add((1, 0, Category.Die, Label.DieFrame0),
            (GetPartBodyPositionData(0f, 0f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 0f)));
        #endregion
        #region idSchool: 1, idPartBody: 1, Category: Die, Label: DieFrame0
        bodyDatas.Add((1, 1, Category.Die, Label.DieFrame0),
            (GetPartBodyPositionData(-0.013f, 0.45f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        #endregion
        #region idSchool: 1, idPartBody: 2, Category: Die, Label: DieFrame0
        bodyDatas.Add((1, 2, Category.Die, Label.DieFrame0),
            (GetPartBodyPositionData(0.01774f, 1.494f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        #endregion
        #region idSchool: 1, idPartBody: 3, Category: Die, Label: DieFrame0
        bodyDatas.Add((1, 3, Category.Die, Label.DieFrame0),
            (GetPartBodyPositionData(0f, 0f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 0f)));
        #endregion
        #region idSchool: 1, idPartBody: 4, Category: Die, Label: DieFrame0
        bodyDatas.Add((1, 4, Category.Die, Label.DieFrame0),
            (GetPartBodyPositionData(0.01874f, 1.15f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        #endregion
        #region idSchool: 1, idPartBody: 5, Category: Die, Label: DieFrame0
        bodyDatas.Add((1, 5, Category.Die, Label.DieFrame0),
            (GetPartBodyPositionData(0.00174f, 1.8f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        #endregion
        #region idSchool: 1, idPartBody: 6, Category: Die, Label: DieFrame0
        bodyDatas.Add((1, 6, Category.Die, Label.DieFrame0),
            (GetPartBodyPositionData(0f, 0f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 0f)));
        #endregion
        #region idSchool: 1, idPartBody: 7, Category: Die, Label: DieFrame0
        bodyDatas.Add((1, 7, Category.Die, Label.DieFrame0),
            (GetPartBodyPositionData(0f, 0f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 0f)));
        #endregion
        #region idSchool: 1, idPartBody: 8, Category: Die, Label: DieFrame0
        bodyDatas.Add((1, 8, Category.Die, Label.DieFrame0),
            (GetPartBodyPositionData(0f, 0f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 0f)));
        #endregion
        #endregion

        #region SatThu
        #region idSchool: 2, idPartBody: 0, Category: Stand, Label: StandFront
        bodyDatas.Add((2, 0, Category.Stand, Label.StandFrontFrame0),
            (GetPartBodyPositionData(0f, 0.27f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        bodyDatas.Add((2, 0, Category.Stand, Label.StandFrontFrame1),
            (GetPartBodyPositionData(0f, 0.27f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        #endregion
        #region idSchool: 2, idPartBody: 1, Category: Stand, Label: StandFront
        bodyDatas.Add((2, 1, Category.Stand, Label.StandFrontFrame0),
            (GetPartBodyPositionData(-0.00079f, 0.739f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        bodyDatas.Add((2, 1, Category.Stand, Label.StandFrontFrame1),
            (GetPartBodyPositionData(-0.00079f, 0.769f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        #endregion
        #region idSchool: 2, idPartBody: 2, Category: Stand, Label: StandFront
        bodyDatas.Add((2, 2, Category.Stand, Label.StandFrontFrame0),
            (GetPartBodyPositionData(-0.03179f, 1.494f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        bodyDatas.Add((2, 2, Category.Stand, Label.StandFrontFrame1),
            (GetPartBodyPositionData(-0.03179f, 1.524f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        #endregion
        #region idSchool: 2, idPartBody: 3, Category: Stand, Label: StandFront
        bodyDatas.Add((2, 3, Category.Stand, Label.StandFrontFrame0),
            (GetPartBodyPositionData(-0.12479f, 1.519f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        bodyDatas.Add((2, 3, Category.Stand, Label.StandFrontFrame1),
            (GetPartBodyPositionData(-0.12479f, 1.549f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        #endregion
        #region idSchool: 2, idPartBody: 4, Category: Stand, Label: StandFront
        bodyDatas.Add((2, 4, Category.Stand, Label.StandFrontFrame0),
            (GetPartBodyPositionData(-0.03179f, 1.355f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        bodyDatas.Add((2, 4, Category.Stand, Label.StandFrontFrame1),
            (GetPartBodyPositionData(-0.03179f, 1.385f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        #endregion
        #region idSchool: 2, idPartBody: 5, Category: Stand, Label: StandFront
        bodyDatas.Add((2, 5, Category.Stand, Label.StandFrontFrame0),
            (GetPartBodyPositionData(-0.04879f, 1.8002f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        bodyDatas.Add((2, 5, Category.Stand, Label.StandFrontFrame1),
            (GetPartBodyPositionData(-0.04879f, 1.8302f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        #endregion
        #region idSchool: 2, idPartBody: 6, Category: Stand, Label: StandFront
        bodyDatas.Add((2, 6, Category.Stand, Label.StandFrontFrame0),
            (GetPartBodyPositionData(-0.13879f, 0.864f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        bodyDatas.Add((2, 6, Category.Stand, Label.StandFrontFrame1),
            (GetPartBodyPositionData(-0.13879f, 0.894f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        #endregion
        #region idSchool: 2, idPartBody: 7, Category: Stand, Label: StandFront
        bodyDatas.Add((2, 7, Category.Stand, Label.StandFrontFrame0),
            (GetPartBodyPositionData(0f, 0f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 0f)));
        bodyDatas.Add((2, 7, Category.Stand, Label.StandFrontFrame1),
            (GetPartBodyPositionData(0f, 0f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 0f)));
        #endregion
        #region idSchool: 2, idPartBody: 8, Category: Stand, Label: StandFront
        bodyDatas.Add((2, 8, Category.Stand, Label.StandFrontFrame0),
            (GetPartBodyPositionData(0f, 0f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 0f)));
        bodyDatas.Add((2, 8, Category.Stand, Label.StandFrontFrame1),
            (GetPartBodyPositionData(0f, 0f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 0f)));
        #endregion

        #region idSchool: 2, idPartBody: 0, Category: Stand, Label: StandBack
        bodyDatas.Add((2, 0, Category.Stand, Label.StandBackFrame0),
            (GetPartBodyPositionData(0f, 0.27f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        bodyDatas.Add((2, 0, Category.Stand, Label.StandBackFrame1),
            (GetPartBodyPositionData(0f, 0.27f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        #endregion
        #region idSchool: 2, idPartBody: 1, Category: Stand, Label: StandBack
        bodyDatas.Add((2, 1, Category.Stand, Label.StandBackFrame0),
            (GetPartBodyPositionData(-0.00079f, 0.734f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        bodyDatas.Add((2, 1, Category.Stand, Label.StandBackFrame1),
            (GetPartBodyPositionData(-0.00079f, 0.764f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        #endregion
        #region idSchool: 2, idPartBody: 2, Category: Stand, Label: StandBack
        bodyDatas.Add((2, 2, Category.Stand, Label.StandBackFrame0),
            (GetPartBodyPositionData(-0.03179f, 1.494f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        bodyDatas.Add((2, 2, Category.Stand, Label.StandBackFrame1),
            (GetPartBodyPositionData(-0.03179f, 1.524f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        #endregion
        #region idSchool: 2, idPartBody: 3, Category: Stand, Label: StandBack
        bodyDatas.Add((2, 3, Category.Stand, Label.StandBackFrame0),
            (GetPartBodyPositionData(-0.03879f, 1.314f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        bodyDatas.Add((2, 3, Category.Stand, Label.StandBackFrame1),
            (GetPartBodyPositionData(-0.03879f, 1.344f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        #endregion
        #region idSchool: 2, idPartBody: 4, Category: Stand, Label: StandBack
        bodyDatas.Add((2, 4, Category.Stand, Label.StandBackFrame0),
            (GetPartBodyPositionData(0f, 0f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 0f)));
        bodyDatas.Add((2, 4, Category.Stand, Label.StandBackFrame1),
            (GetPartBodyPositionData(0f, 0f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 0f)));
        #endregion
        #region idSchool: 2, idPartBody: 5, Category: Stand, Label: StandBack
        bodyDatas.Add((2, 5, Category.Stand, Label.StandBackFrame0),
            (GetPartBodyPositionData(-0.04879f, 1.8002f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        bodyDatas.Add((2, 5, Category.Stand, Label.StandBackFrame1),
            (GetPartBodyPositionData(-0.04879f, 1.8302f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        #endregion
        #region idSchool: 2, idPartBody: 6, Category: Stand, Label: StandBack
        bodyDatas.Add((2, 6, Category.Stand, Label.StandBackFrame0),
            (GetPartBodyPositionData(0f, 0f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 0f)));
        bodyDatas.Add((2, 6, Category.Stand, Label.StandBackFrame1),
            (GetPartBodyPositionData(0f, 0f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 0f)));
        #endregion
        #region idSchool: 2, idPartBody: 7, Category: Stand, Label: StandBack
        bodyDatas.Add((2, 7, Category.Stand, Label.StandBackFrame0),
            (GetPartBodyPositionData(0.14121f, 0.864f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        bodyDatas.Add((2, 7, Category.Stand, Label.StandBackFrame1),
            (GetPartBodyPositionData(0.14121f, 0.894f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        #endregion
        #region idSchool: 2, idPartBody: 8, Category: Stand, Label: StandBack
        bodyDatas.Add((2, 8, Category.Stand, Label.StandBackFrame0),
            (GetPartBodyPositionData(0f, 0f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 0f)));
        bodyDatas.Add((2, 8, Category.Stand, Label.StandBackFrame1),
            (GetPartBodyPositionData(0f, 0f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 0f)));
        #endregion

        #region idSchool: 2, idPartBody: 0, Category: Stand, Label: StandLeft
        bodyDatas.Add((2, 0, Category.Stand, Label.StandLeftFrame0),
            (GetPartBodyPositionData(0f, 0.27f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        bodyDatas.Add((2, 0, Category.Stand, Label.StandLeftFrame1),
            (GetPartBodyPositionData(0f, 0.27f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        #endregion
        #region idSchool: 2, idPartBody: 1, Category: Stand, Label: StandLeft
        bodyDatas.Add((2, 1, Category.Stand, Label.StandLeftFrame0),
            (GetPartBodyPositionData(-0.03079f, 0.714f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        bodyDatas.Add((2, 1, Category.Stand, Label.StandLeftFrame1),
            (GetPartBodyPositionData(-0.03079f, 0.744f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        #endregion
        #region idSchool: 2, idPartBody: 2, Category: Stand, Label: StandLeft
        bodyDatas.Add((2, 2, Category.Stand, Label.StandLeftFrame0),
            (GetPartBodyPositionData(-0.03179f, 1.494f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        bodyDatas.Add((2, 2, Category.Stand, Label.StandLeftFrame1),
            (GetPartBodyPositionData(-0.03179f, 1.524f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        #endregion
        #region idSchool: 2, idPartBody: 3, Category: Stand, Label: StandLeft
        bodyDatas.Add((2, 3, Category.Stand, Label.StandLeftFrame0),
            (GetPartBodyPositionData(0.01121f, 1.519f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        bodyDatas.Add((2, 3, Category.Stand, Label.StandLeftFrame1),
            (GetPartBodyPositionData(0.01121f, 1.549f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        #endregion
        #region idSchool: 2, idPartBody: 4, Category: Stand, Label: StandLeft
        bodyDatas.Add((2, 4, Category.Stand, Label.StandLeftFrame0),
            (GetPartBodyPositionData(-0.20879f, 1.355f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        bodyDatas.Add((2, 4, Category.Stand, Label.StandLeftFrame1),
            (GetPartBodyPositionData(-0.20879f, 1.385f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        #endregion
        #region idSchool: 2, idPartBody: 5, Category: Stand, Label: StandLeft
        bodyDatas.Add((2, 5, Category.Stand, Label.StandLeftFrame0),
            (GetPartBodyPositionData(-0.04879f, 1.8002f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        bodyDatas.Add((2, 5, Category.Stand, Label.StandLeftFrame1),
            (GetPartBodyPositionData(-0.04879f, 1.8302f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        #endregion
        #region idSchool: 2, idPartBody: 6, Category: Stand, Label: StandLeft
        bodyDatas.Add((2, 6, Category.Stand, Label.StandLeftFrame0),
            (GetPartBodyPositionData(-0.13879f, 0.864f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        bodyDatas.Add((2, 6, Category.Stand, Label.StandLeftFrame1),
            (GetPartBodyPositionData(-0.13879f, 0.894f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        #endregion
        #region idSchool: 2, idPartBody: 7, Category: Stand, Label: StandLeft
        bodyDatas.Add((2, 7, Category.Stand, Label.StandLeftFrame0),
            (GetPartBodyPositionData(0f, 0f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 0f)));
        bodyDatas.Add((2, 7, Category.Stand, Label.StandLeftFrame1),
            (GetPartBodyPositionData(0f, 0f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 0f)));
        #endregion
        #region idSchool: 2, idPartBody: 8, Category: Stand, Label: StandLeft
        bodyDatas.Add((2, 8, Category.Stand, Label.StandLeftFrame0),
            (GetPartBodyPositionData(0f, 0f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 0f)));
        bodyDatas.Add((2, 8, Category.Stand, Label.StandLeftFrame1),
            (GetPartBodyPositionData(0f, 0f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 0f)));
        #endregion

        #region idSchool: 2, idPartBody: 0, Category: Stand, Label: StandRight
        bodyDatas.Add((2, 0, Category.Stand, Label.StandRightFrame0),
            (GetPartBodyPositionData(0f, 0.27f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        bodyDatas.Add((2, 0, Category.Stand, Label.StandRightFrame1),
            (GetPartBodyPositionData(0f, 0.27f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        #endregion
        #region idSchool: 2, idPartBody: 1, Category: Stand, Label: StandRight
        bodyDatas.Add((2, 1, Category.Stand, Label.StandRightFrame0),
            (GetPartBodyPositionData(-0.03079f, 0.714f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        bodyDatas.Add((2, 1, Category.Stand, Label.StandRightFrame1),
            (GetPartBodyPositionData(-0.03079f, 0.744f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        #endregion
        #region idSchool: 2, idPartBody: 2, Category: Stand, Label: StandRight
        bodyDatas.Add((2, 2, Category.Stand, Label.StandRightFrame0),
            (GetPartBodyPositionData(-0.03179f, 1.494f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        bodyDatas.Add((2, 2, Category.Stand, Label.StandRightFrame1),
            (GetPartBodyPositionData(-0.03179f, 1.524f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        #endregion
        #region idSchool: 2, idPartBody: 3, Category: Stand, Label: StandRight
        bodyDatas.Add((2, 3, Category.Stand, Label.StandRightFrame0),
            (GetPartBodyPositionData(0.01121f, 1.519f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        bodyDatas.Add((2, 3, Category.Stand, Label.StandRightFrame1),
            (GetPartBodyPositionData(0.01121f, 1.549f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        #endregion
        #region idSchool: 2, idPartBody: 4, Category: Stand, Label: StandRight
        bodyDatas.Add((2, 4, Category.Stand, Label.StandRightFrame0),
            (GetPartBodyPositionData(-0.20879f, 1.355f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        bodyDatas.Add((2, 4, Category.Stand, Label.StandRightFrame1),
            (GetPartBodyPositionData(-0.20879f, 1.385f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        #endregion
        #region idSchool: 2, idPartBody: 5, Category: Stand, Label: StandRight
        bodyDatas.Add((2, 5, Category.Stand, Label.StandRightFrame0),
            (GetPartBodyPositionData(-0.04879f, 1.8002f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        bodyDatas.Add((2, 5, Category.Stand, Label.StandRightFrame1),
            (GetPartBodyPositionData(-0.04879f, 1.8302f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        #endregion
        #region idSchool: 2, idPartBody: 6, Category: Stand, Label: StandRight
        bodyDatas.Add((2, 6, Category.Stand, Label.StandRightFrame0),
            (GetPartBodyPositionData(-0.13879f, 0.864f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        bodyDatas.Add((2, 6, Category.Stand, Label.StandRightFrame1),
            (GetPartBodyPositionData(-0.13879f, 0.894f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        #endregion
        #region idSchool: 2, idPartBody: 7, Category: Stand, Label: StandRight
        bodyDatas.Add((2, 7, Category.Stand, Label.StandRightFrame0),
            (GetPartBodyPositionData(0f, 0f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 0f)));
        bodyDatas.Add((2, 7, Category.Stand, Label.StandRightFrame1),
            (GetPartBodyPositionData(0f, 0f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 0f)));
        #endregion
        #region idSchool: 2, idPartBody: 8, Category: Stand, Label: StandRight
        bodyDatas.Add((2, 8, Category.Stand, Label.StandRightFrame0),
            (GetPartBodyPositionData(0f, 0f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 0f)));
        bodyDatas.Add((2, 8, Category.Stand, Label.StandRightFrame1),
            (GetPartBodyPositionData(0f, 0f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 0f)));
        #endregion

        #region idSchool: 2, idPartBody: 0, Category: Move, Label: MoveFront
        bodyDatas.Add((2, 0, Category.Move, Label.MoveFrontFrame0),
            (GetPartBodyPositionData(0f, 0.27f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        bodyDatas.Add((2, 0, Category.Move, Label.MoveFrontFrame1),
            (GetPartBodyPositionData(0f, 0.27f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        #endregion
        #region idSchool: 2, idPartBody: 1, Category: Move, Label: MoveFront
        bodyDatas.Add((2, 1, Category.Move, Label.MoveFrontFrame0),
            (GetPartBodyPositionData(-0.03279f, 0.739f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        bodyDatas.Add((2, 1, Category.Move, Label.MoveFrontFrame1),
            (GetPartBodyPositionData(0.03121f, 0.739f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        #endregion
        #region idSchool: 2, idPartBody: 2, Category: Move, Label: MoveFront
        bodyDatas.Add((2, 2, Category.Move, Label.MoveFrontFrame0),
            (GetPartBodyPositionData(-0.03179f, 1.494f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        bodyDatas.Add((2, 2, Category.Move, Label.MoveFrontFrame1),
            (GetPartBodyPositionData(-0.03179f, 1.494f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        #endregion
        #region idSchool: 2, idPartBody: 3, Category: Move, Label: MoveFront
        bodyDatas.Add((2, 3, Category.Move, Label.MoveFrontFrame0),
            (GetPartBodyPositionData(-0.12479f, 1.519f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        bodyDatas.Add((2, 3, Category.Move, Label.MoveFrontFrame1),
            (GetPartBodyPositionData(-0.12479f, 1.519f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        #endregion
        #region idSchool: 2, idPartBody: 4, Category: Move, Label: MoveFront
        bodyDatas.Add((2, 4, Category.Move, Label.MoveFrontFrame0),
            (GetPartBodyPositionData(-0.03179f, 1.355f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        bodyDatas.Add((2, 4, Category.Move, Label.MoveFrontFrame1),
            (GetPartBodyPositionData(-0.03179f, 1.355f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        #endregion
        #region idSchool: 2, idPartBody: 5, Category: Move, Label: MoveFront
        bodyDatas.Add((2, 5, Category.Move, Label.MoveFrontFrame0),
            (GetPartBodyPositionData(-0.04879f, 1.8002f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        bodyDatas.Add((2, 5, Category.Move, Label.MoveFrontFrame1),
            (GetPartBodyPositionData(-0.04879f, 1.8002f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        #endregion
        #region idSchool: 2, idPartBody: 6, Category: Move, Label: MoveFront
        bodyDatas.Add((2, 6, Category.Move, Label.MoveFrontFrame0),
            (GetPartBodyPositionData(-0.13879f, 0.864f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        bodyDatas.Add((2, 6, Category.Move, Label.MoveFrontFrame1),
            (GetPartBodyPositionData(-0.13879f, 0.864f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        #endregion
        #region idSchool: 2, idPartBody: 7, Category: Move, Label: MoveFront
        bodyDatas.Add((2, 7, Category.Move, Label.MoveFrontFrame0),
            (GetPartBodyPositionData(0f, 0f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 0f)));
        bodyDatas.Add((2, 7, Category.Move, Label.MoveFrontFrame1),
            (GetPartBodyPositionData(0f, 0f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 0f)));
        #endregion
        #region idSchool: 2, idPartBody: 8, Category: Move, Label: MoveFront
        bodyDatas.Add((2, 8, Category.Move, Label.MoveFrontFrame0),
            (GetPartBodyPositionData(0f, 0f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 0f)));
        bodyDatas.Add((2, 8, Category.Move, Label.MoveFrontFrame1),
            (GetPartBodyPositionData(0f, 0f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 0f)));
        #endregion

        #region idSchool: 2, idPartBody: 0, Category: Move, Label: MoveBack
        bodyDatas.Add((2, 0, Category.Move, Label.MoveBackFrame0),
            (GetPartBodyPositionData(0f, 0.27f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        bodyDatas.Add((2, 0, Category.Move, Label.MoveBackFrame1),
            (GetPartBodyPositionData(0f, 0.27f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        #endregion
        #region idSchool: 2, idPartBody: 1, Category: Move, Label: MoveBack
        bodyDatas.Add((2, 1, Category.Move, Label.MoveBackFrame0),
            (GetPartBodyPositionData(0.03121f, 0.734f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        bodyDatas.Add((2, 1, Category.Move, Label.MoveBackFrame1),
            (GetPartBodyPositionData(-0.03279f, 0.734f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        #endregion
        #region idSchool: 2, idPartBody: 2, Category: Move, Label: MoveBack
        bodyDatas.Add((2, 2, Category.Move, Label.MoveBackFrame0),
            (GetPartBodyPositionData(-0.03179f, 1.494f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        bodyDatas.Add((2, 2, Category.Move, Label.MoveBackFrame1),
            (GetPartBodyPositionData(-0.03179f, 1.494f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        #endregion
        #region idSchool: 2, idPartBody: 3, Category: Move, Label: MoveBack
        bodyDatas.Add((2, 3, Category.Move, Label.MoveBackFrame0),
            (GetPartBodyPositionData(-0.03879f, 1.314f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        bodyDatas.Add((2, 3, Category.Move, Label.MoveBackFrame1),
            (GetPartBodyPositionData(-0.03879f, 1.314f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        #endregion
        #region idSchool: 2, idPartBody: 4, Category: Move, Label: MoveBack
        bodyDatas.Add((2, 4, Category.Move, Label.MoveBackFrame0),
            (GetPartBodyPositionData(0f, 0f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 0f)));
        bodyDatas.Add((2, 4, Category.Move, Label.MoveBackFrame1),
            (GetPartBodyPositionData(0f, 0f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 0f)));
        #endregion
        #region idSchool: 2, idPartBody: 5, Category: Move, Label: MoveBack
        bodyDatas.Add((2, 5, Category.Move, Label.MoveBackFrame0),
            (GetPartBodyPositionData(-0.04879f, 1.8002f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        bodyDatas.Add((2, 5, Category.Move, Label.MoveBackFrame1),
            (GetPartBodyPositionData(-0.04879f, 1.8002f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        #endregion
        #region idSchool: 2, idPartBody: 6, Category: Move, Label: MoveBack
        bodyDatas.Add((2, 6, Category.Move, Label.MoveBackFrame0),
            (GetPartBodyPositionData(0f, 0f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 0f)));
        bodyDatas.Add((2, 6, Category.Move, Label.MoveBackFrame1),
            (GetPartBodyPositionData(0f, 0f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 0f)));
        #endregion
        #region idSchool: 2, idPartBody: 7, Category: Move, Label: MoveBack
        bodyDatas.Add((2, 7, Category.Move, Label.MoveBackFrame0),
            (GetPartBodyPositionData(0.14121f, 0.864f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        bodyDatas.Add((2, 7, Category.Move, Label.MoveBackFrame1),
            (GetPartBodyPositionData(0.14121f, 0.864f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        #endregion
        #region idSchool: 2, idPartBody: 8, Category: Move, Label: MoveBack
        bodyDatas.Add((2, 8, Category.Move, Label.MoveBackFrame0),
            (GetPartBodyPositionData(0f, 0f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 0f)));
        bodyDatas.Add((2, 8, Category.Move, Label.MoveBackFrame1),
            (GetPartBodyPositionData(0f, 0f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 0f)));
        #endregion

        #region idSchool: 2, idPartBody: 0, Category: Move, Label: MoveLeft
        bodyDatas.Add((2, 0, Category.Move, Label.MoveLeftFrame0),
            (GetPartBodyPositionData(0f, 0.314f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        bodyDatas.Add((2, 0, Category.Move, Label.MoveLeftFrame1),
            (GetPartBodyPositionData(-0.00821f, 0.314f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        #endregion
        #region idSchool: 2, idPartBody: 1, Category: Move, Label: MoveLeft
        bodyDatas.Add((2, 1, Category.Move, Label.MoveLeftFrame0),
            (GetPartBodyPositionData(-0.022f, 0.714f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        bodyDatas.Add((2, 1, Category.Move, Label.MoveLeftFrame1),
            (GetPartBodyPositionData(0.03753f, 0.714f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        #endregion
        #region idSchool: 2, idPartBody: 2, Category: Move, Label: MoveLeft
        bodyDatas.Add((2, 2, Category.Move, Label.MoveLeftFrame0),
            (GetPartBodyPositionData(-0.04f, 1.494f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        bodyDatas.Add((2, 2, Category.Move, Label.MoveLeftFrame1),
            (GetPartBodyPositionData(-0.04f, 1.494f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        #endregion
        #region idSchool: 2, idPartBody: 3, Category: Move, Label: MoveLeft
        bodyDatas.Add((2, 3, Category.Move, Label.MoveLeftFrame0),
            (GetPartBodyPositionData(0.003f, 1.519f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        bodyDatas.Add((2, 3, Category.Move, Label.MoveLeftFrame1),
            (GetPartBodyPositionData(0.003f, 1.519f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        #endregion
        #region idSchool: 2, idPartBody: 4, Category: Move, Label: MoveLeft
        bodyDatas.Add((2, 4, Category.Move, Label.MoveLeftFrame0),
            (GetPartBodyPositionData(-0.217f, 1.355f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        bodyDatas.Add((2, 4, Category.Move, Label.MoveLeftFrame1),
            (GetPartBodyPositionData(-0.217f, 1.355f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        #endregion
        #region idSchool: 2, idPartBody: 5, Category: Move, Label: MoveLeft
        bodyDatas.Add((2, 5, Category.Move, Label.MoveLeftFrame0),
            (GetPartBodyPositionData(-0.057f, 1.8002f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        bodyDatas.Add((2, 5, Category.Move, Label.MoveLeftFrame1),
            (GetPartBodyPositionData(-0.057f, 1.8002f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        #endregion
        #region idSchool: 2, idPartBody: 6, Category: Move, Label: MoveLeft
        bodyDatas.Add((2, 6, Category.Move, Label.MoveLeftFrame0),
            (GetPartBodyPositionData(-0.147f, 0.864f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        bodyDatas.Add((2, 6, Category.Move, Label.MoveLeftFrame1),
            (GetPartBodyPositionData(-0.147f, 0.864f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        #endregion
        #region idSchool: 2, idPartBody: 7, Category: Move, Label: MoveLeft
        bodyDatas.Add((2, 7, Category.Move, Label.MoveLeftFrame0),
            (GetPartBodyPositionData(0f, 0f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 0f)));
        bodyDatas.Add((2, 7, Category.Move, Label.MoveLeftFrame1),
            (GetPartBodyPositionData(0f, 0f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 0f)));
        #endregion
        #region idSchool: 2, idPartBody: 8, Category: Move, Label: MoveLeft
        bodyDatas.Add((2, 8, Category.Move, Label.MoveLeftFrame0),
            (GetPartBodyPositionData(0f, 0f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 0f)));
        bodyDatas.Add((2, 8, Category.Move, Label.MoveLeftFrame1),
            (GetPartBodyPositionData(0f, 0f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 0f)));
        #endregion

        #region idSchool: 2, idPartBody: 0, Category: Move, Label: MoveRight
        bodyDatas.Add((2, 0, Category.Move, Label.MoveRightFrame0),
            (GetPartBodyPositionData(0f, 0.314f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        bodyDatas.Add((2, 0, Category.Move, Label.MoveRightFrame1),
            (GetPartBodyPositionData(-0.00821f, 0.314f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        #endregion
        #region idSchool: 2, idPartBody: 1, Category: Move, Label: MoveRight
        bodyDatas.Add((2, 1, Category.Move, Label.MoveRightFrame0),
            (GetPartBodyPositionData(-0.022f, 0.714f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        bodyDatas.Add((2, 1, Category.Move, Label.MoveRightFrame1),
            (GetPartBodyPositionData(0.03753f, 0.714f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        #endregion
        #region idSchool: 2, idPartBody: 2, Category: Move, Label: MoveRight
        bodyDatas.Add((2, 2, Category.Move, Label.MoveRightFrame0),
            (GetPartBodyPositionData(-0.04f, 1.494f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        bodyDatas.Add((2, 2, Category.Move, Label.MoveRightFrame1),
            (GetPartBodyPositionData(-0.04f, 1.494f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        #endregion
        #region idSchool: 2, idPartBody: 3, Category: Move, Label: MoveRight
        bodyDatas.Add((2, 3, Category.Move, Label.MoveRightFrame0),
            (GetPartBodyPositionData(0.003f, 1.519f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        bodyDatas.Add((2, 3, Category.Move, Label.MoveRightFrame1),
            (GetPartBodyPositionData(0.003f, 1.519f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        #endregion
        #region idSchool: 2, idPartBody: 4, Category: Move, Label: MoveRight
        bodyDatas.Add((2, 4, Category.Move, Label.MoveRightFrame0),
            (GetPartBodyPositionData(-0.217f, 1.355f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        bodyDatas.Add((2, 4, Category.Move, Label.MoveRightFrame1),
            (GetPartBodyPositionData(-0.217f, 1.355f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        #endregion
        #region idSchool: 2, idPartBody: 5, Category: Move, Label: MoveRight
        bodyDatas.Add((2, 5, Category.Move, Label.MoveRightFrame0),
            (GetPartBodyPositionData(-0.057f, 1.8002f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        bodyDatas.Add((2, 5, Category.Move, Label.MoveRightFrame1),
            (GetPartBodyPositionData(-0.057f, 1.8002f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        #endregion
        #region idSchool: 2, idPartBody: 6, Category: Move, Label: MoveRight
        bodyDatas.Add((2, 6, Category.Move, Label.MoveRightFrame0),
            (GetPartBodyPositionData(-0.147f, 0.864f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        bodyDatas.Add((2, 6, Category.Move, Label.MoveRightFrame1),
            (GetPartBodyPositionData(-0.147f, 0.864f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        #endregion
        #region idSchool: 2, idPartBody: 7, Category: Move, Label: MoveRight
        bodyDatas.Add((2, 7, Category.Move, Label.MoveRightFrame0),
            (GetPartBodyPositionData(0f, 0f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 0f)));
        bodyDatas.Add((2, 7, Category.Move, Label.MoveRightFrame1),
            (GetPartBodyPositionData(0f, 0f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 0f)));
        #endregion
        #region idSchool: 2, idPartBody: 8, Category: Move, Label: MoveRight
        bodyDatas.Add((2, 8, Category.Move, Label.MoveRightFrame0),
            (GetPartBodyPositionData(0f, 0f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 0f)));
        bodyDatas.Add((2, 8, Category.Move, Label.MoveRightFrame1),
            (GetPartBodyPositionData(0f, 0f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 0f)));
        #endregion

        #region idSchool: 2, idPartBody: 0, Category: Atk, Label: AtkFront
        bodyDatas.Add((2, 0, Category.Atk, Label.AtkFrontFrame0),
            (GetPartBodyPositionData(0f, 0.27f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        bodyDatas.Add((2, 0, Category.Atk, Label.AtkFrontFrame1),
            (GetPartBodyPositionData(0f, 0.27f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        #endregion
        #region idSchool: 2, idPartBody: 1, Category: Atk, Label: AtkFront
        bodyDatas.Add((2, 1, Category.Atk, Label.AtkFrontFrame0),
            (GetPartBodyPositionData(-0.09379f, 0.774f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        bodyDatas.Add((2, 1, Category.Atk, Label.AtkFrontFrame1),
            (GetPartBodyPositionData(-0.09379f, 0.739f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        #endregion
        #region idSchool: 2, idPartBody: 2, Category: Atk, Label: AtkFront
        bodyDatas.Add((2, 2, Category.Atk, Label.AtkFrontFrame0),
            (GetPartBodyPositionData(-0.03179f, 1.494f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        bodyDatas.Add((2, 2, Category.Atk, Label.AtkFrontFrame1),
            (GetPartBodyPositionData(-0.03179f, 1.494f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        #endregion
        #region idSchool: 2, idPartBody: 3, Category: Atk, Label: AtkFront
        bodyDatas.Add((2, 3, Category.Atk, Label.AtkFrontFrame0),
            (GetPartBodyPositionData(-0.12479f, 1.519f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        bodyDatas.Add((2, 3, Category.Atk, Label.AtkFrontFrame1),
            (GetPartBodyPositionData(-0.12479f, 1.519f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        #endregion
        #region idSchool: 2, idPartBody: 4, Category: Atk, Label: AtkFront
        bodyDatas.Add((2, 4, Category.Atk, Label.AtkFrontFrame0),
            (GetPartBodyPositionData(-0.03179f, 1.369f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        bodyDatas.Add((2, 4, Category.Atk, Label.AtkFrontFrame1),
            (GetPartBodyPositionData(-0.03179f, 1.307f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        #endregion
        #region idSchool: 2, idPartBody: 5, Category: Atk, Label: AtkFront
        bodyDatas.Add((2, 5, Category.Atk, Label.AtkFrontFrame0),
            (GetPartBodyPositionData(-0.04879f, 1.8002f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        bodyDatas.Add((2, 5, Category.Atk, Label.AtkFrontFrame1),
            (GetPartBodyPositionData(-0.04879f, 1.8002f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        #endregion
        #region idSchool: 2, idPartBody: 6, Category: Atk, Label: AtkFront
        bodyDatas.Add((2, 6, Category.Atk, Label.AtkFrontFrame0),
            (GetPartBodyPositionData(-0.38879f, 0.814f, 0f), GetPartBodyRotationData(0f, 180f, 135f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        bodyDatas.Add((2, 6, Category.Atk, Label.AtkFrontFrame1),
            (GetPartBodyPositionData(0f, 0f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 0f)));
        #endregion
        #region idSchool: 2, idPartBody: 7, Category: Atk, Label: AtkFront
        bodyDatas.Add((2, 7, Category.Atk, Label.AtkFrontFrame0),
            (GetPartBodyPositionData(0f, 0f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 0f)));
        bodyDatas.Add((2, 7, Category.Atk, Label.AtkFrontFrame1),
            (GetPartBodyPositionData(0.16121f, 0.484f, 0f), GetPartBodyRotationData(0f, 0f, 43f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        #endregion
        #region idSchool: 2, idPartBody: 8, Category: Atk, Label: AtkFront
        bodyDatas.Add((2, 8, Category.Atk, Label.AtkFrontFrame0),
            (GetPartBodyPositionData(-0.73879f, 1.164f, 0f), GetPartBodyRotationData(40f, 180f, 85f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 0f)));
        bodyDatas.Add((2, 8, Category.Atk, Label.AtkFrontFrame1),
            (GetPartBodyPositionData(-0.68879f, -0.036f, 0f), GetPartBodyRotationData(40f, 180f, 70f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        #endregion

        #region idSchool: 2, idPartBody: 0, Category: Atk, Label: AtkBack
        bodyDatas.Add((2, 0, Category.Atk, Label.AtkBackFrame0),
            (GetPartBodyPositionData(0f, 0.27f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        bodyDatas.Add((2, 0, Category.Atk, Label.AtkBackFrame1),
            (GetPartBodyPositionData(0f, 0.27f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        #endregion
        #region idSchool: 2, idPartBody: 1, Category: Atk, Label: AtkBack
        bodyDatas.Add((2, 1, Category.Atk, Label.AtkBackFrame0),
            (GetPartBodyPositionData(0.09421f, 0.754f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        bodyDatas.Add((2, 1, Category.Atk, Label.AtkBackFrame1),
            (GetPartBodyPositionData(0.06321f, 0.754f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        #endregion
        #region idSchool: 2, idPartBody: 2, Category: Atk, Label: AtkBack
        bodyDatas.Add((2, 2, Category.Atk, Label.AtkBackFrame0),
            (GetPartBodyPositionData(-0.03179f, 1.494f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        bodyDatas.Add((2, 2, Category.Atk, Label.AtkBackFrame1),
            (GetPartBodyPositionData(-0.03179f, 1.494f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        #endregion
        #region idSchool: 2, idPartBody: 3, Category: Atk, Label: AtkBack
        bodyDatas.Add((2, 3, Category.Atk, Label.AtkBackFrame0),
            (GetPartBodyPositionData(-0.03879f, 1.314f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        bodyDatas.Add((2, 3, Category.Atk, Label.AtkBackFrame1),
            (GetPartBodyPositionData(-0.03879f, 1.314f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        #endregion
        #region idSchool: 2, idPartBody: 4, Category: Atk, Label: AtkBack
        bodyDatas.Add((2, 4, Category.Atk, Label.AtkBackFrame0),
            (GetPartBodyPositionData(0f, 0f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 0f)));
        bodyDatas.Add((2, 4, Category.Atk, Label.AtkBackFrame1),
            (GetPartBodyPositionData(0f, 0f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 0f)));
        #endregion
        #region idSchool: 2, idPartBody: 5, Category: Atk, Label: AtkBack
        bodyDatas.Add((2, 5, Category.Atk, Label.AtkBackFrame0),
            (GetPartBodyPositionData(-0.04879f, 1.8002f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        bodyDatas.Add((2, 5, Category.Atk, Label.AtkBackFrame1),
            (GetPartBodyPositionData(-0.04879f, 1.8002f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        #endregion
        #region idSchool: 2, idPartBody: 6, Category: Atk, Label: AtkBack
        bodyDatas.Add((2, 6, Category.Atk, Label.AtkBackFrame0),
            (GetPartBodyPositionData(-0.43879f, 1.314f, 0f), GetPartBodyRotationData(0f, 0f, 135f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 0f)));
        bodyDatas.Add((2, 6, Category.Atk, Label.AtkBackFrame1),
            (GetPartBodyPositionData(-0.33879f, 1.044f, 0f), GetPartBodyRotationData(0f, 180f, 140f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        #endregion
        #region idSchool: 2, idPartBody: 7, Category: Atk, Label: AtkBack
        bodyDatas.Add((2, 7, Category.Atk, Label.AtkBackFrame0),
            (GetPartBodyPositionData(0.53121f, 0.584f, 0f), GetPartBodyRotationData(0f, 0f, -135f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        bodyDatas.Add((2, 7, Category.Atk, Label.AtkBackFrame1),
            (GetPartBodyPositionData(0.11121f, -0.036f, 0f), GetPartBodyRotationData(0f, 0f, 35f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 0f)));
        #endregion
        #region idSchool: 2, idPartBody: 8, Category: Atk, Label: AtkBack
        bodyDatas.Add((2, 8, Category.Atk, Label.AtkBackFrame0),
            (GetPartBodyPositionData(0.81121f, 0.664f, 0f), GetPartBodyRotationData(140f, 0f, 85f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 0f)));
        bodyDatas.Add((2, 8, Category.Atk, Label.AtkBackFrame1),
            (GetPartBodyPositionData(0.51121f, 1.464f, 0f), GetPartBodyRotationData(180f, -30f, 50f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        #endregion

        #region idSchool: 2, idPartBody: 0, Category: Atk, Label: AtkLeft
        bodyDatas.Add((2, 0, Category.Atk, Label.AtkLeftFrame0),
            (GetPartBodyPositionData(0f, 0.314f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        bodyDatas.Add((2, 0, Category.Atk, Label.AtkLeftFrame1),
            (GetPartBodyPositionData(0f, 0.314f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        #endregion
        #region idSchool: 2, idPartBody: 1, Category: Atk, Label: AtkLeft
        bodyDatas.Add((2, 1, Category.Atk, Label.AtkLeftFrame0),
            (GetPartBodyPositionData(0.061f, 0.774f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        bodyDatas.Add((2, 1, Category.Atk, Label.AtkLeftFrame1),
            (GetPartBodyPositionData(-0.034f, 0.764f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        #endregion
        #region idSchool: 2, idPartBody: 2, Category: Atk, Label: AtkLeft
        bodyDatas.Add((2, 2, Category.Atk, Label.AtkLeftFrame0),
            (GetPartBodyPositionData(0.091f, 1.494f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        bodyDatas.Add((2, 2, Category.Atk, Label.AtkLeftFrame1),
            (GetPartBodyPositionData(-0.034f, 1.494f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        #endregion
        #region idSchool: 2, idPartBody: 3, Category: Atk, Label: AtkLeft
        bodyDatas.Add((2, 3, Category.Atk, Label.AtkLeftFrame0),
            (GetPartBodyPositionData(0.134f, 1.519f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        bodyDatas.Add((2, 3, Category.Atk, Label.AtkLeftFrame1),
            (GetPartBodyPositionData(0.013f, 1.519f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        #endregion
        #region idSchool: 2, idPartBody: 4, Category: Atk, Label: AtkLeft
        bodyDatas.Add((2, 4, Category.Atk, Label.AtkLeftFrame0),
            (GetPartBodyPositionData(-0.086f, 1.355f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        bodyDatas.Add((2, 4, Category.Atk, Label.AtkLeftFrame1),
            (GetPartBodyPositionData(-0.211f, 1.355f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        #endregion
        #region idSchool: 2, idPartBody: 5, Category: Atk, Label: AtkLeft
        bodyDatas.Add((2, 5, Category.Atk, Label.AtkLeftFrame0),
            (GetPartBodyPositionData(0.076f, 1.8002f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        bodyDatas.Add((2, 5, Category.Atk, Label.AtkLeftFrame1),
            (GetPartBodyPositionData(-0.049f, 1.8002f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        #endregion
        #region idSchool: 2, idPartBody: 6, Category: Atk, Label: AtkLeft
        bodyDatas.Add((2, 6, Category.Atk, Label.AtkLeftFrame0),
            (GetPartBodyPositionData(-0.259f, 0.777f, 0f), GetPartBodyRotationData(180f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        bodyDatas.Add((2, 6, Category.Atk, Label.AtkLeftFrame1),
            (GetPartBodyPositionData(0.016f, 0.864f, 0f), GetPartBodyRotationData(180f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 0f)));
        #endregion
        #region idSchool: 2, idPartBody: 7, Category: Atk, Label: AtkLeft
        bodyDatas.Add((2, 7, Category.Atk, Label.AtkLeftFrame0),
            (GetPartBodyPositionData(0.866f, 0.864f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 0f)));
        bodyDatas.Add((2, 7, Category.Atk, Label.AtkLeftFrame1),
            (GetPartBodyPositionData(-0.594f, 0.764f, 0f), GetPartBodyRotationData(0f, 0f, -45f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        #endregion
        #region idSchool: 2, idPartBody: 8, Category: Atk, Label: AtkLeft
        bodyDatas.Add((2, 8, Category.Atk, Label.AtkLeftFrame0),
            (GetPartBodyPositionData(-0.584f, 1.464f, 0f), GetPartBodyRotationData(40f, 190f, 145f), GetPartBodyScaleData(0.7f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 0f)));
        bodyDatas.Add((2, 8, Category.Atk, Label.AtkLeftFrame1),
            (GetPartBodyPositionData(-0.884f, 1.464f, 0f), GetPartBodyRotationData(40f, 180f, 145f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        #endregion

        #region idSchool: 2, idPartBody: 0, Category: Atk, Label: AtkRight
        bodyDatas.Add((2, 0, Category.Atk, Label.AtkRightFrame0),
            (GetPartBodyPositionData(0f, 0.314f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        bodyDatas.Add((2, 0, Category.Atk, Label.AtkRightFrame1),
            (GetPartBodyPositionData(0f, 0.314f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        #endregion
        #region idSchool: 2, idPartBody: 1, Category: Atk, Label: AtkRight
        bodyDatas.Add((2, 1, Category.Atk, Label.AtkRightFrame0),
            (GetPartBodyPositionData(0.061f, 0.774f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        bodyDatas.Add((2, 1, Category.Atk, Label.AtkRightFrame1),
            (GetPartBodyPositionData(-0.034f, 0.764f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        #endregion
        #region idSchool: 2, idPartBody: 2, Category: Atk, Label: AtkRight
        bodyDatas.Add((2, 2, Category.Atk, Label.AtkRightFrame0),
            (GetPartBodyPositionData(0.091f, 1.494f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        bodyDatas.Add((2, 2, Category.Atk, Label.AtkRightFrame1),
            (GetPartBodyPositionData(-0.034f, 1.494f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        #endregion
        #region idSchool: 2, idPartBody: 3, Category: Atk, Label: AtkRight
        bodyDatas.Add((2, 3, Category.Atk, Label.AtkRightFrame0),
            (GetPartBodyPositionData(0.134f, 1.519f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        bodyDatas.Add((2, 3, Category.Atk, Label.AtkRightFrame1),
            (GetPartBodyPositionData(0.013f, 1.519f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        #endregion
        #region idSchool: 2, idPartBody: 4, Category: Atk, Label: AtkRight
        bodyDatas.Add((2, 4, Category.Atk, Label.AtkRightFrame0),
            (GetPartBodyPositionData(-0.086f, 1.355f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        bodyDatas.Add((2, 4, Category.Atk, Label.AtkRightFrame1),
            (GetPartBodyPositionData(-0.211f, 1.355f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        #endregion
        #region idSchool: 2, idPartBody: 5, Category: Atk, Label: AtkRight
        bodyDatas.Add((2, 5, Category.Atk, Label.AtkRightFrame0),
            (GetPartBodyPositionData(0.076f, 1.8002f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        bodyDatas.Add((2, 5, Category.Atk, Label.AtkRightFrame1),
            (GetPartBodyPositionData(-0.049f, 1.8002f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        #endregion
        #region idSchool: 2, idPartBody: 6, Category: Atk, Label: AtkRight
        bodyDatas.Add((2, 6, Category.Atk, Label.AtkRightFrame0),
            (GetPartBodyPositionData(-0.259f, 0.777f, 0f), GetPartBodyRotationData(180f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        bodyDatas.Add((2, 6, Category.Atk, Label.AtkRightFrame1),
            (GetPartBodyPositionData(0.016f, 0.864f, 0f), GetPartBodyRotationData(180f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 0f)));
        #endregion
        #region idSchool: 2, idPartBody: 7, Category: Atk, Label: AtkRight
        bodyDatas.Add((2, 7, Category.Atk, Label.AtkRightFrame0),
            (GetPartBodyPositionData(0.866f, 0.864f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 0f)));
        bodyDatas.Add((2, 7, Category.Atk, Label.AtkRightFrame1),
            (GetPartBodyPositionData(-0.594f, 0.764f, 0f), GetPartBodyRotationData(0f, 0f, -45f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        #endregion
        #region idSchool: 2, idPartBody: 8, Category: Atk, Label: AtkRight
        bodyDatas.Add((2, 8, Category.Atk, Label.AtkRightFrame0),
            (GetPartBodyPositionData(-0.584f, 1.464f, 0f), GetPartBodyRotationData(40f, 190f, 145f), GetPartBodyScaleData(0.7f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 0f)));
        bodyDatas.Add((2, 8, Category.Atk, Label.AtkRightFrame1),
            (GetPartBodyPositionData(-0.884f, 1.464f, 0f), GetPartBodyRotationData(40f, 180f, 145f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        #endregion

        #region idSchool: 2, idPartBody: 0, Category: Die, Label: DieFrame0
        bodyDatas.Add((2, 0, Category.Die, Label.DieFrame0),
            (GetPartBodyPositionData(0f, 0f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 0f)));
        #endregion
        #region idSchool: 2, idPartBody: 1, Category: Die, Label: DieFrame0
        bodyDatas.Add((2, 1, Category.Die, Label.DieFrame0),
            (GetPartBodyPositionData(-0.063f, 0.45f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        #endregion
        #region idSchool: 2, idPartBody: 2, Category: Die, Label: DieFrame0
        bodyDatas.Add((2, 2, Category.Die, Label.DieFrame0),
            (GetPartBodyPositionData(-0.03179f, 1.494f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        #endregion
        #region idSchool: 2, idPartBody: 3, Category: Die, Label: DieFrame0
        bodyDatas.Add((2, 3, Category.Die, Label.DieFrame0),
            (GetPartBodyPositionData(0f, 0f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 0f)));
        #endregion
        #region idSchool: 2, idPartBody: 4, Category: Die, Label: DieFrame0
        bodyDatas.Add((2, 4, Category.Die, Label.DieFrame0),
            (GetPartBodyPositionData(-0.03179f, 1.15f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        #endregion
        #region idSchool: 2, idPartBody: 5, Category: Die, Label: DieFrame0
        bodyDatas.Add((2, 5, Category.Die, Label.DieFrame0),
            (GetPartBodyPositionData(-0.04879f, 1.8002f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        #endregion
        #region idSchool: 2, idPartBody: 6, Category: Die, Label: DieFrame0
        bodyDatas.Add((2, 6, Category.Die, Label.DieFrame0),
            (GetPartBodyPositionData(0f, 0f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 0f)));
        #endregion
        #region idSchool: 2, idPartBody: 7, Category: Die, Label: DieFrame0
        bodyDatas.Add((2, 7, Category.Die, Label.DieFrame0),
            (GetPartBodyPositionData(0f, 0f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 0f)));
        #endregion
        #region idSchool: 2, idPartBody: 8, Category: Die, Label: DieFrame0
        bodyDatas.Add((2, 8, Category.Die, Label.DieFrame0),
            (GetPartBodyPositionData(0f, 0f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 0f)));
        #endregion
        #endregion

        #region PhapSu
        #region idSchool: 3, idPartBody: 0, Category: Stand, Label: StandFront
        bodyDatas.Add((3, 0, Category.Stand, Label.StandFrontFrame0),
            (GetPartBodyPositionData(0f, 0.27f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        bodyDatas.Add((3, 0, Category.Stand, Label.StandFrontFrame1),
            (GetPartBodyPositionData(0f, 0.27f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        #endregion
        #region idSchool: 3, idPartBody: 1, Category: Stand, Label: StandFront
        bodyDatas.Add((3, 1, Category.Stand, Label.StandFrontFrame0),
            (GetPartBodyPositionData(-0.00079f, 0.739f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        bodyDatas.Add((3, 1, Category.Stand, Label.StandFrontFrame1),
            (GetPartBodyPositionData(-0.00079f, 0.769f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        #endregion
        #region idSchool: 3, idPartBody: 2, Category: Stand, Label: StandFront
        bodyDatas.Add((3, 2, Category.Stand, Label.StandFrontFrame0),
            (GetPartBodyPositionData(-0.00179f, 1.494f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        bodyDatas.Add((3, 2, Category.Stand, Label.StandFrontFrame1),
            (GetPartBodyPositionData(-0.00179f, 1.524f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        #endregion
        #region idSchool: 3, idPartBody: 3, Category: Stand, Label: StandFront
        bodyDatas.Add((3, 3, Category.Stand, Label.StandFrontFrame0),
            (GetPartBodyPositionData(0.00121f, 2.054f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        bodyDatas.Add((3, 3, Category.Stand, Label.StandFrontFrame1),
            (GetPartBodyPositionData(0.00121f, 2.084f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        #endregion
        #region idSchool: 3, idPartBody: 4, Category: Stand, Label: StandFront
        bodyDatas.Add((3, 4, Category.Stand, Label.StandFrontFrame0),
            (GetPartBodyPositionData(-0.00179f, 1.364f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        bodyDatas.Add((3, 4, Category.Stand, Label.StandFrontFrame1),
            (GetPartBodyPositionData(-0.00179f, 1.394f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        #endregion
        #region idSchool: 3, idPartBody: 5, Category: Stand, Label: StandFront
        bodyDatas.Add((3, 5, Category.Stand, Label.StandFrontFrame0),
            (GetPartBodyPositionData(-0.03379f, 1.493f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        bodyDatas.Add((3, 5, Category.Stand, Label.StandFrontFrame1),
            (GetPartBodyPositionData(-0.03379f, 1.523f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        #endregion
        #region idSchool: 3, idPartBody: 6, Category: Stand, Label: StandFront
        bodyDatas.Add((3, 6, Category.Stand, Label.StandFrontFrame0),
            (GetPartBodyPositionData(-0.06879f, 0.914f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        bodyDatas.Add((3, 6, Category.Stand, Label.StandFrontFrame1),
            (GetPartBodyPositionData(-0.06879f, 0.944f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        #endregion
        #region idSchool: 3, idPartBody: 7, Category: Stand, Label: StandFront
        bodyDatas.Add((3, 7, Category.Stand, Label.StandFrontFrame0),
            (GetPartBodyPositionData(0f, 0f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 0f)));
        bodyDatas.Add((3, 7, Category.Stand, Label.StandFrontFrame1),
            (GetPartBodyPositionData(0f, 0f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 0f)));
        #endregion

        #region idSchool: 3, idPartBody: 0, Category: Stand, Label: StandBack
        bodyDatas.Add((3, 0, Category.Stand, Label.StandBackFrame0),
            (GetPartBodyPositionData(0f, 0.27f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        bodyDatas.Add((3, 0, Category.Stand, Label.StandBackFrame1),
            (GetPartBodyPositionData(0f, 0.27f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        #endregion
        #region idSchool: 3, idPartBody: 1, Category: Stand, Label: StandBack
        bodyDatas.Add((3, 1, Category.Stand, Label.StandBackFrame0),
            (GetPartBodyPositionData(-0.00079f, 0.751f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        bodyDatas.Add((3, 1, Category.Stand, Label.StandBackFrame1),
            (GetPartBodyPositionData(-0.00079f, 0.781f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        #endregion
        #region idSchool: 3, idPartBody: 2, Category: Stand, Label: StandBack
        bodyDatas.Add((3, 2, Category.Stand, Label.StandBackFrame0),
            (GetPartBodyPositionData(-0.00179f, 1.494f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        bodyDatas.Add((3, 2, Category.Stand, Label.StandBackFrame1),
            (GetPartBodyPositionData(-0.00179f, 1.524f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        #endregion
        #region idSchool: 3, idPartBody: 3, Category: Stand, Label: StandBack
        bodyDatas.Add((3, 3, Category.Stand, Label.StandBackFrame0),
            (GetPartBodyPositionData(0.00121f, 2.054f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        bodyDatas.Add((3, 3, Category.Stand, Label.StandBackFrame1),
            (GetPartBodyPositionData(0.00121f, 2.084f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        #endregion
        #region idSchool: 3, idPartBody: 4, Category: Stand, Label: StandBack
        bodyDatas.Add((3, 4, Category.Stand, Label.StandBackFrame0),
            (GetPartBodyPositionData(0f, 0f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 0f)));
        bodyDatas.Add((3, 4, Category.Stand, Label.StandBackFrame1),
            (GetPartBodyPositionData(0f, 0f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 0f)));
        #endregion
        #region idSchool: 3, idPartBody: 5, Category: Stand, Label: StandBack
        bodyDatas.Add((3, 5, Category.Stand, Label.StandBackFrame0),
            (GetPartBodyPositionData(0.03121f, 1.493f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        bodyDatas.Add((3, 5, Category.Stand, Label.StandBackFrame1),
            (GetPartBodyPositionData(0.03121f, 1.523f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        #endregion
        #region idSchool: 3, idPartBody: 6, Category: Stand, Label: StandBack
        bodyDatas.Add((3, 6, Category.Stand, Label.StandBackFrame0),
            (GetPartBodyPositionData(0f, 0f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 0f)));
        bodyDatas.Add((3, 6, Category.Stand, Label.StandBackFrame1),
            (GetPartBodyPositionData(0f, 0f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 0f)));
        #endregion
        #region idSchool: 3, idPartBody: 7, Category: Stand, Label: StandBack
        bodyDatas.Add((3, 7, Category.Stand, Label.StandBackFrame0),
            (GetPartBodyPositionData(0.12471f, 0.914f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        bodyDatas.Add((3, 7, Category.Stand, Label.StandBackFrame1),
            (GetPartBodyPositionData(0.12471f, 0.944f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        #endregion

        #region idSchool: 3, idPartBody: 0, Category: Stand, Label: StandLeft
        bodyDatas.Add((3, 0, Category.Stand, Label.StandLeftFrame0),
            (GetPartBodyPositionData(0f, 0.27f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        bodyDatas.Add((3, 0, Category.Stand, Label.StandLeftFrame1),
            (GetPartBodyPositionData(0f, 0.27f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        #endregion
        #region idSchool: 3, idPartBody: 1, Category: Stand, Label: StandLeft
        bodyDatas.Add((3, 1, Category.Stand, Label.StandLeftFrame0),
            (GetPartBodyPositionData(-0.03179f, 0.664f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        bodyDatas.Add((3, 1, Category.Stand, Label.StandLeftFrame1),
            (GetPartBodyPositionData(-0.03179f, 0.694f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        #endregion
        #region idSchool: 3, idPartBody: 2, Category: Stand, Label: StandLeft
        bodyDatas.Add((3, 2, Category.Stand, Label.StandLeftFrame0),
            (GetPartBodyPositionData(-0.00179f, 1.494f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        bodyDatas.Add((3, 2, Category.Stand, Label.StandLeftFrame1),
            (GetPartBodyPositionData(-0.00179f, 1.524f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        #endregion
        #region idSchool: 3, idPartBody: 3, Category: Stand, Label: StandLeft
        bodyDatas.Add((3, 3, Category.Stand, Label.StandLeftFrame0),
            (GetPartBodyPositionData(0.01821f, 2.054f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        bodyDatas.Add((3, 3, Category.Stand, Label.StandLeftFrame1),
            (GetPartBodyPositionData(0.01821f, 2.084f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        #endregion
        #region idSchool: 3, idPartBody: 4, Category: Stand, Label: StandLeft
        bodyDatas.Add((3, 4, Category.Stand, Label.StandLeftFrame0),
            (GetPartBodyPositionData(-0.13879f, 1.355f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        bodyDatas.Add((3, 4, Category.Stand, Label.StandLeftFrame1),
            (GetPartBodyPositionData(-0.201f, 1.385f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        #endregion
        #region idSchool: 3, idPartBody: 5, Category: Stand, Label: StandLeft
        bodyDatas.Add((3, 5, Category.Stand, Label.StandLeftFrame0),
            (GetPartBodyPositionData(-0.01379f, 1.493f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        bodyDatas.Add((3, 5, Category.Stand, Label.StandLeftFrame1),
            (GetPartBodyPositionData(-0.01379f, 1.523f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        #endregion
        #region idSchool: 3, idPartBody: 6, Category: Stand, Label: StandLeft
        bodyDatas.Add((3, 6, Category.Stand, Label.StandLeftFrame0),
            (GetPartBodyPositionData(-0.06879f, 0.914f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        bodyDatas.Add((3, 6, Category.Stand, Label.StandLeftFrame1),
            (GetPartBodyPositionData(-0.06879f, 0.944f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        #endregion
        #region idSchool: 3, idPartBody: 7, Category: Stand, Label: StandLeft
        bodyDatas.Add((3, 7, Category.Stand, Label.StandLeftFrame0),
            (GetPartBodyPositionData(0f, 0f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 0f)));
        bodyDatas.Add((3, 7, Category.Stand, Label.StandLeftFrame1),
            (GetPartBodyPositionData(0f, 0f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 0f)));
        #endregion

        #region idSchool: 3, idPartBody: 0, Category: Stand, Label: StandRight
        bodyDatas.Add((3, 0, Category.Stand, Label.StandRightFrame0),
            (GetPartBodyPositionData(0f, 0.27f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        bodyDatas.Add((3, 0, Category.Stand, Label.StandRightFrame1),
            (GetPartBodyPositionData(0f, 0.27f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        #endregion
        #region idSchool: 3, idPartBody: 1, Category: Stand, Label: StandRight
        bodyDatas.Add((3, 1, Category.Stand, Label.StandRightFrame0),
            (GetPartBodyPositionData(-0.03179f, 0.664f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        bodyDatas.Add((3, 1, Category.Stand, Label.StandRightFrame1),
            (GetPartBodyPositionData(-0.03179f, 0.694f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        #endregion
        #region idSchool: 3, idPartBody: 2, Category: Stand, Label: StandRight
        bodyDatas.Add((3, 2, Category.Stand, Label.StandRightFrame0),
            (GetPartBodyPositionData(-0.00179f, 1.494f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        bodyDatas.Add((3, 2, Category.Stand, Label.StandRightFrame1),
            (GetPartBodyPositionData(-0.00179f, 1.524f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        #endregion
        #region idSchool: 3, idPartBody: 3, Category: Stand, Label: StandRight
        bodyDatas.Add((3, 3, Category.Stand, Label.StandRightFrame0),
            (GetPartBodyPositionData(0.01821f, 2.054f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        bodyDatas.Add((3, 3, Category.Stand, Label.StandRightFrame1),
            (GetPartBodyPositionData(0.01821f, 2.084f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        #endregion
        #region idSchool: 3, idPartBody: 4, Category: Stand, Label: StandRight
        bodyDatas.Add((3, 4, Category.Stand, Label.StandRightFrame0),
            (GetPartBodyPositionData(-0.13879f, 1.355f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        bodyDatas.Add((3, 4, Category.Stand, Label.StandRightFrame1),
            (GetPartBodyPositionData(-0.201f, 1.385f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        #endregion
        #region idSchool: 3, idPartBody: 5, Category: Stand, Label: StandRight
        bodyDatas.Add((3, 5, Category.Stand, Label.StandRightFrame0),
            (GetPartBodyPositionData(-0.01379f, 1.493f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        bodyDatas.Add((3, 5, Category.Stand, Label.StandRightFrame1),
            (GetPartBodyPositionData(-0.01379f, 1.523f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        #endregion
        #region idSchool: 3, idPartBody: 6, Category: Stand, Label: StandRight
        bodyDatas.Add((3, 6, Category.Stand, Label.StandRightFrame0),
            (GetPartBodyPositionData(-0.06879f, 0.914f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        bodyDatas.Add((3, 6, Category.Stand, Label.StandRightFrame1),
            (GetPartBodyPositionData(-0.06879f, 0.944f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        #endregion
        #region idSchool: 3, idPartBody: 7, Category: Stand, Label: StandRight
        bodyDatas.Add((3, 7, Category.Stand, Label.StandRightFrame0),
            (GetPartBodyPositionData(0f, 0f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 0f)));
        bodyDatas.Add((3, 7, Category.Stand, Label.StandRightFrame1),
            (GetPartBodyPositionData(0f, 0f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 0f)));
        #endregion

        #region idSchool: 3, idPartBody: 0, Category: Move, Label: MoveFront
        bodyDatas.Add((3, 0, Category.Move, Label.MoveFrontFrame0),
            (GetPartBodyPositionData(-0.032f, 0.27f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        bodyDatas.Add((3, 0, Category.Move, Label.MoveFrontFrame1),
            (GetPartBodyPositionData(0.031f, 0.27f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        #endregion
        #region idSchool: 3, idPartBody: 1, Category: Move, Label: MoveFront
        bodyDatas.Add((3, 1, Category.Move, Label.MoveFrontFrame0),
            (GetPartBodyPositionData(-0.031f, 0.739f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        bodyDatas.Add((3, 1, Category.Move, Label.MoveFrontFrame1),
            (GetPartBodyPositionData(0.032f, 0.739f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        #endregion
        #region idSchool: 3, idPartBody: 2, Category: Move, Label: MoveFront
        bodyDatas.Add((3, 2, Category.Move, Label.MoveFrontFrame0),
            (GetPartBodyPositionData(0.001f, 1.494f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        bodyDatas.Add((3, 2, Category.Move, Label.MoveFrontFrame1),
            (GetPartBodyPositionData(0.001f, 1.494f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        #endregion
        #region idSchool: 3, idPartBody: 3, Category: Move, Label: MoveFront
        bodyDatas.Add((3, 3, Category.Move, Label.MoveFrontFrame0),
            (GetPartBodyPositionData(0.004f, 2.054f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        bodyDatas.Add((3, 3, Category.Move, Label.MoveFrontFrame1),
            (GetPartBodyPositionData(0.004f, 2.054f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        #endregion
        #region idSchool: 3, idPartBody: 4, Category: Move, Label: MoveFront
        bodyDatas.Add((3, 4, Category.Move, Label.MoveFrontFrame0),
            (GetPartBodyPositionData(-0.03179f, 1.355f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        bodyDatas.Add((3, 4, Category.Move, Label.MoveFrontFrame1),
            (GetPartBodyPositionData(-0.03179f, 1.355f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        #endregion
        #region idSchool: 3, idPartBody: 5, Category: Move, Label: MoveFront
        bodyDatas.Add((3, 5, Category.Move, Label.MoveFrontFrame0),
            (GetPartBodyPositionData(-0.031f, 1.493f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        bodyDatas.Add((3, 5, Category.Move, Label.MoveFrontFrame1),
            (GetPartBodyPositionData(-0.031f, 1.493f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        #endregion
        #region idSchool: 3, idPartBody: 6, Category: Move, Label: MoveFront
        bodyDatas.Add((3, 6, Category.Move, Label.MoveFrontFrame0),
            (GetPartBodyPositionData(-0.066f, 0.914f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        bodyDatas.Add((3, 6, Category.Move, Label.MoveFrontFrame1),
            (GetPartBodyPositionData(-0.066f, 0.914f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        #endregion
        #region idSchool: 3, idPartBody: 7, Category: Move, Label: MoveFront
        bodyDatas.Add((3, 7, Category.Move, Label.MoveFrontFrame0),
            (GetPartBodyPositionData(0f, 0f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 0f)));
        bodyDatas.Add((3, 7, Category.Move, Label.MoveFrontFrame1),
            (GetPartBodyPositionData(0f, 0f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 0f)));
        #endregion

        #region idSchool: 3, idPartBody: 0, Category: Move, Label: MoveBack
        bodyDatas.Add((3, 0, Category.Move, Label.MoveBackFrame0),
            (GetPartBodyPositionData(-0.03f, 0.27f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        bodyDatas.Add((3, 0, Category.Move, Label.MoveBackFrame1),
            (GetPartBodyPositionData(0.033f, 0.27f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        #endregion
        #region idSchool: 3, idPartBody: 1, Category: Move, Label: MoveBack
        bodyDatas.Add((3, 1, Category.Move, Label.MoveBackFrame0),
            (GetPartBodyPositionData(0.0325f, 0.739f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        bodyDatas.Add((3, 1, Category.Move, Label.MoveBackFrame1),
            (GetPartBodyPositionData(-0.0293f, 0.739f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        #endregion
        #region idSchool: 3, idPartBody: 2, Category: Move, Label: MoveBack
        bodyDatas.Add((3, 2, Category.Move, Label.MoveBackFrame0),
            (GetPartBodyPositionData(0.003f, 1.494f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        bodyDatas.Add((3, 2, Category.Move, Label.MoveBackFrame1),
            (GetPartBodyPositionData(0.003f, 1.494f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        #endregion
        #region idSchool: 3, idPartBody: 3, Category: Move, Label: MoveBack
        bodyDatas.Add((3, 3, Category.Move, Label.MoveBackFrame0),
            (GetPartBodyPositionData(0.006f, 2.054f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        bodyDatas.Add((3, 3, Category.Move, Label.MoveBackFrame1),
            (GetPartBodyPositionData(0.006f, 2.054f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        #endregion
        #region idSchool: 3, idPartBody: 4, Category: Move, Label: MoveBack
        bodyDatas.Add((3, 4, Category.Move, Label.MoveBackFrame0),
            (GetPartBodyPositionData(0f, 0f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 0f)));
        bodyDatas.Add((3, 4, Category.Move, Label.MoveBackFrame1),
            (GetPartBodyPositionData(0f, 0f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 0f)));
        #endregion
        #region idSchool: 3, idPartBody: 5, Category: Move, Label: MoveBack
        bodyDatas.Add((3, 5, Category.Move, Label.MoveBackFrame0),
            (GetPartBodyPositionData(0.036f, 1.493f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        bodyDatas.Add((3, 5, Category.Move, Label.MoveBackFrame1),
            (GetPartBodyPositionData(0.036f, 1.493f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        #endregion
        #region idSchool: 3, idPartBody: 6, Category: Move, Label: MoveBack
        bodyDatas.Add((3, 6, Category.Move, Label.MoveBackFrame0),
            (GetPartBodyPositionData(0f, 0f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 0f)));
        bodyDatas.Add((3, 6, Category.Move, Label.MoveBackFrame1),
            (GetPartBodyPositionData(0f, 0f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 0f)));
        #endregion
        #region idSchool: 3, idPartBody: 7, Category: Move, Label: MoveBack
        bodyDatas.Add((3, 7, Category.Move, Label.MoveBackFrame0),
            (GetPartBodyPositionData(0.1295f, 0.914f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        bodyDatas.Add((3, 7, Category.Move, Label.MoveBackFrame1),
            (GetPartBodyPositionData(0.1295f, 0.914f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        #endregion

        #region idSchool: 3, idPartBody: 0, Category: Move, Label: MoveLeft
        bodyDatas.Add((3, 0, Category.Move, Label.MoveLeftFrame0),
            (GetPartBodyPositionData(-0.063f, 0.288f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        bodyDatas.Add((3, 0, Category.Move, Label.MoveLeftFrame1),
            (GetPartBodyPositionData(-0.063f, 0.288f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        #endregion
        #region idSchool: 3, idPartBody: 1, Category: Move, Label: MoveLeft
        bodyDatas.Add((3, 1, Category.Move, Label.MoveLeftFrame0),
            (GetPartBodyPositionData(-0.063f, 0.664f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        bodyDatas.Add((3, 1, Category.Move, Label.MoveLeftFrame1),
            (GetPartBodyPositionData(0f, 0.664f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        #endregion
        #region idSchool: 3, idPartBody: 2, Category: Move, Label: MoveLeft
        bodyDatas.Add((3, 2, Category.Move, Label.MoveLeftFrame0),
            (GetPartBodyPositionData(-0.00179f, 1.494f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        bodyDatas.Add((3, 2, Category.Move, Label.MoveLeftFrame1),
            (GetPartBodyPositionData(-0.00179f, 1.494f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        #endregion
        #region idSchool: 3, idPartBody: 3, Category: Move, Label: MoveLeft
        bodyDatas.Add((3, 3, Category.Move, Label.MoveLeftFrame0),
            (GetPartBodyPositionData(0.01821f, 2.054f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        bodyDatas.Add((3, 3, Category.Move, Label.MoveLeftFrame1),
            (GetPartBodyPositionData(0.01821f, 2.054f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        #endregion
        #region idSchool: 3, idPartBody: 4, Category: Move, Label: MoveLeft
        bodyDatas.Add((3, 4, Category.Move, Label.MoveLeftFrame0),
            (GetPartBodyPositionData(-0.13879f, 1.355f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        bodyDatas.Add((3, 4, Category.Move, Label.MoveLeftFrame1),
            (GetPartBodyPositionData(-0.20129f, 1.355f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        #endregion
        #region idSchool: 3, idPartBody: 5, Category: Move, Label: MoveLeft
        bodyDatas.Add((3, 5, Category.Move, Label.MoveLeftFrame0),
            (GetPartBodyPositionData(-0.01379f, 1.493f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        bodyDatas.Add((3, 5, Category.Move, Label.MoveLeftFrame1),
            (GetPartBodyPositionData(-0.01379f, 1.493f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        #endregion
        #region idSchool: 3, idPartBody: 6, Category: Move, Label: MoveLeft
        bodyDatas.Add((3, 6, Category.Move, Label.MoveLeftFrame0),
            (GetPartBodyPositionData(-0.06879f, 0.914f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        bodyDatas.Add((3, 6, Category.Move, Label.MoveLeftFrame1),
            (GetPartBodyPositionData(-0.06879f, 0.914f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        #endregion
        #region idSchool: 3, idPartBody: 7, Category: Move, Label: MoveLeft
        bodyDatas.Add((3, 7, Category.Move, Label.MoveLeftFrame0),
            (GetPartBodyPositionData(0f, 0f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 0f)));
        bodyDatas.Add((3, 7, Category.Move, Label.MoveLeftFrame1),
            (GetPartBodyPositionData(0f, 0f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 0f)));
        #endregion

        #region idSchool: 3, idPartBody: 0, Category: Move, Label: MoveRight
        bodyDatas.Add((3, 0, Category.Move, Label.MoveRightFrame0),
            (GetPartBodyPositionData(-0.063f, 0.288f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        bodyDatas.Add((3, 0, Category.Move, Label.MoveRightFrame1),
            (GetPartBodyPositionData(-0.063f, 0.288f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        #endregion
        #region idSchool: 3, idPartBody: 1, Category: Move, Label: MoveRight
        bodyDatas.Add((3, 1, Category.Move, Label.MoveRightFrame0),
            (GetPartBodyPositionData(-0.063f, 0.664f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        bodyDatas.Add((3, 1, Category.Move, Label.MoveRightFrame1),
            (GetPartBodyPositionData(0f, 0.664f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        #endregion
        #region idSchool: 3, idPartBody: 2, Category: Move, Label: MoveRight
        bodyDatas.Add((3, 2, Category.Move, Label.MoveRightFrame0),
            (GetPartBodyPositionData(-0.00179f, 1.494f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        bodyDatas.Add((3, 2, Category.Move, Label.MoveRightFrame1),
            (GetPartBodyPositionData(-0.00179f, 1.494f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        #endregion
        #region idSchool: 3, idPartBody: 3, Category: Move, Label: MoveRight
        bodyDatas.Add((3, 3, Category.Move, Label.MoveRightFrame0),
            (GetPartBodyPositionData(0.01821f, 2.054f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        bodyDatas.Add((3, 3, Category.Move, Label.MoveRightFrame1),
            (GetPartBodyPositionData(0.01821f, 2.054f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        #endregion
        #region idSchool: 3, idPartBody: 4, Category: Move, Label: MoveRight
        bodyDatas.Add((3, 4, Category.Move, Label.MoveRightFrame0),
            (GetPartBodyPositionData(-0.13879f, 1.355f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        bodyDatas.Add((3, 4, Category.Move, Label.MoveRightFrame1),
            (GetPartBodyPositionData(-0.20129f, 1.355f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        #endregion
        #region idSchool: 3, idPartBody: 5, Category: Move, Label: MoveRight
        bodyDatas.Add((3, 5, Category.Move, Label.MoveRightFrame0),
            (GetPartBodyPositionData(-0.01379f, 1.493f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        bodyDatas.Add((3, 5, Category.Move, Label.MoveRightFrame1),
            (GetPartBodyPositionData(-0.01379f, 1.493f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        #endregion
        #region idSchool: 3, idPartBody: 6, Category: Move, Label: MoveRight
        bodyDatas.Add((3, 6, Category.Move, Label.MoveRightFrame0),
            (GetPartBodyPositionData(-0.06879f, 0.914f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        bodyDatas.Add((3, 6, Category.Move, Label.MoveRightFrame1),
            (GetPartBodyPositionData(-0.06879f, 0.914f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        #endregion
        #region idSchool: 3, idPartBody: 7, Category: Move, Label: MoveRight
        bodyDatas.Add((3, 7, Category.Move, Label.MoveRightFrame0),
            (GetPartBodyPositionData(0f, 0f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 0f)));
        bodyDatas.Add((3, 7, Category.Move, Label.MoveRightFrame1),
            (GetPartBodyPositionData(0f, 0f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 0f)));
        #endregion

        #region idSchool: 3, idPartBody: 0, Category: Atk, Label: AtkFront
        bodyDatas.Add((3, 0, Category.Atk, Label.AtkFrontFrame0),
            (GetPartBodyPositionData(0f, 0.27f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        bodyDatas.Add((3, 0, Category.Atk, Label.AtkFrontFrame1),
            (GetPartBodyPositionData(0f, 0.27f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        #endregion
        #region idSchool: 3, idPartBody: 1, Category: Atk, Label: AtkFront
        bodyDatas.Add((3, 1, Category.Atk, Label.AtkFrontFrame0),
            (GetPartBodyPositionData(0.0315f, 0.864f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        bodyDatas.Add((3, 1, Category.Atk, Label.AtkFrontFrame1),
            (GetPartBodyPositionData(0.125f, 0.708f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        #endregion
        #region idSchool: 3, idPartBody: 2, Category: Atk, Label: AtkFront
        bodyDatas.Add((3, 2, Category.Atk, Label.AtkFrontFrame0),
            (GetPartBodyPositionData(0f, 1.494f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        bodyDatas.Add((3, 2, Category.Atk, Label.AtkFrontFrame1),
            (GetPartBodyPositionData(0f, 1.494f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        #endregion
        #region idSchool: 3, idPartBody: 3, Category: Atk, Label: AtkFront
        bodyDatas.Add((3, 3, Category.Atk, Label.AtkFrontFrame0),
            (GetPartBodyPositionData(0.003f, 2.054f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        bodyDatas.Add((3, 3, Category.Atk, Label.AtkFrontFrame1),
            (GetPartBodyPositionData(0.003f, 2.054f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        #endregion
        #region idSchool: 3, idPartBody: 4, Category: Atk, Label: AtkFront
        bodyDatas.Add((3, 4, Category.Atk, Label.AtkFrontFrame0),
            (GetPartBodyPositionData(0f, 1.364f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        bodyDatas.Add((3, 4, Category.Atk, Label.AtkFrontFrame1),
            (GetPartBodyPositionData(0f, 1.299f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        #endregion
        #region idSchool: 3, idPartBody: 5, Category: Atk, Label: AtkFront
        bodyDatas.Add((3, 5, Category.Atk, Label.AtkFrontFrame0),
            (GetPartBodyPositionData(-0.032f, 1.493f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        bodyDatas.Add((3, 5, Category.Atk, Label.AtkFrontFrame1),
            (GetPartBodyPositionData(-0.032f, 1.493f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        #endregion
        #region idSchool: 3, idPartBody: 6, Category: Atk, Label: AtkFront
        bodyDatas.Add((3, 6, Category.Atk, Label.AtkFrontFrame0),
            (GetPartBodyPositionData(-0.517f, 1.364f, 0f), GetPartBodyRotationData(0f, 0f, -45f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        bodyDatas.Add((3, 6, Category.Atk, Label.AtkFrontFrame1),
            (GetPartBodyPositionData(-0.517f, 2.228f, 0f), GetPartBodyRotationData(0f, 0f, -45f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 0f)));
        #endregion
        #region idSchool: 3, idPartBody: 7, Category: Atk, Label: AtkFront
        bodyDatas.Add((3, 7, Category.Atk, Label.AtkFrontFrame0),
            (GetPartBodyPositionData(0.713f, 0.864f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 0f)));
        bodyDatas.Add((3, 7, Category.Atk, Label.AtkFrontFrame1),
            (GetPartBodyPositionData(-0.25f, 0.264f, 0f), GetPartBodyRotationData(0f, 0f, -130f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        #endregion

        #region idSchool: 3, idPartBody: 0, Category: Atk, Label: AtkBack
        bodyDatas.Add((3, 0, Category.Atk, Label.AtkBackFrame0),
            (GetPartBodyPositionData(0f, 0.27f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        bodyDatas.Add((3, 0, Category.Atk, Label.AtkBackFrame1),
            (GetPartBodyPositionData(0f, 0.27f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        #endregion
        #region idSchool: 3, idPartBody: 1, Category: Atk, Label: AtkBack
        bodyDatas.Add((3, 1, Category.Atk, Label.AtkBackFrame0),
            (GetPartBodyPositionData(-0.0625f, 0.869f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        bodyDatas.Add((3, 1, Category.Atk, Label.AtkBackFrame1),
            (GetPartBodyPositionData(-0.1236f, 0.775f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        #endregion
        #region idSchool: 3, idPartBody: 2, Category: Atk, Label: AtkBack
        bodyDatas.Add((3, 2, Category.Atk, Label.AtkBackFrame0),
            (GetPartBodyPositionData(0f, 1.494f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        bodyDatas.Add((3, 2, Category.Atk, Label.AtkBackFrame1),
            (GetPartBodyPositionData(0f, 1.494f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        #endregion
        #region idSchool: 3, idPartBody: 3, Category: Atk, Label: AtkBack
        bodyDatas.Add((3, 3, Category.Atk, Label.AtkBackFrame0),
            (GetPartBodyPositionData(0.003f, 2.054f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        bodyDatas.Add((3, 3, Category.Atk, Label.AtkBackFrame1),
            (GetPartBodyPositionData(0.003f, 2.054f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        #endregion
        #region idSchool: 3, idPartBody: 4, Category: Atk, Label: AtkBack
        bodyDatas.Add((3, 4, Category.Atk, Label.AtkBackFrame0),
            (GetPartBodyPositionData(0f, 0f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 0f)));
        bodyDatas.Add((3, 4, Category.Atk, Label.AtkBackFrame1),
            (GetPartBodyPositionData(0f, 0f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 0f)));
        #endregion
        #region idSchool: 3, idPartBody: 5, Category: Atk, Label: AtkBack
        bodyDatas.Add((3, 5, Category.Atk, Label.AtkBackFrame0),
            (GetPartBodyPositionData(0.033f, 1.493f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        bodyDatas.Add((3, 5, Category.Atk, Label.AtkBackFrame1),
            (GetPartBodyPositionData(0.033f, 1.493f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        #endregion
        #region idSchool: 3, idPartBody: 6, Category: Atk, Label: AtkBack
        bodyDatas.Add((3, 6, Category.Atk, Label.AtkBackFrame0),
            (GetPartBodyPositionData(-0.587f, 1.664f, 0f), GetPartBodyRotationData(0f, 0f, 40f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 0f)));
        bodyDatas.Add((3, 6, Category.Atk, Label.AtkBackFrame1),
            (GetPartBodyPositionData(0.39f, 1.344f, 0f), GetPartBodyRotationData(180f, 0f, 130f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        #endregion
        #region idSchool: 3, idPartBody: 7, Category: Atk, Label: AtkBack
        bodyDatas.Add((3, 7, Category.Atk, Label.AtkBackFrame0),
            (GetPartBodyPositionData(0.49f, 0.704f, 0f), GetPartBodyRotationData(180f, 0f, 45f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        bodyDatas.Add((3, 7, Category.Atk, Label.AtkBackFrame1),
            (GetPartBodyPositionData(-0.137f, 0.074f, 0f), GetPartBodyRotationData(180f, 0f, 145f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 0f)));
        #endregion

        #region idSchool: 3, idPartBody: 0, Category: Atk, Label: AtkLeft
        bodyDatas.Add((3, 0, Category.Atk, Label.AtkLeftFrame0),
            (GetPartBodyPositionData(0f, 0.27f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        bodyDatas.Add((3, 0, Category.Atk, Label.AtkLeftFrame1),
            (GetPartBodyPositionData(0f, 0.27f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        #endregion
        #region idSchool: 3, idPartBody: 1, Category: Atk, Label: AtkLeft
        bodyDatas.Add((3, 1, Category.Atk, Label.AtkLeftFrame0),
            (GetPartBodyPositionData(0.125f, 0.744f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        bodyDatas.Add((3, 1, Category.Atk, Label.AtkLeftFrame1),
            (GetPartBodyPositionData(0.157f, 0.71f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        #endregion
        #region idSchool: 3, idPartBody: 2, Category: Atk, Label: AtkLeft
        bodyDatas.Add((3, 2, Category.Atk, Label.AtkLeftFrame0),
            (GetPartBodyPositionData(0.094f, 1.494f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        bodyDatas.Add((3, 2, Category.Atk, Label.AtkLeftFrame1),
            (GetPartBodyPositionData(0.094f, 1.494f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        #endregion
        #region idSchool: 3, idPartBody: 3, Category: Atk, Label: AtkLeft
        bodyDatas.Add((3, 3, Category.Atk, Label.AtkLeftFrame0),
            (GetPartBodyPositionData(0.114f, 2.054f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        bodyDatas.Add((3, 3, Category.Atk, Label.AtkLeftFrame1),
            (GetPartBodyPositionData(0.114f, 2.054f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        #endregion
        #region idSchool: 3, idPartBody: 4, Category: Atk, Label: AtkLeft
        bodyDatas.Add((3, 4, Category.Atk, Label.AtkLeftFrame0),
            (GetPartBodyPositionData(-0.043f, 1.355f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        bodyDatas.Add((3, 4, Category.Atk, Label.AtkLeftFrame1),
            (GetPartBodyPositionData(-0.1055f, 1.293f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        #endregion
        #region idSchool: 3, idPartBody: 5, Category: Atk, Label: AtkLeft
        bodyDatas.Add((3, 5, Category.Atk, Label.AtkLeftFrame0),
            (GetPartBodyPositionData(0.082f, 1.493f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        bodyDatas.Add((3, 5, Category.Atk, Label.AtkLeftFrame1),
            (GetPartBodyPositionData(0.082f, 1.493f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        #endregion
        #region idSchool: 3, idPartBody: 6, Category: Atk, Label: AtkLeft
        bodyDatas.Add((3, 6, Category.Atk, Label.AtkLeftFrame0),
            (GetPartBodyPositionData(0.5f, 1.164f, 0f), GetPartBodyRotationData(0f, 0f, -80f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        bodyDatas.Add((3, 6, Category.Atk, Label.AtkLeftFrame1),
            (GetPartBodyPositionData(0.5f, 1.164f, 0f), GetPartBodyRotationData(0f, 0f, -80f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 0f)));
        #endregion
        #region idSchool: 3, idPartBody: 7, Category: Atk, Label: AtkLeft
        bodyDatas.Add((3, 7, Category.Atk, Label.AtkLeftFrame0),
            (GetPartBodyPositionData(0.037f, 0.864f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 0f)));
        bodyDatas.Add((3, 7, Category.Atk, Label.AtkLeftFrame1),
            (GetPartBodyPositionData(-0.55f, 0.694f, 0f), GetPartBodyRotationData(180f, 0f, 130f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        #endregion

        #region idSchool: 3, idPartBody: 0, Category: Atk, Label: AtkRight
        bodyDatas.Add((3, 0, Category.Atk, Label.AtkRightFrame0),
            (GetPartBodyPositionData(0f, 0.27f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        bodyDatas.Add((3, 0, Category.Atk, Label.AtkRightFrame1),
            (GetPartBodyPositionData(0f, 0.27f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        #endregion
        #region idSchool: 3, idPartBody: 1, Category: Atk, Label: AtkRight
        bodyDatas.Add((3, 1, Category.Atk, Label.AtkRightFrame0),
            (GetPartBodyPositionData(0.125f, 0.744f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        bodyDatas.Add((3, 1, Category.Atk, Label.AtkRightFrame1),
            (GetPartBodyPositionData(0.157f, 0.71f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        #endregion
        #region idSchool: 3, idPartBody: 2, Category: Atk, Label: AtkRight
        bodyDatas.Add((3, 2, Category.Atk, Label.AtkRightFrame0),
            (GetPartBodyPositionData(0.094f, 1.494f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        bodyDatas.Add((3, 2, Category.Atk, Label.AtkRightFrame1),
            (GetPartBodyPositionData(0.094f, 1.494f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        #endregion
        #region idSchool: 3, idPartBody: 3, Category: Atk, Label: AtkRight
        bodyDatas.Add((3, 3, Category.Atk, Label.AtkRightFrame0),
            (GetPartBodyPositionData(0.114f, 2.054f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        bodyDatas.Add((3, 3, Category.Atk, Label.AtkRightFrame1),
            (GetPartBodyPositionData(0.114f, 2.054f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        #endregion
        #region idSchool: 3, idPartBody: 4, Category: Atk, Label: AtkRight
        bodyDatas.Add((3, 4, Category.Atk, Label.AtkRightFrame0),
            (GetPartBodyPositionData(-0.043f, 1.355f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        bodyDatas.Add((3, 4, Category.Atk, Label.AtkRightFrame1),
            (GetPartBodyPositionData(-0.1055f, 1.293f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        #endregion
        #region idSchool: 3, idPartBody: 5, Category: Atk, Label: AtkRight
        bodyDatas.Add((3, 5, Category.Atk, Label.AtkRightFrame0),
            (GetPartBodyPositionData(0.082f, 1.493f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        bodyDatas.Add((3, 5, Category.Atk, Label.AtkRightFrame1),
            (GetPartBodyPositionData(0.082f, 1.493f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        #endregion
        #region idSchool: 3, idPartBody: 6, Category: Atk, Label: AtkRight
        bodyDatas.Add((3, 6, Category.Atk, Label.AtkRightFrame0),
            (GetPartBodyPositionData(0.5f, 1.164f, 0f), GetPartBodyRotationData(0f, 0f, -80f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        bodyDatas.Add((3, 6, Category.Atk, Label.AtkRightFrame1),
            (GetPartBodyPositionData(0.5f, 1.164f, 0f), GetPartBodyRotationData(0f, 0f, -80f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 0f)));
        #endregion
        #region idSchool: 3, idPartBody: 7, Category: Atk, Label: AtkRight
        bodyDatas.Add((3, 7, Category.Atk, Label.AtkRightFrame0),
            (GetPartBodyPositionData(0.037f, 0.864f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 0f)));
        bodyDatas.Add((3, 7, Category.Atk, Label.AtkRightFrame1),
            (GetPartBodyPositionData(-0.55f, 0.694f, 0f), GetPartBodyRotationData(180f, 0f, 130f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        #endregion

        #region idSchool: 3, idPartBody: 0, Category: Die, Label: DieFrame0
        bodyDatas.Add((3, 0, Category.Die, Label.DieFrame0),
            (GetPartBodyPositionData(0f, 0f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 0f)));
        #endregion
        #region idSchool: 3, idPartBody: 1, Category: Die, Label: DieFrame0
        bodyDatas.Add((3, 1, Category.Die, Label.DieFrame0),
            (GetPartBodyPositionData(-0.033f, 0.45f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        #endregion
        #region idSchool: 3, idPartBody: 2, Category: Die, Label: DieFrame0
        bodyDatas.Add((3, 2, Category.Die, Label.DieFrame0),
            (GetPartBodyPositionData(-0.00179f, 1.494f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        #endregion
        #region idSchool: 3, idPartBody: 3, Category: Die, Label: DieFrame0
        bodyDatas.Add((3, 3, Category.Die, Label.DieFrame0),
            (GetPartBodyPositionData(0f, 0f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 0f)));
        #endregion
        #region idSchool: 3, idPartBody: 4, Category: Die, Label: DieFrame0
        bodyDatas.Add((3, 4, Category.Die, Label.DieFrame0),
            (GetPartBodyPositionData(-0.00079f, 1.159f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        #endregion
        #region idSchool: 3, idPartBody: 5, Category: Die, Label: DieFrame0
        bodyDatas.Add((3, 5, Category.Die, Label.DieFrame0),
            (GetPartBodyPositionData(-0.03379f, 1.493f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        #endregion
        #region idSchool: 3, idPartBody: 6, Category: Die, Label: DieFrame0
        bodyDatas.Add((3, 6, Category.Die, Label.DieFrame0),
            (GetPartBodyPositionData(0f, 0f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 0f)));
        #endregion
        #region idSchool: 3, idPartBody: 7, Category: Die, Label: DieFrame0
        bodyDatas.Add((3, 7, Category.Die, Label.DieFrame0),
            (GetPartBodyPositionData(0f, 0f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 0f)));
        #endregion
        #endregion
    }
}