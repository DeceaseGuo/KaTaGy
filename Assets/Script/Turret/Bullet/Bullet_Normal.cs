using UnityEngine;

public class Bullet_Normal : BulletManager
{
    void Update()
    {
        if (targetDead.checkDead && hit)
        {
            returnBulletPool();
            //print("目標已死亡");
            return;
        }
        BulletMove();
        if (Vector3.SqrMagnitude(dir) <= distanceThisFrame * distanceThisFrame)
        {
            //print("擊中");
            hit = true;
            returnBulletPool();

            if (photonView.isMine)
                GiveDamage();
        }
    }
}
