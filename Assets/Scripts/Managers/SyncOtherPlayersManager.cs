using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public class SyncOtherPlayersManager : MonoBehaviour, IUpdatable
{
    [SerializeField] private List<GameObject> otherPlayersPrefab;
    [SerializeField] private GameObject updateHPUI;

    private Dictionary<int, OtherPlayer> otherPlayers = new Dictionary<int, OtherPlayer>();
    private readonly ConcurrentQueue<SyncOtherPlayersResultPacket> syncOtherPlayersResultPacketQueue = new ConcurrentQueue<SyncOtherPlayersResultPacket>();
    private readonly ConcurrentQueue<SyncOtherPlayersResultPacket> syncOtherPlayersRealtimeResultPacketQueue = new ConcurrentQueue<SyncOtherPlayersResultPacket>();
    private readonly ConcurrentQueue<(EnumCmdCode, int, int, int, int)> syncUpdateHPUIQueue = new ConcurrentQueue<(EnumCmdCode, int, int, int, int)>();

    private CancellationTokenSource syncTokenSource;

    private Dictionary<int, float> lastUpdateTime = new Dictionary<int, float>();
    private const float timeOut = 1f;

    private List<int> toRemove = new List<int>();

    public static SyncOtherPlayersManager Instance;

    private SocketManager socketManager;

    private Action<LogOutClickEvent> logoutObserver;

    private void Awake()
    {
        Instance = this;
        socketManager = GameManager.Instance.GetComponent<SocketManager>();
        syncTokenSource = new CancellationTokenSource();
        _ = ReadSyncPacketLoop(syncTokenSource.Token);

        logoutObserver = eventData => PrepareForLogOut();
    }

    private void OnEnable()
    {
        GameManager.Instance.Register(this);

        ObserverManager.Register<LogOutClickEvent>(logoutObserver);
    }
    private void OnDisable()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.Unregister(this);
        }

        ObserverManager.Unregister<LogOutClickEvent>(logoutObserver);
    }

    public async Task ReadSyncPacketLoop(CancellationToken token)
    {
        while (!token.IsCancellationRequested)
        {
            byte[] onlineData = socketManager.GetSyncOtherPlayersData();
            byte[] onlineRealtimeData = socketManager.GetSyncOtherPlayersRealtimeData();
            byte[] updateOtherPlayerHPUIData = socketManager.GetMobsAttackOtherPlayerData();

            if (onlineData != null && onlineData.Length > 0)
            {
                PacketReaderManager reader = new PacketReaderManager(onlineData);

                SyncOtherPlayersResultPacket data = new SyncOtherPlayersResultPacket();
                data.cmd = (EnumCmdCode)reader.ReadInt();
                data.otherPlayersData = new List<OtherPlayerSyncData>();

                int countOtherPlayerData = reader.ReadInt();
                for (int i = 0; i < countOtherPlayerData; i++)
                {
                    OtherPlayerSyncData otherPlayerSyncData = new OtherPlayerSyncData();
                    otherPlayerSyncData.otherPlayerData = new PlayerData();

                    otherPlayerSyncData.otherPlayerData.idAccount = reader.ReadInt();
                    otherPlayerSyncData.otherPlayerData.nameChar = reader.ReadString();
                    otherPlayerSyncData.otherPlayerData.level = reader.ReadInt();
                    otherPlayerSyncData.otherPlayerData.idSchool = reader.ReadInt();
                    otherPlayerSyncData.otherPlayerData.hair = reader.ReadInt();
                    otherPlayerSyncData.otherPlayerData.weapon = reader.ReadInt();
                    otherPlayerSyncData.otherPlayerData.helmet = reader.ReadInt();
                    otherPlayerSyncData.otherPlayerData.armor = reader.ReadInt();
                    otherPlayerSyncData.otherPlayerData.legArmor = reader.ReadInt();

                    data.otherPlayersData.Add(otherPlayerSyncData);
                }
                syncOtherPlayersResultPacketQueue.Enqueue(data);
            }

            if (onlineRealtimeData != null && onlineRealtimeData.Length > 0)
            {
                PacketReaderManager reader = new PacketReaderManager(onlineRealtimeData);

                SyncOtherPlayersResultPacket data = new SyncOtherPlayersResultPacket();
                data.cmd = (EnumCmdCode)reader.ReadInt();
                data.otherPlayersData = new List<OtherPlayerSyncData>();

                int countOtherPlayerData = reader.ReadInt();
                for (int i = 0; i < countOtherPlayerData; i++)
                {
                    OtherPlayerSyncData otherPlayerSyncData = new OtherPlayerSyncData();
                    otherPlayerSyncData.otherPlayerData = new PlayerData();
                    otherPlayerSyncData.otherPlayerTransformData = new PlayerTransformData();
                    otherPlayerSyncData.otherPlayerTransformData.positionData = new PositionData();
                    otherPlayerSyncData.otherPlayerTransformData.scaleData = new ScaleData();
                    otherPlayerSyncData.otherPlayerStateData = new PlayerStateData();

                    otherPlayerSyncData.otherPlayerData.idAccount = reader.ReadInt();
                    otherPlayerSyncData.otherPlayerData.maxHP = reader.ReadInt();
                    otherPlayerSyncData.otherPlayerData.hp = reader.ReadInt();
                    otherPlayerSyncData.otherPlayerData.currentTile = (TileType)reader.ReadInt();

                    otherPlayerSyncData.otherPlayerTransformData.positionData.x = reader.ReadFloat();
                    otherPlayerSyncData.otherPlayerTransformData.positionData.y = reader.ReadFloat();

                    otherPlayerSyncData.otherPlayerTransformData.scaleData.x = reader.ReadFloat();

                    otherPlayerSyncData.otherPlayerStateData.stateData = (State)reader.ReadInt();
                    otherPlayerSyncData.otherPlayerStateData.directionData = (Direction)reader.ReadInt();
                    otherPlayerSyncData.otherPlayerStateData.partBodyTransforms = new List<PartBodyData>();

                    data.otherPlayersData.Add(otherPlayerSyncData);

                    PartBodyData faceBodyData = new PartBodyData();
                    faceBodyData.category = (Category)reader.ReadInt();
                    faceBodyData.label = (Label)reader.ReadInt();
                    data.otherPlayersData[i].otherPlayerStateData.partBodyTransforms.Add(faceBodyData);

                    PartBodyData partBodyData = new PartBodyData();
                    partBodyData.category = (Category)reader.ReadInt();
                    partBodyData.label = (Label)reader.ReadInt();
                    data.otherPlayersData[i].otherPlayerStateData.partBodyTransforms.Add(partBodyData);
                }
                syncOtherPlayersRealtimeResultPacketQueue.Enqueue(data);
            }
            if (updateOtherPlayerHPUIData != null && updateOtherPlayerHPUIData.Length > 0)
            {
                PacketReaderManager reader1 = new PacketReaderManager(updateOtherPlayerHPUIData);
                EnumCmdCode cmd = (EnumCmdCode)reader1.ReadInt();
                int idMob = reader1.ReadInt();
                int idAccount = reader1.ReadInt();
                int mobDamage = reader1.ReadInt();
                int otherPlayerHP = reader1.ReadInt();

                syncUpdateHPUIQueue.Enqueue((cmd, idMob, idAccount, mobDamage, otherPlayerHP));
            }

            await Task.Yield();
        }
    }
    public void OnUpdate()
    {
        foreach (var kv in lastUpdateTime)
        {
            if (Time.time - kv.Value > timeOut)
            {
                toRemove.Add(kv.Key);
            }
        }

        foreach (var idOtherPlayer in toRemove)
        {
            if (otherPlayers.TryGetValue(idOtherPlayer, out OtherPlayer obj))
            {
                PoolManager.Instance.Release(obj.otherPlayerObject);
                otherPlayers.Remove(idOtherPlayer);
                lastUpdateTime.Remove(idOtherPlayer);
            }
        }
        toRemove.Clear();

        SyncOtherPlayersResultPacket onlineData = null;
        SyncOtherPlayersResultPacket onlineRealtimeData = null;

        EnumCmdCode cmd = default;
        int id = 0;
        int idAccount = 0;
        int mobDamage = 0;
        int otherPlayerHP = 0;
        bool hasHPUpdate = false;

        if (syncOtherPlayersResultPacketQueue.TryDequeue(out var syncOnlineData))
            onlineData = syncOnlineData;

        if (syncOtherPlayersRealtimeResultPacketQueue.TryDequeue(out var syncOnlineRealtimeData))
            onlineRealtimeData = syncOnlineRealtimeData;

        if (syncUpdateHPUIQueue.TryDequeue(out var syncUpdateHPUIData))
        {
            cmd = syncUpdateHPUIData.Item1;
            id = syncUpdateHPUIData.Item2;
            idAccount = syncUpdateHPUIData.Item3;
            mobDamage = syncUpdateHPUIData.Item4;
            otherPlayerHP = syncUpdateHPUIData.Item5;
            hasHPUpdate = true;
        }

        if (onlineData != null)
        {
            if (onlineData.otherPlayersData != null)
            {
                foreach (var playerData in onlineData.otherPlayersData)
                {
                    if (playerData == null || playerData.otherPlayerData == null)
                        continue;

                    if (playerData.otherPlayerData.idAccount != LogInView.GetIDAccount())
                       OnDataFromServer(playerData);                  
                }
            }
        }
        if (onlineRealtimeData != null)
        {
            if (onlineRealtimeData.otherPlayersData != null)
            {
                foreach (var playerData in onlineRealtimeData.otherPlayersData)
                {
                    if (playerData == null || playerData.otherPlayerData == null)
                        continue;

                    if (playerData.otherPlayerData.idAccount != LogInView.GetIDAccount())
                        OnRealtimeDataFromServer(playerData);                
                }
            }
        }

        if (hasHPUpdate)
        {
            if (otherPlayers.TryGetValue(idAccount, out OtherPlayer otherPlayer) && otherPlayer != null && otherPlayer.otherPlayerData != null)
            {
                if (mobDamage > 0)
                {
                    UpdateHPUIController injuredDamageUI = PoolManager.Instance.Get(updateHPUI).GetComponent<UpdateHPUIController>();

                    if (injuredDamageUI != null && MobsManager.Instance.GetMobByID(id) != null && MobsManager.Instance.GetMobByID(id).mobObject != null)
                    {
                        ObserverManager.Notify(new Injured
                        {
                            attackObject = MobsManager.Instance.GetMobByID(id).mobObject,
                            damage = mobDamage,
                            injuredObject = otherPlayer.otherPlayerObject,
                            splatterBloodPosition = new Vector3(0f, 1.8f, 0f)
                        });
                    }

                    otherPlayers[idAccount].otherPlayerData.hp = otherPlayerHP;
                }
            }
        }
    }
    public void OnLateUpdate() { }
    public void OnFixedUpdate() { }
    public void RegisterDontDestroyOnLoad()
    {
        GameManager.Instance.RegisterPersistent(this);
    }

    private void OnDataFromServer(OtherPlayerSyncData data)
    {
        OtherPlayer onlinePlayer;

        // Kiểm tra thêm nếu object đã bị destroy
        if (!otherPlayers.TryGetValue(data.otherPlayerData.idAccount, out onlinePlayer))
        {
            onlinePlayer = new OtherPlayer();
            onlinePlayer.otherPlayerData = data.otherPlayerData;

            switch (data.otherPlayerData.idSchool)
            {
                case 1:
                    onlinePlayer.otherPlayerObject = PoolManager.Instance.Get(otherPlayersPrefab[0]);
                    break;
                case 2:
                    onlinePlayer.otherPlayerObject = PoolManager.Instance.Get(otherPlayersPrefab[1]);
                    break;
                case 3:
                    onlinePlayer.otherPlayerObject = PoolManager.Instance.Get(otherPlayersPrefab[2]);
                    break;
            }

            onlinePlayer.syncSpriteController = onlinePlayer.otherPlayerObject.GetComponent<SyncSpriteController>();
            onlinePlayer.syncSpriteController.ApplyServerData(data.otherPlayerData);

            otherPlayers.Add(data.otherPlayerData.idAccount, onlinePlayer);
        }
        else
        {
            onlinePlayer.otherPlayerData = data.otherPlayerData;
            onlinePlayer.syncSpriteController.ApplyServerData(data.otherPlayerData);
        }

        lastUpdateTime[data.otherPlayerData.idAccount] = Time.time;
    }
    private void OnRealtimeDataFromServer(OtherPlayerSyncData data)
    {
        if (otherPlayers.TryGetValue(data.otherPlayerData.idAccount, out OtherPlayer onlinePlayer))
        {
            onlinePlayer.otherPlayerData.maxHP = data.otherPlayerData.maxHP;
            onlinePlayer.otherPlayerData.hp = data.otherPlayerData.hp;
            onlinePlayer.otherPlayerData.currentTile = data.otherPlayerData.currentTile;

            onlinePlayer.otherPlayerObject.transform.SetPositionAndRotation(new Vector2(data.otherPlayerTransformData.positionData.x, data.otherPlayerTransformData.positionData.y), Quaternion.identity);
            onlinePlayer.syncSpriteController.ApplyServerRealtimeData(data.otherPlayerData, data.otherPlayerTransformData, data.otherPlayerStateData);

            lastUpdateTime[data.otherPlayerData.idAccount] = Time.time;
        }
    }

    public Dictionary<int, OtherPlayer> GetOtherPlayers()
    {
        return otherPlayers;
    }
    public OtherPlayer GetOtherPlayerByID(int idAccount)
    {
        if (otherPlayers.TryGetValue(idAccount, out OtherPlayer otherPlayer))
        {
            return otherPlayer;
        }
        return null;
    }
    public void PrepareForLogOut()
    {
        foreach (var kv in otherPlayers)
        {
            if (kv.Value != null)
            {
                PoolManager.Instance.Release(kv.Value.otherPlayerObject);
            }
        }
        otherPlayers.Clear();
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
