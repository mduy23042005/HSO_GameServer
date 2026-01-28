using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour, IUpdatable
{
    [SerializeField] private List<GameObject> playerPrefab;

    GameObject player;

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

    public void OnUpdate() 
    {
        if (player == null && LogInView.GetIDAccount() != 0)
            InitPlayer();
    }
    public void OnLateUpdate() { }
    public void OnFixedUpdate() { }
    public void RegisterDontDestroyOnLoad()
    {
        GameManager.Instance.RegisterPersistent(this);
    }

    public void InitPlayer()
    {
        int idSchool = LogInView.GetIDSchool();

        switch (idSchool)
        {
            case 1:
                player = Instantiate(playerPrefab[0], new Vector2(-9.5f, -4.5f), Quaternion.identity);
                break;
            case 2:
                player = Instantiate(playerPrefab[1], new Vector2(-9.5f, -4.5f), Quaternion.identity);
                break;
            case 3:
                player = Instantiate(playerPrefab[2], new Vector2(-9.5f, -4.5f), Quaternion.identity);
                break;
            case 4:
                player = Instantiate(playerPrefab[3], new Vector2(-9.5f, -4.5f), Quaternion.identity);
                break;
        }
    }

    public void DestroyPlayer()
    {
        if (player != null)
        {
            Destroy(player);
            player = null;
            LogInView.SetIDAccount(0);
        }
    }
}