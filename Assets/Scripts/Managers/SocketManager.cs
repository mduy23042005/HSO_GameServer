using Newtonsoft.Json;
using System;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public class SocketManager : MonoBehaviour
{
    // Singleton instance
    public static SocketManager Instance;

    private ClientWebSocket socket;
    private Uri serverUri = new Uri("wss://localhost:55556/");

    public static Action<SyncModels> OnSyncData;
    private void Awake()
    {
        // Nếu chưa có instance thì gán và giữ qua các scene
        if (Instance == null)
        {
            Instance = this;
            Application.runInBackground = true;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject); // Nếu đã có thì destroy bản duplicate
        }
    }

    async void Start()
    {
        await TestSocket();
    }

    // Kết nối đến WebSocket server
    public async Task TestSocket()
    {
        if (socket != null && socket.State == WebSocketState.Open)
            return;

        socket = new ClientWebSocket();

        try
        {
            await socket.ConnectAsync(serverUri, CancellationToken.None);
            Debug.Log("Socket: Kết nối Server thành công.");
        }
        catch (Exception e)
        {
            Debug.LogError("Socket: Kết nối Server thất bại" + e.Message);
        }
    }
    // Gửi data lên server
    public async Task SendSyncDataToServer(string message)
    {
        if (socket != null && socket.State == WebSocketState.Open)
        {
            var data = Encoding.UTF8.GetBytes(message);
            await socket.SendAsync(new ArraySegment<byte>(data), WebSocketMessageType.Text, true, CancellationToken.None);
        }
    }
    public ClientWebSocket GetSocket()
    {
        return socket;
    }
    private void OnApplicationQuit()
    {
        if (socket != null)
            socket.Dispose();
    }
}
