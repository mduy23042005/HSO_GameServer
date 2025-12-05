using UnityEditor;
using UnityEngine;

public class PlayerDemoController : Controller
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
    protected override void MoveKeyboard()
    {
        return;
    }
    protected override void MoveMouse()
    {
        return;
    }

    public override void OnUpdate()
    {
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
