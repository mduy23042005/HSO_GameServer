using System.Collections.Generic;
using System.Security.Principal;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.SceneManagement;


public class CinemachineCameraController : MonoBehaviour, IUpdatable
{
    private CinemachineCamera virtualCamera;
    private bool hasSetTarget = false;

    private void Awake()
    {
        virtualCamera = GetComponent<CinemachineCamera>();
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
        if (hasSetTarget)
            return;

        GameObject player = GameObject.Find("Player(Clone)");
        if (player == null)
            return;

        int idSchool = LogInView.GetIDSchool();
        Transform target = null;

        switch (idSchool)
        {
            case 1:
                target = player.transform.Find("ChienBinh");
                break;
            case 2:
                target = player.transform.Find("SatThu");
                break;
            case 3:
                target = player.transform.Find("PhapSu");
                break;
        }

        if (target != null)
        {
            virtualCamera.Follow = target;
            virtualCamera.LookAt = target;
            hasSetTarget = true; // chỉ set 1 lần
        }
    }
    public void OnLateUpdate() { }
    public void OnFixedUpdate() { }
    public void RegisterDontDestroyOnLoad()
    {
        GameManager.Instance.RegisterPersistent(this);
    }
}
