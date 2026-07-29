using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public enum PlayerState
{
    Stand = 0,
    Move = 1,
    Attack = 2,
    Injured = 3,
    Die = 4,
}
public enum Direction
{
    Front = 0,
    Back = 1,
    Left = 2,
    Right = 3,
}
public enum Category
{
    Stand = 0,
    Move = 1,
    Atk = 2,
    Injured = 3,
    Die = 4,
}
public enum Label
{
    StandFrontFrame0 = 4,
    StandFrontFrame1 = 5,
    StandBackFrame0 = 6,
    StandBackFrame1 = 7,
    StandLeftFrame0 = 8,
    StandLeftFrame1 = 9,
    StandRightFrame0 = 10,
    StandRightFrame1 = 11,

    MoveFrontFrame0 = 12,
    MoveFrontFrame1 = 13,
    MoveBackFrame0 = 14,
    MoveBackFrame1 = 15,
    MoveLeftFrame0 = 16,
    MoveLeftFrame1 = 17,
    MoveRightFrame0 = 18,
    MoveRightFrame1 = 19,

    AtkFrontFrame0 = 20,
    AtkFrontFrame1 = 21,
    AtkBackFrame0 = 22,
    AtkBackFrame1 = 23,
    AtkLeftFrame0 = 24,
    AtkLeftFrame1 = 25,
    AtkRightFrame0 = 26,
    AtkRightFrame1 = 27,

    InjuredFrontFrame0 = 28,
    InjuredFrontFrame1 = 29,
    InjuredBackFrame0 = 30,
    InjuredBackFrame1 = 31,
    InjuredLeftFrame0 = 32,
    InjuredLeftFrame1 = 33,
    InjuredRightFrame0 = 34,
    InjuredRightFrame1 = 35,

    DieFrame0 = 36
}
public class PositionData
{
    public float x;
    public float y;
    public float z;
}
public class RotationData
{
    public float x;
    public float y;
    public float z;
}
public class ScaleData
{
    public float x;
    public float y;
    public float z;
}
public class ColorData
{
    public float r;
    public float g;
    public float b;
    public float a;
}
public class PartBodyData
{
    public Category category;
    public Label label;
    public PositionData positionData;
    public RotationData rotationData;
    public ScaleData scaleData;
    public ColorData colorData;
}
public class PlayerStateData
{
    public PlayerState stateData;
    public Direction directionData;
    public List<PartBodyData> partBodyTransforms;
}
public class PlayerTransformData
{
    public PositionData positionData;
    public ScaleData scaleData;
}
public class PlayerData
{
    public int idAccount;
    public string nameChar;
    public int level;
    public int idSchool;
    public int hair;
    public int weapon;
    public int helmet;
    public int armor;
    public int legArmor;
    public int gloves;
    public int shoes;
    public int ring1;
    public int ring2;
    public int necklace;
    public int medal;
    public int cloak;
    public int wing;
    public int skinWing;
    public int mounts;
    public int pet;
    public int skin;
    public int maxHP;
    public int maxMP;
    public int hp;
    public int mp;
    public TileType currentTile;
}
public class PlayerSyncData
{
    public PlayerData playerData;
    public PlayerTransformData playerTransformData;
    public PlayerStateData playerStateData;
}
public class PlayerSyncDataRequestPacket
{
    public EnumCmdCode cmd;
    public PlayerSyncData playerSyncData;
}

public class OtherPlayerSyncData
{
    public PlayerData otherPlayerData;
    public PlayerTransformData otherPlayerTransformData;
    public PlayerStateData otherPlayerStateData;
}
public class SyncOtherPlayersResultPacket
{
    public EnumCmdCode cmd;
    public List<OtherPlayerSyncData> otherPlayersData;
}

public class OtherPlayer
{
    public GameObject otherPlayerObject;
    public PlayerData otherPlayerData;

    public Transform canvasTransform;
    public SyncSpriteController syncSpriteController;
}
public class SyncOtherPlayersManager : MonoBehaviour, IUpdatable
{
    [SerializeField] private List<GameObject> otherPlayersPrefab;
    [SerializeField] private GameObject updateHPUI;

    private Dictionary<int, OtherPlayer> otherPlayers = new Dictionary<int, OtherPlayer>();
    private readonly ConcurrentQueue<SyncOtherPlayersResultPacket> syncOtherPlayersResultPacketQueue = new ConcurrentQueue<SyncOtherPlayersResultPacket>();
    private readonly ConcurrentQueue<LogOutRequestPacket> syncLogOutOtherPlayerQueue = new ConcurrentQueue<LogOutRequestPacket>();
    private readonly ConcurrentQueue<(EnumCmdCode, int, int, int)> syncUpdateHPUIQueue = new ConcurrentQueue<(EnumCmdCode, int, int, int)>();

    private CancellationTokenSource syncTokenSource;

    private Dictionary<int, float> lastUpdateTime = new Dictionary<int, float>();
    private const float timeOut = 1f;

    private List<int> toRemove = new List<int>();

    private SocketManager socketManager;

    private void Awake()
    {
        socketManager = GameManager.Instance.GetComponent<SocketManager>();
        syncTokenSource = new CancellationTokenSource();
        _ = ReadSyncPacketLoop(syncTokenSource.Token);
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

    public async Task ReadSyncPacketLoop(CancellationToken token)
    {
        while (!token.IsCancellationRequested)
        {
            byte[] onlineData = socketManager.GetSyncOtherPlayersData();
            byte[] offlineData = socketManager.GetLogOutData();
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
                    otherPlayerSyncData.otherPlayerTransformData = new PlayerTransformData();
                    otherPlayerSyncData.otherPlayerTransformData.positionData = new PositionData();
                    otherPlayerSyncData.otherPlayerTransformData.scaleData = new ScaleData();
                    otherPlayerSyncData.otherPlayerStateData = new PlayerStateData();

                    otherPlayerSyncData.otherPlayerData.idAccount = reader.ReadInt();
                    otherPlayerSyncData.otherPlayerData.level = reader.ReadInt();
                    otherPlayerSyncData.otherPlayerData.idSchool = reader.ReadInt();
                    otherPlayerSyncData.otherPlayerData.hair = reader.ReadInt();
                    otherPlayerSyncData.otherPlayerData.weapon = reader.ReadInt();
                    otherPlayerSyncData.otherPlayerData.helmet = reader.ReadInt();
                    otherPlayerSyncData.otherPlayerData.armor = reader.ReadInt();
                    otherPlayerSyncData.otherPlayerData.legArmor = reader.ReadInt();
                    otherPlayerSyncData.otherPlayerData.maxHP = reader.ReadInt();
                    otherPlayerSyncData.otherPlayerData.hp = reader.ReadInt();
                    otherPlayerSyncData.otherPlayerData.currentTile = (TileType)reader.ReadInt();

                    otherPlayerSyncData.otherPlayerTransformData.positionData.x = reader.ReadFloat();
                    otherPlayerSyncData.otherPlayerTransformData.positionData.y = reader.ReadFloat();

                    otherPlayerSyncData.otherPlayerTransformData.scaleData.x = reader.ReadFloat();
                    otherPlayerSyncData.otherPlayerTransformData.scaleData.y = 1f;
                    otherPlayerSyncData.otherPlayerTransformData.scaleData.z = 1f;

                    otherPlayerSyncData.otherPlayerStateData.stateData = (PlayerState)reader.ReadInt();
                    otherPlayerSyncData.otherPlayerStateData.directionData = (Direction)reader.ReadInt();
                    otherPlayerSyncData.otherPlayerStateData.partBodyTransforms = new List<PartBodyData>();

                    data.otherPlayersData.Add(otherPlayerSyncData);

                    int countPartBodyData = reader.ReadInt();
                    for (int j = 0; j < countPartBodyData; j++)
                    {
                        PartBodyData partBodyData = new PartBodyData();
                        partBodyData.category = (Category)reader.ReadInt();
                        partBodyData.label = (Label)reader.ReadInt();

                        data.otherPlayersData[i].otherPlayerStateData.partBodyTransforms.Add(partBodyData);
                    }
                    
                    syncOtherPlayersResultPacketQueue.Enqueue(data);
                }
            }
            if (offlineData != null && offlineData.Length > 0)
            {
                PacketReaderManager reader = new PacketReaderManager(offlineData);

                LogOutRequestPacket offlinePlayer = new LogOutRequestPacket();
                offlinePlayer.cmd = (EnumCmdCode)reader.ReadInt();
                offlinePlayer.idAccount = reader.ReadInt();
                
                syncLogOutOtherPlayerQueue.Enqueue(offlinePlayer);
            }

            if (updateOtherPlayerHPUIData != null && updateOtherPlayerHPUIData.Length > 0)
            {
                PacketReaderManager reader1 = new PacketReaderManager(updateOtherPlayerHPUIData);
                EnumCmdCode cmd = (EnumCmdCode)reader1.ReadInt();
                int idAccount = reader1.ReadInt();
                int mobDamage = reader1.ReadInt();
                int otherPlayerHP = reader1.ReadInt();

                syncUpdateHPUIQueue.Enqueue((cmd, idAccount, mobDamage, otherPlayerHP));
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

        foreach (var id in toRemove)
        {
            if (otherPlayers.TryGetValue(id, out OtherPlayer obj))
            {
                Destroy(obj.otherPlayerObject);
                otherPlayers.Remove(id);
                lastUpdateTime.Remove(id);
            }
        }
        toRemove.Clear();

        SyncOtherPlayersResultPacket onlineData = null;
        LogOutRequestPacket offlineData = null;

        EnumCmdCode cmd = default;
        int idAccount = 0;
        int mobDamage = 0;
        int otherPlayerHP = 0;
        bool hasHPUpdate = false;

        if (syncOtherPlayersResultPacketQueue.TryDequeue(out var syncOnlineData))
            onlineData = syncOnlineData;

        if (syncLogOutOtherPlayerQueue.TryDequeue(out var syncOfflineData))
            offlineData = syncOfflineData;

        if (syncUpdateHPUIQueue.TryDequeue(out var syncUpdateHPUIData))
        {
            cmd = syncUpdateHPUIData.Item1;
            idAccount = syncUpdateHPUIData.Item2;
            mobDamage = syncUpdateHPUIData.Item3;
            otherPlayerHP = syncUpdateHPUIData.Item4;
            hasHPUpdate = true;
        }

        if (onlineData != null)
        {
            if (onlineData.otherPlayersData != null)
            {
                foreach (var playerData in onlineData.otherPlayersData)
                {
                    if (playerData == null)
                        continue;

                    if (playerData.otherPlayerData == null)
                        continue;

                    if (playerData.otherPlayerData.idAccount != LogInView.GetIDAccount())
                    {
                        OnDataFromServer(playerData);
                    }
                }
            }
        }

        if (offlineData != null)
        {
            GameObject.Find("LogOut").GetComponent<LogOutController>().SetLogOutData(offlineData);
            OffDataFromServer(offlineData);
        }

        if (hasHPUpdate)
        {
            if (otherPlayers.TryGetValue(idAccount, out OtherPlayer otherPlayer) && otherPlayer != null && otherPlayer.otherPlayerData != null)
            {
                if (otherPlayer.otherPlayerData.hp != otherPlayerHP)
                {
                    if (otherPlayerHP < otherPlayer.otherPlayerData.hp)
                    {
                        GameObject objectDamageUI = Instantiate(updateHPUI, otherPlayer.otherPlayerObject.GetComponentInChildren<Canvas>().transform, false);

                        UpdateHPUIController injuredDamageUI = objectDamageUI.GetComponent<UpdateHPUIController>();
                        if (injuredDamageUI != null)
                        {
                            injuredDamageUI.SetInjuredDamage(mobDamage);
                        }
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
                    onlinePlayer.otherPlayerObject = Instantiate(otherPlayersPrefab[0], new Vector2(data.otherPlayerTransformData.positionData.x, data.otherPlayerTransformData.positionData.y), Quaternion.identity);
                    break;
                case 2:
                    onlinePlayer.otherPlayerObject = Instantiate(otherPlayersPrefab[1], new Vector2(data.otherPlayerTransformData.positionData.x, data.otherPlayerTransformData.positionData.y), Quaternion.identity);
                    break;
                case 3:
                    onlinePlayer.otherPlayerObject = Instantiate(otherPlayersPrefab[2], new Vector2(data.otherPlayerTransformData.positionData.x, data.otherPlayerTransformData.positionData.y), Quaternion.identity);
                    break;
            }
            onlinePlayer.canvasTransform = onlinePlayer.otherPlayerObject.GetComponentInChildren<Canvas>().transform;
            onlinePlayer.syncSpriteController = onlinePlayer.otherPlayerObject.GetComponent<SyncSpriteController>();
            onlinePlayer.syncSpriteController.ApplyServerData(data.otherPlayerData, data.otherPlayerTransformData, data.otherPlayerStateData);

            otherPlayers.Add(data.otherPlayerData.idAccount, onlinePlayer);
        }
        else
        {
            onlinePlayer.otherPlayerData = data.otherPlayerData;
            onlinePlayer.syncSpriteController.ApplyServerData(data.otherPlayerData, data.otherPlayerTransformData, data.otherPlayerStateData);
        }

        Vector3 scale = onlinePlayer.canvasTransform.localScale;
        scale.x = data.otherPlayerStateData.directionData == Direction.Right ? -Math.Abs(scale.x) : Math.Abs(scale.x);
        onlinePlayer.canvasTransform.localScale = scale;

        lastUpdateTime[data.otherPlayerData.idAccount] = Time.time;
    }
    private void OffDataFromServer(LogOutRequestPacket data)
    {
        if (otherPlayers.TryGetValue(data.idAccount, out OtherPlayer otherPlayer))
        {
            Destroy(otherPlayer.otherPlayerObject);
            otherPlayers.Remove(data.idAccount);
        }
        if (lastUpdateTime.TryGetValue(data.idAccount, out float lastTime))
        {
            lastUpdateTime.Remove(data.idAccount);
        }
    }

    public Dictionary<int, OtherPlayer> GetOtherPlayers()
    {
        return otherPlayers;
    }
    public void PrepareForLogOut()
    {
        foreach (var kv in otherPlayers)
        {
            if (kv.Value != null)
            {
                Destroy(kv.Value.otherPlayerObject);
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
