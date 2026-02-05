using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SyncMobData
{
    public int id;
    public int idMob;
    public float posX;
    public float posY;
    public string state;
    public int idState;
    public int direction;
}
[Serializable]
public class SyncMobDataPacket
{
    public string cmd;
    public List<SyncMobData> mobsData;
}

public class MobsManager : MonoBehaviour, IUpdatable
{
    [SerializeField] private List<GameObject> mobPrefabs;

    private Dictionary<int, GameObject> mobs = new Dictionary<int, GameObject>();
    private Dictionary<int, SyncMobData> mobsData = new Dictionary<int, SyncMobData>();
    private Dictionary<int, float> lastUpdateTime = new Dictionary<int, float>();

    private const float timeOut = 1.2f; // mob update chậm hơn player

    private SocketManager socketManager;
    private PacketSerializeManager packetSerializeManager;

    private void Awake()
    {
        socketManager = GameManager.Instance.GetComponent<SocketManager>();
        packetSerializeManager = GameManager.Instance.GetComponent<PacketSerializeManager>();
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
        string mobSyncData = socketManager.GetSyncMobsData();

        if (!string.IsNullOrEmpty(mobSyncData))
        {
            var data = packetSerializeManager.HandleReceivedPacket<SyncMobDataPacket>(mobSyncData);

            OnMobDataFromServer(data);
        }

        HandleTimeoutMob();
    }
    public void OnLateUpdate() { }
    public void OnFixedUpdate() { }

    private void HandleTimeoutMob()
    {
        List<int> toRemove = new List<int>();

        foreach (var kv in lastUpdateTime)
        {
            if (Time.time - kv.Value > timeOut)
            {
                toRemove.Add(kv.Key);
            }
        }

        foreach (var id in toRemove)
        {
            RemoveMob(id);
        }
    }

    private void OnMobDataFromServer(SyncMobDataPacket data)
    {
        if (data == null || data.mobsData == null)
            return;

        GameObject mobObj;

        foreach (var mobData in data.mobsData)
        {
            if (!mobs.TryGetValue(mobData.id, out mobObj))
            {
                mobObj = InitMob(mobData);

                if (mobObj == null)
                    continue;

                mobs.Add(mobData.id, mobObj);
                mobsData.Add(mobData.id, mobData);
            }

            mobObj.GetComponent<MovementMobController>().ApplyServerState(mobData);
            lastUpdateTime[mobData.id] = Time.time;
        }
    }

    private GameObject InitMob(SyncMobData mobData)
    {
        //GameObject prefab = mobPrefabs[mobData.idMob];
        GameObject prefab = mobPrefabs[0];
        GameObject mob = Instantiate(prefab, new Vector2(mobData.posX, mobData.posY), Quaternion.identity);

        return mob;
    }

    public void RemoveMob(int idMob)
    {
        if (mobs.TryGetValue(idMob, out GameObject mob))
        {
            Destroy(mob);
            mobs.Remove(idMob);
            mobsData.Remove(idMob);
            lastUpdateTime.Remove(idMob);
        }
    }
    public void ClearAllMobs()
    {
        foreach (var kv in mobs)
        {
            if (kv.Value != null)
            {
                Destroy(kv.Value);
            }
        }

        mobs.Clear();
        mobsData.Clear();
        lastUpdateTime.Clear();
    }

    public void RegisterDontDestroyOnLoad()
    {
        throw new NotImplementedException();
    }
}
