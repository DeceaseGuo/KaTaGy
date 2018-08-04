using UnityEngine;

public class HitChange : StateMachineBehaviour
{

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.gameObject.GetComponent<Player>().stopAnything_Switch(true);
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
            animator.gameObject.GetComponent<Player>().buildManager.nowBuilding = false;
            animator.gameObject.GetComponent<PlayerAni>().WeaponChangePos(1);
            Debug.Log("中斷");
        }
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.gameObject.GetComponent<PlayerAni>().WeaponChangePos(3);
        Debug.Log("離開");
    }
}
