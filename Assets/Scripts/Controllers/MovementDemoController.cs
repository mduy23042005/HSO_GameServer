public class MovementDemoController : MovementController
{
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
    public override void MoveKeyboard()
    {
        return;
    }
    public override void MoveMouse()
    {
        return;
    }

    public override void OnUpdate()
    {
        return;
    }
    public override void OnLateUpdate()
    {
        return;
    }
    public override void OnFixedUpdate()
    {
        return;
    }
}
