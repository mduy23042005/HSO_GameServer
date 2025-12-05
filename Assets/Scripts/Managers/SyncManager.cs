using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public class SyncManager : MonoBehaviour
{
    [SerializeField] private GameObject playerPrefab;

    public static SyncManager Instance;
    private Dictionary<int, SyncController> otherPlayers = new Dictionary<int, SyncController>();

    private SyncModels player;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }

        _ = ReceiveMessages();
    }

    public void OnDataFromServer(SyncModels data)
    {
        // Bỏ qua data của chính mình
        if (data.idAccount == LogInController.GetIDAccount())
            return;

        // Nếu chưa có -> spawn OtherPlayer mới
        if (!otherPlayers.ContainsKey(data.idAccount))
        {
            GameObject obj = Instantiate(playerPrefab, new Vector2(data.posX, data.posY), Quaternion.identity);
            SyncController ctrl = obj.GetComponentInChildren<SyncController>();
            otherPlayers.Add(data.idAccount, ctrl);
        }
        else // Nếu đã tồn tại -> chỉ cập nhật vị trí + trang bị
        {
            SyncController ctrl = otherPlayers[data.idAccount];
            ctrl.transform.position = new Vector2(data.posX, data.posY);
        }
    }
    public async Task ReceiveMessages()
    {
        var buffer = new byte[1024];
        try
        {
            while (SocketManager.Instance.GetSocket().State == WebSocketState.Open)
            {
                var result = await SocketManager.Instance.GetSocket().ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                string syncData = Encoding.UTF8.GetString(buffer, 0, result.Count);

                player = JsonConvert.DeserializeObject<SyncModels>(syncData);
                if (player.idAccount != LogInController.GetIDAccount())
                {
                    OnDataFromServer(player);
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogError("Error receiving messages: " + e.Message);
        }
    }

    public SyncModels GetPlayerData()
    {
        return player;
    }
}
