using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyManager : MonoBehaviour
{
    public static EnemyManager instance;
    private PlayerObtain playerObtain;

    //怪物陣列
    /*[HideInInspector] */
    public List<EnemyControl> enemies;

    [SerializeField] List<EnemyBornPoint> player1_Nodes;
    [SerializeField] List<EnemyBornPoint> player2_Nodes;

    //出生點
    [SerializeField] List<EnemyBornPoint> enemyBornPoints;    //目前正在生產
    [SerializeField] List<EnemyBornPoint> delayBornPoints;    //可用空間
    [SerializeField] List<EnemyBornPoint> tmpBornPoints;     //暫存

    [SerializeField]
    private List<MyEnemyData.Enemies> produceQueue = new List<MyEnemyData.Enemies>();

    [SerializeField]
    private List<GameObject> iconQueue = new List<GameObject>();
    //等待生成怪物icon
    private int waitBornIcon = 0;
    //等待生成怪物icon位子
    [SerializeField] GameObject creatPos;
    [SerializeField] GameObject waitPos;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    private void Start()
    {
        playerObtain = PlayerObtain.instance;
        //getBornPoint();
        getCurrentPlayer();
        addBornPoint();
    }


    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Z))
        {
            if (rock1 != null)
            {
                unLock();
                addBornPoint();
                nextSoldier();
            }
            if (rock1 == null)
            {
                unLock();
                addBornPoint();
                nextSoldier();
            }
        }
    }

    void getCurrentPlayer()
    {
        if (GameManager.instance.getMyPlayer() == GameManager.MyNowPlayer.player_1)
        {
            tmpBornPoints = player1_Nodes;
        }
        else if (GameManager.instance.getMyPlayer() == GameManager.MyNowPlayer.player_2)
        {
            tmpBornPoints = player2_Nodes;
        }
    }

    #region 取得生產佇列
    public void getEnemyQueue(MyEnemyData.Enemies solider)
    {
        if (waitBornIcon < 15)
        {
            produceQueue.Add(solider);
            goToWaitArea(solider);
            nextSoldier();
        }
        else
        {
            Debug.Log("到達最大上限量");
        }
    }
    #endregion

    #region 增加與減少等待區域icon
    public void goToWaitArea(MyEnemyData.Enemies _soldier)
    {
        GameObject _icon= ObjectPooler.instance.getPoolObject(_soldier.UI_Name, transform.position, transform.rotation);
        _icon.transform.SetParent(waitPos.transform, false);
        iconQueue.Add(_icon);
        waitBornIcon++;
    }

    public void decreaseWaitBornIcon()
    {
        waitBornIcon--;
    }
    #endregion

    #region 移除等待等待怪物icon並返回資源
    public void RemoveEnemyIcon(GameManager.whichObject _name ,GameObject _obj)
    {
        decreaseWaitBornIcon();
        if (iconQueue.Contains(_obj))
        {
            //返回資源
            int g = iconQueue.FindIndex(x => x.gameObject == _obj);
            playerObtain.obtaniResource(produceQueue[g].cost_Ore, produceQueue[g].cost_Money);
            //移除陣列
            iconQueue.Remove(_obj);
            produceQueue.RemoveAt(g);
            //icon返回物件池
            returnPoolObject(_name, _obj);
        }
    }
    #endregion

    #region 判斷是否有空閒的出生點與等待出生的怪物
    public void nextSoldier()
    {
        if (delayBornPoints.Count != 0 && produceQueue.Count != 0)
        {
            FindBornPos(produceQueue[0]);
        }
    }
    #endregion

    #region 找到空的生成區
    void FindBornPos(MyEnemyData.Enemies enemy)
    {
        if (delayBornPoints.Count == 0 || produceQueue.Count == 0)
            return;

        delayBornPoints[0].goToBornArea(iconQueue[0] ,enemy);
        iconQueue.RemoveAt(0);
        produceQueue.RemoveAt(0);
        enemyBornPoints.Add(delayBornPoints[0]);
        delayBornPoints.RemoveAt(0);        
    }
    #endregion

    /*  #region 取得預設好的生成點
      void getBornPoint()
      {
          for (int i = 0; i < transform.childCount; i++)
          {
              EnemyBornPoint tmpPos = transform.GetChild(i).GetComponent<EnemyBornPoint>();
              tmpBornPoints.Add(tmpPos);
          }

      }
      #endregion*/
    [SerializeField] int rockTotal = 2;
    [SerializeField] GameObject rock1;
    [SerializeField] GameObject rock2;
    void unLock()
    {
        if (rockTotal > 0)
        {
            if (rock2.activeInHierarchy)
            {
                rock2.SetActive(false);
                rockTotal--;
            }
            else
            {
                rock1.SetActive(false);
                rockTotal--;
            }
        }
    }

    void addLock()
    {
        if (rockTotal < 2)
        {
            if (rock1.activeInHierarchy)
            {
                rock1.SetActive(true);
                rockTotal++;
            }
            else
            {
                rock2.SetActive(true);
                rockTotal++;
            }
        }
    }

    #region 增加一個生產佇列(新增兵營)
    public void addBornPoint()
    {

        if (rockTotal > 0 && tmpBornPoints.Count != 0)
        {
            delayBornPoints.Add(tmpBornPoints[0]);
            tmpBornPoints.RemoveAt(0);
        }
        else
        {
            Debug.Log("已取得最大產怪量");
        }
    }
    #endregion

    public EnemyBornPoint getBornQueue()
    {
        return tmpBornPoints[0];
    }

    #region 移除一個生產佇列(兵營被摧毀)
    public void removeBornPoint(EnemyBornPoint _bornPoint)
    {
        if (delayBornPoints.Contains(_bornPoint))
        {
            tmpBornPoints.Add(_bornPoint);
            delayBornPoints.Remove(_bornPoint);
        }

        if (enemyBornPoints.Contains(_bornPoint))
        {
            tmpBornPoints.Add(_bornPoint);
            enemyBornPoints.Remove(_bornPoint);
        }
    }
    #endregion

    #region 移除敵人陣列裡的此敵人
    public void RemoveEnemy(EnemyControl enemy)
    {
        if (enemies.Contains(enemy))
        {
            enemies.Remove(enemy);
        }
        else
        {
            Debug.Log("此敵人已死亡");
        }
    }
    #endregion

    #region 將怪物返回物件池
    public void returnPoolObject(GameManager.whichObject _name ,GameObject _enemy)
    {
        ObjectPooler.instance.Repool(_name, _enemy);
    }
    #endregion

    #region 移除生成區士兵
    public void RemoveSoldier()
    {
        delayBornPoints.Add(enemyBornPoints[0]);
        enemyBornPoints.RemoveAt(0);
    }
    #endregion
}
