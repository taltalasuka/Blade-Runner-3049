using UnityEngine;

public class PlayerRun : StateMachineBehaviour
{
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (animator.GetComponent<PlayerVFXManager>() != null)
        {
            animator.GetComponent<PlayerVFXManager>().UpdateFootStep(true);
        }
    }
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (animator.GetComponent<PlayerVFXManager>() != null)
        {
            animator.GetComponent<PlayerVFXManager>().UpdateFootStep(false);
        }
    }
}
