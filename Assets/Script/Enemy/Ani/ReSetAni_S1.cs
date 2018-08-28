using UnityEngine;

public class ReSetAni_S1 : StateMachineBehaviour
{
    EnemyControl soldierScript;
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (soldierScript == null)
            soldierScript = animator.gameObject.GetComponent<EnemyControl>();

        soldierScript.NowCC = false;
    }
}
