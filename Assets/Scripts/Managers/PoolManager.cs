using System.Collections.Generic;
using UnityEngine;

public class PoolManager : MonoBehaviour, IUpdatable
{
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

    private class Pool
    {
        public GameObject prefabObject;
        public Queue<GameObject> availableObjectsQueue = new Queue<GameObject>();
    }
    private readonly Dictionary<GameObject, Pool> objectPools = new();
    private readonly Dictionary<GameObject, Pool> activeObjects = new();

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
}