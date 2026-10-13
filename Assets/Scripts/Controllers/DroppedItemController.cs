using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class DroppedItemController : MonoBehaviour, IUpdatable
{
    [SerializeField] private GameObject shadow;
    [SerializeField] private GameObject waterShadow;

    private AStarManager astar = new AStarManager();
    private bool isStandingInWater = false;

    private void OnEnable()
    {
        GameManager.Instance.Register(this);
    }
    private void OnDisable()
    {
        if (GameManager.Instance != null)
            GameManager.Instance.Unregister(this);
    }

    public void OnFixedUpdate() { }

    public void OnLateUpdate() { }

    public void OnUpdate()
    {
        if (astar.IsStandInWater(MapView.mapFileData, transform.position.x, transform.position.y) && transform.position != Vector3.zero)
        {
            shadow.SetActive(false);
            waterShadow.SetActive(true);

            // chỉ chạy đúng 1 lần khi vừa xuống nước
            if (!isStandingInWater)
            {
                transform.position = new Vector3(transform.position.x, transform.position.y - 0.2f, transform.position.z);
                waterShadow.transform.position = new Vector3(transform.position.x, transform.position.y + 0.2f, transform.position.z);

                isStandingInWater = true;
            }
        }
        else
        {
            shadow.SetActive(true);
            waterShadow.SetActive(false);

            // chỉ chạy đúng 1 lần khi vừa lên bờ
            if (isStandingInWater)
            {
                transform.position = new Vector3(transform.position.x, transform.position.y + 0.2f, transform.position.z);
                waterShadow.transform.position = new Vector3(transform.position.x, transform.position.y - 0.2f, transform.position.z);

                isStandingInWater = false;
            }
        }
    }

    public void RegisterDontDestroyOnLoad() { }
}
