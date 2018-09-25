using UnityEngine;

public class Allen_Ani : PlayerAni
{
    public AudioSource comboAudio;

    #region 取得動畫雜湊值
    protected override void SetAniHash()
    {
        base.SetAniHash();
        aniHashValue[26] = Animator.StringToHash("Catch");
    }
    #endregion

    #region 設定攻擊Collider
    protected override void SetCheckBox()
    {
        checkEnemyBox[0] = new Vector3(1.7f, 4.5f, .85f);
        checkEnemyBox[1] = new Vector3(4f, 4f, 2f);

        //將combo裂痕特效移到外面
        swordLight[1].transform.SetParent(GameObject.Find("Player_Original").transform);
    }
    #endregion     

    #region 按下判斷
    public override void TypeCombo(Vector3 atkDir)
    {
        if (canClick)
        {
            if (comboIndex == 0 && (anim.GetCurrentAnimatorStateInfo(0).fullPathHash == aniHashValue[24] || anim.GetCurrentAnimatorStateInfo(0).fullPathHash == aniHashValue[25] ||
                anim.GetCurrentAnimatorStateInfo(0).fullPathHash == aniHashValue[17]))
            {
                canClick = false;
                comboFirst(1, atkDir);                
            }
            if (anim.GetCurrentAnimatorStateInfo(0).fullPathHash == aniHashValue[20] && comboIndex == 1)
            {
                canClick = false;
                Nextcombo(2);
            }
            if (anim.GetCurrentAnimatorStateInfo(0).fullPathHash == aniHashValue[21] && comboIndex == 2)
            {
                canClick = false;
                Nextcombo(3);
            }
            if (anim.GetCurrentAnimatorStateInfo(0).fullPathHash == aniHashValue[22] && comboIndex == 3)
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
                NowComboAudio();
                if (photonView.isMine)
                {
                    canClick = true;
                    anim.SetBool(aniHashValue[6], false);
                    player.lockDodge = false;
                }
                break;
            //結束點
            case (2):
                if (nextComboBool)
                {
                    goNextCombo();
                    SwitchAtkRange(8);
                    player.lockDodge = false;
                }
                else if (!anim.GetBool(aniHashValue[6]))
                {
                    SwitchAtkRange(8);
                    player.lockDodge = false;
                    GoBackIdle_canMove();
                    anim.CrossFade(aniHashValue[24], .25f);
                }
                break;
            //前搖點
            case (3):
                if (photonView.isMine)
                {
                    //鎖閃避
                    player.lockDodge = true;
                    redressOpen = true;
                    brfore_shaking = true;
                }
                break;
            //後搖點
            case (4):
                if (photonView.isMine)
                {
                    //解閃避
                    player.lockDodge = false;
                    redressOpen = false;
                    after_shaking = true;
                    if (!canClick && nextComboBool)
                    {
                        goNextCombo();
                        SwitchAtkRange(8);
                    }
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

        if (startDetect_1)
        {
            ProduceCheckBox(weapon_Detect, checkEnemyBox[0]);
        }
        if (startDetect_2)
        {
            ProduceCheckBox(weapon_Detect, checkEnemyBox[1]);
        }
    }

    void ProduceCheckBox(Transform _pos , Vector3 _size)
    {
        checkBox = Physics.OverlapBox(_pos.position, _size, _pos.rotation, canAtkMask);
        if (photonView.isMine && checkBox.Length != 0)
            GetCurrentTarget();
    }
    #endregion

    #region 給予正確目標傷害
    protected override void GetCurrentTarget()
    {
        arrayAmount = checkBox.Length;
        for (int i = 0; i < arrayAmount; i++)
        {
            if (alreadyDamage.Contains(checkBox[i].gameObject))
                continue;

            checkTag = checkBox[i].GetComponent<isDead>();
            if (!checkTag.checkDead)
            {
                Net = checkBox[i].GetComponent<PhotonView>();
                switch (checkTag.myAttributes)
                {
                    case (GameManager.NowTarget.Soldier):
                        if (startDetect_1)
                        {
                            Net.RPC("takeDamage", PhotonTargets.All, player.Net.viewID, 4f);
                        }
                        else
                            Net.RPC("takeDamage", PhotonTargets.All, player.Net.viewID, 6.0f);
                        break;
                    case (GameManager.NowTarget.Tower):
                        Net.RPC("takeDamage", PhotonTargets.All, 10.0f);
                        break;
                    case (GameManager.NowTarget.Player):
                        if (checkTag.noDamage)
                            return;

                        if (startDetect_1)
                        {
                            Net.RPC("takeDamage", PhotonTargets.All, 4f, currentAtkDir.normalized, true);
                        }
                        else
                        {
                            Net.RPC("takeDamage", PhotonTargets.All, 7f, currentAtkDir.normalized, true);                            
                        }
                        break;
                    case (GameManager.NowTarget.Core):
                        Debug.Log("還沒寫");
                        break;
                    default:
                        Debug.Log("錯誤");
                        break;
                }
                alreadyDamage.Add(checkBox[i].gameObject);
            }
        }
    }
    #endregion

    //粒子特效位子跟旋轉
    //combo1
    Vector3 PS1_Pos = new Vector3(0.78f, 2.54f, .6f);
    Vector3 PS1_Rot = new Vector3(-59.2f, -85, -102.1f);
    //combo2
    Vector3 PS2_Pos = new Vector3(.6f, 3.46f, .32f);
    Vector3 PS2_Rot = new Vector3(54, 96.2f, 102.4f);
    //combo4
    Vector3 PS3_Pos = new Vector3(1.1f, 2.96f, 2.76f);
    Vector3 PS3_Rot = new Vector3(-14.9f, -103.4f, -82.6f);
    //地裂
    [SerializeField] Transform PS4_Pos;

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
            case (2):
                if (comboIndex == 1 || comboIndex == 2)
                {
                    swordLight[0].transform.localPosition = PS1_Pos;
                    swordLight[0].transform.localEulerAngles = PS1_Rot;
                    swordLight[0].Play();
                }
                break;
            //刀光2
            case (3):
                if (comboIndex == 2 || comboIndex == 3)
                {
                    swordLight[0].transform.localPosition = PS2_Pos;
                    swordLight[0].transform.localEulerAngles = PS2_Rot;
                    swordLight[0].Play();
                }
                break;
            //刀光3
            case (4):
                if (comboIndex == 3 || comboIndex == 4)
                {
                    swordLight[2].Play();
                    swordLight[3].Play();
                }
                break;
            //刀光4
            case (5):
                if (comboIndex == 4)
                {
                    swordLight[0].transform.localPosition = PS3_Pos;
                    swordLight[0].transform.localEulerAngles = PS3_Rot;
                    swordLight[1].transform.forward = transform.forward;
                    swordLight[1].transform.localPosition = PS4_Pos.transform.position;
                    swordLight[0].Play();
                    swordLight[1].Play();
                }
                break;
            default://8
                startDetect_1 = false;
                startDetect_2 = false;
                for (int i = 0; i < 4; i++)
                {
                    swordLight[i].Stop();
                }
                alreadyDamage.Clear();
                break;
        }
    }
    #endregion

    void NowComboAudio()
    {
        //刀光1,2
        if (comboIndex == 1 || comboIndex == 2 )
        {
            player.AudioScript.PlayAppointAudio(comboAudio, 0);
        }
        //刀光3
        if (comboIndex == 3 )
        {
            player.AudioScript.PlayAppointAudio(comboAudio, 1);
        }
        //刀光4
        if (comboIndex == 4)
        {
            player.AudioScript.PlayAppointAudio(comboAudio, 2);
        }
    }
}
