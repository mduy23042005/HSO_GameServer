using System.Collections.Generic;
using UnityEngine;

public class SyncOtherPlayersManager : MonoBehaviour, IUpdatable
{
    [SerializeField] private List<GameObject> otherPlayersPrefab;

    private Dictionary<int, GameObject> otherPlayers = new Dictionary<int, GameObject>();
    private Dictionary<int, PlayerData> otherPlayersData = new Dictionary<int, PlayerData>();

    private Dictionary<int, float> lastUpdateTime = new Dictionary<int, float>();
    private const float timeOut = 0.55f;

    private PlayerData onlinePlayer;
    private LogOutRequestPacket offlinePlayer;

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
            if (otherPlayers.TryGetValue(id, out GameObject obj))
            {
                Destroy(obj);
                otherPlayers.Remove(id);
                otherPlayersData.Remove(id);
                lastUpdateTime.Remove(id);
            }
        }

        string logInData = socketManager.GetSyncOtherPlayersData(); //luôn lấy từ sync queue vì online player luôn gửi data đến server khi online
        string logOutData = socketManager.GetLogOutData();

        if (!string.IsNullOrEmpty(logInData))
        {
            var data = packetSerializeManager.HandleReceivedPacket<SyncOtherPlayersResultPacket>(logInData);
            foreach (var playerData in data.otherPlayersData)
            {
                onlinePlayer = playerData;
                if (onlinePlayer.idAccount != LogInView.GetIDAccount())
                {
                    OnDataFromServer(onlinePlayer);
                }
            }
        }
        if (!string.IsNullOrEmpty(logOutData))
        {
            offlinePlayer = packetSerializeManager.HandleReceivedPacket<LogOutRequestPacket>(logOutData);
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

    public void OnDataFromServer(PlayerData data)
    {
        GameObject obj;

        // Kiểm tra thêm nếu object đã bị destroy
        if (!otherPlayers.TryGetValue(data.idAccount, out obj))
        {
            switch (data.idSchool)
            {
                case 1:
                    obj = Instantiate(otherPlayersPrefab[0], new Vector2(data.posX, data.posY), Quaternion.identity);
                    break;
                case 2:
                    obj = Instantiate(otherPlayersPrefab[1], new Vector2(data.posX, data.posY), Quaternion.identity);
                    break;
                case 3:
                    obj = Instantiate(otherPlayersPrefab[2], new Vector2(data.posX, data.posY), Quaternion.identity);
                    break;
            }

            otherPlayers.Add(data.idAccount, obj);
            otherPlayersData.Add(data.idAccount, data);

            obj.GetComponentInChildren<SyncMovementController>().ApplyServerState(data);
            obj.GetComponentInChildren<SyncSpriteController>().ApplyServerState(data);
        }
        else
        {
            obj = otherPlayers[data.idAccount];
            obj.GetComponentInChildren<SyncMovementController>().ApplyServerState(data);
            obj.GetComponentInChildren<SyncSpriteController>().ApplyServerState(data);
        }
        lastUpdateTime[data.idAccount] = Time.time;
    }
    public void OffDataFromServer(LogOutRequestPacket data)
    {
        if (otherPlayers.TryGetValue(data.idAccount, out GameObject obj))
        {
            Destroy(obj.gameObject);
            otherPlayers.Remove(data.idAccount);
            otherPlayersData.Remove(data.idAccount);
        }
    }

    public void PrepareForLogOut()
    {
        foreach (var kv in otherPlayers)
        {
            if (kv.Value != null)
            {
                Destroy(kv.Value);
            }
        }
        otherPlayers.Clear();
        otherPlayersData.Clear();
    }
}
