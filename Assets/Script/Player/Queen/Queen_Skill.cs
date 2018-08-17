using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyCode.Timer;
using MyCode.Projector;


public class Queen_Skill : SkillBase
{
    //技能提示
    private Projector allSkillRange;
    [SerializeField] Projector[] projector_Q = new Projector[2];
    //W技能
    [SerializeField] Projector[] projector_W = new Projector[2]; //0範圍 1攻擊範圍

    [Tooltip("技能圖")]
    public List<Sprite> mySkillIcon;

    //Q技能
    private bool firstQAtk;
    private bool endQAtk;
    IEnumerator skillQ_CT;

    private void Start()
    {
        if (photonView.isMine)
        {
            allSkillRange = GameObject.Find("AllSkillRange_G").GetComponent<Projector>();
            SkillIconManager.SetSkillIcon(mySkillIcon);
        }
        else
        {
            allSkillRange = GameObject.Find("AllSkillRange_R").GetComponent<Projector>();
            skillQ_CT = Timer.NextFrame(SetQ_CT);
        }
    }
    [SerializeField] GameObject testObj;

    //Q的範圍
    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, 5.1f);
        // Gizmos.DrawWireSphere(transform.localPosition + new Vector3(0, 0, 2.2f), 5.5f);
        //  Gizmos.DrawWireCube(transform.localPosition + transform.forward * 3.5f, new Vector3(4.7f, 2f, 13));
    }

    #region 技能Event
    //Q按下&&偵測
    public override void Skill_Q_Click()
    {
        //消耗不足
        if (!playerScript.ConsumeAP(1f, false))
            return;

        playerScript.canSkill_Q = false;
        playerScript.SkillState = Player.SkillData.skill_Q;
        //顯示範圍
        ProjectorManager.SwitchPorjector(projector_Q, true);
    }
    public override void In_Skill_Q()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (playerScript.ConsumeAP(1f, true))
            {
                playerScript.SkillState = Player.SkillData.None;
                ProjectorManager.SwitchPorjector(projector_Q, false);
                //關閉顯示範圍

                transform.forward = playerScript.arrow.forward;
                playerScript.stopAnything_Switch(true);
                playerScript.Net.RPC("Skill_Q_Fun", PhotonTargets.All);
            }
        }
        if (Input.GetMouseButtonDown(1))
        {
            playerScript.CancelNowSkill();
        }
    }

    //W按下&&偵測
    public override void Skill_W_Click()
    {
        //消耗不足
        if (!playerScript.ConsumeAP(1f, false))
            return;

        playerScript.canSkill_W = false;
        playerScript.SkillState = Player.SkillData.skill_W;
        //顯示範圍
        ProjectorManager.SwitchPorjector(projector_W, true);
    }
    public override void In_Skill_W()
    {
        ProjectorManager.ChangePos(projector_W[1], projector_W[0].transform, playerScript.MousePosition, 19);
        if (Input.GetMouseButtonDown(0))
        {
            if (playerScript.ConsumeAP(1f, true))
            {
                playerScript.SkillState = Player.SkillData.None;
                //開啟攻擊範圍
                playerScript.Net.RPC("GetSkillPos", PhotonTargets.All, projector_W[1].transform.position);
                //關閉顯示範圍
                ProjectorManager.SwitchPorjector(projector_W, false);

                transform.forward = playerScript.arrow.forward;
                playerScript.stopAnything_Switch(true);
                playerScript.Net.RPC("Skill_W_Fun", PhotonTargets.All);
            }
        }
        if (Input.GetMouseButtonDown(1))
        {
            playerScript.CancelNowSkill();
        }
    }

    //E按下&&偵測
    public override void Skill_E_Click()
    { }
    public override void In_Skill_E()
    { }

    //R按下&&偵測
    public override void Skill_R_Click()
    { }
    public override void In_Skill_R()
    { }
    #endregion


    #region Q技能
    public void Q_Skill()
    {
        if (!photonView.isMine)
            OpenDetect(true);
    }

    private void OpenDetect(bool _t)
    {
        if (_t)
            StartCoroutine(skillQ_CT);
        else
            StopCoroutine(skillQ_CT);
    }

    public void OpenFirstAtk()
    {
        firstQAtk = true;
    }
    public void OpenEndAtk()
    {
        endQAtk = true;
    }

    void SetQ_CT()
    {
        Debug.Log("偵測中");
        if (firstQAtk)
        {
            firstQAtk = false;
            Collider[] tmpEnemy = Physics.OverlapBox(transform.localPosition + transform.forward * 3.5f, new Vector3(4.7f, 2f, 13), transform.rotation, aniScript.canAtkMask);
            if (tmpEnemy != null)
            {
                foreach (var target in tmpEnemy)
                {
                    isDead who = target.GetComponent<isDead>();
                    if (who != null)
                    {
                        switch (who.myAttributes)
                        {
                            case GameManager.NowTarget.Player:
                                target.transform.forward = -transform.forward.normalized;
                                target.GetComponent<PhotonView>().RPC("takeDamage", PhotonTargets.All, 4.5f, Vector3.zero, false);
                                target.GetComponent<PhotonView>().RPC("pushOtherTarget", PhotonTargets.All);
                                break;
                            case GameManager.NowTarget.Soldier:
                                //catchObj.GetComponent<PhotonView>().RPC("takeDamage", PhotonTargets.All, playerScript.Net.viewID, 2.5f);
                                break;
                            case GameManager.NowTarget.Tower:
                                break;
                            case GameManager.NowTarget.Core:
                                target.transform.forward = -transform.forward.normalized;
                                break;
                            case GameManager.NowTarget.NoChange:
                                return;
                            default:
                                break;
                        }
                    }
                }
            }
        }

        if (endQAtk)
        {
            endQAtk = false;
            Collider[] tmpEnemy = Physics.OverlapSphere(transform.localPosition + transform.forward * 2.2f, 5.5f, aniScript.canAtkMask);
            if (tmpEnemy != null)
            {
                foreach (var target in tmpEnemy)
                {
                    isDead who = target.GetComponent<isDead>();
                    if (who != null)
                    {
                        switch (who.myAttributes)
                        {
                            case GameManager.NowTarget.Player:
                                target.transform.forward = -transform.forward.normalized;
                                target.GetComponent<PhotonView>().RPC("takeDamage", PhotonTargets.All, 9f, Vector3.zero, true);
                                target.GetComponent<PhotonView>().RPC("pushOtherTarget", PhotonTargets.All);
                                break;
                            case GameManager.NowTarget.Soldier:
                                //catchObj.GetComponent<PhotonView>().RPC("takeDamage", PhotonTargets.All, playerScript.Net.viewID, 2.5f);
                                break;
                            case GameManager.NowTarget.Tower:
                                break;
                            case GameManager.NowTarget.Core:
                                target.transform.forward = -transform.forward.normalized;
                                break;
                            case GameManager.NowTarget.NoChange:
                                return;
                            default:
                                break;
                        }
                    }
                }
                OpenDetect(false);
            }
        }
    }

    //清除重置Q
    public override void ResetQ_GoCD()
    {
        if (photonView.isMine)
            StartCoroutine(playerScript.MatchTimeManager.SetCountDown(playerScript.CountDown_Q, playerScript.playerData.skillCD_Q, SkillIconManager.skillContainer[0].nowTime, SkillIconManager.skillContainer[0].cdBar));
        else
            OpenDetect(false);

        firstQAtk = false;
        endQAtk = false;
    }
    //直接恢復cd(中斷,或死亡用)
    public override void ClearQ_Skill()
    {
        playerScript.CountDown_Q();

        if (!photonView.isMine)
            OpenDetect(false);
        firstQAtk = false;
        endQAtk = false;
    }

    #endregion

    #region W技能
    public void W_Skill()
    {
        //設定攻擊範圍
        allSkillRange.transform.position = mySkillPos;
        ProjectorManager.Setsize(allSkillRange, 5.2f, 1, true);

    }

    public void Go_W_Skill()
    {
        GameObject aaa = Instantiate(testObj, allSkillRange.transform.position, allSkillRange.transform.rotation);
        Destroy(aaa, 3f);

        if (photonView.isMine)
            return;

        Collider[] tmpEnemy = Physics.OverlapSphere(allSkillRange.transform.position, 5.1f, aniScript.canAtkMask);
        if (tmpEnemy != null)
        {
            foreach (var target in tmpEnemy)
            {
                isDead who = target.GetComponent<isDead>();
                if (who != null)
                {
                    switch (who.myAttributes)
                    {
                        case GameManager.NowTarget.Player:
                            target.GetComponent<PhotonView>().RPC("takeDamage", PhotonTargets.All, 8f, Vector3.zero, false);
                            target.GetComponent<PhotonView>().RPC("GetDeBuff_Stun", PhotonTargets.All, 1.8f);
                            target.GetComponent<PhotonView>().RPC("HitFlayUp", PhotonTargets.All);                            
                            break;
                        case GameManager.NowTarget.Soldier:
                            //catchObj.GetComponent<PhotonView>().RPC("takeDamage", PhotonTargets.All, playerScript.Net.viewID, 2.5f);
                            break;
                        case GameManager.NowTarget.Tower:
                            break;
                        case GameManager.NowTarget.Core:
                            target.transform.forward = -transform.forward.normalized;
                            break;
                        case GameManager.NowTarget.NoChange:
                            return;
                        default:
                            break;
                    }
                }
            }
        }
    }

    //清除重置W
    public override void ResetW_GoCD()
    {
        allSkillRange.enabled = false;

        if (photonView.isMine)
            StartCoroutine(playerScript.MatchTimeManager.SetCountDown(playerScript.CountDown_W, playerScript.playerData.skillCD_W, SkillIconManager.skillContainer[1].nowTime, SkillIconManager.skillContainer[1].cdBar));
    }
    //直接恢復cd(中斷,或死亡用)
    public override void ClearW_Skill()
    {
        allSkillRange.enabled = false;
        playerScript.CountDown_W();
    }
    #endregion

    #region 關閉技能提示
    public override void CancelDetectSkill(Player.SkillData _nowSkill)
    {
        switch (_nowSkill)
        {
            case Player.SkillData.skill_Q:
                ProjectorManager.SwitchPorjector(projector_Q, false);
                break;
            case Player.SkillData.skill_W:
                ProjectorManager.SwitchPorjector(projector_W, false);
                break;
            case Player.SkillData.skill_R:
                //ProjectorManager.SwitchPorjector(projector_R, false);
                break;
            default:
                if (projector_Q[0].enabled)
                    ProjectorManager.SwitchPorjector(projector_Q, false);
                if (projector_W[0].enabled)
                    ProjectorManager.SwitchPorjector(projector_W, false);
                break;
        }
    }
    #endregion

    #region 中斷技能
    public override void InterruptSkill()
    {
        //前搖之後
        if (!brfore_shaking)
        {
            switch (nowSkill)
            {
                case SkillAction.is_Q:
                    ResetQ_GoCD();
                    break;
                case SkillAction.is_W:
                    ResetW_GoCD();
                    break;
                case SkillAction.is_R:
                    ResetR_GoCD();
                    break;
                default:
                    break;
            }
        }
        else
        {
            switch (nowSkill)
            {
                case SkillAction.is_Q:
                    ClearQ_Skill();
                    break;
                case SkillAction.is_W:
                    ClearW_Skill();
                    break;
                case SkillAction.is_R:
                    ClearR_Skill();
                    break;
                default:
                    break;
            }
        }

        playerScript.deadManager.notFeedBack = false;
        nowSkill = SkillAction.None;
        brfore_shaking = true;
    }
    #endregion
}
