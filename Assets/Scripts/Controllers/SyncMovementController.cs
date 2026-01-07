using UnityEngine;

public class SyncMovementController : MonoBehaviour, IUpdatable
{
    private float moveSpeed = 6f;
    private Vector2 movement;
    private Rigidbody2D rb;
    private Animator animator;

    private string nextAnim = "";
    private string direction;

    private SyncModels syncDataMovement;
    private PlayerState serverState = 0;
    private Direction syncDirection = 0;
    private Vector2 serverDir = Vector2.down;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.interpolation = RigidbodyInterpolation2D.None;
        animator = GetComponentInChildren<Animator>();
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

    public void ApplyServerState(SyncModels data)
    {
        syncDataMovement = data;

        // Lưu hướng từ server
        serverDir = new Vector2(data.posX - transform.position.x, data.posY - transform.position.y);

        if (serverDir.sqrMagnitude > 0.001f)
            serverDir.Normalize();

        // Giả định server có gửi state (bạn sẽ thêm bên SyncModels)
        serverState = data.state;
    }

    public void OnUpdate()
    {
        if (syncDataMovement == null) return;

        Vector2 targetPos = new Vector2(syncDataMovement.posX, syncDataMovement.posY);
        movement = targetPos;

        syncDirection = (Direction)syncDataMovement.direction;
        UpdateAnimation();
    }

    public void OnLateUpdate() { }
    public void OnFixedUpdate()
    {
        float speed = moveSpeed * Time.fixedDeltaTime;

        Vector2 newPos = Vector2.MoveTowards(rb.position, movement, speed);

        rb.MovePosition(newPos);
    }
    public void RegisterDontDestroyOnLoad()
    {
        GameManager.Instance.RegisterPersistent(this);
    }

    private void UpdateAnimation()
    {
        switch (syncDirection)
        {
            case Direction.Front:
                direction = "Front"; break;

            case Direction.Back:
                direction = "Back"; break;

            case Direction.Left:
                direction = "Left"; break;

            case Direction.Right:
                direction = "Right"; break;

            default:
                direction = "Front"; break;
        }
        switch (serverState)
        {
            case PlayerState.Stand:
                nextAnim = $"Stand{direction}";
                break;

            case PlayerState.Move:
                nextAnim = $"Move{direction}";
                break;

            case PlayerState.Attack:
                nextAnim = $"Atk{direction}";
                break;

            case PlayerState.Injured:
                nextAnim = $"Injured{direction}";
                break;
        }

        if (!animator.GetCurrentAnimatorStateInfo(0).IsName(nextAnim))
        {
            animator.Play(nextAnim);
        }
    }
}
