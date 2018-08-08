using UnityEngine;

public class HitChange : StateMachineBehaviour
{
    Player playerScript;
    PlayerAni ani;
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (playerScript == null || ani == null)
        {
            playerScript = animator.gameObject.GetComponent<Player>();
            ani = animator.gameObject.GetComponent<PlayerAni>();
        }
        playerScript.stopAnything_Switch(true);
        animator.SetBool("PullSword", false);
        Debug.Log("進入");
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (animator.GetBool("PullSword"))
        {
            animator.SetBool("PullSword", false);
            animator.SetBool("NowBuild", false);
            playerScript.buildManager.nowBuilding = false;
            ani.WeaponChangePos(1);
            Debug.Log("中斷");
        }
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        ani.WeaponChangePos(3);
        Debug.Log("離開");
    }
}
