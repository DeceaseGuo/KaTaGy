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
            if ((anim.GetCurrentAnimatorStateInfo(0).IsName("Run_Atk") || anim.GetCurrentAnimatorStateInfo(0).IsName("Idle_Atk") ||
                anim.GetCurrentAnimatorStateInfo(0).IsName("dodge")) && comboIndex == 0)
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
            if (anim.GetCurrentAnimatorStateInfo(0).IsName("combo4") && comboIndex == 4)
            {
                canClick = false;
                Nextcombo(5);
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
                anim.SetBool("Action", false);
                break;
            //閃避取消
            case (1):
                GoBackIdle_canMove();
                SwitchAtkRange(2);
                break;
            //結束點
            case (2):
                if (!anim.GetBool("Action"))
                {
                    anim.SetTrigger("ExitCombo");
                    GoBackIdle_canMove();
                    SwitchAtkRange(2);
                }
                break;
            //前搖點
            case (3):
                if (anim.GetCurrentAnimatorStateInfo(0).IsName("dodge") || !photonView.isMine)
                    return;

                brfore_shaking = true;
                if (!nextComboBool)
                    canClick = true;
                if (anim.GetBool("Action"))
                    anim.SetBool("Action", false);
                break;
            //後搖點
            case (4):
                if (anim.GetCurrentAnimatorStateInfo(0).IsName("dodge") || !photonView.isMine)
                    return;

                after_shaking = true;
                if (!canClick && nextComboBool && photonView.isMine)
                {
                    //player.Net.RPC("goNextCombo", PhotonTargets.All);
                    goNextCombo();
                    SwitchAtkRange(2);
                }
                break;
            //確保有回到noCombo
            /*case (5):
                if (!photonView.isMine)
                    return;

                if (player.checkCombo)
                {
                    comboIndex = 0;
                    anim.SetInteger("comboIndex", 0);
                    canClick = true;
                    player.stopExceptCombo(false);
                    nextComboBool = false;
                    after_shaking = false;
                    brfore_shaking = false;
                }
                break;*/
            default:
                break;
        }
    }
    #endregion

    #region 角色攻擊位移
    public override void GoMovePos(int _t)
    {
        if (!photonView.isMine)
            return;
        if (GameManager.instance.TTTEEESSSTTT)
        {

            currentAtkDir = player.nowMouseDir();
            player.CharacterRot = Quaternion.LookRotation(currentAtkDir.normalized);
            transform.rotation = player.CharacterRot;
        }

        switch (anim.GetInteger("comboIndex"))
        {
            case (1):
                GoMovePoint(currentAtkDir, 7f, .4f, Ease.OutBack);
                break;
            case (2):
                GoMovePoint(currentAtkDir, 8f, .4f, Ease.OutBack);
                break;
            case (3):
                GoMovePoint(currentAtkDir, 3.5f, .45f, Ease.OutBack);
                break;
            case (4):
                GoMovePoint(currentAtkDir, 3.5f, .45f, Ease.OutBack);
                break;
            case (5):
                if (_t == 0)
                {
                    GoMovePoint(currentAtkDir, 3f, .5f, Ease.OutBack);
                }
                else
                {
                    GoBackPoint(currentAtkDir, 5.5f, .5f, Ease.OutBack);
                }
                break;
        }
    }
    #endregion

    #region 傷害判定
    public override void DetectAtkRanage()
    {
        if (!photonView.isMine || anim.GetCurrentAnimatorStateInfo(0).IsName("dodge"))
            return;

        //鐮刀本身
       if (startDetect_1)
        {
            Collider[] enemies = Physics.OverlapBox(weapon_Detect.position, new Vector3(5.0f, 3.2f, 1.2f), weapon_Detect.rotation, canAtkMask);
            GetCurrentTarget(enemies);
        }
       //轉刀
         if (startDetect_2)
        {
            Collider[] enemies = Physics.OverlapBox(weapon_Detect_Hand.position, new Vector3(9.0f, 7.5f, 1.6f), weapon_Detect_Hand.rotation, canAtkMask);
            GetCurrentTarget(enemies);
        }

        /*if (startDetect_3)
        {
            Collider[] enemies = Physics.OverlapBox(weapon_Detect.position, new Vector3(2.5f, 8f, 8f), weapon_Detect.rotation, canAtkMask);
            GetCurrentTarget(enemies);
        }*/
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
                    Net.RPC("takeDamage", PhotonTargets.All, 3.0f, currentAtkDir.normalized, true);
                    Net.RPC("pushOtherTarget", PhotonTargets.All, currentAtkDir.normalized, 3.0f);
                    break;
                case (GameManager.NowTarget.Core):
                    Debug.Log("還沒寫");
                    break;
                default:
                    Debug.Log("錯誤");
                    break;
            }
            if (!checkIf(beAtk_Obj.gameObject))
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
                break;
            //關閉目前傷害判斷並清除陣列
            case (2):
                startDetect_1 = false;
                startDetect_2 = false;
                alreadyDamage.Clear();
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
                break;
        }
    }
    #endregion
}
