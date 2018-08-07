using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ComboStart : StateMachineBehaviour
{
    PlayerAni ani;
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (ani == null)
            ani = animator.gameObject.GetComponent<PlayerAni>();

        ani.comboCheck(0);
    }

     /*public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
     {
         ani.ChangeNowDir();
     }*/
}
