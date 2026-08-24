using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.Layouts;
using UnityEngine.InputSystem.OnScreen;
using UnityEngine.UI;

public enum VirtualDPadDirection { Both, Horizontal, Vertical }

public class VirtualDPad : OnScreenControl, IPointerDownHandler, IPointerUpHandler, IDragHandler
{
    [SerializeField] private RectTransform centerArea = null;
    [SerializeField] private RectTransform handle = null;
    [SerializeField] private VirtualDPadDirection direction = VirtualDPadDirection.Both;
    [InputControl(layout = "Vector2")]
    [SerializeField] private string dPadControlPath;
    [SerializeField] private float movementRange = 10f;
    [SerializeField] private float moveThreshold = 0f;
    [SerializeField] private float uiMovementRange = 10f;
    [SerializeField] private bool forceIntValue = true;

    private Vector3 startPos;

    protected override string controlPathInternal
    {
        get => dPadControlPath;
        set => dPadControlPath = value;
    }

    private void Awake()
    {
        if (centerArea == null)
            centerArea = GetComponent<RectTransform>();

        Vector2 center = new Vector2(0.5f, 0.5f);
        centerArea.pivot = center;
        handle.anchorMin = center;
        handle.anchorMax = center;
        handle.pivot = center;
        handle.anchoredPosition = Vector2.zero;

#if UNITY_ANDROID
        gameObject.SetActive(true);
#elif UNITY_STANDALONE || UNITY_EDITOR
        gameObject.SetActive(false);
#endif
    }

    private void Start()
    {
        startPos = handle.anchoredPosition;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (eventData == null)
            throw new System.ArgumentNullException(nameof(eventData));

        OnDrag(eventData);
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (eventData == null)
            throw new System.ArgumentNullException(nameof(eventData));

        RectTransformUtility.ScreenPointToLocalPointInRectangle(handle.parent.GetComponentInParent<RectTransform>(), eventData.position, eventData.pressEventCamera, out var position);
        Vector2 delta = position;

        if (direction == VirtualDPadDirection.Horizontal) delta.y = 0;
        else if (direction == VirtualDPadDirection.Vertical) delta.x = 0;

        Vector2 buttonDelta = Vector2.ClampMagnitude(delta, uiMovementRange);
        handle.anchoredPosition = startPos + (Vector3)buttonDelta;

        Vector2 newPos = SanitizePosition(delta);
        PlayerManager.player.GetComponent<MovementPlayerController>().SetMovement(newPos.x, newPos.y);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        handle.anchoredPosition = startPos;
        PlayerManager.player.GetComponent<MovementPlayerController>().SetMovement(Vector2.zero.x, Vector2.zero.y);
    }

    private Vector2 SanitizePosition(Vector2 pos)
    {
        pos = Vector2.ClampMagnitude(pos, movementRange);

        float minMovementRange = moveThreshold > movementRange ? movementRange : moveThreshold;

        if (Mathf.Abs(pos.x) < minMovementRange)
            pos.x = 0;

        if (Mathf.Abs(pos.y) < minMovementRange)
            pos.y = 0;

        pos /= movementRange;

        if (forceIntValue)
        {
            // Chỉ cho phép 4 hướng
            if (Mathf.Abs(pos.x) > Mathf.Abs(pos.y))
            {
                pos.x = Mathf.Sign(pos.x);
                pos.y = 0;
            }
            else if (Mathf.Abs(pos.y) > Mathf.Abs(pos.x))
            {
                pos.x = 0;
                pos.y = Mathf.Sign(pos.y);
            }
            else
            {
                // Trường hợp kéo đúng 45 độ
                // Có thể ưu tiên trục Y
                if (pos.x != 0 || pos.y != 0)
                {
                    pos.x = 0;
                    pos.y = Mathf.Sign(pos.y);
                }
            }
        }
        return pos;
    }
}
