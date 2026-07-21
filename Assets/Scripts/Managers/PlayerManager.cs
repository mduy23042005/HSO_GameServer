using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour, IUpdatable
{
    [SerializeField] private List<GameObject> playerPrefab;

    // Key: (idSchool, idPartBody, Category, Label)
    public static Dictionary<(int, int, Category, Label), (PositionData, RotationData, ScaleData, ColorData)> bodyDatas;

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

    public void InitPartBodyData()
    {
        // Key: (idSchool, idPartBody, frame, Category, Label)
        bodyDatas = new Dictionary<(int, int, Category, Label), (PositionData, RotationData, ScaleData, ColorData)>();

        #region idSchool: 1, idPartBody: 0, Category: Stand, Label: StandFront
        bodyDatas.Add((1, 0, Category.Stand, Label.StandFront),
            (GetPartBodyPositionData(0f, 0.27f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        bodyDatas.Add((1, 0, Category.Stand, Label.StandFront), 
            (GetPartBodyPositionData(0f, 0.27f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        #endregion
        #region idSchool: 1, idPartBody: 1, Category: Stand, Label: StandFront
        bodyDatas.Add((1, 1, Category.Stand, Label.StandFront), 
            (GetPartBodyPositionData(0.00974f, 0.689f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        bodyDatas.Add((1, 1, Category.Stand, Label.StandFront), 
            (GetPartBodyPositionData(0.00974f, 0.719f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        #endregion
        #region idSchool: 1, idPartBody: 2, Category: Stand, Label: StandFront
        bodyDatas.Add((1, 2, Category.Stand, Label.StandFront), 
            (GetPartBodyPositionData(0.01774f, 1.494f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        bodyDatas.Add((1, 2, Category.Stand, Label.StandFront), 
            (GetPartBodyPositionData(0.01774f, 1.524f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        #endregion
        #region idSchool: 1, idPartBody: 3, Category: Stand, Label: StandFront
        bodyDatas.Add((1, 3, Category.Stand, Label.StandFront), 
            (GetPartBodyPositionData(0.01974f, 1.832f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        bodyDatas.Add((1, 3, Category.Stand, Label.StandFront), 
            (GetPartBodyPositionData(0.01974f, 1.862f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        #endregion
        #region idSchool: 1, idPartBody: 4, Category: Stand, Label: StandFront
        bodyDatas.Add((1, 4, Category.Stand, Label.StandFrontFrame0), 
            (GetPartBodyPositionData(0.01874f, 1.355f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        bodyDatas.Add((1, 4, Category.Stand, Label.StandFrontFrame1), 
            (GetPartBodyPositionData(0.01874f, 1.385f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        #endregion
        #region idSchool: 1, idPartBody: 5, Category: Stand, Label: StandFront
        bodyDatas.Add((1, 5, Category.Stand, Label.StandFront), 
            (GetPartBodyPositionData(0.00174f, 1.8002f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        bodyDatas.Add((1, 5, Category.Stand, Label.StandFront), 
            (GetPartBodyPositionData(0.00174f, 1.8302f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        #endregion
        #region idSchool: 1, idPartBody: 6, Category: Stand, Label: StandFront
        bodyDatas.Add((1, 6, Category.Stand, Label.StandFront), 
            (GetPartBodyPositionData(-0.10379f, 0.879f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        bodyDatas.Add((1, 6, Category.Stand, Label.StandFront), 
            (GetPartBodyPositionData(-0.10379f, 0.909f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        #endregion
        #region idSchool: 1, idPartBody: 7, Category: Stand, Label: StandFront
        bodyDatas.Add((1, 7, Category.Stand, Label.StandFront), 
            (GetPartBodyPositionData(0f, 0f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 0f)));
        bodyDatas.Add((1, 7, Category.Stand, Label.StandFront), 
            (GetPartBodyPositionData(0f, 0f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 0f)));
        #endregion
        #region idSchool: 1, idPartBody: 8, Category: Stand, Label: StandFront
        bodyDatas.Add((1, 8, Category.Stand, Label.StandFront), 
            (GetPartBodyPositionData(0f, 0f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 0f)));
        bodyDatas.Add((1, 8, Category.Stand, Label.StandFront), 
            (GetPartBodyPositionData(0f, 0f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 0f)));
        #endregion

        #region idSchool: 1, idPartBody: 0, Category: Stand, Label: StandBack
        bodyDatas.Add((1, 0, Category.Stand, Label.StandBack), 
            (GetPartBodyPositionData(0f, 0.27f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        bodyDatas.Add((1, 0, Category.Stand, Label.StandBack), 
            (GetPartBodyPositionData(0f, 0.27f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        #endregion
        #region idSchool: 1, idPartBody: 1, Category: Stand, Label: StandBack
        bodyDatas.Add((1, 1, Category.Stand, Label.StandBack), 
            (GetPartBodyPositionData(0.00974f, 0.734f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        bodyDatas.Add((1, 1, Category.Stand, Label.StandBack), 
            (GetPartBodyPositionData(0.00974f, 0.704f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        #endregion
        #region idSchool: 1, idPartBody: 2, Category: Stand, Label: StandBack
        bodyDatas.Add((1, 2, Category.Stand, Label.StandBack), 
            (GetPartBodyPositionData(0.01774f, 1.544f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        bodyDatas.Add((1, 2, Category.Stand, Label.StandBack), 
            (GetPartBodyPositionData(0.01774f, 1.514f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        #endregion
        #region idSchool: 1, idPartBody: 3, Category: Stand, Label: StandBack
        bodyDatas.Add((1, 3, Category.Stand, Label.StandBack), 
            (GetPartBodyPositionData(0.01974f, 1.514f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        bodyDatas.Add((1, 3, Category.Stand, Label.StandBack), 
            (GetPartBodyPositionData(0.01974f, 1.484f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        #endregion
        #region idSchool: 1, idPartBody: 4, Category: Stand, Label: StandBack
        bodyDatas.Add((1, 4, Category.Stand, Label.StandBackFrame0), 
            (GetPartBodyPositionData(0f, 0f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 0f)));
        bodyDatas.Add((1, 4, Category.Stand, Label.StandBackFrame1), 
            (GetPartBodyPositionData(0f, 0f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 0f)));
        #endregion
        #region idSchool: 1, idPartBody: 5, Category: Stand, Label: StandBack
        bodyDatas.Add((1, 5, Category.Stand, Label.StandBack),
            (GetPartBodyPositionData(0.00174f, 1.8502f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        bodyDatas.Add((1, 5, Category.Stand, Label.StandBack),
            (GetPartBodyPositionData(0.00174f, 1.8202f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        #endregion
        #region idSchool: 1, idPartBody: 6, Category: Stand, Label: StandBack
        bodyDatas.Add((1, 6, Category.Stand, Label.StandBack),
            (GetPartBodyPositionData(0f, 0f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 0f)));
        bodyDatas.Add((1, 6, Category.Stand, Label.StandBack),
            (GetPartBodyPositionData(0f, 0f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 0f)));
        #endregion
        #region idSchool: 1, idPartBody: 7, Category: Stand, Label: StandBack
        bodyDatas.Add((1, 7, Category.Stand, Label.StandBack),
            (GetPartBodyPositionData(0.11921f, 0.864f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        bodyDatas.Add((1, 7, Category.Stand, Label.StandBack),
            (GetPartBodyPositionData(0.11921f, 0.834f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        #endregion
        #region idSchool: 1, idPartBody: 8, Category: Stand, Label: StandBack
        bodyDatas.Add((1, 8, Category.Stand, Label.StandBack),
            (GetPartBodyPositionData(0f, 0f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 0f)));
        bodyDatas.Add((1, 8, Category.Stand, Label.StandBack),
            (GetPartBodyPositionData(0f, 0f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 0f)));
        #endregion

        #region idSchool: 1, idPartBody: 0, Category: Stand, Label: StandLeft
        bodyDatas.Add((1, 0, Category.Stand, Label.StandLeft),
            (GetPartBodyPositionData(0f, 0.274f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        bodyDatas.Add((1, 0, Category.Stand, Label.StandLeft),
            (GetPartBodyPositionData(0f, 0.274f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        #endregion
        #region idSchool: 1, idPartBody: 1, Category: Stand, Label: StandLeft
        bodyDatas.Add((1, 1, Category.Stand, Label.StandLeft),
            (GetPartBodyPositionData(0.00974f, 0.714f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        bodyDatas.Add((1, 1, Category.Stand, Label.StandLeft),
            (GetPartBodyPositionData(0.00974f, 0.684f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        #endregion
        #region idSchool: 1, idPartBody: 2, Category: Stand, Label: StandLeft
        bodyDatas.Add((1, 2, Category.Stand, Label.StandLeft),
            (GetPartBodyPositionData(0.01774f, 1.544f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        bodyDatas.Add((1, 2, Category.Stand, Label.StandLeft),
            (GetPartBodyPositionData(0.01774f, 1.514f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        #endregion
        #region idSchool: 1, idPartBody: 3, Category: Stand, Label: StandLeft
        bodyDatas.Add((1, 3, Category.Stand, Label.StandLeft),
            (GetPartBodyPositionData(0.01974f, 1.942f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        bodyDatas.Add((1, 3, Category.Stand, Label.StandLeft),
            (GetPartBodyPositionData(0.01974f, 1.912f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        #endregion
        #region idSchool: 1, idPartBody: 4, Category: Stand, Label: StandLeft
        bodyDatas.Add((1, 4, Category.Stand, Label.StandLeftFrame0),
            (GetPartBodyPositionData(-0.16879f, 1.425f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        bodyDatas.Add((1, 4, Category.Stand, Label.StandLeftFrame1),
            (GetPartBodyPositionData(-0.16879f, 1.395f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        #endregion
        #region idSchool: 1, idPartBody: 5, Category: Stand, Label: StandLeft
        bodyDatas.Add((1, 5, Category.Stand, Label.StandLeft),
            (GetPartBodyPositionData(0.00174f, 1.8502f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        bodyDatas.Add((1, 5, Category.Stand, Label.StandLeft),
            (GetPartBodyPositionData(0.00174f, 1.8202f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        #endregion
        #region idSchool: 1, idPartBody: 6, Category: Stand, Label: StandLeft
        bodyDatas.Add((1, 6, Category.Stand, Label.StandLeft),
            (GetPartBodyPositionData(-0.10379f, 0.979f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        bodyDatas.Add((1, 6, Category.Stand, Label.StandLeft),
            (GetPartBodyPositionData(-0.10379f, 0.949f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        #endregion
        #region idSchool: 1, idPartBody: 7, Category: Stand, Label: StandLeft
        bodyDatas.Add((1, 7, Category.Stand, Label.StandLeft),
            (GetPartBodyPositionData(0f, 0f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 0f)));
        bodyDatas.Add((1, 7, Category.Stand, Label.StandLeft),
            (GetPartBodyPositionData(0f, 0f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 0f)));
        #endregion
        #region idSchool: 1, idPartBody: 8, Category: Stand, Label: StandLeft
        bodyDatas.Add((1, 8, Category.Stand, Label.StandLeft),
            (GetPartBodyPositionData(0f, 0f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 0f)));
        bodyDatas.Add((1, 8, Category.Stand, Label.StandLeft),
            (GetPartBodyPositionData(0f, 0f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 0f)));
        #endregion

        #region idSchool: 1, idPartBody: 0, Category: Stand, Label: StandRight
        bodyDatas.Add((1, 0, Category.Stand, Label.StandRight),
            (GetPartBodyPositionData(0f, 0.274f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        bodyDatas.Add((1, 0, Category.Stand, Label.StandRight),
            (GetPartBodyPositionData(0f, 0.274f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        #endregion
        #region idSchool: 1, idPartBody: 1, Category: Stand, Label: StandRight
        bodyDatas.Add((1, 1, Category.Stand, Label.StandRight),
            (GetPartBodyPositionData(0.00974f, 0.714f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        bodyDatas.Add((1, 1, Category.Stand, Label.StandRight),
            (GetPartBodyPositionData(0.00974f, 0.684f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        #endregion
        #region idSchool: 1, idPartBody: 2, Category: Stand, Label: StandRight
        bodyDatas.Add((1, 2, Category.Stand, Label.StandRight),
            (GetPartBodyPositionData(0.01774f, 1.544f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        bodyDatas.Add((1, 2, Category.Stand, Label.StandRight),
            (GetPartBodyPositionData(0.01774f, 1.514f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        #endregion
        #region idSchool: 1, idPartBody: 3, Category: Stand, Label: StandRight
        bodyDatas.Add((1, 3, Category.Stand, Label.StandRight),
            (GetPartBodyPositionData(0.01974f, 1.942f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        bodyDatas.Add((1, 3, Category.Stand, Label.StandRight),
            (GetPartBodyPositionData(0.01974f, 1.912f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        #endregion
        #region idSchool: 1, idPartBody: 4, Category: Stand, Label: StandRight
        bodyDatas.Add((1, 4, Category.Stand, Label.StandRightFrame0),
            (GetPartBodyPositionData(-0.16879f, 1.425f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        bodyDatas.Add((1, 4, Category.Stand, Label.StandRightFrame1),
            (GetPartBodyPositionData(-0.16879f, 1.395f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        #endregion
        #region idSchool: 1, idPartBody: 5, Category: Stand, Label: StandRight
        bodyDatas.Add((1, 5, Category.Stand, Label.StandRight),
            (GetPartBodyPositionData(0.00174f, 1.8502f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        bodyDatas.Add((1, 5, Category.Stand, Label.StandRight),
            (GetPartBodyPositionData(0.00174f, 1.8202f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        #endregion
        #region idSchool: 1, idPartBody: 6, Category: Stand, Label: StandRight
        bodyDatas.Add((1, 6, Category.Stand, Label.StandRight),
            (GetPartBodyPositionData(-0.10379f, 0.979f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        bodyDatas.Add((1, 6, Category.Stand, Label.StandRight),
            (GetPartBodyPositionData(-0.10379f, 0.949f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        #endregion
        #region idSchool: 1, idPartBody: 7, Category: Stand, Label: StandRight
        bodyDatas.Add((1, 7, Category.Stand, Label.StandRight),
            (GetPartBodyPositionData(0f, 0f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 0f)));
        bodyDatas.Add((1, 7, Category.Stand, Label.StandRight),
            (GetPartBodyPositionData(0f, 0f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 0f)));
        #endregion
        #region idSchool: 1, idPartBody: 8, Category: Stand, Label: StandRight
        bodyDatas.Add((1, 8, Category.Stand, Label.StandRight),
            (GetPartBodyPositionData(0f, 0f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 0f)));
        bodyDatas.Add((1, 8, Category.Stand, Label.StandRight),
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
            (GetPartBodyPositionData(0.01974f, 1.544f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        bodyDatas.Add((1, 3, Category.Move, Label.MoveFrontFrame1),
            (GetPartBodyPositionData(0.01974f, 1.544f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        #endregion
        #region idSchool: 1, idPartBody: 4, Category: Move, Label: MoveFront
        bodyDatas.Add((1, 4, Category.Move, Label.MoveFrontFrame0),
            (GetPartBodyPositionData(0.01974f, 1.883f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
        bodyDatas.Add((1, 4, Category.Move, Label.MoveFrontFrame1),
            (GetPartBodyPositionData(0.01974f, 1.883f, 0f), GetPartBodyRotationData(0f, 0f, 0f), GetPartBodyScaleData(1f, 1f, 1f), GetPartBodyColorData(1f, 1f, 1f, 1f)));
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
        bodyDatas.Add((1, 7, Category.Atk, Label.AtkFrontFrame0),
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
    }
}