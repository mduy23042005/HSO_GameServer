using Newtonsoft.Json.Linq;
using System;
using System.Collections.Concurrent;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public class SocketManager : MonoBehaviour
{
    private ClientWebSocket socket;
    private Uri serverUri;

    private readonly ConcurrentQueue<string> sendQueue = new ConcurrentQueue<string>();
    private readonly ConcurrentQueue<string> receiveQueue = new ConcurrentQueue<string>();

    private readonly ConcurrentQueue<string> syncDataQueue = new ConcurrentQueue<string>();

    private readonly ConcurrentQueue<string> logInQueue = new ConcurrentQueue<string>();
    private readonly ConcurrentQueue<string> logOutQueue = new ConcurrentQueue<string>();
    private readonly ConcurrentQueue<string> registerQueue = new ConcurrentQueue<string>();

    private readonly ConcurrentQueue<string> inventoryQueue = new ConcurrentQueue<string>();
    private readonly ConcurrentQueue<string> inventoryAttributesQueue = new ConcurrentQueue<string>();
    private readonly ConcurrentQueue<string> equipmentQueue = new ConcurrentQueue<string>();
    private readonly ConcurrentQueue<string> equipmentAttributesQueue = new ConcurrentQueue<string>();

    private readonly ConcurrentQueue<string> outfitSpritesQueue = new ConcurrentQueue<string>();

    private void Awake()
    {
#if UNITY_ANDROID
        serverUri = new Uri("ws://192.168.100.12:55556/"); //phải khai báo rõ IP LAN của Server cho thiết bị Android 
#elif UNITY_EDITOR || UNITY_STANDALONE
        serverUri = new Uri($"ws://{IPV4ConfigurationManager.GetLocalIPv4()}:55556/"); // dùng IP LAN tự động khi chạy trên máy tính
#endif
    }

    public async Task InitSocket()
    {
        if (socket != null && socket.State == WebSocketState.Open)
            return;

        socket = new ClientWebSocket();

        try
        {
            await socket.ConnectAsync(serverUri, CancellationToken.None);
            Debug.Log("Socket: Kết nối Server thành công!");

            _ = StartSendLoop();
            _ = ReceiveFromServer();
        }
        catch (Exception e)
        {
            Debug.LogError("Socket: Kết nối Server thất bại! " + e.Message);
        }
    }

    //Gửi Packet đến server
    public void SendToServer(string message)
    {
        sendQueue.Enqueue(message);
    }
    private async Task StartSendLoop()
    {
        while (true)
        {
            if (socket == null || socket.State != WebSocketState.Open)
            {
                await Task.Delay(50);
                continue;
            }

            if (sendQueue.TryDequeue(out var msg))
            {
                try
                {
                    byte[] data = Encoding.UTF8.GetBytes(msg);
                    await socket.SendAsync(new ArraySegment<byte>(data), WebSocketMessageType.Text, true, CancellationToken.None);
                }
                catch (Exception e)
                {
                    Debug.LogError("Send loop error: " + e.Message);
                }
            }
            else
            {
                await Task.Delay(1);
            }
        }
    }

    //Nhận Packet từ server
    public async Task ReceiveFromServer()
    {
        var buffer = new byte[4096];
        var messageBuffer = new StringBuilder();

        try
        {
            while (socket != null && socket.State == WebSocketState.Open)
            {
                WebSocketReceiveResult result;

                do
                {
                    result = await socket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);

                    if (result.MessageType == WebSocketMessageType.Close)
                        return;

                    messageBuffer.Append(Encoding.UTF8.GetString(buffer, 0, result.Count));

                } while (!result.EndOfMessage);

                string fullMessage = messageBuffer.ToString();
                messageBuffer.Clear();

                var token = JToken.Parse(fullMessage);

                switch (token.Type)
                { 
                    case JTokenType.Object:
                        {
                            string cmd = token["cmd"]?.ToString();
                            HandlePacket(cmd, fullMessage);
                            break;
                        }
                    case JTokenType.Array:
                        {
                            if (!token.HasValues)
                            {
                                continue;
                            }

                            if (token[0].Type != JTokenType.Object)
                            {
                                continue;
                            }

                            string cmd = token[0]["cmd"]?.ToString();

                            if (!string.IsNullOrEmpty(cmd))
                            {
                                HandlePacket(cmd, fullMessage);
                            }
                            else
                            {
                                Debug.LogWarning("cmd missing in array packet: " + fullMessage);
                            }
                            break;
                        }
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogError("Error receiving messages: " + e.Message);
        }
    }
    //Phân loại Packet nhận được từ server
    private void HandlePacket(string cmd, string json)
    {
        switch (cmd)
        {
            case "syncData":
                if (LogInView.GetIDAccount() != 0)
                {
                    syncDataQueue.Enqueue(json);
                }
                break;
            case "login_result":
                logInQueue.Enqueue(json);
                break;
            case "logout":
                logOutQueue.Enqueue(json);
                break;
            case "register_result":
                registerQueue.Enqueue(json);
                break;
            case "equipment_result":
                equipmentQueue.Enqueue(json);
                break;
            case "equipmentAttributes_result":
                equipmentAttributesQueue.Enqueue(json);
                break;
            case "inventory_result":
                inventoryQueue.Enqueue(json);
                break;
            case "inventoryAttributes_result":
                inventoryAttributesQueue.Enqueue(json);
                break;
            case "outfitSprites_result":
                outfitSpritesQueue.Enqueue(json);
                break;

            default:
                receiveQueue.Enqueue(json);
                break;
        }
    }

    public string GetReceiveData()
    {
        if (receiveQueue.TryDequeue(out var data))
            return data;

        return null;
    }
    public string GetSyncData()
    {
        if (syncDataQueue.TryDequeue(out var data))
            return data;
        return null;
    }
    public string GetLogInData()
    {
        if (logInQueue.TryDequeue(out var data))
            return data;
        return null;
    }
    public string GetLogOutData()
    {
        if (logOutQueue.TryDequeue(out var data))
            return data;
        return null;
    }
    public string GetRegisterData()
    {
        if (registerQueue.TryDequeue(out var data))
            return data;
        return null;
    }
    public string GetInventoryData()
    {
        if (inventoryQueue.TryDequeue(out var data))
            return data;
        return null;
    }
    public string GetEquipmentData()
    {
        if (equipmentQueue.TryDequeue(out var data))
            return data;
        return null;
    }
    public string GetInventoryAttributesData()
    {
        if (inventoryAttributesQueue.TryDequeue(out var data))
            return data;
        return null;
    }
    public string GetEquipmentAttributesData()
    {
        if (equipmentAttributesQueue.TryDequeue(out var data))
        {
            return data;
        }
        return null;
    }
    public string GetOutfitSpritesData()
    {
        if (outfitSpritesQueue.TryDequeue(out var data))
        {
            return data;
        }

        return null;
    }

    private async void OnApplicationQuit()
    {
        if (socket != null)
        {
            if (socket.State == WebSocketState.Open)
            {
                try
                {
                    await socket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Client quitting", CancellationToken.None);
                }
                catch (Exception e)
                {
                    Debug.LogWarning("Error closing WebSocket: " + e.Message);
                }
            }
            socket.Dispose();
        }
    }

    public void ClearAllQueues()
    {
        ClearQueue(sendQueue);
        ClearQueue(receiveQueue);

        ClearQueue(syncDataQueue);

        ClearQueue(logInQueue);
        ClearQueue(logOutQueue);
        ClearQueue(registerQueue);

        ClearQueue(inventoryQueue);
        ClearQueue(inventoryAttributesQueue);
        ClearQueue(equipmentQueue);
        ClearQueue(equipmentAttributesQueue);

        ClearQueue(outfitSpritesQueue);
    }
    private void ClearQueue(ConcurrentQueue<string> queue)
    {
        while (queue.TryDequeue(out _)) { }
    }
}
