using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Queen_Ani : PlayerAni
{
    #region 按下判斷
    public override void TypeCombo(Vector3 atkDir)
    {
        if (canClick)
        {
            if (comboIndex == 0)
            {
                canClick = false;
                comboFirst(1, atkDir);
            }
            if (anim.GetCurrentAnimatorStateInfo(0).IsName("combo1") && comboIndex == 1)
            {
                canClick = false;
                Nextcombo(2);
            }
            if (anim.GetCurrentAnimatorStateInfo(0).IsName("combo2") && comboIndex == 2)
            {
                canClick = false;
                Nextcombo(3);
            }
            if (anim.GetCurrentAnimatorStateInfo(0).IsName("combo3") && comboIndex == 3)
            {
                canClick = false;
                Nextcombo(4);
            }
        }
    }
    #endregion

    #region Combo動畫播放間判定
    public override void comboCheck(int _n)
    {
        switch (_n)
        {
            //預測點
            case (0):
                if (!photonView.isMine)
                    return;
                canClick = true;
                anim.SetBool("Action", false);
                player.lockDodge = false;
                break;
            //結束點
            case (2):
                if (nextComboBool)
                {
                    goNextCombo();
                    SwitchAtkRange(8);
                }
                else
                {
                    if (!anim.GetBool("Action"))
                    {
                        player.lockDodge = false;
                        anim.SetTrigger("ExitCombo");
                        GoBackIdle_canMove();
                        SwitchAtkRange(8);
                    }
                }
                break;
            //前搖點
            case (3):
                if (!photonView.isMine)
                    return;
                //鎖閃避
                player.lockDodge = true;
                redressOpen = true;
                brfore_shaking = true;
                break;
            //後搖點
            case (4):
                if (!photonView.isMine)
                    return;
                //解閃避
                player.lockDodge = false;
                redressOpen = false;
                after_shaking = true;
                if (!canClick && nextComboBool)
                {
                    goNextCombo();
                    SwitchAtkRange(8);
                }
                break;
            default:
                break;
        }
    }
    #endregion

    #region 傷害判定
    public override void DetectAtkRanage()
    {
        base.DetectAtkRanage();

        //鐮刀本身
       if (startDetect_1)
        {
            ProduceCheckBox(weapon_Detect, new Vector3(3.5f, 1f, 1.5f));
        }
       //轉刀
         if (startDetect_2)
        {
            ProduceCheckBox(weapon_Detect, new Vector3(3.5f, 1f, 3.5f));
        }
    }

    void ProduceCheckBox(Transform _pos, Vector3 _size)
    {
        Collider[] checkBox = Physics.OverlapBox(_pos.position, _size, _pos.rotation, canAtkMask);
        GetCurrentTarget(checkBox);
    }
    #endregion

    #region 給予正確目標傷害
    protected override void GetCurrentTarget(Collider[] _enemies)
    {
        if (!photonView.isMine )
            return;

        for (int i = 0; i < _enemies.Length; i++)
        {
            if (checkIf(_enemies[i].gameObject))
                continue;

            isDead checkTag = _enemies[i].GetComponent<isDead>();
            if (checkTag != null)
            {
                //攻擊無效化
                if (checkTag.myAttributes == GameManager.NowTarget.NoChange)
                {
                    SwitchAtkRange(8);
                    player.Net.RPC("HitNull", PhotonTargets.All);
                    return;
                }
                PhotonView Net = _enemies[i].GetComponent<PhotonView>();
                switch (checkTag.myAttributes)
                {
                    case (GameManager.NowTarget.Soldier):
                        if (startDetect_1)
                        {
                            Net.RPC("takeDamage", PhotonTargets.All, player.Net.viewID, 3.0f);
                        }

                        if (startDetect_2)
                        {
                            Net.RPC("takeDamage", PhotonTargets.All, player.Net.viewID, 3.0f);
                        }
                        break;
                    case (GameManager.NowTarget.Tower):
                        Net.RPC("takeDamage", PhotonTargets.All, 10.0f);
                        break;
                    case (GameManager.NowTarget.Player):
                        if (startDetect_2)
                        {
                            Net.RPC("takeDamage", PhotonTargets.All, 5.5f, currentAtkDir.normalized, true);
                            //Net.RPC("pushOtherTarget", PhotonTargets.All, currentAtkDir.normalized, 6.0f);
                            break;
                        }
                        else
                        {
                            Net.RPC("takeDamage", PhotonTargets.All, 3.0f, currentAtkDir.normalized, true);
                        }
                        break;
                    case (GameManager.NowTarget.Core):
                        Debug.Log("還沒寫");
                        break;
                    default:
                        Debug.Log("錯誤");
                        break;
                }
                alreadyDamage.Add(_enemies[i].gameObject);
            }
        }
    }
    #endregion

    #region 目前傷害判定區及刀光特效
    public override void SwitchAtkRange(int _n)
    {
        switch (_n)
        {
            case (0):
                startDetect_1 = true;
                break;
            case (1):
                startDetect_2 = true;
                break;
            case (2):
                break;
            case (3):

                break;
            //刀光1
            case (4):
               // swordLight[0].SetActive(true);
                break;
            //刀光2
            case (5):
             //   swordLight[1].SetActive(true);
                break;
            //刀光3
            case (6):
               // swordLight[2].SetActive(true);
                break;

            default:
                startDetect_1 = false;
                startDetect_2 = false;
                alreadyDamage.Clear();
                break;
        }
    }
    #endregion
}
