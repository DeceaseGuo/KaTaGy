using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet_Normal : BulletManager
{
    void Update()
    {
        if (photonView.isMine)
        {
            if (targetDead.checkDead)
            {
                returnBulletPool();
                print("目標已死亡");
                return;
            }

            if (dir.magnitude <= distanceThisFrame && !hit)
            {
                print("擊中");
                hit = true;
                GiveDamage();
                returnBulletPool();
            }
        }

        BulletMove();
    }
}
