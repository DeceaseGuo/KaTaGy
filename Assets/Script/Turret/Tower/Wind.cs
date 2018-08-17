using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AtkTower;

public class Wind : Turret_Manager
{
    #region 攻擊
    /*protected override void Tower_shoot()
    {
        base.Tower_shoot();
        GameObject bulletObj = ObjectPooler.instance.getPoolObject(turretData.bullet_Name, Pos_attack.position, Pos_attack.rotation);
        Bullet_WindTower bullet = bulletObj.GetComponent<Bullet_WindTower>();
        bullet.getTarget(target);
    }*/
    #endregion

    #region 計算傷害
    protected override float CalculatorDamage(float _damage)
    {
        //暫時
        return _damage;
    }
    #endregion
}
