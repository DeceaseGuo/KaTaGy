using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine;
using UnityEngine.UI;
using AtkTower;

public class BuildManager : MonoBehaviour
{
    #region 取得單例
    private MatchTimer matchTime;
    private MatchTimer MatchTimeManager { get { if (matchTime == null) matchTime = MatchTimer.Instance; return matchTime; } }

    private SceneObjManager sceneObjManager;
    private SceneObjManager SceneManager { get { if (sceneObjManager == null) sceneObjManager = SceneObjManager.Instance; return sceneObjManager; } }

    private HintManager hintManager;
    private HintManager HintScript { get { if (hintManager == null) hintManager = HintManager.instance; return hintManager; } }

    private ObjectPooler poolManager;
    protected ObjectPooler PoolManager { get { if (poolManager == null) poolManager = ObjectPooler.instance; return poolManager; } }
    #endregion

    public static BuildManager instance;
    private PlayerObtain playerObtain;
    [HideInInspector]
    public UIManager uiManager;

    public Grid_Snap grid_snap;

    [Header("鷹架")]
    [SerializeField] PhotonView build_Scaffolding;
    public CanvasGroup build_CD_Obj;
    public Image build_CD_Bar;

    [Header("目標")]
    public Transform builder;
    public Player playerScript;
    [HideInInspector]
    public Vector3 currentPlayerPos;
    //塔防
    private TurretData.TowerDataBase turretToBuild;
    private GameObject detectObj;

    //偵測
    private GameObject detectObjectPrefab;
    private GameObject TmpObj;

    public bool nowBuilding = false;  //是否在建造模式
    public bool nowSelect = true;   //是否可以按下按鈕
    private bool haveTower;  

    private Vector3 NodePos;
    

    public LayerMask canBuild;
    public bool ifCanBuild;

    [Header("stop")]
    [SerializeField] LayerMask stopMask;
    private bool _start; //前往蓋塔位子(移動)
    private bool nowBuild;//現在是否正在蓋塔
    private byte cancelBuildIndex = 0;//蓋塔取消

    #region 緩存
    private int eAmount;
    private Vector3 withTowerDis;
    private SnapGrid_Pos gridPosScript;
    #endregion

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != null)
            Destroy(gameObject);
    }

    private void Start()
    {
        playerObtain = PlayerObtain.instance;
        uiManager = UIManager.instance;
        builder = Creatplayer.instance.Player_Script.transform;
        playerScript = Creatplayer.instance.Player_Script;
    }

    #region 付款
    public void Payment()
    {
        playerObtain.consumeResource(turretToBuild.cost_Money);
    }
    #endregion

    #region 商店選擇的塔防與偵測器
    public void SelectToBuild(TurretData.TowerDataBase turret, GameObject detect)
    {
        haveTower = true;
        turretToBuild = turret;
        gridPosScript = turretToBuild.detectObjPrefab.GetComponentInChildren<SnapGrid_Pos>();
        closeNowDetectObj();//關掉原本開啟的
        detectObjectPrefab = detect;
        detectObjectPrefab.SetActive(true);

        eAmount = SceneManager.myElectricityObjs.Count;
        for (int i = 0; i < eAmount; i++)
        {
            SceneManager.myElectricityObjs[i].changeGridColor(turretToBuild.cost_Electricity);
        }
    }
    #endregion

    #region 關閉選擇塔防
    public void nowNotSelectSwitch(bool _switch)
    {
        if (!_switch)//執行建造了
        {
            closeNowDetectObj();
        }
        nowSelect = _switch;
    }
    #endregion

    #region 建築模式的開關
    public void BuildSwitch()
    {
        if (nowSelect)
        {
            if (nowBuilding)//關閉建築模式
            {
                nowBuilding = false;
                uiManager.CloseTowerMenu();
                grid_snap.closGrid();
                cancelSelect();
                playerScript.switchWeapon(false);
            }
            else//開起建築模式
            {
                nowBuilding = true;
                uiManager.OpenTowerMenu();
                grid_snap.openGrid();
                playerScript.switchWeapon(true);
            }
        }
    }
    #endregion

    #region 取消目前的選擇    
    public void cancelSelect()
    {
        closeTurretToBuild();//清除目前選擇的塔防
        closeNowDetectObj();//關掉Detect
        closeTmpObj();
        eAmount = SceneManager.myElectricityObjs.Count;
        for (int i = 0; i < eAmount; i++)
        {
            SceneManager.myElectricityObjs[i].changeGridColor(0);
        }
    }
    #endregion

    #region 關閉目前塔防偵測
    void closeNowDetectObj()
    {
        if (detectObjectPrefab != null)
            detectObjectPrefab.SetActive(false);
    }
    #endregion

    #region 創建 和 關閉蓋塔提示透明物件
    public void creatTmpObj(Vector3 _pos)
    {
        TmpObj = PoolManager.getPoolObject(turretToBuild.tspObject_Name, _pos, Quaternion.identity);
    }

    public void closeTmpObj()
    {
        if (TmpObj != null)
        {
            PoolManager.Repool(turretToBuild.tspObject_Name, TmpObj);
            TmpObj = null;
        }
    }
    #endregion

    #region 鷹架的開啟 和 關閉
    public void openScaffolding(Vector3 _pos)
    {
        if (build_Scaffolding != null)
            build_Scaffolding.RPC("SetActiveT", PhotonTargets.All, _pos);
        else
            build_Scaffolding = PhotonNetwork.Instantiate("Scaffolding", _pos, Quaternion.identity, 0).GetComponent<PhotonView>();

        build_CD_Obj.transform.position = _pos;
        build_CD_Obj.alpha = 1;
    }

    public void closeScaffolding()
    {
        build_Scaffolding.RPC("SetActiveF", PhotonTargets.All);
        build_CD_Obj.alpha = 0;
    }
    #endregion

    #region 返回資源懲罰
    public void cancelPunish(float _percent)
    {
        playerObtain.obtaniResource(Mathf.RoundToInt(turretToBuild.cost_Money * _percent));
    }
    #endregion

    #region 關閉選擇塔防
    public void closeTurretToBuild()
    {
        nowNotSelectSwitch(true);
        haveTower = false;
    }
    #endregion

    #region 扣電
    public void consumeElectricity(List<Electricity> e, Turret_Manager tur_manager)
    {
        for (int i = 0; i < e.Count; i++)
        {
            if (Vector3.SqrMagnitude(tur_manager.transform.position - e[i].transform.position) <= e[i].range * e[i].range)
            {
                tur_manager.power = e[i];
                e[i].firstE.connectTowers.Add(tur_manager);
                e[i].firstE.Use_Electricit(-(tur_manager.GetMyElectricity()));
                //Debug.LogFormat("{0}扣除電量:{1}，剩餘電量:{2}", item.firstE.name, t, item.firstE.resource_Electricity);     
                break;
            }
        }
    }
    #endregion

    #region 找firstE
    public void FindfirstE(List<Electricity> electricities, Electricity _build)
    {
        if (electricities.Count <= 1)
        {
            _build.firstE = _build;
            //Debug.Log(_build.name + "electricities.Count < 1");
            return;
        }
        if (_build == _build.firstE)
        {
            //Debug.Log(_build.name + "_build == _build.firstE");
            return;
        }

        for (int a = 0; a < electricities.Count; a++)
        {
            if (_build == electricities[a])
            {
                //Debug.Log(_build.name + "_build == item");
                continue;
            }

            if (Vector3.SqrMagnitude(_build.transform.position - electricities[a].transform.position) <= _build.range * _build.range * 4)
            {
                if (!_build.myTouch.Contains(electricities[a]))
                {
                    _build.myTouch.Add(electricities[a]);
                    electricities[a].myTouch.Add(_build);
                }

                if (electricities[a].firstE == null)
                {
                    if (_build.firstE == null)
                        _build.firstE = _build;
                    //Debug.Log(_build.name + "item.firstE == null");
                    return;
                }

                #region 改變firstE
                if (_build.firstE == null)
                {
                    _build.firstE = electricities[a].firstE;
                    _build.firstE.connectElectricitys.Add(_build);
                    _build.firstE.resource_Electricity += _build.resource_Electricity;

                    //Debug.Log(_build.name + "_build.firstE == null");
                }
                else if (electricities[a].firstE != _build.firstE)
                {
                    _build.firstE.resource_Electricity += electricities[a].firstE.resource_Electricity;
                    _build.firstE.connectElectricitys.Add(electricities[a].firstE);

                    for (int i = 0; i < electricities[a].firstE.connectTowers.Count; i++)
                    {
                        _build.firstE.connectTowers.Add(electricities[a].firstE.connectTowers[i]);
                    }

                    electricities[a].firstE.connectTowers.Clear();
                    for (int i = 0; i < electricities[a].firstE.connectElectricitys.Count; i++)
                    {
                        _build.firstE.connectElectricitys.Add(electricities[a].firstE.connectElectricitys[i]);
                        if (electricities[a].firstE.connectElectricitys[i] != electricities[a])
                            electricities[a].firstE.connectElectricitys[i].firstE = _build.firstE;
                    }
                    electricities[a].firstE.connectElectricitys.Clear();

                    if (electricities[a] != electricities[a].firstE)
                        electricities[a].firstE.firstE = _build.firstE;

                    electricities[a].firstE = _build.firstE;
                    //Debug.Log(_build.name + "item.firstE != _build.firstE");
                }
                #endregion
            }
        }

        if (_build.firstE == null)
        {
            _build.firstE = _build;
        }
    }
    #endregion

    ///////////////////////////////////////蓋塔防相關
    public void NeedToUpdate()
    {
        if (nowBuilding && haveTower)
        {
            if (nowSelect)
            {
                FindCorrectPos();
            }

            if (!nowBuild)
            {
                if (Input.GetMouseButtonDown(1))
                {
                    print("取消建造");
                    cancelSelect();
                    _start = false;
                }

                if (_start && !nowSelect)
                {
                    goBuild();
                }
            }
            else
            {
                if ((Input.GetKeyDown(KeyCode.Escape)) || Input.GetMouseButtonDown(1) || playerScript.deadManager.checkDead)
                {
                    nowBuild = false;
                    if (cancelBuildIndex != 0)
                        MatchTimeManager.ClearThisTask(cancelBuildIndex);
                    cancelBuildIndex = 0;
                    playerScript.switchScaffolding(false);
                    cancelPunish(0.8f);
                    closeScaffolding();
                    closeTurretToBuild();
                    playerScript.stopAnything_Switch(false);
                    HintScript.CreatHint("中斷建造");
                }
            }
        }
    }

    #region 選擇好蓋塔位子
    void FindCorrectPos()
    {
        gridPosScript.NeedToUpdate();

        if (EventSystem.current.IsPointerOverGameObject())
            return;

        if (Input.GetMouseButtonDown(0))
        {
            if (ifCanBuild)
            {
                NodePos = gridPosScript.nodePos();

                nowNotSelectSwitch(false);

                if (Vector3.SqrMagnitude(NodePos - builder.position) <= (turretToBuild.turret_buildDistance * turretToBuild.turret_buildDistance))
                {
                    Payment();
                    playerScript.stopAnything_Switch(true);
                    openScaffolding(NodePos);
                    playerScript.switchScaffolding(true);
                    DelayToBuild();
                }
                else
                {
                    creatTmpObj(NodePos);
                    playerScript.getTatgetPoint(NodePos);
                    _start = true;

                    Debug.Log("距離過遠");
                }
            }
            else
            {
                HintScript.CreatHint("此處不能建造");
            }
        }
    }
    #endregion

    #region 前往蓋塔防位置
    void goBuild()
    {
        if (CheckStopPos())
        {
            Payment();
            playerScript.stopAnything_Switch(true);
            openScaffolding(NodePos);
            closeTmpObj();
            playerScript.switchScaffolding(true);
            DelayToBuild();
            _start = false;
        }
    }
    #endregion

    #region 確認是否到達目標位置了
    bool CheckStopPos()
    {
        withTowerDis = builder.position + builder.up * 2f;
        if (Physics.Linecast(withTowerDis, withTowerDis + builder.forward * 5f, stopMask))
        {
            playerScript.isStop();
            return true;
        }
        else
        {
            if (!playerScript.getNavPath())
                playerScript.getTatgetPoint(NodePos);
            return false;
        }
    }
    #endregion

    #region 開始建造(延遲)
    void DelayToBuild()
    {
        nowBuild = true;
        cancelBuildIndex = MatchTimeManager.SetCountDownReveres(BuildFinish, turretToBuild.turret_delayTime, build_CD_Bar);
    }

    void BuildFinish()
    {
        if (nowBuild)
        {
            nowBuild = false;
            cancelBuildIndex = 0;
            playerScript.switchScaffolding(false);
            BuildTurret(NodePos);
            playerScript.stopAnything_Switch(false);
        }        
    }
    #endregion

    #region 蓋塔防
    void BuildTurret(Vector3 _pos)
    {
        closeScaffolding();

        GameObject obj = PoolManager.getPoolObject(turretToBuild.TurretName, _pos, Quaternion.identity);

        if (turretToBuild.TurretName != GameManager.whichObject.Tower_Electricity)
        {
            consumeElectricity(SceneManager.myElectricityObjs, obj.GetComponent<Turret_Manager>());
        }
        else
        {
            FindfirstE(SceneManager.myElectricityObjs, obj.GetComponent<Electricity>());
        }
        closeTurretToBuild();
    }
    #endregion
}