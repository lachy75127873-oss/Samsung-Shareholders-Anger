using System;
using UnityEngine;

public class PlayerDeadAnimationExitHandler : StateMachineBehaviour
{
    public int index;
    //[SerializeField] PlayerAnimationParameter parameter;
    public event Action OnPlayerDeadAnimationStarted;

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Debug.Log("OnStateExit");
        OnPlayerDeadAnimationStarted?.Invoke();
    }
}