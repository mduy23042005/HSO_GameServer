using System.IO;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MobController : MonoBehaviour, IUpdatable
{
    [SerializeField] private TMP_Text uiNameMob;
    [SerializeField] private Slider hpBar;
    [SerializeField] private GameObject sprite;

    [SerializeField] private GameObject shadow;
    [SerializeField] private GameObject waterShadow;

    private Vector2 movement;

    private SyncMobData syncMobDataMovement;
    private int lastIDState = -1; // nhằm phân biệt các trạng thái atk/injured khác nhau khi có nhiều packet cùng loại chỉ yêu cầu thực hiện 1 trạng thái
    private bool isStandingInWater = false;
    private AStarManager astar = new AStarManager();
    private MapData mapData;
    private string currentNameMap;

    private int lastHP;
    private bool isInjured;

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
        if (syncMobDataMovement == null) return;
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
            isInjured = true;
            lastHP = syncMobDataMovement.hp;
        }

        flipSprite.flipX = syncMobDataMovement.direction == Direction.Left;

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
        if (isInjured)
            return;

        switch (syncMobDataMovement.state)
        {
            case State.Stand:
                if (!animator.GetCurrentAnimatorStateInfo(0).IsName("Stand"))
                    animator.Play("Stand");
                break;

            case State.Attack:
                if (syncMobDataMovement.idState != lastIDState) // 1 packet atk khác (1 đòn đánh khác)
                {
                    lastIDState = syncMobDataMovement.idState;
                    animator.Play("Atk");
                }
                else // nhiều packet atk cùng loại (nhiều packet atk state cùng loại)
                {
                    if (!animator.GetCurrentAnimatorStateInfo(0).IsName("Atk") || animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f)
                        animator.Play("Stand");
                }
                break;

            case State.Injured:
                if (syncMobDataMovement.idState != lastIDState)
                {
                    lastIDState = syncMobDataMovement.idState;
                    animator.Play("Die");
                }
                else // nhiều packet atk cùng loại (nhiều packet atk state cùng loại)
                {
                    if (!animator.GetCurrentAnimatorStateInfo(0).IsName("Die") || animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f)
                    {
                        animator.Play("Stand");
                        isInjured = false;
                    }
                }
                break;

            case State.Die:
                if (!animator.GetCurrentAnimatorStateInfo(0).IsName("Die"))            
                    animator.Play("Die");           
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
