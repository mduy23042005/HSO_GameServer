using Newtonsoft.Json;
using System.Net.Http;
using System.Threading.Tasks;
using UnityEngine;

public class APIManager : MonoBehaviour, IUpdatable
{
    private readonly HttpClient client = new HttpClient();
    private const string apiUrl = "https://localhost:55555";

    private void OnEnable()
    {
        GameManager.Instance.Register(this);
        RegisterDontDestroyOnLoad();
        _ = TestAPI(); // gọi async load dữ liệu
    }
    private void OnDisable()
    {
        if (GameManager.Instance != null)
            GameManager.Instance.Unregister(this);
    }
    public void OnUpdate() { }
    public void OnFixedUpdate() { }
    public void OnLateUpdate() { }
    public void RegisterDontDestroyOnLoad()
    {
        GameManager.Instance.RegisterPersistent(this);
    }

    public string GetApiUrl()
    {
        return apiUrl;
    }
    public HttpClient GetHttpClient()
    {
        return client;
    }

    private async Task TestAPI()
    {
        try
        {
            HttpResponseMessage response = await client.GetAsync(apiUrl);

            if (response.IsSuccessStatusCode)
            {
                Debug.Log("API: Kết nối Database thành công.");
            }
            else
            {
                Debug.LogError($"API trả về mã lỗi: {response.StatusCode}");
            }
        }
        catch (System.Exception ex)
        {
            Debug.LogError("API: Kết nối Database thất bại." + ex.Message);
        }
    }

    public async Task<string> PostAsync(string endpoint)
    {
        try
        {
            string url = endpoint.StartsWith("https") ? endpoint : $"{apiUrl.TrimEnd('/')}/{endpoint.TrimStart('/')}";

            HttpResponseMessage response = await client.PostAsync(url, null);

            string result = await response.Content.ReadAsStringAsync();
            return result;
        }
        catch (System.Exception ex)
        {
            Debug.LogError("POST API lỗi: " + ex.Message);
            return null;
        }
    }
    public async Task<string> PostAsync(string endpoint, HttpContent content)
    {
        try
        {
            string url = endpoint.StartsWith("https")
                ? endpoint
                : $"{apiUrl.TrimEnd('/')}/{endpoint.TrimStart('/')}";

            HttpResponseMessage response = await client.PostAsync(url, content);

            string result = await response.Content.ReadAsStringAsync();
            return result;
        }
        catch (System.Exception ex)
        {
            Debug.LogError("POST API lỗi: " + ex.Message);
            return null;
        }
    }

}
