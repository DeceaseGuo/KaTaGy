using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletManager : Photon.MonoBehaviour {

    public GameManager.whichObject DataName;
    protected PhotonView Net;
    //private Transform target;
    protected TurretData.TowerDataBase Data;
    protected LayerMask atkMask;
    protected bool hit;
    public bool Isfllow = false;
    protected isDead targetDead;

    protected virtual void OnEnable()
    {      
        if (Data.objectName == null)
        {
            //print("初始");
            Data = TurretData.instance.getTowerData(DataName);
            Net = GetComponent<PhotonView>();
            atkMask = GameManager.instance.correctMask(photonView.isMine);
        }       
        distanceThisFrame = Data.bullet_Speed * Time.deltaTime;   
    }

    #region 取得目標，外部使用
    public void getTarget(Transform _target)
    {
        hit = false;
        if (_target == null)
        {
            print("沒有目標");
            returnBulletPool();
            return;
        }
        int viewID = _target.GetComponent<PhotonView>().viewID;
        Net.RPC("TP_Data", PhotonTargets.All, viewID);
    }
    #endregion

    [PunRPC]
    public virtual void TP_Data(int _id)
    {
        PhotonView _Photon = PhotonView.Find(_id);
        targetDead = _Photon.gameObject.GetComponent<isDead>();

        targetPos = targetDead.transform.position;
        targetPos.y += targetOffsetY;
        dir = targetPos - transform.position;
    }

    #region 子彈移動
    protected Vector3 targetPos;
    protected Vector3 dir;
    [SerializeField] float targetOffsetY;
    protected float distanceThisFrame;
    protected virtual void BulletMove()
    {
        if (Isfllow)
        {
            targetPos = targetDead.transform.position;
            targetPos.y = targetDead.transform.position.y + targetOffsetY;
            dir = targetPos - transform.position;
            transform.LookAt(targetPos);
        }
        
        transform.Translate(dir.normalized * distanceThisFrame, Space.World);
    }
    #endregion

    #region 給予傷害
    protected virtual void GiveDamage()
    {
        //print("傷害");
        GameManager.NowTarget _who = targetDead.myAttributes;

        switch (_who)
        {
            case (GameManager.NowTarget.Soldier):
                targetDead.gameObject.GetComponent<PhotonView>().RPC("takeDamage", PhotonTargets.All, 0, Data.Atk_Damage);
                break;
            case (GameManager.NowTarget.Player):
                targetDead.gameObject.GetComponent<PhotonView>().RPC("takeDamage", PhotonTargets.All, Data.Atk_Damage, Vector3.zero, false);
                break;
            case (GameManager.NowTarget.Tower):
                targetDead.gameObject.GetComponent<PhotonView>().RPC("takeDamage", PhotonTargets.All, Data.Atk_Damage);
                break;
            case (GameManager.NowTarget.Core):
                break;
            default:
                return;
        }
    }
    #endregion

    #region 位移
    protected virtual void MoveTarget()
    {
        print("位移囉");
    }
    #endregion

    #region 返回物件池
    protected void returnBulletPool()
    {
        if (photonView.isMine)
        {
            ObjectPooler.instance.Repool(Data.bullet_Name, this.gameObject);
        }
    }
    #endregion
}
