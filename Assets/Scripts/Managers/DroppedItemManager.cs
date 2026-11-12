using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DroppedItemManager : MonoBehaviour, IUpdatable
{
    [SerializeField] private GameObject dropItemPrefab;

    private Dictionary<int, GameObject> droppedItem0s = new Dictionary<int, GameObject>();

    private SocketManager socketManager;

    private Action<LogOutClickEvent> logoutObserver;

    private void Awake()
    {
        socketManager = GameManager.Instance.GetComponent<SocketManager>();

        logoutObserver = eventData => ClearDroppedItem0s();
    }

    private void OnEnable()
    {
        GameManager.Instance.Register(this);

        ObserverManager.Register<LogOutClickEvent>(logoutObserver);
    }
    private void OnDisable()
    {
        if (GameManager.Instance != null)
            GameManager.Instance.Unregister(this);

        ObserverManager.Unregister<LogOutClickEvent>(logoutObserver);
    }

    public void OnUpdate()
    {
        byte[] dropItem0sData = socketManager.GetDroppedItem0sData();

        if (dropItem0sData != null && dropItem0sData.Length > 0)
        {
            PacketReaderManager reader = new PacketReaderManager(dropItem0sData);

            EnumCmdCode cmd = (EnumCmdCode)reader.ReadInt();
            int count = reader.ReadInt();

            HashSet<int> syncedIDs = new HashSet<int>();

            for (int i = 0; i < count; i++)
            {
                int id = reader.ReadInt();
                int idItem0 = reader.ReadInt();
                float posX = reader.ReadFloat();
                float posY = reader.ReadFloat();
                string nameItem0 = reader.ReadString();

                syncedIDs.Add(id);

                CreateDroppedItem0(id, idItem0, posX, posY, nameItem0);
            }

            List<int> removeIDs = new List<int>();

            foreach (var item in droppedItem0s)
            {
                if (!syncedIDs.Contains(item.Key))
                    removeIDs.Add(item.Key);
            }

            foreach (int id in removeIDs)
            {
                PoolManager.Instance.Release(droppedItem0s[id]);
                droppedItem0s.Remove(id);
            }
        }
    }

    private void CreateDroppedItem0(int id, int idItem0, float posX, float posY, string nameItem0)
    {
        if (droppedItem0s.TryGetValue(id, out GameObject existingItem))
        {
            existingItem.transform.position = new Vector3(posX, posY, 0f);
            return;
        }

        GameObject dropItem = PoolManager.Instance.Get(dropItemPrefab);

        dropItem.transform.position = new Vector3(posX, posY, 0f);
        dropItem.GetComponentInChildren<SpriteRenderer>().sprite = ItemController.Instance.GetItem0(idItem0).iconItem0;
        dropItem.GetComponentInChildren<TMP_Text>().text = nameItem0;
        dropItem.GetComponent<ObjectTypeController>().SetID(id);

        droppedItem0s.Add(id, dropItem);
    }

    public void OnLateUpdate() { }
    public void OnFixedUpdate() { }

    public void RegisterDontDestroyOnLoad() { }

    public void ClearDroppedItem0s()
    {
        droppedItem0s.Clear();
    }
}
