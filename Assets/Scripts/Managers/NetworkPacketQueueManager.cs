using System;
using System.Collections.Concurrent;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public class NetworkPacketQueueManager : MonoBehaviour
{
    private readonly ConcurrentQueue<string> packetQueue = new();
    private CancellationTokenSource cts;

    private WebSocket socket;

    // Gọi 1 lần sau khi socket đã CONNECTED
    public void StartReceive(WebSocket webSocket)
    {
        socket = webSocket;

        cts = new CancellationTokenSource();

        // chạy background receive loop
        Task.Run(ReceiveLoop, cts.Token);
    }

    //Background thread
    private async Task ReceiveLoop()
    {
        var buffer = new byte[8192];

        try
        {
            while (socket.State == WebSocketState.Open && !cts.IsCancellationRequested)
            {
                var result = await socket.ReceiveAsync(
                    new ArraySegment<byte>(buffer),
                    CancellationToken.None
                );

                if (result.MessageType == WebSocketMessageType.Close)
                    break;

                string msg = Encoding.UTF8.GetString(buffer, 0, result.Count);

                packetQueue.Enqueue(msg);
            }
        }
        catch (Exception e)
        {
            Debug.LogError("ReceiveLoop Error: " + e.Message);
        }
    }

    /// Unity main thread gọi trong Update
    public bool TryDequeue(out string packet)
    {
        return packetQueue.TryDequeue(out packet);
    }

    private void OnDestroy()
    {
        cts?.Cancel();
        cts?.Dispose();
    }
}
