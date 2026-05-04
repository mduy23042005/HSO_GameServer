using UnityEngine;
using UnityEngine.UI;

public class UIBarsView : MonoBehaviour, IUpdatable
{
    [SerializeField] private Slider hpBar;
    [SerializeField] private Slider mpBar;

    private SocketManager socketManager;

    private void Awake()
    {
        int maxHP = LogInView.GetMaxHP();
        int maxMP = LogInView.GetMaxMP();
        int hp = LogInView.GetHP();
        int mp = LogInView.GetMP();

        Init(maxHP, maxMP, hp, mp);

        socketManager = GameManager.Instance.GetComponent<SocketManager>();
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

    private void Init(int maxHP, int maxMP, int hp, int mp)
    {
        hpBar.maxValue = maxHP;
        hpBar.value = hp;

        mpBar.maxValue = maxMP;
        mpBar.value = mp;
    }

    public void UpdateHP(int hp)
    {
        hpBar.value = hp;
    }

    public void UpdateMP(int mp)
    {
        mpBar.value = mp;
    }

    public void OnUpdate() 
    {
        byte[] data = socketManager.GetReceiveData();

        if (data == null || data.Length == 0)
            return;

        PacketReaderManager reader = new PacketReaderManager(data);
        EnumCmdCode cmd = (EnumCmdCode)reader.ReadInt();
        int hp = reader.ReadInt();
        UpdateHP(hp);
    }

    public void OnLateUpdate() { }
    public void OnFixedUpdate() { }

    public void RegisterDontDestroyOnLoad() { }
}