using System;
using System.Collections.Generic;
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
    public string state;
    public int idState;
    public int direction;
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
        byte[] syncMobsData = socketManager.GetSyncMobsData();
        byte[] playerAttackMob = socketManager.GetPlayerAttackMobData();
        byte[] otherPlayerAttackMob = socketManager.GetOtherPlayerAttackMobData();
        byte[] syncMobsDeadData = socketManager.GetSyncMobsDeadData();

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
                    nameMob = reader.ReadString(),
                    maxHP = reader.ReadInt(),
                    hp = reader.ReadInt(),
                    level = reader.ReadInt(),
                    posX = reader.ReadFloat(),
                    posY = reader.ReadFloat(),
                    state = reader.ReadString(),
                    idState = reader.ReadInt(),
                    direction = reader.ReadInt(),
                    currentTile = (TileType)reader.ReadInt()
                });
            }
            OnMobDataFromServer(data);
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
                    nameMob = reader.ReadString(),
                    maxHP = reader.ReadInt(),
                    hp = reader.ReadInt(),
                    level = reader.ReadInt(),
                    posX = reader.ReadFloat(),
                    posY = reader.ReadFloat(),
                    state = reader.ReadString(),
                    idState = reader.ReadInt(),
                    direction = reader.ReadInt(),
                    currentTile = (TileType)reader.ReadInt()
                });
            }
            OffMobDataFromServer(data);
        }

        if (playerAttackMob != null && playerAttackMob.Length > 0)
        {
            PacketReaderManager reader = new PacketReaderManager(playerAttackMob);
            EnumCmdCode cmd = (EnumCmdCode)reader.ReadInt();
            int aimedMobID = reader.ReadInt();
            int damage = reader.ReadInt();
            int hpMobAfterAttack = reader.ReadInt();

            if (mobs[aimedMobID] != null)
            {
                if (mobs[aimedMobID].mobData.hp != hpMobAfterAttack)
                {
                    if (hpMobAfterAttack < mobs[aimedMobID].mobData.hp)
                    {
                        GameObject objectDamageUI = Instantiate(updateHPUI, mobs[aimedMobID].mobObject.GetComponentInChildren<Canvas>().transform, false);

                        UpdateHPUIController injuredDamageUI = objectDamageUI.GetComponent<UpdateHPUIController>();
                        if (injuredDamageUI != null)
                        {
                            injuredDamageUI.SetInjuredDamage(damage);
                        }
                    }
                    mobs[aimedMobID].mobData.hp = hpMobAfterAttack;
                }
            }
        }

        if (otherPlayerAttackMob != null && otherPlayerAttackMob.Length > 0)
        {
            PacketReaderManager reader = new PacketReaderManager(otherPlayerAttackMob);
            EnumCmdCode cmd = (EnumCmdCode)reader.ReadInt();
            int aimedMobID = reader.ReadInt();
            int damage = reader.ReadInt();
            int hpMobAfterAttack = reader.ReadInt();

            if (mobs[aimedMobID] != null)
            {
                if (mobs[aimedMobID].mobData.hp != hpMobAfterAttack)
                {
                    if (hpMobAfterAttack < mobs[aimedMobID].mobData.hp)
                    {
                        GameObject objectDamageUI = Instantiate(updateHPUI, mobs[aimedMobID].mobObject.GetComponentInChildren<Canvas>().transform, false);

                        UpdateHPUIController injuredDamageUI = objectDamageUI.GetComponent<UpdateHPUIController>();
                        if (injuredDamageUI != null)
                        {
                            injuredDamageUI.SetInjuredDamage(damage);
                        }
                    }
                    mobs[aimedMobID].mobData.hp = hpMobAfterAttack;
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
                mob = new Mob();

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
            Destroy(mobDead.mobObject);
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

    public Dictionary<int, Mob> GetMobs()
    {
        return mobs;
    }

    public void RegisterDontDestroyOnLoad()
    {
        throw new NotImplementedException();
    }
}
