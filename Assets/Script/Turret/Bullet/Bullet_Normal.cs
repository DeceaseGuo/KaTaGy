using UnityEngine;

public class Bullet_Normal : BulletManager
{
    void Update()
    {
        if (targetDead.checkDead)
        {
            returnBulletPool();
            print("目標已死亡");
            return;
        }

        if (Vector3.SqrMagnitude(targetPos - myCachedTransform.position) <= distanceThisFrame * distanceThisFrame && !hit)
        {
            print("擊中");
            hit = true;
            returnBulletPool();

            if (photonView.isMine)
                GiveDamage();
        }

        BulletMove();
    }
}
