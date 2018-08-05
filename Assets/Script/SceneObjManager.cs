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
    //加入敵人角色
    public void SetPlayer(GameObject _player)
    {
        enemy_Player = _player;
    }

    public List<GameObject> CalculationDis(GameObject _me, float _dis, bool canAtkTower, bool canAtkPlay)
    {
        List<GameObject> tmpObj = new List<GameObject>();
        float nowDis = 0;
        for (int i = 0; i < enemySoldierObjs.Count; i++)
        {
            nowDis = Vector3.Distance(enemySoldierObjs[i].transform.position, _me.transform.position);
            if (nowDis < _dis)
                tmpObj.Add(enemySoldierObjs[i]);            
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

        if (canAtkPlay && enemy_Player != null)
        {
            nowDis = Vector3.Distance(enemy_Player.transform.position, _me.transform.position);
            if (nowDis < _dis)
                tmpObj.Add(enemy_Player);
        }

        return tmpObj;
    }

    #region 增加
    public void AddMyList(GameObject _obj,GameManager.NowTarget _whoIs)
    {
        Debug.Log("AddMyList " + _obj.name);
        
        switch (_whoIs)
        {
            case GameManager.NowTarget.Player:
                break;
            case GameManager.NowTarget.Soldier:
                mySoldierObjs.Add(_obj);
                break;
            case GameManager.NowTarget.Electricity:
                {
                    myElectricityObjs.Add(_obj.GetComponent<Electricity>());
                    myTowerObjs.Add(_obj);
                }
                break;
            case GameManager.NowTarget.Tower:
                myTowerObjs.Add(_obj);
                break;
            case GameManager.NowTarget.Core:
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
            case GameManager.NowTarget.Player:
                enemy_Player = _obj;
                break;
            case GameManager.NowTarget.Soldier:
                enemySoldierObjs.Add(_obj);
                break;
            case GameManager.NowTarget.Electricity:
                {
                    enemyTowerObjs.Add(_obj);
                }
                break;
            case GameManager.NowTarget.Tower:
                enemyTowerObjs.Add(_obj);
                break;
            case GameManager.NowTarget.Core:
                break;
            default:
                break;
        }
    }
    #endregion

    #region 移除
    public void RemoveMyList(GameObject _obj, GameManager.NowTarget _whoIs)
    {
        Debug.Log("RemoveMyList " + _obj.name);
        switch (_whoIs)
        {
            case GameManager.NowTarget.Player:
                enemy_Player = null;
                break;
            case GameManager.NowTarget.Soldier:
                {
                    if (mySoldierObjs.Contains(_obj))
                        mySoldierObjs.Remove(_obj);
                }
                break;
            case GameManager.NowTarget.Electricity:
                {
                    if (myElectricityObjs.Contains(_obj.GetComponent<Electricity>()))
                    {
                        myElectricityObjs.Remove(_obj.GetComponent<Electricity>());
                        myTowerObjs.Remove(_obj);
                    }
                }
                break;
            case GameManager.NowTarget.Tower:
                {
                    if (myTowerObjs.Contains(_obj))
                        myTowerObjs.Remove(_obj);
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
            case GameManager.NowTarget.Player:
                break;
            case GameManager.NowTarget.Soldier:
                if (enemySoldierObjs.Contains(_obj))
                    enemySoldierObjs.Remove(_obj);
                break;
            case GameManager.NowTarget.Electricity:
                {
                    if (enemyTowerObjs.Contains(_obj))
                    {
                        enemyTowerObjs.Remove(_obj);
                    }
                }
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
