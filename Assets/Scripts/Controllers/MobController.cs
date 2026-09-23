using System.IO;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static UnityEditorInternal.VersionControl.ListControl;

public class MobController : MonoBehaviour, IUpdatable
{
    [SerializeField] private TMP_Text uiNameMob;
    [SerializeField] private Slider hpBar;
    [SerializeField] private GameObject sprite;

    [SerializeField] private GameObject shadow;
    [SerializeField] private GameObject waterShadow;

    private Vector2 movement;

    private SyncMobData syncMobDataMovement;
    private int lastIDStateStand = -1;
    private int lastIDStateAtk = -1;
    private int lastIDStateInjured = -1;
    private int lastIDStateDie = -1;
    private bool isStandingInWater = false;
    private AStarManager astar = new AStarManager();
    private MapData mapData;
    private string currentNameMap;

    private int lastHP;

    private SpriteRenderer flipSprite;
    private Animator animator;

    private void Awake()
    {
        hpBar.interactable = false;
        flipSprite = sprite.GetComponent<SpriteRenderer>();
        animator = sprite.GetComponent<Animator>();
        uiNameMob.text = $"{gameObject.name.Replace("(Clone)", "")}";
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
    public void RegisterDontDestroyOnLoad() { }
    public void OnUpdate() 
    {
        if (syncMobDataMovement == null) 
            return;

        Vector2 targetPos = new Vector2(syncMobDataMovement.posX, syncMobDataMovement.posY);
        movement = targetPos;

        ReadMap();

        if (syncMobDataMovement.currentTile == TileType.Water && astar.IsStandInWater(mapData, transform.position.x, transform.position.y))
        {
            waterShadow.SetActive(true);

            // chỉ chạy đúng 1 lần khi vừa xuống nước
            if (!isStandingInWater)
            {
                transform.position = new Vector3(transform.position.x, transform.position.y + 0.3f, transform.position.z);
                waterShadow.transform.position = new Vector3(transform.position.x, transform.position.y - 0.3f, transform.position.z);

                isStandingInWater = true;
            }
        }
        else
        {
            waterShadow.SetActive(false);

            if (isStandingInWater)
            {
                transform.position = new Vector3(transform.position.x, transform.position.y - 0.3f, transform.position.z);
                waterShadow.transform.position = new Vector3(transform.position.x, transform.position.y + 0.3f, transform.position.z);

                isStandingInWater = false;
            }
        }

        UpdateAnimation();
    }
    public void OnLateUpdate() { }
    public void OnFixedUpdate() 
    {
        if (animator.GetCurrentAnimatorStateInfo(0).IsName("Atk") || animator.GetCurrentAnimatorStateInfo(0).IsName("Injured") || animator.GetCurrentAnimatorStateInfo(0).IsName("Die"))
            return;

        flipSprite.flipX = syncMobDataMovement.direction == Direction.Left;
        transform.position = Vector2.MoveTowards(transform.position, movement, 2f * Time.fixedDeltaTime);
    }

    public void ApplyServerState(SyncMobData data)
    {
        syncMobDataMovement = data;

        if (lastHP == 0)
            lastHP = syncMobDataMovement.hp;

        if (lastHP > syncMobDataMovement.hp)
        {
            syncMobDataMovement.state = State.Injured;
            lastHP = syncMobDataMovement.hp;
        }

        hpBar.maxValue = syncMobDataMovement.maxHP;
        hpBar.value = syncMobDataMovement.hp;
    }
    private void ReadMap()
    {
        string sceneName = SceneManager.GetActiveScene().name;

        if (currentNameMap != sceneName)
        {
            currentNameMap = sceneName;
            mapData = MapView.mapFileData;
        }
    }
    private void UpdateAnimation()
    {
        switch (syncMobDataMovement.state)
        {
            case State.Stand:
                if (syncMobDataMovement.idState != lastIDStateStand)
                {
                    lastIDStateStand = syncMobDataMovement.idState;
                    animator.SetTrigger("Stand");
                }
                break;

            case State.Attack:
                if (syncMobDataMovement.idState != lastIDStateAtk)
                {
                    lastIDStateAtk = syncMobDataMovement.idState;
                    animator.SetTrigger("Atk");
                }
                break;

            case State.Injured:
                if (syncMobDataMovement.idState != lastIDStateInjured)
                {
                    lastIDStateInjured = syncMobDataMovement.idState;
                    animator.SetTrigger("Injured");
                }
                break;

            case State.Die:
                if (syncMobDataMovement.idState != lastIDStateDie)
                {
                    lastIDStateDie = syncMobDataMovement.idState;
                    animator.SetTrigger("Die");
                }

                if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.9f)
                    MobsManager.Instance.ApplyMobDead(syncMobDataMovement.id);

                break;
        }
    }

    public int GetID()
    {
        if (syncMobDataMovement == null) return 0;
        return syncMobDataMovement.id;
    }

    public string GetNameMob()
    {
        if (syncMobDataMovement == null) return "";
        return syncMobDataMovement.nameMob;
    }
}
