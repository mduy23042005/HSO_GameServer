using Newtonsoft.Json;
using System.Collections;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;

public class MovementController : MonoBehaviour, IUpdatable
{
    private float moveSpeed = 6f;
    private Rigidbody2D rb;
    private Vector2 movement;
    private Vector2 lastMove = new Vector2(0, -1);
    private Vector2 targetPosition;
    private bool movingHorizontalFirst = false;
    private bool isMovingToTarget = false;
    private Animator animator;
    private MenuController menu;
    private bool isBusy = false;
    private float syncTimer;
    private const float syncInterval = 0.05f; // 20 lần / giây

    private SocketManager socketManager;
    private SpriteController spriteController;
    private PlayerState currentState;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        menu = FindAnyObjectByType<MenuController>(FindObjectsInactive.Include);
        spriteController = GetComponent<SpriteController>();
        socketManager = GameManager.Instance.GetComponent<SocketManager>();
    }

    private void OnEnable()
    {
        GameManager.Instance.Register(this);
        animator.SetFloat("LastHorizontal", 0);
        animator.SetFloat("LastVertical", -1);
    }

    private void OnDisable()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.Unregister(this);
        }
    }

    public virtual void OnUpdate()
    {
        MoveKeyboard();
        MoveMouse();
        UpdateAnimation();
        syncTimer += Time.deltaTime;

        if (syncTimer >= syncInterval)
        {
            syncTimer = 0f;
            SendSyncData();
        }
    }

    public virtual void OnLateUpdate() { }

    public virtual void OnFixedUpdate()
    {
        float speed = moveSpeed * Time.fixedDeltaTime;

        if (movement == Vector2.zero)
            return;

        Vector2 targetPos = rb.position + movement;

        Vector2 newPos = Vector2.MoveTowards(rb.position, targetPos, speed);

        rb.MovePosition(newPos);
    }

    public void RegisterDontDestroyOnLoad()
    {
        GameManager.Instance.RegisterPersistent(this);
    }

    private async Task SendSyncData()
    {
        SyncModels packet = new SyncModels
        {
            cmd = "syncData",
            idAccount = LogInView.GetIDAccount() ?? 0,
            idSchool = LogInView.GetIDSchool(),
            posX = transform.position.x,
            posY = transform.position.y,
            lastPosX = lastMove.x,
            lastPosY = lastMove.y,
            state = currentState,
            direction = spriteController.GetCurrentDirection(),
            frame = spriteController.GetCurrentFrame(),

            hair = spriteController.GetHairData(),
            weapon = spriteController.GetWeaponData(),
            helmet = spriteController.GetHelmetData(),
            armor = spriteController.GetArmorData(),
            legArmor = spriteController.GetLegArmorData(),
        };
        string sendSyncDataPacket = JsonConvert.SerializeObject(packet);
        socketManager.SendToServer(sendSyncDataPacket);
    }

    protected virtual void LeftClick()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 clickPos = new Vector2(mouseWorldPos.x, mouseWorldPos.y);
            RaycastHit2D hit = Physics2D.Raycast(clickPos, Vector2.zero);
            if (hit.collider != null)
            {
                //Nếu collider có tag "Mob" thì chỉ debug, không di chuyển
                if (hit.collider.CompareTag("Mob"))
                {
                    Debug.Log($"Clicked on Mob: {hit.collider.name}");
                    isMovingToTarget = false;
                    return;
                }
            }
            // Nếu không click vào mob thì tiến hành di chuyển đến vị trí click
            targetPosition = clickPos;
            // Quyết định hướng ưu tiên
            float deltaX = Mathf.Abs(targetPosition.x - rb.position.x);
            float deltaY = Mathf.Abs(targetPosition.y - rb.position.y);
            movingHorizontalFirst = deltaX > deltaY;
            isMovingToTarget = true;
        }
    }
    protected virtual void MoveKeyboard()
    {
        if (isBusy)
        {
            return;
        }
        if (menu == null || !menu.getIsActive())
        {
            movement.x = Input.GetAxisRaw("Horizontal");
            movement.y = Input.GetAxisRaw("Vertical");
            if (movement.x != 0)
            {
                movement.y = 0;
            }
            if (movement.y != 0)
            {
                movement.x = 0;
            }
            MoveStop();
        }
        else
        {
            return;
        }
    }
    protected virtual void MoveMouse()
    {
        if ((menu != null && menu.getIsActive()) || EventSystem.current.IsPointerOverGameObject() || isBusy)
        {
            return;
        }
        LeftClick();
        if (isMovingToTarget)
        {
            Vector2 currentPos = rb.position;
            float deltaX = targetPosition.x - currentPos.x;
            float deltaY = targetPosition.y - currentPos.y;

            if (Mathf.Abs(deltaX) > 0.1f || Mathf.Abs(deltaY) > 0.1f)
            {
                if (movingHorizontalFirst)
                {
                    if (Mathf.Abs(deltaX) > 0.1f)
                    {
                        movement = new Vector2(Mathf.Sign(deltaX), 0);
                    }
                    else
                    {
                        movement = new Vector2(0, Mathf.Sign(deltaY));
                    }
                }
                else
                {
                    if (Mathf.Abs(deltaY) > 0.1f)
                    {
                        movement = new Vector2(0, Mathf.Sign(deltaY));
                    }
                    else
                    {
                        movement = new Vector2(Mathf.Sign(deltaX), 0);
                    }
                }
                MoveStop();
            }
            else
            {
                isMovingToTarget = false;
                return;
            }
        }
    }

    private void MoveStop()
    {
        if (movement != Vector2.zero)
        {
            lastMove = movement;
        }
    }

    public Vector2 GetMovement()
    {
        return movement;
    }
    public Vector2 GetLastMovement()
    {
        return lastMove;
    }
    public void SetMovement(Vector2 value)
    {
        movement = value;
    }
    public void SetLastMovement(Vector2 value)
    {
        lastMove = value;
    }
    public bool GetIsMovingToTarget()
    {
        return isMovingToTarget;
    }

    private void UpdateMoveToAnimator()
    {
        animator.SetFloat("Horizontal", movement.x);
        animator.SetFloat("Vertical", movement.y);
    }
    private void UpdateLastMoveToAnimator()
    {
        animator.SetFloat("LastHorizontal", lastMove.x);
        animator.SetFloat("LastVertical", lastMove.y);
    }

    private void TriggerAnimation(string anim, float duration)
    {
        animator.SetTrigger(anim);
        isBusy = true;
        UpdateLastMoveToAnimator();
        StartCoroutine(ResetBusy(duration));
    }
    private IEnumerator ResetBusy(float duration)
    {
        yield return new WaitForSeconds(duration);
        isBusy = false;
    }

    protected virtual void UpdateAnimation()
    {
        if (isBusy)
        {
            return;
        }
        if (movement.x == 0 && movement.y == 0)
        {
            currentState = PlayerState.Stand;
            animator.SetBool("isMove", false);
            UpdateLastMoveToAnimator();
        }
        if (movement.x != 0 || movement.y != 0)
        {
            currentState = PlayerState.Move;
            animator.SetBool("isMove", true);
            UpdateMoveToAnimator();
        }
        if (Input.GetKeyDown(KeyCode.J))
        {
            currentState = PlayerState.Attack;
            TriggerAnimation("Atk", 0.25f);
            UpdateLastMoveToAnimator();
        }
        if (Input.GetKeyDown(KeyCode.K))
        {
            currentState = PlayerState.Injured;
            TriggerAnimation("Injured", 0.3f);
            UpdateLastMoveToAnimator();
        }
    }
}
