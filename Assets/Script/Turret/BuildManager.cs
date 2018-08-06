using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using AtkTower;

public class BuildManager : MonoBehaviour
{
    public static BuildManager instance;
    private PlayerObtain playerObtain;
    private UIManager uiManager;

    [SerializeField] Grid_Snap grid_snap;

    [Header("鷹架")]
    [SerializeField] GameObject build_Scaffolding;
    [SerializeField] GameObject build_CD_Obj;
    [SerializeField] Image build_CD_Bar;

    [Header("目標")]
    public GameObject builder;
    public Player playerScript;

    //塔防
    private TurretData.TowerDataBase turretToBuild;
    private GameObject detectObj;
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

    TurretData turretData;

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
        builder = Creatplayer.instance.MyNowPlayer;
        playerScript = Creatplayer.instance.Player_Script;
        turretData = TurretData.instance;
    }

    [SerializeField] GameObject DestoryObj;
    private void Update()
    {
        if (Input.GetKeyDown("a"))
        {
            Testdead(DestoryObj);
        }
    }

    #region 測試用死亡方法
    void Testdead(GameObject _obj)
    {
        GameManager.NowTarget _who = _obj.GetComponent<isDead>().myAttributes;
        switch (_who)
        {
            case (GameManager.NowTarget.Soldier):
                _obj.GetComponent<PhotonView>().RPC("takeDamage", PhotonTargets.All, 0, 1000f);
                break;
            case (GameManager.NowTarget.Player):
                _obj.GetComponent<PhotonView>().RPC("takeDamage", PhotonTargets.All, 1000f, Vector3.zero, false);
                break;
            case (GameManager.NowTarget.Tower):
                _obj.GetComponent<PhotonView>().RPC("takeDamage", PhotonTargets.All, 1000f);
                break;
            case (GameManager.NowTarget.Core):
                break;
            default:
                return;
        }
    }
    #endregion

    #region 付款
    public bool payment(bool _paid)
    {
        if (_paid)
        {
            playerObtain.consumeResource(turretToBuild.cost_Ore, turretToBuild.cost_Money);
        }
        return true;
    }
    #endregion

    #region 商店選擇的塔防與偵測器
    public void SelectToBuild(TurretData.TowerDataBase turret, GameObject detect)
    {
        if (nowSelect)
        {
            HaveTower = true;
            turretToBuild = turret;
            lastDetectObj = detectObjectPrefab;
            detectObjectPrefab = detect;
            closeLastDetectObj();
            //調整位置
            Vector3 tmpMousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            detectObjectPrefab.transform.position = tmpMousePos;
            detectObjectPrefab.SetActive(true);

            foreach (var item in SceneManager.myElectricityObjs)
            {
                item.changeGridColor(turretToBuild.cost_Electricity);
            }
        }
    }
    #endregion

    #region 關閉選擇塔防功能
    public void nowNotSelectSwitch(bool _switch)
    {
        if (!_switch)
        {
            nowSelect = _switch;
            closeNowDetectObj();
        }
        else
        {
            nowSelect = _switch;
        }
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
            else
            {
                grid_snap.openGrid();
                uiManager.OpenTowerMenu();
                nowBuilding = true;
                playerScript.switchWeapon(true);
            }
        }
    }
    #endregion

    #region 取消目前的選擇
    public void cancelSelect()
    {
        closeTurretToBuild();
        closeNowDetectObj();

        foreach (var item in SceneManager.myElectricityObjs)
        {
            item.changeGridColor(0);
        }
    }
    #endregion

    #region 關閉上一個 和 關閉目前塔防偵測
    void closeLastDetectObj()
    {
        if (lastDetectObj != null)
            lastDetectObj.SetActive(false);
    }

    void closeNowDetectObj()
    {
        if (detectObjectPrefab != null)
            detectObjectPrefab.SetActive(false);
    }
    #endregion

    #region 創建 和 關閉蓋塔提示透明物件
    public void creatTmpObj(Vector3 _pos)
    {
        TmpObj = ObjectPooler.instance.getPoolObject(turretToBuild.tspObject_Name, _pos, Quaternion.identity);
    }

    public void closeTmpObj()
    {
        ObjectPooler.instance.Repool(turretToBuild.tspObject_Name, TmpObj);
    }
    #endregion

    #region 生成 與 關閉塔防 從物件池
    public GameObject creatTower(GameManager.whichObject _name, Vector3 _pos, Quaternion _rot)
    {
        GameObject towerObj = ObjectPooler.instance.getPoolObject(_name, _pos, _rot);
        return towerObj;
    }

    public void returnPoolTower(GameManager.whichObject _name,GameObject _tower)
    {
        ObjectPooler.instance.Repool(_name, _tower);
    }
    #endregion

    #region 鷹架的開啟 和 關閉
    public void openScaffolding(Vector3 _pos)
    {
        if (build_Scaffolding == null)
        {
            build_Scaffolding = PhotonNetwork.Instantiate("Scaffolding", _pos, Quaternion.identity, 0);
        }
        else
        {
            build_Scaffolding.transform.position = _pos;
            build_Scaffolding.SetActive(true);
            build_Scaffolding.GetComponent<PhotonView>().RPC("SetActiveT", PhotonTargets.Others, _pos);
        }

        build_CD_Obj.transform.position = _pos + new Vector3(0, 7.5f, 0);
        build_CD_Obj.SetActive(true);
    }

    public void closeScaffolding()
    {
        build_Scaffolding.SetActive(false);
        build_Scaffolding.GetComponent<PhotonView>().RPC("SetActiveF", PhotonTargets.Others);
        build_CD_Obj.SetActive(false);
    }
    #endregion

    #region 返回資源懲罰
    public void cancelPunish(float _percent)
    {
        int _ore = Mathf.RoundToInt( turretToBuild.cost_Ore * _percent);
        int _Money = Mathf.RoundToInt(turretToBuild.cost_Money * _percent);

        playerObtain.obtaniResource(_ore, _Money);
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
        foreach (var item in e)
        {
            if (Vector3.Distance(tur_manager.transform.position, item.transform.position) <= item.range)
            {
                tur_manager.power = item;
                item.firstE.connectTowers.Add(tur_manager.gameObject);
                int t = findCost_Electricity(tur_manager.DataName);
                item.firstE.Use_Electricit(-t);
                Debug.LogFormat("{0}扣除電量:{1}，剩餘電量:{2}", item.firstE.name, t, item.firstE.resource_Electricity);
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
            Debug.LogFormat("{0}回復電量:{1}，剩餘電量:{2}", tur_manager.power.firstE.name, t, tur_manager.power.firstE.resource_Electricity);
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
            Debug.Log(_build.firstE);
            Debug.Log(_build.name + "electricities.Count < 1");
            return;
        }
        if (_build == _build.firstE)
        {
            Debug.Log(_build.name + "_build == _build.firstE");
            return;
        }

        foreach (var item in electricities)
        {

            if (_build == item)
            {
                Debug.Log(_build.name + "_build == item");
                continue;
            }

            if (Vector3.Distance(_build.transform.position, item.transform.position) <= _build.range * 2)
            {
                if (!_build.myTouch.Contains(item))
                {
                    _build.myTouch.Add(item);
                    item.myTouch.Add(_build);
                }

                if (item.firstE == null)
                {
                    if (_build.firstE == null)
                        _build.firstE = _build;
                    Debug.Log(_build.name + "item.firstE == null");
                    return;
                }

                #region 改變firstE
                if (_build.firstE == null)
                {
                    _build.firstE = item.firstE;
                    _build.firstE.connectElectricitys.Add(_build);
                    _build.firstE.resource_Electricity += _build.resource_Electricity;

                    Debug.Log(_build.name + "_build.firstE == null");
                }
                else if (item.firstE != _build.firstE)
                {
                    _build.firstE.resource_Electricity += item.firstE.resource_Electricity;
                    _build.firstE.connectElectricitys.Add(item.firstE);

                    foreach (var i in item.firstE.connectTowers)
                        _build.firstE.connectTowers.Add(i);

                    item.firstE.connectTowers.Clear();
                    foreach (var i in item.firstE.connectElectricitys)
                    {
                        _build.firstE.connectElectricitys.Add(i);
                        if (i != item)
                            i.firstE = _build.firstE;
                    }
                    item.firstE.connectElectricitys.Clear();

                    if (item != item.firstE)
                        item.firstE.firstE = _build.firstE;

                    item.firstE = _build.firstE;
                    Debug.Log(_build.name + "item.firstE != _build.firstE");
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