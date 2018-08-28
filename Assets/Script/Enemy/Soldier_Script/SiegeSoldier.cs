using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyCode.Timer;

public class SiegeSoldier : EnemyControl
{
    private IEnumerator atkCT;
    private bool changeState;
    private int stunAtk = 0;

    #region 小兵攻擊
    protected override IEnumerator enemyAttack()
    {
        if (nowState == states.Atk)
        {
            stunAtk++;
            deadManager.notFeedBack = true;
            firstAtk = true;

            //轉向目標
            rotToTarget();
            resetChaseTime();
            canAtking = false;
            Net.RPC("getAtkAnimator", PhotonTargets.All);
            delayTimeToAtk();
            yield return new WaitForSeconds(3f);
            nowState = states.Wait_Move;

            OverAtkDis = false;
            shortPos = null;
            sotpWait_time = StartCoroutine(stopWait());
        }
    }

    [PunRPC]
    public void getAtkAnimator()
    {
        if (!deadManager.checkDead)
        {
            if (stunAtk == 3)
            {
                ani.CrossFade("暈擊", 0.01f, 0);
                stunAtk = 0;
            }
            else
                ani.CrossFade("attack", 0.01f, 0);
        }

        if (!photonView.isMine)
            StartCoroutine(atkCT);
    }
    #endregion

    #region 攻擊動畫判定開關
    public override void changeCanHit(int c)
    {
        if (!photonView.isMine)
        {
            if (c == 0)
            {
                if (haveHit)
                {
                    haveHit = false;
                    StopCoroutine(atkCT);
                }
                //alreadytakeDamage.Clear();
            }
            else
            {
                haveHit = true;
            }
        }
    }
    #endregion
    private Collider[] enemies;
    protected override void AtkDetectSet()
    {
        atkCT = Timer.NextFrame(() =>
        {
            if (haveHit)
            {
                enemies = Physics.OverlapBox(sword_1.position, new Vector3(.25f, 1.4f, .15f), sword_1.rotation, currentMask);
                if (enemies.Length != 0)
                {
                    giveCurrentDamage(enemies[0].gameObject.GetComponent<isDead>());
                    changeCanHit(0);
                }
            }
        });
    }

    protected override void MyHelath(float _damage)
    {
        //血量顯示與消失
        UI_HpObj.alpha = 1;
        CloseHP();

        //扣血
        if (enemyData.UI_HP > 0)
        {
            enemyData.UI_HP -= _damage;

            if (enemyData.UI_HP <= enemyData.UI_MaxHp * 0.4f && !changeState)
            {

            }
            if (enemyData.UI_HP > 0)
            {
                if(!changeState && enemyData.UI_HP <= enemyData.UI_MaxHp * 0.4f)
                {
                    changeState = true;
                }
            }
            else
            {
                deadManager.ifDead(true);
                changeState = false;
                Death();
                UI_HpObj.alpha = 0;
            }
            BeHitChangeColor();
            Feedback();
            openPopupObject(_damage);
        }
    }
}
