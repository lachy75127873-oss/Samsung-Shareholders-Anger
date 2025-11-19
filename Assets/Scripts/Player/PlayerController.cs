using System;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public enum PlayerAnimationParameter
{
    isJump,
    isSlide,
    isDead,
    isSpin,
    isInjury
}

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
    [Range(1, 1000)][Tooltip("점프 파워")] public float jumpPower;
    [Range(1, 100)][Tooltip("점프 높이")] public float MaxHeight;
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

    [Header("Injure")]
    [SerializeField] Transform injuryRayPoint;
    [SerializeField][Tooltip("부상 여부")] bool isInjured = false;
    [SerializeField][Tooltip("부상 타이머")] float injuryTimer = 0f;
    [SerializeField][Range(0, 5)][Tooltip("부상 지속 시간")] float injuryDuration;
    [SerializeField][Range(0, 5)][Tooltip("부상 판정 레이 길이")] float injuryRayLength;
    [SerializeField][Tooltip("부상 판정 레이어 마스크")] LayerMask injuryLayerMask;
    [SerializeField] bool? lastSideInput = null; // left = false, right = true

    [Header("Item")]
    [SerializeField] PlayerItemRootRange itemRootRange;

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

    public bool IsGrounded
    {
        get => isGrounded;
        set
        {
            if (isGrounded == value) return;

            if (!isGrounded)
            {
                if (isAirborne)
                {
                    isAirborne = false;

                }
                animator.SetBool("isJump", false);
            }

            isGrounded = value;
        }
    }

    #region Event
    public event Action OnPlayerDead;
    #endregion

    #region LifeCycle

#if UNITY_EDITOR

    private void Reset()
    {
        animator = GameObject.Find("Root").GetComponent<Animator>();
        rb = GameObject.Find("Player").GetComponent<Rigidbody>();
        capsuleCollider = GameObject.Find("Player").GetComponent<CapsuleCollider>();
        injuryRayPoint = GameObject.Find("RayPoint").transform;
        itemRootRange = GetComponentInChildren<PlayerItemRootRange>();

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

    private void Awake()
    {
        ManagerRoot.gameManager.RegisterPlayer(this);
    }

    private void OnEnable()
    {
        animator.GetBehaviours<PlayerDeadAnimationEndHandler>()
            .FirstOrDefault(x => x.index == 0)
            .OnPlayerDeadAnimationStarted += PlayerController_OnPlayerDeadAnimationStarted;
    }

    private void OnDisable()
    {
        animator.GetBehaviours<PlayerDeadAnimationEndHandler>()
            .FirstOrDefault(x => x.index == 0)
            .OnPlayerDeadAnimationStarted -= PlayerController_OnPlayerDeadAnimationStarted;
    }

    private void Start()
    {
        targetMovePos = rb.position;
    }

    private void Update()
    {
        /*슬라이드 애니메이션이 끝나야 슬라이드가 끝*/
        if (isSlide)
            isSlide = animator.GetBool("isSlide");

        if (isInjured)
        {
            injuryTimer += Time.deltaTime;
            if(injuryTimer >= injuryDuration)
            {
                isInjured = false;
                injuryTimer = default;
                animator.SetFloat("isInjury", 0f);
            }
        }

#if UNITY_EDITOR

        if (Input.GetKeyDown(KeyCode.R))
        {
            StopRun = false;
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
        else if (Input.GetKeyDown(KeyCode.X))
        {
            Debug.Log("게임 오버");
            isDead = true;
            OnPlayerDead?.Invoke();
            animator.SetBool("isDead", true);
        }

        else if (Input.GetKeyDown(KeyCode.I))
        {
            ManagerRoot.Instance.itemEffectManager?.DebugLogStatus();
        }
#endif
    }

    private void FixedUpdate()
    {
        /*사망시 로직*/
        if (isDead)
            Dead();

        if (!isInjured)
            if (CheckInjured())
                ResotreLastRail();

        /*캐릭터 상태에 따라 콜라이더 조절*/
        ChangeColliderSize();
        /*지상 or 공중 체크, 프로퍼티 확인*/
        IsGrounded = CheckGrouded();

        /*점프*/
        if (isJump)
        {
            Jump();
            isJump = false;
        }

        /*점프 후 공중에 뜬 상태*/
        if (!IsGrounded)
        {
            if (!isAirborne)
            {
                if (IsMaxHeight())
                    ChangeStateAirBorne();
            }
            else
                /*공중 시간 체크*/
                CheckAirborneTimer();
        }

        /*자연 낙하 or 슬라이드 낙하*/
        if (isSlide) Slide();
        else if (IsFalling())
        {
            rb.velocity += fallAccel * Time.fixedDeltaTime * Vector3.down;
        }

        /*좌우 이동*/
        if (IsSideMoving())
            MoveSide();

        /*앞으로 전진*/
        if (!StopRun)
            rb.MovePosition(rb.position + runSpeed * Time.fixedDeltaTime * Vector3.forward);

#if UNITY_EDITOR
        currentVelocity = rb.velocity;
        currentPos = rb.position;
#endif
    }

    private void OnDestroy()
    {
        // GameManager에서 해제
        ManagerRoot.gameManager?.UnregisterPlayer();
    }
    #endregion

    #region Input
    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started)
        {
            if (isDead) return;
            if (!IsGrounded) return;

            // 점프 로직 활성화
            isJump = true;

            // 애니메이션 활성화
            animator.SetBool("isJump", true);

            // 슬라이딩 중이었을 경우
            isSlide = false;
            if (animator.GetBool("isSlide"))
                animator.SetBool("isSlide", false);
        }
    }
    public void OnSlide(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started)
        {
            if (isDead) return;
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
    }
    public void OnMoveLeft(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started)
        {
            if (isDead) return;
            if (currentRail == -1) return;

            currentRail--;
            targetMovePos += Vector3.left * sideMoveDistance;

            if (isSlide)
            {
                isSlide = false;
                animator.SetBool("isSlide", false);
            }

            lastSideInput = false;
        }
    }
    public void OnMoveRight(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started)
        {
            if (isDead) return;
            if (currentRail == 1) return;

            currentRail++;
            targetMovePos += Vector3.right * sideMoveDistance;

            if (isSlide)
            {
                isSlide = false;
                animator.SetBool("isSlide", false);
            }

            lastSideInput = true;
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

    #region Injury
    bool CheckInjured()
    {
        Ray[] rays = new Ray[2]
        {
            new(injuryRayPoint.position, Vector3.right),
            new(injuryRayPoint.position, Vector3.left)
        };

        for (int i = 0; i < rays.Length; i++)
        {

            Debug.DrawRay(rays[i].origin, rays[i].direction * injuryRayLength, Color.green);
            if (Physics.Raycast(rays[i], injuryRayLength, injuryLayerMask))
            {
                isInjured = true;
                animator.SetBool("isSpin", true);
                animator.SetFloat("isInjury", 1f);
                return true;
            }
        }

        return false;
    }
    void ResotreLastRail()
    {
        if(lastSideInput == true)
        {
            currentRail--;
            targetMovePos += Vector3.left * sideMoveDistance;
        }
        else if(lastSideInput == false)
        {
            currentRail++;
            targetMovePos += Vector3.right * sideMoveDistance;
        }
        else
        {
            currentRail = default;
            targetMovePos.x = default;
        }
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
    bool CheckGrouded()
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
        //Debug.Log(other.gameObject.layer);
        if (other.gameObject.layer == 7)
        {
            isDead = true;
            StopRun = true;
            animator.SetBool("isDead", true);
        }

        if(other.CompareTag("ScoreCheck"))
        {
            ManagerRoot.Instance.scoreManager.combo += 1;
            Debug.LogWarning(ManagerRoot.Instance.scoreManager.combo);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {

    }

    #endregion

    #region Item
    public void ApplyItemEffect(ItemData itemData)
    {
        ManagerRoot.Instance.itemEffectManager?.ApplyItem(itemData);
    }

    public void SetJumpPower(float value)
    {
        jumpPower = value;
        Debug.Log($"[Item] 점프력 변경: {value}");
    }

    public void SetMaxHeight(float value)
    {
        MaxHeight = value;
        Debug.Log($"[Item] 최대 높이 변경: {value}");
    }

    public void EnableMagnetRange(float range)
    {
        itemRootRange?.ApplyMagnetRange(range);
    }

    public void DisableMagnetRange()
    {
        itemRootRange?.ApplyDefaultRange();
    }
    #endregion

    #region Dead
    private void PlayerController_OnPlayerDeadAnimationStarted()
        => OnPlayerDead?.Invoke();
    
    #endregion

}