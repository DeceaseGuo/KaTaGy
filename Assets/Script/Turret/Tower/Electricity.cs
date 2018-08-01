using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using AtkTower;

[RequireComponent(typeof(isDead))]
public class Electricity : Photon.MonoBehaviour
{
    [Header("電力範圍")]
    public int resource_Electricity;
    public float range;

    [Header("連接")]
    public Electricity firstE;
    public List<Electricity> connectElectricitys = new List<Electricity>();
    public List<Turret_Manager> connectTowers = new List<Turret_Manager>();
    public List<Electricity> myTouch = new List<Electricity>();

    [Header("網格變色")]
    [SerializeField] Color origonalColor;
    [SerializeField] Color notBuildColor;

    //數據
    public GameManager.whichObject DataName;
    TurretData.TowerDataBase turretData;
    TurretData.TowerDataBase originalTurretData;

    [SerializeField] LayerMask GridMask;
    [SerializeField] LayerMask currentMask;
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

    private void OnEnable()
    {
        formatData();
        /*if (!photonView.isMine)
        {
            this.enabled = false;
            return;
        }*/

        ShowElectricitRange(true);
    }

    int electricity;
    int _mytouch;
    private void FixedUpdate()
    {
        if (myTouch.Count > 0 && myTouch.Count > _mytouch)
        {
            _mytouch = myTouch.Count;
            FindTower(buildManager.Turrets, this);
            Debug.Log(name + "FindTower");
        }

        if (myTouch.Count != _mytouch)
            _mytouch = myTouch.Count;

        if (firstE == this && electricity != resource_Electricity)
        {
            electricity = resource_Electricity;
            SynchronizeElectricity();
            
            changeGridColor(0);
            Debug.Log("電量改變囉");
        }
    }

    #region 目前為玩家幾
    public void checkCurrentPlay()
    {
        if (GameManager.instance.getMyPlayer() == GameManager.MyNowPlayer.player_1)
        {
            Net.RPC("changeLayer", PhotonTargets.All, 30);
            currentMask = GameManager.instance.getPlayer1_Mask;
        }
        else if (GameManager.instance.getMyPlayer() == GameManager.MyNowPlayer.player_2)
        {
            Net.RPC("changeLayer", PhotonTargets.All, 31);
            currentMask = GameManager.instance.getPlayer2_Mask;
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

        myTouch.Clear();
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
            ShowElectricitRange(false);
            
            dead();

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
        formatData();
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
        else
            Net.RPC("SetActiveRPC", PhotonTargets.All, false);
    }
    #endregion

    #region 電力範圍
    [SerializeField] List<Collider> gridList;
    Transform gridparent = null;
    void ShowElectricitRange(bool _open)
    {
        Collider[] ColliderGrids = Physics.OverlapSphere(transform.position, range, GridMask);

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
    public void FindTower(List<Turret_Manager> TurretList, Electricity _electricity)
    {
        foreach (var manager in TurretList)
        {
            if (_electricity.firstE.connectTowers.Contains(manager))
            {
                continue;
            }

            if (Vector3.Distance(manager.transform.position, _electricity.transform.position) <= range)
            {
                Debug.Log(_electricity.name + "在電力範圍內");

                Collider[] grids = Physics.OverlapBox(manager.transform.position, new Vector3(5.5f, 2, 5.5f), manager.transform.localRotation, currentMask);

                if (grids.Length >= manager.GridNumber)
                {
                    _electricity.firstE.connectTowers.Add(manager);
                    _electricity.firstE.Use_Electricit(-buildManager.findCost_Electricity(manager.DataName));
                    manager.power = _electricity;
                    Debug.Log(_electricity.name + "grids.Length >= manager.GridNumber");
                }
                else
                {
                    manager.power = null;
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
            MeshRenderer render = gridparent.Find("_grid").GetComponent<MeshRenderer>();

            if (playerObtain.Check_ElectricityAmount(resource_Electricity, _electricity))
            {
                grid.gameObject.layer = 25;
                render.material.SetColor("_EmissionColor", origonalColor);
            }
            else
            {
                grid.gameObject.layer = 10;
                render.material.SetColor("_EmissionColor", notBuildColor);
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
        List<Electricity> e = new List<Electricity>();
        List<Turret_Manager> o = new List<Turret_Manager>();
        foreach (var item in myTouch)
        {
            item.myTouch.Remove(this);
        }
        buildManager.electricityTurrets.Remove(this);

        if (firstE != this)
        {
            firstE.connectElectricitys.Remove(this);
            e.Add(firstE);
            firstE.resource_Electricity = firstE.origine_Electricity;
        }

        foreach (var item in firstE.connectTowers)
        {
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

        firstE = null;
        
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
