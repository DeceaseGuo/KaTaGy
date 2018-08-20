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
            deadManager.notFeedBack = true;
            firstAtk = true;

            nav.enabled = false;
            stopNav.enabled = true;
            //轉向目標
            rotToTarget();

            resetChaseTime();
            canAtking = false;            
            Net.RPC("getAtkAnimator", PhotonTargets.All);

            delayTimeToAtk();
            yield return new WaitForSeconds(1.5f);
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
            ani.SetTrigger("Atk");
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
        Collider[] enemies = Physics.OverlapBox(sword_1.position, new Vector3(.25f, 1.4f, .15f), sword_1.rotation, currentMask);
        foreach (var target in enemies)
        {
            if (!alreadytakeDamage.Contains(target.gameObject))
            {
                if (target.gameObject == currentTarget.gameObject)
                {

                    giveCurrentDamage(targetDeadScript);
                    alreadytakeDamage.Add(target.gameObject);

                }
            }
        }
    }
    #endregion

    #region 給與正確目標傷害
    protected override void giveCurrentDamage(isDead _target)
    {
        if (!photonView.isMine || currentTarget == null || _target == null)
            return;

        switch (_target.myAttributes)
        {
            case GameManager.NowTarget.Player:
                currentTarget.gameObject.GetComponent<PhotonView>().RPC("takeDamage", PhotonTargets.All, enemyData.atk_Damage, Vector3.zero, false);
                //target.gameObject.SendMessage("GetDeBuff_Stun");
                break;
            case GameManager.NowTarget.Soldier:
                currentTarget.gameObject.GetComponent<PhotonView>().RPC("takeDamage", PhotonTargets.All, Net.viewID, enemyData.atk_Damage);
                break;
            case GameManager.NowTarget.Tower:
                currentTarget.gameObject.GetComponent<PhotonView>().RPC("takeDamage", PhotonTargets.All, 8.5f);
                break;
            case GameManager.NowTarget.Core:
                break;
            default:
                break;
        }
    }
    #endregion

    #region 打擊感功能
    protected override void Feedback()
    {
        if (!deadManager.notFeedBack)
        {
            if (!deadManager.checkDead)
                ani.SetTrigger("Hit");
        }
    }
    #endregion
}
