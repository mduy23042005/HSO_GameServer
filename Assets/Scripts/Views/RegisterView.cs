using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.U2D.Animation;

public class RegisterRequestPacket
{
    public EnumCmdCode cmd;
    public int idSchool;
    public string nameChar;
    public string username;
    public string password;
    public int hair;
    public int blessingPoints;
}
public class RegisterResultPacket
{
    public EnumCmdCode cmd;
    public bool success;
}

public class RegisterView : MonoBehaviour, IUpdatable
{
    [Header("Quản lý Input")]
    [SerializeField] private TMP_InputField inputNameChar;
    [SerializeField] private TMP_InputField inputUsername;
    [SerializeField] private TMP_InputField inputPassword;
    [SerializeField] private TMP_Text inputNameHair;
    [SerializeField] private TMP_Text inputNameBlessing;

    [Header("Quản lý danh sách kiểu tóc")]
    [SerializeField] private List<SpriteLibrary> spriteLibrary;

    [Header("Xuất thông báo lỗi")]
    [SerializeField] private TMP_Text textMessageNameChar;
    [SerializeField] private TMP_Text textMessageUsername;
    [SerializeField] private TMP_Text textMessagePassword;

    [Header("Thông báo Error School")]
    [SerializeField] private Animator uiPickChienBinh;
    [SerializeField] private Animator uiPickSatThu;
    [SerializeField] private Animator uiPickPhapSu;
    [SerializeField] private Animator uiPickXaThu;

    private int idSchool = 0;
    private string nameSchool;
    private int[] idHair = new int[4];
    private int idBlessing = 0;
    private string[] nameBlessing;

    private SocketManager socketManager;

    private void Awake()
    {
        if (GameObject.Find("CharaterSelectionUI"))
        {
            uiPickChienBinh = GameObject.Find("UIPickChienBinh").GetComponent<Animator>();
            uiPickSatThu = GameObject.Find("UIPickSatThu").GetComponent<Animator>();
            uiPickPhapSu = GameObject.Find("UIPickPhapSu").GetComponent<Animator>();
            uiPickXaThu = GameObject.Find("UIPickXaThu").GetComponent<Animator>();
        }
        nameBlessing = new string[] { "Ánh sáng", "Bóng tối" };

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
        byte[] data = socketManager.GetRegisterData();

        if (data == null || data.Length == 0)
            return;

        Debug.Log("Received register data successfully!");

        PacketReaderManager reader = new PacketReaderManager(data);

        RegisterResultPacket registerResult = new RegisterResultPacket();
        registerResult.cmd = (EnumCmdCode)reader.ReadInt();
        registerResult.success = reader.ReadBool();

        if (registerResult.success)
        {
            Debug.Log("Đăng ký thành công!");
            SceneManager.LoadScene("Main");
        }
        else
        {
            Debug.LogError($"Đăng ký thất bại!");
        }
    }

    public void OnLateUpdate() { }
    public void OnFixedUpdate() { }
    public void RegisterDontDestroyOnLoad()
    {
        DontDestroyOnLoad(this.gameObject);
    }

    #region ChangeHair
    private bool CheckOptionSchool()
    {
        if (idSchool == 0) return false;
        switch (idSchool)
        {
            case 1: 
                nameSchool = "ChienBinh";
                break;
            case 2: 
                nameSchool = "SatThu"; 
                break;
            case 3: 
                nameSchool = "PhapSu"; 
                break;
            case 4: 
                nameSchool = "XaThu"; 
                break;
            default: 
                return false;
        }

        return true;
    }
    public void OnSelectSchool()
    {
        idSchool = RegisterDemoController.GetIDSchool();
        if (CheckOptionSchool())
        {
            EquipHair(idHair[idSchool - 1]);
            UpdateNameHair();
        }
    }
    public void NextHair()
    {
        if (idSchool == 0)
        {
            SendErrorSchool();
            return;
        }
        idHair[idSchool - 1] = (idHair[idSchool - 1] + 1) % 9; //9 là số kiểu tóc trong Male và Female Hair Libraries
        EquipHair(idHair[idSchool - 1]);
        UpdateNameHair();
    }
    public void PrevHair()
    {
        if (idSchool == 0)
        {
            SendErrorSchool();
            return;
        }
        idHair[idSchool - 1] = (idHair[idSchool - 1] - 1 + 9) % 9; //9 là số kiểu tóc trong Male và Female Hair Libraries
        EquipHair(idHair[idSchool - 1]);
        UpdateNameHair();
    }

    private void EquipHair(int hairIndex)
    {
        switch (nameSchool)
        {
            case "ChienBinh":
                spriteLibrary[idSchool - 1].spriteLibraryAsset = ItemController.Instance.GetMaleHairLibrary(hairIndex).hairLibrariesAsset;
                break;
            case "SatThu":
                spriteLibrary[idSchool - 1].spriteLibraryAsset = ItemController.Instance.GetMaleHairLibrary(hairIndex).hairLibrariesAsset;
                break;
            case "PhapSu":
                spriteLibrary[idSchool - 1].spriteLibraryAsset = ItemController.Instance.GetFemaleHairLibrary(hairIndex).hairLibrariesAsset;
                break;
            case "XaThu":
                spriteLibrary[idSchool - 1].spriteLibraryAsset = ItemController.Instance.GetFemaleHairLibrary(hairIndex).hairLibrariesAsset;
                break;
            default:
                return;
        }
    }
    private void UpdateNameHair()
    {
        inputNameHair.text = $"Kiểu: {idHair[idSchool - 1]}";
    }
    #endregion ChangeHair
    #region ChangeBlessing
    public void NextBlessing()
    {
        idBlessing = (idBlessing + 1) % nameBlessing.Length;
        inputNameBlessing.text = $"{nameBlessing[idBlessing]}";
    }
    public void PrevBlessing()
    {
        idBlessing = (idBlessing - 1 + nameBlessing.Length) % nameBlessing.Length;
        inputNameBlessing.text = $"{nameBlessing[idBlessing]}";
    }
    #endregion ChangeBlessing

    public void ClickRegister()
    {
        idSchool = RegisterDemoController.GetIDSchool();
        if (idSchool == 0)
        {
            SendErrorSchool();
            return;
        }

        string nameChar = inputNameChar.text.Trim();
        string username = inputUsername.text.Trim();
        string password = inputPassword.text.Trim();
        int hair = idHair[idSchool - 1];
        int blessingPoints = idBlessing;

        if (!CheckAllInfo(idSchool, inputNameChar, inputUsername, inputPassword)) return;

        RegisterRequestPacket registerRequestPacket = new RegisterRequestPacket
        {
            cmd = EnumCmdCode.register,
            idSchool = idSchool,
            nameChar = nameChar,
            username = username,
            password = password,
            hair = hair,
            blessingPoints = blessingPoints
        };

        PacketWriterManager writer = new PacketWriterManager();
        writer.WriteInt((int)registerRequestPacket.cmd);
        writer.WriteInt(registerRequestPacket.idSchool);
        writer.WriteString(registerRequestPacket.nameChar);
        writer.WriteString(registerRequestPacket.username);
        writer.WriteString(registerRequestPacket.password);
        writer.WriteInt(registerRequestPacket.hair);
        writer.WriteInt(registerRequestPacket.blessingPoints);

        socketManager.SendToServer(writer.ToArray());
    }

    private bool CheckAllInfo(int idSchool, TMP_InputField nameChar, TMP_InputField username, TMP_InputField password)
    {
        bool isValid = true;

        //Kiểm tra trường phái
        if (idSchool == 0)
        {
            SendErrorSchool();
            isValid = false;
        }

        //Kiểm tra tên nhân vật
        if (string.IsNullOrEmpty(nameChar.text))
        {
            textMessageNameChar.color = Color.red;
            textMessageNameChar.text = "!";
            isValid = false;
        }
        else
        {
            textMessageNameChar.text = "";
        }
        //Kiểm tra tên đăng nhập
        if (string.IsNullOrEmpty(username.text))
        {
            textMessageUsername.color = Color.red;
            textMessageUsername.text = "!";
            isValid = false;
        }
        else
        {
            textMessageUsername.text = "";
        }
        //Kiểm tra mật khẩu
        if (string.IsNullOrEmpty(password.text))
        {
            textMessagePassword.color = Color.red;
            textMessagePassword.text = "!";
            isValid = false;
        }
        else
        {
            textMessagePassword.text = "";
        }
        return isValid;
    }
    private void SendErrorSchool()
    {
        uiPickChienBinh.SetTrigger("Error");
        uiPickSatThu.SetTrigger("Error");
        uiPickPhapSu.SetTrigger("Error");
        uiPickXaThu.SetTrigger("Error");
    }
}