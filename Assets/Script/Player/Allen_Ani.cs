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
            if (anim.GetCurrentAnimatorStateInfo(0).IsName("Idle_Atk") && comboIndex == 0)
            {
                player.stopExceptCombo(true);
                comboIndex = 1;
                anim.SetInteger("comboIndex", comboIndex);
                anim.SetTrigger("Combo");
                canClick = false;
                anim.SetBool("isEnd", false);
                currentAtkDir = atkDir.normalized;
                Debug.Log("COMBO0");
            }
            if (anim.GetCurrentAnimatorStateInfo(0).IsName("Run_Atk") && comboIndex == 0)
            {
                player.stopExceptCombo(true);
                comboIndex = 1;
                anim.SetInteger("comboIndex", comboIndex);
                anim.SetTrigger("Combo");
                canClick = false;
                anim.SetBool("isEnd", false);
                currentAtkDir = atkDir.normalized;
                Debug.Log("COMBO0");
            }
            if (anim.GetCurrentAnimatorStateInfo(0).IsName("combo1") && comboIndex == 1)
            {
                comboIndex = 2;
                anim.SetInteger("comboIndex", comboIndex);
                canClick = false;
                anim.SetBool("isEnd", false);
                Debug.Log("COMBO1");
                nextComboBool = true;
            }
            if (anim.GetCurrentAnimatorStateInfo(0).IsName("combo2") && comboIndex == 2)
            {
                comboIndex = 3;
                anim.SetInteger("comboIndex", comboIndex);
                canClick = false;
                anim.SetBool("isEnd", false);
                Debug.Log("COMBO2");
                nextComboBool = true;
            }
            if (anim.GetCurrentAnimatorStateInfo(0).IsName("combo3") && comboIndex == 3)
            {
                comboIndex = 4;
                anim.SetInteger("comboIndex", comboIndex);
                canClick = false;
                anim.SetBool("isEnd", false);
                Debug.Log("COMBO3");
                nextComboBool = true;
            }
        }
    }
    #endregion

    #region 動畫播放間判定
    public override void comboCheck(int _n)
    {
        switch (_n)
        {
            //可以點擊
            case (0):
                canClick = true;
                break;
            //不能點擊
            case (1):
                if (nextComboBool)
                {
                    anim.SetBool("Action", true);
                    nextComboBool = false;
                }
                else
                {
                    anim.SetBool("isEnd", true);
                    canClick = false;
                }
                break;
            //此段combo結束
            case (2):
                if (anim.GetBool("isEnd") == true)
                {
                    anim.SetTrigger("ExitCombo");
                    comboIndex = 0;
                    anim.SetInteger("comboIndex", comboIndex);
                    canClick = true;
                    Ani_Run(false);
                    player.stopExceptCombo(false);
                    anim.SetBool("Action", false);
                    nextComboBool = false;
                }
                break;
            //開頭
            default:
                anim.SetBool("isEnd", false);
                canClick = false;
                Ani_Run(false);
                anim.SetBool("Action", false);
                nextComboBool = false;
                break;
        }
    }
    #endregion

    #region 角色攻擊位移
    public override void GoMovePos()
    {
        Vector3 currentPoint = new Vector3();
        switch (comboIndex)
        {
            case (1):
                currentPoint = DetectAtkDistance(player.gameObject.transform.localPosition + currentAtkDir.normalized * 7);
                currentPoint.y = 0;
                
                player.gameObject.transform.DOMove(currentPoint, .5f).SetEase(Ease.OutBack);
                break;
            case (2):
                currentPoint = DetectAtkDistance(player.gameObject.transform.localPosition + currentAtkDir.normalized * 3.5f);
                currentPoint.y = 0;
                player.gameObject.transform.DOMove(currentPoint, .5f);
                break;
            case (4):
                currentPoint = DetectAtkDistance(player.gameObject.transform.localPosition + currentAtkDir.normalized * 13);
                currentPoint.y = 0;
                player.gameObject.transform.DOMove(currentPoint, .5f);
                break;
        }
    }
    #endregion

    #endregion

    #region 傷害判定
    public override void DetectAtkRanage()
    {
        if (startDetect_1)
        {
            Collider[] enemies = Physics.OverlapBox(weapon_Detect_Hand.position, new Vector3(2.5f, 1.5f, 1.5f), weapon_Detect_Hand.rotation, canAtkMask);
            GetCurrentTarget(enemies);
        }
        if (startDetect_2)
        {
            Collider[] enemies = Physics.OverlapBox(weapon_Detect.position, new Vector3(1.8f, 4.2f, 0.5f), weapon_Detect.rotation, canAtkMask);
            GetCurrentTarget(enemies);
        }

        if (startDetect_3)
        {
            Collider[] enemies = Physics.OverlapBox(weapon_Detect.position, new Vector3(2.5f, 8f, 8f), weapon_Detect.rotation, canAtkMask);
            GetCurrentTarget(enemies);
        }
    }
    #endregion

    #region 給予正確目標傷害
    protected override void GetCurrentTarget(Collider[] _enemies)
    {
        foreach (Collider beAtk_Obj in _enemies)
        {
            if (checkIf(beAtk_Obj.gameObject))
                return;

            isDead checkTag = beAtk_Obj.GetComponent<isDead>();
            switch (checkTag.myAttributes)
            {
                case (GameManager.NowTarget.Soldier):

                    if (startDetect_1)
                    {
                        beAtk_Obj.GetComponent<PhotonView>().RPC("takeDamage", PhotonTargets.All, GetComponent<PhotonView>().viewID, 2.5f);
                    }

                    if (startDetect_2)
                    {
                        beAtk_Obj.GetComponent<PhotonView>().RPC("takeDamage", PhotonTargets.All, GetComponent<PhotonView>().viewID, 3.0f);
                    }

                    if (startDetect_3)
                    {
                        beAtk_Obj.GetComponent<PhotonView>().RPC("takeDamage", PhotonTargets.All, GetComponent<PhotonView>().viewID, 6.0f);
                        Vector3 dir = beAtk_Obj.transform.localPosition - weapon_Detect.position;
                        dir.y = 1f;
                        beAtk_Obj.GetComponent<PhotonView>().RPC("pushOtherTarget", PhotonTargets.All, dir, 5.5f);
                    }

                    break;
                case (GameManager.NowTarget.Tower):
                    beAtk_Obj.GetComponent<PhotonView>().RPC("takeDamage", PhotonTargets.All, 10.0f);
                    break;
                case (GameManager.NowTarget.Player):
                    if (startDetect_3)
                    {
                        beAtk_Obj.GetComponent<PhotonView>().RPC("takeDamage", PhotonTargets.All, 8.0f);
                        Vector3 dir = beAtk_Obj.transform.localPosition - weapon_Detect.position;
                        dir.y = 1f;
                        beAtk_Obj.GetComponent<PhotonView>().RPC("pushOtherTarget", PhotonTargets.All, dir, 6.0f);
                        break;
                    }
                    else
                        beAtk_Obj.GetComponent<PhotonView>().RPC("takeDamage", PhotonTargets.All, 5.0f);
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

    #region 擊退
    [PunRPC]
    public void pushOtherTarget(Vector3 _dir, float _dis)
    {
        this.transform.DOMove(transform.localPosition + _dir.normalized * _dis, .8f).SetEase(Ease.OutBounce);
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
                startDetect_3 = true;
                StartCoroutine(cameraControl.CameraShake(.15f, .3f));
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
                startDetect_3 = false;
                alreadyDamage.Clear();
                swordLight[0].SetActive(false);
                swordLight[1].SetActive(false);
                swordLight[2].SetActive(false);
                break;
        }
    }
    #endregion
}
