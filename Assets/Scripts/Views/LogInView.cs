using Newtonsoft.Json;
using System;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

[Serializable]
public class LogInRequestPacket
{
    public string cmd;
    public string username;
    public string password;
}

[Serializable]
public class LogInResultPacket
{
    public string cmd;
    public bool success;
    public int idAccount;
    public int idSchool;
    public string nameChar;
    public int hair;
}
[Serializable]
public class LogOutRequestPacket
{
    public string cmd;
    public int idAccount;
}

public class LogInView : MonoBehaviour, IUpdatable
{
    [SerializeField] private TMP_InputField inputUsername;
    [SerializeField] private TMP_InputField inputPassword;
    [SerializeField] private TMP_Text textMessage;

    private static int idSchool;
    private static int idAccount;
    private static int idHair;

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
    public void OnUpdate()
    {
        string data = socketManager.GetLogInData();

        if (string.IsNullOrEmpty(data))
            return;

        LogInResultPacket logInResult = JsonConvert.DeserializeObject<LogInResultPacket>(data);

        if (logInResult.cmd != "login_result")
            return;

        if (logInResult.success)
        {
            idAccount = logInResult.idAccount;
            idSchool = logInResult.idSchool;
            idHair = logInResult.hair;

            textMessage.color = Color.green;
            textMessage.text = $"Đăng nhập {logInResult.nameChar} thành công.";
            SceneManager.LoadScene("Map1");
        }
        else
        {
            textMessage.color = Color.red;
            textMessage.text = "Username hoặc Password không đúng.";
        }
    }
    public void OnLateUpdate() { }
    public void OnFixedUpdate() { }
    public void RegisterDontDestroyOnLoad()
    {
        GameManager.Instance.RegisterPersistent(this);
    }

    public void ClickLogIn()
    {
        string username = inputUsername.text.Trim();
        string password = inputPassword.text.Trim();

        LogInRequestPacket sendLogInRequestPacket = new LogInRequestPacket
        {
            cmd = "login",
            username = username,
            password = password
        };

        string packet = JsonConvert.SerializeObject(sendLogInRequestPacket);
        socketManager.SendToServer(packet);

        textMessage.color = Color.yellow;
        textMessage.text = "Đang đăng nhập...";
    }

    private void OnApplicationQuit()
    {
        LogOutRequestPacket sendLogOutRequestPacket = new LogOutRequestPacket
        {
            cmd = "logout",
            idAccount = idAccount
        };

        string packet = JsonConvert.SerializeObject(sendLogOutRequestPacket);
        socketManager.SendToServer(packet);
    }
    public void ClickRegister()
    {
        SceneManager.LoadScene("SelectCharacterScene");
    }
    public static int GetIDSchool()
    {
        return idSchool;
    }
    public static int? GetIDAccount() //nếu bấm vào Đăng ký thì idAccount = 0
    {
        return idAccount;
    }
    public static int GetHair()
    {
        return idHair;
    }
}
