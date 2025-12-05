using UnityEngine;

public class PlayerController : MonoBehaviour, IUpdatable
{
    private GameObject player;
    private GameObject chienBinh;
    private GameObject satThu;
    private GameObject phapSu;
    //private GameObject XaThu;

    private void Awake()
    {
        if (GameObject.Find("Player"))
        {
            player = GameObject.Find("Player").gameObject;
            chienBinh = player.transform.Find("ChienBinh").gameObject;
            satThu = player.transform.Find("SatThu").gameObject;
            phapSu = player.transform.Find("PhapSu").gameObject;
            //xaThu = player.transform.Find("XaThu").gameObject;
        }
        if (GameObject.Find("Player"))
        {
            var idSchool = LogInController.GetIDSchool();
            switch (idSchool)
            {
                case 1:
                    chienBinh.SetActive(true);
                    Destroy(satThu);
                    Destroy(phapSu);
                    //Destroy(xaThu);
                    break;
                case 2:
                    Destroy(chienBinh);
                    satThu.SetActive(true);
                    Destroy(phapSu);
                    //Destroy(xaThu);
                    break;
                case 3:
                    Destroy(chienBinh);
                    Destroy(satThu);
                    phapSu.SetActive(true);
                    //Destroy(xaThu);
                    break;
            }
        }
    }
    private void OnEnable()
    {
        GameManager.Instance.Register(this);
        RegisterDontDestroyOnLoad();
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
}
