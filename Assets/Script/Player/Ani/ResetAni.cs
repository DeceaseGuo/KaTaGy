using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ResetAni : StateMachineBehaviour
{
    PlayerAni ani;
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (ani == null)
            ani = animator.gameObject.GetComponent<PlayerAni>();
        ani.GoBackIdle_canMove();
        animator.SetBool("Run", false);
        animator.SetInteger("comboIndex", 0);
        animator.SetBool("Action", false);
        animator.SetBool("PullSword", false);
        animator.SetBool("StunRock", false);
        animator.SetBool("Catch", false);
        animator.SetBool("CanSkill", true);
    }
}
