using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MenuView : MonoBehaviour, IUpdatable
{
    [SerializeField] private GameObject nameItem;
    [SerializeField] private GameObject itemInfo;

    [SerializeField] private List<GameObject> listDemo;

    private TMP_Text nameItemText;
    private TMP_Text infoText;
    private bool isActive = false;

    private void Awake()
    {
        infoText = itemInfo.GetComponent<TMP_Text>();
        nameItemText = nameItem.GetComponent<TMP_Text>();
    }
    private void Start()
    {
        int idSchool = LogInView.GetIDSchool();

        for (int i = 0; i < listDemo.Count; i++)
        {
            if (i != idSchool - 1)
            {
                Destroy(listDemo[i]);
            }
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
        itemInfo.SetActive(false);

        nameItemText.text = "";
        nameItem.SetActive(false);
    }
    public bool GetIsActive()
    {
        return isActive;
    }
}
