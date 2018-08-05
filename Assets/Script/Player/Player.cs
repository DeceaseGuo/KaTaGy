using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using UnityEngine.Events;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(NavMeshAgent))]
public class Player : Photon.MonoBehaviour
{
    private MatchTimer matchTime;
    public MatchTimer MatchTimeManager { get { if (matchTime == null) matchTime = MatchTimer.Instance; return matchTime; } }

    public PlayerData.PlayerDataBase playerData;
    public PlayerData.PlayerDataBase originalData;
    public PlayerAni AniControll;
    [HideInInspector]
    public BuildManager buildManager;
    private HintManager hintManager;
    [HideInInspector] public isDead deadManager;
    private Ray ray;
    private RaycastHit hit;
    private bool canDodge = true;
    private CharacterController Chara;
    private NavMeshAgent nav;

    private Vector3 MoveDir = Vector3.zero;  //角色方向
    [SerializeField] Transform nextTargetRot;
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

    [HideInInspector]
    public Quaternion CharacterRot;
    public PhotonView Net;
    //方向
    private Vector3 mousePosition;
    public Transform arrow;

    public UnityEvent skill_Q;
    public UnityEvent cancelSkill;

    #region 狀態
    public enum statesData
    {
        None,
        canMove_Atk,
        canMvoe_Build,
        UAV,
        notMove,
        Combo
    }
    private statesData myState = statesData.canMove_Atk;
    public statesData MyState { get { return myState; } set { myState = value; } }
    /*public enum skillData
    {
        None,
        Q,
        W,
        E,
        R
    }
    private skillData mySkill = skillData.None;
    public skillData MySkill { get { return mySkill; } private set { mySkill = value; } }
    */
    public enum buffData
    {
        None,
        NoDamage,
        NoCC,
        Shield //盾
    }
    private buffData nowBuff = buffData.None;
    public buffData NowBuff { get { return nowBuff; } private set { nowBuff = value; } }
    #endregion

    private bool canSkill_Q = true;

    private void Awake()
    {
        Chara = GetComponent<CharacterController>();
        MoveDir = transform.forward;
        Net = GetComponent<PhotonView>();
    }

    private void OnEnable()
    {
        if (photonView.isMine)
            formatData();
    }

    private void Start()
    {
        clickPointPos = GameObject.Find("clickPointPos");
        nextTargetRot = GameObject.Find("Detect").transform;
        hintManager = HintManager.instance;
        buildManager = BuildManager.instance;
        nav = GetComponent<NavMeshAgent>();
        deadManager = GetComponent<isDead>();
        nav.updateRotation = false;
        if (photonView.isMine)
        {
            AniControll = Creatplayer.instance.Player_AniScript;
            if (headImage == null)
                headImage = GameObject.Find("headImage_leftTop").GetComponent<Image>();

            headImage.sprite = playerData.headImage;
            checkCurrentPlay();
        }
        else
        {
            nav.enabled = false;
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
            AniControll.canAtkMask = GameManager.instance.getPlayer1_Mask;
            AniControll.farDistance += GameManager.instance.getPlayer1_Mask;
            arrow.gameObject.layer = 0;
            Net.RPC("changeMask_1", PhotonTargets.Others);
        }
        else if (GameManager.instance.getMyPlayer() == GameManager.MyNowPlayer.player_2)
        {
            Net.RPC("changeLayer", PhotonTargets.All, 31);
            AniControll.canAtkMask = GameManager.instance.getPlayer2_Mask;
            AniControll.farDistance += GameManager.instance.getPlayer2_Mask;
            arrow.gameObject.layer = 0;
            Net.RPC("changeMask_2", PhotonTargets.Others);
        }
    }
    [PunRPC]
    public void changeMask_1()
    {
        AniControll.canAtkMask = GameManager.instance.getPlayer1_Mask;
        AniControll.farDistance += GameManager.instance.getPlayer1_Mask;
    }
    [PunRPC]
    public void changeMask_2()
    {
        AniControll.canAtkMask = GameManager.instance.getPlayer2_Mask;
        AniControll.farDistance += GameManager.instance.getPlayer2_Mask;
    }
    #endregion

    #region 恢復初始數據
    public void formatData()
    {
        Debug.Log("初始數據");
        if (leftTopPowerBar == null)
            leftTopPowerBar = GameObject.Find("mpBar_0020").GetComponent<Image>();
        if (buildManager != null && buildManager.nowBuilding)
            buildManager.BuildSwitch();

        leftTopPowerBar.fillAmount = 1;
        if (originalData.headImage == null)
            originalData = PlayerData.instance.getPlayerData(GameManager.instance.Meis);
        playerData = originalData;
        playerData.Ap_original = playerData.Ap_Max;
        Chara.enabled = true;
        Net.RPC("TP_resetHp", PhotonTargets.Others, originalData.Hp_Max);
    }
    [PunRPC]
    public void TP_resetHp(float _hp)
    {
        Chara.enabled = true;
        playerData.Hp_original = _hp;
        playerData.Hp_Max = _hp;
    }
    #endregion

    #region 升級
    public void UpdateMyData(int _level, bool _atk, bool _def, bool _Q, bool _W, bool _E, bool _R)
    {
        switch (_level)
        {
            case (1):
                //功
                if (_atk && originalData.ATK_Level != _level)
                {
                    originalData.ATK_Level = _level;
                    originalData.Atk_Damage += originalData.Add_atk1;
                    playerData.Atk_Damage += originalData.Add_atk1;
                }
                //防
                if (_def && originalData.DEF_Level != _level)
                {
                    originalData.DEF_Level = _level;
                    originalData.def_base += originalData.Add_def1;
                    playerData.def_base += originalData.Add_def1;
                }
                break;
            case (2):
                //功
                if (_atk && originalData.ATK_Level != _level)
                {
                    originalData.ATK_Level = _level;
                    originalData.Atk_Damage += originalData.Add_atk2;
                    playerData.Atk_Damage += originalData.Add_atk2;
                }
                //防
                if (_def && originalData.DEF_Level != _level)
                {
                    originalData.DEF_Level = _level;
                    originalData.def_base += originalData.Add_def2;
                    playerData.def_base += originalData.Add_def2;
                }
                break;
            case (3):
                //功
                if (_atk && originalData.ATK_Level != _level)
                {
                    originalData.ATK_Level = _level;
                    originalData.Atk_Damage += originalData.Add_atk3;
                    playerData.Atk_Damage += originalData.Add_atk3;
                }
                //防
                if (_def && originalData.DEF_Level != _level)
                {
                    originalData.DEF_Level = _level;
                    originalData.def_base += originalData.Add_def3;
                    playerData.def_base += originalData.Add_def3;
                }
                break;
            default:
                break;
        }
    }
    #endregion

    /*
    public float TestAtkTime;
    void TestAtk()
    {
        getIsRunning = false;
        nav.ResetPath();
        Vector3 tmpDir = transform.forward;
        if (MyState != statesData.Combo)
        {
            MyState = statesData.Combo;
            tmpDir = nowMouseDir();
            CharacterRot = Quaternion.LookRotation(tmpDir.normalized);
            transform.rotation = CharacterRot;
        }

        AniControll.TypeCombo(tmpDir);
    }*/

    private void Update()
    {
        if (!photonView.isMine || deadManager.checkDead || MyState == statesData.None)
            return;

        if (leftTopPowerBar.fillAmount != 1)
            AddPower();

        nowCanDo();
        CorrectDirection();
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {

            transform.position = mousePosition;
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
                UAV_Btn(statesData.UAV);
                DetectSkillBtn();
                CharacterAtk_F();
                Dodge_Btn();
                ATK_Build_Btn();
                break;
            case statesData.canMvoe_Build:
                if (Input.GetMouseButtonDown(1))
                    ClickPoint();
                UAV_Btn(statesData.UAV);
                ATK_Build_Btn();
                break;
            case statesData.UAV:
                if (!buildManager.nowBuilding)
                    UAV_Btn(statesData.canMove_Atk);
                else
                    UAV_Btn(statesData.canMvoe_Build);
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
        if (buildManager.nowSelect && !StopClick)
        {
            if (Input.GetKeyDown(KeyCode.Tab))
            {
                buildManager.BuildSwitch();
                StopClick = true;
            }
        }
    }
    //閃避
    private void Dodge_Btn()
    {
        if (canDodge && (Input.GetKeyDown(KeyCode.LeftAlt) || Input.GetKeyDown(KeyCode.RightAlt)))
        {
            if (ConsumeAP(20f))
            {
                // MySkill = skillData.Dodge;
                MyState = statesData.canMove_Atk;
                Dodge_FCN(nowMouseDir());
            }
        }
    }
    //開關UAV
    private void UAV_Btn(statesData _data)
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            SmoothFollow.instance.switch_UAV();
            MyState = _data;
        }
    }
    //按下F→目前為combo
    private void CharacterAtk_F()
    {
        if (Input.GetKeyDown(KeyCode.F) && AniControll.canClick)
        {
            getIsRunning = false;
            nav.ResetPath();
            Vector3 tmpDir = transform.forward;
            if (MyState != statesData.Combo)
            {
                MyState = statesData.Combo;
                tmpDir = nowMouseDir();
                CharacterRot = Quaternion.LookRotation(tmpDir.normalized);
                transform.rotation = CharacterRot;
            }

            AniControll.TypeCombo(tmpDir);
        }
    }
    //技能
    private void DetectSkillBtn()
    {
        Character_Skill_Q();
    }
    private void Character_Skill_Q()
    {
        if (Input.GetKeyDown(KeyCode.Q) && canSkill_Q)
        {
            canSkill_Q = false;
            StopAllOnlyDodge();
            transform.forward = arrow.forward;
            Net.RPC("Skill_Q", PhotonTargets.All);
            //skill_Q.Invoke();
            StartCoroutine(MatchTimeManager.SetCountDown(CountDown_Q, playerData.skillCD_Q));
        }
    }
    #endregion

    #region 偵測點擊位置
    void ClickPoint()
    {
        ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out hit, 150, canClickToMove_Layer))
        {
            if (hit.transform.tag == "CanClickMove")
            {
                clickPointPos.transform.position = hit.point;
                getTatgetPoint(clickPointPos.transform.position);
            }
        }
    }
    //combo狀態時偵測滑鼠位置用
    void CorrectDirection()
    {
        ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out hit, 150, canClickToMove_Layer))
        {
            mousePosition = hit.point;
            nowMouseDir();
        }
    }
    //滑鼠目前方向
    public Vector3 nowMouseDir()
    {
        Vector3 tmpDir = mousePosition - transform.position;
        tmpDir.y = transform.localPosition.y;
        arrow.rotation = Quaternion.LookRotation(tmpDir);
        return tmpDir.normalized;
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
            /*  if (!Chara.isGrounded)
              {
                  Debug.Log("不在地板上");
                  return;
              }*/

            #region 尋找下一個位置方向
            Vector3 tmpNextPos = nav.steeringTarget - transform.localPosition;
            tmpNextPos.y = transform.localPosition.y;
            if (tmpNextPos != Vector3.zero)
            {
                CharacterRot = Quaternion.LookRotation(tmpNextPos);
                nextTargetRot.rotation = CharacterRot;
                MoveDir = nextTargetRot.forward;
            }
            #endregion

            #region 判斷是否到最終目標點→否則執行移動
            Vector3 maxDisGap = nav.destination - transform.localPosition;
            float maxDis = maxDisGap.sqrMagnitude;
            if (maxDis < Mathf.Pow(playerData.stoppingDst, 2))
            {
                isStop();
            }
            else
            {
                transform.rotation = Quaternion.Lerp(transform.rotation, CharacterRot, playerData.rotSpeed);
                Chara.Move(MoveDir * playerData.moveSpeed * Time.deltaTime);
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
        }
    }
    #endregion

    #region 負面效果
    //暈眩 僵直
    public void GetDeBuff_Stun()
    {        
        stopAnything_Switch(true);
        StartCoroutine(MatchTimeManager.SetCountDown(Recover_Stun, 0.75f));
    }
    //緩速
    protected virtual void GetDeBuff_Slow()
    {

    }
    //破甲
    protected virtual void GetDeBuff_DestoryDef()
    {

    }
    //燒傷
    protected virtual void GetDeBuff_Burn()
    {

    }
    //擊退
    /*[PunRPC]
    protected virtual void pushOtherTarget(Vector3 _dir, float _dis)
    {
        this.transform.DOMove(transform.localPosition + _dir.normalized * _dis, .8f).SetEase(Ease.OutBounce);
        Quaternion Rot = Quaternion.LookRotation(-_dir.normalized);
        this.transform.rotation = Rot;
    }*/

    void Recover_Stun()
    {
        GoBack_AtkState();
        //if (!photonView.isMine)
        //    GetComponent<PhotonTransformView>().enabled = true;
    }
    #endregion

    #region 功能
    //消耗能量
    private bool ConsumeAP(float _value)
    {
        if (playerData.Ap_original - _value >= 0)
        {
            playerData.Ap_original -= _value;
            leftTopPowerBar.fillAmount = playerData.Ap_original / playerData.Ap_Max;
            return true;
        }
        else
        {
            hintManager.CreatHint("能量不足");
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
        getIsRunning = false;
        nav.ResetPath();
        CharacterRot = Quaternion.LookRotation(_dir.normalized);
        transform.rotation = CharacterRot;
        Net.RPC("GoDodge", PhotonTargets.All, _dir);
        StartCoroutine(MatchTimeManager.SetCountDown(Dodge_End, playerData.Dodget_Delay));
    }
    //閃避結束執行
    void Dodge_End()
    {
        //   MySkill = skillData.None;
        canDodge = true;
    }

    [PunRPC]
    public void Skill_Q()
    {
        Invoke("testCatch", .6f);
    }

    public void testCatch()
    {
        skill_Q.Invoke();
    }
    //技能Q冷卻時間
    void CountDown_Q()
    {
        canSkill_Q = true;
    }
    public void KillAllSkill()
    {
        cancelSkill.Invoke();
    }
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
    }
    //回攻擊狀態
    public void GoBack_AtkState()
    {
        print("atkover");
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
    #endregion

    #region 偵測目前是否有路徑
    public bool getNavPath()
    {
        return nav.hasPath;
    }
    #endregion

    #region 死亡
    public IEnumerator Death()
    {
        stopAnything_Switch(true);
        Chara.enabled = false;
        canDodge = true;
        if (photonView.isMine)
        {
            CameraEffect.instance.nowDie(true);
            Net.RPC("Die", PhotonTargets.All);
            Creatplayer.instance.player_ReBorn(playerData.ReBorn_CountDown);
        }
        yield return new WaitForSeconds(3f);
        if (photonView.isMine)
        {
            Net.RPC("SetActiveF", PhotonTargets.All);            
        }
    }
    #endregion
}
