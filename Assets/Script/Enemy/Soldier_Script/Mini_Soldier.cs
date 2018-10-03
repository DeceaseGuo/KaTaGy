using System.Collections;
using UnityEngine;
using DG.Tweening;

public class Mini_Soldier : EnemyControl
{
    private Tweener flyUp;

    #region 取得動畫雜湊值
    protected override void SetAniHash()
    {
        base.SetAniHash();
        aniHashValue[3] = Animator.StringToHash("Base Layer.Idle");
        aniHashValue[4] = Animator.StringToHash("Base Layer.DeBuff.HitFly");
        aniHashValue[5] = Animator.StringToHash("Base Layer.attack");
    }
    #endregion

    protected override void AtkDetectSet()
    {
        //checkBox (.65 .6 2.7)
        enemiesCon = Physics.OverlapBox(sword_1.position, checkEnemyBox, Quaternion.identity, currentMask);
        if (enemiesCon.Length != 0)
        {
            for (int i = 0; i < enemiesCon.Length; i++)
            {
                atkTarget = enemiesCon[i].gameObject.GetComponent<isDead>();
                if (!atkTarget.checkDead)
                {
                    giveCurrentDamage();
                    changeCanHit(0);
                    break;
                }
            }
        }
    }

    #region 小兵攻擊
    protected override void SoldierAttack()
    {
        if (nowState == states.Atk && !NowCC)
        {
            firstAtk = true;

            //轉向目標
            rotToTarget();
            resetChaseTime();
            canAtking = false;

            Net.RPC("getAtkAnimator", PhotonTargets.All);

            delayTimeToAtk();
            Invoke("GoWaitMove", waitNextActionTime);
        }
    }

    [PunRPC]
    public void getAtkAnimator()
    {
        if (!deadManager.checkDead && !NowCC)
            ani.CrossFade(aniHashValue[5], 0.01f, 0);
    }
    #endregion

    #region 攻擊動畫判定開關
    public override void changeCanHit(int c)
    {
        if (photonView.isMine)
            return;

        if (c == 0)
        {
            if (haveHit)
                haveHit = false;
        }
        else
            haveHit = true;
    }
    #endregion

    #region 給與正確目標傷害
    protected override void giveCurrentDamage()
    {
        switch (atkTarget.myAttributes)
        {
            case GameManager.NowTarget.Player:
                atkTarget.GetComponent<PhotonView>().RPC("takeDamage", PhotonTargets.All, enemyData.atk_Damage, Vector3.zero, false);
                break;
            case GameManager.NowTarget.Soldier:
                atkTarget.GetComponent<PhotonView>().RPC("takeDamage", PhotonTargets.All, Net.viewID, enemyData.atk_Damage);
                break;
            case GameManager.NowTarget.Tower:
                atkTarget.GetComponent<PhotonView>().RPC("takeDamage", PhotonTargets.All, enemyData.atk_Damage);
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
            ani.CrossFade(aniHashValue[3], 0.01f, 0);

            if (photonView.isMine)
            {
                if (correctPos != null)
                    points.RemoveThisPoint(correctPos);
            }

            MatchTimeManager.SetCountDownNoCancel(Recover_Stun, _time);
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
    //擊退
    [PunRPC]
    protected override void pushOtherTarget(Vector3 _dir)
    {
        if (!deadManager.checkDead)
        {
            StopAll();
            NowCC = true;
            ani.CrossFade(aniHashValue[4], 0.01f, 0);
            if (photonView.isMine)
            {
                if (correctPos != null)
                    points.RemoveThisPoint(correctPos);

                myCachedTransform.DOMove(myCachedTransform.localPosition + (_dir.normalized * -15f), .85f);
                myCachedTransform.rotation = Quaternion.LookRotation(_dir.normalized);
            }
        }
    }

    //往上擊飛
    [PunRPC]
    protected override void HitFlayUp()
    {
        if (!deadManager.checkDead)
        {
            StopAll();
            flyUp = myCachedTransform.DOMoveY(myCachedTransform.position.y + 5.5f, 0.27f).SetAutoKill(false).SetEase(Ease.OutBack);
            flyUp.onComplete = delegate () { EndFlyUp(); };
            if (!NowCC)
            {
                GetDeBuff_Stun(1.2f);
            }

            if (photonView.isMine)
            {
                if (correctPos != null)
                    points.RemoveThisPoint(correctPos);
            }
        }
    }
    #endregion

    #region 負面狀態恢復
    //回到地上
    void EndFlyUp()
    {
        flyUp.PlayBackwards();
    }
    #endregion
}
