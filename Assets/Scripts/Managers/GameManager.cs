using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private static GameManager instance;
    public static GameManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindAnyObjectByType<GameManager>();
            }
            return instance;
        }
    }

    private List<IUpdatable> updatables = new List<IUpdatable>();

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void Register(IUpdatable obj)
    {
        if (obj != null && !updatables.Contains(obj))
        {
            updatables.Add(obj);
        }
    }

    public void Unregister(IUpdatable obj)
    {
        if (obj != null && updatables.Contains(obj))
        {
            updatables.Remove(obj);
        }
    }

    public void RegisterPersistent(IUpdatable obj)
    {
        if (obj != null && obj is MonoBehaviour)
        {
            MonoBehaviour mb = (MonoBehaviour)obj;
            DontDestroyOnLoad(mb.gameObject);
        }
    }

    private void Update()
    {
        for (int i = updatables.Count - 1; i >= 0; i--)
        {
            var updatable = updatables[i];
            // check null hoặc bị destroy
            if (updatable == null || (updatable is Object unityObj && unityObj == null))
            {
                updatables.RemoveAt(i);
                continue;
            }
            updatable.OnUpdate();
        }
    }
    private void LateUpdate()
    {
        for (int i = updatables.Count - 1; i >= 0; i--)
        {
            var updatable = updatables[i];
            // check null hoặc bị destroy
            if (updatable == null || (updatable is Object unityObj && unityObj == null))
            {
                updatables.RemoveAt(i);
                continue;
            }
            updatable.OnLateUpdate();
        }
    }
    private void FixedUpdate()
    {
        for (int i = updatables.Count - 1; i >= 0; i--)
        {
            var updatable = updatables[i];
            // check null hoặc bị destroy
            if (updatable == null || (updatable is Object unityObj && unityObj == null))
            {
                updatables.RemoveAt(i);
                continue;
            }
            updatable.OnFixedUpdate();
        }
    }
}
