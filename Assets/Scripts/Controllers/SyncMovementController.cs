using UnityEngine;

public class SyncMovementController : MonoBehaviour, IUpdatable
{
    private float moveSpeed = 6f;
    private Vector2 movement;
    private Animator animator;

    private string nextAnim;
    private string direction;

    private SyncDataPacket syncDataMovement;
    private PlayerState otherPlayerState = 0;
    private Direction syncDirection = 0;
    private Vector2 serverDir = Vector2.down;

    private void Awake()
    {
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

    public void ApplyServerState(SyncDataPacket data)
    {
        syncDataMovement = data;

        // Lưu hướng từ server
        serverDir = new Vector2(data.posX - transform.position.x, data.posY - transform.position.y);

        if (serverDir.sqrMagnitude > 0.001f)
            serverDir.Normalize();

        // Giả định server có gửi state (bạn sẽ thêm bên SyncModels)
        otherPlayerState = data.state;
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
        transform.position = Vector2.MoveTowards(transform.position, movement, moveSpeed * Time.fixedDeltaTime);
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

        switch (otherPlayerState)
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

        if (!animator.GetCurrentAnimatorStateInfo(0).IsName(nextAnim) || animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f)
        {
            animator.Play(nextAnim);
        }
    }
}
