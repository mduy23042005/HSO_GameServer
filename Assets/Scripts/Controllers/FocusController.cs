using UnityEngine;

public class FocusController : MonoBehaviour, IUpdatable
{
    [SerializeField] private GameObject focusUI; // prefab

    private GameObject focusedObjectUI; // object chính điều khiển UI

    public static GameObject focusedObject; // object bị chỉ định để object chính thay đổi vị trí

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

    public void OnFixedUpdate()
    {
        if (focusedObject != null)
        {
            if (focusedObjectUI == null )
            {
                focusedObjectUI = PoolManager.Instance.Get(focusUI);
            }

            focusedObjectUI.transform.position = new Vector3(focusedObject.transform.position.x, focusedObject.transform.position.y + 3.5f, 0);
        }
        else
        {
            if (focusedObjectUI != null)
            {
                focusedObject = null;
                PoolManager.Instance.Release(focusedObjectUI);
                focusedObjectUI = null;
            }
        }
    }

    public void OnLateUpdate() { }

    public void OnUpdate() { }

    public void RegisterDontDestroyOnLoad()
    {
        GameManager.Instance.RegisterPersistent(this);
    }
}
