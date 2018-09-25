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
    [SerializeField] Projector projector_E;

    [Tooltip("技能圖")]
    public List<Sprite> mySkillIcon;

    //Q技能
    private bool firstQAtk;
    private bool endQAtk;
    [SerializeField] Transform q_DetectPos;
    IEnumerator skillQ_CT;
    //E技能
    IEnumerator skillE_CT;

    //(持續偵測的) 0 是技能Q  1是技能E 
    private Vector3[] checkEnemyBox = new Vector3[2];     

    private void Start()
    {
        if (photonView.isMine)
        {
            allSkillRange = GameObject.Find("AllSkillRange_G").GetComponent<Projector>();
            SkillIconManager.SetSkillIcon(mySkillIcon);
        }
        else
        {
            checkEnemyBox[0] = new Vector3(3.1f, 1.5f, 4f);
            checkEnemyBox[1] = new Vector3(12, 1, 8);
            allSkillRange = GameObject.Find("AllSkillRange_R").GetComponent<Projector>();
            skillQ_CT = Timer.NextFrame(SetQ_CT);
        }
    }

    [SerializeField] GameObject testObj;
  /*  private void OnDrawGizmos()
    {
        //E的範圍//84度
       // Gizmos.DrawWireCube(e_DetectPos.position, new Vector3(12, 1, 8));
        //W的範圍
        //Gizmos.DrawWireSphere(transform.position, 7);
        //Q的範圍
         //Gizmos.DrawWireSphere(transform.localPosition + transform.forward * 2.2f, 9);
       // Gizmos.DrawWireCube(q_DetectPos.position, new Vector3(6.2f, 3f, 8f));
    }*/

    #region 技能Event
    //Q按下&&偵測
    public override void Skill_Q_Click()
    {
        //消耗不足
        if (!playerScript.ConsumeAP(skillQ_needAP, false))
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
            if (playerScript.ConsumeAP(skillQ_needAP, true))
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
        if (!playerScript.ConsumeAP(skillW_needAP, false))
            return;

        playerScript.canSkill_W = false;
        playerScript.SkillState = Player.SkillData.skill_W;
        //顯示範圍
        ProjectorManager.Setsize(projector_W[0], 26, 1,true);
        ProjectorManager.Setsize(projector_W[1], 7.2f, 1, true);
    }
    public override void In_Skill_W()
    {
        ProjectorManager.ChangePos(projector_W[1], projector_W[0].transform, playerScript.GetNowMousePoint(), 22.4f);
        if (Input.GetMouseButtonDown(0))
        {
            if (playerScript.ConsumeAP(skillW_needAP, true))
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
    {
        //消耗不足
        if (!playerScript.ConsumeAP(skillE_needAP, false))
            return;

        playerScript.canSkill_E = false;
        playerScript.SkillState = Player.SkillData.skill_E;
        //顯示範圍
        projector_E.enabled = true;
        //ProjectorManager.SwitchPorjector(projector_E, true);
    }
    public override void In_Skill_E()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (playerScript.ConsumeAP(skillE_needAP, true))
            {
                playerScript.SkillState = Player.SkillData.None;
                //關閉顯示範圍
                projector_E.enabled = false;
               // ProjectorManager.SwitchPorjector(projector_E, false);

                transform.forward = playerScript.arrow.forward;
                playerScript.stopAnything_Switch(true);
                playerScript.Net.RPC("Skill_E_Fun", PhotonTargets.All);
            }
        }
        if (Input.GetMouseButtonDown(1))
        {
            playerScript.CancelNowSkill();
        }
    }

    //R按下&&偵測
    public override void Skill_R_Click()
    {
        //消耗不足
        if (!playerScript.ConsumeAP(skillR_needAP, false))
            return;

        playerScript.canSkill_R = false;
        playerScript.SkillState = Player.SkillData.skill_R;
        //顯示範圍
        ProjectorManager.Setsize(projector_W[0], 50, 1, true);
        ProjectorManager.Setsize(projector_W[1], 2.5f, 1, true);
    }
    public override void In_Skill_R()
    {
        ProjectorManager.ChangePos(projector_W[1], projector_W[0].transform, playerScript.GetNowMousePoint(), 48.75f);
        if (Input.GetMouseButtonDown(0))
        {
            if (playerScript.ConsumeAP(skillR_needAP, true))
            {
                playerScript.SkillState = Player.SkillData.None;
                //開啟攻擊範圍
                playerScript.Net.RPC("GetSkillPos", PhotonTargets.All, projector_W[1].transform.position);
                //關閉顯示範圍
                ProjectorManager.SwitchPorjector(projector_W, false);

                transform.forward = playerScript.arrow.forward;
                playerScript.stopAnything_Switch(true);
                playerScript.Net.RPC("Skill_R_Fun", PhotonTargets.All);
            }
        }
        if (Input.GetMouseButtonDown(1))
        {
            playerScript.CancelNowSkill();
        }
    }
    #endregion

    #region Q技能
    public void Q_Skill()
    {
        if (!playerScript.deadManager.noCC)
            playerScript.deadManager.noCC = true;

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

    #region 開關偵測範圍
    public void OpenFirstAtk()
    {
        if (!photonView.isMine)
            firstQAtk = true;
    }
    public void CloseFirstAtk()
    {
        if (!photonView.isMine)
        {
            firstQAtk = false;
            alreadyDamage.Clear();
        }
    }
    public void OpenEndAtk()
    {
        if (!photonView.isMine)
            endQAtk = true;
    }
    #endregion

    public List<Collider> alreadyDamage;
    void SetQ_CT()
    {
        //第一次刷
        if (firstQAtk && !endQAtk)
        {
            tmpEnemy = Physics.OverlapBox(q_DetectPos.position, checkEnemyBox[0], Quaternion.identity, aniScript.canAtkMask);
            if (tmpEnemy.Length != 0)
            {
                targetAmount = tmpEnemy.Length;
                for (int i = 0; i < targetAmount; i++)
                {
                    if (alreadyDamage.Contains(tmpEnemy[i]))
                        continue;

                    who = tmpEnemy[i].GetComponent<isDead>();
                    if (who != null)
                    {
                        Net = tmpEnemy[i].GetComponent<PhotonView>();
                        switch (who.myAttributes)
                        {
                            case GameManager.NowTarget.Player:
                                if (!who.noCC)
                                    tmpEnemy[i].transform.forward = -transform.forward.normalized;
                                Net.RPC("takeDamage", PhotonTargets.All, 4.5f, Vector3.zero, false);
                                Net.RPC("pushOtherTarget", PhotonTargets.All);
                                break;
                            case GameManager.NowTarget.Soldier:
                                if (!who.noCC)
                                    Net.RPC("pushOtherTarget", PhotonTargets.All, -transform.forward.normalized);
                                Net.RPC("takeDamage", PhotonTargets.All, playerScript.Net.viewID, 4f);
                                break;
                            case GameManager.NowTarget.Tower:
                                Net.RPC("takeDamage", PhotonTargets.All, 4.5f);
                                break;
                            case GameManager.NowTarget.Core:
                                break;
                            case GameManager.NowTarget.NoChange:
                                return;
                            default:
                                break;
                        }
                    }
                    alreadyDamage.Add(tmpEnemy[i]);
                }            
            }
        }
        //第二次攻擊
        if (endQAtk)
        {
            endQAtk = false;
            tmpEnemy = Physics.OverlapSphere(transform.localPosition + transform.forward * 2.2f, 9, aniScript.canAtkMask);
            if (tmpEnemy.Length != 0)
            {
                targetAmount = tmpEnemy.Length;
                for (int i = 0; i < targetAmount; i++)
                {
                    who = tmpEnemy[i].GetComponent<isDead>();
                    if (who != null)
                    {
                        Net = tmpEnemy[i].GetComponent<PhotonView>();
                        switch (who.myAttributes)
                        {
                            case GameManager.NowTarget.Player:
                                if (!who.noCC)
                                    tmpEnemy[i].transform.forward = -transform.forward.normalized;
                                Net.RPC("HitFlayUp", PhotonTargets.All);
                                Net.RPC("takeDamage", PhotonTargets.All, 9f, Vector3.zero, false);
                                break;
                            case GameManager.NowTarget.Soldier:
                                if (!who.noCC)
                                    Net.RPC("HitFlayUp", PhotonTargets.All);
                                Net.RPC("takeDamage", PhotonTargets.All, playerScript.Net.viewID, 4f);
                                break;
                            case GameManager.NowTarget.Tower:
                                Net.RPC("takeDamage", PhotonTargets.All, 5.5f);
                                break;
                            case GameManager.NowTarget.Core:
                                break;
                            case GameManager.NowTarget.NoChange:
                                return;
                            default:
                                break;
                        }
                    }
                }
                alreadyDamage.Clear();
                OpenDetect(false);
            }
        }
    }
    #endregion

    #region W技能
    public void W_Skill()
    {
        //設定攻擊範圍
        allSkillRange.transform.position = mySkillPos;
        ProjectorManager.Setsize(allSkillRange, 7.2f, 1, true);

    }

    public void Go_W_Skill()
    {
        if (!photonView.isMine)
        {
            tmpEnemy = Physics.OverlapSphere(allSkillRange.transform.position, 7f, aniScript.canAtkMask);
            if (tmpEnemy.Length != 0)
            {
                targetAmount = tmpEnemy.Length;
                for (int i = 0; i < targetAmount; i++)
                {
                    who = tmpEnemy[i].GetComponent<isDead>();
                    if (who != null)
                    {
                        Net = tmpEnemy[i].GetComponent<PhotonView>();
                        switch (who.myAttributes)
                        {
                            case GameManager.NowTarget.Player:
                                Net.RPC("HitFlayUp", PhotonTargets.All);
                                Net.RPC("takeDamage", PhotonTargets.All, 8f, Vector3.zero, false);
                                Net.RPC("GetDeBuff_Stun", PhotonTargets.All, 1.8f);
                                break;
                            case GameManager.NowTarget.Soldier:
                                Net.RPC("GetDeBuff_Stun", PhotonTargets.All, 1.8f);
                                if (!who.noCC)
                                    Net.RPC("HitFlayUp", PhotonTargets.All);
                                Net.RPC("takeDamage", PhotonTargets.All, playerScript.Net.viewID, 4f);
                                break;
                            case GameManager.NowTarget.Tower:
                                Net.RPC("takeDamage", PhotonTargets.All, 5f);
                                break;
                            case GameManager.NowTarget.Core:
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

        GameObject aaa = Instantiate(testObj, allSkillRange.transform.position, allSkillRange.transform.rotation);
        Destroy(aaa, 1.5f);
    }
    #endregion

    #region E技能
    public void E_Skill()
    {
        if (!photonView.isMine)
        {
            skillE_CT = Timer.FirstAction(0.23f, () =>
            {
                tmpEnemy = Physics.OverlapBox(transform.localPosition + transform.forward * 10f, checkEnemyBox[1], Quaternion.identity, aniScript.canAtkMask);
                if (tmpEnemy.Length != 0)
                {
                    targetAmount = tmpEnemy.Length;
                    for (int i = 0; i < targetAmount; i++)
                    {
                        dirToTarget = (tmpEnemy[i].transform.position - transform.position).normalized;
                        if (Vector3.Angle(transform.forward, dirToTarget) > 42)
                            continue;

                        who = tmpEnemy[i].GetComponent<isDead>();
                        if (who != null)
                        {
                            Net = tmpEnemy[i].GetComponent<PhotonView>();
                            switch (who.myAttributes)
                            {
                                case GameManager.NowTarget.Player:
                                    Net.RPC("takeDamage", PhotonTargets.All, 2f, dirToTarget, true);
                                    break;
                                case GameManager.NowTarget.Soldier:
                                    Net.RPC("takeDamage", PhotonTargets.All, playerScript.Net.viewID, 2.5f);
                                    break;
                                case GameManager.NowTarget.Tower:
                                    Net.RPC("takeDamage", PhotonTargets.All, 3f);
                                    break;
                                case GameManager.NowTarget.Core:
                                    break;
                                case GameManager.NowTarget.NoChange:
                                    return;
                                default:
                                    break;
                            }
                        }
                    }
                }
            });
            Debug.Log("開始計算傷害");
            StartCoroutine(skillE_CT);
        }
    }

    public void End_E_skill()
    {
        if (skillE_CT != null)
        {
            StopCoroutine(skillE_CT);
            skillE_CT = null;
        }
    }


    #endregion 

    #region R技能
    public void R_Skill()
    {        
        allSkillRange.transform.position = mySkillPos;
        ProjectorManager.Setsize(allSkillRange, 2.5f, 1, true);
    }

    public void Teleport()
    {
        playerScript.TeleportPos(allSkillRange.transform.position);
    }
    #endregion

    #region 重置技能(需跑CD)
    //Q
    public override void ResetQ_GoCD()
    {
        if (photonView.isMine)
            skillCancelIndex[0] = playerScript.MatchTimeManager.SetCountDown(playerScript.CountDown_Q, playerScript.playerData.skillCD_Q, SkillIconManager.skillContainer[0].nowTime, SkillIconManager.skillContainer[0].cdBar);
        else
            OpenDetect(false);

        firstQAtk = false;
        endQAtk = false;
    }
    //W
    public override void ResetW_GoCD()
    {
        allSkillRange.enabled = false;

          if (photonView.isMine)
            skillCancelIndex [1]= playerScript.MatchTimeManager.SetCountDown(playerScript.CountDown_W, playerScript.playerData.skillCD_W, SkillIconManager.skillContainer[1].nowTime, SkillIconManager.skillContainer[1].cdBar);
    }
    //E
    public override void ResetE_GoCD()
    {
        if (photonView.isMine)
            skillCancelIndex[2] = playerScript.MatchTimeManager.SetCountDown(playerScript.CountDown_E, playerScript.playerData.skillCD_E, SkillIconManager.skillContainer[2].nowTime, SkillIconManager.skillContainer[2].cdBar);
        else
            End_E_skill();
    }
    //R
    public override void ResetR_GoCD()
    {
        allSkillRange.enabled = false;

        if (photonView.isMine)
            skillCancelIndex[3] = playerScript.MatchTimeManager.SetCountDown(playerScript.CountDown_R, playerScript.playerData.skillCD_R, SkillIconManager.skillContainer[3].nowTime, SkillIconManager.skillContainer[3].cdBar);
    }
    #endregion

    #region 直接恢復cd(中斷,或死亡用)
    //Q
    public override void ClearQ_Skill()
    {
        if (!photonView.isMine)
            OpenDetect(false);
        else
        {
            if (skillCancelIndex[0] != 0)
            {
                playerScript.MatchTimeManager.ClearThisTask(skillCancelIndex[0]);
                skillCancelIndex[0] = 0;
            }
            SkillIconManager.ClearSkillCD(0);
        }

        playerScript.CountDown_Q();
        firstQAtk = false;
        endQAtk = false;
    }
    //W
    public override void ClearW_Skill()
    {
        if (photonView.isMine)
        {
            if (skillCancelIndex[1] != 0)
            {
                playerScript.MatchTimeManager.ClearThisTask(skillCancelIndex[1]);
                skillCancelIndex[1] = 0;
            }
            SkillIconManager.ClearSkillCD(1);
        }

        allSkillRange.enabled = false;
        playerScript.CountDown_W();
    }
    //E
    public override void ClearE_Skill()
    {
        if (!photonView.isMine)
            End_E_skill();
        else
        {
            if (skillCancelIndex[2] != 0)
            {
                playerScript.MatchTimeManager.ClearThisTask(skillCancelIndex[2]);
                skillCancelIndex[2] = 0;
            }
            SkillIconManager.ClearSkillCD(2);
        }

        playerScript.CountDown_E();
    }
    //R
    public override void ClearR_Skill()
    {
        if (photonView.isMine)
        {
            if (skillCancelIndex[3] != 0)
            {
                playerScript.MatchTimeManager.ClearThisTask(skillCancelIndex[3]);
                skillCancelIndex[3] = 0;
            }
            SkillIconManager.ClearSkillCD(3);
        }

        allSkillRange.enabled = false;
        playerScript.CountDown_R();
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
                ProjectorManager.SwitchPorjector(projector_W, false);
                break;
            case Player.SkillData.skill_E:
                projector_E.enabled = false;
                break;
            default:
                if (projector_Q[0].enabled)
                    ProjectorManager.SwitchPorjector(projector_Q, false);
                if (projector_W[1].enabled)
                    ProjectorManager.SwitchPorjector(projector_W, false);
                if (projector_E.enabled)
                    projector_E.enabled = false;
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
                case SkillAction.is_E:
                    ResetE_GoCD();
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
                case SkillAction.is_E:
                    ClearE_Skill();
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
