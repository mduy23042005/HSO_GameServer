public interface IUpdatable
{
    void OnUpdate();
    void OnLateUpdate();
    void OnFixedUpdate();
    void RegisterDontDestroyOnLoad();
}