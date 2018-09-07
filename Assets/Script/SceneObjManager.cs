using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
    #endregion
    [HideInInspector]
    public MinMapSyn minmap;
    //塔
    public List<Electricity> myElectricityObjs = new List<Electricity>();
    public List<GameObject> myTowerObjs = new List<GameObject>();
    public List<GameObject> enemyTowerObjs = new List<GameObject>();
    //士兵
    public List<GameObject> mySoldierObjs = new List<GameObject>();
    public List<GameObject> enemySoldierObjs = new List<GameObject>();

    //玩家
    public GameObject enemy_Player;

    ObjectPooler pool;

    private int towerAmount;
    private int soldierAmount;

    private void Start()
    {
        if (Instance != this)
            Destroy(this);

        pool = ObjectPooler.instance;
    }

    //加入敵人核心
    public void SetCore(GameObject _core)
    {
        enemySoldierObjs.Add(_core);
    }

    /*private void Update()
    {
        if (Input.GetKeyDown("a"))
        {
            RectTransform r = null;
            for (int i = 0; i < enemySoldierObjs.Count; i++)
            {
                r = Instantiate(minmap.SoliderIcon, minmap.transform);
                r.gameObject.SetActive(false);
                minmap.enemySoliderIcons.Add(r);
            }
            for (int i = 0; i < enemyTowerObjs.Count; i++)
            {
                r = Instantiate(minmap.TowerIcon, minmap.transform);
                r.gameObject.SetActive(false);
                minmap.enemyTowerIcons.Add(r);
            }           
        }
    }*/
    private List<GameObject> tmpObjs = new List<GameObject>();
    public List<GameObject> CalculationDis(GameObject _me, float _dis, bool canAtkTower, bool canAtkPlay)
    {
        // List<GameObject> tmpObj = new List<GameObject>();        
        tmpObjs.Clear();

        if (canAtkPlay && enemy_Player != null)
        {
            if (!enemy_Player.GetComponent<isDead>().checkDead)//player不會被移出去，所以要判斷死了沒
            {
                if (Vector3.Distance(enemy_Player.transform.position, _me.transform.position) < _dis)
                    tmpObjs.Add(enemy_Player);
            }           
        }

        if (canAtkTower)
        {
            for (int i = 0; i < towerAmount; i++)
            {
                if (Vector3.Distance(enemyTowerObjs[i].transform.position, _me.transform.position) < _dis)
                    tmpObjs.Add(enemyTowerObjs[i]);
            }
        }

        for (int i = 0; i < soldierAmount; i++)
        {
            if (Vector3.Distance(enemySoldierObjs[i].transform.position, _me.transform.position) < _dis)
                tmpObjs.Add(enemySoldierObjs[i]);            
        }

        return tmpObjs;
    }

    #region Icon
    //取出icon
    void GetIcon(GameManager.whichObject whichObject, List<RectTransform> Icons)
    {
        GameObject icon_obj = pool.getPoolObject(whichObject, Vector3.zero, Quaternion.identity);
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
        pool.Repool(whichObject, Icons[_index].gameObject);
        Icons.RemoveAt(_index);
    }
    #endregion

    #region 增加
    public void AddMyList(GameObject _obj,GameManager.NowTarget _whoIs)
    {
        switch (_whoIs)
        {
            case GameManager.NowTarget.Soldier:
                {
                    mySoldierObjs.Add(_obj);
                   // GetIcon(GameManager.whichObject.SoldierIcon, minmap.mySoliderIcons);
                }
                break;
            case GameManager.NowTarget.Tower:
                {
                    myTowerObjs.Add(_obj);
                    Electricity _e = _obj.GetComponent<Electricity>();
                    if (_e != null)
                    {
                        myElectricityObjs.Add(_e);
                     //   GetIcon(GameManager.whichObject.EIcon, minmap.myTowerIcons);
                    }
                    else
                    {
                     //   GetIcon(GameManager.whichObject.TowerIcon, minmap.myTowerIcons);
                    }                    
                }
                break;
            default:
                break;
        }
    }

    public void AddEnemyList(GameObject _obj, GameManager.NowTarget _whoIs)
    {
        switch (_whoIs)
        {
            case GameManager.NowTarget.Soldier:
                {
                    enemySoldierObjs.Add(_obj);
                    soldierAmount= enemySoldierObjs.Count;
                  //  GetIcon(GameManager.whichObject.SoldierIcon, minmap.enemySoliderIcons);
                }
                break;
            case GameManager.NowTarget.Tower:
                {
                    
                    enemyTowerObjs.Add(_obj);
                    towerAmount = enemyTowerObjs.Count;
                    if (_obj.GetComponent<Electricity>() != null)
                    {
                       // GetIcon(GameManager.whichObject.EIcon, minmap.enemyTowerIcons);
                    }
                    else
                    {
                     //   GetIcon(GameManager.whichObject.TowerIcon, minmap.enemyTowerIcons);
                    }
                }
                break;
            default:
                break;
        }
    }
    #endregion

    #region 移除
    public void RemoveMyList(GameObject _obj, GameManager.NowTarget _whoIs)
    {
        switch (_whoIs)
        {
            case GameManager.NowTarget.Soldier:
                {
                    if (mySoldierObjs.Contains(_obj))
                    {
                     //   ReIcon(GameManager.whichObject.SoldierIcon, minmap.mySoliderIcons, mySoldierObjs.IndexOf(_obj));
                        mySoldierObjs.Remove(_obj);
                    }
                }
                break;
            case GameManager.NowTarget.Tower:
                {
                    if (myTowerObjs.Contains(_obj))
                    {
                        Electricity _e = _obj.GetComponent<Electricity>();
                        if (_e != null)
                        {
                        //    ReIcon(GameManager.whichObject.EIcon, minmap.myTowerIcons, myTowerObjs.IndexOf(_obj));
                            myElectricityObjs.Remove(_e);
                        }
                        else
                        {
                            //ReIcon(GameManager.whichObject.TowerIcon, minmap.myTowerIcons, myTowerObjs.IndexOf(_obj));
                        }
                        myTowerObjs.Remove(_obj);
                    }
                }
                break;
            default:
                break;
        }
    }

    public void RemoveEnemyList(GameObject _obj, GameManager.NowTarget _whoIs)
    {
        switch (_whoIs)
        {
            case GameManager.NowTarget.Soldier:
                if (enemySoldierObjs.Contains(_obj))
                {
                   // ReIcon(GameManager.whichObject.SoldierIcon, minmap.enemySoliderIcons, enemySoldierObjs.IndexOf(_obj));
                    enemySoldierObjs.Remove(_obj);
                    soldierAmount= enemySoldierObjs.Count;
                }
                break;
            case GameManager.NowTarget.Tower:
                if (enemyTowerObjs.Contains(_obj))
                {
                    // ReIcon(GameManager.whichObject.TowerIcon, minmap.enemyTowerIcons, enemyTowerObjs.IndexOf(_obj));
                    enemyTowerObjs.Remove(_obj);
                    towerAmount= enemyTowerObjs.Count;
                }
                break;
            default:
                break;
        }
    }
    #endregion

    #region 升級士兵
    public void UpdataMySoldier(int _level, int _whatAbility)
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
    public void UpdataClientSoldier(int _level, int _whatAbility)
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

    void UpdateMySoldier_Atk(int _level)
    {
        EnemyControl tmpScript = null;
        switch (_level)
        {
            case (1):
                for (int i = 0; i < mySoldierObjs.Count; i++)
                {
                    tmpScript = mySoldierObjs[i].GetComponent<EnemyControl>();
                    if (tmpScript.originalData.ATK_Level != _level)
                    {
                        tmpScript.originalData.ATK_Level = _level;
                        tmpScript.originalData.atk_maxDamage+= tmpScript.originalData.updateData.Add_atk1;
                        tmpScript.originalData.atk_Damage+= tmpScript.originalData.updateData.Add_atk1;
                        tmpScript.enemyData.atk_maxDamage += tmpScript.originalData.updateData.Add_atk1;
                        tmpScript.enemyData.atk_Damage += tmpScript.originalData.updateData.Add_atk1;
                    }
                }
                break;
            case (2):
                for (int i = 0; i < mySoldierObjs.Count; i++)
                {
                    tmpScript = mySoldierObjs[i].GetComponent<EnemyControl>();
                    if (tmpScript.originalData.ATK_Level != _level)
                    {
                        tmpScript.originalData.ATK_Level = _level;
                        tmpScript.originalData.atk_maxDamage += tmpScript.originalData.updateData.Add_atk2;
                        tmpScript.originalData.atk_Damage += tmpScript.originalData.updateData.Add_atk2;
                        tmpScript.enemyData.atk_maxDamage += tmpScript.originalData.updateData.Add_atk2;
                        tmpScript.enemyData.atk_Damage += tmpScript.originalData.updateData.Add_atk2;
                    }
                }
                break;
            case (3):
                for (int i = 0; i < mySoldierObjs.Count; i++)
                {
                    tmpScript = mySoldierObjs[i].GetComponent<EnemyControl>();
                    if (tmpScript.originalData.ATK_Level != _level)
                    {
                        tmpScript.originalData.ATK_Level = _level;
                        tmpScript.originalData.atk_maxDamage += tmpScript.originalData.updateData.Add_atk3;
                        tmpScript.originalData.atk_Damage += tmpScript.originalData.updateData.Add_atk3;
                        tmpScript.enemyData.atk_maxDamage += tmpScript.originalData.updateData.Add_atk3;
                        tmpScript.enemyData.atk_Damage += tmpScript.originalData.updateData.Add_atk3;
                    }
                }
                break;
            default:
                break;
        }

        MyEnemyData.instance.changeMyData(tmpScript.originalData._soldierName, tmpScript.originalData);
    }
    void UpdateClientSoldier_Atk(int _level)
    {
        EnemyControl tmpScript = null;
        switch (_level)
        {
            case (1):
                for (int i = 0; i < soldierAmount; i++)
                {
                    tmpScript = enemySoldierObjs[i].GetComponent<EnemyControl>();
                    if (tmpScript.originalData.ATK_Level != _level)
                    {
                        tmpScript.originalData.ATK_Level = _level;
                        tmpScript.originalData.atk_maxDamage += tmpScript.originalData.updateData.Add_atk1;
                        tmpScript.originalData.atk_Damage += tmpScript.originalData.updateData.Add_atk1;
                        tmpScript.enemyData.atk_maxDamage += tmpScript.originalData.updateData.Add_atk1;
                        tmpScript.enemyData.atk_Damage += tmpScript.originalData.updateData.Add_atk1;
                    }
                }
                break;
            case (2):
                for (int i = 0; i < soldierAmount; i++)
                {
                    tmpScript = enemySoldierObjs[i].GetComponent<EnemyControl>();
                    if (tmpScript.originalData.ATK_Level != _level)
                    {
                        tmpScript.originalData.ATK_Level = _level;
                        tmpScript.originalData.atk_maxDamage += tmpScript.originalData.updateData.Add_atk2;
                        tmpScript.originalData.atk_Damage += tmpScript.originalData.updateData.Add_atk2;
                        tmpScript.enemyData.atk_maxDamage += tmpScript.originalData.updateData.Add_atk2;
                        tmpScript.enemyData.atk_Damage += tmpScript.originalData.updateData.Add_atk2;
                    }
                }
                break;
            case (3):
                for (int i = 0; i < soldierAmount; i++)
                {
                    tmpScript = enemySoldierObjs[i].GetComponent<EnemyControl>();
                    if (tmpScript.originalData.ATK_Level != _level)
                    {
                        tmpScript.originalData.ATK_Level = _level;
                        tmpScript.originalData.atk_maxDamage += tmpScript.originalData.updateData.Add_atk3;
                        tmpScript.originalData.atk_Damage += tmpScript.originalData.updateData.Add_atk3;
                        tmpScript.enemyData.atk_maxDamage += tmpScript.originalData.updateData.Add_atk3;
                        tmpScript.enemyData.atk_Damage += tmpScript.originalData.updateData.Add_atk3;
                    }
                }
                break;
            default:
                break;
        }
        MyEnemyData.instance.changeEnemyData(tmpScript.originalData._soldierName, tmpScript.originalData);
    }

    void UpdateMySoldier_Def(int _level)
    {
        EnemyControl tmpScript = null;
        switch (_level)
        {
            case (1):
                for (int i = 0; i < mySoldierObjs.Count; i++)
                {
                    tmpScript = mySoldierObjs[i].GetComponent<EnemyControl>();
                    if (tmpScript.originalData.ATK_Level != _level)
                    {
                        tmpScript.originalData.DEF_Level = _level;
                        tmpScript.originalData.def_base += tmpScript.originalData.updateData.Add_def1;
                        tmpScript.originalData.UI_HP+= tmpScript.originalData.updateData.Add_hp1;
                        tmpScript.originalData.UI_MaxHp += tmpScript.originalData.updateData.Add_hp1;

                        tmpScript.enemyData.def_base += tmpScript.originalData.updateData.Add_def1;
                        tmpScript.enemyData.UI_HP += tmpScript.originalData.updateData.Add_hp1;
                        tmpScript.enemyData.UI_MaxHp += tmpScript.originalData.updateData.Add_hp1;
                    }
                }
                break;
            case (2):
                for (int i = 0; i < mySoldierObjs.Count; i++)
                {
                    tmpScript = mySoldierObjs[i].GetComponent<EnemyControl>();
                    if (tmpScript.originalData.ATK_Level != _level)
                    {
                        tmpScript.originalData.DEF_Level = _level;
                        tmpScript.originalData.def_base += tmpScript.originalData.updateData.Add_def2;
                        tmpScript.originalData.UI_HP += tmpScript.originalData.updateData.Add_hp2;
                        tmpScript.originalData.UI_MaxHp += tmpScript.originalData.updateData.Add_hp2;

                        tmpScript.enemyData.def_base += tmpScript.originalData.updateData.Add_def2;
                        tmpScript.enemyData.UI_HP += tmpScript.originalData.updateData.Add_hp2;
                        tmpScript.enemyData.UI_MaxHp += tmpScript.originalData.updateData.Add_hp2;
                    }
                }
                break;
            case (3):
                for (int i = 0; i < mySoldierObjs.Count; i++)
                {
                    tmpScript = mySoldierObjs[i].GetComponent<EnemyControl>();
                    if (tmpScript.originalData.ATK_Level != _level)
                    {
                        tmpScript.originalData.DEF_Level = _level;
                        tmpScript.originalData.def_base += tmpScript.originalData.updateData.Add_def3;
                        tmpScript.originalData.UI_HP += tmpScript.originalData.updateData.Add_hp3;
                        tmpScript.originalData.UI_MaxHp += tmpScript.originalData.updateData.Add_hp3;

                        tmpScript.enemyData.def_base += tmpScript.originalData.updateData.Add_def3;
                        tmpScript.enemyData.UI_HP += tmpScript.originalData.updateData.Add_hp3;
                        tmpScript.enemyData.UI_MaxHp += tmpScript.originalData.updateData.Add_hp3;
                    }
                }
                break;
            default:
                break;
        }

        MyEnemyData.instance.changeMyData(tmpScript.originalData._soldierName, tmpScript.originalData);
    }
    void UpdateClientSoldier_Def(int _level)
    {
        EnemyControl tmpScript = null;
        switch (_level)
        {
            case (1):
                for (int i = 0; i < soldierAmount; i++)
                {
                    tmpScript = enemySoldierObjs[i].GetComponent<EnemyControl>();
                    if (tmpScript.originalData.ATK_Level != _level)
                    {
                        tmpScript.originalData.DEF_Level = _level;
                        tmpScript.originalData.def_base += tmpScript.originalData.updateData.Add_def1;
                        tmpScript.originalData.UI_HP += tmpScript.originalData.updateData.Add_hp1;
                        tmpScript.originalData.UI_MaxHp += tmpScript.originalData.updateData.Add_hp1;

                        tmpScript.enemyData.def_base += tmpScript.originalData.updateData.Add_def1;
                        tmpScript.enemyData.UI_HP += tmpScript.originalData.updateData.Add_hp1;
                        tmpScript.enemyData.UI_MaxHp += tmpScript.originalData.updateData.Add_hp1;
                    }
                }
                break;
            case (2):
                for (int i = 0; i < soldierAmount; i++)
                {
                    tmpScript = enemySoldierObjs[i].GetComponent<EnemyControl>();
                    if (tmpScript.originalData.ATK_Level != _level)
                    {
                        tmpScript.originalData.DEF_Level = _level;
                        tmpScript.originalData.def_base += tmpScript.originalData.updateData.Add_def2;
                        tmpScript.originalData.UI_HP += tmpScript.originalData.updateData.Add_hp2;
                        tmpScript.originalData.UI_MaxHp += tmpScript.originalData.updateData.Add_hp2;

                        tmpScript.enemyData.def_base += tmpScript.originalData.updateData.Add_def2;
                        tmpScript.enemyData.UI_HP += tmpScript.originalData.updateData.Add_hp2;
                        tmpScript.enemyData.UI_MaxHp += tmpScript.originalData.updateData.Add_hp2;
                    }
                }
                break;
            case (3):
                for (int i = 0; i < soldierAmount; i++)
                {
                    tmpScript = enemySoldierObjs[i].GetComponent<EnemyControl>();
                    if (tmpScript.originalData.ATK_Level != _level)
                    {
                        tmpScript.originalData.DEF_Level = _level;
                        tmpScript.originalData.def_base += tmpScript.originalData.updateData.Add_def3;
                        tmpScript.originalData.UI_HP += tmpScript.originalData.updateData.Add_hp3;
                        tmpScript.originalData.UI_MaxHp += tmpScript.originalData.updateData.Add_hp3;

                        tmpScript.enemyData.def_base += tmpScript.originalData.updateData.Add_def3;
                        tmpScript.enemyData.UI_HP += tmpScript.originalData.updateData.Add_hp3;
                        tmpScript.enemyData.UI_MaxHp += tmpScript.originalData.updateData.Add_hp3;
                    }
                }
                break;
            default:
                break;
        }
        MyEnemyData.instance.changeEnemyData(tmpScript.originalData._soldierName, tmpScript.originalData);
    }
    #endregion

    #region 升級塔防
    public void UpdataMyTower(int _level, int _whatAbility)
    {
        switch (((UpdateManager.Myability)_whatAbility))
        {
            case (UpdateManager.Myability.Tower_ATK):
                break;
            case (UpdateManager.Myability.Tower_DEF):
                break;
            default:
                break;
        }
    }
    public void UpdataClientTower(int _level, int _whatAbility)
    {
        switch (((UpdateManager.Myability)_whatAbility))
        {
            case (UpdateManager.Myability.Tower_ATK):
                break;
            case (UpdateManager.Myability.Tower_DEF):
                break;
            default:
                break;
        }
    }
    #endregion
}
