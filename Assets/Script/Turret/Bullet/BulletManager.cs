using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletManager : Photon.MonoBehaviour {

    public GameManager.whichObject DataName;
    protected PhotonView Net;
    protected Transform target;
    protected TurretData.TowerDataBase Data;
    protected LayerMask atkMask;
    protected bool hit;
    public bool Isfllow = false;
    protected isDead targetDead;

    private void OnEnable()
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
        if (_target == null && photonView.isMine)
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
        print("RPC");
        PhotonView _Photon = PhotonView.Find(_id);
        target = _Photon.gameObject.transform;
        targetDead = target.gameObject.GetComponent<isDead>();
        dir = target.position - transform.position;
        transform.LookAt(target);
    }

    #region 子彈移動
    protected Vector3 dir;
    protected float distanceThisFrame;
    protected virtual void BulletMove()
    {
        if (!targetDead.checkDead && Isfllow)
        {
            dir = target.position - transform.position;
            transform.LookAt(target);
        }
        transform.Translate(dir.normalized * distanceThisFrame, Space.World);        
    }
    #endregion

    #region 擊中目標
    protected virtual void HitTarget()
    {
        if (photonView.isMine)
        {           
            GiveDamage(targetDead);            
        }
        else
        {
            MoveTarget();
        }
    }
    #endregion

    #region 給予傷害
    protected virtual void GiveDamage(isDead _targetDead)
    {
        print("傷害");
        GameManager.NowTarget _who = _targetDead.myAttributes;
        switch (_who)
        {
            case (GameManager.NowTarget.Soldier):
                _targetDead.gameObject.GetComponent<PhotonView>().RPC("takeDamage", PhotonTargets.All, 0, Data.Atk_Damage);
                break;
            case (GameManager.NowTarget.Player):
                _targetDead.gameObject.GetComponent<PhotonView>().RPC("takeDamage", PhotonTargets.All, Data.Atk_Damage, Vector3.zero, false);
                break;
            case (GameManager.NowTarget.Tower):
                _targetDead.gameObject.GetComponent<PhotonView>().RPC("takeDamage", PhotonTargets.All, Data.Atk_Damage);
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
