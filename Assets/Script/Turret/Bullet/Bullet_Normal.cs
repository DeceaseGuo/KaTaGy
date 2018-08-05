using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet_Normal : Photon.MonoBehaviour
{
    [SerializeField] GameManager.whichObject DataName;
    private Transform target;
    private TurretData.TowerDataBase Data;
    bool hit;

    private void OnEnable()
    {
        if (Data.objectName == null)
            Data = TurretData.instance.getTowerData(DataName);
    }

    private void Update()
    {
        if (target != null)
        {
            Vector3 dir = target.position - transform.position;
            float distanceThisFrame = Data.bullet_Speed * Time.deltaTime;


            if (photonView.isMine && dir.magnitude <= distanceThisFrame && !hit)
            {
                HitTarget();
                return;
            }


            transform.Translate(dir.normalized * distanceThisFrame, Space.World);
            transform.LookAt(target);
        }
        else
        {
            if (photonView.isMine)
                GetComponent<PhotonView>().RPC("returnBulletPool", PhotonTargets.All);
        }
    }

    #region 取得目標
    public void getTarget(Transform _target)
    {
        hit = false;
        int viewID = _target.GetComponent<PhotonView>().viewID;
        GetComponent<PhotonView>().RPC("TP_Data", PhotonTargets.All, viewID);
    }
    #endregion

    [PunRPC]
    public void TP_Data(int _id)
    {
        PhotonView _Photon = PhotonView.Find(_id);
        target = _Photon.gameObject.transform;
    }

    void HitTarget()
    {
        hit = true;
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
                target.gameObject.GetComponent<PhotonView>().RPC("takeDamage", PhotonTargets.All, CalculatorDamage(), transform.forward, false);
                break;
            case (GameManager.NowTarget.Tower):
                target.gameObject.GetComponent<PhotonView>().RPC("takeDamage", PhotonTargets.All, CalculatorDamage());
                break;
            case (GameManager.NowTarget.Core):
                break;
            default:
                return;
        }
        if (photonView.isMine)
            GetComponent<PhotonView>().RPC("returnBulletPool", PhotonTargets.All);
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
        else
            GetComponent<PhotonView>().RPC("SetActiveF", PhotonTargets.All);
    }
}
