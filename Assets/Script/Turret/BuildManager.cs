using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using AtkTower;

public class BuildManager : MonoBehaviour
{
    public static BuildManager instance;
    private PlayerObtain playerObtain;
    [HideInInspector]
    public UIManager uiManager;

    public Grid_Snap grid_snap;

    [Header("鷹架")]
    [SerializeField] PhotonView build_Scaffolding;
    [SerializeField] GameObject build_CD_Obj;
    [SerializeField] Image build_CD_Bar;

    [Header("目標")]
    public GameObject builder;
    public Player playerScript;
    [HideInInspector]
    public Vector3 currentPlayerPos;
    //塔防
    private TurretData turretData;
    private TurretData.TowerDataBase turretToBuild;
    private GameObject detectObj;

    public bool stopBuild = false;
    //偵測
    private GameObject detectObjectPrefab;
    private GameObject lastDetectObj;
    private GameObject TmpObj;

    public bool nowBuilding = false;  //是否在建造模式
    public bool nowSelect = true;   //是否可以按下按鈕
    private bool haveTower;
    public bool HaveTower { get { return haveTower; } private set { haveTower = value; } }

    private SceneObjManager sceneObjManager;
    public SceneObjManager SceneManager { get { if (sceneObjManager == null) sceneObjManager = SceneObjManager.Instance; return sceneObjManager; } }
    
    ObjectPooler objPool;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != null)
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        playerObtain = PlayerObtain.instance;
        uiManager = UIManager.instance;
        builder = Creatplayer.instance.Player_Script.gameObject;
        playerScript = Creatplayer.instance.Player_Script;
        turretData = TurretData.instance;
        objPool = ObjectPooler.instance;
    }

    #region 付款
    public bool payment(bool _paid)
    {
        if (_paid)
        {
            playerObtain.consumeResource(turretToBuild.cost_Money);
        }
        return true;
    }
    #endregion

    #region 商店選擇的塔防與偵測器
    public void SelectToBuild(TurretData.TowerDataBase turret, GameObject detect)
    {
        HaveTower = true;
        turretToBuild = turret;

        closeNowDetectObj();//關掉原本開啟的
        detectObjectPrefab = detect;
        detectObjectPrefab.SetActive(true);

        for (int i = 0; i < SceneManager.myElectricityObjs.Count; i++)
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

        for (int i = 0; i < SceneManager.myElectricityObjs.Count; i++)
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
        TmpObj = objPool.getPoolObject(turretToBuild.tspObject_Name, _pos, Quaternion.identity);
    }

    public void closeTmpObj()
    {
        if (TmpObj != null)
        {
            returnPoolTower(turretToBuild.tspObject_Name, TmpObj);
            TmpObj = null;
        }
    }
    #endregion

    #region 生成 與 關閉塔防 從物件池
    public GameObject creatTower(GameManager.whichObject _name, Vector3 _pos, Quaternion _rot)
    {
        GameObject towerObj = objPool.getPoolObject(_name, _pos, _rot);
        return towerObj;
    }

    public void returnPoolTower(GameManager.whichObject _name, GameObject _tower)
    {
        objPool.Repool(_name, _tower);
    }
    #endregion

    #region 鷹架的開啟 和 關閉
    public void openScaffolding(Vector3 _pos)
    {
        if (build_Scaffolding == null)
        {
            build_Scaffolding = PhotonNetwork.Instantiate("Scaffolding", _pos, Quaternion.identity, 0).GetComponent<PhotonView>();
        }
        else
        {
            build_Scaffolding.RPC("SetActiveT", PhotonTargets.All, _pos);
        }

        build_CD_Obj.transform.position = _pos + new Vector3(0, 7.5f, 0);
        build_CD_Obj.SetActive(true);
    }

    public void closeScaffolding()
    {
        build_Scaffolding.RPC("SetActiveF", PhotonTargets.All);
        build_CD_Obj.SetActive(false);
    }
    #endregion

    #region 返回資源懲罰
    public void cancelPunish(float _percent)
    {
        int _Money = Mathf.RoundToInt(turretToBuild.cost_Money * _percent);

        playerObtain.obtaniResource(_Money);
        closeTmpObj();
    }
    #endregion

    #region 鷹架出現時蓋塔時間
    public void build_countDown(float _cd)
    {
        build_CD_Bar.fillAmount = _cd / turretToBuild.turret_delayTime;
    }
    #endregion

    #region 關閉選擇塔防
    public void closeTurretToBuild()
    {
        nowNotSelectSwitch(true);
        HaveTower = false;
        turretToBuild = new TurretData.TowerDataBase();
    }
    #endregion

    #region 將選擇的塔防傳給其他腳本
    public TurretData.TowerDataBase GetTurretToBuild()
    {
        return turretToBuild;
    }
    #endregion

    #region 扣電
    public void consumeElectricity(List<Electricity> e, Turret_Manager tur_manager)
    {
        for (int i = 0; i < e.Count; i++)
        {
            if (Vector3.Distance(tur_manager.transform.position, e[i].transform.position) <= e[i].range)
            {
                tur_manager.power = e[i];
                e[i].firstE.connectTowers.Add(tur_manager.gameObject);
                int t = findCost_Electricity(tur_manager.DataName);
                e[i].firstE.Use_Electricit(-t);
                //Debug.LogFormat("{0}扣除電量:{1}，剩餘電量:{2}", item.firstE.name, t, item.firstE.resource_Electricity);
                return;
            }
            else
            {
                tur_manager.power = null;
            }
        }
    }
    #endregion

    #region 退回電量
    public void obtaniElectricity(Turret_Manager tur_manager)
    {
        if (tur_manager.power != null)
        {
            int t = findCost_Electricity(tur_manager.DataName);
            tur_manager.power.firstE.Use_Electricit(t);
            //Debug.LogFormat("{0}回復電量:{1}，剩餘電量:{2}", tur_manager.power.firstE.name, t, tur_manager.power.firstE.resource_Electricity);
        }
    }
    #endregion

    #region 找物件消耗電力
    public int findCost_Electricity(GameManager.whichObject _name)
    {
        return turretData.getTowerData(_name).cost_Electricity;
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

            if (Vector3.Distance(_build.transform.position, electricities[a].transform.position) <= _build.range * 2)
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
}