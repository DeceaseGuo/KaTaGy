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
    [SerializeField] float handRadiu;
    private bool isForward = false;
    private GameObject catchObj = null;

    
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

    #region 抓取技能
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
            Collider[] enemy = Physics.OverlapSphere(grab_MovePos.position, handRadiu, aniScript.canAtkMask);
            if (enemy.Length != 0)
            {
                catchObj = enemy[0].gameObject;
                
                isDead who = catchObj.GetComponent<isDead>();
                if (who != null)
                {
                    switch (who.myAttributes)
                    {
                        case GameManager.NowTarget.Player:
                            catchObj.SendMessage("GetDeBuff_Stun",0.7f);
                            if (!photonView.isMine)
                                catchObj.GetComponent<PhotonView>().RPC("takeDamage", PhotonTargets.All, 5.5f, Vector3.zero, false);
                            break;
                        case GameManager.NowTarget.Soldier:
                            catchObj.SendMessage("GetDeBuff_Stun", 1f);
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
            if (!photonView.isMine)
                if (catchObj != null)
                    catchObj.transform.position = new Vector3(grab_MovePos.transform.position.x, catchObj.transform.position.y, grab_MovePos.transform.position.z);
        }
    }
    void ChangeHand_start()
    {
        chain.enabled = true;
        handSmall.enabled = false;
        handBig.enabled = true;
    }

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
}
