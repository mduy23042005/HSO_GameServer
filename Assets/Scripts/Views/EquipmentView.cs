using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EquipmentView : MonoBehaviour, IUpdatable
{
    [Header("Danh sách các ô hành trang")]
    [SerializeField] private List<Image> equipmentSlots;

    public static List<Image> listImagesEquipment;
    public static List<EquipmentData> equipments = new List<EquipmentData>();

    private SocketManager socketManager;

    private Action<LogOutClickEvent> logoutObserver;

    private void Awake()
    {
        socketManager = GameManager.Instance.GetComponent<SocketManager>();

        for (int i = 0; i < equipmentSlots.Count && i < equipments.Count; i++)
        {
            int itemId = equipments[i].idItem0_1;
            if (itemId == 0)
                equipmentSlots[i].sprite = ItemController.Instance.GetDefaultItemSprite(i);
            else
                equipmentSlots[i].sprite = ItemController.Instance.GetItem0(itemId).iconItem0;
        }
        listImagesEquipment = equipmentSlots;

        logoutObserver = eventData =>
        {
            ClearEquipmentData();
            ClearListImagesEquipmentSlots();
        };
    }

    private void OnEnable()
    {
        GameManager.Instance.Register(this);

        ObserverManager.Register<LogOutClickEvent>(logoutObserver);
    }
    private void OnDisable()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.Unregister(this);
        }

        ObserverManager.Unregister<LogOutClickEvent>(logoutObserver);
    }
    public void OnUpdate()
    {
        byte[] data = socketManager.GetEquipmentData();

        if (data == null || data.Length == 0)
            return;

        PacketReaderManager reader = new PacketReaderManager(data);

        EquipmentResultPacket equipmentResult = new EquipmentResultPacket();
        equipmentResult.cmd = (EnumCmdCode)reader.ReadInt();
        equipmentResult.equipmentData = new List<EquipmentData>();

        int countEquipmentData = reader.ReadInt();
        for (int i = 0; i < countEquipmentData; i++)
        {
            equipmentResult.equipmentData.Add(new EquipmentData
            {
                id = reader.ReadInt(),
                idItem0_1 = reader.ReadInt(),
                nameItem0_1 = reader.ReadString(),
                category = reader.ReadInt(),
                slotName = reader.ReadString()
            });
        }

        for (int i = 0; i < equipmentResult.equipmentData.Count; i++)
        {
            if (i >= equipments.Count)
            {
                equipments.Add(equipmentResult.equipmentData[i]);
            }
            else
            {
                equipments[i] = equipmentResult.equipmentData[i];
            }
        }

        // Update UI
        for (int i = 0; i < equipmentSlots.Count && i < equipments.Count; i++)
        {
            int itemId = equipments[i].idItem0_1;
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

    public static List<Image> GetListImagesEquipmentSlots()
    {
        return listImagesEquipment;
    }

    public static void ClearEquipmentData()
    {
        equipments.Clear();
    }
    public static void ClearListImagesEquipmentSlots()
    {
        listImagesEquipment.Clear();
    }
}
