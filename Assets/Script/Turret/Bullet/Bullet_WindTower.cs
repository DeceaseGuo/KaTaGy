using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Bullet_WindTower : BulletManager
{
    [Header("碰撞區")]
    [SerializeField] Vector3 pushBox_Size;
    [SerializeField] Vector3 offset;
    [SerializeField] Vector3 pushBox_Offset;

    [Header("飛行與傷害間隔時間")]
    [SerializeField] float flyTime;
    [SerializeField] float fireCd = 0;
    public List<GameObject> tmpNoDamage;
    protected Tweener myTweener;

    private void OnEnable()
    {
        if (photonView.isMine)
            StartCoroutine("DisappearThis");
    }

    void Update()
    {
        if (!hit)
        {
            hit = true;
            StartCoroutine("GetCollider");
        }
    }

    IEnumerator GetCollider()
    {
        yield return new WaitForSeconds(0.1f);
        Collider[] colliders = Physics.OverlapBox(transform.position + offset, pushBox_Size, Quaternion.identity, atkMask);
        print("抓攻擊對象");
        for (int i = 0; i < colliders.Length; i++)
        {
            targetDead = colliders[i].gameObject.GetComponent<isDead>();
            if (targetDead == null)
                break;

            HitTarget();
        }
        hit = false;
    }

    [PunRPC]
    public override void TP_Data(int _id)
    {
        base.TP_Data(_id);

        BulletMove();
    }

    #region 子彈移動
    protected override void BulletMove()
    {
        #region 移動1
        //子彈移動距離
        /*distanceThisFrame = Data.bullet_Speed * Time.deltaTime;
       //子彈移動
         transform.Translate(dir.normalized * distanceThisFrame, Space.World);
         //子彈朝前
         if (transform.position.y < 5)
         {
             dir.y = 0;
             Quaternion tmpRot = Quaternion.Euler(0, transform.localEulerAngles.y, transform.localEulerAngles.z);
             transform.rotation = Quaternion.Lerp(transform.rotation, tmpRot, .2f);
         }*/
        #endregion
        if (!photonView.isMine)
        {
            print("克龍體有移動喔");
        }
        Vector3 _targetPoint = dir.normalized * Data.bullet_Speed * flyTime;
        _targetPoint.y = target.localPosition.y;
        myTweener = transform.DOBlendableMoveBy(_targetPoint, flyTime + .5f).SetEase(/*Ease.InOutQuart*/ Ease.InOutCubic);
        myTweener.OnUpdate(Reset_Rot);
    }
    #endregion

    public void Reset_Rot()
    {
        Quaternion tmpRot = Quaternion.Euler(0, transform.localEulerAngles.y, transform.localEulerAngles.z);
        transform.rotation = Quaternion.Lerp(transform.rotation, tmpRot, .1f);
    }

    #region 給予傷害
    protected override void GiveDamage(isDead _targetDead)
    {
        if (!checkIf(_targetDead.gameObject))
        {
            base.GiveDamage(_targetDead);

            tmpNoDamage.Add(_targetDead.gameObject);
            StartCoroutine(DelayDamage(_targetDead.gameObject));
        }
    }
    #endregion

    #region 位移
    protected override void MoveTarget()
    {
        if (targetDead.myAttributes != GameManager.NowTarget.Tower && targetDead.myAttributes != GameManager.NowTarget.Core)
            targetDead.transform.localPosition += dir.normalized * /*distanceThisFrame*/1.7f;
    }
    #endregion

    #region 檢查敵人是否在無傷害間隔區 
    bool checkIf(GameObject _enemy)
    {
        if (tmpNoDamage.Contains(_enemy))
            return true;
        else
            return false;
    }
    #endregion

    #region 離開無傷害間隔區
    IEnumerator DelayDamage(GameObject _enemy)
    {
        yield return new WaitForSeconds(fireCd);
        if (checkIf(_enemy))
            tmpNoDamage.Remove(_enemy);
    }
    #endregion

    #region 過一段時間後消失
    IEnumerator DisappearThis()
    {
        yield return new WaitForSeconds(flyTime);
        tmpNoDamage.Clear();
        returnBulletPool();
    }
    #endregion

    void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(transform.position + offset, pushBox_Size);
    }
}
