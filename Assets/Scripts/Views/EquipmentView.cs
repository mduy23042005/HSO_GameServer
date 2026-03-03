using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EquipmentRequestPacket
{
    public string cmd;
    public int idAccount;
}
public class EquipmentData
{
    public int id;
    public int idItem0_1;
    public string nameItem0_1;
    public int category;
    public string slotName;
    public List<Item0_Attribute> item0_Attributes;
    public List<Attribute> nameAttributes;
}
public class EquipmentResultPacket
{
    public string cmd;
    public List<EquipmentData> equipmentData;
}

public class EquipmentView : MonoBehaviour, IUpdatable
{
    [Header("Danh sách các ô hành trang")]
    [SerializeField] private List<Image> equipmentSlots;

    private static List<Image> listImagesEquipment;
    private static List<EquipmentData> equipment = new List<EquipmentData>();

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
        string data = socketManager.GetEquipmentData();

        if (string.IsNullOrEmpty(data))
            return;

        EquipmentResultPacket equipmentResult = packetSerializeManager.HandleReceivedPacket<EquipmentResultPacket>(data);

        for (int i = 0; i < equipmentResult.equipmentData.Count; i++)
        {
            if (i >= equipment.Count)
            {
                equipment.Add(equipmentResult.equipmentData[i]);
            }
            else
            {
                equipment[i] = equipmentResult.equipmentData[i];
            }
        }

        // Update UI
        for (int i = 0; i < equipmentSlots.Count && i < equipment.Count; i++)
        {
            int itemId = equipment[i].idItem0_1;
            if (itemId == 0)
                equipmentSlots[i].sprite = ItemController.Instance.GetDefaultItemSprite(i);
            else
                equipmentSlots[i].sprite = ItemController.Instance.GetItem0(itemId).iconItem0;
        }
        listImagesEquipment = equipmentSlots;
    }
    public void OnLateUpdate() { }
    public void OnFixedUpdate() { }
    public void RegisterDontDestroyOnLoad()
    {
        DontDestroyOnLoad(this.gameObject);
    }

    // Đọc dữ liệu từ database và hiển thị vào Equipment Slots
    private async void ReadCache()
    {
        int idAccount = LogInView.GetIDAccount() ?? 0;
        EquipmentRequestPacket sendEquipmentRequestPacket = new EquipmentRequestPacket
        {
            cmd = "equipment",
            idAccount = idAccount,
        };

        packetSerializeManager.HandleSentPacket(sendEquipmentRequestPacket);
    }

    public static List<EquipmentData> GetListEquipmentSlots()
    {
        return equipment;
    }
    public static List<Image> GetListImagesEquipmentSlots()
    {
        return listImagesEquipment;
    }

    public static void ClearEquipmentData()
    {
        equipment.Clear();
    }
    public static void ClearListImagesEquipmentSlots()
    {
        listImagesEquipment.Clear();
    }
}
