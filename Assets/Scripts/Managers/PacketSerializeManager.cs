using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using UnityEngine;

public class JsonArrayWrapper<T>
{
    public List<T> items;
}

public class PacketSerializeManager : MonoBehaviour
{
    private SocketManager socketManager;
    private string packet;

    private void Awake()
    {
        socketManager = GameManager.Instance.GetComponent<SocketManager>();
    }

    public void HandleSentPacket<T>(T data)
    {
#if UNITY_ANDROID
        packet = JsonUtility.ToJson(data);
#else
        packet = JsonConvert.SerializeObject(data);
#endif
        socketManager.SendToServer(packet);
    }

    public T HandleReceivedPacket<T>(string json)
    {
        if (string.IsNullOrEmpty(json))
            return default;

#if UNITY_ANDROID
        // Kiểm tra xem json có phải array hay không
        if (typeof(T).IsGenericType && typeof(T).GetGenericTypeDefinition() == typeof(List<>))
        {
            var elementType = typeof(T).GetGenericArguments()[0];

            // Nếu json là array, wrap
            if (json.StartsWith("["))
            {
                string wrappedJson = "{ \"items\": " + json + " }";

                // Tạo kiểu JsonArrayWrapper<U>
                var wrapperType = typeof(JsonArrayWrapper<>).MakeGenericType(elementType);

                // Parse vào wrapper
                var wrapper = JsonUtility.FromJson(wrappedJson, wrapperType);

                // Lấy field "items" từ wrapper
                var itemsField = wrapperType.GetField("items");
                var list = itemsField.GetValue(wrapper);

                return (T)list; // trả về List<U>
            }
            else
            {
                // Nếu không phải array thì parse bình thường
                return JsonUtility.FromJson<T>(json);
            }
        }
        else
        {
            // Nếu là object bình thường { ... } thì parse trực tiếp
            return JsonUtility.FromJson<T>(json);
        }
#else
        return JsonConvert.DeserializeObject<T>(json);
#endif
    }
    public string GetPacket()
    {
        return packet;
    }
}

