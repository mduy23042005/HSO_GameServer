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
    public string message;
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
    private PacketSerializeManager packetSerializeManager;
    private const float timeOut = 10f;
    private bool isLoggingIn = false;
    private float startTime;

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
        if (!isLoggingIn)
            return;

        string data = socketManager.GetLogInData();

        if (!string.IsNullOrEmpty(data))
        {
            LogInResultPacket logInResult = packetSerializeManager.HandleReceivedPacket<LogInResultPacket>(data);

            if (logInResult.success)
            {
                idAccount = logInResult.idAccount;
                idSchool = logInResult.idSchool;
                idHair = logInResult.hair;

                textMessage.color = Color.green;
                textMessage.text = logInResult.message;

                isLoggingIn = false;
                SceneManager.LoadScene("Ngôi Làng Nhỏ");
            }
            else
            {
                textMessage.color = Color.red;
                textMessage.text = logInResult.message;
                isLoggingIn = false;
            }
        }
        else
        {
            if (Time.time - startTime > timeOut)
            {
                textMessage.color = Color.red;
                textMessage.text = "Không thể kết nối tới server.";
                isLoggingIn = false;
            }
        }
    }
    public void OnLateUpdate() { }
    public void OnFixedUpdate() { }
    public void RegisterDontDestroyOnLoad()
    {
        GameManager.Instance.RegisterPersistent(this);
    }

    private async Task LogIn()
    {
        textMessage.text = "";
        await socketManager.InitSocket();

        string username = inputUsername.text.Trim();
        string password = inputPassword.text.Trim();

        LogInRequestPacket sendLogInRequestPacket = new LogInRequestPacket
        {
            cmd = "login",
            username = username,
            password = password
        };

        packetSerializeManager.HandleSentPacket(sendLogInRequestPacket);

        textMessage.color = Color.yellow;
        textMessage.text = "Đang đăng nhập...";
        isLoggingIn = true;
        startTime = Time.time;
    }
    public void ClickLogIn()
    {
        _ = LogIn();
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
    public static void SetIDAccount(int idAcc)
    {
        idAccount = idAcc;
    }
    public static int GetHair()
    {
        return idHair;
    }
}
