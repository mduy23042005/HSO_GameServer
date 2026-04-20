using System;
using System.Collections.Generic;
using UnityEngine;

public class SyncMobData
{
    public int id;
    public int idMob;
    public string nameMob;
    public float posX;
    public float posY;
    public string state;
    public int idState;
    public int direction;
}
public class SyncMobDataPacket
{
    public EnumCmdCode cmd;
    public List<SyncMobData> mobsData;
}

public class Mob
{
    public GameObject mobObject;
    public SyncMobData mobData;
}

public class MobsManager : MonoBehaviour, IUpdatable
{
    [SerializeField] private List<GameObject> mobPrefabs;

    private Dictionary<int, Mob> mobs = new Dictionary<int, Mob>();
    private Dictionary<int, float> lastUpdateTime = new Dictionary<int, float>();

    private const float timeOut = 1.2f; // mob update chậm hơn player

    private SocketManager socketManager;

    private void Awake()
    {
        socketManager = GameManager.Instance.GetComponent<SocketManager>();
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
        byte[] mobSyncData = socketManager.GetSyncMobsData();

        if (mobSyncData == null || mobSyncData.Length == 0)
        {
            return;
        }

        PacketReaderManager reader = new PacketReaderManager(mobSyncData);

        SyncMobDataPacket data = new SyncMobDataPacket();
        data.cmd = (EnumCmdCode)reader.ReadInt();
        data.mobsData = new List<SyncMobData>();

        int countSyncMobData = reader.ReadInt();
        for (int i = 0; i < countSyncMobData; i++)
        {
            data.mobsData.Add(new SyncMobData
            {
                id = reader.ReadInt(),
                idMob = reader.ReadInt(),
                nameMob = reader.ReadString(),
                posX = reader.ReadFloat(),
                posY = reader.ReadFloat(),
                state = reader.ReadString(),
                idState = reader.ReadInt(),
                direction = reader.ReadInt(),
            });
        }

        OnMobDataFromServer(data);

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

        Mob mob;

        foreach (var mobData in data.mobsData)
        {
            if (!mobs.TryGetValue(mobData.id, out mob))
            {
                mob = new Mob();

                mob.mobObject = InitMob(mobData);

                if (mob.mobObject == null)
                    continue;

                mobs.Add(mobData.id, mob);
            }

            mob.mobObject.GetComponent<MovementMobController>().ApplyServerState(mobData);
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
        if (mobs.TryGetValue(idMob, out Mob mob))
        {
            Destroy(mob.mobObject);
            mobs.Remove(idMob);
            lastUpdateTime.Remove(idMob);
        }
    }
    public void ClearAllMobs()
    {
        foreach (var kv in mobs)
        {
            if (kv.Value != null)
            {
                Destroy(kv.Value.mobObject);
            }
        }

        mobs.Clear();
        lastUpdateTime.Clear();
    }

    public void RegisterDontDestroyOnLoad()
    {
        throw new NotImplementedException();
    }
}
