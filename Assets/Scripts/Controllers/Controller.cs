using Newtonsoft.Json;
using System.Collections;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;

public class Controller : MonoBehaviour, IUpdatable
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
    private SpriteController spriteController;

    private float syncInterval = 0.05f;
    private float syncTimer = 0f;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        menu = FindAnyObjectByType<MenuController>(FindObjectsInactive.Include);
        spriteController = GetComponent<SpriteController>();
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
            syncTimer = 0;
            _ = SendSyncData();
        }
    }

    public virtual void OnLateUpdate() { }

    public virtual void OnFixedUpdate()
    {
        rb.MovePosition(rb.position + movement * moveSpeed * Time.fixedDeltaTime);
    }

    public void RegisterDontDestroyOnLoad()
    {
        GameManager.Instance.RegisterPersistent(this);
    }

    private async Task SendSyncData()
    {
        // Tạo object sync
        SyncModels data = new SyncModels();

        // Gán đúng data Movement của bạn
        data.posX = transform.position.x;
        data.posY = transform.position.y;
        data.lastPosX = lastMove.x;
        data.lastPosY = lastMove.y;

        data.weapon = spriteController.GetWeaponData();
        data.helmet = spriteController.GetHelmetData();
        data.armor = spriteController.GetArmorData();
        data.legArmor = spriteController.GetLegArmorData();
        data.hair = spriteController.GetHairData();

        data.idAccount = LogInController.GetIDAccount() ?? 0;
        data.school = LogInController.GetIDSchool();

        string json = JsonConvert.SerializeObject(data);
        await SocketManager.Instance.SendSyncDataToServer(json);
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
            animator.SetBool("isMove", false);
            UpdateLastMoveToAnimator();
        }
        if (movement.x != 0 || movement.y != 0)
        {
            animator.SetBool("isMove", true);
            UpdateMoveToAnimator();
        }
        if (Input.GetKeyDown(KeyCode.J))
        {
            TriggerAnimation("Atk", 0.25f);
            UpdateLastMoveToAnimator();
        }
        if (Input.GetKeyDown(KeyCode.K))
        {
            TriggerAnimation("Injured", 0.3f);
            UpdateLastMoveToAnimator();
        }
    }
}
