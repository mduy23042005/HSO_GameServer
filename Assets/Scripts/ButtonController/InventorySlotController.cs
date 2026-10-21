using UnityEngine;
using UnityEngine.EventSystems;

public class InventorySlotClickEvent
{
    public int idSlot;
}

public class InventorySlotController : MonoBehaviour, IPointerClickHandler
{
    public void OnPointerClick(PointerEventData eventData)
    {
        int idSlot = transform.GetSiblingIndex();

        ObserverManager.Notify(new InventorySlotClickEvent { idSlot = idSlot });
    }
}