using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ResetAni : StateMachineBehaviour
{
    PlayerAni ani;
    Player player;
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (ani == null)
            ani = animator.gameObject.GetComponent<PlayerAni>();
        if(player==null)
            player= animator.gameObject.GetComponent<Player>();
        player.NowCC = false;
        ani.GoBackIdle_canMove();
        animator.SetBool("Run", false);
        animator.SetInteger("comboIndex", 0);
        animator.SetBool("Action", false);
        animator.SetBool("PullSword", false);
        animator.SetBool("StunRock", false);
        if (animator.GetBool("Die"))
            animator.SetBool("Die", false);
    }
}
