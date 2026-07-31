using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static System.Net.Mime.MediaTypeNames;

public class UIBarsView : MonoBehaviour, IUpdatable
{
    [SerializeField] private Slider hpBar;
    [SerializeField] private TMP_Text hpInfo;
    [SerializeField] private Slider mpBar;
    [SerializeField] private TMP_Text mpInfo;
    [SerializeField] private TMP_Text fpsInfo;

    [SerializeField] private GameObject updateHPUI;

    private int lastHP;

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
        if (PlayerManager.player == null)
            return;

        Vector3 canvasScaleInPlayer = PlayerManager.player.GetComponentInChildren<Canvas>().transform.localScale;
        canvasScaleInPlayer.x = PlayerManager.player.transform.localScale.x < 0 ? -Math.Abs(canvasScaleInPlayer.x) : Math.Abs(canvasScaleInPlayer.x);
        PlayerManager.player.GetComponentInChildren<Canvas>().transform.localScale = canvasScaleInPlayer;

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
                    PlayerManager.player.GetComponent<MovementPlayerController>().UpdateDieAnimation();
                else
                {
                    PlayerManager.player.GetComponent<MovementPlayerController>().UpdateInjuredAnimation();
                    PlayerManager.player.GetComponent<SpritePlayerController>().UpdateInjuredSprite();
                }

                GameObject objectDamageUI = PoolManager.Instance.Get(updateHPUI);
                objectDamageUI.transform.SetParent(PlayerManager.player.GetComponentInChildren<Canvas>().transform, false);
                objectDamageUI.transform.localPosition = Vector3.zero;

                UpdateHPUIController injuredDamageUI = objectDamageUI.GetComponent<UpdateHPUIController>();
                if (injuredDamageUI != null)
                {
                    injuredDamageUI.SetInjuredDamage(mobDamage);
                }
            }
            lastHP = playerHP;
        }
    }

    public void RegisterDontDestroyOnLoad() { }
}