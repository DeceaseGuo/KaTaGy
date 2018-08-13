using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet_Missile : BulletManager
{
    [SerializeField] Collider[] colliders;
    float DamageRange;

    protected override void HitTarget()
    {
        colliders = Physics.OverlapSphere(transform.position, DamageRange, atkMask);
        if (colliders.Length > 0)
        {
            if (photonView.isMine)
            {
                for (int i = 0; i < colliders.Length; i++)
                {
                    targetDead = colliders[i].gameObject.GetComponent<isDead>();
                    base.HitTarget();
                }
            }
            else
            {
                for (int i = 0; i < colliders.Length; i++)
                {
                    MoveTarget();
                }
            }
        }
    }
    
    public void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, Data.Atk_Range);
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, DamageRange);
    }
}
