using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public class MobsManager : MonoBehaviour, IUpdatable
{
    [SerializeField] private List<RuntimeAnimatorController> listMobAnimator; // animator quyết định sprite của mob nên chỉ cần lưu mỗi animator
    private Dictionary<int, RuntimeAnimatorController> mapMobAnimator = new Dictionary<int, RuntimeAnimatorController>();

    [SerializeField] private GameObject mobPrefab;
    [SerializeField] private GameObject updateHPUI;

    private readonly ConcurrentQueue<SyncMobDataPacket> syncMobsResultPacketQueue = new ConcurrentQueue<SyncMobDataPacket>();
    private readonly ConcurrentQueue<(EnumCmdCode, int, int, int, int)> playerAttackMobResultPacketQueue = new ConcurrentQueue<(EnumCmdCode, int, int, int, int)>();

    private Dictionary<int, Mob> mobs = new Dictionary<int, Mob>();
    private Dictionary<int, float> lastUpdateTime = new Dictionary<int, float>();

    private const float timeOut = 1.2f; // mob update chậm hơn player

    private SocketManager socketManager;
    private OtherPlayersManager syncOtherPlayersManager;

    private void Awake()
    {
        socketManager = GameManager.Instance.GetComponent<SocketManager>();
        syncOtherPlayersManager = GameManager.Instance.GetComponent<OtherPlayersManager>();

        ConvertListToMap();

        _ = ReadMobsPacketLoop(socketManager.shutdownCts);
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

    private void ConvertListToMap()
    {
        for (int i = 0; i < listMobAnimator.Count; i++)
        {
            mapMobAnimator[i + 1] = listMobAnimator[i];
        }
    }   

    private async Task ReadMobsPacketLoop(CancellationTokenSource token)
    {
        while (!token.IsCancellationRequested)
        {
            byte[] syncMobsData = socketManager.GetSyncMobsData();
            byte[] playerAttackMob = socketManager.GetPlayerAttackMobData();

            if (syncMobsData != null && syncMobsData.Length > 0)
            {
                PacketReaderManager reader = new PacketReaderManager(syncMobsData);

                SyncMobDataPacket data = new SyncMobDataPacket();
                data.cmd = (EnumCmdCode)reader.ReadInt();
                int countSyncMobData = reader.ReadInt();

                data.mobsData = new List<SyncMobData>();
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

            if (playerAttackMob != null && playerAttackMob.Length > 0)
            {
                PacketReaderManager reader = new PacketReaderManager(playerAttackMob);
                EnumCmdCode cmd = (EnumCmdCode)reader.ReadInt();
                int idAccount = reader.ReadInt();
                int aimedMobID = reader.ReadInt();
                int damage = reader.ReadInt();
                int hpMobAfterAttack = reader.ReadInt();

                playerAttackMobResultPacketQueue.Enqueue((cmd, idAccount, aimedMobID, damage, hpMobAfterAttack));
            }

            await Task.Yield();
        }
    }
    public void OnUpdate()
    {
        if (LogInView.GetIDAccount() == 0)
        {
            HandleTimeoutMob();
            return;
        }

        SyncMobDataPacket syncMobsData = null;

        EnumCmdCode cmd = default;
        int idAccount = 0;
        int aimedMobID = 0;
        int damage = 0;
        int hpMobAfterAttack = 0;
        bool hasPlayerAttack = false;

        if (syncMobsResultPacketQueue.TryDequeue(out var syncMobPacket))
            syncMobsData = syncMobPacket;

        if (playerAttackMobResultPacketQueue.TryDequeue(out var playerAttackData))
        {
            cmd = playerAttackData.Item1;
            idAccount = playerAttackData.Item2;
            aimedMobID = playerAttackData.Item3;
            damage = playerAttackData.Item4;
            hpMobAfterAttack = playerAttackData.Item5;
            hasPlayerAttack = true;
        }

        if (syncMobsData != null)
            OnMobDataFromServer(syncMobsData);

        if (hasPlayerAttack)
        {
            if (mobs.TryGetValue(aimedMobID, out Mob mob) && mob != null && mob.mobData != null)
            {
                if (damage > 0)
                {
                    UpdateHPUIController injuredDamageUI = PoolManager.Instance.Get(updateHPUI).GetComponent<UpdateHPUIController>();

                    if (idAccount == LogInView.GetIDAccount())
                    {
                        if (injuredDamageUI != null && PlayerManager.player != null)
                        {
                            ObserverManager.Notify(new Injured
                            {
                                attackObject = PlayerManager.player,
                                damage = damage,
                                injuredObject = mob.mobObject,
                                splatterBloodPosition = new Vector3(0.25f, 0.5f, 0f)
                            });
                        }
                    }
                    else
                    {
                        if (injuredDamageUI != null && syncOtherPlayersManager.GetOtherPlayerByID(idAccount) != null && syncOtherPlayersManager.GetOtherPlayerByID(idAccount).otherPlayerObject != null)
                        {
                            ObserverManager.Notify(new Injured
                            {
                                attackObject = syncOtherPlayersManager.GetOtherPlayerByID(idAccount).otherPlayerObject,
                                damage = damage,
                                injuredObject = mob.mobObject,
                                splatterBloodPosition = new Vector3(0.25f, 0.5f, 0f)
                            });
                        }
                    }

                    mob.mobData.hp = hpMobAfterAttack;
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
            if (mobs.TryGetValue(id, out Mob mob))
            {
                PoolManager.Instance.Release(mob.mobObject);
                mobs.Remove(id);
                lastUpdateTime.Remove(id);
            }
        }
    }
    private void OnMobDataFromServer(SyncMobDataPacket data)
    {
        if (data == null || data.mobsData == null)
            return;

        Mob mob;

        foreach (var mobData in data.mobsData)
        {
            if (!mobs.TryGetValue(mobData.id, out mob) && mobData.hp > 0) // trường hợp mob chưa tồn tại trên client nhưng có tồn tại ở server
            {
                mob = new Mob();

                if (mob.mobObject == null)
                    mob.mobObject = InitMob(mobData);

                mob.mobData = mobData;
                mobs.Add(mobData.id, mob);

                MobController mobController = mob.mobObject.GetComponent<MobController>();

                if (mobController.GetSpriteObject().GetComponent<Animator>().runtimeAnimatorController == null)
                    mobController.GetSpriteObject().GetComponent<Animator>().runtimeAnimatorController = mapMobAnimator[mobData.idMob];

                mobController.ApplyServerState(mobData);
                lastUpdateTime[mobData.id] = Time.time;
                continue;
            }

            if (mob != null) // trường hợp mob đã tồn tại trên client nhưng có thay đổi ở server
            {
                mob.mobData = mobData;
                mob.mobObject.GetComponent<MobController>().ApplyServerState(mobData);
                lastUpdateTime[mobData.id] = Time.time;
            }
        }
    }
    private void ClearMobInjuredUI(GameObject mobObject)
    {
        if (mobObject == null)
            return;

        // Xóa hiệu ứng máu và UI sát thương đang nằm trong Canvas của mob
        Canvas canvas = mobObject.GetComponentInChildren<Canvas>();

        if (canvas != null)
        {
            UpdateHPUIController[] hpUIs = canvas.GetComponentsInChildren<UpdateHPUIController>(true);
            foreach (UpdateHPUIController hpUI in hpUIs)
            {
                hpUI.ClearInjuredUI();
            }
        }

        // Xóa các hiệu ứng máu còn nằm trực tiếp trong mob
        UpdateHPUIController[] childUIs = mobObject.GetComponentsInChildren<UpdateHPUIController>(true);
        foreach (UpdateHPUIController hpUI in childUIs)
        {
            hpUI.ClearInjuredUI();
        }
    }
    public void ApplyMobDead(int id)
    {
        Mob mobDead;

        if (mobs.TryGetValue(id, out mobDead))
        {
            ClearMobInjuredUI(mobDead.mobObject);

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
        GameObject prefab = mobPrefab;
        GameObject mob = PoolManager.Instance.Get(prefab); 

        mob.transform.SetPositionAndRotation(new Vector2(mobData.posX, mobData.posY), Quaternion.identity);

        return mob;
    }

    public Dictionary<int, Mob> GetMobs()
    {
        return mobs;
    }
    public Mob GetMobByID(int id)
    {
        if (mobs.TryGetValue(id, out Mob mob))
        {
            return mob;
        }
        return null;
    }

    public void RegisterDontDestroyOnLoad()
    {
        GameManager.Instance.RegisterPersistent(this);
    }

    public void ClearMobs()
    {
        foreach (var mob in mobs.Values)
        {
            if (mob.mobObject != null)
            {
                PoolManager.Instance.Release(mob.mobObject);
            }
        }
        mobs.Clear();
        lastUpdateTime.Clear();
    }
}
