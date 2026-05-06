using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.U2D.Animation;

public class RegisterDemoController : MovementPlayerController
{
    private Animator animatorChild;
    private GameObject player;
    private Animator uiPickChienBinh;
    private Animator uiPickSatThu;
    private Animator uiPickPhapSu;
    private Animator uiPickXaThu;
    private RegisterDemoController demo;
    private RegisterView register;
    private static int idSchool;
    private List<SpriteResolver> resolvers;

    private int lastFrame = -1;
    private string lastState = "";

    private void Awake()
    {
        animatorChild = GetComponent<Animator>();
        resolvers = GetComponentsInChildren<SpriteResolver>().ToList();
        register = GameObject.Find("Register").GetComponent<RegisterView>();
        if (GameObject.Find("CharaterSelectionUI"))
        {
            uiPickChienBinh = GameObject.Find("UIPickChienBinh").GetComponent<Animator>();
            uiPickSatThu = GameObject.Find("UIPickSatThu").GetComponent<Animator>();
            uiPickPhapSu = GameObject.Find("UIPickPhapSu").GetComponent<Animator>();
            uiPickXaThu = GameObject.Find("UIPickXaThu").GetComponent<Animator>();
        }
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
    public override void OnUpdate()
    {
        LeftClick();
        UpdateSprite();
    }
    public override void OnLateUpdate()
    {
        return;
    }
    public override void OnFixedUpdate()
    {
        return;
    }

    public override void LeftClick()
    {
        if (Input.GetMouseButtonDown(0))
        {
            // Lấy vị trí chuột trong thế giới
            Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 clickPos = new Vector2(mouseWorldPos.x, mouseWorldPos.y);

            RaycastHit2D hit = Physics2D.Raycast(clickPos, Vector2.zero);

            if (hit.collider != null)
            {
                if (hit.collider.CompareTag("Player"))
                {
                    //Tiếp nhận gameobject player bị click
                    player = hit.collider.gameObject;
                    //Tạo biến demo hứng component demo từ player
                    demo = player.GetComponent<RegisterDemoController>();
                    if (GameObject.Find("CharaterSelectionUI"))
                    {
                        switch (player.name)
                        {
                            case "ChienBinh":
                                uiPickChienBinh.SetBool("isPicked", true);
                                uiPickSatThu.SetBool("isPicked", false);
                                uiPickPhapSu.SetBool("isPicked", false);
                                uiPickXaThu.SetBool("isPicked", false);
                                idSchool = 1;
                                break;
                            case "SatThu":
                                uiPickChienBinh.SetBool("isPicked", false);
                                uiPickSatThu.SetBool("isPicked", true);
                                uiPickPhapSu.SetBool("isPicked", false);
                                uiPickXaThu.SetBool("isPicked", false);
                                idSchool = 2;
                                break;
                            case "PhapSu":
                                uiPickChienBinh.SetBool("isPicked", false);
                                uiPickSatThu.SetBool("isPicked", false);
                                uiPickPhapSu.SetBool("isPicked", true);
                                uiPickXaThu.SetBool("isPicked", false);
                                idSchool = 3;
                                break;
                            case "XaThu":
                                uiPickChienBinh.SetBool("isPicked", false);
                                uiPickSatThu.SetBool("isPicked", false);
                                uiPickPhapSu.SetBool("isPicked", false);
                                uiPickXaThu.SetBool("isPicked", true);
                                idSchool = 4;
                                break;
                            default:
                                uiPickChienBinh.SetBool("isPicked", false);
                                uiPickSatThu.SetBool("isPicked", false);
                                uiPickPhapSu.SetBool("isPicked", false);
                                uiPickXaThu.SetBool("isPicked", false);
                                break;
                        }
                    }
                    register.OnSelectSchool();
                    demo.UpdateAnimation();
                    return;
                }
            }
        }
    }
    public static int GetIDSchool()
    {
        return idSchool;
    }
    public override void UpdateAnimation()
    {
        animatorChild.SetTrigger("Atk");
    }

    private int GetFrameByTime(float t, float[] changeTimes)
    {
        t %= 1f;

        for (int i = 0; i < changeTimes.Length; i++)
        {
            if (t < changeTimes[i])
                return Mathf.Max(0, i - 1);
        }

        return changeTimes.Length - 1;
    }
    private void UpdateSprite()
    {
        if (animatorChild == null) return;

        for (int i = 0; i < resolvers.Count; i++)
        {
            if (resolvers[i] == null)
                continue;
        }

        AnimatorStateInfo state = animatorChild.GetCurrentAnimatorStateInfo(0);

        // Stand
        if (state.IsName("Stand"))
        {
            float t = state.normalizedTime % 1f;

            float[] moveChangeTimes = { 0.0f, 0.5f }; // Clip dài 0:40 giây, đổi frame ở 0 / 0.4, 0.2 / 0.4

            int frame = GetFrameByTime(t, moveChangeTimes);

            if (lastState != "StandFront")
            {
                lastFrame = -1;
                lastState = "StandFront";
                SetAllResolvers("Stand", $"StandFront");
            }
            foreach (var r in resolvers)
            {
                if (r != null && r.spriteLibrary != null && r.gameObject.name == "4_0_0")
                {
                    r.SetCategoryAndLabel("Stand", $"StandFrontFrame{frame}");
                    r.ResolveSpriteToSpriteRenderer();
                }
            }
        }
        // Attack
        if (state.IsName("Atk"))
        {
            float t = state.normalizedTime % 1f;

            float[] moveChangeTimes = { 0.0f, 0.6667f }; // Clip dài 0:15 giây, đổi frame ở 0 / 0.15, 0.1 / 0.15

            int frame = GetFrameByTime(t, moveChangeTimes);

            if (frame != lastFrame || lastState != "AtkFront")
            {
                lastFrame = frame;
                lastState = "AtkFront";
                SetAllResolvers("Atk", $"AtkFrontFrame{frame}");
            }
        }
    }
    public void SetAllResolvers(string category, string label)
    {
        foreach (var r in resolvers)
        {
            if (r != null && r.spriteLibrary != null)
            {
                r.SetCategoryAndLabel(category, label);
                r.ResolveSpriteToSpriteRenderer();
            }
        }
    }
}
