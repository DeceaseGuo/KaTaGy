using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using UnityEngine.Events;
using DG.Tweening;

[RequireComponent(typeof(NavMeshAgent))]
public class Player : Photon.MonoBehaviour
{
    private MatchTimer matchTime;
    public MatchTimer MatchTimeManager { get { if (matchTime == null) matchTime = MatchTimer.Instance; return matchTime; } }

    private HintManager hintManager;
    public HintManager HintScript { get { if (hintManager == null) hintManager = HintManager.instance; return hintManager; } }

    public GameManager.meIs meIs;
    public PlayerData.PlayerDataBase playerData;
    public PlayerData.PlayerDataBase originalData;
    public PlayerAni AniControll;
    [HideInInspector]
    public BuildManager buildManager;
    [HideInInspector] public isDead deadManager;
    private Ray ray;
    private RaycastHit hit;
    public bool lockDodge;
    private bool canDodge = true;
    
    private NavMeshAgent nav;

    [SerializeField] GameObject clickPointPos;
    [SerializeField] LayerMask canClickToMove_Layer;
    [SerializeField] LayerMask currentDir_Layer;

    [Header("左上能量UI")]
    private Image leftTopPowerBar;
    private Image headImage;

    #region 偵測目前跑步動畫
    private bool isRunning;
    public bool getIsRunning { get { return isRunning; } private set { isRunning = value; } }
    #endregion
    private bool stopClick;
    public bool StopClick { get { return stopClick; } set { stopClick = value; } }
        
    public PhotonView Net;
    //方向
    private Vector3 mousePosition;
    public Vector3 MousePosition { get { return mousePosition; } private set { mousePosition = value; } }
    public Transform arrow;
    [SerializeField] Projector arrowProjector;

    public UnityEvent skill_Q;
    public UnityEvent skill_W;
    public UnityEvent skill_E;
    public UnityEvent skill_R;

    public UnityEvent cancelSkill;

    #region 狀態
    public enum statesData
    {
        None,
        canMove_Atk,
        canMvoe_Build,
        notMove,
        Combo
    }
    private statesData myState = statesData.canMove_Atk;
    public statesData MyState { get { return myState; } set { myState = value; } }

    public enum SkillData
    {
        None,
        skill_Q,
        skill_W,
        skill_E,
        skill_R
    }
    private SkillData skillState = SkillData.None;
    public SkillData SkillState { get { return skillState; } set { skillState = value; } }

    private bool nowCC;
    public bool NowCC { get { return nowCC; } set { nowCC = value; } }
    #endregion
    private CapsuleCollider CharaCollider;
    public BoxCollider shieldCollider;
    //技能
    [HideInInspector]
    public SkillBase skillManager;
    [HideInInspector]
    public bool canSkill_Q = true;
    [HideInInspector]
    public bool canSkill_W = true;
    [HideInInspector]
    public bool canSkill_E = true;
    [HideInInspector]
    public bool canSkill_R = true;

    //負面
    private Tweener flyUp;

    #region 移動所需變量
    private Vector3 tmpNextPos;
    [HideInInspector]
    public Quaternion CharacterRot;
    private Vector3 maxDisGap;
    #endregion

    #region 小地圖所需
    RectTransform map;
    private float mapX;
    private float mapY;
    #endregion

    private void Awake()
    {
        CharaCollider = GetComponent<CapsuleCollider>();
        Net = GetComponent<PhotonView>();
    }
    //    
    
    //
    private void Start()
    {
        //
        map = GameObject.Find("smallMapContainer (1)").GetComponent<RectTransform>();
        //
        skillManager = GetComponent<SkillBase>();
        clickPointPos = GameObject.Find("clickPointPos");
        buildManager = BuildManager.instance;
        nav = GetComponent<NavMeshAgent>();
        deadManager = GetComponent<isDead>();
        nav.updateRotation = false;
        nav.speed = playerData.moveSpeed;
        AniControll = GetComponent<PlayerAni>();
        if (photonView.isMine)
        {
            if (headImage == null)
                headImage = GameObject.Find("headImage_leftTop").GetComponent<Image>();

            headImage.sprite = playerData.headImage;
            checkCurrentPlay();
        }
        else
        {
            SceneObjManager.Instance.enemy_Player = gameObject;
            this.enabled = false;
        }
    }

    #region 改變正確玩家(可以攻擊的對象)
    void checkCurrentPlay()
    {
        if (GameManager.instance.getMyPlayer() == GameManager.MyNowPlayer.player_1)
        {
            Net.RPC("changeLayer", PhotonTargets.All, 30);
            arrowProjector.gameObject.layer = 0;
            Net.RPC("changeMask_1", PhotonTargets.All,(int)GameManager.instance.Meis);
        }
        else if (GameManager.instance.getMyPlayer() == GameManager.MyNowPlayer.player_2)
        {
            Net.RPC("changeLayer", PhotonTargets.All, 31);
            arrowProjector.gameObject.layer = 0;
            Net.RPC("changeMask_2", PhotonTargets.All, (int)GameManager.instance.Meis);
        }
    }
    [PunRPC]
    public void changeMask_1(int _who)
    {
        GetComponent<PlayerAni>().canAtkMask = GameManager.instance.getPlayer1_Mask;
        shieldCollider.gameObject.layer = 30;
        meIs = ((GameManager.meIs)_who);
    }
    [PunRPC]
    public void changeMask_2(int _who)
    {
        GetComponent<PlayerAni>().canAtkMask = GameManager.instance.getPlayer2_Mask;
        shieldCollider.gameObject.layer = 31;
        meIs = ((GameManager.meIs)_who);
    }
    #endregion

    #region 恢復初始數據
    public void formatData()
    {
        Debug.Log("初始數據");
        if (photonView.isMine)
        {
            if (leftTopPowerBar == null)
                leftTopPowerBar = GameObject.Find("mpBar_0020").GetComponent<Image>();
            if (buildManager != null && buildManager.nowBuilding)
                buildManager.BuildSwitch();
            if (nav != null)
                nav.speed = playerData.moveSpeed;

            leftTopPowerBar.fillAmount = 1;
        }

        if (AniControll != null)
            AniControll.WeaponChangePos(1);

        originalData = PlayerData.instance.getPlayerData(meIs);
        playerData = originalData;
        playerData.Ap_original = playerData.Ap_Max;
        playerData.Hp_original = playerData.Hp_Max;
        ChangeMyCollider(true);
    }
    #endregion

    #region 升級
    #region 玩家
    [PunRPC]
    public void UpdataData(int _level,int _whatAbility)
    {
        switch (((UpdateManager.Myability) _whatAbility))
        {
            case (UpdateManager.Myability.Player_ATK):
                UpdataData_Atk(_level);
                break;
            case (UpdateManager.Myability.Player_DEF):
                UpdataData_Def(_level);
                break;
            case (UpdateManager.Myability.Skill_Q_Player):
                UpdataData_Skill_Q(_level);
                break;
            case (UpdateManager.Myability.Skill_W_Player):
                UpdataData_Skill_W(_level);
                break;
            case (UpdateManager.Myability.Skill_E_Player):
                UpdataData_Skill_E(_level);
                break;
            case (UpdateManager.Myability.Skill_R_Player):
                UpdataData_Skill_R(_level);
                break;
            default:
                break;
        }

        PlayerData.instance.ChangeMyData(meIs, originalData);
    }

    void UpdataData_Atk(int _level)
    {
        switch (_level)
        {
            case (1):
                if (originalData.ATK_Level != _level)
                {
                    originalData.ATK_Level = _level;
                    originalData.Atk_Damage += originalData.updateData.Add_atk1;
                    originalData.Atk_maxDamage += originalData.updateData.Add_atk1;
                    originalData.Ap_original += originalData.updateData.Add_ap1;
                    originalData.Ap_Max += originalData.updateData.Add_ap1;

                    playerData.Atk_Damage += originalData.updateData.Add_atk1;
                    playerData.Atk_maxDamage += originalData.updateData.Add_atk1;
                    playerData.Ap_original += originalData.updateData.Add_ap1;
                    playerData.Ap_Max += originalData.updateData.Add_ap1;
                }
                break;
            case (2):
                if (originalData.ATK_Level != _level)
                {
                    originalData.ATK_Level = _level;
                    originalData.Atk_Damage += originalData.updateData.Add_atk2;
                    originalData.Atk_maxDamage += originalData.updateData.Add_atk2;
                    originalData.Ap_original += originalData.updateData.Add_ap2;
                    originalData.Ap_Max += originalData.updateData.Add_ap2;

                    playerData.Atk_Damage += originalData.updateData.Add_atk2;
                    playerData.Atk_maxDamage += originalData.updateData.Add_atk2;
                    playerData.Ap_original += originalData.updateData.Add_ap2;
                    playerData.Ap_Max += originalData.updateData.Add_ap2;
                }
                break;
            case (3):
                if (originalData.ATK_Level != _level)
                {
                    originalData.ATK_Level = _level;
                    originalData.Atk_Damage += originalData.updateData.Add_atk3;
                    originalData.Atk_maxDamage += originalData.updateData.Add_atk3;
                    originalData.Ap_original += originalData.updateData.Add_ap3;
                    originalData.Ap_Max += originalData.updateData.Add_ap3;

                    playerData.Atk_Damage += originalData.updateData.Add_atk3;
                    playerData.Atk_maxDamage += originalData.updateData.Add_atk3;
                    playerData.Ap_original += originalData.updateData.Add_ap3;
                    playerData.Ap_Max += originalData.updateData.Add_ap3;
                }
                break;
            default:
                break;
        }
    }
    void UpdataData_Def(int _level)
    {
        switch (_level)
        {
            case (1):
                if (originalData.DEF_Level != _level)
                {
                    originalData.DEF_Level = _level;
                    originalData.def_base += originalData.updateData.Add_def1;
                    originalData.Hp_Max += originalData.updateData.Add_hp1;
                    originalData.Hp_original += originalData.updateData.Add_hp1;

                    playerData.def_base += originalData.updateData.Add_def1;
                    playerData.Hp_Max += originalData.updateData.Add_hp1;
                    playerData.Hp_original += originalData.updateData.Add_hp1;
                }
                break;
            case (2):
                if (originalData.DEF_Level != _level)
                {
                    originalData.DEF_Level = _level;
                    originalData.def_base += originalData.updateData.Add_def2;
                    originalData.Hp_Max += originalData.updateData.Add_hp2;
                    originalData.Hp_original += originalData.updateData.Add_hp2;

                    playerData.def_base += originalData.updateData.Add_def2;
                    playerData.Hp_Max += originalData.updateData.Add_hp2;
                    playerData.Hp_original += originalData.updateData.Add_hp2;
                }
                break;
            case (3):
                if (originalData.DEF_Level != _level)
                {
                    originalData.DEF_Level = _level;
                    originalData.def_base += originalData.updateData.Add_def3;
                    originalData.Hp_Max += originalData.updateData.Add_hp3;
                    originalData.Hp_original += originalData.updateData.Add_hp3;

                    playerData.def_base += originalData.updateData.Add_def3;
                    playerData.Hp_Max += originalData.updateData.Add_hp3;
                    playerData.Hp_original += originalData.updateData.Add_hp3;
                }
                break;
            default:
                break;
        }
    }
    void UpdataData_Skill_Q(int _level)
    { }
    void UpdataData_Skill_W(int _level)
    { }
    void UpdataData_Skill_E(int _level)
    { }
    void UpdataData_Skill_R(int _level)
    { }
    #endregion

    #region 士兵
    [PunRPC]
    public void UpdataSoldier(int _level, int _whatAbility)
    {
        if (photonView.isMine)
            SceneObjManager.Instance.UpdataMySoldier(_level, _whatAbility);
        else
            SceneObjManager.Instance.UpdataClientSoldier(_level, _whatAbility);
    }
    #endregion

    #region 塔防
    [PunRPC]
    public void UpdataTower(int _level, int _whatAbility)
    {
        if (photonView.isMine)
            SceneObjManager.Instance.UpdataMyTower(_level, _whatAbility);
        else
            SceneObjManager.Instance.UpdataClientTower(_level, _whatAbility);
    }
    #endregion
    #endregion

    private void Update()
    {
        if (!photonView.isMine || deadManager.checkDead)
            return;

        if (leftTopPowerBar.fillAmount != 1)
            AddPower();

        CorrectDirection();        

        if (MyState != statesData.None)
        {
            nowCanDo();
            ///
            if (Input.GetKeyDown(KeyCode.LeftShift))
            {
                transform.position = MousePosition;
            }
        }
    }

    private void FixedUpdate()
    {
        if (!photonView.isMine || deadManager.checkDead || (MyState != statesData.canMove_Atk && MyState != statesData.canMvoe_Build))
            return;

        CharacterRun();
    }

    #region 目前狀態執行→update
    void nowCanDo()
    {
        switch (MyState)
        {
            case statesData.canMove_Atk:
                if (Input.GetMouseButtonDown(1))
                    ClickPoint();
                DetectSkillBtn();
                CharacterAtk_F();
                Dodge_Btn();
                ATK_Build_Btn();
                break;
            case statesData.canMvoe_Build:
                if (Input.GetMouseButtonDown(1))
                    ClickPoint();
                ATK_Build_Btn();
                break;
            case statesData.notMove:
                Dodge_Btn();
                break;
            case statesData.Combo:
                DetectSkillBtn();
                CharacterAtk_F();
                Dodge_Btn();
                AniControll.DetectAtkRanage();
                break;
            default:
                break;
        }
    }
    #endregion

    #region 偵測按下
    //切換攻擊與建造模式
    private void ATK_Build_Btn()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            if (buildManager.nowSelect && !StopClick && (AniControll.anim.GetCurrentAnimatorStateInfo(0).IsName("Idle_Atk") ||
             AniControll.anim.GetCurrentAnimatorStateInfo(0).IsName("Run_Atk") || AniControll.anim.GetCurrentAnimatorStateInfo(0).IsName("build_Idle")
             || AniControll.anim.GetCurrentAnimatorStateInfo(0).IsName("build_run")))
            {
                if (SkillState != SkillData.None)
                    CancelNowSkill();

                buildManager.BuildSwitch();
                StopClick = true;
            }
        }

    }
    //閃避
    private void Dodge_Btn()
    {
        if (Input.GetKeyDown(KeyCode.LeftAlt))
        {
            if (canDodge && !lockDodge)
            {
                if (SkillState != SkillData.None)
                    CancelNowSkill();

                if (ConsumeAP(10f, true))
                {
                    stopAnything_Switch(true);
                    Dodge_FCN(nowMouseDir());
                }
            }
        }
    }
    //按下F→目前為combo
    private void CharacterAtk_F()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            if (AniControll.canClick)
            {
                if (SkillState != SkillData.None)
                    CancelNowSkill();

                if (MyState != statesData.Combo)
                {
                    getIsRunning = false;
                    nav.ResetPath();
                    MyState = statesData.Combo;
                    transform.forward = nowMouseDir().normalized;
                }

                AniControll.TypeCombo(transform.forward);
            }
        }
    }
    #endregion

    #region 玩家技能
    private void DetectSkillBtn()
    {
        Character_Skill_Q();
        Character_Skill_W();
        Character_Skill_E();
        Character_Skill_R();
    }
    //Q
    private void Character_Skill_Q()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            if (SkillState != SkillData.skill_Q && canSkill_Q && skillManager.nowSkill == SkillBase.SkillAction.None)
            {
                if (SkillState != SkillData.None)
                    CancelNowSkill();

                skillManager.Skill_Q_Click();
            }
        }

        if (SkillState == SkillData.skill_Q)
            skillManager.In_Skill_Q();    
    }
    //W
    private void Character_Skill_W()
    {
        if (Input.GetKeyDown(KeyCode.W))
        {
            if (SkillState != SkillData.skill_W && canSkill_W && skillManager.nowSkill == SkillBase.SkillAction.None)
            {
                if (SkillState != SkillData.None)
                    CancelNowSkill();
                skillManager.Skill_W_Click();
            }
        }

        if (SkillState == SkillData.skill_W)
            skillManager.In_Skill_W();
    }
    //E
    private void Character_Skill_E()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (SkillState != SkillData.skill_E && canSkill_E && skillManager.nowSkill == SkillBase.SkillAction.None)
            {
                if (SkillState != SkillData.None)
                    CancelNowSkill();
                skillManager.Skill_E_Click();
            }
        }

        if (SkillState == SkillData.skill_E)
            skillManager.In_Skill_E();
    }
    //R
    private void Character_Skill_R()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            if (SkillState != SkillData.skill_R && canSkill_R && skillManager.nowSkill == SkillBase.SkillAction.None)
            {
                if (SkillState != SkillData.None)
                    CancelNowSkill();
                skillManager.Skill_R_Click();
            }
        }

        if (SkillState == SkillData.skill_R)
            skillManager.In_Skill_R();
    }
    #endregion

    #region 偵測點擊位置
    void ClickPoint()
    {
        if (!IsMap())//我新加的
        {
            ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit, 150, canClickToMove_Layer))
            {
                if (hit.transform.CompareTag("CanClickMove"))
                {
                    clickPointPos.transform.position = hit.point;
                    getTatgetPoint(clickPointPos.transform.position);
                }
            }
        }
    }
    bool IsMap()
    {
        mapX = Input.mousePosition.x - (map.position.x - (map.rect.width * 0.5f));
        mapY = Input.mousePosition.y - (map.position.y - (map.rect.height * 0.5f));

        return (mapX < map.rect.width && mapX > 0 && mapY < map.rect.height && mapY > 0);
    }

    //無時無刻偵測滑鼠方向
    void CorrectDirection()
    {
        ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out hit, 150, canClickToMove_Layer))
        {
            MousePosition = hit.point;
            nowMouseDir();
        }
    }
    //滑鼠目前方向
    public Vector3 nowMouseDir()
    {
        MousePosition = new Vector3(MousePosition.x, transform.localPosition.y, MousePosition.z);
        arrow.rotation = Quaternion.LookRotation(MousePosition - transform.position);
        return arrow.forward;
    }
    #endregion

    #region 得到移動終點位置
    public void getTatgetPoint(Vector3 tragetPoint)
    {
        nav.SetDestination(tragetPoint);
        getIsRunning = true;
        Net.RPC("Ani_Run", PhotonTargets.All, getIsRunning);
    }
    #endregion

    #region 角色移動
    void CharacterRun()
    {
        if (getIsRunning)
        {
            #region 尋找下一個位置方向
            tmpNextPos = nav.steeringTarget - transform.localPosition;
            tmpNextPos.y = transform.localPosition.y;
            CharacterRot = Quaternion.LookRotation(tmpNextPos);
            #endregion

            #region 判斷是否到最終目標點→否則執行移動
            maxDisGap = nav.destination - transform.localPosition;            
            if (maxDisGap.sqrMagnitude < Mathf.Pow(playerData.stoppingDst, 2))
            {
                isStop();
            }
            else
            {
                transform.rotation = Quaternion.Lerp(transform.rotation, CharacterRot, playerData.rotSpeed);
            }
            #endregion
        }
    }
    #endregion

    #region 被攻擊
    public void beHit(Vector3 _dir)
    {
        CharacterRot = Quaternion.LookRotation(-_dir.normalized);
        transform.rotation = CharacterRot;
        if (photonView.isMine)
        {
            AniControll.beOtherHit();
            MyState = statesData.notMove;
            lockDodge = false;
        }
    }
    #endregion

    #region 負面效果
    //暈眩 僵直
    [PunRPC]
    public void GetDeBuff_Stun(float _time)
    {
        if (deadManager.noCC)
            return;

        if (!deadManager.checkDead)
        {
            if (photonView.isMine)
            {
                stopAnything_Switch(true);
                CancelNowSkill();
            }
            NowCC = true;
            if (!AniControll.anim.GetBool("Die"))
            {
                AniControll.anim.CrossFade("Stun", 0.02f, 0);
                AniControll.anim.SetBool("StunRock", true);
            }
            StartCoroutine(MatchTimeManager.SetCountDown(Recover_Stun, _time));
        }
    }
    //緩速
    public void GetDeBuff_Slow()
    {

    }
    //破甲
    public void GetDeBuff_DestoryDef()
    {

    }
    //燒傷
    public void GetDeBuff_Burn()
    {

    }
    //擊飛
    [PunRPC]
    public void pushOtherTarget()
    {
        if (deadManager.noCC)
            return;

        if (!deadManager.checkDead)
        {
            CancelNowSkill();
            stopAnything_Switch(true);
            NowCC = true;
            if (!AniControll.anim.GetBool("Die"))
            {
                AniControll.anim.CrossFade("HitFly", 0.02f, 0);
            }
        }
    }

    //往上擊飛
    [PunRPC]
    public void HitFlayUp()
    {
        if (deadManager.noCC)
            return;

        flyUp = transform.DOMoveY(transform.position.y + 6, 0.3f).SetAutoKill(false).SetEase(Ease.OutBack);
        flyUp.onComplete = delegate () { EndFlyUp(); };
        if (!NowCC)
        {
            GetDeBuff_Stun(1.2f);
        }
    }
    #endregion

    #region 負面狀態恢復
    //恢復暈眩
    void Recover_Stun()
    {
        GoBack_AtkState();
        AniControll.anim.SetBool("StunRock", false);
    }
    //回到地上
    void EndFlyUp()
    {
        flyUp.PlayBackwards();
    }
    #endregion

    #region 功能
    //消耗能量
    public bool ConsumeAP(float _value, bool _nowConsumer)
    {
        if (playerData.Ap_original - _value >= 0)
        {
            if (_nowConsumer)
            {
                playerData.Ap_original -= _value;
                leftTopPowerBar.fillAmount = playerData.Ap_original / playerData.Ap_Max;
            }
            return true;
        }
        else
        {
            HintScript.CreatHint("能量不足");
            return false;
        }
    }
    private void AddPower()
    {
        playerData.Ap_original += playerData.add_APValue * Time.deltaTime;
        playerData.Ap_original = Mathf.Clamp(playerData.Ap_original, 0, playerData.Ap_Max);
        leftTopPowerBar.fillAmount = playerData.Ap_original / playerData.Ap_Max;
    }
    //閃避執行
    private void Dodge_FCN(Vector3 _dir)
    {
        canDodge = false;
        transform.forward = _dir.normalized;
        Net.RPC("GoDodge", PhotonTargets.All);
        StartCoroutine(MatchTimeManager.SetCountDown(Dodge_End, playerData.Dodget_Delay));
    }
    //閃避cd結束
    void Dodge_End()
    {
        canDodge = true;
    }

    #region 技能相關
    //技能冷卻時間
    public void CountDown_Q()
    {
        canSkill_Q = true;
    }
    public void CountDown_W()
    {
        canSkill_W = true;
    }
    public void CountDown_E()
    {
        canSkill_E = true;
    }
    public void CountDown_R()
    {
        canSkill_R = true;
    }

    public void CancelNowSkill()
    {
        switch (SkillState)
        {
            case SkillData.skill_Q:
                canSkill_Q = true;               
                break;
            case SkillData.skill_W:
                canSkill_W = true;
                break;
            case SkillData.skill_E:
                canSkill_E = true;                
                break;
            case SkillData.skill_R:
                canSkill_R = true;
                break;
            default:
                break;
        }
        skillManager.CancelDetectSkill(SkillState);
        SkillState = SkillData.None;
    }
    #endregion
    #endregion

    #region 其他腳本需求
    //停止角色移動
    public void isStop()
    {
        if (getIsRunning)
        {
            getIsRunning = false;
            nav.ResetPath();
            Net.RPC("Ani_Run", PhotonTargets.All, getIsRunning);
        }
    }
    //停止一切行為(無法操控)
    public void stopAnything_Switch(bool _stop)
    {
        isStop();
        if (_stop)
            MyState = statesData.None;
        else
        {
            if (!buildManager.nowBuilding)
                MyState = statesData.canMove_Atk;
            else
                MyState = statesData.canMvoe_Build;
        }
    }
    //停止行動 只能閃避
    public void StopAllOnlyDodge()
    {
        isStop();
        MyState = statesData.notMove;
        lockDodge = false;
    }
    //回攻擊狀態
    public void GoBack_AtkState()
    {
        if (!NowCC)
            MyState = statesData.canMove_Atk;
    }

    //切換目前模式(攻擊 , 建造)
    public void switchWeapon(bool _can)
    {
        if (photonView.isMine && !deadManager.checkDead)
        {
            Net.RPC("weaponOC", PhotonTargets.All, _can);
        }
    }
    //等待建造時間
    public void switchScaffolding(bool _t)
    {
        if (photonView.isMine)
            Net.RPC("waitBuild", PhotonTargets.All, _t);
    }

    //改變碰撞(無敵用)
    public void ChangeMyCollider(bool _myCollider)
    {
        CharaCollider.enabled = _myCollider;
        shieldCollider.enabled = !_myCollider;
    }
    //精靈王傳送用
    public void TeleportPos(Vector3 _pos)
    {
        nav.Warp(_pos);
    }
    #endregion

    #region 偵測目前是否有路徑
    public bool getNavPath()
    {
        return nav.hasPath;
    }
    #endregion

    #region 死亡
    public void Death()
    {
        stopAnything_Switch(true);
        CharaCollider.enabled = false;
        AniControll.Die();
        if (photonView.isMine)
        {
            Invoke("Return_ObjPool", 3f);
            CancelNowSkill();
            cancelSkill.Invoke();
            CameraEffect.instance.nowDie(true);
            Creatplayer.instance.player_ReBorn(playerData.ReBorn_CountDown);
        }
    }

    void Return_ObjPool()
    {
        if (photonView.isMine)
        {
            Net.RPC("SetActiveF", PhotonTargets.All);
        }
    }
    #endregion
}
