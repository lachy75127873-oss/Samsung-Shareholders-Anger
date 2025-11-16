using UnityEngine;


public class Player : MonoBehaviour
{
    [Header("Animator")]
    [SerializeField] Animator animator;

    [Header("Movement")]
    [SerializeField] Rigidbody rb;
    [SerializeField][Range(0, 1000)][Tooltip("달리기 속도")] float runSpeed;
    [SerializeField][Range(0, 100)][Tooltip("좌우 이동 속도")] float sideMoveSpeed;
    [SerializeField][Range(0, 100)][Tooltip("좌우 이동 간격")] float sideMoveDistance;
    [SerializeField][Range(-1, 1)][Tooltip("현재 위치한 레일")] int currentRail;
    [SerializeField][Range(1, 1000)][Tooltip("점프 파워")] float jumpPower;
    [SerializeField][Range(1, 100)][Tooltip("점프 높이")] float MaxHeight;
    [SerializeField][Range(0, 10)][Tooltip("공중에 머무는 시간")] float airborneTime;
    [SerializeField][Range(0, -100)][Tooltip("하강 시작 가속도")] float velocityY;
    [SerializeField][Range(0, 10)][Tooltip("하강 속도 보정")] float gravityCorrection;
    [SerializeField][Range(0, 100)][Tooltip("착지 판정 높이")] float landDistance;

    [Header("Debug")]
    [SerializeField] Vector3 targetMovePos;
    [SerializeField] bool isJump = false;
    [SerializeField] bool isAirborne = false;
    [SerializeField] float airborneTimer = 0f;
    [SerializeField] Vector3 currentVelocity;

#if UNITY_EDITOR
    private void Reset()
    {
        animator = GameObject.Find("Root").GetComponent<Animator>();
        rb = GameObject.Find("Player").GetComponent<Rigidbody>();

        sideMoveSpeed = 100f;
        sideMoveDistance = 30f;
        currentRail = default;
        jumpPower = 200f;
        MaxHeight = 30f;
        airborneTime = 3f;
        velocityY = -1f;
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
            Debug.Log("점프");

            if (!isJump)
                isJump = true;

            animator.SetBool("isJump", true);
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            Debug.Log("슬라이드");

            animator.SetBool("isJump", false);
            animator.SetBool("isSlide", true);
        }
        if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.D))
        {
            if (Input.GetKeyDown(KeyCode.A))
            {
                if (currentRail == -1)
                    return;

                //Debug.Log("좌 이동");
                currentRail--;
                targetMovePos += Vector3.left * sideMoveDistance;
            }
            else
            {
                if (currentRail == 1)
                    return;

                //Debug.Log("우 이동");
                currentRail++;
                targetMovePos += Vector3.right * sideMoveDistance;
            }
            animator.SetBool("isSlide", false);
        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            Debug.Log("상태 초기화, 디버그용");
            animator.SetBool("isJump", false);
            animator.SetBool("isSlide", false);
            animator.SetBool("isDead", false);
        }
        if (Input.GetKeyDown(KeyCode.X))
        {
            Debug.Log("게임 오버");
            animator.SetBool("isDead", true);
        }
        if (Input.GetKeyDown(KeyCode.Q))
        {
            Debug.Log("땅에 닿았다.");
            animator.SetBool("isJump", false);
        }

        /*보간 테스트*/
        if (Input.GetKeyDown(KeyCode.T))
        {
            Vector3 testA = Vector3.MoveTowards(
                new Vector3(0, 0, 0),
                new Vector3(1, 1, 1),
                Time.deltaTime);

            Vector3 testB = Vector3.MoveTowards(
                new Vector3(0, 0, 0),
                new Vector3(1, 0, 1),
                Time.deltaTime);

            Debug.Log($"x = {testA.x}, y = {testA.y}, z = {testA.z}");
            Debug.Log($"x = {testB.x}, y = {testB.y}, z = {testB.z}");
        }
    }

    private void FixedUpdate()
    {
        /*Jump*/
        if (isJump)
        {
            rb.AddForce(jumpPower * Vector3.up, ForceMode.Impulse);
            isJump = false;
        }

        if (rb.position.y > MaxHeight)
        {
            rb.velocity = new(rb.velocity.x, default, rb.velocity.z);
            rb.position = new(rb.position.x, MaxHeight, rb.position.z);
            /*Airborne*/
            isAirborne = true;
            rb.useGravity = false;
        }

        /*Airborne*/
        if (isAirborne)
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

        /*Debug*/
        currentVelocity = rb.velocity;
    }
}