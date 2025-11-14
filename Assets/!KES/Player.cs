using UnityEngine;

public class Player : MonoBehaviour
{
    [Header("Animator")]
    [SerializeField] Animator animator;
    [SerializeField] bool isJump = false;
    [SerializeField] bool isSlide = false;
    [SerializeField] bool isDead = false;

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
            if (!isDead)
                if (!isJump)
                {
                    isJump = true;
                    isSlide = false;
                }
        }
        if (Input.GetKeyDown(KeyCode.A))
        {
            if (!isDead)
                if (isSlide)
                    isSlide = false;
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            if (!isDead)
                if (!isSlide)
                {
                    isJump = false;
                    isSlide = true;
                }
        }
        if (Input.GetKeyDown(KeyCode.D))
        {
            if (!isDead)
                if (isSlide)
                    isSlide = false;
        }
        if (Input.GetKeyDown(KeyCode.X))
        {
            isDead = true;
        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            isJump = false;
            isSlide = false;
            isDead = false;
        }

        animator.SetBool("isJump", isJump);
        animator.SetBool("isSlide", isSlide);
        animator.SetBool("isDead", isDead);
    }
}