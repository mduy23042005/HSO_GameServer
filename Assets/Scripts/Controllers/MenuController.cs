using TMPro;
using UnityEngine;

public class MenuController : MonoBehaviour, IUpdatable
{
    [SerializeField] private GameObject itemInfo;

    private GameObject playerDemo;
    private GameObject chienBinhDemo;
    private GameObject satThuDemo;
    private GameObject phapSuDemo;
    //private GameObject xaThuDemo;

    private TMP_Text infoText;

    private bool isActive = false;
    private void Awake()
    {
        playerDemo = GameObject.Find("PlayerDemo").gameObject;
        chienBinhDemo = playerDemo.transform.Find("ChienBinh").gameObject;
        satThuDemo = playerDemo.transform.Find("SatThu").gameObject;
        phapSuDemo = playerDemo.transform.Find("PhapSu").gameObject;
        //xaThuDemo = playerDemo.transform.Find("XaThu").gameObject;

        infoText = itemInfo.GetComponent<TMP_Text>();
    }
    private void Start()
    {
        if (GameObject.Find("Player"))
        {
            var idSchool = LogInController.GetIDSchool();
            switch (idSchool)
            {
                case 1:
                    chienBinhDemo.SetActive(true);
                    Destroy(satThuDemo);
                    Destroy(phapSuDemo);
                    //Destroy(xaThuDemo);
                    break;
                case 2:
                    Destroy(chienBinhDemo);
                    satThuDemo.SetActive(true);
                    Destroy(phapSuDemo);
                    //Destroy(xaThuDemo);
                    break;
                case 3:
                    Destroy(chienBinhDemo);
                    Destroy(satThuDemo);
                    phapSuDemo.SetActive(true);
                    //Destroy(xaThuDemo);
                    break;
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
    public void OnUpdate()
    {

    }
    public void OnLateUpdate()
    {
    }
    public void OnFixedUpdate()
    {
    }
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
    }
    public bool getIsActive()
    {
        return isActive;
    }
}
