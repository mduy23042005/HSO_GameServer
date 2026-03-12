using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

public class MovementController : MonoBehaviour, IUpdatable
{
    private float moveSpeed = 6f;
    private Vector2 movement;
    private float stateStartTime;
    private Vector2 lastMove = new Vector2(0, -1);
    private Vector2 targetPosition;
    private bool movingHorizontalFirst = false;
    private bool isMovingToTarget = false;
    private Animator animator;
    private MenuView menu;
    private bool isBusy = false;

    private PlayerState lastState;
    private PlayerState currentState;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        menu = FindAnyObjectByType<MenuView>(FindObjectsInactive.Include);
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
    }
    public virtual void OnLateUpdate() { }
    public virtual void OnFixedUpdate()
    {
        if (movement == Vector2.zero)
            return;

        float speed = moveSpeed * Time.fixedDeltaTime;

        transform.position += new Vector3(movement.x, movement.y, 0) * speed;
    }

    public void RegisterDontDestroyOnLoad()
    {
        GameManager.Instance.RegisterPersistent(this);
    }

    public virtual void LeftClick()
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
            float deltaX = Mathf.Abs(targetPosition.x - transform.position.x);
            float deltaY = Mathf.Abs(targetPosition.y - transform.position.y);
            movingHorizontalFirst = deltaX > deltaY;
            isMovingToTarget = true;
        }
    }
    public virtual void MoveKeyboard()
    {
        if (isBusy)
        {
            return;
        }
        if (menu == null || !menu.GetIsActive())
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
    public virtual void MoveMouse()
    {
        if ((menu != null && menu.GetIsActive()) || EventSystem.current.IsPointerOverGameObject() || isBusy)
        {
            return;
        }
        LeftClick();
        if (isMovingToTarget)
        {
            Vector2 currentPos = transform.position;
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

    public bool GetIsMovingToTarget()
    {
        return isMovingToTarget;
    }

    public PlayerState GetCurrentState()
    {
        return currentState;
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

    public virtual void UpdateAnimation()
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
