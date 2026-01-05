using Newtonsoft.Json;
using System.Collections.Generic;
using UnityEngine;

public class SyncManager : MonoBehaviour, IUpdatable
{
    [SerializeField] private GameObject otherPlayersPrefab;

    public static SyncManager Instance;
    private Dictionary<int, GameObject> otherPlayers = new Dictionary<int, GameObject>();
    private Dictionary<int, SyncModels> otherPlayersData = new Dictionary<int, SyncModels>();

    private SyncModels onlinePlayer;
    private SyncModels offlinePlayer;
    private SocketManager socketManager;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
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
        string syncData = socketManager.GetSyncData();
        string logOutData = socketManager.GetLogOutData();

        if (!string.IsNullOrEmpty(syncData))
        {
            onlinePlayer = JsonConvert.DeserializeObject<SyncModels>(syncData);
            if (onlinePlayer.idAccount != LogInView.GetIDAccount())
            {
                OnDataFromServer(onlinePlayer);
            }
        }
        return;
    }
    public virtual void OnLateUpdate() { }
    public virtual void OnFixedUpdate() { }
    public void RegisterDontDestroyOnLoad()
    {
        GameManager.Instance.RegisterPersistent(this);
    }

    public void OnDataFromServer(SyncModels data)
    {
        GameObject obj;

        // Kiểm tra thêm nếu object đã bị destroy
        if (!otherPlayers.TryGetValue(data.idAccount, out obj))
        {
            obj = Instantiate(otherPlayersPrefab, new Vector2(data.posX, data.posY), Quaternion.identity);

            otherPlayers.Add(data.idAccount, obj);
            otherPlayersData.Add(data.idAccount, data);

            OtherPlayersController opc = obj.GetComponent<OtherPlayersController>();
            if (opc != null)
            {
                opc.Init(data);
            }
            obj.GetComponentInChildren<SyncMovementController>().ApplyServerState(data);
            obj.GetComponentInChildren<SyncSpriteController>().ApplyServerState(data);

        }
        else
        {
            obj = otherPlayers[data.idAccount];
            obj.GetComponentInChildren<SyncMovementController>().ApplyServerState(data);
            obj.GetComponentInChildren<SyncSpriteController>().ApplyServerState(data);
        }
    }
    
    public void OffDataFromServer(SyncModels data)
    {
        if (otherPlayers.TryGetValue(data.idAccount, out GameObject obj))
        {
            // Xoá GameObject khỏi scene
            if (obj != null)
            {
                Destroy(obj.gameObject);
            }

            // Xoá khỏi dictionary
            otherPlayers.Remove(data.idAccount);
            otherPlayersData.Remove(data.idAccount);
        }
    }

    public SyncModels GetPlayerData(int idAccount)
    {
        if (otherPlayersData.TryGetValue(idAccount, out var data))
        {
            return data;
        }

        return null;
    }
}
