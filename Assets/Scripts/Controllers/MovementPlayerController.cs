using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;

public class MovementPlayerController : MonoBehaviour, IUpdatable
{
    private float moveSpeed = 6f;
    private Vector2 movement;
    private Vector2 lastMove = new Vector2(0, -1);
    private Vector2 targetPosition;
    private bool isMovingToTarget = false;
    private Animator animator;
    private MenuView menu;
    private bool isBusy = false;

    private MapView minimap;
    private RectTransform minimapUI;
    private RectTransform fullMinimapUI;
    private Camera uiCamera;
    private bool isFullMinimapOpen = false;
    private string currentNameMap;

    private (int x, int y) startPosition;
    private (int x, int y) endMovementPosition;
    private MovementMobController mob;
    private List<(int x, int y)> path;
    private int pathIndex;
    private AStarManager astar = new AStarManager();
    private MapData mapData;

    private PlayerState currentState;
    private SocketManager socketManager;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        menu = FindAnyObjectByType<MenuView>(FindObjectsInactive.Include);

        socketManager = GameManager.Instance.GetComponent<SocketManager>();
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
        InitMinimap();
        LeftClick();
        MoveKeyboard();
        MoveMouse();
        UpdateAnimation();

        byte[] data = socketManager.GetSyncCallBackData();
        if (data != null && data.Length > 0)
        {
            PacketReaderManager reader = new PacketReaderManager(data);
            var callBackPacket = new
            {
                cmd = (EnumCmdCode)reader.ReadInt(),
                positionData = new
                {
                    x = reader.ReadFloat(),
                    y = reader.ReadFloat(),
                    z = reader.ReadFloat()
                },
                scaleData = new
                {
                    x = reader.ReadFloat(),
                    y = reader.ReadFloat(),
                    z = reader.ReadFloat()
                }
            };

            transform.position = new Vector3(callBackPacket.positionData.x, callBackPacket.positionData.y, callBackPacket.positionData.z);
            transform.localScale = new Vector3(callBackPacket.scaleData.x, callBackPacket.scaleData.y, callBackPacket.scaleData.z);
        }

    }
    public virtual void OnLateUpdate() { }
    public virtual void OnFixedUpdate() { }

    public void RegisterDontDestroyOnLoad()
    {
        GameManager.Instance.RegisterPersistent(this);
    }

    private void InitMinimap()
    {
        if (currentNameMap != SceneManager.GetActiveScene().name)
        {
            minimap = GameObject.Find("Grid").GetComponent<MapView>();
            minimapUI = GameObject.Find("MinimapUI").GetComponent<RectTransform>();
            fullMinimapUI = Resources.FindObjectsOfTypeAll<RectTransform>().FirstOrDefault(t => t.name == "FullMinimapUI");
            uiCamera = GameObject.Find("Canvas").GetComponent<Canvas>().worldCamera;
            currentNameMap = SceneManager.GetActiveScene().name;

            string pathMapFile = Path.Combine(Application.streamingAssetsPath, $"Maps/{SceneManager.GetActiveScene().name}.bin");
            mapData = new MapData();

            if (!File.Exists(pathMapFile))
                return;

            using (BinaryReader reader = new BinaryReader(File.Open(pathMapFile, FileMode.Open)))
            {
                mapData.width = reader.ReadInt32();
                mapData.height = reader.ReadInt32();

                mapData.offsetX = reader.ReadInt32();
                mapData.offsetY = reader.ReadInt32();

                mapData.tiles = new byte[mapData.width, mapData.height];

                for (int y = 0; y < mapData.height; y++)
                {
                    for (int x = 0; x < mapData.width; x++)
                    {
                        mapData.tiles[x, y] = reader.ReadByte();
                    }
                }
            }
        }
    }
    private void ShowFullMinimap()
    {
        fullMinimapUI.gameObject.SetActive(true);
        isFullMinimapOpen = true;
    }
    private void HideFullMinimap()
    {
        fullMinimapUI.gameObject.SetActive(false);
        isFullMinimapOpen = false;
    }
    public virtual void LeftClick()
    {
        if (isFullMinimapOpen)
        {
            if (Input.GetMouseButtonDown(0))
            {
                HideFullMinimap();
                return;
            }
            return;
        }

        if (Input.GetMouseButtonDown(0))
        {
            if (RectTransformUtility.RectangleContainsScreenPoint(minimapUI, Input.mousePosition, uiCamera))
            {
                if (!isFullMinimapOpen)
                {
                    ShowFullMinimap();
                    return;
                }
            }
        }
    }
    public virtual void RightClick()
    {
        if (isFullMinimapOpen)
        {
            if (Input.GetMouseButtonDown(1))
            {
                if (!RectTransformUtility.RectangleContainsScreenPoint(fullMinimapUI, Input.mousePosition, uiCamera))
                {
                    HideFullMinimap();
                    return;
                }
                if (RectTransformUtility.ScreenPointToLocalPointInRectangle(fullMinimapUI, Input.mousePosition, uiCamera, out var localPoint))
                {
                    localPoint.x += fullMinimapUI.rect.width / 2;
                    localPoint.y += fullMinimapUI.rect.height / 2;

                    Vector3 worldPos = minimap.MinimapToWorldPosition(localPoint);

                    targetPosition = new Vector2(worldPos.x, worldPos.y);
                    isMovingToTarget = true;
                    path = null;
                    mob = null;
                    return;
                }
                return;
            }
        }
        if (Input.GetMouseButtonDown(1))
        {
            Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 clickPos = new Vector2(mouseWorldPos.x, mouseWorldPos.y);
            RaycastHit2D hit = Physics2D.GetRayIntersection(Camera.main.ScreenPointToRay(Input.mousePosition));
            if (hit.collider.CompareTag("Mob"))
            {
                mob = hit.collider.GetComponent<MovementMobController>();

                if (mob != null)
                {
                    int id = mob.GetID();
                    string nameMob = mob.GetNameMob();

                    Debug.Log($"Clicked mob [{id}] {nameMob}");

                    targetPosition = new Vector2(mob.transform.position.x, mob.transform.position.y);
                    isMovingToTarget = true;
                    return;
                }
            }

            var gridPos = ToGrid(clickPos);

            if (!astar.IsWalkable(mapData, gridPos.x, gridPos.y))
            {
                return;
            }

            // nếu hợp lệ thì move
            targetPosition = clickPos;
            isMovingToTarget = true;
            path = null;
            mob = null;
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

            if (movement == Vector2.zero)
                return;
            else
            {
                path = null; // reset để lần sau tạo path mới
                startPosition = endMovementPosition;
                isMovingToTarget = false;
                if (minimap != null)
                    minimap.ClearAStarPath();
                mob = null;
            }

            float speed = moveSpeed * Time.deltaTime;

            transform.position += new Vector3(movement.x, movement.y, 0) * speed;
        }
        else
        {
            return;
        }
    }
    public virtual void MoveMouse()
    {
        if ((menu != null && menu.GetIsActive()) || isBusy)     
            return;

        RightClick();

        if (isMovingToTarget)
        {
            if (mob != null)
            {
                targetPosition = new Vector2(mob.transform.position.x, mob.transform.position.y);

                if (Vector2.Distance(transform.position, targetPosition) <= 0.5f)
                {
                    path = null;
                    isMovingToTarget = false;
                    if (minimap != null)
                        minimap.ClearAStarPath();
                    movement = lastMove;
                    return;
                }

                // kiểm tra xem mob có đổi vị trí không (tính theo grid)
                var newMobGrid = ToGrid(targetPosition);

                if (newMobGrid != endMovementPosition) // nếu mà đã đổi vị trí thì ép FindPath() của A* chạy lại
                {
                    endMovementPosition = newMobGrid;
                    path = null;
                }
            }

            // chưa có path thì tạo mới
            if (path == null || path.Count == 0)
            {
                startPosition = ToGrid(transform.position);
                endMovementPosition = ToGrid(targetPosition);

                if (!astar.IsWalkable(mapData, endMovementPosition.x, endMovementPosition.y))
                    return;

                path = astar.FindPath(mapData, startPosition.x, startPosition.y, endMovementPosition.x, endMovementPosition.y);

                if (path == null || path.Count == 0)
                    return;

                if (minimap != null)
                    minimap.DrawAStarPath(path);
                pathIndex = 0;
            }

            // nếu đi hết path thì nghỉ
            if (pathIndex >= path.Count)
            {
                path = null; // reset để lần sau tạo path mới
                startPosition = endMovementPosition;
                isMovingToTarget = false;
                if (minimap != null)
                    minimap.ClearAStarPath();
                movement = lastMove;
                return;
            }

            // lấy node tiếp theo
            var node = path[pathIndex];
            Vector2 targetNode = new Vector2(node.x + 0.5f, node.y + 0.5f);
            Vector2 currentPosition = transform.position;
            Vector2 directionToTarget = targetNode - currentPosition; // vector hướng tới node tiếp theo
            float distanceToTarget = directionToTarget.magnitude;

            if (distanceToTarget > 0.05f)
            {
                if (Mathf.Abs(directionToTarget.x) > Mathf.Abs(directionToTarget.y))
                {
                    movement.x = directionToTarget.x > 0 ? 1 : -1;
                    movement.y = 0;
                }
                else
                {
                    movement.y = directionToTarget.y > 0 ? 1 : -1;
                    movement.x = 0;
                }

                float speed = moveSpeed * Time.deltaTime;
                Vector2 nextPosition = currentPosition + movement * speed;

                if (astar.IsWalkable(mapData, targetNode.x, targetNode.y))
                {
                    transform.position = nextPosition;
                    MoveStop();
                }
                else
                {
                    path = null; // reset để lần sau tạo path mới
                    startPosition = endMovementPosition;
                    isMovingToTarget = false;
                    if (minimap != null)
                        minimap.ClearAStarPath();
                    movement = lastMove;
                    return;
                }
            }

            // kiểm tra chuyển Node
            if (Vector2.Distance(currentPosition, targetNode) < 0.1f)
            {
                if (minimap != null)
                    minimap.ClearAStarNodeMarker(pathIndex);
                pathIndex++;
            }
        }
    }
    private (int x, int y) ToGrid(Vector2 pos)
    {
        return ((int)Math.Floor(pos.x), (int)Math.Floor(pos.y));
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
    }
    public void UpdateInjuredAnimation()
    {
        currentState = PlayerState.Injured;
        TriggerAnimation("Injured", 0.3f);
        UpdateLastMoveToAnimator();
    }
    public void UpdateDieAnimation()
    {
        currentState = PlayerState.Die;
        animator.SetBool("isDie", true);
        UpdateLastMoveToAnimator();
    }
}
