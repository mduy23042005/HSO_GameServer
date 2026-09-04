using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class PlayerAttackDataPacket 
{ 
    public EnumCmdCode cmd; 
    public int idAccount; 
    public int aimedMobID; 
}

public class MovementPlayerController : MonoBehaviour, IUpdatable
{
    [SerializeField] private GameObject shadow; 
    [SerializeField] private GameObject waterShadow;
    [SerializeField] private LayerMask focusLayer;

    private float moveSpeed = 6f;
    private Vector2 movement;
    private Vector2 lastMove = new Vector2(0, -1);
    private Vector2 targetPosition;
    private bool isMovingToTarget = false;
    private Animator animator;
    private MenuView menu;
    private bool isBusy = false;
    private bool isStandingInWater = false;
    private bool isDPadPressed;

    private MapView minimap;
    private RectTransform minimapUI;
    private RectTransform fullMinimapUI;
    private Camera uiCamera;
    private bool isFullMinimapOpen = false;
    private string currentNameMap;

    private Vector2 currentPosition;
    private (int x, int y) startMovementPosition;
    private (int x, int y) endMovementPosition;
    private List<(int x, int y)> path;
    private int pathIndex;
    private AStarManager astar = new AStarManager();
    private MapData mapData;

    private State currentState;
    private SocketManager socketManager;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        menu = FindAnyObjectByType<MenuView>(FindObjectsInactive.Include);
        if (waterShadow != null)
            waterShadow.SetActive(false);

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
#if UNITY_ANDROID
        TouchScreen();
        MoveDPad();
#elif UNITY_STANDALONE || UNITY_EDITOR
        LeftClick();
        RightClick();
        MoveKeyboard();
#endif
        MoveToTargetPosition();
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
                },
                scaleData = new
                {
                    x = reader.ReadFloat(),
                }
            };

            transform.position = new Vector3(callBackPacket.positionData.x, callBackPacket.positionData.y, 0);
            transform.localScale = new Vector3(callBackPacket.scaleData.x, 1, 1);
        }

        if (astar.IsStandInWater(mapData, transform.position.x, transform.position.y))
        {
            shadow.SetActive(false);
            waterShadow.SetActive(true);

            // chỉ chạy đúng 1 lần khi vừa xuống nước
            if (!isStandingInWater)
            {
                transform.position = new Vector3(transform.position.x, transform.position.y - 0.2f, transform.position.z);
                waterShadow.transform.position = new Vector3(transform.position.x, transform.position.y + 0.2f, transform.position.z);

                isStandingInWater = true;
            }
        }
        else
        {
            shadow.SetActive(true);
            waterShadow.SetActive(false);

            // chỉ chạy đúng 1 lần khi vừa lên bờ
            if (isStandingInWater)
            {
                transform.position = new Vector3(transform.position.x, transform.position.y + 0.2f, transform.position.z);
                waterShadow.transform.position = new Vector3(transform.position.x, transform.position.y - 0.2f, transform.position.z);

                isStandingInWater = false;
            }
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

            mapData = MapView.mapFileData;
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
    private void TouchScreen()
    {
        if (Input.touchCount == 0)
            return;

        Touch touch = Input.GetTouch(0);

        if (touch.phase != TouchPhase.Began)
            return;

        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject(touch.fingerId))
            return;

        Vector2 touchPos = touch.position;

        if (isFullMinimapOpen)
        {
            if (!RectTransformUtility.RectangleContainsScreenPoint(fullMinimapUI, touchPos, uiCamera))
            {
                HideFullMinimap();
                return;
            }

            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(fullMinimapUI, touchPos, uiCamera, out Vector2 localPoint))
            {
                localPoint.x += fullMinimapUI.rect.width / 2;
                localPoint.y += fullMinimapUI.rect.height / 2;

                Vector3 worldPos = minimap.MinimapToWorldPosition(localPoint);

                targetPosition = new Vector2(worldPos.x, worldPos.y);
                isMovingToTarget = true;
                path = null;
                FocusController.focusedObject = null;
                return;
            }
            return;
        }

        if (minimapUI != null && RectTransformUtility.RectangleContainsScreenPoint(minimapUI, touchPos, uiCamera))
        {
            ShowFullMinimap();
            return;
        }

        Vector3 touchWorldPos = Camera.main.ScreenToWorldPoint(touchPos);
        Collider2D hit = Physics2D.OverlapPoint(touchWorldPos, focusLayer);
        if (hit != null)
            FocusController.focusedObject = hit.gameObject;
        else
            FocusController.focusedObject = null;
    }
    public virtual void LeftClick()
    {
        if (isFullMinimapOpen)
        {
            if (Input.GetMouseButtonDown(0))
                HideFullMinimap();

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

            Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Collider2D hit = Physics2D.OverlapPoint(mouseWorldPos, focusLayer);
            if (hit != null)
                FocusController.focusedObject = hit.gameObject;
            else
                FocusController.focusedObject = null;
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
                    FocusController.focusedObject = null;
                    return;
                }
                return;
            }
        }
        if (Input.GetMouseButtonDown(1))
        {
            Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 clickPos = new Vector2(mouseWorldPos.x, mouseWorldPos.y);

            var gridPos = ToGrid(clickPos);

            if (!astar.IsWalkable(mapData, gridPos.x, gridPos.y))
                return;

            // nếu hợp lệ thì move
            targetPosition = clickPos;
            isMovingToTarget = true;
            FocusController.focusedObject = null;
        }
    }
    public virtual void MoveKeyboard()
    {
        if (isBusy)
            return;

        if (menu == null || !menu.GetIsActive())
        {
            movement.x = Input.GetAxisRaw("Horizontal");
            movement.y = Input.GetAxisRaw("Vertical");

            if (movement.x != 0)
                movement.y = 0;
            
            if (movement.y != 0)
                movement.x = 0;
            
            MoveStop();

            if (movement == Vector2.zero)
                return;

            path = null; // reset để lần sau tạo path mới
            startMovementPosition = endMovementPosition;
            isMovingToTarget = false;
            if (minimap != null)
                minimap.ClearAStarPath();

            float speed = moveSpeed * Time.deltaTime;
            transform.position += new Vector3(movement.x, movement.y, 0) * speed;
        }
    }
    public virtual void MoveDPad()
    {
        if (isBusy)
            return;

        if (!isDPadPressed)
            return;

        if (menu == null || !menu.GetIsActive())
        {
            if (movement.x != 0)
                movement.y = 0;

            if (movement.y != 0)
                movement.x = 0;

            MoveStop();

            if (movement == Vector2.zero)
                return;
            else
            {
                path = null; // reset để lần sau tạo path mới
                startMovementPosition = endMovementPosition;
                isMovingToTarget = false;
                if (minimap != null)
                    minimap.ClearAStarPath();
            }

            float speed = moveSpeed * Time.deltaTime;

            transform.position += new Vector3(movement.x, movement.y, 0) * speed;
        }
        else
        {
            return;
        }
    }
    public virtual void MoveToTargetPosition()
    {
        if ((menu != null && menu.GetIsActive()) || isBusy)
            return;

        if (isMovingToTarget)
        {
            currentPosition = transform.position;
            var targetGrid = ToGrid(targetPosition);

            if (path == null || path.Count == 0 || endMovementPosition != targetGrid)
            {
                // nếu đang di chuyển mà click chỗ khác, lấy node kế tiếp làm điểm bắt đầu để tránh giật lùi
                if (path != null && pathIndex < path.Count)
                {
                    startMovementPosition = path[pathIndex];
                }
                else
                {
                    startMovementPosition = ToGrid(currentPosition);
                }

                var newPath = astar.FindPath(mapData, startMovementPosition.x, startMovementPosition.y, targetGrid.x, targetGrid.y);

                if (newPath != null && newPath.Count > 0)
                {
                    endMovementPosition = targetGrid;
                    path = newPath;
                    pathIndex = 0;

                    if (minimap != null)
                        minimap.DrawAStarPath(path);
                }
                else if (path == null)
                {
                    isMovingToTarget = false;
                    return;
                }
            }

            // nếu đã đi hết path thì dừng lại
            if (pathIndex >= path.Count)
            {
                path = null;
                startMovementPosition = endMovementPosition;
                isMovingToTarget = false;
                if (minimap != null)
                    minimap.ClearAStarPath();
                movement = Vector2.zero;
                return;
            }

            var node = path[pathIndex];
            Vector2 targetNode = new Vector2(node.x + 0.5f, node.y + 0.5f);
            Vector2 directionToTarget = targetNode - currentPosition;
            float distanceToTarget = directionToTarget.magnitude;

            if (distanceToTarget > 0.01f)
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

                // di chuyển trực tiếp thay vì đợi frame sau
                if (speed > distanceToTarget)
                {
                    transform.position = targetNode;
                    currentPosition = targetNode;
                }
                else
                {
                    transform.position = currentPosition + movement * speed;
                    currentPosition = transform.position;
                }

                MoveStop();

                // xử lý khi click vào quái
                if (FocusController.focusedObject != null && FocusController.focusedObject.GetComponent<MobController>() != null)
                {
                    targetPosition = new Vector2(FocusController.focusedObject.transform.position.x, FocusController.focusedObject.transform.position.y);

                    var mobGrid = ToGrid(targetPosition);

                    if (Vector2.Distance(transform.position, targetPosition) <= 1.5f)
                    {
                        path = null;
                        isMovingToTarget = false;
                        if (minimap != null)
                            minimap.ClearAStarPath();
                        movement = Vector2.zero;

                        UpdateAtkAnimation();
                        return;
                    }

                    var newMobGrid = ToGrid(targetPosition);
                    if (newMobGrid != endMovementPosition)
                    {
                        path = null;
                    }
                }
            }

            // kiểm tra chuyển đổi Node
            if (Vector2.Distance(currentPosition, targetNode) <= 0.01f)
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

    public void SetMovement(float x, float y)
    {
        movement = new Vector2(x, y);

        isDPadPressed = x != 0 || y != 0;
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

    public State GetCurrentState()
    {
        return currentState;
    }

    public TileType GetCurrentTileType()
    {
        return astar.GetTileType(mapData, transform.position.x, transform.position.y);
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
        isBusy = true;
        animator.SetBool("isMove", false);
        animator.SetTrigger(anim);
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
            return;
        if (movement.x == 0 && movement.y == 0)
        {
            currentState = State.Stand;
            animator.SetBool("isMove", false);
            UpdateLastMoveToAnimator();
        }
        if (movement.x != 0 || movement.y != 0)
        {
            currentState = State.Move;
            animator.SetBool("isMove", true);
            UpdateMoveToAnimator();
        }
        if (Input.GetKeyDown(KeyCode.J))
        {
            ChaseAndAttack();
        }
    }
    public void ChaseAndAttack()
    {
        if (FocusController.focusedObject == null)
            return;

        var gridPos = ToGrid(FocusController.focusedObject.transform.position);

        if (!astar.IsWalkable(mapData, gridPos.x, gridPos.y))
            return;

        if (FocusController.focusedObject != null && FocusController.focusedObject.GetComponent<MobController>() != null)
        {
            targetPosition = new Vector2(FocusController.focusedObject.transform.position.x, FocusController.focusedObject.transform.position.y);

            var mobGrid = ToGrid(targetPosition);

            if (Vector2.Distance(transform.position, targetPosition) <= 1.5f)
            {
                UpdateAtkAnimation();
                return;
            }
            else
            {
                targetPosition = FocusController.focusedObject.transform.position;
                isMovingToTarget = true;
                path = null;
            }
        }
    }
    public void UpdateAtkAnimation()
    {
        currentState = State.Attack;
        TriggerAnimation("Atk", 0.5f);

        PacketWriterManager writer = new PacketWriterManager();
        writer.WriteInt((int)(EnumCmdCode)EnumCmdCode.playerAttackMob); // sau này sẽ quy ước chung thành atk để có thể attack cả other player và mob
        writer.WriteInt(LogInView.GetIDAccount() ?? 0);
        writer.WriteInt(FocusController.focusedObject.GetComponent<MobController>().GetID());

        _ = socketManager.SendToServer(writer.ToArray());
    }
    public void UpdateInjuredAnimation()
    {
        currentState = State.Injured;
        UpdateLastMoveToAnimator();
    }
    public void UpdateDieAnimation()
    {
        currentState = State.Die;
        animator.SetBool("isDie", true);
        UpdateLastMoveToAnimator();
    }
}