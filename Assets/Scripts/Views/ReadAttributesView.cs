using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

//Packet đọc attribute trong equipment
public class ReadAttributesEquipmentRequestPacket
{
    public EnumCmdCode cmd;
    public int idAccount;
    public int id;
    public int idItem0_1;
}
public class ReadAttributesEquipmentResultPacket
{
    public EnumCmdCode cmd;
    public EquipmentData attributesData;
}

//Packet đọc attribute trong inventory
public class ReadAttributesInventoryRequestPacket
{
    public EnumCmdCode cmd;
    public int idAccount;
    public int id;
    public int idItem0;
}
public class ReadAttributesInventoryResultPacket
{
    public EnumCmdCode cmd;
    public InventoryItem0Data attributesItem0Data;
    public InventoryItem1Data attributesItem1Data;
    public InventoryItem2Data attributesItem2Data;
    public InventoryItem3Data attributesItem3Data;
    public InventoryItem4Data attributesItem4Data;
}

//Packet trang bị item0 từ inventory vào equipment
public class EquipItem0RequestPacket
{
    public EnumCmdCode cmd;
    public int idAccount;
    public int id;
    public int idItem0;
    public string slotName;
}
//Sử dụng 2 packet EquipmentResultPacket và InventoryResultPacket để cập nhật lại UI sau khi trang bị

public class ReadAttributesView : MonoBehaviour, IUpdatable
{
    [SerializeField] private GameObject itemInfo;
    [SerializeField] private GameObject nameItem;
    [SerializeField] private GameObject ring1Ring2Menu;
    [SerializeField] private List<Image> ring1Ring2Choice;

    private TMP_Text itemInfoText;
    private TMP_Text nameItemText;
    private string ringSlot;
    private int indexSlot;

    private SocketManager socketManager;
    private string cmdReadAttributes;

    private void Awake()
    {
        socketManager = GameManager.Instance.GetComponent<SocketManager>();

        itemInfoText = itemInfo.GetComponent<TMP_Text>();
        nameItemText = nameItem.GetComponent<TMP_Text>();
    }

    private void Start()
    {
        itemInfo.SetActive(false);
        ring1Ring2Menu.SetActive(false);
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
        PacketReaderManager reader;
        byte[] data;

        switch (cmdReadAttributes)
        { 
            case "equipmentAttributes":
                data = socketManager.GetEquipmentAttributesData();

                if (data == null || data.Length == 0)
                    return;

                reader = new PacketReaderManager(data);

                ReadAttributesEquipmentResultPacket equipmentAttributesResult = new ReadAttributesEquipmentResultPacket();
                equipmentAttributesResult.cmd = (EnumCmdCode)reader.ReadInt();
                equipmentAttributesResult.attributesData = new EquipmentData();
                equipmentAttributesResult.attributesData.id = reader.ReadInt();
                equipmentAttributesResult.attributesData.idItem0_1 = reader.ReadInt();
                equipmentAttributesResult.attributesData.nameItem0_1 = reader.ReadString();
                equipmentAttributesResult.attributesData.category = reader.ReadInt();
                equipmentAttributesResult.attributesData.slotName = reader.ReadString();
                equipmentAttributesResult.attributesData.item0_Attributes = new List<Item0_Attribute>();
                equipmentAttributesResult.attributesData.nameAttributes = new List<Attribute>();

                int countEquipmentAttribute = reader.ReadInt();
                for (int i = 0; i < countEquipmentAttribute; i++)
                {
                    equipmentAttributesResult.attributesData.item0_Attributes.Add(new Item0_Attribute
                    {
                        ID = reader.ReadInt(),
                        IDItem0 = reader.ReadInt(),
                        IDAttribute = reader.ReadInt(),
                        Value = reader.ReadInt(),
                        Category = reader.ReadInt()
                    });
                    equipmentAttributesResult.attributesData.nameAttributes.Add(new Attribute
                    {
                        NameAttribute = reader.ReadString()
                    });
                }

                if (equipmentAttributesResult == null || equipmentAttributesResult.attributesData.item0_Attributes.Count == 0)
                    return;

                nameItemText.text = $"{equipmentAttributesResult.attributesData.nameItem0_1}";
                for (int i = 0; i < equipmentAttributesResult.attributesData.item0_Attributes.Count; i++)
                {
                    itemInfoText.text += $"{equipmentAttributesResult.attributesData.item0_Attributes[i].Value} {equipmentAttributesResult.attributesData.nameAttributes[i].NameAttribute} \n";
                }
                break;

            case "inventoryAttributes":
                data = socketManager.GetInventoryAttributesData();

                if (data == null || data.Length == 0)
                    return;

                reader = new PacketReaderManager(data);

                ReadAttributesInventoryResultPacket inventoryAttributesResult = new ReadAttributesInventoryResultPacket();
                inventoryAttributesResult.cmd = (EnumCmdCode)reader.ReadInt();
                inventoryAttributesResult.attributesItem0Data = new InventoryItem0Data();
                inventoryAttributesResult.attributesItem0Data.id = reader.ReadInt();
                inventoryAttributesResult.attributesItem0Data.idItem0 = reader.ReadInt();
                inventoryAttributesResult.attributesItem0Data.nameItem0 = reader.ReadString();
                inventoryAttributesResult.attributesItem0Data.typeItem0 = reader.ReadString();
                inventoryAttributesResult.attributesItem0Data.category = reader.ReadInt();
                inventoryAttributesResult.attributesItem0Data.idSchool = reader.ReadInt();
                inventoryAttributesResult.attributesItem0Data.level = reader.ReadInt();
                inventoryAttributesResult.attributesItem0Data.item0_Attributes = new List<Item0_Attribute>();
                inventoryAttributesResult.attributesItem0Data.nameAttributes = new List<Attribute>();

                int countInventoryAttribute = reader.ReadInt();
                for (int i = 0; i < countInventoryAttribute; i++)
                {
                    inventoryAttributesResult.attributesItem0Data.item0_Attributes.Add(new Item0_Attribute
                    {
                        ID = reader.ReadInt(),
                        IDItem0 = reader.ReadInt(),
                        IDAttribute = reader.ReadInt(),
                        Value = reader.ReadInt(),
                        Category = reader.ReadInt()
                    });
                    inventoryAttributesResult.attributesItem0Data.nameAttributes.Add(new Attribute
                    {
                        NameAttribute = reader.ReadString()
                    });
                }

                // tạm thời chỉ xét item0
                if (inventoryAttributesResult == null || inventoryAttributesResult.attributesItem0Data.item0_Attributes.Count == 0)
                    return;

                nameItemText.text = $"{inventoryAttributesResult.attributesItem0Data.nameItem0}";
                for (int i = 0; i < inventoryAttributesResult.attributesItem0Data.item0_Attributes.Count; i++)
                {
                    itemInfoText.text += $"{inventoryAttributesResult.attributesItem0Data.item0_Attributes[i].Value} {inventoryAttributesResult.attributesItem0Data.nameAttributes[i].NameAttribute} \n";
                }
                break;

            default:
                break;
        }
    }

    public void OnLateUpdate() { }
    public void OnFixedUpdate() { }
    public void RegisterDontDestroyOnLoad()
    {
        DontDestroyOnLoad(this.gameObject);
    }

    // Đọc attribute của item trong equipment
    private async void ReadAttributeInEquipment(int idSlot)
    {
        int idAccount = LogInView.GetIDAccount() ?? 0;
        List<EquipmentData> equipmentInfo = EquipmentView.GetListEquipmentSlots();
        int idItem0_1 = equipmentInfo[idSlot].idItem0_1;

        if (idItem0_1 == 0 || idSlot >= equipmentInfo.Count) // Slot trống hoặc ngoài phạm vi
        {
            Debug.Log("Slot này chưa có item");
            itemInfo.SetActive(false);
            return;
        }

        ReadAttributesEquipmentRequestPacket equipmentAttributesRequestPacket = new ReadAttributesEquipmentRequestPacket
        {
            cmd = EnumCmdCode.equipmentAttributes,
            idAccount = idAccount,
            id = equipmentInfo[idSlot].id,
            idItem0_1 = idItem0_1
        };

        PacketWriterManager writer = new PacketWriterManager();
        writer.WriteInt((int)equipmentAttributesRequestPacket.cmd);
        writer.WriteInt(equipmentAttributesRequestPacket.idAccount);
        writer.WriteInt(equipmentAttributesRequestPacket.id);
        writer.WriteInt(equipmentAttributesRequestPacket.idItem0_1);

        await socketManager.SendToServer(writer.ToArray());

        itemInfo.SetActive(true);
        itemInfoText.text = "";

        nameItem.SetActive(true);
        nameItemText.text = "";

        cmdReadAttributes = "equipmentAttributes";
    }
    public void ClickReadAttributeInEquipment(int idSlot)
    {
        ReadAttributeInEquipment(idSlot);
    }

    // Đọc attribute của item trong inventory
    private async void ReadAttributeInInventory(int idSlot)
    {
        indexSlot = idSlot;
        int idAccount = LogInView.GetIDAccount() ?? 0;
        List<InventoryItem0Data> inventoryInfo = InventoryView.GetListInventoryItem0Slots();

        if (idSlot < 0 || idSlot >= inventoryInfo.Count)
        {
            Debug.Log("Slot không có item");
            itemInfo.SetActive(false);
            return;
        }

        int idItem0 = inventoryInfo[idSlot].idItem0;

        ReadAttributesInventoryRequestPacket inventoryAttributesRequestPacket = new ReadAttributesInventoryRequestPacket
        {
            cmd = EnumCmdCode.inventoryAttributes,
            idAccount = idAccount,
            id = inventoryInfo[idSlot].id,
            idItem0 = inventoryInfo[idSlot].idItem0
        };

        PacketWriterManager writer = new PacketWriterManager();
        writer.WriteInt((int)inventoryAttributesRequestPacket.cmd);
        writer.WriteInt(inventoryAttributesRequestPacket.idAccount);
        writer.WriteInt(inventoryAttributesRequestPacket.id);
        writer.WriteInt(inventoryAttributesRequestPacket.idItem0);

        await socketManager.SendToServer(writer.ToArray());

        itemInfo.SetActive(true);
        itemInfoText.text = "";

        nameItem.SetActive(true);
        nameItemText.text = "";

        cmdReadAttributes = "inventoryAttributes";
    }
    public void ClickReadAttributeInInventory(int idSlot)
    {
        ReadAttributeInInventory(idSlot);
    }

    // Trang bị item từ inventory vào equipment
    private async void EquipItem0(int idSlot)
    {
        int idAccount = LogInView.GetIDAccount() ?? 0;
        int idSchool = LogInView.GetIDSchool();
        try
        {
            List<InventoryItem0Data> inventoryInfo = InventoryView.GetListInventoryItem0Slots();

            if (idSlot < 0 || idSlot >= inventoryInfo.Count)
            {
                Debug.LogError($"Index slot không hợp lệ: {idSlot}");
                return;
            }

            int idItem0 = inventoryInfo[idSlot].idItem0;

            if (idItem0 == 0)
            {
                Debug.Log("Slot này chưa có item");
                return;
            }

            if (idSchool != inventoryInfo[idSlot].idSchool && inventoryInfo[idSlot].idSchool != 0)
            {
                switch (inventoryInfo[idSlot].idSchool)
                {
                    case 1:
                        Debug.Log("Chỉ có Chiến Binh mới có thể sử dụng vật phẩm này.");
                        return;
                    case 2:
                        Debug.Log("Chỉ có Sát Thủ mới có thể sử dụng vật phẩm này.");
                        return;
                    case 3:
                        Debug.Log("Chỉ có Pháp Sư mới có thể sử dụng vật phẩm này.");
                        return;
                    case 4:
                        Debug.Log("Chỉ có Xạ Thủ mới có thể sử dụng vật phẩm này.");
                        return;
                }
            }

            List<Image> equipmentImages = EquipmentView.GetListImagesEquipmentSlots();

            string typeItem0 = inventoryInfo[idSlot].typeItem0;

            if (typeItem0.Equals("Ring") && ringSlot == null)
            {
                ring1Ring2Menu.SetActive(true);
                ring1Ring2Choice[0].sprite = equipmentImages[6].sprite;
                ring1Ring2Choice[1].sprite = equipmentImages[7].sprite;
                return;
            }

            if (ringSlot != null)
            {
                typeItem0 = ringSlot;
            }

            EquipItem0RequestPacket equipItem0RequestPacket = new EquipItem0RequestPacket
            {
                cmd = EnumCmdCode.equipItem0,
                idAccount = idAccount,
                id = inventoryInfo[idSlot].id,
                idItem0 = inventoryInfo[idSlot].idItem0,
                slotName = typeItem0
            };

            PacketWriterManager writer = new PacketWriterManager();
            writer.WriteInt((int)equipItem0RequestPacket.cmd);
            writer.WriteInt(equipItem0RequestPacket.idAccount);
            writer.WriteInt(equipItem0RequestPacket.id);
            writer.WriteInt(equipItem0RequestPacket.idItem0);
            writer.WriteString(equipItem0RequestPacket.slotName);

            await socketManager.SendToServer(writer.ToArray());
            
            ringSlot = null;
            itemInfoText.text = "";
            itemInfo.SetActive(false);

            nameItemText.text = "";
            nameItem.SetActive(false);
        }
        catch (Exception ex)
        {
            Debug.LogError("Lỗi khi trang bị item: " + ex.Message);
            return;
        }
    }
    public void ClickEquipItem0()
    {
        EquipItem0(indexSlot);
    }
    public void ClickRing1Choice()
    {
        ring1Ring2Menu.SetActive(false);
        ringSlot = "Ring1";

        EquipItem0(indexSlot);
    }
    public void ClickRing2Choice()
    {
        ring1Ring2Menu.SetActive(false);
        ringSlot = "Ring2";

        EquipItem0(indexSlot);
    }
}