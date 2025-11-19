using UnityEngine;

public class PlayerAnimationHandler : StateMachineBehaviour
{
    public int index;
    [SerializeField] PlayerAnimationParameter parameter;

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.SetBool(parameter.ToString(), false);
    }
}