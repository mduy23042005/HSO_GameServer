using UnityEngine;

public class PlayerController : MonoBehaviour, IUpdatable
{
    [SerializeField] private GameObject chienBinh;
    [SerializeField] private GameObject satThu;
    [SerializeField] private GameObject phapSu;
    //private GameObject XaThu;

    private void Awake() 
    {
        if (gameObject.name == "PlayerDemo")
        {
            switch (LogInView.GetIDSchool())
            {
                case 1:
                    chienBinh.SetActive(true);
                    Destroy(satThu);
                    Destroy(phapSu);
                    break;
                case 2:
                    satThu.SetActive(true);
                    Destroy(chienBinh);
                    Destroy(phapSu);
                    break;
                case 3:
                    phapSu.SetActive(true);
                    Destroy(chienBinh);
                    Destroy(satThu);
                    break;
            }
        }
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
    public void OnLateUpdate() { }
    public void OnFixedUpdate() { }
    public void RegisterDontDestroyOnLoad()
    {
        GameManager.Instance.RegisterPersistent(this);
    }

    public void SetupBySchool(int idSchool)
    {
        switch (idSchool)
        {
            case 1:
                chienBinh.SetActive(true);
                Destroy(satThu);
                Destroy(phapSu);
                break;
            case 2:
                satThu.SetActive(true);
                Destroy(chienBinh);
                Destroy(phapSu);
                break;
            case 3:
                phapSu.SetActive(true);
                Destroy(chienBinh);
                Destroy(satThu);
                break;
        }
    }
}
