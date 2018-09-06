using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet_Missile : BulletManager
{
    [SerializeField] float DamageRange;
    [SerializeField] float pushDis;
    Collider[] colliders;
    
    protected override void OnEnable()
    {
        base.OnEnable();
        Isfllow = true;
    }

    private void Update()
    {
        if (hit)
            return;

        BulletMove();

        if (targetDead.checkDead && Isfllow)
        {
            Isfllow = false;
        }

        if (dir.magnitude <= distanceThisFrame)
        {
            hit = true;
            colliders = Physics.OverlapSphere(transform.position, DamageRange, atkMask);
            if (colliders.Length > 0)
            {
                if (photonView.isMine)
                {
                    for (int i = 0; i < colliders.Length; i++)
                    {
                        targetDead = colliders[i].gameObject.GetComponent<isDead>();
                        GiveDamage();
                    }
                    returnBulletPool();
                }
                /*else
                {
                    for (int i = 0; i < colliders.Length; i++)
                    {
                        targetDead = colliders[i].gameObject.GetComponent<isDead>();
                        MoveTarget();
                    }
                }*/
            }
        }
    }

    #region 位移
    protected override void MoveTarget()
    {
        if (targetDead.myAttributes != GameManager.NowTarget.Tower && targetDead.myAttributes != GameManager.NowTarget.Core)
        {
            if ((targetDead.transform.position - transform.position) != Vector3.zero)
            {
                dir = targetDead.transform.position - transform.position;
            }
            else
            {
                dir = -(transform.forward);
            }
            targetDead.transform.localPosition += dir.normalized * pushDis;
        }
    }
    #endregion

    public void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, DamageRange);
    }
}
