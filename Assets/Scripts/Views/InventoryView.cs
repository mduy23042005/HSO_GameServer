using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryRequestPacket
{
    public string cmd;
    public int idAccount;
}

public class InventoryItem0Data
{
    public int id;
    public int idItem0;
    public string nameItem0;
    public string typeItem0;
    public int category;
    public int idSchool;
    public int level;
    public List<Item0_Attribute> item0_Attributes;
    public List<Attribute> nameAttributes;
}
public class InventoryItem1Data
{
    public int id;
    public int idItem1;
    public string nameItem1;
    public string typeItem1;
    public int level;
    public List<Item1_Attribute> item1_Attributes;
    public List<Attribute> nameAttributes;
}
public class InventoryItem2Data
{
    public int id;
    public int idItem2;
    public string nameItem2;
    public int level;
    public int quality;
}
public class InventoryItem3Data
{
    public int id;
    public int idItem3;
    public string nameItem3;
    public int level;
    public string details;
    public int quality;
}
public class InventoryItem4Data
{
    public int id;
    public int idItem4;
    public string nameItem4;
    public int level;
    public string details;
    public int quality;
}
public class InventoryResultPacket
{
    public string cmd;
    public List<InventoryItem0Data> inventoryItem0Data;
    public List<InventoryItem1Data> inventoryItem1Data;
    public List<InventoryItem2Data> inventoryItem2Data;
    public List<InventoryItem3Data> inventoryItem3Data;
    public List<InventoryItem4Data> inventoryItem4Data;
}

public class InventoryView : MonoBehaviour, IUpdatable
{
    [Header("Các ô hành trang")]
    [SerializeField] private List<Image> inventorySlots;

    private static List<InventoryItem0Data> inventoryItem0;

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

    public void OnUpdate() // tạm thời chỉ có item0
    {
        string data = socketManager.GetInventoryData();

        if (string.IsNullOrEmpty(data))
        {
            return;
        }

        InventoryResultPacket inventoryResult = packetSerializeManager.HandleReceivedPacket<InventoryResultPacket>(data);

        if (inventoryItem0 == null)
            inventoryItem0 = new List<InventoryItem0Data>();

        for (int i = 0; i < inventoryResult.inventoryItem0Data.Count; i++)
        {
            if (i >= inventoryItem0.Count)
            {
                inventoryItem0.Add(inventoryResult.inventoryItem0Data[i]);
            }
            else
            {
                inventoryItem0[i] = inventoryResult.inventoryItem0Data[i];
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
        for (int i = inventoryResult.inventoryItem0Data.Count; i < inventorySlots.Count; i++)
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

    public static List<InventoryItem0Data> GetListInventoryItem0Slots()
    {
        return inventoryItem0;
    }
    public static void ClearInventoryData()
    {
        inventoryItem0.Clear();
    }
}
