using Newtonsoft.Json.Linq;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.U2D.Animation;

public class SocketManager : MonoBehaviour, IUpdatable
{
    private ClientWebSocket socket;
    private Uri serverUri;
    private PacketSerializeManager packetSerializeManager;
    private CancellationTokenSource shutdownCts = new CancellationTokenSource();

    private readonly ConcurrentQueue<string> sendQueue = new ConcurrentQueue<string>();
    private readonly ConcurrentQueue<string> receiveQueue = new ConcurrentQueue<string>();

    private readonly ConcurrentQueue<string> syncOtherPlayersQueue = new ConcurrentQueue<string>();
    private readonly ConcurrentQueue<string> syncMobsQueue = new ConcurrentQueue<string>();

    private readonly ConcurrentQueue<string> logInQueue = new ConcurrentQueue<string>();
    private readonly ConcurrentQueue<string> logOutQueue = new ConcurrentQueue<string>();
    private readonly ConcurrentQueue<string> registerQueue = new ConcurrentQueue<string>();

    private readonly ConcurrentQueue<string> inventoryQueue = new ConcurrentQueue<string>();
    private readonly ConcurrentQueue<string> inventoryAttributesQueue = new ConcurrentQueue<string>();
    private readonly ConcurrentQueue<string> equipmentQueue = new ConcurrentQueue<string>();
    private readonly ConcurrentQueue<string> equipmentAttributesQueue = new ConcurrentQueue<string>();

    private readonly ConcurrentQueue<string> outfitSpritesQueue = new ConcurrentQueue<string>();

    GameObject player;

    private void Awake()
    {
#if UNITY_ANDROID
        serverUri = new Uri("ws://192.168.100.7:55556/"); //phải khai báo rõ IP LAN của Server cho thiết bị Android 
        packetSerializeManager = GameManager.Instance.GetComponent<PacketSerializeManager>();
#elif UNITY_EDITOR || UNITY_STANDALONE
        serverUri = new Uri($"ws://{IPV4ConfigurationManager.GetLocalIPv4()}:55556/"); // dùng IP LAN tự động khi chạy trên máy tính
        packetSerializeManager = GameManager.Instance.GetComponent<PacketSerializeManager>();
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

            _ = StartSyncPlayerLoop(shutdownCts.Token);
            _ = StartReceiveLoop(shutdownCts.Token);
        }
        catch (Exception e)
        {
            Debug.LogError("Socket: Kết nối Server thất bại! " + e.Message);
        }
    }

    public void OnUpdate() { }
    public void OnLateUpdate() { }
    public void OnFixedUpdate() { }

    public void RegisterDontDestroyOnLoad() { }

    private PlayerSyncDataRequestPacket GetSyncPlayerDataRequestPacket()
    {
        switch (LogInView.GetIDSchool())
        {
            case 1:
                player = GameObject.Find("ChienBinh(Clone)");
                break;
            case 2:
                player = GameObject.Find("SatThu(Clone)");
                break;
            case 3:
                player = GameObject.Find("PhapSu(Clone)");
                break;
            default:
                player = GameObject.Find("XaThu(Clone)");
                break;
        }

        if (player == null)
            return null;

        MovementController playerMovementController = player.GetComponent<MovementController>();
        SpriteController playerSpriteController = player.GetComponent<SpriteController>();

        PlayerSyncDataRequestPacket packet = new PlayerSyncDataRequestPacket
        {
            cmd = "syncPlayerData",
            playerSyncData = new PlayerSyncData
            {
                playerData = new PlayerData
                {
                    idAccount = LogInView.GetIDAccount() ?? 0,
                    idSchool = LogInView.GetIDSchool(),
                    hair = playerSpriteController.GetHairData(),
                    weapon = playerSpriteController.GetWeaponData(),
                    helmet = playerSpriteController.GetHelmetData(),
                    armor = playerSpriteController.GetArmorData(),
                    legArmor = playerSpriteController.GetLegArmorData(),
                },
                playerTransformData = new PlayerTransformData
                {
                    positionData = new PositionData
                    {
                        x = playerMovementController.transform.position.x,
                        y = playerMovementController.transform.position.y,
                        z = playerMovementController.transform.position.z
                    },
                    scaleData = new ScaleData
                    {
                        x = playerMovementController.transform.localScale.x,
                        y = playerMovementController.transform.localScale.y,
                        z = playerMovementController.transform.localScale.z
                    }
                },
                playerStateData = new PlayerStateData
                {
                    stateData = playerMovementController.GetCurrentState(),
                    directionData = playerSpriteController.GetCurrentDirection(),
                    partBodyTransforms = new List<PartBodyData>(),
                }
            }
        };

        foreach (var partBody in playerSpriteController.GetListSpriteLibrary())
        {
            PartBodyData partBodyData = new PartBodyData
            {
                positionData = new PositionData(),
                rotationData = new RotationData(),
                scaleData = new ScaleData(),
                colorData = new ColorData(),
            };

            partBodyData.category = partBody.GetComponent<SpriteResolver>().GetCategory();
            partBodyData.label = partBody.GetComponent<SpriteResolver>().GetLabel();

            partBodyData.positionData.x = partBody.transform.localPosition.x;
            partBodyData.positionData.y = partBody.transform.localPosition.y;
            partBodyData.positionData.z = partBody.transform.localPosition.z;

            partBodyData.rotationData.x = partBody.transform.localEulerAngles.x;
            partBodyData.rotationData.y = partBody.transform.localEulerAngles.y;
            partBodyData.rotationData.z = partBody.transform.localEulerAngles.z;

            partBodyData.scaleData.x = partBody.transform.localScale.x;
            partBodyData.scaleData.y = partBody.transform.localScale.y;
            partBodyData.scaleData.z = partBody.transform.localScale.z;

            partBodyData.colorData.r = partBody.GetComponent<Renderer>().material.color.r;
            partBodyData.colorData.g = partBody.GetComponent<Renderer>().material.color.g;
            partBodyData.colorData.b = partBody.GetComponent<Renderer>().material.color.b;
            partBodyData.colorData.a = partBody.GetComponent<Renderer>().material.color.a;

            packet.playerSyncData.playerStateData.partBodyTransforms.Add(partBodyData);
        }

        return packet;
    }
    private async Task StartSyncPlayerLoop(CancellationToken token)
    {
        const int targetTickRate = 70;
        const int tickMS = 1000 / targetTickRate;

        var stopwatch = new System.Diagnostics.Stopwatch();

        try
        {
            while (!token.IsCancellationRequested)
            {
                stopwatch.Restart();

                if (socket != null && socket.State == WebSocketState.Open)
                {
                    PlayerSyncDataRequestPacket packet = GetSyncPlayerDataRequestPacket();

                    if (packet != null)
                    {
                        packetSerializeManager.HandleSentPacket(packet);
                    }
                }

                stopwatch.Stop();

                int sleep = tickMS - (int)stopwatch.ElapsedMilliseconds;

                if (sleep > 0)
                    await Task.Delay(sleep, token);
                else
                    await Task.Yield();
            }
        }
        catch (TaskCanceledException)
        {
            Debug.Log("SyncPlayerLoop stopped.");
        }
        catch (Exception e)
        {
            Debug.LogError("StartSyncPlayerLoop error: " + e.Message);
        }
    }
    public async Task SendToServer(string message)
    {
        if (socket == null)
            return;

        if (socket.State != WebSocketState.Open)
            return;

        try
        {
            byte[] data = Encoding.UTF8.GetBytes(message);

            await socket.SendAsync(new ArraySegment<byte>(data), WebSocketMessageType.Text, true, CancellationToken.None);
        }
        catch (Exception e)
        {
            Debug.LogError("SendToServer error: " + e.Message);
        }
    }

    private async Task StartReceiveLoop(CancellationToken token)
    {
        var buffer = new byte[4096];
        var messageBuffer = new StringBuilder();

        try
        {
            while (!token.IsCancellationRequested && socket != null && socket.State == WebSocketState.Open)
            {
                WebSocketReceiveResult result;

                do
                {
                    result = await socket.ReceiveAsync(new ArraySegment<byte>(buffer), token);

                    if (result.MessageType == WebSocketMessageType.Close)
                        return;

                    messageBuffer.Append(Encoding.UTF8.GetString(buffer, 0, result.Count));

                } while (!result.EndOfMessage);

                string fullMessage = messageBuffer.ToString();
                messageBuffer.Clear();

                var tokenJson = JToken.Parse(fullMessage);

                switch (tokenJson.Type)
                {
                    case JTokenType.Object:
                        {
                            string cmd = tokenJson["cmd"]?.ToString();
                            HandlePacket(cmd, fullMessage);
                            break;
                        }
                    case JTokenType.Array:
                        {
                            if (!tokenJson.HasValues)
                                continue;

                            if (tokenJson[0].Type != JTokenType.Object)
                                continue;

                            string cmd = tokenJson[0]["cmd"]?.ToString();

                            if (!string.IsNullOrEmpty(cmd))
                                HandlePacket(cmd, fullMessage);
                            break;
                        }
                }
            }
        }
        catch (OperationCanceledException)
        {
            Debug.Log("ReceiveLoop stopped.");
        }
        catch (Exception ex)
        {
            Debug.Log($"Socket: Mất kết nối tới Server! {ex}");
        }
    }
    private void HandlePacket(string cmd, string json)
    {
        switch (cmd)
        {
            case "syncOtherPlayers":
                if (LogInView.GetIDAccount() != 0)
                    syncOtherPlayersQueue.Enqueue(json);
                break;

            case "syncMobs":
                syncMobsQueue.Enqueue(json);
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
    public string GetSyncOtherPlayersData()
    {
        if (syncOtherPlayersQueue.TryDequeue(out var data))
            return data;
        return null;
    }
    public string GetSyncMobsData()
    {
        if (syncMobsQueue.TryDequeue(out var data))
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

        ClearQueue(syncOtherPlayersQueue);
        ClearQueue(syncMobsQueue);

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
