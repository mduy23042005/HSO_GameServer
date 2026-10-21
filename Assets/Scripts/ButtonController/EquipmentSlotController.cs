using UnityEngine;
using UnityEngine.EventSystems;

public class EquipmentSlotClickEvent
{
    public int idSlot;
}
public class EquipmentSlotController : MonoBehaviour, IPointerClickHandler
{
    public void OnPointerClick(PointerEventData eventData)
    {
        int idSlot = transform.GetSiblingIndex();

        ObserverManager.Notify(new EquipmentSlotClickEvent { idSlot = idSlot });
    }
}