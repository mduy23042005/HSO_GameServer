using System.Collections.Generic;
using UnityEngine;

public enum PlayerState
{
    Stand = 0,
    Move = 1,
    Attack = 2,
    Injured = 3,
    Die = 4
}
public enum Direction
{
    Front = 0,
    Back = 1,
    Left = 2,
    Right = 3,
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
    public string category;
    public string label;
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
}
public class SyncOtherPlayersManager : MonoBehaviour, IUpdatable
{
    [SerializeField] private List<GameObject> otherPlayersPrefab;

    private Dictionary<int, OtherPlayer> otherPlayers = new Dictionary<int, OtherPlayer>();

    private Dictionary<int, float> lastUpdateTime = new Dictionary<int, float>();
    private const float timeOut = 0.55f;

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

    public virtual void OnUpdate()
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
            if (otherPlayers.TryGetValue(id, out OtherPlayer obj))
            {
                Destroy(obj.otherPlayerObject);
                otherPlayers.Remove(id);
                lastUpdateTime.Remove(id);
            }
        }

        byte[] onlineData = socketManager.GetSyncOtherPlayersData(); //luôn lấy từ sync queue vì online player luôn gửi data đến server khi online
        byte[] offlineData = socketManager.GetLogOutData();

        if (onlineData != null && onlineData.Length > 0)
        {
            PacketReaderManager reader = new PacketReaderManager(onlineData);

            SyncOtherPlayersResultPacket data = new SyncOtherPlayersResultPacket();
            data.cmd = (EnumCmdCode)reader.ReadInt();
            data.otherPlayersData = new List<OtherPlayerSyncData>();

            int countOtherPlayerData = reader.ReadInt();
            for (int i = 0; i < countOtherPlayerData; i++)
            {
                data.otherPlayersData.Add(new OtherPlayerSyncData
                {
                    otherPlayerData = new PlayerData
                    {
                        idAccount = reader.ReadInt(),
                        nameChar = reader.ReadString(),
                        level = reader.ReadInt(),
                        idSchool = reader.ReadInt(),
                        hair = reader.ReadInt(),
                        weapon = reader.ReadInt(),
                        helmet = reader.ReadInt(),
                        armor = reader.ReadInt(),
                        legArmor = reader.ReadInt(),
                        gloves = reader.ReadInt(),
                        shoes = reader.ReadInt(),
                        ring1 = reader.ReadInt(),
                        ring2 = reader.ReadInt(),
                        necklace = reader.ReadInt(),
                        medal = reader.ReadInt(),
                        cloak = reader.ReadInt(),
                        wing = reader.ReadInt(),
                        skinWing = reader.ReadInt(),
                        mounts = reader.ReadInt(),
                        pet = reader.ReadInt(),
                        skin = reader.ReadInt(),
                    },
                    otherPlayerTransformData = new PlayerTransformData
                    {
                        positionData = new PositionData
                        {
                            x = reader.ReadFloat(),
                            y = reader.ReadFloat(),
                            z = reader.ReadFloat(),
                        },
                        scaleData = new ScaleData
                        {
                            x = reader.ReadFloat(),
                            y = reader.ReadFloat(),
                            z = reader.ReadFloat(),
                        }
                    },
                    otherPlayerStateData = new PlayerStateData
                    {
                        stateData = (PlayerState)reader.ReadInt(),
                        directionData = (Direction)reader.ReadInt(),
                        partBodyTransforms = new List<PartBodyData>()
                    }
                });

                int countPartBodyData = reader.ReadInt();
                for (int j = 0; j < countPartBodyData; j++)
                {
                    data.otherPlayersData[i].otherPlayerStateData.partBodyTransforms.Add(new PartBodyData
                    {
                        category = reader.ReadString(),
                        label = reader.ReadString(),
                        positionData =  new PositionData 
                        { 
                            x = reader.ReadFloat(),
                            y = reader.ReadFloat(),
                            z = reader.ReadFloat(),
                        },
                        rotationData = new RotationData
                        {
                            x = reader.ReadFloat(),
                            y = reader.ReadFloat(),
                            z = reader.ReadFloat(),
                        },
                        scaleData = new ScaleData
                        {
                            x = reader.ReadFloat(),
                            y = reader.ReadFloat(),
                            z = reader.ReadFloat(),
                        },
                        colorData = new ColorData
                        {
                            r = reader.ReadFloat(),
                            g = reader.ReadFloat(),
                            b = reader.ReadFloat(),
                            a = reader.ReadFloat(),
                        }
                    });
                }
            }

            if (data?.otherPlayersData == null)
                return;

            foreach (var playerData in data.otherPlayersData)
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
        if (offlineData != null && offlineData.Length > 0)
        {
            PacketReaderManager reader = new PacketReaderManager(offlineData);

            LogOutRequestPacket offlinePlayer = new LogOutRequestPacket();
            offlinePlayer.cmd = (EnumCmdCode)reader.ReadInt();
            offlinePlayer.idAccount = reader.ReadInt();

            GameObject.Find("LogOut").GetComponent<LogOutController>().SetLogOutData(offlinePlayer);
            OffDataFromServer(offlinePlayer);
        }
    }
    public virtual void OnLateUpdate() { }
    public virtual void OnFixedUpdate() { }
    public void RegisterDontDestroyOnLoad()
    {
        GameManager.Instance.RegisterPersistent(this);
    }

    public void OnDataFromServer(OtherPlayerSyncData data)
    {
        OtherPlayer onlinePlayer;

        // Kiểm tra thêm nếu object đã bị destroy
        if (!otherPlayers.TryGetValue(data.otherPlayerData.idAccount, out onlinePlayer))
        {
            onlinePlayer = new OtherPlayer();

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

            otherPlayers.Add(data.otherPlayerData.idAccount, onlinePlayer);

            onlinePlayer.otherPlayerObject.GetComponentInChildren<SyncSpriteController>().ApplyServerData(data.otherPlayerData, data.otherPlayerTransformData, data.otherPlayerStateData);
        }
        else
        {
            onlinePlayer.otherPlayerObject.GetComponentInChildren<SyncSpriteController>().ApplyServerData(data.otherPlayerData, data.otherPlayerTransformData, data.otherPlayerStateData);
        }
        lastUpdateTime[data.otherPlayerData.idAccount] = Time.time;
    }
    public void OffDataFromServer(LogOutRequestPacket data)
    {
        if (otherPlayers.TryGetValue(data.idAccount, out OtherPlayer otherPlayer))
        {
            Destroy(otherPlayer.otherPlayerObject);
            otherPlayers.Remove(data.idAccount);
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
}
