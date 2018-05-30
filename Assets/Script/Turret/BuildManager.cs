using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BuildManager : MonoBehaviour
{
    public static BuildManager instance;
    private PlayerObtain playerObtain;
    private UIManager uiManager;

    [SerializeField] List<Grid_Snap> grid_snap;

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

    public bool nowBuilding = false;
    public bool nowSelect = true;

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
    }

    #region 付款
    public bool payment(bool _paid)
    {
        if (playerObtain.Check_OreAmount(turretToBuild.cost_Ore) && playerObtain.Check_MoneyAmount(turretToBuild.cost_Money))
        {
            if (playerObtain.Check_ElectricityAmount(turretToBuild.cost_Electricity))
            {
                if (_paid)
                {
                    playerObtain.consumeResource(turretToBuild.cost_Ore, turretToBuild.cost_Money);
                }
                return true;
            }
            else
            {
                HintManager.instance.CreatHint("電力不足");
                closeTmpObj();
                return false;
            }
        }
        else
        {
            HintManager.instance.CreatHint("資源不足");
            closeTmpObj();
            return false;
        }
    }
    #endregion

    #region 商店選擇的塔防與偵測器
    public void SelectToBuild(TurretData.TowerDataBase turret, GameObject detect)
    {
        if (nowSelect)
        {
            turretToBuild = turret;
            lastDetectObj = detectObjectPrefab;
            detectObjectPrefab = detect;
            closeLastDetectObj();
            //調整位置
            Vector3 tmpMousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            detectObjectPrefab.transform.position = tmpMousePos;
            detectObjectPrefab.SetActive(true);
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
            if (nowBuilding)
            {                
                nowBuilding = false;
                uiManager.CloseTowerMenu();
                for (int i = 0; i < grid_snap.Count; i++)
                {
                    grid_snap[i].closGrid();
                }
                cancelSelect();
                playerScript.switchWeapon(false);
            }
            else
            {                
                for (int i = 0; i < grid_snap.Count; i++)
                {
                    grid_snap[i].openGrid();
                }
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
        build_Scaffolding.transform.position = _pos;
        build_CD_Obj.transform.position = _pos +new Vector3(0,7.5f,0);
        build_Scaffolding.SetActive(true);
        build_CD_Obj.SetActive(true);
    }

    public void closeScaffolding()
    {
        build_Scaffolding.SetActive(false);
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

    #region 檢查是否有選擇塔防
    public bool CheckDetectTurret()
    {
        if (turretToBuild.TurretName == GameManager.whichObject.None)
        {
            Debug.Log("沒有選擇塔防");
            return false;
        }
        else
            return true;
    }
    #endregion

    #region 關閉選擇塔防
    public void closeTurretToBuild()
    {
        turretToBuild = new TurretData.TowerDataBase();
    }
    #endregion

    #region 將選擇的塔防傳給其他腳本
    public TurretData.TowerDataBase GetTurretToBuild()
    {
        return turretToBuild;
    }
    #endregion
}