using System;
using UnityEngine;

public class PlayerDeadAnimationEndHandler : StateMachineBehaviour
{
    public int index;
    private bool isSended = false;// 연속 호출 막기 위함
    public event Action OnPlayerDeadAnimationStarted;

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (!isSended && stateInfo.normalizedTime >= 1f)
        {
            OnPlayerDeadAnimationStarted?.Invoke();
            isSended = true;
        }
    }
}