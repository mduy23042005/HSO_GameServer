using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class RenderPart
{
    public SpriteRenderer renderer;
    public int offset;
}
public class YSortRenderController : MonoBehaviour, IUpdatable
{
    [SerializeField] private List<RenderPart> partsBody;

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

    public void OnUpdate() { }
    public void OnFixedUpdate() { }
    public void OnLateUpdate()
    {
        int baseOrder = Mathf.RoundToInt(-transform.position.y * 100);

        foreach (var part in partsBody)
        {
            if (part.renderer == null) continue;
            part.renderer.sortingOrder = baseOrder + part.offset;
        }
    }

    public void RegisterDontDestroyOnLoad(){ }
}
