using Newtonsoft.Json.Linq;
using System;
using System.Collections.Concurrent;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SocketManager : MonoBehaviour
{
    private ClientWebSocket socket;
    private Uri serverUri;
    private PacketSerializeManager packetSerializeManager;

    private SyncPlayerRequestPacket oldPacket;
    private PlayerState playerState;
    private Direction direction;
    private float stateStartTime;

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

            _ = StartSyncPlayerLoop();
            _ = StartReceiveLoop();
        }
        catch (Exception e)
        {
            Debug.LogError("Socket: Kết nối Server thất bại! " + e.Message);
        }
    }

    private SyncPlayerRequestPacket GetSyncDataPacket()
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

        if (playerState != playerMovementController.GetCurrentState() || direction != playerSpriteController.GetCurrentDirection())
        {
            playerState = playerMovementController.GetCurrentState();
            direction = playerSpriteController.GetCurrentDirection();
            stateStartTime = Time.time;
        }

        SyncPlayerRequestPacket packet = new SyncPlayerRequestPacket
        {
            cmd = "syncPlayerData",
            playerData = new PlayerData
            {
                idAccount = LogInView.GetIDAccount() ?? 0,
                idSchool = LogInView.GetIDSchool(),
                posX = player.transform.position.x,
                posY = player.transform.position.y,
                lastPosX = playerMovementController.GetLastMovement().x,
                lastPosY = playerMovementController.GetLastMovement().y,
                state = playerState,
                direction = direction,
                stateStartTime = stateStartTime,

                hair = playerSpriteController.GetHairData(),
                weapon = playerSpriteController.GetWeaponData(),
                helmet = playerSpriteController.GetHelmetData(),
                armor = playerSpriteController.GetArmorData(),
                legArmor = playerSpriteController.GetLegArmorData(),
            }
        };

        if (oldPacket == packet)
        {
            return null;
        }
        oldPacket = packet;

        return packet;
    }
    private async Task StartSyncPlayerLoop()
    {
        const int targetTickRate = 20;
        const int tickMS = 1000 / targetTickRate;

        var stopwatch = new System.Diagnostics.Stopwatch();

        while (true)
        {
            stopwatch.Restart();

            try
            {
                if (socket != null && socket.State == WebSocketState.Open)
                {
                    SyncPlayerRequestPacket packet = GetSyncDataPacket();

                    if (packet != null)
                    {
                        packetSerializeManager.HandleSentPacket(packet);
                    }
                }
            }
            catch (Exception e)
            {
                Debug.LogError("StartSyncPlayerLoop error: " + e.Message);
            }

            stopwatch.Stop();

            int sleep = tickMS - (int)stopwatch.ElapsedMilliseconds;

            if (sleep > 0)
                await Task.Delay(sleep);
            else
                await Task.Yield();
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

    private async Task StartReceiveLoop()
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
        catch (Exception ex)
        {
            GameObject.Find("SyncManager").gameObject.GetComponent<SyncOtherPlayersManager>().PrepareForLogOut();

            //Dọn sạch danh sách quản lý Queue nhận dữ liệu từ Server
            ClearAllQueues();

            if (EquipmentView.GetListEquipmentSlots() != null)
            {
                EquipmentView.ClearEquipmentData();
            }
            if (EquipmentView.GetListImagesEquipmentSlots() != null)
            {
                EquipmentView.ClearListImagesEquipmentSlots();
            }

            if (InventoryView.GetListInventoryItem0Slots() != null)
            {
                InventoryView.ClearInventoryData();
            }

            GameManager.Instance.GetComponent<PlayerManager>().DestroyPlayer();

            Debug.Log($"Socket: Mất kết nối tới Server! {ex}");

            SceneManager.LoadScene("Main");
        }
    }
    private void HandlePacket(string cmd, string json)
    {
        switch (cmd)
        {
            case "syncOtherPlayers":
                if (LogInView.GetIDAccount() != 0)
                {
                    syncOtherPlayersQueue.Enqueue(json);
                }
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
