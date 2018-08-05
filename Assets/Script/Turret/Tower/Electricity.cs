using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using AtkTower;

[RequireComponent(typeof(isDead))]
public class Electricity : Photon.MonoBehaviour
{
    //數據
    public GameManager.whichObject DataName;
    TurretData.TowerDataBase turretData;
    TurretData.TowerDataBase originalTurretData;

    [Header("電力範圍")]
    public int resource_Electricity;
    public float range;

    [Header("連接")]
    public Electricity firstE;
    public List<Electricity> connectElectricitys = new List<Electricity>();
    public List<GameObject> connectTowers = new List<GameObject>();
    public List<Electricity> myTouch = new List<Electricity>();

    [Header("網格變色")]
    [SerializeField] Color origonalColor;
    [SerializeField] Color notBuildColor;
    [SerializeField] LayerMask GridMask;
    [SerializeField] LayerMask TowerMask;

    int origine_Electricity;

    //正確目標
    protected Transform target;
    [Header("位置")]
    public Transform Pos_rotation;

    [Header("UI部分")]
    public Image healthBar;

    protected isDead deadManager;
    protected PhotonView Net;
    PlayerObtain playerObtain;
    BuildManager buildManager;

    private SceneObjManager sceneObjManager;
    private SceneObjManager SceneManager { get { if (sceneObjManager == null) sceneObjManager = SceneObjManager.Instance; return sceneObjManager; } }

    private void Awake()
    {
        Net = GetComponent<PhotonView>();
        originalTurretData = TurretData.instance.getTowerData(DataName);
        buildManager = BuildManager.instance;
        playerObtain = PlayerObtain.instance;
        origine_Electricity = resource_Electricity;
    }

    private void Start()
    {
        if (photonView.isMine)
        {
            checkCurrentPlay();
        }
        else
        {
            this.enabled = false;
        }
    }

    private void OnEnable()
    {
        formatData();
        if (photonView.isMine)
        {
            ShowElectricitRange(true);
        }
    }

    int electricity;
    int _mytouch = -1;
    private void FixedUpdate()
    {
        if (myTouch.Count > 0 && myTouch.Count > _mytouch)
        {
            _mytouch = myTouch.Count;
            FindTower(SceneManager.myTowerObjs, this);
            Debug.Log(name + "FindTower");
        }

        if (myTouch.Count != _mytouch)
            _mytouch = myTouch.Count;

        if (firstE == this && electricity != resource_Electricity)
        {
            electricity = resource_Electricity;
            SynchronizeElectricity();
            
            changeGridColor(0);
            Debug.Log("電量改變囉 " + resource_Electricity);
        }
    }

    #region 目前為玩家幾
    public void checkCurrentPlay()
    {
        if (GameManager.instance.getMyPlayer() == GameManager.MyNowPlayer.player_1)
        {
            Net.RPC("changeLayer", PhotonTargets.All, 30);
        }
        else if (GameManager.instance.getMyPlayer() == GameManager.MyNowPlayer.player_2)
        {
            Net.RPC("changeLayer", PhotonTargets.All, 31);
        }
    }
    #endregion

    #region 恢復初始數據
    protected void formatData()
    {
        if (deadManager == null)
        {
            deadManager = GetComponent<isDead>();
            deadManager.ifDead(false);
        }
        else
        {
            deadManager.ifDead(false);
            if (photonView.isMine)
            {
                SceneManager.AddMyList(gameObject, deadManager.myAttributes);
            }
            else
            {
                SceneManager.AddEnemyList(gameObject, deadManager.myAttributes);
            }
        }

        turretData = originalTurretData;
        healthBar.fillAmount = turretData.UI_Hp / turretData.UI_maxHp;
        resource_Electricity = origine_Electricity;
        electricity = resource_Electricity;
        _mytouch = -1;
    }
    #endregion

    Coroutine deathTimer;

    #region 傷害
    [PunRPC]
    public void takeDamage(float _damage)
    {
        if (deadManager.checkDead)
            return;

        float tureDamage = CalculatorDamage(_damage);
        turretData.UI_Hp -= tureDamage;

        if (turretData.UI_Hp <= 0)
        {
            if(photonView.isMine)
            {
                SceneManager.RemoveMyList(gameObject, GameManager.NowTarget.Electricity);
                ShowElectricitRange(false);
                dead();
            }
            else
            {
                SceneManager.RemoveEnemyList(gameObject, GameManager.NowTarget.Electricity);
            }
            deadManager.ifDead(true);
            deathTimer = StartCoroutine(Death());
        }

        openPopupObject(tureDamage);
    }
    #endregion

    #region 傷害顯示
    void openPopupObject(float _damage)
    {
        FloatingTextController.instance.CreateFloatingText(_damage.ToString("0.0"), this.transform);
        healthBar.fillAmount = turretData.UI_Hp / turretData.UI_maxHp;
    }
    #endregion

    #region 計算傷害
    protected virtual float CalculatorDamage(float _damage)
    {
        return _damage;
    }
    #endregion

    #region 死亡
    protected virtual IEnumerator Death()
    {
        yield return new WaitForSeconds(1.5f);
        firstE = null;
        connectElectricitys.Clear();
        connectTowers.Clear();
        myTouch.Clear();
        //formatData();
        returnBulletPool();
        StopCoroutine(deathTimer);
        deathTimer = null;
    }
    #endregion

    #region 返回物件池
    protected void returnBulletPool()
    {
        if (photonView.isMine)
            ObjectPooler.instance.Repool(DataName, this.gameObject);
        /*else
            Net.RPC("SetActiveF", PhotonTargets.All);*/
    }
    #endregion

    #region 電力範圍
    [SerializeField] List<Collider> gridList;
    [SerializeField] Collider[] ColliderGrids;
    Transform gridparent = null;
    void ShowElectricitRange(bool _open)
    {
        ColliderGrids = Physics.OverlapSphere(transform.position, range, GridMask);

        foreach (var grid in ColliderGrids)
        {
            if (Vector3.Distance(grid.transform.position, transform.position) <= range)
            {
                gridparent = grid.transform.parent;
                if (_open)
                {
                    gridparent.Find("_grid").GetComponent<MeshRenderer>().enabled = true;
                    grid.gameObject.layer = 25;
                    gridList.Add(grid);
                }
                else
                {
                    gridparent.Find("_grid").GetComponent<MeshRenderer>().enabled = false;
                    grid.gameObject.layer = 10;
                    gridList.Remove(grid);
                }
            }
        }
    }
    #endregion
    
    #region 找塔防
    void FindTower(List<GameObject> TurretList, Electricity _electricity)
    {
        foreach (var manager in TurretList)
        {
            if (_electricity.firstE.connectTowers.Contains(manager) || manager.GetComponent<isDead>().myAttributes == GameManager.NowTarget.Electricity)
            {
                Debug.Log("continue");
                continue;
            }

            if (manager.GetComponent<isDead>().myAttributes == GameManager.NowTarget.Electricity)
            {
                Debug.Log("myAttributes");
                continue;
            }

            if (Vector3.Distance(manager.transform.position, _electricity.transform.position) <= range)
            {
                Debug.Log(_electricity.name + "在電力範圍內");

                Collider[] TowerGrids = Physics.OverlapBox(manager.transform.position, new Vector3(5.5f, 2, 5.5f), manager.transform.localRotation, TowerMask);
                Turret_Manager t = manager.GetComponent<Turret_Manager>();
                if (TowerGrids.Length >= t.GridNumber)
                {
                    _electricity.firstE.connectTowers.Add(manager);
                    _electricity.firstE.Use_Electricit(-buildManager.findCost_Electricity(t.DataName));
                    t.power = _electricity;
                    Debug.Log(_electricity.name + "grids.Length >= manager.GridNumber");
                }
                else
                {
                    t.power = null;
                    Debug.Log(_electricity.name + "沒有在網格內");
                }
            }
        }
    }
    #endregion

    #region 改變網格顏色
    public void changeGridColor(int _electricity)
    {
        foreach (var grid in gridList)
        {
            gridparent = grid.transform.parent;

            if (playerObtain.Check_ElectricityAmount(resource_Electricity, _electricity))
            {
                grid.gameObject.layer = 25;
                gridparent.Find("_grid").GetComponent<MeshRenderer>().material.SetColor("_EmissionColor", origonalColor);
            }
            else
            {
                grid.gameObject.layer = 10;
                gridparent.Find("_grid").GetComponent<MeshRenderer>().material.SetColor("_EmissionColor", notBuildColor);
            }
        }
    }
    #endregion

    /*private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, tetst);
    }*/

    #region 使用電力
    public int Use_Electricit(int _needElectr)
    {
        return resource_Electricity += _needElectr;
    }
    #endregion

    #region 同步電量
    void SynchronizeElectricity()
    {
        if (connectElectricitys.Count > 0)
        {
            foreach (var i in connectElectricitys)
            {
                if (i.resource_Electricity == resource_Electricity)
                {
                    Debug.Log("電量相等 : " + i.name);
                    continue;
                }

                i.resource_Electricity = resource_Electricity;
                i.changeGridColor(0);
                Debug.Log(i.name + "有做同步?");
            }
            Debug.Log("誰做" + name);
        }
        else
        {
            Debug.Log(name + " touchElectricitys.Count < 0");
        }
    }
    #endregion

    #region 死亡做的事
    public void dead()
    {
        Debug.Log("然後就死掉了");
        List<Electricity> e = new List<Electricity>();
        List<GameObject> o = new List<GameObject>();
        foreach (var item in myTouch)
        {
            item.myTouch.Remove(this);
        }

        if (firstE != this)
        {
            firstE.connectElectricitys.Remove(this);
            e.Add(firstE);
            firstE.resource_Electricity = firstE.origine_Electricity;
        }

        foreach (var item in firstE.connectTowers)
        {
            item.GetComponent<Turret_Manager>().power = null;
            o.Add(item);
        }

        firstE.connectTowers.Clear();
        
        foreach (var item in firstE.connectElectricitys)
        {
            item.firstE = null;
            item.resource_Electricity = item.origine_Electricity;
            e.Add(item);
        }
        firstE.connectElectricitys.Clear();

        foreach (var item in e)
        {
            Debug.Log("開始重新找FindfirstE" + item.name);
            item.ShowElectricitRange(true);
            buildManager.FindfirstE(e, item);
        }

        foreach (var item in e)
        {
            FindTower(o, item);
        }
    }
    #endregion
}
