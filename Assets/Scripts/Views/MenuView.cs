using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MenuView : MonoBehaviour, IUpdatable
{
    [SerializeField] private GameObject nameItem;
    [SerializeField] private GameObject attributeItem;

    [SerializeField] private List<RectTransform> demoPlayerUIPrefabs;

    [SerializeField] private RectTransform demoPlayerParentUIObject;
    private RectTransform demoPlayerUI;

    private TMP_Text nameItemText;
    private TMP_Text infoText;
    private bool isActive = false;

    private void Awake()
    {
        infoText = attributeItem.GetComponent<TMP_Text>();
        nameItemText = nameItem.GetComponent<TMP_Text>();
    }
    private void Start()
    {
        int idSchool = LogInView.GetIDSchool() - 1;

        if (idSchool < 0)
            idSchool = 0;

        if (LogInView.GetIDAccount() != 0)
        {
            demoPlayerUI = PoolManager.Instance.Get(demoPlayerUIPrefabs[idSchool], demoPlayerParentUIObject);

            demoPlayerUI.anchorMin = new Vector2(0.5f, 0.5f);
            demoPlayerUI.anchorMax = new Vector2(0.5f, 0.5f);
            demoPlayerUI.pivot = new Vector2(0.5f, 0.5f);

            demoPlayerUI.anchoredPosition = Vector2.zero;
            demoPlayerUI.localPosition = Vector3.zero;
        }

        gameObject.SetActive(isActive);
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

    public void OnUpdate(){ }
    public void OnLateUpdate() { }
    public void OnFixedUpdate() { }

    public void RegisterDontDestroyOnLoad()
    {
        GameManager.Instance.RegisterPersistent(this);
    }

    public void OpenMenu()
    {
        isActive = true;
        gameObject.SetActive(isActive);
    }
    public void CloseMenu()
    {
        isActive = false;
        gameObject.SetActive(isActive);

        infoText.text = "";
        attributeItem.SetActive(false);

        nameItemText.text = "";
        nameItem.SetActive(false);
    }
    public bool GetIsActive()
    {
        return isActive;
    }
}
