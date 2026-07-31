using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public class SyncMobData
{
    public int id;
    public int idMob;
    public string nameMob;
    public int maxHP;
    public int hp;
    public int level;
    public float posX;
    public float posY;
    public State state;
    public int idState;
    public Direction direction;
    public TileType currentTile;
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
    [SerializeField] private GameObject updateHPUI;

    private readonly ConcurrentQueue<SyncMobDataPacket> syncMobsResultPacketQueue = new ConcurrentQueue<SyncMobDataPacket>();
    private readonly ConcurrentQueue<SyncMobDataPacket> syncMobsDeadResultPacketQueue = new ConcurrentQueue<SyncMobDataPacket>();
    private readonly ConcurrentQueue<(EnumCmdCode, int, int, int)> playerAttackMobResultPacketQueue = new ConcurrentQueue<(EnumCmdCode, int, int, int)>();
    private readonly ConcurrentQueue<(EnumCmdCode, int, int, int)> otherPlayerAttackMobResultPacketQueue = new ConcurrentQueue<(EnumCmdCode, int, int, int)>();

    private CancellationTokenSource syncTokenSource;

    private Dictionary<int, Mob> mobs = new Dictionary<int, Mob>();
    private Dictionary<int, float> lastUpdateTime = new Dictionary<int, float>();

    private const float timeOut = 1.2f; // mob update chậm hơn player

    private SocketManager socketManager;

    private void Awake()
    {
        socketManager = GameManager.Instance.GetComponent<SocketManager>();

        syncTokenSource = new CancellationTokenSource();
        _ = ReadMobsPacketLoop(syncTokenSource.Token);
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

    public async Task ReadMobsPacketLoop(CancellationToken token)
    {
        while (!token.IsCancellationRequested)
        {
            byte[] syncMobsData = socketManager.GetSyncMobsData();
            byte[] syncMobsDeadData = socketManager.GetSyncMobsDeadData();
            byte[] playerAttackMob = socketManager.GetPlayerAttackMobData();
            byte[] otherPlayerAttackMob = socketManager.GetOtherPlayerAttackMobData();

            if (syncMobsData != null && syncMobsData.Length > 0)
            {
                PacketReaderManager reader = new PacketReaderManager(syncMobsData);

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
                        maxHP = reader.ReadInt(),
                        hp = reader.ReadInt(),
                        level = reader.ReadInt(),
                        posX = reader.ReadFloat(),
                        posY = reader.ReadFloat(),
                        state = (State)reader.ReadInt(),
                        idState = reader.ReadInt(),
                        direction = (Direction)reader.ReadInt(),
                        currentTile = (TileType)reader.ReadInt()
                    });
                }

                syncMobsResultPacketQueue.Enqueue(data);
            }

            if (syncMobsDeadData != null && syncMobsDeadData.Length > 0)
            {
                PacketReaderManager reader = new PacketReaderManager(syncMobsDeadData);

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
                        maxHP = reader.ReadInt(),
                        hp = reader.ReadInt(),
                        level = reader.ReadInt(),
                        posX = reader.ReadFloat(),
                        posY = reader.ReadFloat(),
                        state = (State)reader.ReadInt(),
                        idState = reader.ReadInt(),
                        direction = (Direction)reader.ReadInt(),
                        currentTile = (TileType)reader.ReadInt()
                    });
                }

                syncMobsDeadResultPacketQueue.Enqueue(data);
            }

            if (playerAttackMob != null && playerAttackMob.Length > 0)
            {
                PacketReaderManager reader = new PacketReaderManager(playerAttackMob);
                EnumCmdCode cmd = (EnumCmdCode)reader.ReadInt();
                int aimedMobID = reader.ReadInt();
                int damage = reader.ReadInt();
                int hpMobAfterAttack = reader.ReadInt();

                playerAttackMobResultPacketQueue.Enqueue((cmd, aimedMobID, damage, hpMobAfterAttack));
            }

            if (otherPlayerAttackMob != null && otherPlayerAttackMob.Length > 0)
            {
                PacketReaderManager reader = new PacketReaderManager(otherPlayerAttackMob);
                EnumCmdCode cmd = (EnumCmdCode)reader.ReadInt();
                int aimedMobID = reader.ReadInt();
                int damage = reader.ReadInt();
                int hpMobAfterAttack = reader.ReadInt();

                otherPlayerAttackMobResultPacketQueue.Enqueue((cmd, aimedMobID, damage, hpMobAfterAttack));
            }

            await Task.Yield();
        }
    }

    public void OnUpdate()
    {
        SyncMobDataPacket syncMobsData = null;
        SyncMobDataPacket syncMobsDeadData = null;

        EnumCmdCode cmd = default;
        int aimedMobID = 0;
        int damage = 0;
        int hpMobAfterAttack = 0;
        bool hasPlayerAttack = false;

        EnumCmdCode otherCmd = default;
        int otherAimedMobID = 0;
        int otherDamage = 0;
        int otherHpMobAfterAttack = 0;
        bool hasOtherPlayerAttack = false;

        if (syncMobsResultPacketQueue.TryDequeue(out var syncMobPacket))
            syncMobsData = syncMobPacket;

        if (syncMobsDeadResultPacketQueue.TryDequeue(out var syncMobDeadPacket))
            syncMobsDeadData = syncMobDeadPacket;

        if (playerAttackMobResultPacketQueue.TryDequeue(out var playerAttackData))
        {
            cmd = playerAttackData.Item1;
            aimedMobID = playerAttackData.Item2;
            damage = playerAttackData.Item3;
            hpMobAfterAttack = playerAttackData.Item4;
            hasPlayerAttack = true;
        }

        if (otherPlayerAttackMobResultPacketQueue.TryDequeue(out var otherPlayerAttackData))
        {
            otherCmd = otherPlayerAttackData.Item1;
            otherAimedMobID = otherPlayerAttackData.Item2;
            otherDamage = otherPlayerAttackData.Item3;
            otherHpMobAfterAttack = otherPlayerAttackData.Item4;
            hasOtherPlayerAttack = true;
        }

        if (syncMobsData != null)
            OnMobDataFromServer(syncMobsData);

        if (syncMobsDeadData != null)
            OffMobDataFromServer(syncMobsDeadData);

        if (hasPlayerAttack)
        {
            if (mobs.TryGetValue(aimedMobID, out Mob mob) && mob != null && mob.mobData != null)
            {
                if (mob.mobData.hp != hpMobAfterAttack)
                {
                    if (hpMobAfterAttack < mob.mobData.hp)
                    {
                        GameObject objectDamageUI = PoolManager.Instance.Get(updateHPUI);
                        objectDamageUI.transform.SetParent(mob.mobObject.GetComponentInChildren<Canvas>().transform, false);
                        objectDamageUI.transform.localPosition = Vector3.zero;

                        UpdateHPUIController injuredDamageUI = objectDamageUI.GetComponent<UpdateHPUIController>();

                        if (injuredDamageUI != null)
                            injuredDamageUI.SetInjuredDamage(damage);
                    }

                    mob.mobData.hp = hpMobAfterAttack;
                }
            }
        }

        if (hasOtherPlayerAttack)
        {
            if (mobs.TryGetValue(otherAimedMobID, out Mob mob) && mob != null && mob.mobData != null)
            {
                if (mob.mobData.hp != otherHpMobAfterAttack)
                {
                    if (otherHpMobAfterAttack < mob.mobData.hp)
                    {
                        GameObject objectDamageUI = PoolManager.Instance.Get(updateHPUI);

                        objectDamageUI.transform.SetParent(mob.mobObject.GetComponentInChildren<Canvas>().transform, false);

                        objectDamageUI.transform.localPosition = Vector3.zero;

                        UpdateHPUIController injuredDamageUI = objectDamageUI.GetComponent<UpdateHPUIController>();

                        if (injuredDamageUI != null)
                            injuredDamageUI.SetInjuredDamage(otherDamage);
                    }

                    mob.mobData.hp = otherHpMobAfterAttack;
                }
            }
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

        Mob mob;

        foreach (var mobData in data.mobsData)
        {
            if (!mobs.TryGetValue(mobData.id, out mob) && mobData.hp > 0)
            {
                mob = new Mob();

                mob.mobObject = InitMob(mobData);

                if (mob.mobObject == null)
                    continue;

                mob.mobData = mobData;
                mobs.Add(mobData.id, mob);
            }
            else
            {
                if (mobData != null)
                    mob.mobData = mobData;
            }

            mob.mobObject.GetComponent<MobController>().ApplyServerState(mobData);
            lastUpdateTime[mobData.id] = Time.time;
        }
    }
    private void OffMobDataFromServer(SyncMobDataPacket data)
    {
        if (data == null || data.mobsData == null)
            return;

        Mob mob;

        foreach (var mobDeadData in data.mobsData)
        {
            if (mobDeadData == null)
                continue;

            if (mobs.TryGetValue(mobDeadData.id, out mob))
            {
                mob.mobObject = mobs[mobDeadData.id].mobObject;
                mob.mobData = mobDeadData;

                mob.mobObject.GetComponent<MobController>().ApplyServerState(mobDeadData);
                lastUpdateTime[mobDeadData.id] = Time.time;
            }
        }
    }
    public void ApplyMobDead(int id)
    {
        Mob mobDead;

        if (mobs.TryGetValue(id, out mobDead))
        {
            PoolManager.Instance.Release(mobDead.mobObject);
            mobs.Remove(id);
        }
        if (lastUpdateTime.TryGetValue(id, out float lastTime))
        {
            lastUpdateTime.Remove(id);
        }
    }
    private GameObject InitMob(SyncMobData mobData)
    {
        //GameObject prefab = mobPrefabs[mobData.idMob];
        GameObject prefab = mobPrefabs[0];
        GameObject mob = PoolManager.Instance.Get(prefab); 

        mob.transform.SetPositionAndRotation(new Vector2(mobData.posX, mobData.posY), Quaternion.identity);

        return mob;
    }

    public void RemoveMob(int idMob)
    {
        if (mobs.TryGetValue(idMob, out Mob mob))
        {
            PoolManager.Instance.Release(mob.mobObject);
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
                PoolManager.Instance.Release(kv.Value.mobObject);
            }
        }

        mobs.Clear();
        lastUpdateTime.Clear();
    }

    public Dictionary<int, Mob> GetMobs()
    {
        return mobs;
    }

    public void RegisterDontDestroyOnLoad()
    {
        GameManager.Instance.RegisterPersistent(this);
    }

    private void OnDestroy()
    {
        if (syncTokenSource != null)
        {
            syncTokenSource.Cancel();
            syncTokenSource.Dispose();
        }
    }
}
