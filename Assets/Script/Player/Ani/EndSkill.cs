using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndSkill : StateMachineBehaviour
{
    SkillBase skillScript;
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (skillScript == null)
            skillScript = animator.gameObject.GetComponent<SkillBase>();

        skillScript.InterruptSkill();
    }
}
