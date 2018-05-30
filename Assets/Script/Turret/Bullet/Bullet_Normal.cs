using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet_Normal : Photon.MonoBehaviour
{
    private Transform target;
    private TurretData.TowerDataBase Data;
    private float bulletSpeed;

    private void OnEnable()
    {
       // if (!photonView.isMine)
          //  this.enabled = false;
    }

    private void Update()
    {
        if (target != null)
        {
            Vector3 dir = target.position - transform.position;
            float distanceThisFrame = bulletSpeed * Time.deltaTime;

            if (dir.magnitude <= distanceThisFrame && photonView.isMine)
            {
                HitTarget();
                return;
            }

            transform.Translate(dir.normalized * distanceThisFrame, Space.World);
            transform.LookAt(target);
        }
        else
            returnBulletPool();
    }

    #region 取得目標
    public void getTarget(Transform _target, TurretData.TowerDataBase _turretData)
    {
        //target = _target;
        int viewID = _target.GetComponent<PhotonView>().viewID;
        Data = _turretData;
        GetComponent<PhotonView>().RPC("TP_Data", PhotonTargets.All, viewID, Data.bullet_Speed);
        //bulletSpeed = Data.bullet_Speed;
    }
    #endregion

    [PunRPC]
    public void TP_Data(int _id, float _speed)
    {
        PhotonView _Photon = PhotonView.Find(_id);

        target = _Photon.gameObject.transform;
        bulletSpeed = _speed;
    }

    void HitTarget()
    {
        if (target != null)
            GiveDamage(target.GetComponent<isDead>().myAttributes);
    }

    void GiveDamage(GameManager.NowTarget _who)
    {
        switch (_who)
        {
            case (GameManager.NowTarget.Soldier):
                target.gameObject.GetComponent<PhotonView>().RPC("takeDamage", PhotonTargets.All, 0, CalculatorDamage());
                break;
            case (GameManager.NowTarget.Player):
                target.gameObject.GetComponent<PhotonView>().RPC("takeDamage", PhotonTargets.All, CalculatorDamage());
                break;
            case (GameManager.NowTarget.Tower):
                target.gameObject.GetComponent<PhotonView>().RPC("takeDamage", PhotonTargets.All, CalculatorDamage());
                break;
            case (GameManager.NowTarget.Core):
                break;
            default:
                return;
        }
        GetComponent<PhotonView>().RPC("returnBulletPool", PhotonTargets.All);
        //returnBulletPool();
    }
    //計算傷害
    float CalculatorDamage()
    {
        return Data.Atk_Damage;
    }
    //返回物件池
    [PunRPC]
    public void returnBulletPool()
    {
        if (photonView.isMine)
            ObjectPooler.instance.Repool(Data.bullet_Name, this.gameObject);
    }
}
