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
}
public class PlayerSyncData
{
    public PlayerData playerData;
    public PlayerTransformData playerTransformData;
    public PlayerStateData playerStateData;
}
public class PlayerSyncDataRequestPacket
{
    public string cmd;
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
    public string cmd;
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

        string logInData = socketManager.GetSyncOtherPlayersData(); //luôn lấy từ sync queue vì online player luôn gửi data đến server khi online
        string logOutData = socketManager.GetLogOutData();

        if (!string.IsNullOrEmpty(logInData))
        {
            var packet = packetSerializeManager.HandleReceivedPacket<SyncOtherPlayersResultPacket>(logInData);

            if (packet?.otherPlayersData == null)
                return;

            foreach (var playerData in packet.otherPlayersData)
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
        if (!string.IsNullOrEmpty(logOutData))
        {
            var offlinePlayer = packetSerializeManager.HandleReceivedPacket<LogOutRequestPacket>(logOutData);
            GameObject.Find("LogOut").GetComponent<LogOutController>().SetLogOutData(logOutData);
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
