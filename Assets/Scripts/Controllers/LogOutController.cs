using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LogOutRequestPacket
{
    public EnumCmdCode cmd;
    public int idAccount;
}

public class LogOutController : MonoBehaviour, IUpdatable
{
    private SocketManager socketManager;
    private LogOutRequestPacket logOutData;

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
        if (logOutData == null)
            return;

        //Dọn sạch danh sách quản lý Other Players trước khi chuyển về Main Scene
        GameObject.Find("SyncManager").gameObject.GetComponent<SyncOtherPlayersManager>().PrepareForLogOut();

        //Dọn sạch danh sách quản lý Queue nhận dữ liệu từ Server
        socketManager.ClearAllQueues();

        if (EquipmentView.equipments != null)
        {
            EquipmentView.ClearEquipmentData();
        }
        if (EquipmentView.GetListImagesEquipmentSlots() != null)
        {
            EquipmentView.ClearListImagesEquipmentSlots();
        }
        if (InventoryView.inventoryItem0s != null)
        {
            InventoryView.ClearInventoryData();
        }

        GameManager.Instance.GetComponent<PlayerManager>().DestroyPlayer();

        logOutData = null;

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
        LogOutRequestPacket logOutRequestPacket = new LogOutRequestPacket
        {
            cmd = EnumCmdCode.logout,
            idAccount = LogInView.GetIDAccount() ?? 0,
        };

        PacketWriterManager writer = new PacketWriterManager();
        writer.WriteInt((int)logOutRequestPacket.cmd);
        writer.WriteInt(logOutRequestPacket.idAccount);

        _ = socketManager.SendToServer(writer.ToArray());
    }

    public void SetLogOutData(LogOutRequestPacket data)
    {
        logOutData = data;
    }
}