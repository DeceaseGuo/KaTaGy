using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mini_Soldier : EnemyControl
{
    #region 小兵攻擊
    protected override IEnumerator enemyAttack()
    {
        if (nowState == states.Atk)
        {
            Debug.Log("可攻擊區域");
            nav.avoidancePriority = 10;
            firstAtk = true;

            nav.enabled = false;
            stopNav.enabled = true;
            //轉向目標
            rotToTarget();


            fieldOfView.resetChaseTime();
            //ani.SetTrigger("Atk");
            ani.SetBool("Stop", true);
            canAtking = false;

            Net.RPC("getAtkAnimator", PhotonTargets.All);
            delayToAtk_time = StartCoroutine(delayTimeToAtk(enemyData.atk_delay));
            yield return new WaitForSeconds(2.5f);
            nav.avoidancePriority = 50;
            nowState = states.Wait_Move;

            OverAtkDis = false;
            shortPos = null;
            stopWait_time = StartCoroutine(stopWait());
        }
    }
    #endregion

    #region 攻擊動畫判定開關
    public override void changeCanHit(int c)
    {
        if (c == 0)
        {
            haveHit = false;
            alreadytakeDamage.Clear();
        }
        else
            haveHit = true;
    }
    #endregion

    #region 攻擊是否打中
    protected override void TouchTarget()
    {
        if (haveHit)
        {
            Collider[] enemies = Physics.OverlapBox(sword_1.position, new Vector3(.65f, 3f, .8f), sword_1.rotation, fieldOfView.currentMask);
            foreach (var target in enemies)
            {
                if (!alreadytakeDamage.Contains(target.gameObject))
                {
                    if (target.gameObject == fieldOfView.currentTarget.gameObject)
                    {

                        giveCurrentDamage(fieldOfView.targetDeadScript);
                        alreadytakeDamage.Add(target.gameObject);

                    }
                }
            }
        }
    }
    #endregion

    #region 給與正確目標傷害
    protected override void giveCurrentDamage(isDead _target)
    {
        if (target == null || _target == null)
            return;

        switch (DataName)
        {
            case (GameManager.whichObject.Soldier_1):
                switch (_target.myAttributes)
                {
                    case GameManager.NowTarget.Player:
                        target.gameObject.GetComponent<PhotonView>().RPC("takeDamage", PhotonTargets.All, enemyData.atk_Damage);
                        //target.gameObject.SendMessage("GetDeBuff_Stun");
                        break;
                    case GameManager.NowTarget.Soldier:
                        target.gameObject.GetComponent<PhotonView>().RPC("takeDamage", PhotonTargets.All, GetComponent<PhotonView>().viewID, enemyData.atk_Damage);
                        break;
                    case GameManager.NowTarget.Tower:
                        target.gameObject.GetComponent<PhotonView>().RPC("takeDamage", PhotonTargets.All, 8.5f);
                        break;
                    case GameManager.NowTarget.Core:
                        break;
                    default:
                        break;
                }
                break;
        }
    }
    #endregion
}
