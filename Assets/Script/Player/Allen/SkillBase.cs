using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillBase : Photon.MonoBehaviour
{
    protected Player playerScript;
    protected PlayerAni aniScript;
    protected SkillIcon skillIconManager;
    protected SkillIcon SkillIconManager { get { if (skillIconManager == null) skillIconManager = SkillIcon.instance; return skillIconManager; } }
    protected Vector3 mySkillPos;

    public float skillQ_needAP = 1;
    public float skillW_needAP = 1;
    public float skillE_needAP = 1;
    public float skillR_needAP = 1;

    protected Coroutine[] skillCD_CT = new Coroutine[4]; 

    public enum SkillAction
    {
        None,
        is_Q,
        is_W,
        is_E,
        is_R
    }
    public bool brfore_shaking = true;
    public SkillAction nowSkill = SkillAction.None;

    private void Awake()
    {
        playerScript = GetComponent<Player>();
        aniScript = GetComponent<PlayerAni>();
    }

    public void ArriveBP()
    {
        brfore_shaking = false;
    }

    [PunRPC]
    public void GetSkillPos(Vector3 _pos)
    {
        mySkillPos = _pos;
    }

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

    #region 直接恢復cd(中斷,或死亡用●前搖點之前)
    public virtual void ClearQ_Skill()
    { }
    public virtual void ClearW_Skill()
    { }
    public virtual void ClearE_Skill()
    { }
    public virtual void ClearR_Skill()
    { }
    #endregion

    #region 中斷技能(●前搖點之後進入CD)
    public virtual void ResetQ_GoCD()
    { }
    public virtual void ResetW_GoCD()
    { }
    public virtual void ResetE_GoCD()
    { }
    public virtual void ResetR_GoCD()
    { }
    #endregion

    public virtual void CancelDetectSkill(Player.SkillData _nowSkill)
    { }

    public virtual void InterruptSkill()
    { }
}
