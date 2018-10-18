using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Fire_Soldier : EnemyControl
{
    private Tweener flyUp;
    private LinkedList<Collider> alreadytakeDamage = new LinkedList<Collider>();
    public GameObject testFireEffect;

    #region 取得動畫雜湊值
    protected override void SetAniHash()
    {
        base.SetAniHash();
        aniHashValue[3] = Animator.StringToHash("Base Layer.Idle");
        aniHashValue[4] = Animator.StringToHash("Base Layer.DeBuff.HitFly");
        aniHashValue[5] = Animator.StringToHash("Base Layer.attack");
        aniHashValue[6] = Animator.StringToHash("AtkStop");
    }
    #endregion

    protected override void AtkDetectSet()
    {
        //checkBox (4 1 7.5)
        enemiesCon = Physics.OverlapBox(sword_1.position, checkEnemyBox, Quaternion.identity, currentMask);
        if (enemiesCon.Length != 0)
            giveCurrentDamage();
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
            Invoke("GoWaitMove", waitNextActionTime);
        }
    }

    [PunRPC]
    public void getAtkAnimator()
    {
        if (!deadManager.checkDead && !NowCC)
        {
            haveHit = true;
            ani.SetBool(aniHashValue[6], false);
            //噴火特效開啟
            testFireEffect.SetActive(true);
            ani.CrossFade(aniHashValue[5], 0.01f, 0);
        }
    }

    [PunRPC]
    public void StopAtkAnimator()
    {
        canAtking = true;
        haveHit = false;
        //噴火特效關閉
        testFireEffect.SetActive(false);
        ani.SetBool(aniHashValue[6], true);    
    }
    #endregion

    protected override void StopWait()
    {
        if (deadManager.checkDead)
            return;

        tmpNowDis_F = Vector3.Distance(myCachedTransform.position, currentTarget.transform.position);


        //超過攻擊範圍時 →OverAtkDis=True
        if (!points.IFDis(enemyData.atk_Range, tmpNowDis_F - enemyData.stoppingDst))
        {
            Net.RPC("StopAtkAnimator", PhotonTargets.All);
            points.RemoveThisPoint(correctPos);
            nowState = states.AtkWait;
        }
        else //沒超過攻擊範圍 士兵自動換點
        {
            rotToTarget();            

            if (lastTmpPos != correctPos.position)
            {
                tmpPos = points.GoComparing(enemyData.atk_Range, myCachedTransform, correctPos, enemyData.width);
                if (tmpPos != null)
                {
                    points.RemoveThisPoint(correctPos);
                    correctPos = tmpPos;
                    points.AddPoint(correctPos);
                }
                lastTmpPos = correctPos.position;
            }
        }
    }

    #region 給與正確目標傷害
    protected override void giveCurrentDamage()
    {
        resetChaseTime();

        for (int i = 0; i < enemiesCon.Length; i++)
        {
            if (alreadytakeDamage.Contains(enemiesCon[i]))
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
                        atkNet.RPC("takeDamage", PhotonTargets.All, enemyData.atk_Damage, (atkNet.transform.position - myCachedTransform.position).normalized, true);
                        break;
                    case GameManager.NowTarget.Soldier:
                        atkNet.RPC("takeDamage", PhotonTargets.All, Net.viewID, enemyData.atk_Damage);
                        break;
                    case GameManager.NowTarget.Tower:
                        atkNet.RPC("takeDamage", PhotonTargets.All, enemyData.atk_Damage * 2);
                        break;
                    case GameManager.NowTarget.Core:
                        break;
                    default:
                        break;
                }
                alreadytakeDamage.AddLast(enemiesCon[i]);
                Invoke("NextTimeCanBeAtk", enemyData.atk_delay);
            }
        }
    }
    #endregion

    void NextTimeCanBeAtk()
    {
        if (alreadytakeDamage.Count != 0)
            alreadytakeDamage.RemoveFirst();
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
            StopAtkAnimator();
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
            StopAtkAnimator();
            NowCC = true;
            ani.CrossFade(aniHashValue[4], 0.01f, 0);
            if (photonView.isMine)
            {
                if (correctPos != null)
                    points.RemoveThisPoint(correctPos);

                myCachedTransform.DOMove(myCachedTransform.localPosition + (_dir.normalized * -15f), .65f);
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
            StopAtkAnimator();
            flyUp = myCachedTransform.DOMoveY(myCachedTransform.position.y + 5.5f, 0.27f).SetAutoKill(false).SetEase(Ease.OutBack);
            flyUp.onComplete = delegate () { EndFlyUp(); };
            if (!NowCC)
            {
                GetDeBuff_Stun(1.5f);
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

    protected override void OtherSoldierNeedCancel()
    {
        Net.RPC("StopAtkAnimator", PhotonTargets.All);
    }
}
