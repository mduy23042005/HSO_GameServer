using UnityEngine;

public class MovementMobController : MonoBehaviour, IUpdatable
{
    private Vector2 movement;

    private SyncMobData syncMobDataMovement;
    private int lastIDState = -1; // nhằm phân biệt các trạng thái atk/injured khác nhau khi có nhiều packet cùng loại chỉ yêu cầu thực hiện 1 trạng thái

    private SpriteRenderer flipSprite;
    private Animator animator;

    private void Awake()
    {
        flipSprite = GetComponent<SpriteRenderer>();
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
    public void OnUpdate() 
    {
        if (syncMobDataMovement == null) return;

        Vector2 targetPos = new Vector2(syncMobDataMovement.posX, syncMobDataMovement.posY);
        movement = targetPos;

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

        flipSprite.flipX = syncMobDataMovement.direction > 0;

        movement = new Vector2(syncMobDataMovement.posX, syncMobDataMovement.posY);
    }
    private void UpdateAnimation()
    {
        switch (syncMobDataMovement.state)
        {
            case "Stand":
                if (!animator.GetCurrentAnimatorStateInfo(0).IsName("Stand"))
                    animator.Play("Stand");
                break;

            case "Move":
                if (!animator.GetCurrentAnimatorStateInfo(0).IsName("Move"))
                    animator.Play("Move");
                break;

            case "Atk":
                if (syncMobDataMovement.idState != lastIDState) // 1 packet atk khác (1 đòn đánh khác)
                {
                    lastIDState = syncMobDataMovement.idState;
                    animator.Play("Atk", 0, 0f);
                }
                else // nhiều packet atk cùng loại (nhiều packet atk state cùng loại)
                {
                    if (!animator.GetCurrentAnimatorStateInfo(0).IsName("Atk") || animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f)
                    {
                        animator.Play("Stand");
                    }
                }
                break;

            case "Injured":
                if (syncMobDataMovement.idState != lastIDState)
                {
                    lastIDState = syncMobDataMovement.idState;
                    animator.Play("Injured", 0, 0f);
                }
                else
                {
                    if (!animator.GetCurrentAnimatorStateInfo(0).IsName("Injured") || animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f)
                    {
                        animator.Play("Stand");
                    }
                }
                break;
        }
    }
}
