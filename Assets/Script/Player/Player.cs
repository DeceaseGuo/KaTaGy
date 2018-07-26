using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(NavMeshAgent))]
public class Player : Photon.MonoBehaviour
{
    public PlayerData.PlayerDataBase playerData;
    public PlayerData.PlayerDataBase originalData;
    private PlayerAni AniControll;
    private BuildManager buildManager;
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

    #region 狀態
    public enum statesData
    {
        None,
        canMove_Atk,
        canMvoe_Build,
        UAV,
        beHit,
        Combo
    }
    private statesData myState = statesData.canMove_Atk;
    public statesData MyState { get { return myState; } set { myState = value; } }
    public enum skillData
    {
        None,
        Dodge
    }
    private skillData mySkill = skillData.None;
    public skillData MySkill { get { return mySkill; } private set { mySkill = value; } }
    #endregion

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
            //MyCore.P_ATK += UpdateAtkData;
        }
        else
        {
            nav.enabled = false;
            this.enabled = false;
        }

        //    InvokeRepeating("TestAtk", 2f, TestAtkTime);
    }

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

        if (Input.GetKeyDown(KeyCode.F3))
            StartCoroutine(Death());
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
                ClickPoint();
                UAV_Btn(statesData.UAV);

                CharacterAtk_Q();
                Dodge_Btn();
                ATK_Build_Btn();
                break;
            case statesData.canMvoe_Build:
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
            case statesData.beHit:
                Dodge_Btn();
                break;
            case statesData.Combo:
                comboRayPoint();
                CharacterAtk_Q();
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
            Dodge_FCN(nowMouseDir());
            ConsumeAP(20f);
            MySkill = skillData.Dodge;
            MyState = statesData.canMove_Atk;
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
    //按下Q→目前為combo
    private void CharacterAtk_Q()
    {
        if (Input.GetKeyDown("q") && AniControll.canClick)
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
    #endregion

    #region 偵測點擊位置
    void ClickPoint()
    {
        ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out hit, 150, canClickToMove_Layer))
        {
            mousePosition = hit.point;
            nowMouseDir();
            if (hit.transform.tag == "CanClickMove")
            {
                if (Input.GetMouseButtonDown(1))
                {
                    clickPointPos.transform.position = hit.point;
                    getTatgetPoint(clickPointPos.transform.position);

                }
            }

            if (Input.GetKeyDown(KeyCode.LeftShift))
            {

                transform.position = hit.point;
            }
        }
    }
    //combo狀態時偵測滑鼠位置用
    void comboRayPoint()
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
        arrow.transform.rotation = Quaternion.LookRotation(tmpDir);
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
            Vector3 tmpNextPos = nav.steeringTarget - transform.position;
            tmpNextPos.y = transform.localPosition.y;
            if (tmpNextPos != Vector3.zero)
            {
                CharacterRot = Quaternion.LookRotation(tmpNextPos);
                nextTargetRot.rotation = CharacterRot;
                MoveDir = nextTargetRot.forward;
            }
            #endregion

            #region 判斷是否到最終目標點→否則執行移動
            Vector3 maxDisGap = nav.destination - transform.position;
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
            MyState = statesData.beHit;
        }
    }
    #endregion

    #region 功能
    //消耗能量
    private void ConsumeAP(float _value)
    {
        if (playerData.Ap_original - _value >= 0)
        {
            playerData.Ap_original -= _value;
            leftTopPowerBar.fillAmount = playerData.Ap_original / playerData.Ap_Max;
        }
        else
            hintManager.CreatHint("能量不足");
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
        //AniControll.GoDodge(_dir);
        StartCoroutine(Dodge_Delay(playerData.Dodget_Delay-0.3f));
    }
    //閃避功能延遲
    IEnumerator Dodge_Delay(float _time)
    {
        yield return new WaitForSeconds(0.3f);
        MySkill = skillData.None;
        yield return new WaitForSeconds(_time);
        canDodge = true;
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
}
