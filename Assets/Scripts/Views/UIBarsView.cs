using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIBarsView : MonoBehaviour, IUpdatable
{
    [SerializeField] private Slider hpBar;
    [SerializeField] private TMP_Text hpInfo;
    [SerializeField] private Slider mpBar;
    [SerializeField] private TMP_Text mpInfo;

    [SerializeField] private GameObject updateHPUI;

    private int lastHP;

    private GameObject player;
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
        hpInfo.text = $"{hpBar.value} / {hpBar.maxValue}";
        lastHP = hp;

        mpBar.maxValue = maxMP;
        mpBar.value = mp;
        mpInfo.text = $"{mpBar.value} / {mpBar.maxValue}";
    }

    public void UpdateHP(int hp)
    {
        hpBar.value = hp;
        hpInfo.text = $"{hpBar.value} / {hpBar.maxValue}";
    }

    public void UpdateMP(int mp)
    {
        mpBar.value = mp;
        mpInfo.text = $"{mpBar.value} / {mpBar.maxValue}";
    }

    public void OnUpdate() 
    {
        if (player == null)
        {
            switch (LogInView.GetIDSchool())
            {
                case 1:
                    player = GameObject.Find("ChienBinh(Clone)");
                    break;
                case 2:
                    player = GameObject.Find("SatThu(Clone)");
                    break;
                case 3:
                    player = GameObject.Find("PhapSu(Clone)");
                    break;
                case 4:
                    player = GameObject.Find("XaThu(Clone)");
                    break;
            }

            if (player == null)
                return;
        }

        Vector3 canvasScaleInPlayer = player.GetComponentInChildren<Canvas>().transform.localScale;
        canvasScaleInPlayer.x = player.transform.localScale.x < 0 ? -Math.Abs(canvasScaleInPlayer.x) : Math.Abs(canvasScaleInPlayer.x);
        player.GetComponentInChildren<Canvas>().transform.localScale = canvasScaleInPlayer;

        PlayerInjured();
        // tính năng heal sẽ cập nhật tiếp theo ở đây
    }

    public void OnLateUpdate() { }
    public void OnFixedUpdate() { }

    private void PlayerInjured()
    {
        byte[] data = socketManager.GetMobsAttackPlayerData();

        if (data == null || data.Length == 0)
            return;

        PacketReaderManager reader = new PacketReaderManager(data);
        EnumCmdCode cmd = (EnumCmdCode)reader.ReadInt();
        int idAccount = reader.ReadInt();
        int mobDamage = reader.ReadInt();
        int playerHP = reader.ReadInt();
        UpdateHP(playerHP);

        if (lastHP != playerHP)
        {
            if (playerHP < lastHP)
            {
                if (playerHP <= 0)
                    player.GetComponent<MovementPlayerController>().UpdateDieAnimation();

                GameObject objectDamageUI = Instantiate(updateHPUI, player.GetComponentInChildren<Canvas>().transform, false);

                UpdateHPUIController injuredDamageUI = objectDamageUI.GetComponent<UpdateHPUIController>();
                if (injuredDamageUI != null)
                {
                    injuredDamageUI.SetInjuredDamage(mobDamage);
                }
                player.GetComponent<MovementPlayerController>().UpdateInjuredAnimation();
            }
            lastHP = playerHP;
        }
    }

    public void RegisterDontDestroyOnLoad() { }
}