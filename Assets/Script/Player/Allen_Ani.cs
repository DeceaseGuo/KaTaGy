using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Allen_Ani : PlayerAni
{
    #region Combo

    #region 按下判斷
    public override void TypeCombo(Vector3 atkDir)
    {
        if (canClick)
        {
            if (/*(anim.GetCurrentAnimatorStateInfo(0).IsName("Run_Atk") || anim.GetCurrentAnimatorStateInfo(0).IsName("Idle_Atk") ||
                anim.GetCurrentAnimatorStateInfo(0).IsName("dodge")) &&*/ comboIndex == 0)
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

    #region 動畫播放間判定
    public override void comboCheck(int _n)
    {
        switch (_n)
        {
            //預測點
            case (0):
                if (!photonView.isMine)
                    return;
                canClick = true;
                redressOpen = true;
                anim.SetBool("Action", false);
                ChangeNowDir();
                break;
            //結束點
            case (2):
                if (!anim.GetBool("Action"))
                {
                    anim.SetTrigger("ExitCombo");
                    GoBackIdle_canMove();
                    SwitchAtkRange(8);
                }
                break;
            //前搖點
            case (3):
                if (!photonView.isMine)
                    return;
                brfore_shaking = true;
                if (!nextComboBool)
                    canClick = true;
                if (anim.GetBool("Action"))
                    anim.SetBool("Action", false);
                break;
            //後搖點
            case (4):
                if (!photonView.isMine)
                    return;
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

    #endregion

    #region 傷害判定
    public override void DetectAtkRanage()
    {
        base.DetectAtkRanage();

        if (startDetect_1)
        {
          ProduceCheckBox(weapon_Detect, new Vector3(1.7f, 4.5f, .85f));
        }
        if (startDetect_2)
        {
            ProduceCheckBox(weapon_Detect, new Vector3(4f, 4f, 2f));
        }
    }

    void ProduceCheckBox(Transform _pos , Vector3 _size)
    {
        Collider[] checkBox = Physics.OverlapBox(_pos.position, _size, _pos.rotation, canAtkMask);
        GetCurrentTarget(checkBox);
    }
    #endregion

    #region 給予正確目標傷害
    protected override void GetCurrentTarget(Collider[] _enemies)
    {
        if (!photonView.isMine || anim.GetCurrentAnimatorStateInfo(0).IsName("dodge"))
            return;

        foreach (Collider beAtk_Obj in _enemies)
        {
            if (checkIf(beAtk_Obj.gameObject))
                return;

            isDead checkTag = beAtk_Obj.GetComponent<isDead>();
            PhotonView Net = beAtk_Obj.GetComponent<PhotonView>();
            switch (checkTag.myAttributes)
            {
                case (GameManager.NowTarget.Soldier):
                    if (startDetect_1)
                    {
                        Net.RPC("takeDamage", PhotonTargets.All, player.Net.viewID, 2.5f);
                    }
                    if (startDetect_2)
                    {
                        Net.RPC("takeDamage", PhotonTargets.All, player.Net.viewID, 6.0f);
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
            alreadyDamage.Add(beAtk_Obj.gameObject);
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
                if (photonView.isMine)
                    StartCoroutine(cameraControl.CameraShake(.2f, .35f));
                break;
            //刀光1
            case (3):
                swordLight[0].SetActive(true);
                break;
            //刀光2
            case (4):
                swordLight[1].SetActive(true);
                break;
            //刀光3
            case (5):
                swordLight[2].SetActive(true);
                break;

            default:
                startDetect_1 = false;
                startDetect_2 = false;               
                swordLight[0].SetActive(false);
                swordLight[1].SetActive(false);
                swordLight[2].SetActive(false);
                if (photonView.isMine)
                    alreadyDamage.Clear();
                break;
        }
    }
    #endregion
}
