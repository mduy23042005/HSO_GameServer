using System;
using TMPro;
using UnityEngine;

public class DropItem0 
{
    public Vector3 droppedPosition;
    public int idItem0;
    public string nameItem0;
}
public class DroppedItemManager : MonoBehaviour
{
    [SerializeField] private GameObject dropItemPrefab;
    private Action<DropItem0> dropItem0Observer;

    private void Awake()
    {
        dropItem0Observer = eventData => 
        { 
            GameObject dropItem = PoolManager.Instance.Get(dropItemPrefab);

            ItemController.Instance.GetItem0(eventData.idItem0);

            dropItem.GetComponentInChildren<SpriteRenderer>().sprite = ItemController.Instance.GetItem0(eventData.idItem0).iconItem0;
            dropItem.GetComponentInChildren<TMP_Text>().text = eventData.nameItem0;
            dropItem.transform.position = eventData.droppedPosition;
        };
    }

    private void OnEnable()
    {
        ObserverManager.Register<DropItem0>(dropItem0Observer);
    }
    private void OnDisable()
    {
        ObserverManager.Unregister<DropItem0>(dropItem0Observer);
    }
}
