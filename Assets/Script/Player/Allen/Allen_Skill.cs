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
    [SerializeField] MeshRenderer handSmall;
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

        if (catchObj == null)
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
                            catchObj.SendMessage("GetDeBuff_Stun");
                            break;
                        case GameManager.NowTarget.Soldier:
                            break;
                        case GameManager.NowTarget.Tower:
                            break;
                        case GameManager.NowTarget.Core:
                            break;
                        default:
                            break;
                    }
                }
                else
                {
                    grabSkill.PlayBackwards();
                    isForward = false;
                }
            }
        }
        else
        {
            if (!photonView.isMine)
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
        
        isForward = false;
        catchObj = null;
        chain.enabled = false;
        handSmall.enabled = true;
        handBig.enabled = false;
        grab_MovePos.position = grab_StartPos.position;
    }
    #endregion
}
