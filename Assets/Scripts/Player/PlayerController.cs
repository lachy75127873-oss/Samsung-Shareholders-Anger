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
    [SerializeField] bool isGrounded = false;
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

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.W))
        {
            TryJump();
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            TrySlide();
        }
        if (Input.GetKeyDown(KeyCode.A))
        {
            TryMove(KeyCode.A);
        }
        if (Input.GetKeyDown(KeyCode.D))
        {
            TryMove(KeyCode.D);
        }

        if(isSlide)
            isSlide = animator.GetBool("isSlide");
    }

    private void FixedUpdate()
    {
        if (isDead)
            Dead();

        ChangeColliderSize();

        var _ = IsGrounded();
        if (isGrounded != _)
        {
            Debug.Log($"IsGrounded = {_}");
            if (!isGrounded)
            {
                if (isAirborne)
                {
                    isAirborne = false;
                    
                }
                animator.SetBool("isJump", false);
            }

            isGrounded = _;
        }

        if (isJump)
        {
            Jump();
            isJump = false;
        }

        // 잘못하면 무한 에어본 되는거 아닌가

        if (!isGrounded)
        {
            if (!isAirborne)
            {
                if (IsMaxHeight())
                    ChangeStateAirBorne();
            }
            else
                CheckAirborneTimer();
        }

        // 하강은 점프 => 슬라이드거나 일반 하강
        if (isSlide) Slide();
        else if (IsFalling())
        {
            rb.velocity += fallAccel * Time.fixedDeltaTime * Vector3.down;
        }

        if (IsSideMoving())
            MoveSide();

        if (!StopRun)
        {
            rb.MovePosition(rb.position + runSpeed * Time.fixedDeltaTime * Vector3.forward);
        }

#if UNITY_EDITOR
        currentVelocity = rb.velocity;
        currentPos = rb.position;
#endif
    }

    #region Input
    private void TryJump()
    {
        if (isDead) return;

        // 공중 상태가 아니라면 점프 가능
        if (!isGrounded) return;

        // 점프 로직 활성화
        isJump = true;

        // 애니메이션 활성화
        animator.SetBool("isJump", true);

        // 슬라이딩 중이었을 경우
        isSlide = false;
        if (animator.GetBool("isSlide"))
            animator.SetBool("isSlide", false);
    }
    private void TrySlide()
    {
        if (isDead) return;

        // 슬라이딩 중이 아니라면 점프 가능
        if (isSlide) return;

        // 슬라이딩 로직 활성화
        isSlide = true;

        // 애니메이션 활성화
        animator.SetBool("isSlide", true);

        // 점프 중이었을 경우
        isAirborne = false;
        airborneTimer = default;
        if (animator.GetBool("isJump"))
            animator.SetBool("isJump", false);
    }
    private void TryMove(KeyCode key)
    {
        if (isDead) return;

        if (key == KeyCode.A)
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

        if (isSlide)
        {
            isSlide = false;
            animator.SetBool("isSlide", false);
        }
    }
    #endregion

    #region MoveLogic
    private void Jump()
    {
        // 점프 로직
        beforeJumpPos = rb.position;
        rb.AddForce(jumpPower * Vector3.up, ForceMode.Impulse);
    }
    private void MoveSide()
    {
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
    }
    private void Slide()
    {
        rb.AddForce(slidDownSpeed * Vector3.down, ForceMode.Impulse);
    }
    private void Dead()
    {
        rb.velocity = Vector3.zero;
        rb.AddForce(slidDownSpeed * Vector3.down, ForceMode.Impulse);
    }
    #endregion

    #region CheckCondition
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
    void CheckAirborneTimer()
    {
        airborneTimer += Time.fixedDeltaTime;

        if (airborneTimer >= airborneTime)
        {
            isAirborne = false;
            rb.useGravity = true;
            airborneTimer = default;
            rb.velocity = new(rb.velocity.x, velocityY, rb.velocity.z);
        }
    }
    void ChangeStateAirBorne()
    {
        rb.velocity = new(rb.velocity.x, default, rb.velocity.z);
        rb.position = new(rb.position.x, beforeJumpPos.y + MaxHeight, rb.position.z);

        isAirborne = true;
        rb.useGravity = false;
    }
    bool IsMaxHeight()
    {
        return rb.position.y > beforeJumpPos.y + MaxHeight;
    }
    bool IsFalling()
    {
        return rb.velocity.y < 0 && !isSlide;
    }
    bool IsSideMoving()
    {
        return rb.position.x != targetMovePos.x;
    }
    bool IsGrounded()
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

            Debug.DrawRay(rays[i].origin, rays[i].direction * landDistance, Color.green);
            if (Physics.Raycast(rays[i], landDistance, groundLayerMask))
            {
                return true;
            }
        }

        return false;
    }
    #endregion

    #region Trigger/Collision

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == 7)
        {
            isDead = true;
            StopRun = true;
            animator.SetBool("isDead", true);
            OnPlayerDead?.Invoke();
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
    }

    #endregion
}