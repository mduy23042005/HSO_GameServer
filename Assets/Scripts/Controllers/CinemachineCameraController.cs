using System.Collections.Generic;
using System.Linq;
using Unity.Cinemachine;
using UnityEngine;


public class CinemachineCameraController : MonoBehaviour
{
    [SerializeField] private List<Transform> accountTransform;
    private CinemachineCamera virtualCamera;

    private void Awake()
    {
        virtualCamera = GetComponent<CinemachineCamera>();
    }
    void Start()
    {
        int idSchool = LogInController.GetIDSchool();
        Transform player = null;
        switch (idSchool)
        {
            case 1:
                player = accountTransform[0];
                break;
            case 2:
                player = accountTransform[1];
                break;
            case 3:
                player = accountTransform[2];
                break;
                /*
            case 4:
                player = accountTransform[3];
                break;
                */
        }
        virtualCamera.Follow = player;
    }
}
