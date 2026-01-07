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
    private string logOutData;

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
        if (string.IsNullOrEmpty(logOutData))
            return;

        LogOutRequestPacket logOutResult = JsonConvert.DeserializeObject<LogOutRequestPacket>(logOutData); ;

        if (logOutResult.idAccount != LogInView.GetIDAccount())
        {
            return;
        }
        //Dọn sạch danh sách quản lý Other Players trước khi chuyển về Main Scene
        SyncManager.Instance.PrepareForLogOut();

        //Dọn sạch danh sách quản lý Queue nhận dữ liệu từ Server
        socketManager.ClearAllQueues();

        //Xóa Player Object để reset đăng nhập lại
        GameObject.Find("Player").gameObject.GetComponent<PlayerController>().DestroyPlayerObject();

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

        string packet = JsonConvert.SerializeObject(sendLogOutRequestPacket);
        socketManager.SendToServer(packet);
    }

    public void SetLogOutData(string data)
    {
        logOutData = data;
    }

    private void OnApplicationQuit() 
    { }
}