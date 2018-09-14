using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using MyCode.Projector;

public class Allen_Skill : SkillBase
{
    //技能提示
    private Projector allSkillRange;
    
    [Tooltip("抓取範圍")]
    [SerializeField] Projector projector_Q;
    [Tooltip("大絕範圍")]
    [SerializeField] Projector[] projector_R = new Projector[2]; //0攻擊 //1範圍

    [Tooltip("技能圖")]
    public List<Sprite> mySkillIcon;

    //Q抓
    private Tweener grabSkill;
    [Tooltip("lineRenderer鎖鏈")]
    [SerializeField] LineRenderer chain;
    [Tooltip("初始位置")]
    [SerializeField] Transform[] chain_Pos = new Transform[3]; //0鎖鏈開始位置 1鎖鏈跟著位子 //2初始位置
    [Tooltip("移動所需位置")]
    [SerializeField] Transform grab_MovePos;
    [Tooltip("開始時的手")]
    [SerializeField] SkinnedMeshRenderer handSmall;
    [Tooltip("抓取時的手")]
    [SerializeField] MeshRenderer handBig;
    private bool isForward;
    private GameObject catchObj = null;

    //W    
    [Tooltip("W技能偵測位子")]
    [SerializeField] Transform whirlwindPos;

    //E盾減傷協成
    Coroutine shieldCoroutine;
    private bool canShield;
    private bool shieldCanOpen;
    //格檔次數
    private int shieldNum = 0;
    [Tooltip("左上顯示圖")]
    [SerializeField] Sprite iconImg;
    private SkillIcon.MyStates shieldIcon;    

    //R
    [Tooltip("大絕傷害半徑")]
    [SerializeField] float skillR_radius;



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
        }
    }

    private void LateUpdate()
    {
        if (canShield)
        {
            NowCanOpenShield();
        }
    }

    //手的抓取範圍
   /*  private void OnDrawGizmos()
      {
         Gizmos.DrawWireCube(grab_MovePos.position, new Vector3(6.4f, 4f, 4f));
      }*/
    //大絕的範圍

    /*private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.localPosition, skillR_radius);
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
        projector_Q.enabled = true;
    }
    public override void In_Skill_Q()
    {        
        if (Input.GetMouseButtonDown(0))
        {
            if (playerScript.ConsumeAP(skillQ_needAP, true))
            {
                playerScript.SkillState = Player.SkillData.None;
                projector_Q.enabled = false;
                //關閉顯示範圍

                playerScript.stopAnything_Switch(true);
                transform.forward = playerScript.arrow.forward;
                playerScript.Net.RPC("Skill_Q_Fun", PhotonTargets.All);
            }
        }
        if (Input.GetMouseButtonDown(1))
        {
            playerScript.CancelNowSkill();
        }
    }

    //W按下
    public override void Skill_W_Click()
    {
        if (playerScript.ConsumeAP(skillW_needAP, true))
        {
            playerScript.canSkill_W = false;
            playerScript.SkillState = Player.SkillData.None;
            playerScript.canSkill_W = false;
            playerScript.stopAnything_Switch(true);
            transform.forward = playerScript.arrow.forward;
            playerScript.Net.RPC("Skill_W_Fun", PhotonTargets.All);
        }
    }

    //E按下
    public override void Skill_E_Click()
    {
        if (playerScript.ConsumeAP(skillE_needAP, true))
        {
            playerScript.canSkill_E = false;            
            playerScript.Net.RPC("Skill_E_Fun", PhotonTargets.All);
        }   
    }

    //R按下&&偵測
    public override void Skill_R_Click()
    {
        if (!playerScript.ConsumeAP(skillR_needAP, false))
            return;

        playerScript.canSkill_R = false;
        playerScript.SkillState = Player.SkillData.skill_R;        
        //顯示範圍
        ProjectorManager.SwitchPorjector(projector_R, true);
    }
    public override void In_Skill_R()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (playerScript.ConsumeAP(skillR_needAP, true))
            {
                playerScript.SkillState = Player.SkillData.None;
                //開啟攻擊範圍
                playerScript.Net.RPC("GetSkillPos", PhotonTargets.All, projector_R[0].transform.position);
                //關閉顯示範圍
                ProjectorManager.SwitchPorjector(projector_R, false);
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

    #region Q抓取
    public void Q_Skill()
    {
        if (grabSkill != null)
            grabSkill.Kill();

        isForward = true;        
        chain.SetPosition(0, chain_Pos[0].position);
        grabSkill = grab_MovePos.DOBlendableMoveBy(grab_MovePos.forward * 22, 0.28f).SetEase(Ease.InOutQuad).SetAutoKill(false).OnUpdate(PushHand);
        grabSkill.OnStart(ChangeHand_start);
        grabSkill.onStepComplete = delegate () { ChangeHand_end(); };
        grabSkill.PlayForward();
    }
    void PushHand()
    {
        if (chain.enabled)
        {
            chain.SetPosition(0, chain_Pos[0].position);
            chain.SetPosition(1, chain_Pos[1].position);
        }

        if (catchObj == null && isForward)
        {            
            tmpEnemy = Physics.OverlapBox(grab_MovePos.position, new Vector3(3.35f, 1.6f, 1.9f), Quaternion.identity, aniScript.canAtkMask);

            if (tmpEnemy.Length != 0)
            {
                catchObj = tmpEnemy[0].gameObject;

                who = catchObj.GetComponent<isDead>();
                if (who != null)
                {
                    Net = catchObj.GetComponent<PhotonView>();
                    switch (who.myAttributes)
                    {
                        case GameManager.NowTarget.Player:
                            if (!photonView.isMine)
                            {
                                Net.RPC("takeDamage", PhotonTargets.All, 5.5f, Vector3.zero, false);
                                Net.RPC("GetDeBuff_Stun", PhotonTargets.All, 1.5f);
                            }
                            break;
                        case GameManager.NowTarget.Soldier:
                            if (!photonView.isMine)
                            {
                                Net.RPC("GetDeBuff_Stun", PhotonTargets.All, 1.8f);
                                Net.RPC("takeDamage", PhotonTargets.All, playerScript.Net.viewID, 2.5f);
                            }
                            break;
                        case GameManager.NowTarget.Tower:
                            if (!photonView.isMine)
                            {
                                Net.RPC("takeDamage", PhotonTargets.All, 4f);
                            }
                            catchObj = null;
                            break;
                        case GameManager.NowTarget.Core:
                            catchObj = null;
                            break;
                        case GameManager.NowTarget.NoChange:
                            catchObj = null;
                            playerScript.Net.RPC("HitNull", PhotonTargets.All);
                            break;
                        default:
                            break;
                    }
                    aniScript.anim.SetBool(aniScript.aniHashValue[24], true);
                    grabSkill.PlayBackwards();
                    isForward = false;
                }
                else
                {
                    aniScript.anim.SetBool(aniScript.aniHashValue[24], true);
                    grabSkill.PlayBackwards();
                    isForward = false;
                }
            }
        }
        else
        {
            //clone體執行
            if (!photonView.isMine)
                if (catchObj != null)
                    catchObj.transform.position = new Vector3(grab_MovePos.transform.position.x, catchObj.transform.position.y, grab_MovePos.transform.position.z);
        }
    }
    //開始伸手
    void ChangeHand_start()
    {
        chain.enabled = true;
        handSmall.enabled = false;
        handBig.enabled = true;
    }
    //收手
    void ChangeHand_end()
    {
        if (isForward)
        {
            aniScript.anim.SetBool(aniScript.aniHashValue[24], true);
            grabSkill.PlayBackwards();
            isForward = false;
        }
        else
        {
            ResetQ_GoCD();
        }
    }
    #endregion

    #region W轉 擊飛
    public void W_Skill()
    {
        //clone體執行
        if (photonView.isMine)
            return;

        tmpEnemy = Physics.OverlapBox(whirlwindPos.position, new Vector3(10, 1, 11), Quaternion.identity, aniScript.canAtkMask);
        if (tmpEnemy.Length != 0)
        {
            targetAmount = tmpEnemy.Length;
            for (int i = 0; i < targetAmount; i++)
            {
                who = tmpEnemy[i].GetComponent<isDead>();
                if (who != null)
                {
                    Net = tmpEnemy[i].GetComponent<PhotonView>();
                    dirToTarget = transform.position - tmpEnemy[i].transform.position;
                    switch (who.myAttributes)
                    {
                        case GameManager.NowTarget.Player:
                            if (!who.noCC)
                                tmpEnemy[i].transform.forward = dirToTarget.normalized;
                            Net.RPC("takeDamage", PhotonTargets.All, 5.5f, Vector3.zero, false);
                            Net.RPC("pushOtherTarget", PhotonTargets.All);
                            break;
                        case GameManager.NowTarget.Soldier:
                            Net.RPC("pushOtherTarget", PhotonTargets.All, dirToTarget.normalized);
                            Net.RPC("takeDamage", PhotonTargets.All, playerScript.Net.viewID, 2.5f);
                            break;
                        case GameManager.NowTarget.Tower:
                            Net.RPC("takeDamage", PhotonTargets.All, 2.5f);
                            break;
                        case GameManager.NowTarget.Core:
                            break;
                        case GameManager.NowTarget.NoChange:
                            // playerScript.Net.RPC("HitNull", PhotonTargets.All);
                            //ResetAllData_Grab();
                            return;
                        default:
                            break;
                    }
                }
            }
        }
    }
    #endregion

    #region E減傷 盾
    public void E_Skill()
    {
        if (photonView.isMine)
        {
            playerScript.stopAnything_Switch(true);
            shieldNum = 3;
            canShield = true;
            SwitchShieldIcon(true);
            shieldCoroutine = StartCoroutine(playerScript.MatchTimeManager.SetCountDown(CancelShield, 10f, null, shieldIcon.cdBar));
        }
        //特效
    }

    //開盾
    void Shield()
    {
        if (shieldNum > 0 && canShield)
        {
            canShield = false;
            shieldNum--;
            shieldIcon.nowAmount.text = shieldNum.ToString();
            playerScript.Net.RPC("NowShield", PhotonTargets.All);
            if (shieldNum == 0)
            {
                if (shieldCoroutine != null)
                {
                    StopCoroutine(shieldCoroutine);
                    shieldCoroutine = null;
                    SwitchShieldIcon(false);
                }
            }
        }
    }

    public void EndShield()
    {
        playerScript.ChangeMyCollider(true);
        if (shieldNum != 0)
            canShield = true;
        else
            CancelShield();
    }

    public void CancelShield()
    {
        Debug.Log("技能E  " + "結束");
        if (shieldCoroutine != null)
        {
            StopCoroutine(shieldCoroutine);
            shieldCoroutine = null;
        }
        SwitchShieldIcon(false);
        shieldNum = 0;        
        playerScript.ChangeMyCollider(true);
        canShield = false;
        if (photonView.isMine)
        {
            shieldCanOpen = false;
            skillCD_CT[2] = StartCoroutine(playerScript.MatchTimeManager.SetCountDown(playerScript.CountDown_E, playerScript.playerData.skillCD_E, SkillIconManager.skillContainer[2].nowTime, SkillIconManager.skillContainer[2].cdBar));
        }
    }
    #region 盾牌功能
    void NowCanOpenShield()
    {
        if (Input.GetKeyUp(KeyCode.E) && !shieldCanOpen)
        {
            shieldCanOpen = true;
        }

        if (Input.GetKeyDown(KeyCode.E) && shieldCanOpen)
        {
            Shield();
        }
    }

    void SwitchShieldIcon(bool _t)
    {
        if (_t)
        {
            shieldIcon = SkillIconManager.GetNewStateCT();
            shieldIcon.stateImg.sprite = iconImg;
            shieldIcon.nowAmount.text = shieldNum.ToString();
            SkillIconManager.GoHintArea(shieldIcon.statePrefab);
        }
        else
        {
            if (photonView.isMine && shieldIcon != null)
            {
                SkillIconManager.ClearThisCT(shieldIcon.listNum);
            }
        }
    }
    #endregion

    [PunRPC]
    public void NowShield()
    {
        playerScript.ChangeMyCollider(false);
        StartCoroutine(playerScript.MatchTimeManager.SetCountDown(EndShield, 0.7f));
    }
    #endregion

    #region R大絕(開大無敵)
    public void Go_RSkill()
    {
        playerScript.ChangeMyCollider(false);
        //設定攻擊範圍
        allSkillRange.transform.position = mySkillPos;
        ProjectorManager.Setsize(allSkillRange, 17.5f, 1, true);
        //clone體執行
        if (!photonView.isMine)
            StartCoroutine(playerScript.MatchTimeManager.SetCountDown(R_Skill, .9f));
    }

    public void R_Skill()
    {
        tmpEnemy = Physics.OverlapSphere(transform.localPosition, skillR_radius, aniScript.canAtkMask);
        if (tmpEnemy.Length != 0)
         {
            targetAmount = tmpEnemy.Length;
            Vector3 hitPoint = transform.position + new Vector3(0, 0, 4f);
            for (int i = 0; i < targetAmount; i++)
            {
                who = tmpEnemy[i].GetComponent<isDead>();
                if (who != null)
                {
                    hitPoint.y = tmpEnemy[i].transform.position.y;
                    dirToTarget = hitPoint - tmpEnemy[i].transform.position;
                    Net = tmpEnemy[i].GetComponent<PhotonView>();
                    switch (who.myAttributes)
                    {
                        case GameManager.NowTarget.Player:
                            if (!who.noCC)
                                tmpEnemy[i].transform.forward = dirToTarget.normalized;
                            Net.RPC("takeDamage", PhotonTargets.All, 9f, Vector3.zero, false);
                            Net.RPC("pushOtherTarget", PhotonTargets.All);
                            break;
                        case GameManager.NowTarget.Soldier:
                            Net.RPC("pushOtherTarget", PhotonTargets.All, dirToTarget.normalized);
                            Net.RPC("takeDamage", PhotonTargets.All, playerScript.Net.viewID, 9f);
                            break;
                        case GameManager.NowTarget.Tower:
                            Net.RPC("takeDamage", PhotonTargets.All, 9f);
                            break;
                        case GameManager.NowTarget.Core:
                            transform.forward = dirToTarget.normalized;
                            break;
                        case GameManager.NowTarget.NoChange:
                            // playerScript.Net.RPC("HitNull", PhotonTargets.All);
                            //ResetAllData_Grab();
                            return;
                        default:
                            break;
                    }
                }
            }
         }
    }
    #endregion

    #region 重置技能(需跑CD)
    //Q
    public override void ResetQ_GoCD()
    {
        playerScript.GoBack_AtkState();

        if (photonView.isMine)
            skillCD_CT[0] = StartCoroutine(playerScript.MatchTimeManager.SetCountDown(playerScript.CountDown_Q, playerScript.playerData.skillCD_Q, SkillIconManager.skillContainer[0].nowTime, SkillIconManager.skillContainer[0].cdBar));

        if (grabSkill != null)
            grabSkill.Kill();
        aniScript.anim.SetBool(aniScript.aniHashValue[24], false);
        grab_MovePos.position = chain_Pos[2].position;
        isForward = false;
        catchObj = null;
        chain.enabled = false;
        handSmall.enabled = true;
        handBig.enabled = false;
    }
    //W
    public override void ResetW_GoCD()
    {
        if (photonView.isMine)
            skillCD_CT[1] = StartCoroutine(playerScript.MatchTimeManager.SetCountDown(playerScript.CountDown_W, playerScript.playerData.skillCD_W, SkillIconManager.skillContainer[1].nowTime, SkillIconManager.skillContainer[1].cdBar));
    }
    //R
    public override void ResetR_GoCD()
    {
        playerScript.ChangeMyCollider(true);
        allSkillRange.enabled = false;

        if (photonView.isMine)
            skillCD_CT[3] = StartCoroutine(playerScript.MatchTimeManager.SetCountDown(playerScript.CountDown_R, playerScript.playerData.skillCD_R, SkillIconManager.skillContainer[3].nowTime, SkillIconManager.skillContainer[3].cdBar));
    }
    #endregion

    #region 直接恢復cd(中斷,或死亡用)
    //Q
    public override void ClearQ_Skill()
    {
        if (photonView.isMine)
        {
            if (skillCD_CT[0] != null)
                StopCoroutine(skillCD_CT[0]);
            SkillIconManager.ClearSkillCD(0);
        }
        if (grabSkill != null)
            grabSkill.Kill();

        playerScript.CountDown_Q();
        aniScript.anim.SetBool(aniScript.aniHashValue[24], false);
        grab_MovePos.position = chain_Pos[2].position;
        isForward = false;
        catchObj = null;
        chain.enabled = false;
        handSmall.enabled = true;
        handBig.enabled = false;
    }
    //W
    public override void ClearW_Skill()
    {
        if (photonView.isMine)
        {
            if (skillCD_CT[1] != null)
                StopCoroutine(skillCD_CT[1]);
            SkillIconManager.ClearSkillCD(1);
        }

        playerScript.CountDown_W();
    }
    //E
    public override void ClearE_Skill()
    {
        if (photonView.isMine)
        {
            if (skillCD_CT[2] != null)
                StopCoroutine(skillCD_CT[2]);
            SkillIconManager.ClearSkillCD(2);
        }

        SwitchShieldIcon(false);
        shieldNum = 0;
        playerScript.ChangeMyCollider(true);
        canShield = false;
        playerScript.CountDown_E();
    }
    //R
    public override void ClearR_Skill()
    {
        if (photonView.isMine)
        {
            if (skillCD_CT[3] != null)
                StopCoroutine(skillCD_CT[3]);
            SkillIconManager.ClearSkillCD(3);
        }

        allSkillRange.enabled = false;
        playerScript.CountDown_R();
        playerScript.ChangeMyCollider(true);
    }
    #endregion

    #region 關閉技能提示
    public override void CancelDetectSkill(Player.SkillData _nowSkill)
    {
        switch (_nowSkill)
        {
            case Player.SkillData.skill_Q:
                projector_Q.enabled = false;
                break;
            case Player.SkillData.skill_R:
                ProjectorManager.SwitchPorjector(projector_R, false);                
                break;
            default:
                if (projector_Q.enabled)
                    projector_Q.enabled = false;
                if (projector_R[0].enabled)
                    ProjectorManager.SwitchPorjector(projector_R, false);
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
                    if (playerScript.NowCC)
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