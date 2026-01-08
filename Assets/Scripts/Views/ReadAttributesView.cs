using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

//Packet đọc attribute trong equipment
[Serializable]
public class ReadAttributesEquipmentRequestPacket
{
    public string cmd;
    public int idAccount;
    public int id;
    public int idItem0_1;
}
[Serializable]
public class ReadAttributesEquipmentResultPacket
{
    public string cmd;
    public int idItem0_1;
    public int category;
    public string nameItem;
    public int value;
    public int idAttribute;
    public string attributes;
}

//Packet đọc attribute trong inventory
[Serializable]
public class ReadAttributesInventoryRequestPacket
{
    public string cmd;
    public int idAccount;
    public int id;
    public int idItem0;
}
[Serializable]
public class ReadAttributesInventoryResultPacket
{
    public string cmd;
    public int idItem0;
    public int category;
    public string nameItem;
    public int value;
    public int idAttribute;
    public string attributes;
}

//Packet trang bị item0 từ inventory vào equipment
public class EquipItem0RequestPacket
{
    public string cmd;
    public int idAccount;
    public int id;
    public int idItem0;
    public string slotName;
}
//Sử dụng 2 packet EquipmentResultPacket và InventoryResultPacket để cập nhật lại UI sau khi trang bị

class ReadAttributesView : MonoBehaviour, IUpdatable
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
    private PacketSerializeManager packetSerializeManager;
    private string cmdReadAttributes;

    List<ReadAttributesInventoryResultPacket> inventoryAttributesResult;

    private void Awake()
    {
        socketManager = GameManager.Instance.GetComponent<SocketManager>();
        packetSerializeManager = GameManager.Instance.GetComponent<PacketSerializeManager>();

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
        string data;

        switch (cmdReadAttributes)
        { 
            case "equipmentAttributes":
                data = socketManager.GetEquipmentAttributesData();

                if (string.IsNullOrEmpty(data))
                    return;

                List<ReadAttributesEquipmentResultPacket> equipmentAttributesResult = packetSerializeManager.HandleReceivedPacket<List<ReadAttributesEquipmentResultPacket>>(data);

                if (equipmentAttributesResult == null || equipmentAttributesResult.Count == 0)
                    return;

                for (int i = 0; i < equipmentAttributesResult.Count; i++)
                {
                    if (equipmentAttributesResult[i].cmd != "equipmentAttributes_result")
                        continue;

                    nameItemText.text = $"{equipmentAttributesResult[i].nameItem}";
                    itemInfoText.text += $"{equipmentAttributesResult[i].value} {equipmentAttributesResult[i].attributes} \n";
                }
                break;

            case "inventoryAttributes":
                data = socketManager.GetInventoryAttributesData();

                if (string.IsNullOrEmpty(data))
                    return;

                inventoryAttributesResult = packetSerializeManager.HandleReceivedPacket<List<ReadAttributesInventoryResultPacket>>(data);

                if (inventoryAttributesResult == null || inventoryAttributesResult.Count == 0)
                    return;

                for (int i = 0; i < inventoryAttributesResult.Count; i++)
                {
                    if (inventoryAttributesResult[i].cmd != "inventoryAttributes_result")
                        continue;

                    nameItemText.text = $"{inventoryAttributesResult[i].nameItem}";
                    itemInfoText.text += $"{inventoryAttributesResult[i].value} {inventoryAttributesResult[i].attributes} \n";
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
        List<EquipmentResultPacket> equipmentInfo = EquipmentView.GetListEquipmentSlots();
        int idItem0_1 = equipmentInfo[idSlot].idItem0_1;

        if (idItem0_1 == 0 || idSlot >= equipmentInfo.Count) // Slot trống hoặc ngoài phạm vi
        {
            Debug.Log("Slot này chưa có item");
            itemInfo.SetActive(false);
            return;
        }

        ReadAttributesEquipmentRequestPacket sendEquipmentAttributesRequestPacket = new ReadAttributesEquipmentRequestPacket
        {
            cmd = "equipmentAttributes",
            idAccount = idAccount,
            id = equipmentInfo[idSlot].id,
            idItem0_1 = idItem0_1
        };

        packetSerializeManager.HandleSentPacket(sendEquipmentAttributesRequestPacket);

        itemInfo.SetActive(true);
        itemInfoText.text = "";

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
        List<InventoryResultPacket> inventoryInfo = InventoryView.GetListInventorySlots();

        if (idSlot < 0 || idSlot >= inventoryInfo.Count)
        {
            Debug.Log("Slot không có item");
            itemInfo.SetActive(false);
            return;
        }

        int idItem0 = inventoryInfo[idSlot].idItem0;

        ReadAttributesInventoryRequestPacket sendInventoryAttributesRequestPacket = new ReadAttributesInventoryRequestPacket
        {
            cmd = "inventoryAttributes",
            idAccount = idAccount,
            id = inventoryInfo[idSlot].id,
            idItem0 = inventoryInfo[idSlot].idItem0
        };

        packetSerializeManager.HandleSentPacket(sendInventoryAttributesRequestPacket);

        itemInfo.SetActive(true);
        itemInfoText.text = "";

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
            List<InventoryResultPacket> inventoryInfo = InventoryView.GetListInventorySlots();

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

            EquipItem0RequestPacket sendEquipItem0RequestPacket = new EquipItem0RequestPacket
            {
                cmd = "equipItem0",
                idAccount = idAccount,
                id = inventoryInfo[idSlot].id,
                idItem0 = inventoryInfo[idSlot].idItem0,
                slotName = typeItem0
            };

            packetSerializeManager.HandleSentPacket(sendEquipItem0RequestPacket);

            ringSlot = null;
            itemInfoText.text = "";
            itemInfo.SetActive(false);
        }
        catch (System.Exception ex)
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