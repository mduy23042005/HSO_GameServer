using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SocketManager : MonoBehaviour, IUpdatable
{
    private ClientWebSocket socket;
    private Uri serverUri;
    private CancellationTokenSource shutdownCts = new CancellationTokenSource();

    private readonly ConcurrentQueue<byte[]> sendQueue = new ConcurrentQueue<byte[]>();
    private readonly ConcurrentQueue<byte[]> receiveQueue = new ConcurrentQueue<byte[]>();

    private readonly ConcurrentQueue<byte[]> updateHPQueue = new ConcurrentQueue<byte[]>();
    private readonly ConcurrentQueue<byte[]> updateMPQueue = new ConcurrentQueue<byte[]>();

    private readonly ConcurrentQueue<byte[]> mobsAttackPlayerQueue = new ConcurrentQueue<byte[]>();
    private readonly ConcurrentQueue<byte[]> mobsHealQueue = new ConcurrentQueue<byte[]>();
    private readonly ConcurrentQueue<byte[]> mobsInjuredQueue = new ConcurrentQueue<byte[]>();
    private readonly ConcurrentQueue<byte[]> mobsAttackOtherPlayerQueue = new ConcurrentQueue<byte[]>();

    private readonly ConcurrentQueue<byte[]> syncCallBackQueue = new ConcurrentQueue<byte[]>();
    private readonly ConcurrentQueue<byte[]> syncOtherPlayersQueue = new ConcurrentQueue<byte[]>();
    private readonly ConcurrentQueue<byte[]> syncMobsQueue = new ConcurrentQueue<byte[]>();
    private readonly ConcurrentQueue<byte[]> syncMobsDeadQueue = new ConcurrentQueue<byte[]>();

    private readonly ConcurrentQueue<byte[]> logInQueue = new ConcurrentQueue<byte[]>();
    private readonly ConcurrentQueue<byte[]> logOutQueue = new ConcurrentQueue<byte[]>();
    private readonly ConcurrentQueue<byte[]> registerQueue = new ConcurrentQueue<byte[]>();

    private readonly ConcurrentQueue<byte[]> inventoryQueue = new ConcurrentQueue<byte[]>();
    private readonly ConcurrentQueue<byte[]> inventoryAttributesQueue = new ConcurrentQueue<byte[]>();
    private readonly ConcurrentQueue<byte[]> equipmentQueue = new ConcurrentQueue<byte[]>();
    private readonly ConcurrentQueue<byte[]> equipmentAttributesQueue = new ConcurrentQueue<byte[]>();

    private readonly ConcurrentQueue<byte[]> outfitSpritesQueue = new ConcurrentQueue<byte[]>();

    private readonly ConcurrentQueue<byte[]> playerAttackMobQueues = new ConcurrentQueue<byte[]>();
    private readonly ConcurrentQueue<byte[]> otherPlayerAttackMobQueues = new ConcurrentQueue<byte[]>();

    private MovementPlayerController playerMovementController;
    private SpritePlayerController playerSpriteController;
    private Dictionary<int, (Category, Label)> spriteResolversInfos = new Dictionary<int, (Category, Label)>();

    private void Awake()
    {
#if UNITY_ANDROID
        serverUri = new Uri("ws://172.16.55.110:55556/"); //phải khai báo rõ IP LAN của Server cho thiết bị Android 
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

            _ = StartSyncPlayerLoop(shutdownCts.Token);
            _ = Task.Run(() => StartReceiveLoop(shutdownCts.Token));
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

    private byte[] GetSyncPlayerDataRequestByteArray()
    {
        if (PlayerManager.player == null)
            return null;

        playerMovementController = PlayerManager.player.GetComponent<MovementPlayerController>();
        playerSpriteController = PlayerManager.player.GetComponent<SpritePlayerController>();

        PacketWriterManager writer = new PacketWriterManager();
        writer.WriteInt((int)EnumCmdCode.syncPlayerData);
        writer.WriteString(SceneManager.GetActiveScene().name);
        writer.WriteInt(LogInView.GetIDAccount() ?? 0);
        writer.WriteInt(LogInView.GetLevel());
        writer.WriteInt(LogInView.GetIDSchool());
        writer.WriteInt(playerSpriteController.GetHairData());
        writer.WriteInt(playerSpriteController.GetWeaponData());
        writer.WriteInt(playerSpriteController.GetHelmetData());
        writer.WriteInt(playerSpriteController.GetArmorData());
        writer.WriteInt(playerSpriteController.GetLegArmorData());
        writer.WriteInt((int)playerMovementController.GetCurrentTileType());

        writer.WriteFloat(playerMovementController.transform.position.x);
        writer.WriteFloat(playerMovementController.transform.position.y);

        writer.WriteFloat(playerMovementController.transform.localScale.x);

        writer.WriteInt((int)playerMovementController.GetCurrentState());
        writer.WriteInt((int)playerSpriteController.GetCurrentDirection());
        writer.WriteListCount(playerSpriteController.GetListSpriteLibrary().Count);

        spriteResolversInfos = playerSpriteController.GetSpriteResolversInfos();

        for (int i = 0; i < playerSpriteController.GetListSpriteLibrary().Count; i++)
        {
            writer.WriteInt((int)spriteResolversInfos[i].Item1); //category sprite resolver
            writer.WriteInt((int)spriteResolversInfos[i].Item2); //label sprite resolver
        }

        return writer.ToArray();
    }
    private async Task StartSyncPlayerLoop(CancellationToken token)
    {
        const int targetTickRate = 20;
        const int tickMS = 1000 / targetTickRate;

        var stopwatch = new System.Diagnostics.Stopwatch();

        try
        {
            while (!token.IsCancellationRequested)
            {
                stopwatch.Restart();

                if (socket != null && socket.State == WebSocketState.Open)
                {
                    byte[] byteData = GetSyncPlayerDataRequestByteArray();

                    if (byteData != null && byteData.Length > 0)
                    {
                        await SendToServer(byteData);
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
    public async Task SendToServer(byte[] data)
    {
        if (socket == null)
            return;

        if (socket.State != WebSocketState.Open)
            return;

        try
        {
            await socket.SendAsync(new ArraySegment<byte>(data), WebSocketMessageType.Binary, true, CancellationToken.None);
        }
        catch (Exception e)
        {
            Debug.LogError("SendToServer error: " + e.Message);
        }
    }

    private async Task StartReceiveLoop(CancellationToken token)
    {
        var buffer = new byte[4096];
        var messageBuffer = new List<byte>();

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

                    messageBuffer.AddRange(new ArraySegment<byte>(buffer, 0, result.Count));

                } while (!result.EndOfMessage);

                byte[] fullMessage = messageBuffer.ToArray();
                messageBuffer.Clear();

                HandlePacket(fullMessage);
            }
        }
        catch (OperationCanceledException)
        {
            Debug.Log("ReceiveLoop stopped.");
        }
        catch (Exception ex)
        {

        }
    }
    private void HandlePacket(byte[] data)
    {
        PacketReaderManager reader = new PacketReaderManager(data);
        EnumCmdCode cmd = (EnumCmdCode)reader.ReadInt();

        switch (cmd)
        {
            case EnumCmdCode.updateHP:
                updateHPQueue.Enqueue(data);
                break;
            case EnumCmdCode.updateMP:
                updateMPQueue.Enqueue(data);
                break;

            case EnumCmdCode.mobsAttackPlayer:
                mobsAttackPlayerQueue.Enqueue(data);
                break;
            case EnumCmdCode.mobsHeal:
                mobsHealQueue.Enqueue(data);
                break;
            case EnumCmdCode.mobsInjured:
                mobsInjuredQueue.Enqueue(data);
                break;
            case EnumCmdCode.mobsAttackOtherPlayer:
                mobsAttackOtherPlayerQueue.Enqueue(data);
                break;

            case EnumCmdCode.syncCallBack:
                syncCallBackQueue.Enqueue(data);
                break;

            case EnumCmdCode.syncPlayerData:
                if (LogInView.GetIDAccount() != 0)
                    syncOtherPlayersQueue.Enqueue(data);
                break;

            case EnumCmdCode.syncMobsData:
                if (LogInView.GetIDAccount() != 0)
                    syncMobsQueue.Enqueue(data);
                break;
            case EnumCmdCode.syncMobsDeadData:
                if (LogInView.GetIDAccount() != 0)
                    syncMobsDeadQueue.Enqueue(data);
                break;

            case EnumCmdCode.login:
                logInQueue.Enqueue(data);
                break;

            case EnumCmdCode.logout:
                logOutQueue.Enqueue(data);
                break;

            case EnumCmdCode.register:
                registerQueue.Enqueue(data);
                break;

            case EnumCmdCode.equipment:
                equipmentQueue.Enqueue(data);
                break;

            case EnumCmdCode.equipmentAttributes:
                equipmentAttributesQueue.Enqueue(data);
                break;

            case EnumCmdCode.inventory:
                inventoryQueue.Enqueue(data);
                break;

            case EnumCmdCode.inventoryAttributes:
                inventoryAttributesQueue.Enqueue(data);
                break;

            case EnumCmdCode.outfitSprites:
                outfitSpritesQueue.Enqueue(data);
                break;

            case EnumCmdCode.playerAttackMob:
                playerAttackMobQueues.Enqueue(data);
                break;
            case EnumCmdCode.otherPlayerAttackMob:
                otherPlayerAttackMobQueues.Enqueue(data);
                break;

            default:
                receiveQueue.Enqueue(data);
                break;
        }
    }

    public byte[] GetReceiveData()
    {
        if (receiveQueue.TryDequeue(out var data))
            return data;

        return null;
    }
    public byte[] GetUpdateHPData()
    {
        if (updateHPQueue.TryDequeue(out var data))
            return data;

        return null;
    }
    public byte[] GetUpdateMPData()
    {
        if (updateMPQueue.TryDequeue(out var data))
            return data;

        return null;
    }

    public byte[] GetMobsAttackPlayerData()
    {
        if (mobsAttackPlayerQueue.TryDequeue(out var data))
            return data;

        return null;
    }
    public byte[] GetMobsAttackOtherPlayerData()
    {
        if (mobsAttackOtherPlayerQueue.TryDequeue(out var data))
            return data;

        return null;
    }
    public byte[] GetMobsHealData()
    {
        if (mobsHealQueue.TryDequeue(out var data))
            return data;

        return null;
    }
    public byte[] GetMobsInjuredData()
    {
        if (mobsInjuredQueue.TryDequeue(out var data))
            return data;

        return null;
    }

    public byte[] GetSyncCallBackData()
    {
        if (syncCallBackQueue.TryDequeue(out var data))
            return data;
        return null;
    }
    public byte[] GetSyncOtherPlayersData()
    {
        if (syncOtherPlayersQueue.TryDequeue(out var data))
            return data;
        return null;
    }
    public byte[] GetSyncMobsData()
    {
        if (syncMobsQueue.TryDequeue(out var data))
            return data;
        return null;
    }
    public byte[] GetSyncMobsDeadData()
    {
        if (syncMobsDeadQueue.TryDequeue(out var data))
            return data;
        return null;
    }
    public byte[] GetLogInData()
    {
        if (logInQueue.TryDequeue(out var data))
            return data;
        return null;
    }
    public byte[] GetLogOutData()
    {
        if (logOutQueue.TryDequeue(out var data))
            return data;
        return null;
    }
    public byte[] GetRegisterData()
    {
        if (registerQueue.TryDequeue(out var data))
            return data;
        return null;
    }
    public byte[] GetInventoryData()
    {
        if (inventoryQueue.TryDequeue(out var data))
            return data;
        return null;
    }
    public byte[] GetEquipmentData()
    {
        if (equipmentQueue.TryDequeue(out var data))
            return data;
        return null;
    }
    public byte[] GetInventoryAttributesData()
    {
        if (inventoryAttributesQueue.TryDequeue(out var data))
            return data;
        return null;
    }
    public byte[] GetEquipmentAttributesData()
    {
        if (equipmentAttributesQueue.TryDequeue(out var data))
        {
            return data;
        }
        return null;
    }
    public byte[] GetOutfitSpritesData()
    {
        if (outfitSpritesQueue.TryDequeue(out var data))
        {
            return data;
        }

        return null;
    }
    public byte[] GetPlayerAttackMobData()
    {
        if (playerAttackMobQueues.TryDequeue(out var data))
            return data;
        return null;
    }
    public byte[] GetOtherPlayerAttackMobData()
    {
        if (otherPlayerAttackMobQueues.TryDequeue(out var data))
            return data;
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

        ClearQueue(syncCallBackQueue);
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

        ClearQueue(playerAttackMobQueues);
        ClearQueue(otherPlayerAttackMobQueues);
    }
    private void ClearQueue(ConcurrentQueue<byte[]> queue)
    {
        while (queue.TryDequeue(out _)) { }
    }
}