using UnityEngine;

class OtherPlayersController : MonoBehaviour, IUpdatable
{
    [SerializeField] private GameObject chienBinh;
    [SerializeField] private GameObject satThu;
    [SerializeField] private GameObject phapSu;
    //private GameObject XaThu;

    private void Awake() { }
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
    public void OnUpdate() { }
    public virtual void OnLateUpdate() { }
    public virtual void OnFixedUpdate() { }
    public void RegisterDontDestroyOnLoad()
    {
        GameManager.Instance.RegisterPersistent(this);
    }

    public void Init(SyncModels data)
    {
        switch (data.idSchool)
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