    using UnityEngine;

public class MobsController : MonoBehaviour, IUpdatable
{
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private BoxCollider2D moveArea;
    [SerializeField] private BoxCollider2D attackArea;

    private Vector2 targetPos;
    private float waitAfterMove = 0f; // thời gian chờ sau khi di chuyển xong
    private SpriteRenderer flipSprite;
    private float changeTargetCooldown = 0f;
    private Animator animator;
    private bool isAttacking = false;

    private void Awake()
    {
        flipSprite = GetComponent<SpriteRenderer>();
        targetPos = GetRandomPosition();
        animator = GetComponent<Animator>();
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
    public void OnUpdate(){ }
    public void OnLateUpdate() { }
    public void OnFixedUpdate()
    {
        Move();
    }

    private Vector2 GetRandomPosition()
    {
        if (moveArea == null)
        {
            Debug.LogWarning($"{name} chưa có vùng moveArea!");
            return transform.position;
        }

        Bounds b = moveArea.bounds;
        float randomX = Random.Range(b.min.x, b.max.x);
        float randomY = Random.Range(b.min.y, b.max.y);
        return new Vector2(randomX, randomY);
    }
    private void Move()
    {
        if (isAttacking)
            return; // đang tấn công, đứng yên hoàn toàn

        // Nếu đang trong thời gian chờ
        if (waitAfterMove > 0f)
        {
            waitAfterMove -= Time.fixedDeltaTime;
            return; // vẫn chờ, không di chuyển
        }

        // Kiểm tra nếu đã tới target hoặc cooldown hết
        if (Vector2.Distance(transform.position, targetPos) < 0.1f || changeTargetCooldown <= 0f)
        {
            targetPos = GetRandomPosition();
            changeTargetCooldown = Random.Range(1f, 3f);

            // Bắt đầu thời gian chờ 1s trước khi di chuyển
            waitAfterMove = Random.Range(0.3f, 1f);
            return; // tạm dừng 1 frame, sẽ tiếp tục di chuyển sau 1s
        }
        else
        {
            changeTargetCooldown -= Time.fixedDeltaTime;
        }

        // Di chuyển mob
        Vector2 newPos = Vector2.MoveTowards(transform.position, targetPos, moveSpeed * Time.fixedDeltaTime);
        transform.position = newPos;

        // Lật sprite theo hướng di chuyển
        if (flipSprite != null && !isAttacking)
            flipSprite.flipX = (targetPos.x > transform.position.x);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Kiểm tra đúng collider attackArea
        if (other == attackArea) return; // parent collider không dùng, chỉ xét player
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        // Kiểm tra player và collider attackArea
        if (other.CompareTag("Player") && attackArea != null && attackArea.bounds.Contains(other.transform.position))
        {
            Vector2 dir = other.transform.position - transform.position;

            if (flipSprite != null && !isAttacking)
                flipSprite.flipX = (dir.x > 0);

            if (Vector2.Distance(transform.position, other.transform.position) > 1.5f)
            {
                Vector2 newPos = Vector2.MoveTowards(transform.position, other.transform.position, moveSpeed * Time.fixedDeltaTime);
                transform.position = newPos;
                animator.ResetTrigger("Atk");
            }
            else
            {
                isAttacking = true;
                targetPos = transform.position;
                animator.SetTrigger("Atk");
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isAttacking = false;
            animator.ResetTrigger("Atk");
            waitAfterMove = 0.5f;
            targetPos = GetRandomPosition();
        }
    }
}
