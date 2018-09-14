using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyCode.Timer;
using DG.Tweening;

public class Mini_Soldier : EnemyControl
{
    private IEnumerator atkCT;
    private Tweener flyUp;

    #region 取得動畫雜湊值
    protected override void SetAniHash()
    {
        base.SetAniHash();
        aniHashValue[3] = Animator.StringToHash("StunRock");
        aniHashValue[4] = Animator.StringToHash("Base Layer.DeBuff.Stun");
        aniHashValue[5] = Animator.StringToHash("Base Layer.DeBuff.HitFly");
    }
    #endregion

    protected override void AtkDetectSet()
    {
        atkCT = Timer.NextFrame(() =>
          {
              if (haveHit)
              {
                  //checkBox (.65 .6 2.7)
                  enemiesCon = Physics.OverlapBox(sword_1.position, checkEnemyBox, Quaternion.identity, currentMask);
                  if (enemiesCon.Length != 0)
                  {
                      atkTarget = enemiesCon[0].gameObject.GetComponent<isDead>();
                      if (!atkTarget.checkDead)
                      {
                          giveCurrentDamage();
                          changeCanHit(0);
                      }
                  }
              }
          });
    }

    #region 小兵攻擊
    protected override IEnumerator enemyAttack()
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
        if (!deadManager.checkDead && !NowCC)
            ani.CrossFade("attack", 0.01f, 0);

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
        switch (atkTarget.myAttributes)
        {
            case GameManager.NowTarget.Player:
                atkTarget.GetComponent<PhotonView>().RPC("takeDamage", PhotonTargets.All, enemyData.atk_Damage, Vector3.zero, false);
                break;
            case GameManager.NowTarget.Soldier:
                atkTarget.GetComponent<PhotonView>().RPC("takeDamage", PhotonTargets.All, Net.viewID, enemyData.atk_Damage);
                break;
            case GameManager.NowTarget.Tower:
                atkTarget.GetComponent<PhotonView>().RPC("takeDamage", PhotonTargets.All, 8.5f);
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
            StopAll();
            NowCC = true;
            ani.CrossFade(aniHashValue[4], 0.01f, 0);
            ani.SetBool(aniHashValue[3], true);

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
    //擊退
    [PunRPC]
    protected override void pushOtherTarget(Vector3 _dir)
    {
        if (!deadManager.checkDead)
        {
            StopAll();
            NowCC = true;
            ani.CrossFade(aniHashValue[5], 0.01f, 0);
            if (photonView.isMine)
            {
                transform.DOMove(transform.localPosition + (_dir.normalized * -15f), .65f);
                transform.rotation = Quaternion.LookRotation(_dir.normalized);
            }
        }
    }

    //往上擊飛
    [PunRPC]
    protected override void HitFlayUp()
    {
        flyUp = transform.DOMoveY(transform.position.y + 5.5f, 0.27f).SetAutoKill(false).SetEase(Ease.OutBack);
        flyUp.onComplete = delegate () { EndFlyUp(); };
        if (!NowCC)
        {
            GetDeBuff_Stun(1.2f);
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
