using Newtonsoft.Json;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;

[Serializable]
public class LogOutRequestPacket
{
    public string cmd;
    public int idAccount;
}

public class LogOutController : MonoBehaviour, IUpdatable
{
    private SocketManager socketManager;
    private PacketSerializeManager packetSerializeManager;
    private string logOutData;

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

    public void OnUpdate()
    {
        if (string.IsNullOrEmpty(logOutData))
            return;

        LogOutRequestPacket logOutResult = packetSerializeManager.HandleReceivedPacket<LogOutRequestPacket>(logOutData);

        if (logOutResult.idAccount != LogInView.GetIDAccount())
        {
            return;
        }

        //Dọn sạch danh sách quản lý Other Players trước khi chuyển về Main Scene
        GameObject.Find("SyncManager").gameObject.GetComponent<SyncManager>().PrepareForLogOut();

        //Dọn sạch danh sách quản lý Queue nhận dữ liệu từ Server
        socketManager.ClearAllQueues();

        if (EquipmentView.GetListEquipmentSlots() != null)
        {
            EquipmentView.ClearEquipmentData();
        }
        if (EquipmentView.GetListImagesEquipmentSlots() != null)
        {
            EquipmentView.ClearListImagesEquipmentSlots();
        }

        if (InventoryView.GetListInventorySlots() != null)
        {
            InventoryView.ClearInventoryData();
        }

        GameManager.Instance.GetComponent<PlayerManager>().DestroyPlayer();

        logOutData = string.Empty;

        SceneManager.LoadScene("Main");
    }
    public void OnLateUpdate() { }
    public void OnFixedUpdate() { }
    public void RegisterDontDestroyOnLoad()
    {
        GameManager.Instance.RegisterPersistent(this);
    }

    public void CLickLogOut()
    {
        LogOutRequestPacket sendLogOutRequestPacket = new LogOutRequestPacket
        {
            cmd = "logout",
            idAccount = LogInView.GetIDAccount() ?? 0,
        };

        packetSerializeManager.HandleSentPacket(sendLogOutRequestPacket);
    }

    public void SetLogOutData(string data)
    {
        logOutData = data;
    }
}