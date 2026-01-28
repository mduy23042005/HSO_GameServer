using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.SceneManagement;


public class CinemachineCameraController : MonoBehaviour, IUpdatable
{
    private CinemachineCamera virtualCamera;
    private bool hasSetTarget = false;

    private CinemachineConfiner2D confiner;
    private BoxCollider2D mapBounds;

    private void Awake()
    {
        virtualCamera = GetComponent<CinemachineCamera>();
        confiner = GetComponent<CinemachineConfiner2D>();
    }
    private void OnEnable()
    {
        GameManager.Instance.Register(this);
        SceneManager.sceneLoaded += OnSceneLoaded;
    }
    private void OnDisable()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.Unregister(this);
        }
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        hasSetTarget = false;

        if (confiner == null)
            return;

        GameObject boundsObj = GameObject.Find("CameraBounds");
        if (boundsObj != null)
        {
            mapBounds = boundsObj.GetComponent<BoxCollider2D>();
            if (mapBounds != null)
            {
                confiner.BoundingShape2D = mapBounds;
                confiner.InvalidateBoundingShapeCache();
            }
            else
            {
                Debug.LogWarning("CameraBounds không có BoxCollider2D");
            }
        }
        else
        {
            Debug.LogWarning("Không tìm thấy CameraBounds trong scene");
        }
    }

    public void OnUpdate()
    {
        if (hasSetTarget)
            return;

        int idSchool = LogInView.GetIDSchool();
        Transform target = null;

        switch (idSchool)
        {
            case 1:
                target = GameObject.Find("ChienBinh(Clone)")?.transform;
                break;
            case 2:
                target = GameObject.Find("SatThu(Clone)")?.transform;
                break;
            case 3:
                target = GameObject.Find("PhapSu(Clone)")?.transform;
                break;
            case 4:
                target = GameObject.Find("XaThu(Clone)")?.transform;
                break;
        }

        if (target != null)
        {
            virtualCamera.Follow = target;
            virtualCamera.LookAt = target;
            hasSetTarget = true;
        }
    }
    public void OnLateUpdate() { }
    public void OnFixedUpdate() { }
    public void RegisterDontDestroyOnLoad()
    {
        GameManager.Instance.RegisterPersistent(this);
    }
}
