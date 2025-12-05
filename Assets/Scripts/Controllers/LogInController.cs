using Newtonsoft.Json;
using System.Net.Http;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LogInController : MonoBehaviour
{
    [SerializeField] private TMP_InputField inputUsername;
    [SerializeField] private TMP_InputField inputPassword;
    [SerializeField] private TMP_Text textMessage;
    private static int idSchool;
    private static int idAccount;
    private APIManager api;

    private void Awake()
    {
        api = Object.FindFirstObjectByType<APIManager>();
    }
    public async Task<Account> LoginAsync(string username, string password) 
    { 
        string url = $"{api.GetApiUrl()}/api/account/login?username={username}&password={password}"; 
        try 
        { 
            HttpResponseMessage res = await api.GetHttpClient().GetAsync(url); 
            if (!res.IsSuccessStatusCode) return null; string json = await res.Content.ReadAsStringAsync(); 
            return JsonConvert.DeserializeObject<Account>(json); 
        } 
        catch 
        { 
            return null; 
        } 
    }
    public async void ClickLogIn()
    {
        string username = inputUsername.text;
        string password = inputPassword.text;

        var acc = await LoginAsync(username, password);

        if (acc != null)
        {
            textMessage.color = Color.green;
            textMessage.text = $"Đăng nhập {acc.NameChar} thành công.";

            idSchool = acc.IDSchool;
            idAccount = acc.IDAccount;

            SceneManager.LoadScene("Map1");
        }
        else
        {
            textMessage.color = Color.red;
            textMessage.text = "Username hoặc Password không đúng.";
        }
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
}
