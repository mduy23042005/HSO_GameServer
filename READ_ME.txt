--------------------------------------------------------Cách chạy được GameServer------------------------------------------------------

Bước 1: Cấu hình Port kết nối HSO_WebSocket
- Mở Project HSO_GameServer bằng Unity.
- Mở C# project -> Chỉ định đúng port kết nối tới WebSocket trong file SocketManager.cs cho 2 dòng:
	serverUri = new Uri($"ws://{IPV4ConfigurationManager.GetLocalIPv4()}:55556/"); cho PC clients
	serverUri = new Uri("ws://192.168.100.10:55556/"); cho Android clients
- Build client và thử nghiệm.
