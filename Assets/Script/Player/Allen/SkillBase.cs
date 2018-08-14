using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillBase : Photon.MonoBehaviour
{
    protected enum SkillAction
    {
        None,
        is_Q,
        is_W,
        is_E,
        is_R
    }
    public bool brfore_shaking;
    protected SkillAction nowSkill = SkillAction.None;
    
    #region 技能Event
    //Q按下&&偵測
    public virtual void Skill_Q_Click()
    { }
    public virtual void In_Skill_Q()
    { }

    //WQ按下&&偵測
    public virtual void Skill_W_Click()
    { }
    public virtual void In_Skill_W()
    { }

    //EQ按下&&偵測
    public virtual void Skill_E_Click()
    { }
    public virtual void In_Skill_E()
    { }

    //RQ按下&&偵測
    public virtual void Skill_R_Click()
    { }
    public virtual void In_Skill_R()
    { }
    #endregion

    public virtual void CancelDetectSkill(Player.SkillData _nowSkill)
    { }

    public virtual void InterruptSkill(bool _absolute)
    { }
}
