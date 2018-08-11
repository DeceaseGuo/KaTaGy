//using System.Collections;
//using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Allen_Skill : SkillBase
{
    private Player playerScript;
    private PlayerAni aniScript;
    //抓
    private Tweener grabSkill;
    [Tooltip("lineRenderer鎖鏈")]
    [SerializeField] LineRenderer chain;
    [Tooltip("初始位置")]
    [SerializeField] Transform grab_StartPos;
    [Tooltip("移動所需位置")]
    [SerializeField] Transform grab_MovePos;
    [Tooltip("開始時的手")]
    [SerializeField] SkinnedMeshRenderer handSmall;
    [Tooltip("抓取時的手")]
    [SerializeField] MeshRenderer handBig;
    private bool isForward = false;
    private GameObject catchObj = null;
    //[Tooltip("抓取範圍")]
   // [SerializeField] Projector projector;
    [SerializeField] Material material_Q;
    /// <summary>
    /// ////////////////
    /// </summary>
    [Tooltip("W技能偵測位子")]
    [SerializeField] Transform whirlwindPos;


    //////

    //盾減商協成
    Coroutine shieldCoroutine;
    private bool canShield = false;
    [Tooltip("格檔次數")]
    [SerializeField] int shieldNum = 3;
    /*private void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            ResetAllData_Grab();
        }
    }*/

    private void Start()
    {
        playerScript = GetComponent<Player>();
        aniScript = GetComponent<PlayerAni>();
    }
    //手的抓取範圍
    /* private void OnDrawGizmos()
     {
         Gizmos.DrawWireSphere(grab_MovePos.position, handRadiu);
     }*/

    #region 技能Event
    //Q按下
    public override void Skill_Q_Click()
    {
        //消耗不足
        if (!playerScript.ConsumeAP(1f))
            return;

        playerScript.canSkill_Q = false;
        playerScript.SkillState = Player.SkillData.skill_Q;
        //顯示範圍
        playerScript.projector.enabled = true;
        playerScript.projector.material = material_Q;
    }
    //Q偵測
    public override void In_Skill_Q()
    {        
        if (Input.GetMouseButtonDown(0))
        {
            if (playerScript.ConsumeAP(1f))
            {
                playerScript.SkillState = Player.SkillData.None;
                playerScript.projector.enabled = false;
                //關閉顯示範圍
                
                transform.forward = playerScript.arrow.forward;
                playerScript.stopAnything_Switch(true);
                playerScript.Net.RPC("Skill_Q_Fun", PhotonTargets.All);
                StartCoroutine(playerScript.MatchTimeManager.SetCountDown(playerScript.CountDown_Q, playerScript.playerData.skillCD_Q));
            }
        }
        if (Input.GetMouseButtonDown(1))
        {
            playerScript.canSkill_Q = true;
            playerScript.SkillState = Player.SkillData.None;
            //關閉顯示範圍
            playerScript.projector.enabled = false;
        }
    }
    //W按下
    public override void Skill_W_Click()
    {
        if (playerScript.ConsumeAP(1f))
        {
            playerScript.canSkill_W = false;
            playerScript.SkillState = Player.SkillData.None;
            playerScript.canSkill_W = false;
            playerScript.stopAnything_Switch(true);
            transform.forward = playerScript.arrow.forward;
            playerScript.Net.RPC("Skill_W_Fun", PhotonTargets.All);
            StartCoroutine(playerScript.MatchTimeManager.SetCountDown(playerScript.CountDown_W, playerScript.playerData.skillCD_W));
        }
    }

    //E按下
    public override void Skill_E_Click()
    {
        if (playerScript.ConsumeAP(1f))
        {
            playerScript.canSkill_E = false;
            //playerScript.skillSecondClick = true;
            playerScript.Net.RPC("Skill_E_Fun", PhotonTargets.All);
        }   
    }
    //E偵測
    public override void In_Skill_E()
    {
        if (canShield)
            Shield();
    }

    #endregion

    #region Q抓取
    public void Q_GoGrab()
    {
        if (grabSkill != null)
            grabSkill.Kill();

        isForward = true;
        chain.SetPosition(0, grab_StartPos.position);
        grabSkill = grab_MovePos.DOBlendableMoveBy(grab_MovePos.forward * 22, 0.28f).SetEase(Ease.InOutQuad).SetAutoKill(false).OnUpdate(PushHand);
        grabSkill.OnStart(ChangeHand_start);
        grabSkill.onStepComplete = delegate () { ChangeHand_end(); };
        grabSkill.PlayForward();
    }
    void PushHand()
    {
        if (chain.enabled)
            chain.SetPosition(1, grab_MovePos.position);

        if (catchObj == null && isForward)
        {
            Collider[] enemy = Physics.OverlapBox(grab_MovePos.position, new Vector3(4, 2.5f, 2.2f), Quaternion.identity, aniScript.canAtkMask);
            if (enemy.Length != 0)
            {
                catchObj = enemy[0].gameObject;

                isDead who = catchObj.GetComponent<isDead>();
                if (who != null)
                {
                    switch (who.myAttributes)
                    {
                        case GameManager.NowTarget.Player:
                            if (!photonView.isMine)
                            {
                                catchObj.GetComponent<PhotonView>().RPC("takeDamage", PhotonTargets.All, 5.5f, Vector3.zero, false);
                                catchObj.GetComponent<PhotonView>().RPC("GetDeBuff_Stun", PhotonTargets.All, 1f);
                            }
                            break;
                        case GameManager.NowTarget.Soldier:
                            if (!photonView.isMine)
                                catchObj.GetComponent<PhotonView>().RPC("takeDamage", PhotonTargets.All, playerScript.Net.viewID, 2.5f);
                            break;
                        case GameManager.NowTarget.Tower:
                            break;
                        case GameManager.NowTarget.Core:
                            break;
                        case GameManager.NowTarget.NoChange:
                            playerScript.Net.RPC("HitNull", PhotonTargets.All);
                            break;
                        default:
                            break;
                    }
                    aniScript.anim.SetBool("Catch", true);
                    grabSkill.PlayBackwards();
                    isForward = false;
                }
                else
                {
                    aniScript.anim.SetBool("Catch", true);
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
            print("回收手");
            aniScript.anim.SetBool("Catch", true);
            grabSkill.PlayBackwards();
            isForward = false;
        }
        else
        {
            print("手停止");
            playerScript.GoBack_AtkState();
            ResetAllData_Grab();
        }
    }
    //清除重置Q
    public void ResetAllData_Grab()
    {
        if (grabSkill != null)
            grabSkill.Kill();
        aniScript.anim.SetBool("Catch", false);
        isForward = false;
        catchObj = null;
        chain.enabled = false;
        handSmall.enabled = true;
        handBig.enabled = false;
        grab_MovePos.position = grab_StartPos.position;
    }
    #endregion

    #region W轉 擊飛
    public void W_Whirlwind()
    {
        Debug.Log("w技能");
        //clone體執行
        if (photonView.isMine)
            return;

        Collider[] tmpEnemy = Physics.OverlapBox(whirlwindPos.position, new Vector3(10, 1, 11), Quaternion.identity, aniScript.canAtkMask);
        if (tmpEnemy != null)
        {
            foreach (var target in tmpEnemy)
            {
                isDead who = target.GetComponent<isDead>();
                if (who != null)
                {
                    Vector3 tmpDir = transform.position - target.transform.position;
                    switch (who.myAttributes)
                    {
                        case GameManager.NowTarget.Player:
                            target.transform.forward = tmpDir.normalized;
                            target.GetComponent<PhotonView>().RPC("takeDamage", PhotonTargets.All, 5.5f, Vector3.zero, false);
                            target.GetComponent<PhotonView>().RPC("pushOtherTarget", PhotonTargets.All);
                            break;
                        case GameManager.NowTarget.Soldier:
                            catchObj.GetComponent<PhotonView>().RPC("takeDamage", PhotonTargets.All, playerScript.Net.viewID, 2.5f);
                            break;
                        case GameManager.NowTarget.Tower:
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
    public void ReduceDamage()
    {
        playerScript.stopAnything_Switch(true);
        shieldCoroutine = StartCoroutine(playerScript.MatchTimeManager.SetCountDown(CancelShield, 10f));
        shieldNum = 3;
        canShield = true;
        Debug.Log("技能E  " + "開始減商");
    }

    public void Shield()
    {
        if (shieldNum > 0 && canShield)
        {
            canShield = false;
            playerScript.skillSecondClick = false;
            shieldNum--;
            playerScript.Net.RPC("NowShield", PhotonTargets.All);            
            Debug.Log("技能E  " + "減少次數" + shieldNum);
            if (shieldNum == 0)
            {
                if (shieldCoroutine != null)
                {
                    StopCoroutine(shieldCoroutine);
                    shieldCoroutine = null;
                }
            }
        }
    }

    public void EndShield()
    {
        playerScript.skillSecondClick = true;
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
        shieldNum = 0;
        playerScript.ChangeMyCollider(true);
        canShield = false;
        playerScript.skillSecondClick = false;
        playerScript.GoCountDownE();
    }

    [PunRPC]
    public void NowShield()
    {
        playerScript.ChangeMyCollider(false);
        StartCoroutine(playerScript.MatchTimeManager.SetCountDown(EndShield, 0.8f));
    }
    #endregion
}
