using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerIdle : StateMachineBehaviour
{
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (animator.TryGetComponent(out PlayerController playerController))
        {
            playerController.UpdateIsHurtServerRpc(false);
        }

        animator.ResetTrigger("Hurt");
        animator.ResetTrigger("Die");
        animator.ResetTrigger("Revive");
    }
}
