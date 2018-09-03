using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class PlayerAni : Photon.MonoBehaviour
{
    protected SmoothFollow cameraControl;
    protected Player player;
    public Animator anim;
    
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
    public GameObject[] swordLight = new GameObject[3];
    public List<GameObject> alreadyDamage;

    protected Vector3 currentAtkDir;
    //combo
    protected int comboIndex;
    protected float beHit_time = 0.25f;
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

    private void Start()
    {
        anim = GetComponent<Animator>();
        player = GetComponent<Player>();

        cameraControl = SmoothFollow.instance;
    }

    #region 武器切換
    [PunRPC]
    public void weaponOC(bool _t)
    {
        anim.SetBool("NowBuild", _t);
        anim.SetTrigger("Switch");
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
                StartCoroutine(player.MatchTimeManager.SetCountDown(StiffnessEnd, beHit_time));
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
        anim.SetInteger("comboIndex", 0);
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

        if (!anim.GetBool("Die"))
            anim.CrossFade("Dodge", 0.01f, 0);
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
    protected void comboFirst(int Index, Vector3 Dir)
    {
        comboIndex = Index;
        anim.SetInteger("comboIndex", comboIndex);
        anim.SetTrigger("Combo");

        currentAtkDir = Dir.normalized;
        player.Net.RPC("TP_Combo", PhotonTargets.Others, comboIndex);
    }
    //之後按下的
    protected void Nextcombo(int Index)
    {
        comboIndex = Index;
        nextComboBool = true;
        if (after_shaking && photonView.isMine)
            goNextCombo();
    }
    //前往下個combo
    protected void goNextCombo()
    {
        anim.SetInteger("comboIndex", comboIndex);
        anim.SetBool("Action", true);
        nextComboBool = false;
        after_shaking = false;
        brfore_shaking = false;
        player.Net.RPC("TP_Combo", PhotonTargets.All, comboIndex);
    }

    [PunRPC]
    protected void TP_Combo(int _i)
    {
        anim.CrossFade("combo" + _i, 0.05f, 0);
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
            currentAtkDir = player.nowMouseDir();
            transform.forward = currentAtkDir.normalized;
        }
    }
    #endregion

    [PunRPC]
    public void waitBuild(bool _t)
    {
        anim.SetBool("Building", _t);
    }

    //攻擊區間傷害判斷
    public virtual void DetectAtkRanage()
    {
        RedressDir();
        ChangeNowDir();
    }
    //給予正確目標傷害
    protected virtual void GetCurrentTarget(Collider[] _enemies)
    {
        
    }
    //目前傷害判定區及刀光特效
    public virtual void SwitchAtkRange(int _n)
    {

    }

    [PunRPC]
    public void HitNull()
    {
        hitNullEffect.Play();
    }

    [PunRPC]
    public void Skill_Q_Fun()
    {
        anim.SetTrigger("Skill_Q");
        player.deadManager.notFeedBack = true;
        player.skillManager.nowSkill = SkillBase.SkillAction.is_Q;
        if (player.skill_Q != null)
            player.skill_Q.Invoke();
    }

    [PunRPC]
    public void Skill_W_Fun()
    {
        anim.SetTrigger("Skill_W");
        player.deadManager.notFeedBack = true;
        player.skillManager.nowSkill = SkillBase.SkillAction.is_W;
        if (player.skill_W != null)
            player.skill_W.Invoke();
    }

    [PunRPC]
    public void Skill_E_Fun()
    {
        anim.SetTrigger("Skill_E");
        player.deadManager.notFeedBack = true;
        player.skillManager.nowSkill = SkillBase.SkillAction.is_E;
        if (player.skill_E != null)
            player.skill_E.Invoke();
    }

    [PunRPC]
    public void Skill_R_Fun()
    {
        anim.SetTrigger("Skill_R");
        player.deadManager.notFeedBack = true;
        player.skillManager.nowSkill = SkillBase.SkillAction.is_R;
        if (player.skill_R != null)
            player.skill_R.Invoke();
    }

    [PunRPC]
    public void Ani_Run(bool isRun)
    {
        anim.SetBool("Run", isRun);        
    }

    public void Die()
    {
        anim.CrossFade("dead" , 0.05f, 0);
        anim.SetBool("Die", true);
    }
}