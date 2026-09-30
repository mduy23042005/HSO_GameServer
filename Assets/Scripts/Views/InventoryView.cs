using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryView : MonoBehaviour, IUpdatable
{
    [Header("Các ô hành trang")]
    [SerializeField] private List<Image> inventorySlots;

    public static List<InventoryItem0Data> inventoryItem0s;

    private SocketManager socketManager;

    private Action<LogOutClickEvent> logoutObserver;

    private void Awake()
    {
        socketManager = GameManager.Instance.GetComponent<SocketManager>();

        // Update UI
        for (int i = 0; i < inventoryItem0s.Count; i++)
        {
            int itemId = inventoryItem0s[i].idItem0;
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
        for (int i = inventoryItem0s.Count; i < inventorySlots.Count; i++)
        {
            inventorySlots[i].sprite = null;
            inventorySlots[i].color = new Color(0f, 0f, 0f, 0f);
        }

        logoutObserver = eventData => ClearInventoryData();
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

    public void OnUpdate() // tạm thời chỉ có item0
    {
        byte[] data = socketManager.GetInventoryData();

        if (data == null || data.Length == 0)
            return;

        PacketReaderManager reader = new PacketReaderManager(data);

        InventoryResultPacket inventoryResult = new InventoryResultPacket();
        inventoryResult.cmd = (EnumCmdCode)reader.ReadInt();
        inventoryResult.inventoryItem0Data = new List<InventoryItem0Data>();
        inventoryResult.inventoryItem1Data = new List<InventoryItem1Data>();
        inventoryResult.inventoryItem2Data = new List<InventoryItem2Data>();
        inventoryResult.inventoryItem3Data = new List<InventoryItem3Data>();
        inventoryResult.inventoryItem4Data = new List<InventoryItem4Data>();

        int countInventoryItem0Data = reader.ReadInt();
        for (int i = 0; i < countInventoryItem0Data; i++)
        {
            inventoryResult.inventoryItem0Data.Add(new InventoryItem0Data 
            {
                id = reader.ReadInt(),
                idItem0 = reader.ReadInt(),
                nameItem0 = reader.ReadString(),
                typeItem0 = reader.ReadString(),
                category = reader.ReadInt(),
                idSchool = reader.ReadInt(),
            });
        }

        if (inventoryItem0s == null)
            inventoryItem0s = new List<InventoryItem0Data>();

        for (int i = 0; i < inventoryResult.inventoryItem0Data.Count; i++)
        {
            if (i >= inventoryItem0s.Count)
            {
                inventoryItem0s.Add(inventoryResult.inventoryItem0Data[i]);
            }
            else
            {
                inventoryItem0s[i] = inventoryResult.inventoryItem0Data[i];
            }
        }

        // Update UI
        for (int i = 0; i < inventoryItem0s.Count; i++)
        {
            int itemId = inventoryItem0s[i].idItem0;
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

    public static void ClearInventoryData()
    {
        inventoryItem0s.Clear();
    }
}