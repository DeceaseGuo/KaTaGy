using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneObjManager : MonoBehaviour
{
    #region 單例模式
    private static SceneObjManager instance;
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
    public List<GameObject> myElectricityObjs = new List<GameObject>();
    public List<GameObject> myTowerObjs = new List<GameObject>();
    public List<GameObject> enemyTowerObjs = new List<GameObject>();
    //士兵
    public List<GameObject> mySoldierObjs = new List<GameObject>();
    public List<GameObject> enemySoldierObjs = new List<GameObject>();
    //玩家
    private GameObject isMe_Player;
    private GameObject enemy_Player;
    //核心
    private GameObject isMe_Core;
    private GameObject enemy_Core;

    private void Start()
    {
        if (Instance != this)
            Destroy(this);
    }

    #region 增加
    public void AddMyList(GameObject _obj,GameManager.NowTarget _whoIs)
    {
        switch (_whoIs)
        {
            case GameManager.NowTarget.Player:
                break;
            case GameManager.NowTarget.Soldier:
                mySoldierObjs.Add(_obj);
                break;
            case GameManager.NowTarget.Electricity:
                myElectricityObjs.Add(_obj);
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
        switch (_whoIs)
        {
            case GameManager.NowTarget.Player:
                break;
            case GameManager.NowTarget.Soldier:
                enemySoldierObjs.Add(_obj);
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
        switch (_whoIs)
        {
            case GameManager.NowTarget.Player:
                break;
            case GameManager.NowTarget.Soldier:
                if (mySoldierObjs.Contains(_obj))
                    mySoldierObjs.Remove(_obj);
                break;
            case GameManager.NowTarget.Electricity:
                if (myElectricityObjs.Contains(_obj))
                    myElectricityObjs.Remove(_obj);
                break;
            case GameManager.NowTarget.Tower:
                if (myTowerObjs.Contains(_obj))
                    myTowerObjs.Remove(_obj);
                break;
            case GameManager.NowTarget.Core:
                break;
            default:
                break;
        }
    }

    public void RemoveEnemyList(GameObject _obj, GameManager.NowTarget _whoIs)
    {
        switch (_whoIs)
        {
            case GameManager.NowTarget.Player:
                break;
            case GameManager.NowTarget.Soldier:
                if (enemySoldierObjs.Contains(_obj))
                    enemySoldierObjs.Remove(_obj);
                break;
            case GameManager.NowTarget.Tower:
                if (enemyTowerObjs.Contains(_obj))
                    enemyTowerObjs.Remove(_obj);
                break;
            case GameManager.NowTarget.Core:
                break;
            default:
                break;
        }
    }
    #endregion
}
