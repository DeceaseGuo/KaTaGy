using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyCode.Timer;
using DG.Tweening;

public class Mini_Soldier : EnemyControl
{
    private IEnumerator atkCT;
    private Tweener flyUp;

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
        if (!deadManager.checkDead && !ani.GetBool("StunRock"))
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
                  enemies = Physics.OverlapBox(sword_1.position, new Vector3(.5f, .6f, 2.4f), Quaternion.identity, currentMask);
                  if (enemies.Length != 0)
                  {
                      giveCurrentDamage(enemies[0].gameObject.GetComponent<isDead>());
                      changeCanHit(0);
                  }
              }
          });
    }

    #region 給與正確目標傷害
    protected override void giveCurrentDamage(isDead _target)
    {
        if (_target.checkDead)
            return;

        switch (_target.myAttributes)
        {
            case GameManager.NowTarget.Player:
                //target.gameObject.SendMessage("GetDeBuff_Stun");
                _target.gameObject.GetComponent<PhotonView>().RPC("takeDamage", PhotonTargets.All, enemyData.atk_Damage, Vector3.zero, false);
                break;
            case GameManager.NowTarget.Soldier:
                _target.gameObject.GetComponent<PhotonView>().RPC("takeDamage", PhotonTargets.All, Net.viewID, enemyData.atk_Damage);
                break;
            case GameManager.NowTarget.Tower:
                _target.gameObject.GetComponent<PhotonView>().RPC("takeDamage", PhotonTargets.All, 8.5f);
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
                ani.SetTrigger("Hit");
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
            // if (photonView.isMine)
            {
                StopAll();

            }
            NowCC = true;
            //if (!ani.GetBool("Die"))
            {
                ani.CrossFade("Stun", 0.01f, 0);
                ani.SetBool("StunRock", true);
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
    //擊退
    [PunRPC]
    protected override void pushOtherTarget(Vector3 _dir, float _dis)
    {
        this.transform.DOMove(transform.localPosition + (_dir.normalized * -_dis), .7f).SetEase(Ease.OutBounce);
        Quaternion Rot = Quaternion.LookRotation(_dir.normalized);
        this.transform.rotation = Rot;
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
