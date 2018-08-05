//using System.Collections;
//using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Allen_Skill : Photon.MonoBehaviour
{
    [SerializeField] Player playerScript;
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

    //暫時
    public LayerMask testMask;

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

        // if (!photonView.isMine)
        {
            if (catchObj == null)
            {
                Collider[] enemy = Physics.OverlapSphere(grab_MovePos.position, handRadiu, testMask);
                if (enemy.Length != 0)
                {
                    catchObj = enemy[0].gameObject;
                    // catchObj.GetComponent<PhotonTransformView>().enabled = false;
                    catchObj.SendMessage("GetDeBuff_Stun");
                    print("抓到");
                    grabSkill.PlayBackwards();
                    isForward = false;
                }
            }
            else
            {
                if (!photonView.isMine)
                    catchObj.transform.position = new Vector3(grab_MovePos.transform.position.x, catchObj.transform.position.y, grab_MovePos.transform.position.z);
            }
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
