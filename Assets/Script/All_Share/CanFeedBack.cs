using UnityEngine;

public class CanFeedBack : StateMachineBehaviour
{
    isDead isDeadScript;
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (isDeadScript == null)
            isDeadScript = animator.GetComponent<isDead>();

        animator.GetComponent<isDead>().notFeedBack = true;
    }
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.GetComponent<isDead>().notFeedBack = false;
    }
}
