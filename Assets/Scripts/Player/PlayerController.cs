using System;
using UnityEngine;
using UnityEngine.InputSystem.Processors;

public class PlayerController : MonoBehaviour
{
    [Header("Animator")]
    [SerializeField] Animator animator;

    [Header("Movement")]
    [SerializeField] Rigidbody rb;
    [SerializeField][Range(0, 1000)][Tooltip("달리기 속도")] float runSpeed;

    [Header("Side")]
    [SerializeField][Range(-1, 1)][Tooltip("현재 위치한 레일")] int currentRail;
    [SerializeField][Range(0, 100)][Tooltip("좌우 이동 속도")] float sideMoveSpeed;
    [SerializeField][Range(0, 100)][Tooltip("좌우 이동 간격")] float sideMoveDistance;

    [Header("Jump")]
    [SerializeField][Range(1, 1000)][Tooltip("점프 파워")] float jumpPower;
    [SerializeField][Range(1, 100)][Tooltip("점프 높이")] float MaxHeight;
    [SerializeField][Range(0, 10)][Tooltip("공중에 머무는 시간")] float airborneTime;

    [Header("Fall")]
    [SerializeField][Range(0, -100)][Tooltip("하강 시작 속도")] float velocityY;
    [SerializeField][Range(0, 100)][Tooltip("하강 속도 가속")] float fallAccel;
    [SerializeField][Range(0, 1)][Tooltip("착지 판정 높이")] float landDistance;
    [SerializeField][Tooltip("착지 판정 레이어 마스크")] LayerMask groundLayerMask;
    [SerializeField][Range(0, 1)][Tooltip("RayCast 간격")] float rayRadius;
    [SerializeField][Range(0, 1)][Tooltip("RayCast 시작 높이")] float startRayY;

    [Header("Slide")]
    [SerializeField] CapsuleCollider capsuleCollider;
    [SerializeField][Range(0, 1000)][Tooltip("공중에서 슬라이드시 내려오는 속도")] float slidDownSpeed;
    [SerializeField][Tooltip("보통의 콜라이더 크기")] Vector2 defaultCollider;
    [SerializeField][Tooltip("슬라이딩 콜라이더 크기")] Vector2 slideCollider;

    [Header("Debug")]
    [SerializeField] bool isDead = false; // 플레이어 사망시 해당 변수 전환
    [SerializeField] bool isJump = false;
    [SerializeField] bool isAirborne = false;
    [SerializeField] bool isSlide = false;
    [SerializeField] float airborneTimer = 0f;
    [SerializeField][Tooltip("좌우 이동용 Pos")] Vector3 targetMovePos;
    [SerializeField][Tooltip("점프 시점 기억용 Pos")] Vector3 beforeJumpPos;
#if UNITY_EDITOR
    [SerializeField] bool StopRun = false;
    [SerializeField][Tooltip("현재 Pos Debug용")] Vector3 currentPos;
    [SerializeField][Tooltip("현재 Velocity Debug용")] Vector3 currentVelocity;
#endif

    /*플레이어 사망시 호출*/
    public event Action OnPlayerDead;

#if UNITY_EDITOR

    private void Reset()
    {
        animator = GameObject.Find("Root").GetComponent<Animator>();
        rb = GameObject.Find("Player").GetComponent<Rigidbody>();
        capsuleCollider = GameObject.Find("Player").GetComponent<CapsuleCollider>();

        #region /*초기 값 세팅*/

        runSpeed = 30f;
        sideMoveSpeed = 100f;
        sideMoveDistance = 30f;
        currentRail = default;
        jumpPower = 200f;
        MaxHeight = 30f;
        airborneTime = 3f;
        velocityY = -1f;
        fallAccel = 20f;
        landDistance = 2.65f;
        groundLayerMask = LayerMask.GetMask("Default");
        rayRadius = 3.2f;
        startRayY = 2.7f;
        slidDownSpeed = 400f;
        defaultCollider = new(1.8f, 3.6f);
        slideCollider = new(0.8f, 1.5f);

        #endregion
    }
#endif

    private void Start()
    {
        targetMovePos = rb.position;
    }

    #region Movement

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.W))
        {
            if (!isJump && !isAirborne)
            {
                isSlide = false;
                isJump = true;
                //ChangeColliderSize();
                //animator.SetBool("isJump", true);
            }
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            if (!isSlide)
            {
                isJump = false;
                rb.useGravity = true;
                isSlide = true;
                //ChangeColliderSize();
            }

            //animator.SetBool("isJump", false);
            animator.SetBool("isSlide", true);
        }
        if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.D))
        {
            if (Input.GetKeyDown(KeyCode.A))
            {
                if (currentRail == -1)
                    return;

                currentRail--;
                targetMovePos += Vector3.left * sideMoveDistance;
            }
            else
            {
                if (currentRail == 1)
                    return;

                currentRail++;
                targetMovePos += Vector3.right * sideMoveDistance;
            }
            isSlide = false;
            animator.SetBool("isSlide", false);
        }

        ///*CheckJumpEnd*/
        //if (CheckGround())
        //{
        //    animator.SetBool("isJump", false);
        //}

        /*CheckSlideEnd*/
        if (!animator.GetBool("isSlide"))
        {
            isSlide = false;
            //ChangeColliderSize();
        }

#if UNITY_EDITOR // 디버그 전용 인풋

        if (Input.GetKeyDown(KeyCode.R))
        {
            Debug.Log("상태 초기화, 디버그용");

            isDead = false;
            isJump = false;
            isAirborne = false;
            isSlide = false;
            airborneTimer = default;
            currentRail = default;
            transform.position = default;
            rb.position = default;
            rb.useGravity = true;

            animator.SetBool("isJump", false);
            animator.SetBool("isSlide", false);
            animator.SetBool("isDead", false);
        }
        if (Input.GetKeyDown(KeyCode.X))
        {
            Debug.Log("게임 오버");
            isDead = true;
            OnPlayerDead?.Invoke();
            animator.SetBool("isDead", true);
        }
        if (Input.GetKeyDown(KeyCode.Q))
        {
            Debug.Log("땅에 닿았다.");
            animator.SetBool("isJump", false);
        }
#endif
    }

    private void FixedUpdate()
    {
        if (isDead)
        {
            rb.velocity = Vector3.zero;
            rb.AddForce(slidDownSpeed * Vector3.down, ForceMode.Impulse);
            return;
        }

        /*Jump*/
        if (isJump)
        {
            beforeJumpPos = rb.position;
            rb.AddForce(jumpPower * Vector3.up, ForceMode.Impulse);
        }

        /*Checked MaxHeight*/
        if (rb.position.y > beforeJumpPos.y + MaxHeight)
        {
            rb.velocity = new(rb.velocity.x, default, rb.velocity.z);
            rb.position = new(rb.position.x, beforeJumpPos.y + MaxHeight, rb.position.z);
            /*Airborne*/
            isAirborne = true;
            rb.useGravity = false;

            //Debug.Log("Checked MaxHeight");
        }

        /*Airborne*/
        if (isAirborne)
        {
            airborneTimer += Time.fixedDeltaTime;
            //Debug.Log("airborneTimer");

            if (airborneTimer >= airborneTime)
            {
                isAirborne = false;
                rb.useGravity = true;
                airborneTimer = default;
                rb.velocity = new(rb.velocity.x, velocityY, rb.velocity.z);

                Debug.Log("Airborne End");
            }
        }

        if (isSlide)
        {
            isJump = false;
            isAirborne = false;
            //rb.velocity += slidDownSpeed * Time.fixedDeltaTime * Vector3.down;
            rb.AddForce(slidDownSpeed * Vector3.down, ForceMode.Impulse);
        }

        if (rb.velocity.y < 0 && !isSlide)
        {
            rb.velocity += fallAccel * Time.fixedDeltaTime * Vector3.down;
            //Debug.Log("Falling");
        }

        /*SideMove*/
        if (rb.position.x != targetMovePos.x)
        {
            float nextX = Mathf.MoveTowards(
                rb.position.x,
                targetMovePos.x,
                sideMoveSpeed * Time.fixedDeltaTime
            );

            var next = rb.position;
            next.x = nextX;
            rb.MovePosition(next);
        }

#if UNITY_EDITOR 
        /*ForwardMove*/
        if (!StopRun)
        {
#endif
            //rb.velocity = new(rb.velocity.x, rb.velocity.y, runSpeed);
            rb.MovePosition(rb.position + runSpeed * Time.fixedDeltaTime * Vector3.forward);

#if UNITY_EDITOR

        }
#endif

        /*CheckJumpEnd*/
        if (CheckGround())
        {
            isJump = false;
            animator.SetBool("isJump", false);
        }
        else
        {
            animator.SetBool("isJump", true);
        }

        ChangeColliderSize();

#if UNITY_EDITOR // 디버그 전용 속성
            currentVelocity = rb.velocity;
        currentPos = rb.position;
#endif
    }

    /// <summary>
    /// Raycast를 통해 플레이어 착지 판단
    /// </summary>
    bool CheckGround()
    {
        Ray[] rays = new Ray[4]
        {
            new(rb.position + (Vector3.forward * rayRadius) + (Vector3.up * startRayY), Vector3.down),
            new(rb.position + (Vector3.back * rayRadius) + (Vector3.up * startRayY), Vector3.down),
            new(rb.position + (Vector3.right * rayRadius) + (Vector3.up * startRayY), Vector3.down),
            new(rb.position + (Vector3.left * rayRadius) + (Vector3.up * startRayY), Vector3.down)
        };

        for (int i = 0; i < rays.Length; i++)
        {
#if UNITY_EDITOR

            Debug.DrawRay(rays[i].origin, rays[i].direction * landDistance, Color.green);
#endif
            if (Physics.Raycast(rays[i], landDistance, groundLayerMask))
            {
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// 슬라이딩 여부에 따라 콜라이더 사이즈를 변경
    /// </summary>
    void ChangeColliderSize()
    {
        if (isSlide)
        {
            capsuleCollider.center = new Vector3(
                capsuleCollider.center.x,
                slideCollider.x,
                capsuleCollider.center.z
            );
            capsuleCollider.height = slideCollider.y;
        }
        else
        {
            capsuleCollider.center = new Vector3(
                capsuleCollider.center.x,
                defaultCollider.x,
                capsuleCollider.center.z
            );
            capsuleCollider.height = defaultCollider.y;
        }
    }

    #endregion

    #region Trigger/Collision

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("트리거");

        if (other.gameObject.layer == 7)
        {
            isDead = true;
            animator.SetBool("isDead", true);
            OnPlayerDead?.Invoke();
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
    }

    #endregion
}