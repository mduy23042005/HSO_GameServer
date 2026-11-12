using UnityEngine;

public enum TargetType
{
    None,
    DroppedItem,
    Mob,
    NPC,
    Player
}

public class TargetableObject : MonoBehaviour
{
    [Header("Type Info")]
    public TargetType targetType;
    public string displayName;

    public int uniqueId;
}

public class FocusController : MonoBehaviour, IUpdatable
{
    [SerializeField] private GameObject focusUI; // prefab

    public static FocusObject focus = new FocusObject(); // object chính đang focus

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
        if (focus.focusedObject != null && focus.focusedObject.activeInHierarchy)
        {
            if (focus.focusedObjectUI == null)
                focus.focusedObjectUI = PoolManager.Instance.Get(focusUI);

            focus.focusedObjectUI.transform.position = new Vector3(focus.focusedObject.transform.position.x, focus.focusedObject.transform.position.y + 3.5f, 0);

            if (focus.focusedObject.GetComponent<ObjectTypeController>() != null)
                focus.focusedObjectType = focus.focusedObject.GetComponent<ObjectTypeController>().GetObjectType();
        }
        else
        {
            if (focus.focusedObjectUI != null)
            {
                focus.focusedObject = null;
                PoolManager.Instance.Release(focus.focusedObjectUI);

                focus.focusedObjectUI = null;
                focus.focusedObjectType = ObjectType.None;
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
