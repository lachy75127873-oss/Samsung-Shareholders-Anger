using UnityEngine;

public class PlayerAnimationHandler : StateMachineBehaviour
{
    [SerializeField] string parameter;

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.SetBool(parameter, false);
    }
}