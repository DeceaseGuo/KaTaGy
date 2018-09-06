using UnityEngine;

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
        player.deadManager.noCC = false;
        player.lockDodge = false;
        ani.GoBackIdle_canMove();
        animator.SetBool(ani.aniHashValue[2], false);
        animator.SetInteger(ani.aniHashValue[3], 0);
        animator.SetBool(ani.aniHashValue[6], false);
        animator.SetBool(ani.aniHashValue[8], false);
        animator.SetBool(ani.aniHashValue[9], false);
        if (animator.GetBool(ani.aniHashValue[15]))
            animator.SetBool(ani.aniHashValue[15], false);
    }
}
