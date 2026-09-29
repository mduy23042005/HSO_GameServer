using System;
using System.Collections.Generic;
using UnityEngine;

public static class ObserverManager
{
    private static Dictionary<Type, List<Delegate>> observers = new Dictionary<Type, List<Delegate>>();

    public static void Register<T>(Action<T> callback)
    {
        Type type = typeof(T);

        if (!observers.ContainsKey(type))
            observers.Add(type, new List<Delegate>());

        observers[type].Add(callback);
    }
    public static void Unregister<T>(Action<T> callback)
    {
        Type type = typeof(T);

        if (!observers.ContainsKey(type))
            return;

        observers[type].Remove(callback);
    }

    public static void Notify<T>(T eventData)
    {
        Type type = typeof(T);

        if (!observers.ContainsKey(type))
        {
            return;
        }

        foreach (var action in observers[type])
        {
            try
            {
                if (action is Action<T> callback)
                    callback?.Invoke(eventData);
            }
            catch (Exception ex)
            {
                Debug.LogError(ex);
            }
        }
    }
}
