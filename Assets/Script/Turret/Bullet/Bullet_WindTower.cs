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
    Collider[] colliders;

    [Header("飛行與傷害間隔時間")]
    [SerializeField] float flyTime;
    [SerializeField] float fireCd = 0.08f;
    [SerializeField] float moveDis = 1.7f;
    private List<GameObject> tmpNoDamage = new List<GameObject>();
    Tweener myTweener;

    protected void OnEnable()
    {
        if (photonView.isMine)
            StartCoroutine(DisappearThis());

        //StartCoroutine("GetCollider");
    }
   
    void Hit()
    {
        colliders = Physics.OverlapBox(transform.position + offset, pushBox_Size, Quaternion.identity, atkMask);
        //print("抓攻擊對象");
        for (int i = 0; i < colliders.Length; i++)
        {
            targetDead = colliders[i].gameObject.GetComponent<isDead>();
            if (targetDead == null || targetDead.checkDead)
                continue;

            if (!checkIf(colliders[i].gameObject))
            {
                GiveDamage();
                tmpNoDamage.Add(targetDead.gameObject);
                StartCoroutine(DelayDamage(targetDead.gameObject));
            }

            MoveTarget();
        }
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
        targetPos = dir.normalized * bullet_Speed * flyTime;
        targetPos.y = targetDead.transform.localPosition.y;
        myTweener = transform.DOBlendableMoveBy(targetPos, flyTime + .5f).SetEase(/*Ease.InOutQuart*/ Ease.InOutCubic);
        myTweener.OnUpdate(Reset_Rot);
        if (!photonView.isMine)
        {
            myTweener.OnUpdate(Hit);
        }
    }
    #endregion

    Quaternion tmpRot;
    public void Reset_Rot()
    {
        tmpRot = Quaternion.Euler(0, transform.localEulerAngles.y, transform.localEulerAngles.z);
        transform.rotation = Quaternion.Lerp(transform.rotation, tmpRot, .1f);
    }

    #region 位移
    protected override void MoveTarget()
    {
        if (targetDead.myAttributes != GameManager.NowTarget.Tower && targetDead.myAttributes != GameManager.NowTarget.Core)
            targetDead.transform.localPosition += dir.normalized * moveDis;
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
