using TMPro;
using UnityEngine;

public class UpdateHPUI : MonoBehaviour, IUpdatable
{
    private float moveSpeed = 250f;
    private float lifeTime = 0.5f;

    private float timer;
    private RectTransform rect;

    private void Awake()
    {
        rect = GetComponent<RectTransform>();
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
        // di chuyển lên trên
        rect.anchoredPosition += Vector2.up * moveSpeed * Time.deltaTime;
        // đếm thời gian
        timer += Time.deltaTime;

        if (timer >= lifeTime)
        {
            Destroy(gameObject);
        }
    }

    public void OnLateUpdate() 
    {
        rect.localScale = Vector3.one;
    }
    public void OnFixedUpdate() { }

    public void RegisterDontDestroyOnLoad() { }

    public void SetInjuredDamage(int damage)
    {
        var txt = GetComponentInChildren<TMP_Text>();
        if (txt != null)
        {
            txt.text = $"- {damage}";
        }
    }
}