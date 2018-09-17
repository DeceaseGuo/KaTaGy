using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyCode.Timer;

public class SiegeSoldier : EnemyControl
{
    private IEnumerator atkCT;
    private bool changeState;
    private byte nowAtkIndex = 0;

    #region 取得動畫雜湊值
    protected override void SetAniHash()
    {
        base.SetAniHash();
        //激勵
        aniHashValue[3] = 0;
        aniHashValue[4] = Animator.StringToHash("Base Layer.DeBuffOrBuff.hit");
        aniHashValue[5] = Animator.StringToHash("Base Layer.ATK.attack1");
        aniHashValue[6] = Animator.StringToHash("Base Layer.ATK.attack2");
        aniHashValue[7] = Animator.StringToHash("Base Layer.ATK.attack3");
    }
    #endregion

    protected override void AtkDetectSet()
    {
        atkCT = Timer.NextFrame(() =>
        {
            if (haveHit)
            {
                enemiesCon = Physics.OverlapBox(sword_1.position, checkEnemyBox, Quaternion.identity, currentMask);
                if (enemiesCon.Length != 0)
                    giveCurrentDamage();
            }
        });
    }

    #region 小兵攻擊
    protected override IEnumerator enemyAttack()
    {
        if (nowState == states.Atk)
        {            
            nowAtkIndex++;
            deadManager.notFeedBack = true;
            firstAtk = true;

            //轉向目標
            rotToTarget();
            resetChaseTime();
            canAtking = false;
            Net.RPC("getAtkAnimator", PhotonTargets.All, nowAtkIndex);
            delayTimeToAtk();
            yield return new WaitForSeconds(waitNextActionTime);
            nowState = states.Wait_Move;

            OverAtkDis = false;
            sotpWait_time = StartCoroutine(stopWait());
        }
    }

    [PunRPC]
    public void getAtkAnimator(byte _index)
    {
        if (!deadManager.checkDead && !NowCC)
        {
            switch (_index)
            {
                case (1):
                    ani.CrossFade(aniHashValue[5], 0.01f, 0);
                    break;
                case (2):
                    ani.CrossFade(aniHashValue[6], 0.01f, 0);
                    break;
                case (3):
                    ani.CrossFade(aniHashValue[7], 0.01f, 0);
                    nowAtkIndex = 0;
                    break;
            }
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
                    alreadytakeDamage.Clear();
                }
            }
            else
            {
                haveHit = true;
            }
        }
    }
    #endregion

    #region 給與正確目標傷害
    protected override void giveCurrentDamage()
    {
        for (int i = 0; i < enemiesCon.Length; i++)
        {
            if (alreadytakeDamage.Contains(enemiesCon[i].gameObject))
                continue;

            atkTarget = enemiesCon[i].GetComponent<isDead>();
            if (!atkTarget.checkDead)
            {
                atkNet = atkTarget.GetComponent<PhotonView>();
                switch (atkTarget.myAttributes)
                {
                    case GameManager.NowTarget.Null:
                        break;
                    case GameManager.NowTarget.Player:
                        atkNet.RPC("takeDamage", PhotonTargets.All, enemyData.atk_Damage, (atkNet.transform.position-transform.position).normalized, true);
                        break;
                    case GameManager.NowTarget.Soldier:
                        atkNet.RPC("takeDamage", PhotonTargets.All, Net.viewID, enemyData.atk_Damage);
                        break;
                    case GameManager.NowTarget.Tower:
                        atkNet.RPC("takeDamage", PhotonTargets.All, enemyData.atk_Damage * 2);
                        break;
                    case GameManager.NowTarget.Core:
                        break;
                    case GameManager.NowTarget.NoChange:
                        break;
                    default:
                        break;
                }
                alreadytakeDamage.Add(atkNet.gameObject);
            }
        }
    }
    #endregion

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

    #region 打擊感功能
    protected override void Feedback()
    {
        if (!deadManager.notFeedBack)
        {
            if (!deadManager.checkDead && !NowCC)
                ani.SetTrigger(aniHashValue[2]);
        }
    }
    #endregion

    #region 負面效果
    //暈眩
    [PunRPC]
    protected override void GetDeBuff_Stun(float _time)
    {
        if (!deadManager.checkDead)
        {
            NowCC = true;
            StopAll();
            ani.CrossFade(aniHashValue[4], 0.01f, 0);
            if (photonView.isMine)
            {
                if (correctPos != null)
                    points.RemoveThisPoint(correctPos);
            }

            StartCoroutine(MatchTimeManager.SetCountDown(Recover_Stun, _time));
        }
    }
    //緩速
    protected override void GetDeBuff_Slow()
    {

    }
    //破甲
    protected override void GetDeBuff_DestoryDef()
    {

    }
    //燒傷
    protected override void GetDeBuff_Burn()
    {

    }
    #endregion
}
