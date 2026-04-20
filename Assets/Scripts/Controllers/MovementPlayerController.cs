using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class MovementController : MonoBehaviour, IUpdatable
{
    private float moveSpeed = 6f;
    private Vector2 movement;
    private Vector2 lastMove = new Vector2(0, -1);
    private Vector2 targetPosition;
    private bool movingHorizontalFirst = false;
    private bool isMovingToTarget = false;
    private Animator animator;
    private MenuView menu;
    private bool isBusy = false;

    private (int x, int y) startPosition;
    private (int x, int y) endMovementPosition;
    private (int x, int y) endAttackPosition;
    private List<(int x, int y)> path;
    private int pathIndex;
    private AStarManager astar = new AStarManager();

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

    public virtual void LeftClick()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 clickPos = new Vector2(mouseWorldPos.x, mouseWorldPos.y);
            RaycastHit2D hit = Physics2D.Raycast(clickPos, Vector2.zero);

            if (hit.collider.CompareTag("Mob"))
            {
                MovementMobController mob = hit.collider.GetComponent<MovementMobController>();

                if (mob != null)
                {
                    int id = mob.GetID();
                    string nameMob = mob.GetNameMob();

                    Debug.Log($"Clicked mob [{id}] {nameMob}");
                }

                return;
            }
            // Nếu không click vào mob thì tiến hành di chuyển đến vị trí click
            targetPosition = clickPos;
            isMovingToTarget = true;
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
        if ((menu != null && menu.GetIsActive()) || EventSystem.current.IsPointerOverGameObject() || isBusy)     
            return;

        LeftClick();

        if (isMovingToTarget)
        {
            string pathMapFile = Application.dataPath + $"/Map/{SceneManager.GetActiveScene().name}.bin";
            MapData mapData = new MapData();

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

                pathIndex = 0;
            }

            // nếu đi hết path thì nghỉ
            if (pathIndex >= path.Count)
            {
                path = null; // reset để lần sau tạo path mới
                startPosition = endMovementPosition;
                isMovingToTarget = false;
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
                }
                else
                {
                    movement.y = directionToTarget.y > 0 ? 1 : -1;
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
                    movement = lastMove;
                    return;
                }
            }

            // kiểm tra chuyển Node
            if (Vector2.Distance(currentPosition, targetNode) < 0.1f)
            {
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
        if (Input.GetKeyDown(KeyCode.K))
        {
            currentState = PlayerState.Injured;
            TriggerAnimation("Injured", 0.3f);
            UpdateLastMoveToAnimator();
        }
    }
}
