using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.U2D.Animation;

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
    private readonly ConcurrentQueue<byte[]> mobsDieQueue = new ConcurrentQueue<byte[]>();
    private readonly ConcurrentQueue<byte[]> mobsAttackOtherPlayerQueue = new ConcurrentQueue<byte[]>();

    private readonly ConcurrentQueue<byte[]> syncCallBackQueue = new ConcurrentQueue<byte[]>();
    private readonly ConcurrentQueue<byte[]> syncOtherPlayersQueue = new ConcurrentQueue<byte[]>();
    private readonly ConcurrentQueue<byte[]> syncMobsQueue = new ConcurrentQueue<byte[]>();

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

    private GameObject player;

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

    private byte[] GetSyncPlayerDataRequestByteArray()
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

        MovementPlayerController playerMovementController = player.GetComponent<MovementPlayerController>();
        SpriteController playerSpriteController = player.GetComponent<SpriteController>();

        PlayerSyncDataRequestPacket packet = new PlayerSyncDataRequestPacket
        {
            cmd = EnumCmdCode.syncPlayerData,
            playerSyncData = new PlayerSyncData
            {
                playerData = new PlayerData
                {
                    idAccount = LogInView.GetIDAccount() ?? 0,
                    nameChar = LogInView.GetNameChar(),
                    level = LogInView.GetLevel(),
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

            partBodyData.colorData.r = partBody.GetComponent<SpriteRenderer>().color.r;
            partBodyData.colorData.g = partBody.GetComponent<SpriteRenderer>().color.g;
            partBodyData.colorData.b = partBody.GetComponent<SpriteRenderer>().color.b;
            partBodyData.colorData.a = partBody.GetComponent<SpriteRenderer>().color.a;

            packet.playerSyncData.playerStateData.partBodyTransforms.Add(partBodyData);
        }

        PacketWriterManager writer = new PacketWriterManager();

        writer.WriteInt((int)packet.cmd);
        writer.WriteString(SceneManager.GetActiveScene().name);
        writer.WriteInt(packet.playerSyncData.playerData.idAccount);
        writer.WriteString(packet.playerSyncData.playerData.nameChar);
        writer.WriteInt(packet.playerSyncData.playerData.level);
        writer.WriteInt(packet.playerSyncData.playerData.idSchool);
        writer.WriteInt(packet.playerSyncData.playerData.hair);
        writer.WriteInt(packet.playerSyncData.playerData.weapon);
        writer.WriteInt(packet.playerSyncData.playerData.helmet);
        writer.WriteInt(packet.playerSyncData.playerData.armor);
        writer.WriteInt(packet.playerSyncData.playerData.legArmor);
        writer.WriteInt(packet.playerSyncData.playerData.gloves);
        writer.WriteInt(packet.playerSyncData.playerData.shoes);
        writer.WriteInt(packet.playerSyncData.playerData.ring1);
        writer.WriteInt(packet.playerSyncData.playerData.ring2);
        writer.WriteInt(packet.playerSyncData.playerData.necklace);
        writer.WriteInt(packet.playerSyncData.playerData.medal);
        writer.WriteInt(packet.playerSyncData.playerData.cloak);
        writer.WriteInt(packet.playerSyncData.playerData.wing);
        writer.WriteInt(packet.playerSyncData.playerData.skinWing);
        writer.WriteInt(packet.playerSyncData.playerData.mounts);
        writer.WriteInt(packet.playerSyncData.playerData.pet);
        writer.WriteInt(packet.playerSyncData.playerData.skin);

        writer.WriteFloat(packet.playerSyncData.playerTransformData.positionData.x);
        writer.WriteFloat(packet.playerSyncData.playerTransformData.positionData.y);
        writer.WriteFloat(packet.playerSyncData.playerTransformData.positionData.z);
        writer.WriteFloat(packet.playerSyncData.playerTransformData.scaleData.x);
        writer.WriteFloat(packet.playerSyncData.playerTransformData.scaleData.y);
        writer.WriteFloat(packet.playerSyncData.playerTransformData.scaleData.z);

        writer.WriteInt((int)packet.playerSyncData.playerStateData.stateData);
        writer.WriteInt((int)packet.playerSyncData.playerStateData.directionData);
        writer.WriteListCount(packet.playerSyncData.playerStateData.partBodyTransforms.Count);
        foreach (var partBodyData in packet.playerSyncData.playerStateData.partBodyTransforms)
        {
            writer.WriteString(partBodyData.category);
            writer.WriteString(partBodyData.label);
            writer.WriteFloat(partBodyData.positionData.x);
            writer.WriteFloat(partBodyData.positionData.y);
            writer.WriteFloat(partBodyData.positionData.z);
            writer.WriteFloat(partBodyData.rotationData.x);
            writer.WriteFloat(partBodyData.rotationData.y);
            writer.WriteFloat(partBodyData.rotationData.z);
            writer.WriteFloat(partBodyData.scaleData.x);
            writer.WriteFloat(partBodyData.scaleData.y);
            writer.WriteFloat(partBodyData.scaleData.z);
            writer.WriteFloat(partBodyData.colorData.r);
            writer.WriteFloat(partBodyData.colorData.g);
            writer.WriteFloat(partBodyData.colorData.b);
            writer.WriteFloat(partBodyData.colorData.a);
        }

        return writer.ToArray();
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
            Debug.Log($"Socket: Mất kết nối tới Server! {ex}");
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
            case EnumCmdCode.mobsDie:
                mobsDieQueue.Enqueue(data);
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

            case EnumCmdCode.syncMobData:
                if (LogInView.GetIDAccount() != 0)
                    syncMobsQueue.Enqueue(data);
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
                Debug.Log("Received otherPlayerAttackMob packet");
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
    public byte[] GetMobsDieData()
    {
        if (mobsDieQueue.TryDequeue(out var data))
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
