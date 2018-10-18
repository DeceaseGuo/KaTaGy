using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using AtkTower;

public class SceneObjManager : Photon.MonoBehaviour
{
    #region 單例模式
    public static SceneObjManager instance;
    public static SceneObjManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType(typeof(SceneObjManager)) as SceneObjManager;
                if (instance == null)
                {
                    GameObject go = new GameObject("SceneObjManager");
                    instance = go.AddComponent<SceneObjManager>();
                }
            }
            return instance;
        }
    }

    private ObjectPooler poolManager;
    private ObjectPooler PoolManager { get { if (poolManager == null) poolManager = ObjectPooler.instance; return poolManager; } }
    #endregion

    #region 其他腳本Update需求
    private SmoothFollow smoothFollowScript;
    private SmoothFollow SmoothFollowScript { get { if (smoothFollowScript == null) smoothFollowScript = SmoothFollow.instance; return smoothFollowScript; } }

    private GameManager gameManagerScript;
    private GameManager GameManagerScript { get { if (gameManagerScript == null) gameManagerScript = GameManager.instance; return gameManagerScript; } }

    private MatchTimer matchTime;
    private MatchTimer MatchTimeManager { get { if (matchTime == null) matchTime = MatchTimer.Instance; return matchTime; } }

    public ButtonManager_Tower buttonTower;
    public ButtonManager_Solider buttonSoldier;
    #endregion

    [HideInInspector]
    public MinMapSyn minmap;
    //電力塔
    public List<Electricity> myElectricityObjs = new List<Electricity>();

    //攻擊塔
    public List<Turret_Manager> myTowerObjs = new List<Turret_Manager>();
    public List<Turret_Manager> enemyTowerObjs = new List<Turret_Manager>();

    //士兵
    public List<EnemyControl> mySoldierObjs = new List<EnemyControl>();
    public List<EnemyControl> enemySoldierObjs = new List<EnemyControl>();

    //玩家
    public Player myPlayer;
    public Player enemy_Player;

    //我方數量
    private int mySoldierAmount = 0;
    private int myTowerAmount = 0;
    //敵方數量
    private int towerAmount = 0;
    private int soldierAmount = 0;

    //怪物尋找所需
    public struct myCorrectTarget
    {
        public CreatPoints nowPointScript;
        public Transform goPos;     //新目標可以前進的位子
        public GameObject myTarget; //新目標
    }
    private CreatPoints tmpPointScript;
    private Transform tmpTransform;
    private myCorrectTarget tmpTargetStruct;

    //塔防尋找所需
    private float tmpDistance;
    private float compareDis;
    private GameObject tmpTowerTarget;


    private void Start()
    {
        if (Instance != this)
            Destroy(this);
    }

    private void Update()
    {
        //玩家update
        if (myPlayer != null)
            myPlayer.NeedToUpdate();

        //士兵update
        if (mySoldierAmount != 0)
        {
            for (int i = 0; i < mySoldierAmount; i++)
            {
                mySoldierObjs[i].NeedToUpdate();
            }
        }

        //塔防update
        if (myTowerAmount != 0)
        {
            for (int i = 0; i < myTowerAmount; i++)
            {
                myTowerObjs[i].NeedToUpdate();
            }
        }

        GameBotton();
        buttonSoldier.NeedToUpdate();
        buttonTower.NeedToUpdate();
    }

    private void LateUpdate()
    {
        SmoothFollowScript.NeedToLateUpdate();

        //時間管理
        MatchTimeManager.NeedToLateUpdate();

        //Clone玩家的Points的位子跟隨
        if (enemy_Player != null)
            enemy_Player.MyCreatPoints.NeedToLateUpdate();

        //敵方Clone士兵
        if (soldierAmount != 0)
        {
            for (int i = 0; i < soldierAmount; i++)
            {
                enemySoldierObjs[i].NeedToLateUpdate();
            }
        }
    }

    private void GameBotton()
    {
        GameManagerScript.NeedToUpdate_Btn();

        if (Input.GetKeyDown(KeyCode.T))
        {
            SmoothFollowScript.switch_UAV();
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            SmoothFollowScript.GoBackMyPos();
        }
    }

    //加入敵人核心(待修改)
    /* public void SetCore(GameObject _core)
     {
         enemySoldierObjs.Add(_core);
     }*/

    #region 塔防找目標
    public GameObject CalculationDis(Transform _me, float _maxdis,float _mindis)
    {
        tmpTowerTarget = null;
        compareDis = 0;

        //士兵
        if (soldierAmount != 0)
        {
            for (int i = 0; i < soldierAmount; i++)
            {
                tmpDistance = Vector3.SqrMagnitude(enemySoldierObjs[i].transform.position - _me.position);
                if (tmpDistance < (_maxdis * _maxdis) && tmpDistance > (_mindis * _mindis))
                {
                    if (compareDis < tmpDistance)
                    {
                        compareDis = tmpDistance;
                        tmpTowerTarget = enemySoldierObjs[i].gameObject;
                    }
                }
            }
            if (tmpTowerTarget != null)
                return tmpTowerTarget;
        }

        //塔
        if (towerAmount != 0)
        {
            for (int i = 0; i < towerAmount; i++)
            {
                tmpDistance = Vector3.SqrMagnitude(enemyTowerObjs[i].transform.position - _me.position);
                if (tmpDistance < (_maxdis * _maxdis) && tmpDistance > (_mindis * _mindis))
                {
                    if (compareDis < tmpDistance)
                    {
                        compareDis = tmpDistance;
                        tmpTowerTarget = enemyTowerObjs[i].gameObject;
                    }
                }
            }
            if (tmpTowerTarget != null)
                return tmpTowerTarget;
        }

        //人
        if (enemy_Player != null)
        {
            if (!enemy_Player.GetComponent<isDead>().checkDead)//player不會被移出去，所以要判斷死了沒
            {
                tmpDistance = Vector3.SqrMagnitude(enemy_Player.transform.position - _me.position);
                if (tmpDistance < (_maxdis * _maxdis) && tmpDistance > (_mindis * _mindis))
                    return enemy_Player.gameObject;
            }
        }
        return tmpTowerTarget;
    }
    #endregion

    #region 士兵找目標
    //優先攻擊塔(除了反擊不會打玩家)
    public myCorrectTarget CalculationDis_Tower(EnemyControl _soldierScript)
    {
        tmpTargetStruct.myTarget = null;
        tmpTargetStruct.goPos = null;

        //核心 ??

        //塔
        if (towerAmount != 0)
        {
            for (int i = 0; i < towerAmount; i++)
            {
                if (Vector3.SqrMagnitude(enemyTowerObjs[i].transform.position - _soldierScript.transform.position) < _soldierScript.viewRadius * _soldierScript.viewRadius)
                {
                    tmpPointScript = enemyTowerObjs[i].MyCreatPoints;
                    if (!tmpPointScript.CheckFull(_soldierScript.enemyData.atk_Range))
                    {
                        tmpTransform = tmpPointScript.FindClosePoint(_soldierScript.enemyData.atk_Range, _soldierScript.transform, _soldierScript.enemyData.width);
                        if (tmpTransform != null)
                        {

                            if (tmpTargetStruct.myTarget != null)
                            {
                                if (Vector3.SqrMagnitude(enemyTowerObjs[i].transform.position - _soldierScript.transform.position) < Vector3.SqrMagnitude(tmpTargetStruct.myTarget.transform.position - _soldierScript.transform.position))
                                {
                                    tmpTargetStruct.nowPointScript = tmpPointScript;
                                    tmpTargetStruct.myTarget = enemyTowerObjs[i].gameObject;
                                    tmpTargetStruct.goPos = tmpTransform;
                                }
                            }
                            else
                            {
                                tmpTargetStruct.nowPointScript = tmpPointScript;
                                tmpTargetStruct.myTarget = enemyTowerObjs[i].gameObject;
                                tmpTargetStruct.goPos = tmpTransform;
                            }
                        }
                    }
                }
            }          

            if (tmpTargetStruct.myTarget != null)
                return tmpTargetStruct;
        }

        //士兵
        if (soldierAmount != 0)
        {
            for (int i = 0; i < soldierAmount; i++)
            {
                if (Vector3.SqrMagnitude(enemySoldierObjs[i].transform.position - _soldierScript.transform.position) < _soldierScript.viewRadius * _soldierScript.viewRadius)
                {
                    tmpPointScript = enemySoldierObjs[i].myCreatPoints;
                    if (!tmpPointScript.CheckFull(_soldierScript.enemyData.atk_Range))
                    {
                        tmpTransform = tmpPointScript.FindClosePoint(_soldierScript.enemyData.atk_Range, _soldierScript.transform, _soldierScript.enemyData.width);
                        if (tmpTransform != null)
                        {
                            if (tmpTargetStruct.myTarget != null)
                            {
                                if (Vector3.SqrMagnitude(enemySoldierObjs[i].transform.position - _soldierScript.transform.position) < Vector3.SqrMagnitude(tmpTargetStruct.myTarget.transform.position - _soldierScript.transform.position))
                                {
                                    tmpTargetStruct.nowPointScript = tmpPointScript;
                                    tmpTargetStruct.myTarget = enemySoldierObjs[i].gameObject;
                                    tmpTargetStruct.goPos = tmpTransform;
                                }
                            }
                            else
                            {
                                tmpTargetStruct.nowPointScript = tmpPointScript;
                                tmpTargetStruct.myTarget = enemySoldierObjs[i].gameObject;
                                tmpTargetStruct.goPos = tmpTransform;
                            }
                        }
                    }
                }
            }
            if (tmpTargetStruct.myTarget != null)
                return tmpTargetStruct;
        }

        return tmpTargetStruct;
    }

    //優先攻擊士兵(可選擇打不打玩家)
    public myCorrectTarget CalculationDis_Soldier(EnemyControl _soldierScript, bool canAtkPlay)
    {
        tmpTargetStruct.myTarget = null;
        tmpTargetStruct.goPos = null;

        //核心 ??

        //人
        if (canAtkPlay && enemy_Player != null)
        {
            if (!enemy_Player.GetComponent<isDead>().checkDead)//player不會被移出去，所以要判斷死了沒
            {
                if (Vector3.SqrMagnitude(enemy_Player.transform.position - _soldierScript.transform.position) < _soldierScript.viewRadius * _soldierScript.viewRadius)
                {
                    tmpPointScript = enemy_Player.MyCreatPoints;
                    if (!tmpPointScript.CheckFull(_soldierScript.enemyData.atk_Range))
                    {
                        tmpTransform = tmpPointScript.FindClosePoint(_soldierScript.enemyData.atk_Range, _soldierScript.transform, _soldierScript.enemyData.width);
                        if (tmpTransform != null)
                        {
                            tmpTargetStruct.nowPointScript = tmpPointScript;
                            tmpTargetStruct.myTarget = enemy_Player.gameObject;
                            tmpTargetStruct.goPos = tmpTransform;
                            return tmpTargetStruct;
                        }
                    }
                }
            }
        }

        //士兵
        if (soldierAmount != 0)
        {
            for (int i = 0; i < soldierAmount; i++)
            {
                if (Vector3.SqrMagnitude(enemySoldierObjs[i].transform.position - _soldierScript.transform.position) < _soldierScript.viewRadius * _soldierScript.viewRadius)
                {
                    tmpPointScript = enemySoldierObjs[i].myCreatPoints;
                    if(!tmpPointScript.CheckFull(_soldierScript.enemyData.atk_Range))
                    {
                        tmpTransform = tmpPointScript.FindClosePoint(_soldierScript.enemyData.atk_Range, _soldierScript.transform, _soldierScript.enemyData.width);
                        if (tmpTransform != null)
                        {
                            if (tmpTargetStruct.myTarget != null)
                            {
                                if (Vector3.SqrMagnitude(enemySoldierObjs[i].transform.position - _soldierScript.transform.position) < Vector3.SqrMagnitude(tmpTargetStruct.myTarget.transform.position - _soldierScript.transform.position))
                                {
                                    tmpTargetStruct.nowPointScript = tmpPointScript;
                                    tmpTargetStruct.myTarget = enemySoldierObjs[i].gameObject;
                                    tmpTargetStruct.goPos = tmpTransform;
                                }
                            }
                            else
                            {
                                tmpTargetStruct.nowPointScript = tmpPointScript;
                                tmpTargetStruct.myTarget = enemySoldierObjs[i].gameObject;
                                tmpTargetStruct.goPos = tmpTransform;
                            }
                        }
                    }
                }
            }
            if (tmpTargetStruct.myTarget != null)
                return tmpTargetStruct;
        }

        //塔
        if (towerAmount != 0)
        {
            for (int i = 0; i < towerAmount; i++)
            {
                if (Vector3.SqrMagnitude(enemyTowerObjs[i].transform.position - _soldierScript.transform.position) < _soldierScript.viewRadius * _soldierScript.viewRadius)
                {
                    tmpPointScript = enemyTowerObjs[i].MyCreatPoints;
                    if (!tmpPointScript.CheckFull(_soldierScript.enemyData.atk_Range))
                    {
                        tmpTransform = tmpPointScript.FindClosePoint(_soldierScript.enemyData.atk_Range, _soldierScript.transform, _soldierScript.enemyData.width);
                        if (tmpTransform != null)
                        {
                            if (tmpTargetStruct.myTarget != null)
                            {
                                if (Vector3.SqrMagnitude(enemyTowerObjs[i].transform.position - _soldierScript.transform.position) < Vector3.SqrMagnitude(tmpTargetStruct.myTarget.transform.position - _soldierScript.transform.position))
                                {
                                    tmpTargetStruct.nowPointScript = tmpPointScript;
                                    tmpTargetStruct.myTarget = enemyTowerObjs[i].gameObject;
                                    tmpTargetStruct.goPos = tmpTransform;
                                }
                            }
                            else
                            {
                                tmpTargetStruct.nowPointScript = tmpPointScript;
                                tmpTargetStruct.myTarget = enemyTowerObjs[i].gameObject;
                                tmpTargetStruct.goPos = tmpTransform;
                            }
                        }
                    }
                }
            }
            if (tmpTargetStruct.myTarget != null)
                return tmpTargetStruct;
        }

        return tmpTargetStruct;
    }
    #endregion

    #region Icon
    //取出icon
    void GetIcon(GameManager.whichObject whichObject, List<RectTransform> Icons)
    {
        GameObject icon_obj = PoolManager.getPoolObject(whichObject, Vector3.zero, Quaternion.identity);
        if (Icons == minmap.enemySoliderIcons || Icons == minmap.enemyTowerIcons)
        {
            icon_obj.GetComponent<Image>().color = Color.red;
        }
        RectTransform r = icon_obj.GetComponent<RectTransform>();
        icon_obj.transform.SetParent(minmap.transform);
        Icons.Add(r);

        if (whichObject == GameManager.whichObject.SoldierIcon)
        {
            r.SetAsFirstSibling();
        }
        else
        {
            minmap.enemyplayerIcon.SetAsLastSibling();
            minmap.myplayerIcon.SetAsLastSibling();
        }
    }

    //收回icon
    void ReIcon(GameManager.whichObject whichObject, List<RectTransform> Icons, int _index)
    {
        if (Icons == minmap.enemySoliderIcons || Icons == minmap.enemyTowerIcons)
        {
            Icons[_index].gameObject.GetComponent<Image>().color = Color.white;
        }
        PoolManager.Repool(whichObject, Icons[_index].gameObject);
        Icons.RemoveAt(_index);
    }
    #endregion

    #region 增加
    //自己端--------------------------------------------------------------------------------
    //士兵
    public void AddMy_SoldierList(EnemyControl _obj)
    {
        mySoldierObjs.Add(_obj);
        mySoldierAmount = mySoldierObjs.Count;
    }
    //塔防
    public void AddMy_TowerList(Turret_Manager _obj)
    {
        myTowerObjs.Add(_obj);
        myTowerAmount = myTowerObjs.Count;
        //   GetIcon(GameManager.whichObject.TowerIcon, minmap.myTowerIcons);
    }
    //電力塔
    public void AddMy_Electricity(Electricity _obj)
    {
        myTowerObjs.Add(_obj);
        myTowerAmount = myTowerObjs.Count;
        myElectricityObjs.Add(_obj);
        //   GetIcon(GameManager.whichObject.EIcon, minmap.myTowerIcons);
    }

    //敵人端------------------------------------------------------------------------------------
    //士兵
    public void AddEnemy_SoldierList(EnemyControl _obj)
    {
        enemySoldierObjs.Add(_obj);
        soldierAmount = enemySoldierObjs.Count;
        //  GetIcon(GameManager.whichObject.SoldierIcon, minmap.enemySoliderIcons);
    }
    //塔防
    public void AddEnemy_TowerList(Turret_Manager _obj)
    {
        enemyTowerObjs.Add(_obj);
        towerAmount = enemyTowerObjs.Count;
    }
    //電力塔
    public void AddEnemy_Electricity(Electricity _obj)
    {
        enemyTowerObjs.Add(_obj);
        towerAmount = enemyTowerObjs.Count;
        // GetIcon(GameManager.whichObject.EIcon, minmap.enemyTowerIcons);
    }
    #endregion

    #region 移除
    public void RemoveMy_SoldierList(EnemyControl _obj)
    {
        if (mySoldierObjs.Contains(_obj))
        {
            //   ReIcon(GameManager.whichObject.SoldierIcon, minmap.mySoliderIcons, mySoldierObjs.IndexOf(_obj));
            mySoldierObjs.Remove(_obj);
            mySoldierAmount = mySoldierObjs.Count;
        }
    }
    //塔防
    public void RemoveMy_TowerList(Turret_Manager _obj)
    {
        if (myTowerObjs.Contains(_obj))
        {
            //ReIcon(GameManager.whichObject.TowerIcon, minmap.myTowerIcons, myTowerObjs.IndexOf(_obj));
            myTowerObjs.Remove(_obj);
            myTowerAmount = myTowerObjs.Count;
        }
    }
    //電力塔
    public void RemoveMy_Electricity(Electricity _obj)
    {
        if (myTowerObjs.Contains(_obj))
        {
            //    ReIcon(GameManager.whichObject.EIcon, minmap.myTowerIcons, myTowerObjs.IndexOf(_obj));
            myElectricityObjs.Remove(_obj);
            myTowerObjs.Remove(_obj);
            myTowerAmount = myTowerObjs.Count;
        }
    }

    //敵人端------------------------------------------------------------------------------------
    //士兵
    public void RemoveEnemy_SoldierList(EnemyControl _obj)
    {
        if (enemySoldierObjs.Contains(_obj))
        {
            // ReIcon(GameManager.whichObject.SoldierIcon, minmap.enemySoliderIcons, enemySoldierObjs.IndexOf(_obj));
            enemySoldierObjs.Remove(_obj);
            soldierAmount = enemySoldierObjs.Count;
        }
    }
    //塔防
    public void RemoveEnemy_TowerList(Turret_Manager _obj)
    {
        if (enemyTowerObjs.Contains(_obj))
        {
            // ReIcon(GameManager.whichObject.TowerIcon, minmap.enemyTowerIcons, enemyTowerObjs.IndexOf(_obj));
            enemyTowerObjs.Remove(_obj);
            towerAmount = enemyTowerObjs.Count;
        }
    }
    //電力塔
    public void RemoveEnemy_Electricity(Electricity _obj)
    {
        if (enemyTowerObjs.Contains(_obj))
        {
            // ReIcon(GameManager.whichObject.TowerIcon, minmap.enemyTowerIcons, enemyTowerObjs.IndexOf(_obj));
            enemyTowerObjs.Remove(_obj);
            towerAmount = enemyTowerObjs.Count;
        }
    }
    #endregion

    #region 升級士兵
    public void UpdataMySoldier(byte _level, int _whatAbility)
    {
        switch (((UpdateManager.Myability)_whatAbility))
        {
            case (UpdateManager.Myability.Soldier_ATK):
                UpdateMySoldier_Atk(_level);
                break;
            case (UpdateManager.Myability.Soldier_DEF):
                UpdateMySoldier_Def(_level);
                break;
            default:
                break;
        }
    }
    public void UpdataClientSoldier(byte _level, int _whatAbility)
    {
        switch (((UpdateManager.Myability)_whatAbility))
        {
            case (UpdateManager.Myability.Soldier_ATK):
                UpdateClientSoldier_Atk(_level);
                break;
            case (UpdateManager.Myability.Soldier_DEF):
                UpdateClientSoldier_Def(_level);
                break;
            default:
                break;
        }
    }

    void UpdateMySoldier_Atk(byte _level)
    {
        UpdateDataBase.SoldierUpdateData tmpData;
        switch (_level)
        {            
            case (1):
                for (int i = 0; i < mySoldierAmount; i++)
                {
                    tmpData = mySoldierObjs[i].originalData.updateData;
                    if (mySoldierObjs[i].originalData.ATK_Level != _level)
                    {
                        mySoldierObjs[i].originalData.ATK_Level = _level;
                        mySoldierObjs[i].originalData.atk_maxDamage+= tmpData.Add_atk1;
                        mySoldierObjs[i].originalData.atk_Damage+= tmpData.Add_atk1;
                        mySoldierObjs[i].enemyData.atk_maxDamage += tmpData.Add_atk1;
                        mySoldierObjs[i].enemyData.atk_Damage += tmpData.Add_atk1;
                    }
                }
                break;
            case (2):
                for (int i = 0; i < mySoldierAmount; i++)
                {
                    tmpData = mySoldierObjs[i].originalData.updateData;
                    if (mySoldierObjs[i].originalData.ATK_Level != _level)
                    {
                        mySoldierObjs[i].originalData.ATK_Level = _level;
                        mySoldierObjs[i].originalData.atk_maxDamage += tmpData.Add_atk2;
                        mySoldierObjs[i].originalData.atk_Damage += tmpData.Add_atk2;
                        mySoldierObjs[i].enemyData.atk_maxDamage += tmpData.Add_atk2;
                        mySoldierObjs[i].enemyData.atk_Damage += tmpData.Add_atk2;
                    }
                }
                break;
            case (3):
                for (int i = 0; i < mySoldierAmount; i++)
                {
                    tmpData = mySoldierObjs[i].originalData.updateData;
                    if (mySoldierObjs[i].originalData.ATK_Level != _level)
                    {
                        mySoldierObjs[i].originalData.ATK_Level = _level;
                        mySoldierObjs[i].originalData.atk_maxDamage += tmpData.Add_atk3;
                        mySoldierObjs[i].originalData.atk_Damage += tmpData.Add_atk3;
                        mySoldierObjs[i].enemyData.atk_maxDamage += tmpData.Add_atk3;
                        mySoldierObjs[i].enemyData.atk_Damage += tmpData.Add_atk3;
                    }
                }
                break;
            default:
                break;
        }

        MyEnemyData.instance.ChangeMyAtkData(_level);
    }
    void UpdateClientSoldier_Atk(byte _level)
    {
        UpdateDataBase.SoldierUpdateData tmpData;
        switch (_level)
        {
            case (1):
                for (int i = 0; i < soldierAmount; i++)
                {
                    tmpData = enemySoldierObjs[i].originalData.updateData;
                    if (enemySoldierObjs[i].originalData.ATK_Level != _level)
                    {
                        enemySoldierObjs[i].originalData.ATK_Level = _level;
                        enemySoldierObjs[i].originalData.atk_maxDamage += tmpData.Add_atk1;
                        enemySoldierObjs[i].originalData.atk_Damage += tmpData.Add_atk1;
                        enemySoldierObjs[i].enemyData.atk_maxDamage += tmpData.Add_atk1;
                        enemySoldierObjs[i].enemyData.atk_Damage += tmpData.Add_atk1;
                    }
                }
                break;
            case (2):
                for (int i = 0; i < soldierAmount; i++)
                {
                    tmpData = enemySoldierObjs[i].originalData.updateData;
                    if (enemySoldierObjs[i].originalData.ATK_Level != _level)
                    {
                        enemySoldierObjs[i].originalData.ATK_Level = _level;
                        enemySoldierObjs[i].originalData.atk_maxDamage += tmpData.Add_atk2;
                        enemySoldierObjs[i].originalData.atk_Damage += tmpData.Add_atk2;
                        enemySoldierObjs[i].enemyData.atk_maxDamage += tmpData.Add_atk2;
                        enemySoldierObjs[i].enemyData.atk_Damage += tmpData.Add_atk2;
                    }
                }
                break;
            case (3):
                for (int i = 0; i < soldierAmount; i++)
                {
                    tmpData = enemySoldierObjs[i].originalData.updateData;
                    if (enemySoldierObjs[i].originalData.ATK_Level != _level)
                    {
                        enemySoldierObjs[i].originalData.ATK_Level = _level;
                        enemySoldierObjs[i].originalData.atk_maxDamage += tmpData.Add_atk3;
                        enemySoldierObjs[i].originalData.atk_Damage += tmpData.Add_atk3;
                        enemySoldierObjs[i].enemyData.atk_maxDamage += tmpData.Add_atk3;
                        enemySoldierObjs[i].enemyData.atk_Damage += tmpData.Add_atk3;
                    }
                }
                break;
            default:
                break;
        }
        MyEnemyData.instance.ChangeEnemyAtkData(_level);
    }

    void UpdateMySoldier_Def(byte _level)
    {
        UpdateDataBase.SoldierUpdateData tmpData;
        switch (_level)
        {
            case (1):
                for (int i = 0; i < mySoldierAmount; i++)
                {
                    tmpData = mySoldierObjs[i].originalData.updateData;
                    if (mySoldierObjs[i].originalData.DEF_Level != _level)
                    {
                        mySoldierObjs[i].originalData.DEF_Level = _level;
                        mySoldierObjs[i].originalData.def_base += tmpData.Add_def1;
                        mySoldierObjs[i].originalData.UI_HP+= tmpData.Add_hp1;
                        mySoldierObjs[i].originalData.UI_MaxHp += tmpData.Add_hp1;

                        mySoldierObjs[i].enemyData.def_base += tmpData.Add_def1;
                        mySoldierObjs[i].enemyData.UI_HP += tmpData.Add_hp1;
                        mySoldierObjs[i].enemyData.UI_MaxHp += tmpData.Add_hp1;
                    }
                }
                break;
            case (2):
                for (int i = 0; i < mySoldierAmount; i++)
                {
                    tmpData = mySoldierObjs[i].originalData.updateData;
                    if (mySoldierObjs[i].originalData.DEF_Level != _level)
                    {
                        mySoldierObjs[i].originalData.DEF_Level = _level;
                        mySoldierObjs[i].originalData.def_base += tmpData.Add_def2;
                        mySoldierObjs[i].originalData.UI_HP += tmpData.Add_hp2;
                        mySoldierObjs[i].originalData.UI_MaxHp += tmpData.Add_hp2;

                        mySoldierObjs[i].enemyData.def_base += tmpData.Add_def2;
                        mySoldierObjs[i].enemyData.UI_HP += tmpData.Add_hp2;
                        mySoldierObjs[i].enemyData.UI_MaxHp += tmpData.Add_hp2;
                    }
                }
                break;
            case (3):
                for (int i = 0; i < mySoldierAmount; i++)
                {
                    tmpData = mySoldierObjs[i].originalData.updateData;
                    if (mySoldierObjs[i].originalData.DEF_Level != _level)
                    {
                        mySoldierObjs[i].originalData.DEF_Level = _level;
                        mySoldierObjs[i].originalData.def_base += tmpData.Add_def3;
                        mySoldierObjs[i].originalData.UI_HP += tmpData.Add_hp3;
                        mySoldierObjs[i].originalData.UI_MaxHp += tmpData.Add_hp3;

                        mySoldierObjs[i].enemyData.def_base += tmpData.Add_def3;
                        mySoldierObjs[i].enemyData.UI_HP += tmpData.Add_hp3;
                        mySoldierObjs[i].enemyData.UI_MaxHp += tmpData.Add_hp3;
                    }
                }
                break;
            default:
                break;
        }

        MyEnemyData.instance.ChangeMyDefData(_level);
    }
    void UpdateClientSoldier_Def(byte _level)
    {
        UpdateDataBase.SoldierUpdateData tmpData;
        switch (_level)
        {
            case (1):
                for (int i = 0; i < soldierAmount; i++)
                {
                    tmpData = enemySoldierObjs[i].originalData.updateData;
                    if (enemySoldierObjs[i].originalData.DEF_Level != _level)
                    {
                        enemySoldierObjs[i].originalData.DEF_Level = _level;
                        enemySoldierObjs[i].originalData.def_base += tmpData.Add_def1;
                        enemySoldierObjs[i].originalData.UI_HP += tmpData.Add_hp1;
                        enemySoldierObjs[i].originalData.UI_MaxHp += tmpData.Add_hp1;

                        enemySoldierObjs[i].enemyData.def_base += tmpData.Add_def1;
                        enemySoldierObjs[i].enemyData.UI_HP += tmpData.Add_hp1;
                        enemySoldierObjs[i].enemyData.UI_MaxHp += tmpData.Add_hp1;
                    }
                }
                break;
            case (2):
                for (int i = 0; i < soldierAmount; i++)
                {
                    tmpData = enemySoldierObjs[i].originalData.updateData;
                    if (enemySoldierObjs[i].originalData.DEF_Level != _level)
                    {
                        enemySoldierObjs[i].originalData.DEF_Level = _level;
                        enemySoldierObjs[i].originalData.def_base += tmpData.Add_def2;
                        enemySoldierObjs[i].originalData.UI_HP += tmpData.Add_hp2;
                        enemySoldierObjs[i].originalData.UI_MaxHp += tmpData.Add_hp2;

                        enemySoldierObjs[i].enemyData.def_base += tmpData.Add_def2;
                        enemySoldierObjs[i].enemyData.UI_HP += tmpData.Add_hp2;
                        enemySoldierObjs[i].enemyData.UI_MaxHp += tmpData.Add_hp2;
                    }
                }
                break;
            case (3):
                for (int i = 0; i < soldierAmount; i++)
                {
                    tmpData = enemySoldierObjs[i].originalData.updateData;
                    if (enemySoldierObjs[i].originalData.DEF_Level != _level)
                    {
                        enemySoldierObjs[i].originalData.DEF_Level = _level;
                        enemySoldierObjs[i].originalData.def_base += tmpData.Add_def3;
                        enemySoldierObjs[i].originalData.UI_HP += tmpData.Add_hp3;
                        enemySoldierObjs[i].originalData.UI_MaxHp += tmpData.Add_hp3;

                        enemySoldierObjs[i].enemyData.def_base += tmpData.Add_def3;
                        enemySoldierObjs[i].enemyData.UI_HP += tmpData.Add_hp3;
                        enemySoldierObjs[i].enemyData.UI_MaxHp += tmpData.Add_hp3;
                    }
                }
                break;
            default:
                break;
        }
        MyEnemyData.instance.ChangeEnemyDefData(_level);
    }
    #endregion

    #region 升級塔防
    public void UpdataMyTower(byte _level, int _whatAbility)
    {
        switch (((UpdateManager.Myability)_whatAbility))
        {
            case (UpdateManager.Myability.Tower_ATK):
                UpdateMyTower_Atk(_level);
                break;
            case (UpdateManager.Myability.Tower_DEF):
                UpdateMyTower_Def(_level);
                break;
            default:
                break;
        }
    }
    public void UpdataClientTower(byte _level, int _whatAbility)
    {
        switch (((UpdateManager.Myability)_whatAbility))
        {
            case (UpdateManager.Myability.Tower_ATK):
                UpdateClientTower_Atk(_level);
                break;
            case (UpdateManager.Myability.Tower_DEF):
                UpdateClientTower_Def(_level);
                break;
            default:
                break;
        }
    }

    void UpdateMyTower_Atk(byte _level)
    {
        UpdateDataBase.TowerUpdateData tmpData;
        switch (_level)
        {
            case (1):
                for (int i = 0; i < myTowerAmount; i++)
                {
                    tmpData = myTowerObjs[i].originalTurretData.updateData;
                    if (myTowerObjs[i].originalTurretData.ATK_Level != _level)
                    {
                        myTowerObjs[i].originalTurretData.ATK_Level = _level;
                        myTowerObjs[i].originalTurretData.Atk_maxDamage += tmpData.Add_atk1;
                        myTowerObjs[i].originalTurretData.Atk_Damage += tmpData.Add_atk1;
                        myTowerObjs[i].turretData.Atk_maxDamage += tmpData.Add_atk1;
                        myTowerObjs[i].turretData.Atk_Damage += tmpData.Add_atk1;
                    }
                }
                break;
            case (2):
                for (int i = 0; i < myTowerAmount; i++)
                {
                    tmpData = myTowerObjs[i].originalTurretData.updateData;
                    if (myTowerObjs[i].originalTurretData.ATK_Level != _level)
                    {
                        myTowerObjs[i].originalTurretData.ATK_Level = _level;
                        myTowerObjs[i].originalTurretData.Atk_maxDamage += tmpData.Add_atk2;
                        myTowerObjs[i].originalTurretData.Atk_Damage += tmpData.Add_atk2;
                        myTowerObjs[i].turretData.Atk_maxDamage += tmpData.Add_atk2;
                        myTowerObjs[i].turretData.Atk_Damage += tmpData.Add_atk2;
                    }
                }
                break;
            case (3):
                for (int i = 0; i < myTowerAmount; i++)
                {
                    tmpData = myTowerObjs[i].originalTurretData.updateData;
                    if (myTowerObjs[i].originalTurretData.ATK_Level != _level)
                    {
                        myTowerObjs[i].originalTurretData.ATK_Level = _level;
                        myTowerObjs[i].originalTurretData.Atk_maxDamage += tmpData.Add_atk3;
                        myTowerObjs[i].originalTurretData.Atk_Damage += tmpData.Add_atk3;
                        myTowerObjs[i].turretData.Atk_maxDamage += tmpData.Add_atk3;
                        myTowerObjs[i].turretData.Atk_Damage += tmpData.Add_atk3;
                    }
                }
                break;
            default:
                break;
        }
        TurretData.instance.ChangeMyAtkData(_level);
    }
    void UpdateClientTower_Atk(byte _level)
    {
        UpdateDataBase.TowerUpdateData tmpData;
        switch (_level)
        {
            case (1):
                for (int i = 0; i < towerAmount; i++)
                {
                    tmpData = enemyTowerObjs[i].originalTurretData.updateData;
                    if (enemyTowerObjs[i].originalTurretData.ATK_Level != _level)
                    {
                        enemyTowerObjs[i].originalTurretData.ATK_Level = _level;
                        enemyTowerObjs[i].originalTurretData.Atk_maxDamage += tmpData.Add_atk1;
                        enemyTowerObjs[i].originalTurretData.Atk_Damage += tmpData.Add_atk1;
                        enemyTowerObjs[i].turretData.Atk_maxDamage += tmpData.Add_atk1;
                        enemyTowerObjs[i].turretData.Atk_Damage += tmpData.Add_atk1;
                    }
                }
                break;
            case (2):
                for (int i = 0; i < towerAmount; i++)
                {
                    tmpData = enemyTowerObjs[i].originalTurretData.updateData;
                    if (enemyTowerObjs[i].originalTurretData.ATK_Level != _level)
                    {
                        enemyTowerObjs[i].originalTurretData.ATK_Level = _level;
                        enemyTowerObjs[i].originalTurretData.Atk_maxDamage += tmpData.Add_atk2;
                        enemyTowerObjs[i].originalTurretData.Atk_Damage += tmpData.Add_atk2;
                        enemyTowerObjs[i].turretData.Atk_maxDamage += tmpData.Add_atk2;
                        enemyTowerObjs[i].turretData.Atk_Damage += tmpData.Add_atk2;
                    }
                }
                break;
            case (3):
                for (int i = 0; i < towerAmount; i++)
                {
                    tmpData = enemyTowerObjs[i].originalTurretData.updateData;
                    if (enemyTowerObjs[i].originalTurretData.ATK_Level != _level)
                    {
                        enemyTowerObjs[i].originalTurretData.ATK_Level = _level;
                        enemyTowerObjs[i].originalTurretData.Atk_maxDamage += tmpData.Add_atk3;
                        enemyTowerObjs[i].originalTurretData.Atk_Damage += tmpData.Add_atk3;
                        enemyTowerObjs[i].turretData.Atk_maxDamage += tmpData.Add_atk3;
                        enemyTowerObjs[i].turretData.Atk_Damage += tmpData.Add_atk3;
                    }
                }
                break;
            default:
                break;
        }
        TurretData.instance.ChangeEnemyAtkData(_level);
    }

    void UpdateMyTower_Def(byte _level)
    {
        UpdateDataBase.TowerUpdateData tmpData;
        switch (_level)
        {
            case (1):
                for (int i = 0; i < myTowerAmount; i++)
                {
                    tmpData = myTowerObjs[i].originalTurretData.updateData;
                    if (myTowerObjs[i].originalTurretData.DEF_Level != _level)
                    {
                        myTowerObjs[i].originalTurretData.DEF_Level = _level;
                        myTowerObjs[i].originalTurretData.def_base += tmpData.Add_def1;
                        myTowerObjs[i].originalTurretData.UI_Hp += tmpData.Add_hp1;
                        myTowerObjs[i].originalTurretData.UI_maxHp += tmpData.Add_hp1;

                        myTowerObjs[i].turretData.def_base += tmpData.Add_def1;
                        myTowerObjs[i].turretData.UI_Hp += tmpData.Add_hp1;
                        myTowerObjs[i].turretData.UI_maxHp += tmpData.Add_hp1;
                    }
                }
                break;
            case (2):
                for (int i = 0; i < myTowerAmount; i++)
                {
                    tmpData = myTowerObjs[i].originalTurretData.updateData;
                    if (myTowerObjs[i].originalTurretData.DEF_Level != _level)
                    {
                        myTowerObjs[i].originalTurretData.DEF_Level = _level;
                        myTowerObjs[i].originalTurretData.def_base += tmpData.Add_def2;
                        myTowerObjs[i].originalTurretData.UI_Hp += tmpData.Add_hp2;
                        myTowerObjs[i].originalTurretData.UI_maxHp += tmpData.Add_hp2;

                        myTowerObjs[i].turretData.def_base += tmpData.Add_def2;
                        myTowerObjs[i].turretData.UI_Hp += tmpData.Add_hp2;
                        myTowerObjs[i].turretData.UI_maxHp += tmpData.Add_hp2;
                    }
                }
                break;
            case (3):
                for (int i = 0; i < myTowerAmount; i++)
                {
                    tmpData = myTowerObjs[i].originalTurretData.updateData;
                    if (myTowerObjs[i].originalTurretData.DEF_Level != _level)
                    {
                        myTowerObjs[i].originalTurretData.DEF_Level = _level;
                        myTowerObjs[i].originalTurretData.def_base += tmpData.Add_def3;
                        myTowerObjs[i].originalTurretData.UI_Hp += tmpData.Add_hp3;
                        myTowerObjs[i].originalTurretData.UI_maxHp += tmpData.Add_hp3;

                        myTowerObjs[i].turretData.def_base += tmpData.Add_def3;
                        myTowerObjs[i].turretData.UI_Hp += tmpData.Add_hp3;
                        myTowerObjs[i].turretData.UI_maxHp += tmpData.Add_hp3;
                    }
                }
                break;
            default:
                break;
        }

        TurretData.instance.ChangeMyDefData(_level);
    }
    void UpdateClientTower_Def(byte _level)
    {
        UpdateDataBase.TowerUpdateData tmpData;
        switch (_level)
        {
            case (1):
                for (int i = 0; i < towerAmount; i++)
                {
                    tmpData = enemyTowerObjs[i].originalTurretData.updateData;
                    if (enemyTowerObjs[i].originalTurretData.DEF_Level != _level)
                    {
                        enemyTowerObjs[i].originalTurretData.DEF_Level = _level;
                        enemyTowerObjs[i].originalTurretData.def_base += tmpData.Add_def1;
                        enemyTowerObjs[i].originalTurretData.UI_Hp += tmpData.Add_hp1;
                        enemyTowerObjs[i].originalTurretData.UI_maxHp += tmpData.Add_hp1;

                        enemyTowerObjs[i].turretData.def_base += tmpData.Add_def1;
                        enemyTowerObjs[i].turretData.UI_Hp += tmpData.Add_hp1;
                        enemyTowerObjs[i].turretData.UI_maxHp += tmpData.Add_hp1;
                    }
                }
                break;
            case (2):
                for (int i = 0; i < towerAmount; i++)
                {
                    tmpData = enemyTowerObjs[i].originalTurretData.updateData;
                    if (enemyTowerObjs[i].originalTurretData.DEF_Level != _level)
                    {
                        enemyTowerObjs[i].originalTurretData.DEF_Level = _level;
                        enemyTowerObjs[i].originalTurretData.def_base += tmpData.Add_def2;
                        enemyTowerObjs[i].originalTurretData.UI_Hp += tmpData.Add_hp2;
                        enemyTowerObjs[i].originalTurretData.UI_maxHp += tmpData.Add_hp2;

                        enemyTowerObjs[i].turretData.def_base += tmpData.Add_def2;
                        enemyTowerObjs[i].turretData.UI_Hp += tmpData.Add_hp2;
                        enemyTowerObjs[i].turretData.UI_maxHp += tmpData.Add_hp2;
                    }
                }
                break;
            case (3):
                for (int i = 0; i < towerAmount; i++)
                {
                    tmpData = enemyTowerObjs[i].originalTurretData.updateData;
                    if (enemyTowerObjs[i].originalTurretData.DEF_Level != _level)
                    {
                        enemyTowerObjs[i].originalTurretData.DEF_Level = _level;
                        enemyTowerObjs[i].originalTurretData.def_base += tmpData.Add_def3;
                        enemyTowerObjs[i].originalTurretData.UI_Hp += tmpData.Add_hp3;
                        enemyTowerObjs[i].originalTurretData.UI_maxHp += tmpData.Add_hp3;

                        enemyTowerObjs[i].turretData.def_base += tmpData.Add_def3;
                        enemyTowerObjs[i].turretData.UI_Hp += tmpData.Add_hp3;
                        enemyTowerObjs[i].turretData.UI_maxHp += tmpData.Add_hp3;
                    }
                }
                break;
            default:
                break;
        }

        TurretData.instance.ChangeEnemyDefData(_level);
    }
    #endregion
}