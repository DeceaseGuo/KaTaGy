using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using UnityEngine.Events;

//[RequireComponent(typeof(NavMeshAgent))]
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
    private CapsuleCollider CharaCollider;
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

    [HideInInspector]
    public Quaternion CharacterRot;
    public PhotonView Net;
    //方向
    private Vector3 mousePosition;
    public Transform arrow;

    public UnityEvent skill_W;
    public UnityEvent skill_E;
    public UnityEvent skill_second;
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

    public enum buffData
    {
        None,
        NowCC,
        Shield //盾
    }
    private buffData nowBuff = buffData.None;
    public buffData NowBuff { get { return nowBuff; } private set { nowBuff = value; } }
    #endregion

    private bool canSkill_Q = true;
    private bool canSkill_W = true;
    private bool canSkill_E = true;
    //private bool canSkill_R = true;
    [HideInInspector]
    public bool skillSecondClick = false;

    private void Awake()
    {
        CharaCollider = GetComponent<CapsuleCollider>();
        Net = GetComponent<PhotonView>();
    }

    private void Start()
    {
        clickPointPos = GameObject.Find("clickPointPos");
        hintManager = HintManager.instance;
        buildManager = BuildManager.instance;
        nav = GetComponent<NavMeshAgent>();
        deadManager = GetComponent<isDead>();
        nav.updateRotation = false;
        nav.speed = playerData.moveSpeed;
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
            AniControll = GetComponent<PlayerAni>();
            //nav.enabled = false;
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
            arrow.gameObject.layer = 0;
            Net.RPC("changeMask_1", PhotonTargets.All);
        }
        else if (GameManager.instance.getMyPlayer() == GameManager.MyNowPlayer.player_2)
        {
            Net.RPC("changeLayer", PhotonTargets.All, 31);
            arrow.gameObject.layer = 0;
            Net.RPC("changeMask_2", PhotonTargets.All);
        }
    }
    [PunRPC]
    public void changeMask_1()
    {
        GetComponent<PlayerAni>().canAtkMask = GameManager.instance.getPlayer1_Mask;
    }
    [PunRPC]
    public void changeMask_2()
    {
        GetComponent<PlayerAni>().canAtkMask = GameManager.instance.getPlayer2_Mask;
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
        if (nav != null)
            nav.speed = playerData.moveSpeed;

        leftTopPowerBar.fillAmount = 1;
        if (originalData.headImage == null)
            originalData = PlayerData.instance.getPlayerData(GameManager.instance.Meis);
        playerData = originalData;
        playerData.Ap_original = playerData.Ap_Max;
        CharaCollider.enabled = true;
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

                transform.position = mousePosition;
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
                //UAV_Btn(statesData.UAV);
                DetectSkillBtn();
                CharacterAtk_F();
                Dodge_Btn();
                ATK_Build_Btn();
                break;
            case statesData.canMvoe_Build:
                if (Input.GetMouseButtonDown(1))
                    ClickPoint();
              //  UAV_Btn(statesData.UAV);
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
                stopAnything_Switch(true);
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
            Vector3 tmpDir = transform.forward;
            if (MyState != statesData.Combo)
            {
                getIsRunning = false;
                nav.ResetPath();
                MyState = statesData.Combo;
                tmpDir = nowMouseDir();
                transform.forward = tmpDir.normalized;
            }

            AniControll.TypeCombo(tmpDir);
        }
    }
    //技能
    private void DetectSkillBtn()
    {
        Character_Skill_Q();
        Character_Skill_W();
        Character_Skill_E();
    }
    private void Character_Skill_Q()
    {
        if (Input.GetKeyDown(KeyCode.Q) && canSkill_Q)
        {
            if (ConsumeAP(1f))
            {
                canSkill_Q = false;
                stopAnything_Switch(true);
                transform.forward = arrow.forward;
                Net.RPC("Skill_Q_Fun", PhotonTargets.All);
                StartCoroutine(MatchTimeManager.SetCountDown(CountDown_Q, playerData.skillCD_Q));
            }
        }
    }
    private void Character_Skill_W()
    {
        if (Input.GetKeyDown(KeyCode.W) && canSkill_W)
        {
            if (ConsumeAP(1f))
            {
                canSkill_W = false;
                stopAnything_Switch(true);
                transform.forward = arrow.forward;
                Net.RPC("Skill_W_Fun", PhotonTargets.All);
                StartCoroutine(MatchTimeManager.SetCountDown(CountDown_W, playerData.skillCD_W));
            }
        }
    }
    private void Character_Skill_E()
    {
        if (Input.GetKeyDown(KeyCode.E) && skillSecondClick)
        {
            skill_second.Invoke();
        }

        if (Input.GetKeyDown(KeyCode.E) && canSkill_E)
        {
            if (ConsumeAP(1f))
            {
                canSkill_E = false;
                skillSecondClick = true;                
                Net.RPC("Skill_E_Fun", PhotonTargets.All);
            }
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
    //無時無刻偵測滑鼠方向
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
        mousePosition = new Vector3(mousePosition.x, transform.localPosition.y, mousePosition.z);
        Vector3 tmpDir = mousePosition - transform.position;
        arrow.rotation = Quaternion.LookRotation(tmpDir);
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
            Vector3 tmpNextPos = nav.steeringTarget - transform.localPosition;
            tmpNextPos.y = transform.localPosition.y;
            if (tmpNextPos != Vector3.zero)
            {
                CharacterRot = Quaternion.LookRotation(tmpNextPos);
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
    [PunRPC]
    public void GetDeBuff_Stun(float _time)
    {        
        stopAnything_Switch(true);
        AniControll.anim.SetBool("StunRock", true);
        StartCoroutine(MatchTimeManager.SetCountDown(Recover_Stun, _time));
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
    //擊飛
    [PunRPC]
    public void pushOtherTarget(/*Vector3 _dir*/)
    {
        if (!deadManager.checkDead)
        {
            stopAnything_Switch(true);
            AniControll.anim.SetTrigger("HitFly");
        }
    }

    void Recover_Stun()
    {
        GoBack_AtkState();
        AniControll.anim.SetBool("StunRock", false);
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
        transform.forward = _dir.normalized;
        Net.RPC("GoDodge", PhotonTargets.All);
        StartCoroutine(MatchTimeManager.SetCountDown(Dodge_End, playerData.Dodget_Delay));
    }
    //閃避cd結束
    void Dodge_End()
    {
        //   MySkill = skillData.None;
        canDodge = true;
    }

    #region 技能相關
    //技能冷卻時間
    void CountDown_Q()
    {
        canSkill_Q = true;
    }
    void CountDown_W()
    {
        canSkill_W = true;
    }

    public void GoCountDownE()
    {
        StartCoroutine(MatchTimeManager.SetCountDown(CountDown_E, playerData.skillCD_E));
    }
    void CountDown_E()
    {
        canSkill_E = true;
        Debug.Log("技能E  " + "cd完成");
    }
    public void KillAllSkill()
    {
        cancelSkill.Invoke();
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
    }
    //回攻擊狀態
    public void GoBack_AtkState()
    {
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

    void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.isWriting)
        {
            stream.SendNext(playerData.Hp_original);
            stream.SendNext(playerData.Hp_Max);
            stream.SendNext(playerData.Atk_Damage);
        }
        else
        {
            playerData.Hp_original = (float)stream.ReceiveNext();
            playerData.Hp_Max = (float)stream.ReceiveNext();
            playerData.Atk_Damage = (float)stream.ReceiveNext();
            if (playerData.Hp_Max != originalData.Hp_Max)
                GetComponent<Attribute_HP>().UI_HpBar.fillAmount = playerData.Hp_original / playerData.Hp_Max;
        }
    }


    #region 死亡
    public IEnumerator Death()
    {
        stopAnything_Switch(true);
        CharaCollider.enabled = false;
        AniControll.Die();
        if (photonView.isMine)
        {
            CameraEffect.instance.nowDie(true);
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
