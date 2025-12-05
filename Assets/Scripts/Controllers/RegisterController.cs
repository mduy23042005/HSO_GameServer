using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net.Http;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.U2D.Animation;

public class RegisterController : MonoBehaviour
{
    [Header("Quản lý Input")]
    [SerializeField] private TMP_InputField inputNameChar;
    [SerializeField] private TMP_InputField inputUsername;
    [SerializeField] private TMP_InputField inputPassword;
    [SerializeField] private TMP_Text inputNameHair;
    [SerializeField] private TMP_Text inputNameBlessing;

    [Header("Quản lý danh sách kiểu tóc")]
    [SerializeField] private List<SpriteLibrary> spriteLibrary;
    private List<HairLibraries> hairLibraries;

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

    private APIManager api;

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
        api = Object.FindFirstObjectByType<APIManager>();
    }
    #region ChangeHair
    private bool CheckOptionSchool()
    {
        if (idSchool == 0) return false;
        switch (idSchool)
        {
            case 1: nameSchool = "ChienBinh"; break;
            case 2: nameSchool = "SatThu"; break;
            case 3: nameSchool = "PhapSu"; break;
            case 4: nameSchool = "XaThu"; break;
            default: return false;
        }

        hairLibraries = GameObject.Find(nameSchool).GetComponent<SpriteController>().GetListHair();
        EquipHair(idHair[idSchool - 1]);
        UpdateNameHair();
        return true;
    }
    public void OnSelectSchool()
    {
        idSchool = DemoController.GetIDSchool();
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
        idHair[idSchool - 1] = (idHair[idSchool - 1] + 1) % hairLibraries.Count;
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
        idHair[idSchool - 1] = (idHair[idSchool - 1] - 1 + hairLibraries.Count) % hairLibraries.Count;
        EquipHair(idHair[idSchool - 1]);
        UpdateNameHair();
    }

    private void EquipHair(int hairIndex)
    {
        spriteLibrary[idSchool - 1].spriteLibraryAsset = hairLibraries[hairIndex].hairLibrariesAsset;
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

    public async void ClickRegister()
    {
        idSchool = DemoController.GetIDSchool();
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

        int weapon = 0, helmet = 0, armor = 0, legArmor = 0;
        switch (idSchool)
        {
            case 1: weapon = 1; helmet = 9; armor = 17; legArmor = 25; break;
            case 2: weapon = 2; helmet = 10; armor = 18; legArmor = 26; break;
            case 3: weapon = 3; helmet = 11; armor = 19; legArmor = 27; break;
            case 4: weapon = 4; helmet = 12; armor = 20; legArmor = 28; break;
        }

        if (!CheckAllInfo(idSchool, inputNameChar, inputUsername, inputPassword)) return;

        // Tạo object để gửi lên API
        var registerData = new
        {
            Account = new Account
            {
                Username = username,
                Password = password,
                NameChar = nameChar,
                IDSchool = idSchool,
                Level = 1,
                SkillPoints = 0,
                StatPoints = 0,
                Exp = 0,
                Hair = hair,
                Gold = 20000,
                Gem = 2000,
                Point0 = 0,
                Point1 = 0,
                Point2 = 0,
                Point3 = 0,
                PointArena = 0,
                PointActive = 100000,
                Skill0 = 1,
                Skill1 = 0,
                Skill2 = 0,
                Skill3 = 0,
                Skill4 = 0,
                Skill5 = 0,
                Skill6 = 0,
                Skill7 = 0,
                Skill8 = 0,
                Skill9 = 0,
                Skill10 = 0,
                Skill11 = 0,
                Skill12 = 0,
                Skill13 = 0,
                Skill14 = 0,
                Skill15 = 0,
                Skill16 = 0,
                Skill17 = 0,
                Skill18 = 0,
                Skill19 = 0,
                Skill20 = 0,
                Clan = null,
                BlessingPoints = blessingPoints
            },
            Equipment = new List<Account_Equipment>
        {
            new Account_Equipment { IDItem0_1 = weapon, SlotName = "Weapon", Category = 1 },
            new Account_Equipment { IDItem0_1 = helmet, SlotName = "Helmet", Category = 1 },
            new Account_Equipment { IDItem0_1 = armor, SlotName = "Armor", Category = 1 },
            new Account_Equipment { IDItem0_1 = legArmor, SlotName = "LegArmor", Category = 1 },
            new Account_Equipment { IDItem0_1 = 0, SlotName = "Gloves", Category = 1 },
            new Account_Equipment { IDItem0_1 = 0, SlotName = "Shoes", Category = 1 },
            new Account_Equipment { IDItem0_1 = 0, SlotName = "Ring1", Category = 1 },
            new Account_Equipment { IDItem0_1 = 0, SlotName = "Ring2", Category = 1 },
            new Account_Equipment { IDItem0_1 = 0, SlotName = "Necklace", Category = 1 },
            new Account_Equipment { IDItem0_1 = 0, SlotName = "Medal", Category = 1 },
            new Account_Equipment { IDItem0_1 = 0, SlotName = "Cloak", Category = 1 },
            new Account_Equipment { IDItem0_1 = 0, SlotName = "Wing", Category = 1 },
            new Account_Equipment { IDItem0_1 = 0, SlotName = "SkinWing", Category = 1 },
            new Account_Equipment { IDItem0_1 = 0, SlotName = "Mounts", Category = 1 },
            new Account_Equipment { IDItem0_1 = 0, SlotName = "Pet", Category = 1 },
            new Account_Equipment { IDItem0_1 = 0, SlotName = "Skin", Category = 1 },
        }
        };

        try
        {
            string json = JsonConvert.SerializeObject(registerData);
            var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");

            HttpResponseMessage result = await api.GetHttpClient().PostAsync($"{api.GetApiUrl()}/api/account/register", content);

            if (result.IsSuccessStatusCode)
            {
                Debug.Log("Đăng ký thành công!");
                SceneManager.LoadScene("Main");
            }
            else
            {
                string errorMsg = await result.Content.ReadAsStringAsync();
                Debug.LogError($"Đăng ký thất bại: {errorMsg}");
                // Hiển thị lỗi lên UI nếu cần
            }
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"Lỗi khi gọi API: {ex.Message}");
        }
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