using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneObjManager : MonoBehaviour
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

    private void Start()
    {
        if (Instance != this)
            Destroy(this);
    }

    //加入敵人核心
    public void SetCore(GameObject _core)
    {
        enemySoldierObjs.Add(_core);
    }

    public List<GameObject> CalculationDis(GameObject _me, float _dis, bool canAtkTower, bool canAtkPlay)
    {
        List<GameObject> tmpObj = new List<GameObject>();
        float nowDis = 0;

        if (canAtkPlay && enemy_Player != null)
        {
            if (!enemy_Player.GetComponent<isDead>().checkDead)//player不會被移出去，所以要判斷死了沒
            {
                nowDis = Vector3.Distance(enemy_Player.transform.position, _me.transform.position);
                if (nowDis < _dis)
                    tmpObj.Add(enemy_Player);
            }           
        }

        if (canAtkTower)
        {
            for (int i = 0; i < enemyTowerObjs.Count; i++)
            {
                nowDis = Vector3.Distance(enemyTowerObjs[i].transform.position, _me.transform.position);
                if (nowDis < _dis)
                    tmpObj.Add(enemyTowerObjs[i]);
            }
        }

        for (int i = 0; i < enemySoldierObjs.Count; i++)
        {
            nowDis = Vector3.Distance(enemySoldierObjs[i].transform.position, _me.transform.position);
            if (nowDis < _dis)
                tmpObj.Add(enemySoldierObjs[i]);            
        }

        return tmpObj;
    }

    #region 增加
    public void AddMyList(GameObject _obj,GameManager.NowTarget _whoIs)
    {
        RectTransform r = null;
        switch (_whoIs)
        {
            case GameManager.NowTarget.Soldier:
                {
                    mySoldierObjs.Add(_obj);
                    r = Instantiate(minmap.SoliderIcon, minmap.transform);
                    r.SetAsFirstSibling();
                    minmap.mySoliderIcons.Add(r);
                }
                break;
            case GameManager.NowTarget.Tower:
                {
                    myTowerObjs.Add(_obj);
                    Electricity _e = _obj.GetComponent<Electricity>();
                    if (_e != null)
                    {
                        myElectricityObjs.Add(_e);
                        r = Instantiate(minmap.EIcon, minmap.transform);
                    }
                    else
                    {
                        r = Instantiate(minmap.TowerIcon, minmap.transform);
                    }
                    minmap.myplayerIcon.SetAsLastSibling();
                    minmap.myTowerIcons.Add(r);
                }
                break;
            default:
                break;
        }
    }

    public void AddEnemyList(GameObject _obj, GameManager.NowTarget _whoIs)
    {
        Debug.Log("AddEnemyList " + _obj.name);
        switch (_whoIs)
        {
            case GameManager.NowTarget.Soldier:
                enemySoldierObjs.Add(_obj);
                break;
            case GameManager.NowTarget.Tower:
                enemyTowerObjs.Add(_obj);
                break;
            default:
                break;
        }
    }
    #endregion

    #region 移除
    public void RemoveMyList(GameObject _obj, GameManager.NowTarget _whoIs)
    {
        int _index = -1;
        switch (_whoIs)
        {
            case GameManager.NowTarget.Soldier:
                {
                    if (mySoldierObjs.Contains(_obj))
                    {
                        mySoldierObjs.Remove(_obj);
                        _index = mySoldierObjs.IndexOf(_obj);
                        minmap.mySoliderIcons[_index].gameObject.SetActive(false);
                        minmap.mySoliderIcons.RemoveAt(_index);
                    }
                }
                break;
            case GameManager.NowTarget.Tower:
                {
                    if (myTowerObjs.Contains(_obj))
                    {
                        myTowerObjs.Remove(_obj);
                        Electricity _e = _obj.GetComponent<Electricity>();
                        if (_e != null)
                        {
                            myElectricityObjs.Remove(_e);
                        }
                        _index = myTowerObjs.IndexOf(_obj);
                        minmap.myTowerIcons[_index].gameObject.SetActive(false);
                        minmap.myTowerIcons.RemoveAt(_index);
                    }
                }
                break;
            default:
                break;
        }
    }

    public void RemoveEnemyList(GameObject _obj, GameManager.NowTarget _whoIs)
    {
        Debug.Log("RemoveEnemyList " + _obj.name);
        switch (_whoIs)
        {
            case GameManager.NowTarget.Soldier:
                if (enemySoldierObjs.Contains(_obj))
                    enemySoldierObjs.Remove(_obj);
                break;
            case GameManager.NowTarget.Tower:
                if (enemyTowerObjs.Contains(_obj))
                    enemyTowerObjs.Remove(_obj);
                break;
            default:
                break;
        }
    }
    #endregion
}
