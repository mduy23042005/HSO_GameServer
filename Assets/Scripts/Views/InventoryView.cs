using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class InventoryRequestPacket
{
    public string cmd;
    public int idAccount;
}
[Serializable]
public class InventoryResultPacket
{
    public string cmd;
    public int id;
    public int idItem0;
    public int category;
    public string typeItem0;
    public int idSchool;
    public List<Item0_Attribute> item0_Attributes;
    public List<Attribute> nameAttributes;
}

public class InventoryView : MonoBehaviour, IUpdatable
{
    [Header("Các ô hành trang")]
    [SerializeField] private List<Image> inventorySlots;

    private static List<InventoryResultPacket> inventoryItem0;
    private SocketManager socketManager;
    private PacketSerializeManager packetSerializeManager;

    private void Awake()
    {
        socketManager = GameManager.Instance.GetComponent<SocketManager>();
        packetSerializeManager = GameManager.Instance.GetComponent<PacketSerializeManager>();

        int idAccount = LogInView.GetIDAccount() ?? 0;
        if (idAccount != 0)
        {
            ReadCache();
        }
    }

    private void OnEnable()
    {
        GameManager.Instance.Register(this);
    }
    private void OnDisable()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.Unregister(this);
        }
    }
    public void OnUpdate()
    {
        string data = socketManager.GetInventoryData();

        if (string.IsNullOrEmpty(data))
        {
            return;
        }

        Debug.Log("Received inventory data successfully!");

        List<InventoryResultPacket> inventoryResult = packetSerializeManager.HandleReceivedPacket<List<InventoryResultPacket>>(data);

        if (inventoryItem0 == null)
            inventoryItem0 = new List<InventoryResultPacket>();

        for (int i = 0; i < inventoryResult.Count; i++)
        {
            if (i >= inventoryItem0.Count)
            {
                if (inventoryResult[i].cmd != "inventory_result")
                    continue;

                inventoryItem0.Add(inventoryResult[i]);
            }
            else
            {
                inventoryItem0[i] = inventoryResult[i];
            }
        }

        // Update UI
        for (int i = 0; i < inventoryItem0.Count; i++)
        {
            int itemId = inventoryItem0[i].idItem0;
            if (itemId == 0)
            {
                inventorySlots[i].sprite = null;
                inventorySlots[i].color = new Color(0f, 0f, 0f, 0f);
            }
            else
            {
                inventorySlots[i].sprite = ItemController.Instance.GetItem0(itemId).iconItem0;
            }
        }
        for (int i = inventoryResult.Count; i < inventorySlots.Count; i++)
        {
            inventorySlots[i].sprite = null;
            inventorySlots[i].color = new Color(0f, 0f, 0f, 0f);
        }
    }

    public void OnLateUpdate() { }
    public void OnFixedUpdate() { }
    public void RegisterDontDestroyOnLoad()
    {
        DontDestroyOnLoad(this.gameObject);
    }

    // Đọc dữ liệu từ database và hiển thị vào Inventory Slots
    private async void ReadCache()
    {
        int idAccount = LogInView.GetIDAccount() ?? 0;
        InventoryRequestPacket sendInventoryRequestPacket = new InventoryRequestPacket
        {
            cmd = "inventory",
            idAccount = idAccount,
        };

        packetSerializeManager.HandleSentPacket(sendInventoryRequestPacket);
    }
    public static List<InventoryResultPacket> GetListInventorySlots()
    {
        return inventoryItem0;
    }
    public static void ClearInventoryData()
    {
        inventoryItem0.Clear();
    }
}
