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

    [Header("位置")]
    public Transform Pos_rotation;

    [Header("UI部分")]
    public Image healthBar;

    protected isDead deadManager;
    protected PhotonView Net;
    PlayerObtain playerObtain;
    BuildManager buildManager;
    int origine_Electricity;

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
            Collider[] ColliderGrids = Physics.OverlapSphere(transform.position, range, GridMask);

            if (ColliderGrids.Length != 0)
            {
                for (int i = 0; i < ColliderGrids.Length; i++)
                {
                    if (Vector3.Distance(ColliderGrids[i].transform.position, transform.position) <= range)
                    {
                        gridparent = ColliderGrids[i].transform.parent;
                        gridparent.Find("_grid").GetComponent<MeshRenderer>().enabled = true;
                        ColliderGrids[i].gameObject.layer = 25;
                        gridList.Add(ColliderGrids[i]);
                    }
                }
            }
        }
    }

    int electricity;
    int _mytouch = -1;
    private void FixedUpdate()
    {
        if (myTouch.Count > _mytouch)
        {
            _mytouch = myTouch.Count;
            FindTower(SceneManager.myTowerObjs, this);
        }

        if (myTouch.Count != _mytouch)
            _mytouch = myTouch.Count;

        if (firstE == this && electricity != resource_Electricity)
        {
            electricity = resource_Electricity;
            SynchronizeElectricity();
            
            changeGridColor(0);
            //Debug.Log("電量改變囉 " + resource_Electricity);
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
                SceneManager.AddMy_Electricity(this);
            }
            else
            {
                SceneManager.AddEnemy_Electricity(this);
            }
        }

        turretData = originalTurretData;
        healthBar.fillAmount = turretData.UI_Hp / turretData.UI_maxHp;
        resource_Electricity = origine_Electricity;
        electricity = resource_Electricity;
        _mytouch = -1;
    }
    #endregion

    #region 傷害
    [PunRPC]
    public void takeDamage(float _damage)
    {
        if (deadManager.checkDead)
            return;

        float tureDamage = CalculatorDamage(_damage);
        turretData.UI_Hp -= tureDamage;
        openPopupObject(tureDamage);

        if (turretData.UI_Hp <= 0)
        {
            if(photonView.isMine)
            {
                SceneManager.RemoveMy_Electricity(this);
                ShowElectricitRange(false);
                dead();
            }
            else
            {
                SceneManager.RemoveEnemy_Electricity(this);
            }
            deadManager.ifDead(true);
            StartCoroutine(Death());
        }
    }
    #endregion

    #region 傷害顯示
    void openPopupObject(float _damage)
    {
        FloatingTextController.instance.CreateFloatingText(_damage, transform);
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
        gridList.Clear();
        returnBulletPool();
    }
    #endregion

    #region 返回物件池
    protected void returnBulletPool()
    {
        if (photonView.isMine)
            ObjectPooler.instance.Repool(DataName, this.gameObject);
    }
    #endregion

    #region 電力範圍
    [SerializeField] List<Collider> gridList = new List<Collider>();
    Transform gridparent = null;
    void ShowElectricitRange(bool _open)
    {
        for (int i = 0; i < gridList.Count; i++)
        {
            gridparent = gridList[i].transform.parent;
            gridparent.Find("_grid").GetComponent<MeshRenderer>().enabled = _open;
            gridList[i].gameObject.layer = (_open) ? 25 : 10;
        }
    }
    #endregion

    #region 找塔防
    void FindTower(List<GameObject> TurretList, Electricity _electricity)
    {
        for (int i = 0; i < TurretList.Count; i++)
        {
            Turret_Manager t = TurretList[i].GetComponent<Turret_Manager>();
            if (_electricity.firstE.connectTowers.Contains(TurretList[i]) || t == null)
                continue;

            if (Vector3.Distance(TurretList[i].transform.position, _electricity.transform.position) <= range)
            {               
                if (t.GridNumber <= 1)
                {
                    _electricity.firstE.connectTowers.Add(TurretList[i]);
                    _electricity.firstE.Use_Electricit(-buildManager.findCost_Electricity(t.DataName));
                    t.power = _electricity;
                    continue;
                }

                Collider[] TowerGrids = Physics.OverlapBox(TurretList[i].transform.position, new Vector3(5.5f, 2, 5.5f), TurretList[i].transform.localRotation, TowerMask);
                if (TowerGrids.Length >= t.GridNumber)
                {
                    _electricity.firstE.connectTowers.Add(TurretList[i]);
                    _electricity.firstE.Use_Electricit(-buildManager.findCost_Electricity(t.DataName));
                    t.power = _electricity;
                }
                else
                    t.power = null;
            }
        }
    }
    #endregion

    #region 改變網格顏色
    public void changeGridColor(int _electricity)
    {
        if (playerObtain.Check_ElectricityAmount(resource_Electricity, _electricity))
        {
            for (int i = 0; i < gridList.Count; i++)
            {
                gridparent = gridList[i].transform.parent;
                gridList[i].gameObject.layer = 25;
                gridparent.Find("_grid").GetComponent<MeshRenderer>().material.SetColor("_EmissionColor", origonalColor);
            }
        }
        else
        {
            for (int i = 0; i < gridList.Count; i++)
            {
                gridparent = gridList[i].transform.parent;
                gridList[i].gameObject.layer = 10;
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
            for (int i = 0; i < connectElectricitys.Count; i++)
            {
                if (connectElectricitys[i].resource_Electricity != resource_Electricity)
                {
                    connectElectricitys[i].resource_Electricity = resource_Electricity;
                    connectElectricitys[i].changeGridColor(0);
                }
            }
            //Debug.Log("誰做" + name);
        }
    }
    #endregion

    #region 死亡做的事
    public void dead()
    {
        //Debug.Log("然後就死掉了");
        List<Electricity> e = new List<Electricity>();
        List<GameObject> o = new List<GameObject>();
        for (int i = 0; i < myTouch.Count; i++)
        {
            myTouch[i].myTouch.Remove(this);
        }

        if (firstE != this)
        {
            firstE.connectElectricitys.Remove(this);
            e.Add(firstE);
            firstE.resource_Electricity = firstE.origine_Electricity;
        }

        for (int i = 0; i < firstE.connectTowers.Count; i++)
        {
            firstE.connectTowers[i].GetComponent<Turret_Manager>().power = null;
            o.Add(firstE.connectTowers[i]);
        }
        
        for (int i = 0; i < firstE.connectElectricitys.Count; i++)
        {
            firstE.connectElectricitys[i].firstE = null;
            firstE.connectElectricitys[i].resource_Electricity = firstE.connectElectricitys[i].origine_Electricity;
            e.Add(firstE.connectElectricitys[i]);
        }

        firstE.connectTowers.Clear();
        firstE.connectElectricitys.Clear();

        for (int i = 0; i < e.Count; i++)
        {
            e[i].ShowElectricitRange(true);
            buildManager.FindfirstE(e, e[i]);
        }

        for (int i = 0; i < e.Count; i++)
        {
            FindTower(o, e[i]);
        }
    }
    #endregion
}
