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
    private Uri serverUri = new Uri("ws://localhost:55556/");

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

    private async void Start()
    {
        await TestSocket();

        _ = StartSendLoop();
        _ = ReceiveFromServer();
    }

    public async Task TestSocket()
    {
        if (socket != null && socket.State == WebSocketState.Open)
            return;

        socket = new ClientWebSocket();

        try
        {
            await socket.ConnectAsync(serverUri, CancellationToken.None);
            Debug.Log("Socket: Kết nối Server thành công!");
        }
        catch (Exception e)
        {
            Debug.LogError("Socket: Kết nối Server thất bại! " + e.Message);
        }
    }

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

    public async Task ReceiveFromServer()
    {
        var buffer = new byte[4096];

        try
        {
            while (socket != null && socket.State == WebSocketState.Open)
            {
                var result = await socket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);

                if (result.MessageType == WebSocketMessageType.Close)
                    break;

                string msg = Encoding.UTF8.GetString(buffer, 0, result.Count);
                var token = JToken.Parse(msg);

                switch (token.Type)
                {
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
                                HandlePacket(cmd, msg);
                            }
                            else
                            {
                                Debug.LogWarning("cmd missing in array packet: " + msg);
                            }
                            break;
                        }
                    case JTokenType.Object:
                        {
                            string cmd = token["cmd"]?.ToString();
                            HandlePacket(cmd, msg);
                            break;
                        }
                    default:
                        break;
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogError("Error receiving messages: " + e.Message);
        }
    }
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

    private void OnApplicationQuit()
    {
        if (socket != null)
            socket.Dispose();
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
