using System.Collections.Generic;
using UnityEngine;

public class PoolManager : MonoBehaviour, IUpdatable
{
    private class UIPool
    {
        public RectTransform prefabUIObject;
        public Queue<RectTransform> availableUIObjectsQueue = new Queue<RectTransform>();
    }
    private class Pool
    {
        public GameObject prefabObject;
        public Queue<GameObject> availableObjectsQueue = new Queue<GameObject>();
    }

    private static PoolManager instance;
    public static PoolManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindAnyObjectByType<GameManager>().GetComponent<PoolManager>();
            }
            return instance;
        }
    }

    private readonly Dictionary<GameObject, Pool> objectPools = new();
    private readonly Dictionary<GameObject, Pool> activeObjects = new();

    private readonly Dictionary<RectTransform, UIPool> uiObjectPools = new();
    private readonly Dictionary<RectTransform, UIPool> activeUIObjects = new();

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
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

    public void OnUpdate() { }
    public void OnLateUpdate() { }
    public void OnFixedUpdate() { }

    public void RegisterDontDestroyOnLoad() 
    {
        GameManager.Instance.RegisterPersistent(this);
    }

    public GameObject Get(GameObject prefab)
    {
        if (!objectPools.TryGetValue(prefab, out Pool pool))
        {
            pool = CreatePool(prefab);
        }

        if (pool.availableObjectsQueue.Count == 0)
        {
            ExpandPool(pool);
        }
        GameObject obj = pool.availableObjectsQueue.Dequeue();

        activeObjects[obj] = pool;
        obj.SetActive(true);
        return obj;
    }
    public void Release(GameObject obj)
    {
        if (obj == null) return;

        if (!activeObjects.TryGetValue(obj, out Pool pool))
        {
            Destroy(obj);
            return;
        }

        activeObjects.Remove(obj);

        obj.SetActive(false);
        pool.availableObjectsQueue.Enqueue(obj);
    }
    private Pool CreatePool(GameObject prefab)
    {
        Pool pool = new Pool();
        pool.prefabObject = prefab;
        int size = 10; // Initial size of the pool
        for (int i = 0; i < size; i++)
        {
            GameObject obj = Instantiate(pool.prefabObject);
            obj.SetActive(false);

            pool.availableObjectsQueue.Enqueue(obj);
        }
        objectPools.Add(prefab, pool);
        return pool;
    }
    private void ExpandPool(Pool pool)
    {
        GameObject obj = Instantiate(pool.prefabObject);
        obj.SetActive(false);
        pool.availableObjectsQueue.Enqueue(obj);
    }

    public RectTransform Get(RectTransform prefab, RectTransform parentUIObject)
    {
        if (!uiObjectPools.TryGetValue(prefab, out UIPool uiPool))
        {
            uiPool = CreatePool(prefab, parentUIObject);
        }

        if (uiPool.availableUIObjectsQueue.Count == 0)
        {
            ExpandPool(uiPool, parentUIObject);
        }
        RectTransform uiObj = uiPool.availableUIObjectsQueue.Dequeue();

        uiObj.SetParent(parentUIObject, false);

        activeUIObjects[uiObj] = uiPool;
        uiObj.gameObject.SetActive(true);
        return uiObj;
    }
    public void Release(RectTransform uiObj)
    {
        if (uiObj == null) return;

        if (!activeUIObjects.TryGetValue(uiObj, out UIPool uiPool))
        {
            Destroy(uiObj.gameObject);
            return;
        }

        activeUIObjects.Remove(uiObj);
        uiObj.gameObject.SetActive(false);
        uiPool.availableUIObjectsQueue.Enqueue(uiObj);
    }
    private UIPool CreatePool(RectTransform prefab, RectTransform parentUIObject)
    {
        UIPool uiPool = new UIPool();
        uiPool.prefabUIObject = prefab;
        int size = 10; // Initial size of the pool
        for (int i = 0; i < size; i++)
        {
            RectTransform uiObj = Instantiate(uiPool.prefabUIObject, parentUIObject);
            uiObj.gameObject.SetActive(false);

            uiPool.availableUIObjectsQueue.Enqueue(uiObj);
        }
        uiObjectPools.Add(prefab, uiPool);
        return uiPool;
    }
    private void ExpandPool(UIPool pool, RectTransform parentUIObject)
    {
        RectTransform uiObj = Instantiate(pool.prefabUIObject, parentUIObject);
        uiObj.gameObject.SetActive(false);
        pool.availableUIObjectsQueue.Enqueue(uiObj);
    }
}