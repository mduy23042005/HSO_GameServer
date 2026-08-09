using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LogInRequestPacket
{
    public EnumCmdCode cmd;
    public string username;
    public string password;
}
public class LogInResultPacket
{
    public EnumCmdCode cmd;
    public bool success;
    public int idAccount;
    public int idSchool;
    public string nameChar;
    public int hair;
    public int level;
    public int maxHP;
    public int maxMP;
    public int hp;
    public int mp;
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
    private static string nameChar;
    private static int level;
    private static int maxHP;
    private static int maxMP;
    private static int hp;
    private static int mp;

    private SocketManager socketManager;

    private const float timeOut = 10f;
    private bool isLoggingIn = false;
    private float startTime;

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
        if (!isLoggingIn)
            return;

        byte[] data = socketManager.GetLogInData();

        if (data != null && data.Length > 0)
        {
            PacketReaderManager reader = new PacketReaderManager(data);

            LogInResultPacket loginResult = new LogInResultPacket();
            loginResult.cmd = (EnumCmdCode)reader.ReadInt();
            loginResult.success = reader.ReadBool();
            loginResult.idAccount = reader.ReadInt();
            loginResult.idSchool = reader.ReadInt();
            loginResult.nameChar = reader.ReadString();
            loginResult.hair = reader.ReadInt();
            loginResult.level = reader.ReadInt();
            loginResult.maxHP = reader.ReadInt();
            loginResult.maxMP = reader.ReadInt();
            loginResult.hp = reader.ReadInt();
            loginResult.mp = reader.ReadInt();
            loginResult.message = reader.ReadString();
            
            if (loginResult.success)
            {
                LoadLoginData(loginResult);
            }
            else
            {
                textMessage.color = Color.red;
                textMessage.text = loginResult.message;
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

    private void LoadLoginData(LogInResultPacket loginResult)
    {
        idAccount = loginResult.idAccount;
        idSchool = loginResult.idSchool;
        idHair = loginResult.hair;
        nameChar = loginResult.nameChar;
        level = loginResult.level;
        maxHP = loginResult.hp;
        maxMP = loginResult.mp;
        hp = loginResult.hp;
        mp = loginResult.mp;

        textMessage.color = Color.green;
        textMessage.text = loginResult.message;

        while (EquipmentView.equipments == null || EquipmentView.equipments.Count == 0)
        {
            byte[] data = socketManager.GetEquipmentData();

            if (data == null || data.Length == 0)
                return;

            PacketReaderManager reader = new PacketReaderManager(data);

            EquipmentResultPacket equipmentResult = new EquipmentResultPacket();
            equipmentResult.cmd = (EnumCmdCode)reader.ReadInt();
            equipmentResult.equipmentData = new List<EquipmentData>();

            int countEquipmentData = reader.ReadInt();
            for (int i = 0; i < countEquipmentData; i++)
            {
                equipmentResult.equipmentData.Add(new EquipmentData
                {
                    id = reader.ReadInt(),
                    idItem0_1 = reader.ReadInt(),
                    nameItem0_1 = reader.ReadString(),
                    category = reader.ReadInt(),
                    slotName = reader.ReadString()
                });
            }

            for (int i = 0; i < equipmentResult.equipmentData.Count; i++)
            {
                if (i >= EquipmentView.equipments.Count)
                {
                    EquipmentView.equipments.Add(equipmentResult.equipmentData[i]);
                }
                else
                {
                    EquipmentView.equipments[i] = equipmentResult.equipmentData[i];
                }
            }
        }
        while (InventoryView.inventoryItem0s == null || InventoryView.inventoryItem0s.Count == 0)
        {
            byte[] data = socketManager.GetInventoryData();

            if (data == null || data.Length == 0)
                return;

            PacketReaderManager reader = new PacketReaderManager(data);

            InventoryResultPacket inventoryResult = new InventoryResultPacket();
            inventoryResult.cmd = (EnumCmdCode)reader.ReadInt();
            inventoryResult.inventoryItem0Data = new List<InventoryItem0Data>();
            inventoryResult.inventoryItem1Data = new List<InventoryItem1Data>();
            inventoryResult.inventoryItem2Data = new List<InventoryItem2Data>();
            inventoryResult.inventoryItem3Data = new List<InventoryItem3Data>();
            inventoryResult.inventoryItem4Data = new List<InventoryItem4Data>();

            int countInventoryItem0Data = reader.ReadInt();
            for (int i = 0; i < countInventoryItem0Data; i++)
            {
                inventoryResult.inventoryItem0Data.Add(new InventoryItem0Data
                {
                    id = reader.ReadInt(),
                    idItem0 = reader.ReadInt(),
                    nameItem0 = reader.ReadString(),
                    typeItem0 = reader.ReadString(),
                    category = reader.ReadInt(),
                    idSchool = reader.ReadInt(),
                });
            }

            if (InventoryView.inventoryItem0s == null)
                InventoryView.inventoryItem0s = new List<InventoryItem0Data>();

            for (int i = 0; i < inventoryResult.inventoryItem0Data.Count; i++)
            {
                if (i >= InventoryView.inventoryItem0s.Count)
                {
                    InventoryView.inventoryItem0s.Add(inventoryResult.inventoryItem0Data[i]);
                }
                else
                {
                    InventoryView.inventoryItem0s[i] = inventoryResult.inventoryItem0Data[i];
                }
            }
        }

        isLoggingIn = false;
        SceneManager.LoadScene("Ngôi Làng Nhỏ");
    }

    private async Task LogIn()
    {
        textMessage.text = "";
        await socketManager.InitSocket();

        string username = inputUsername.text.Trim();
        string password = inputPassword.text.Trim();

        LogInRequestPacket logInRequestPacket = new LogInRequestPacket
        {
            cmd = EnumCmdCode.login,
            username = username,
            password = password
        };

        PacketWriterManager writer = new PacketWriterManager();
        writer.WriteInt((int)logInRequestPacket.cmd);
        writer.WriteString(logInRequestPacket.username);
        writer.WriteString(logInRequestPacket.password);

        await socketManager.SendToServer(writer.ToArray());

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
    public static string GetNameChar()
    {
        return nameChar;
    }
    public static int GetLevel()
    {
        return level;
    }
    public static int GetMaxHP()
    {
        return maxHP;
    }
    public static int GetMaxMP()
    {
        return maxMP;
    }
    public static int GetHP()
    {
        return hp;
    }
    public static int GetMP()
    {
        return mp;
    }
}
