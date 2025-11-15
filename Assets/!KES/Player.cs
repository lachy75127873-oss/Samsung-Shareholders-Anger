using UnityEngine;

public class Player : MonoBehaviour
{
    [Header("Animator")]
    [SerializeField] Animator animator;

#if UNITY_EDITOR
    private void Reset()
    {
        animator = GameObject.Find("Root").GetComponent<Animator>();
    }
#endif

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.W))
        {
            Debug.Log("점프");
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
            Debug.Log("좌 또는 우 이동");
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
    }
}