using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class EquipmentRequestPacket
{
    public string cmd;
    public int idAccount;
}
[Serializable]
public class EquipmentResultPacket
{
    public string cmd;
    public int id;
    public int idItem0_1;
    public int category;
}

public class EquipmentView : MonoBehaviour, IUpdatable
{
    [Header("Danh sách các ô hành trang")]
    [SerializeField] private List<Image> equipmentSlots;

    private static List<Image> listImagesEquiment;
    private static List<EquipmentResultPacket> equipment = new List<EquipmentResultPacket>();

    private SocketManager socketManager;
    private PacketSerializeManager packetSerializeManager;

    private void Awake()
    {
        socketManager = GameManager.Instance.GetComponent<SocketManager>();
        packetSerializeManager = GameManager.Instance.GetComponent<PacketSerializeManager>();

        int idAccount = LogInView.GetIDAccount() ?? 0;
        if (idAccount != 0)
        {
            ReadDatabase();
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
        {
            return;
        }

        Debug.Log("Received equipment data successfully!");

        List<EquipmentResultPacket> equipmentResult = packetSerializeManager.HandleReceivedPacket<List<EquipmentResultPacket>>(data);

        for (int i = 0; i < equipmentResult.Count; i++)
        {
            if (i >= equipment.Count)
            {
                if (equipmentResult[i].cmd != "equipment_result")
                    continue;

                equipment.Add(equipmentResult[i]);
            }
            else
            {
                equipment[i] = equipmentResult[i];
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
        listImagesEquiment = equipmentSlots;
    }
    public void OnLateUpdate() { }
    public void OnFixedUpdate() { }
    public void RegisterDontDestroyOnLoad()
    {
        DontDestroyOnLoad(this.gameObject);
    }

    // Đọc dữ liệu từ database và hiển thị vào Equipment Slots
    private async void ReadDatabase()
    {
        int idAccount = LogInView.GetIDAccount() ?? 0;
        EquipmentRequestPacket sendEquipmentRequestPacket = new EquipmentRequestPacket
        {
            cmd = "equipment",
            idAccount = idAccount,
        };

        packetSerializeManager.HandleSentPacket(sendEquipmentRequestPacket);
    }

    public static List<EquipmentResultPacket> GetListEquipmentSlots()
    {
        return equipment;
    }
    public static List<Image> GetListImagesEquipmentSlots()
    {
        return listImagesEquiment;
    }

    public static void ClearEquipmentData()
    {
        equipment.Clear();
    }
    public static void ClearListImagesEquipmentSlots()
    {
        listImagesEquiment.Clear();
    }
}
