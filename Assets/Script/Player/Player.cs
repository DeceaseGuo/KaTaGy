using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(NavMeshAgent))]
public class Player : Photon.MonoBehaviour
{
    public PlayerData.PlayerDataBase playerData;
    public PlayerData.PlayerDataBase originalData;
    private PlayerAni AniControll;
    private BuildManager buildManager;
    private bool nowStop = false;
    [HideInInspector] public isDead deadManager;
    private bool nowCombo = false;
    private CharacterController Chara;
    private NavMeshAgent nav;

    private Vector3 MoveDir = Vector3.zero;  //角色方向
    [SerializeField] Transform nextTargetRot;
    [SerializeField] GameObject clickPointPos;
    [SerializeField] LayerMask canClickToMove_Layer;
    [SerializeField] LayerMask currentDir_Layer;
    private bool isRunning;
    private Quaternion CharacterRot;
    public PhotonView Net;
    //方向
    private Vector3 mousePosition;    

    private void Awake()
    {
        Chara = GetComponent<CharacterController>();
        MoveDir = transform.forward;

        Net = GetComponent<PhotonView>();
    }

    private void Start()
    {
        clickPointPos = GameObject.Find("clickPointPos");
        nextTargetRot = GameObject.Find("Detect").transform;

        buildManager = BuildManager.instance;
        nav = GetComponent<NavMeshAgent>();
        deadManager = GetComponent<isDead>();
        nav.updateRotation = false;
            checkCurrentPlay();
        if (photonView.isMine)
        {
            formatData();
        }
       /* else if (!photonView.isMine)
        {
            this.enabled = false;
            return;
        }*/
    }

    private void Update()
    {
        if (!Application.isPlaying/* || !photonView.isMine */|| deadManager.checkDead)
            return;

       /* if (Input.GetKeyDown(KeyCode.B))
        {
            Vector3 randomPos = Random.insideUnitSphere * 20f;
            NavMeshHit navHit;
            NavMesh.SamplePosition(transform.position + randomPos, out navHit, 10f, NavMesh.AllAreas);
            getTatgetPoint(navHit.position);
        }*/


        if (Input.GetKeyDown(KeyCode.Space))
        {
            SmoothFollow.instance.switch_UAV();
            if (BuildManager.instance.nowBuilding)
            {
                BuildManager.instance.BuildSwitch();
            }
        }

        if (!nowStop)
        {
            if (Input.GetKeyDown("q") && AniControll.canClick && !buildManager.nowBuilding && photonView.isMine)
            {
                //CharacterAtk_Q();
                Vector3 tmpDir = Vector3.zero;
                isRunning = false;
                nav.ResetPath();
                if (!nowCombo)
                {
                    tmpDir = mousePosition - transform.position;
                    tmpDir.y = 0;
                    CharacterRot = Quaternion.LookRotation(tmpDir.normalized);
                    transform.rotation = CharacterRot;
                }
                AniControll.TypeCombo(tmpDir);

                GetComponent<PhotonView>().RPC("CharacterAtk_Q", PhotonTargets.Others, tmpDir, CharacterRot);

            }

            if (nowCombo && photonView.isMine)
            {
                AniControll.DetectAtkRanage();
            }
            else
            {
                if (photonView.isMine)
                {
                    ClickPoint();
                }
                
                CharacterRun();
                // CharacterJump();
            }
        }
    }

    #region 恢復初始數據
    void formatData()
    {
        originalData = PlayerData.instance.getPlayerData(GameManager.instance.Meis);
        playerData = originalData;
    }
    #endregion

    #region 目前為那個角色
    public void checkCurrentPlay()
    {
        switch (GameManager.instance.Meis)
        {
            case GameManager.meIs.Allen:
                AniControll = GetComponent<Allen_Ani>();
                if (photonView.isMine)
                {
                    buildManager.builder = this.gameObject;
                    buildManager.playerScript = this;
                }
                
                return;
            case GameManager.meIs.Queen:
                AniControll = GetComponent<Queen_Ani>();
                if (photonView.isMine)
                {
                    buildManager.builder = this.gameObject;
                    buildManager.playerScript = this;
                }
                return;
            default:
                return;
        }
    }
    #endregion

    #region 偵測點擊位置
    void ClickPoint()
    {
        if (nowCombo)
            return;

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 1000, canClickToMove_Layer))
        {
            mousePosition = hit.point;
            if (hit.transform.tag == "CanClickMove")
            {
                if (Input.GetMouseButtonDown(1) && photonView.isMine)
                {
                    clickPointPos.transform.position = hit.point;
                    //getTatgetPoint(clickPointPos.transform.position);
                    GetComponent<PhotonView>().RPC("getTatgetPoint", PhotonTargets.All, clickPointPos.transform.position);
                }
            }
        }
    }
    #endregion

    #region 得到移動終點位置
    [PunRPC]
    public void getTatgetPoint(Vector3 tragetPoint)
    {
        nav.SetDestination(tragetPoint);
        isRunning = true;
        AniControll.Ani_Run(isRunning);
    }
    #endregion

    #region 角色移動
    void CharacterRun()
    {
        if (isRunning)
        {
          /*  if (!Chara.isGrounded)
            {
                Debug.Log("不在地板上");
                return;
            }*/

            #region 尋找下一個位置方向
            Vector3 tmpNextPos = nav.steeringTarget - transform.position;
            tmpNextPos.y = 0;
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

    void CharacterJump()
    {
        if (Chara.isGrounded)
        {
            MoveDir.y -= playerData.GV * Time.deltaTime;
            if (Input.GetKeyDown(KeyCode.Space))
            {
                Debug.Log("jump");
            }
        }
        else
        {
            MoveDir.y -= playerData.GV * Time.deltaTime;
        }
    }

    [PunRPC]
    public void CharacterAtk_Q(Vector3 MyDir, Quaternion MyRot)
    {
        isRunning = false;
        nav.ResetPath();

        transform.rotation = MyRot;
        AniControll.TypeCombo(MyDir);
    }

    void CharacterAtk_W()
    {
        isStop();        
    }

    #region 停止角色移動
    public void isStop()
    {
        if (getIsRunning())
        {
            isRunning = false;
            nav.ResetPath();
            AniControll.Ani_Run(isRunning);
        }
    }
    #endregion

    #region 偵測目前跑步動畫
    public bool getIsRunning()
    {
        if (isRunning)
            return true;
        else
            return false;
    }
    #endregion

    #region 切換目前模式(攻擊 , 建造)
    public void switchWeapon(bool _can)
    {
        if (!deadManager.checkDead)
            AniControll.switchWeapon_Pattren(_can);
    }
    #endregion

    #region 停止除了combo之外一切動作
    public void stopExceptCombo(bool _combo)
    {
        nowCombo = _combo;
    }
    #endregion

    #region 停止一切行為(無法操控)
    public void stopAnything_Switch(bool _stop)
    {
        isStop();
        nowStop = _stop;
    }
    #endregion
}
