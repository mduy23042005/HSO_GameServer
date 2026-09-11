using TMPro;
using UnityEngine;

public class UpdateHPUIController : MonoBehaviour, IUpdatable
{
    [SerializeField] private GameObject splatterBloodUI;
    private float moveSpeed = 250f;
    private float lifeTime = 0.5f;

    private float timer;
    private RectTransform rect;

    private GameObject splatterBlood;

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
            timer = 0;

            PoolManager.Instance.Release(gameObject);
        }

        if (splatterBlood != null && splatterBlood.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.5f)
        {
            PoolManager.Instance.Release(splatterBlood);
            splatterBlood = null;
        }
    }

    public void OnLateUpdate() { }
    public void OnFixedUpdate() { }

    public void RegisterDontDestroyOnLoad() { }

    public void SetInjuredDamage(GameObject attackObject, int damage, GameObject injuredObject, Vector3 splatterBloodPosition)
    {
        var txt = GetComponentInChildren<TMP_Text>();
        if (txt != null)
            txt.text = $"- {damage}";

        this.transform.SetParent(injuredObject.GetComponentInChildren<Canvas>().transform, false);
        this.transform.localPosition = Vector3.zero;

        splatterBlood = PoolManager.Instance.Get(splatterBloodUI);
        splatterBlood.transform.SetParent(injuredObject.transform, false);

        if (attackObject.transform.position.x > injuredObject.transform.position.x)
        {
            splatterBlood.transform.localPosition = new Vector3(splatterBloodPosition.x * -1, splatterBloodPosition.y, splatterBloodPosition.z);
            splatterBlood.transform.localScale = new Vector3(-1, 1, 1);
        }
        else
        {
            splatterBlood.transform.localPosition = new Vector3(splatterBloodPosition.x, splatterBloodPosition.y, splatterBloodPosition.z);
            splatterBlood.transform.localScale = new Vector3(1, 1, 1);
        }
    }
}