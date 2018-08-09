//using System.Collections;
//using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Allen_Skill : Photon.MonoBehaviour
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
    [Tooltip("W技能偵測位子")]
    [SerializeField] Transform whirlwindPos;

    //盾減商協成
    Coroutine shieldCoroutine;
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
                                catchObj.GetComponent<PhotonView>().RPC("GetDeBuff_Stun", PhotonTargets.All, .8f);
                            }
                            break;
                        case GameManager.NowTarget.Soldier:
                            //  catchObj.SendMessage("GetDeBuff_Stun", 1f);
                            if (!photonView.isMine)
                                catchObj.GetComponent<PhotonView>().RPC("takeDamage", PhotonTargets.All, playerScript.Net.viewID, 2.5f);
                            break;
                        case GameManager.NowTarget.Tower:
                            break;
                        case GameManager.NowTarget.Core:
                            break;
                        case GameManager.NowTarget.NoChange:
                            //傳送一個值給無傷害
                            catchObj = null;
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
                            //傳送一個值給無傷害
                            break;
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
        playerScript.StopAllOnlyDodge();
        shieldCoroutine = StartCoroutine(playerScript.MatchTimeManager.SetCountDown(CancelShield, 10f));
        shieldNum = 3;
        Debug.Log("技能E  " + "開始減商");
    }

    public void Shield()
    {
        if (shieldNum > 0)
        {
            playerScript.StopAllOnlyDodge();
            transform.forward = playerScript.arrow.forward;
            playerScript.shieldCollider.enabled = true;
            playerScript.skillSecondClick = false;
            shieldNum--;
            playerScript.Net.RPC("NowShield", PhotonTargets.All);
            Debug.Log("技能E  " + "減少次數" + shieldNum);
            if (shieldNum == 0)
                CancelShield();
        }
    }

    public void EndShield()
    {
        playerScript.skillSecondClick = true;
        playerScript.shieldCollider.enabled = false;
    }

    public void CancelShield()
    {
        Debug.Log("技能E  " + "結束");
        if (shieldCoroutine != null)
        {
            StopCoroutine(shieldCoroutine);
            shieldCoroutine = null;
        }
        playerScript.skillSecondClick = false;
        playerScript.GoCountDownE();

    }

    [PunRPC]
    public void NowShield()
    {
        aniScript.anim.SetTrigger("Shield");
    }
    #endregion
}
