using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class PlayerAni : Photon.MonoBehaviour
{
    protected SmoothFollow cameraControl;
    protected Player player;
    public Animator anim;
    public AudioSource comboAudio;

    [Header("武器")]
    public MeshRenderer swordRecyclePos;   //回收武器的地方
    public MeshRenderer pullSwordPos;      //拔起武器的地方
    public Transform weapon_Detect;     //武器攻擊判斷中心
    //public Transform weapon_Detect_Hand; //第2個攻擊判斷區域(艾倫手)
    public LayerMask canAtkMask;

    [Header("Combo")]
    public bool canClick = true;
    //後續combo被點擊
    public bool nextComboBool;
    //前搖點
    protected bool brfore_shaking;
    //後搖點
    protected bool after_shaking;

    //攻擊判定用
    protected bool startDetect_1 = false;
    protected bool startDetect_2 = false;
    public ParticleSystem[] swordLight = new ParticleSystem[3];
    public List<GameObject> alreadyDamage;

    protected Vector3 currentAtkDir;
    //combo
    public byte comboIndex;
    protected float beHit_time = 0.4f;
    protected bool canStiffness = true;
    //攻擊矯正
    protected bool redressOpen = false;
    protected Tweener myTweener;

    //武器打中防禦特效
    [SerializeField] ParticleSystem hitNullEffect;

    #region combo目標判斷所需變數
    //combo的Overlap使用
    protected Collider[] checkBox;
    protected PhotonView Net;
    protected isDead checkTag;
    protected int arrayAmount;
    #endregion

    #region 攻擊矯正方向所需變數
    public float viewRadius;
    [Range(0, 360)]
    public int viewAngle;
    private Collider[] Enemy;
    private Vector3 dirToTarget;
    private int targetAmount;
    #endregion

    //攻擊偵測
    protected Vector3[] checkEnemyBox = new Vector3[2];

    //動畫雜湊值
    [HideInInspector]
    public int[] aniHashValue;
    [SerializeField]
    protected int allHashAmount = 26;

    private void Awake()
    {
        SetCheckBox();
        SetAniHash();
    }

    private void Start()
    {
        anim = GetComponent<Animator>();
        player = GetComponent<Player>();

        cameraControl = SmoothFollow.instance;
    }

    #region 取得動畫雜湊值
    protected virtual void SetAniHash()
    {
        if (allHashAmount <= 25)
            return;

        aniHashValue = new int[allHashAmount];

        aniHashValue[0] = Animator.StringToHash("Switch");
        aniHashValue[1] = Animator.StringToHash("NowBuild");
        aniHashValue[2] = Animator.StringToHash("Run");
        aniHashValue[3] = Animator.StringToHash("comboIndex");
        aniHashValue[4] = Animator.StringToHash("Combo");
        aniHashValue[5] = Animator.StringToHash("ExitCombo");
        aniHashValue[6] = Animator.StringToHash("Action");
        aniHashValue[7] = Animator.StringToHash("Building");
        aniHashValue[8] = Animator.StringToHash("PullSword");
        aniHashValue[9] = Animator.StringToHash("StunRock");
        aniHashValue[10] = Animator.StringToHash("Skill_Q");
        aniHashValue[11] = Animator.StringToHash("Skill_W");
        aniHashValue[12] = Animator.StringToHash("Skill_E");
        aniHashValue[13] = Animator.StringToHash("Skill_R");
        aniHashValue[14] = Animator.StringToHash("Hit");
        aniHashValue[15] = Animator.StringToHash("Die");
        //crossfade死亡
        aniHashValue[16] = Animator.StringToHash("Base Layer.dead");
        //crossfade閃避
        aniHashValue[17] = Animator.StringToHash("Base Layer.Skill.Dodge");
        //crossfade暈眩
        aniHashValue[18] = Animator.StringToHash("Base Layer.Hit.Stun");
        //crossfade擊飛
        aniHashValue[19] = Animator.StringToHash("Base Layer.Hit.HitFly");
        //crossfade  Combo1~4
        aniHashValue[20] = Animator.StringToHash("Base Layer.Combo.combo1");
        aniHashValue[21] = Animator.StringToHash("Base Layer.Combo.combo2");
        aniHashValue[22] = Animator.StringToHash("Base Layer.Combo.combo3");
        aniHashValue[23] = Animator.StringToHash("Base Layer.Combo.combo4");
        //攻擊狀態 站與跑
        aniHashValue[24] = Animator.StringToHash("Base Layer.Idle_Atk");
        aniHashValue[25] = Animator.StringToHash("Base Layer.Run_Atk");
    }
    #endregion
    protected virtual void SetCheckBox()
    { }

    #region 武器切換
    [PunRPC]
    public void weaponOC(bool _t)
    {
        anim.SetBool(aniHashValue[1], _t);
        anim.SetTrigger(aniHashValue[0]);
    }

    public void WeaponChangePos(int _n)
    {
        switch (_n)
        {
            //武器回背上
            case (0):
                swordRecyclePos.enabled = true;
                pullSwordPos.enabled = false;

                break;
            //武器回手上
            case (1):
                swordRecyclePos.enabled = false;
                pullSwordPos.enabled = true;
                break;
            //玩家不可移動
            case (2):
                player.stopAnything_Switch(true);
                break;
            //玩家可移動,模式可在切換
            case (3):
                player.stopAnything_Switch(false);
                player.StopClick = false;
                break;
        }
    }
    #endregion

    #region 被攻擊僵直反饋
    public void beOtherHit()
    {
        if (!player.deadManager.checkDead)
        {
            if (canStiffness)
            {
                canStiffness = false;
                player.MatchTimeManager.SetCountDownNoCancel(StiffnessEnd, beHit_time);
            }
        }
    }
    void StiffnessEnd()
    {
        canStiffness = true;
        player.stopAnything_Switch(false);
    }
    #endregion

    #region 取消動作
    //取消動作Ani
    protected void CancleAllAni()
    {
        ComboAniEnd();
        SwitchAtkRange(8);
    }

    public void GoBackIdle_canMove()
    {
        ComboAniEnd();
        player.stopAnything_Switch(false);
        canStiffness = true;
    }

    void ComboAniEnd()
    {
        comboIndex = 0;
        anim.SetInteger(aniHashValue[3], 0);
        canClick = true;
        redressOpen = false;
        nextComboBool = false;
        after_shaking = false;
        brfore_shaking = false;
        if (myTweener != null)
            myTweener.Kill();
    }
    #endregion

    #region 閃避
    [PunRPC]
    public void GoDodge()
    {
        CancleAllAni();
        player.deadManager.notFeedBack = true;

        if (!anim.GetBool(aniHashValue[15]))
            anim.CrossFade(aniHashValue[17], 0.01f, 0);
    }
    #endregion

    #region Combo
    //按下判斷
    public virtual void TypeCombo(Vector3 atkDir)
    {

    }
    //動畫播放間判定
    public virtual void comboCheck(int _n)
    {

    }
    #endregion

    #region Combo傳輸
    //第一次按下
    protected void comboFirst(byte Index, Vector3 Dir)
    {
        comboIndex = Index;
        anim.SetInteger(aniHashValue[3], comboIndex);
        anim.SetTrigger(aniHashValue[4]);

        currentAtkDir = Dir.normalized;
        player.Net.RPC("TP_Combo", PhotonTargets.Others, comboIndex);
    }
    //之後按下的
    protected void Nextcombo(byte Index)
    {
        comboIndex = Index;
        nextComboBool = true;
        if (after_shaking && photonView.isMine)
            goNextCombo();
    }
    //前往下個combo
    protected void goNextCombo()
    {
        anim.SetInteger(aniHashValue[3], comboIndex);
        anim.SetBool(aniHashValue[6], true);
        nextComboBool = false;
        after_shaking = false;
        brfore_shaking = false;
        player.Net.RPC("TP_Combo", PhotonTargets.Others, comboIndex);
    }

    [PunRPC]
    protected void TP_Combo(byte _i)
    {
        comboIndex = _i;
        _i += 19;
        anim.CrossFade(aniHashValue[_i], 0.01f, 0);
    }
    #endregion

    #region 攻擊矯正方向
    void RedressDir()
    {
        if (redressOpen)
        {
            Enemy = Physics.OverlapSphere(transform.position, viewRadius, canAtkMask);
            if (Enemy.Length != 0)
            {
                targetAmount = Enemy.Length;
                for (int i = 0; i < targetAmount; i++)
                {
                    dirToTarget = (Enemy[i].transform.position - transform.position).normalized;
                    if (Vector3.Angle(transform.forward, dirToTarget) < (viewAngle * 0.5f))
                    {
                        player.CharacterRot = Quaternion.LookRotation(dirToTarget.normalized);
                        transform.rotation = player.CharacterRot;
                        currentAtkDir = dirToTarget.normalized;
                        Debug.Log("矯正結束");
                        redressOpen = false;
                        break;
                    }
                }
            }
        }
    }

    /// <summary>
    /// editor觀看用矯正區域
    /// </summary>
    public Vector3 DirFromAngle(float angleInDegrees, bool angleIsGlobal)
    {
        if (!angleIsGlobal)
        {
            angleInDegrees += transform.eulerAngles.y;
        }
        return new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), 0, Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));
    }
    #endregion

    #region 前搖點前可變動方向
    protected void ChangeNowDir()
    {
        if (photonView.isMine && !brfore_shaking && !redressOpen)
        {
            currentAtkDir = player.arrow.forward;
            transform.forward = currentAtkDir.normalized;
        }
    }
    #endregion

    [PunRPC]
    public void waitBuild(bool _t)
    {
        anim.SetBool(aniHashValue[7], _t);
    }

    //攻擊區間傷害判斷
    public virtual void DetectAtkRanage()
    {
        RedressDir();
        ChangeNowDir();
    }
    //給予正確目標傷害
    protected virtual void GetCurrentTarget()
    {
        
    }
    //目前傷害判定區及刀光特效
    public virtual void SwitchAtkRange(int _n)
    {

    }

    public void StopComboAudio()
    {
        comboAudio.Stop();
    }

    [PunRPC]
    public void HitNull()
    {
        hitNullEffect.Play();
    }

    [PunRPC]
    public void Skill_Q_Fun()
    {
        player.deadManager.notFeedBack = true;
        player.skillManager.nowSkill = SkillBase.SkillAction.is_Q;
        anim.SetTrigger(aniHashValue[10]);
        if (player.skill_Q != null)
            player.skill_Q.Invoke();
    }

    [PunRPC]
    public void Skill_W_Fun()
    {
        player.deadManager.notFeedBack = true;
        player.skillManager.nowSkill = SkillBase.SkillAction.is_W;
        anim.SetTrigger(aniHashValue[11]);
        if (player.skill_W != null)
            player.skill_W.Invoke();
    }

    [PunRPC]
    public void Skill_E_Fun()
    {
        player.deadManager.notFeedBack = true;
        player.skillManager.nowSkill = SkillBase.SkillAction.is_E;
        anim.SetTrigger(aniHashValue[12]);
        if (player.skill_E != null)
            player.skill_E.Invoke();
    }

    [PunRPC]
    public void Skill_R_Fun()
    {
        player.deadManager.notFeedBack = true;
        player.skillManager.nowSkill = SkillBase.SkillAction.is_R;
        anim.SetTrigger(aniHashValue[13]);
        if (player.skill_R != null)
            player.skill_R.Invoke();
    }

    [PunRPC]
    public void Ani_Run(bool isRun)
    {
        anim.SetBool(aniHashValue[2], isRun);        
    }

    public void Die()
    {        
        anim.CrossFade(aniHashValue[16], 0.05f, 0);
        anim.SetBool(aniHashValue[15], true);
    }
}