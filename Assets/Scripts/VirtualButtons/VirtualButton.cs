using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class VirtualButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IUpdatable
{
    [SerializeField] private float scaleLimit = 0.9f;
    [SerializeField] private float scaleSpeed = 15;

    private bool scale = false;

    private void Awake()
    {
#if UNITY_ANDROID
        gameObject.SetActive(true);
#elif UNITY_STANDALONE || UNITY_EDITOR
        gameObject.SetActive(false);
#endif
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

    public void OnPointerDown(PointerEventData pointerEventData)
    {
        scale = true;
        PlayerManager.player.GetComponent<MovementPlayerController>().ChaseAndAttack();
    }

    public void OnPointerUp(PointerEventData pointerEventData)
    {
        scale = false;
    }

    public void OnUpdate() { }

    public void OnLateUpdate() { }

    public void OnFixedUpdate()
    {
        float targetScale = scale ? scaleLimit : 1f;

        // Rotate the cube by converting the angles into a quaternion.
        Vector3 target = new Vector3(targetScale, targetScale, 1f);

        // Dampen towards the target rotation
        transform.localScale = Vector3.Lerp(transform.localScale, target, Time.fixedDeltaTime * scaleSpeed);
    }

    public void RegisterDontDestroyOnLoad()
    {
        GameManager.Instance.RegisterPersistent(this);
    }
}
