using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet_Normal : Photon.MonoBehaviour
{
    [SerializeField] GameManager.whichObject DataName;
    private Transform target;
    private TurretData.TowerDataBase Data;
    bool hit;

    private void Start()
    {
        if (Data.objectName == null)
            Data = TurretData.instance.getTowerData(DataName);
    }
    isDead targetDead;
    private void Update()
    {
        if (target != null)
        {
            if (photonView.isMine && targetDead.checkDead)
            {
                returnBulletPool();
                print("目標已死亡");
                return;
            }

            Vector3 dir = target.position - transform.position;
            float distanceThisFrame = Data.bullet_Speed * Time.deltaTime;

            if (photonView.isMine && dir.magnitude <= distanceThisFrame && !hit)
            {
                HitTarget();
                print("擊中目標");
                return;
            }

            transform.Translate(dir.normalized * distanceThisFrame, Space.World);
            transform.LookAt(target);
        }
        else
        {
            returnBulletPool();
        }
    }

    #region 取得目標
    public void getTarget(Transform _target)
    {
        hit = false;
        int viewID = _target.GetComponent<PhotonView>().viewID;
        targetDead = _target.GetComponent<isDead>();
        GetComponent<PhotonView>().RPC("TP_Data", PhotonTargets.All, viewID);
    }
    #endregion

    [PunRPC]
    public void TP_Data(int _id)
    {
        PhotonView _Photon = PhotonView.Find(_id);
        target = _Photon.gameObject.transform;
        //targetDead = _Photon.transform.GetComponent<isDead>();
    }

    void HitTarget()
    {
        hit = true;
        if (target != null)
        {
            GiveDamage(targetDead.myAttributes);
        }           
        returnBulletPool();
    }

    void GiveDamage(GameManager.NowTarget _who)
    {
        switch (_who)
        {
            case (GameManager.NowTarget.Soldier):
                target.gameObject.GetComponent<PhotonView>().RPC("takeDamage", PhotonTargets.All, 0, CalculatorDamage());
                break;
            case (GameManager.NowTarget.Player):
                target.gameObject.GetComponent<PhotonView>().RPC("takeDamage", PhotonTargets.All, CalculatorDamage(), Vector3.zero, false);
                break;
            case (GameManager.NowTarget.Tower):
                target.gameObject.GetComponent<PhotonView>().RPC("takeDamage", PhotonTargets.All, CalculatorDamage());
                break;
            case (GameManager.NowTarget.Core):
                break;
            default:
                return;
        }
    }

    //計算傷害
    float CalculatorDamage()
    {
        return Data.Atk_Damage;
    }

    //返回物件池
    void returnBulletPool()
    {
        if (photonView.isMine)
        {
            ObjectPooler.instance.Repool(Data.bullet_Name, this.gameObject);
        }
    }
}
