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
    IEnumerator skillQ_CT;
    //E技能
    [SerializeField] Transform e_DetectPos;
    IEnumerator skillE_CT;

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
   /* private void OnDrawGizmos()
    {
        //E的範圍//84度
        Gizmos.DrawWireCube(e_DetectPos.position, new Vector3(12, 1, 8));
        //W的範圍
        //Gizmos.DrawWireSphere(transform.position, 5);
        //Q的範圍
        // Gizmos.DrawWireSphere(transform.localPosition + new Vector3(0, 0, 2.2f), 5.5f);
        //  Gizmos.DrawWireCube(transform.localPosition + transform.forward * 3.5f, new Vector3(4.7f, 2f, 13));
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
        ProjectorManager.Setsize(projector_W[0], 20, 1,true);
        ProjectorManager.Setsize(projector_W[1], 5, 1, true);
    }
    public override void In_Skill_W()
    {
        ProjectorManager.ChangePos(projector_W[1], projector_W[0].transform, playerScript.MousePosition, 17.5f);
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
        ProjectorManager.ChangePos(projector_W[1], projector_W[0].transform, playerScript.MousePosition, 48.75f);
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
        //第一次刷
        if (firstQAtk)
        {
            firstQAtk = false;
            Collider[] tmpEnemy = Physics.OverlapBox(transform.localPosition + transform.forward * 3.5f, new Vector3(2.35f, 1f, 8f), transform.rotation, aniScript.canAtkMask);
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
        //第二次攻擊
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
                                target.GetComponent<PhotonView>().RPC("HitFlayUp", PhotonTargets.All);
                                target.GetComponent<PhotonView>().RPC("takeDamage", PhotonTargets.All, 9f, Vector3.zero, false);
                                
                                //target.GetComponent<PhotonView>().RPC("pushOtherTarget", PhotonTargets.All);
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
        if (!photonView.isMine)
        {
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
                                target.GetComponent<PhotonView>().RPC("HitFlayUp", PhotonTargets.All);
                                target.GetComponent<PhotonView>().RPC("takeDamage", PhotonTargets.All, 8f, Vector3.zero, false);
                                target.GetComponent<PhotonView>().RPC("GetDeBuff_Stun", PhotonTargets.All, 1.8f);
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

        GameObject aaa = Instantiate(testObj, allSkillRange.transform.position, allSkillRange.transform.rotation);
        Destroy(aaa, 3f);
    }
    #endregion

    public Collider[] seeTest;
    #region E技能
    public void E_Skill()
    {
        if (!photonView.isMine)
        {
            skillE_CT = Timer.FirstAction(0.23f, () =>
            {                
                Collider[] tmpEnemy = Physics.OverlapBox(e_DetectPos.position, new Vector3(12, 1, 8), Quaternion.identity, aniScript.canAtkMask);
                Debug.Log("偵測e技能打到誰中");
                seeTest = tmpEnemy;
                if (tmpEnemy != null)
                {
                    foreach (var target in tmpEnemy)
                    {
                        Vector3 dirToTarget = (target.transform.position - transform.position).normalized;
                        if (Vector3.Angle(transform.forward, dirToTarget) > 42)
                            continue;

                        isDead who = target.GetComponent<isDead>();
                        if (who != null)
                        {
                            switch (who.myAttributes)
                            {
                                case GameManager.NowTarget.Player:
                                    target.GetComponent<PhotonView>().RPC("takeDamage", PhotonTargets.All, 2f, dirToTarget, true);
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
        if (photonView.isMine)
        {
            playerScript.TeleportPos(allSkillRange.transform.position);
        }
    }
    #endregion

    #region 重置技能(需跑CD)
    //Q
    public override void ResetQ_GoCD()
    {
        if (photonView.isMine)
            StartCoroutine(playerScript.MatchTimeManager.SetCountDown(playerScript.CountDown_Q, playerScript.playerData.skillCD_Q, SkillIconManager.skillContainer[0].nowTime, SkillIconManager.skillContainer[0].cdBar));
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
            StartCoroutine(playerScript.MatchTimeManager.SetCountDown(playerScript.CountDown_W, playerScript.playerData.skillCD_W, SkillIconManager.skillContainer[1].nowTime, SkillIconManager.skillContainer[1].cdBar));
    }
    //E
    public override void ResetE_GoCD()
    {
        if (photonView.isMine)
            StartCoroutine(playerScript.MatchTimeManager.SetCountDown(playerScript.CountDown_E, playerScript.playerData.skillCD_E, SkillIconManager.skillContainer[2].nowTime, SkillIconManager.skillContainer[2].cdBar));
        else
            End_E_skill();
    }
    //R
    public override void ResetR_GoCD()
    {
        allSkillRange.enabled = false;

        if (photonView.isMine)
            StartCoroutine(playerScript.MatchTimeManager.SetCountDown(playerScript.CountDown_R, playerScript.playerData.skillCD_R, SkillIconManager.skillContainer[3].nowTime, SkillIconManager.skillContainer[3].cdBar));
    }
    #endregion

    #region 直接恢復cd(中斷,或死亡用)
    //Q
    public override void ClearQ_Skill()
    {
        playerScript.CountDown_Q();

        if (!photonView.isMine)
            OpenDetect(false);
        firstQAtk = false;
        endQAtk = false;
    }
    //W
    public override void ClearW_Skill()
    {
        allSkillRange.enabled = false;
        playerScript.CountDown_W();
    }
    //E
    public override void ClearE_Skill()
    {
        playerScript.CountDown_E();

        if (!photonView.isMine)
            End_E_skill();
    }
    //R
    public override void ClearR_Skill()
    {
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
