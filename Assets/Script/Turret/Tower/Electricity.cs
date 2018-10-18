using System.Collections.Generic;
using UnityEngine;
using AtkTower;

public class Electricity : Turret_Manager
{
    //原始電力數據→→→ 塔防數據的Atk_Damage
    //電力Bar條 →→→ Fad_energyBar
    [Header("電力範圍")]
    public int resource_Electricity;
    public float range;
    private Vector3 rangeV3;

    [Header("連接")]
    public Electricity firstE;
    public List<Electricity> connectElectricitys = new List<Electricity>();
    public List<Turret_Manager> connectTowers = new List<Turret_Manager>();
    public List<Electricity> myTouch = new List<Electricity>();

    [Header("網格變色")]
    //原始顏色→→→orininalColor
    //過熱顏色→→→overHeatColor
    [SerializeField] LayerMask GridMask;
    [SerializeField] LayerMask TowerMask;

    PlayerObtain playerObtain;
    BuildManager buildManager;

    private Collider[] tmpCollider;

    public List<MeshRenderer> gridMeshList = new List<MeshRenderer>();

    int electricity;  //電力閃控
    int _mytouch = -1;

    /*private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position, rangeV3);
    }*/

    private void Start()
    {
        buildManager = BuildManager.instance;
        playerObtain = PlayerObtain.instance;
    }

    #region 恢復初始數據
    protected override void FormatData()
    {
        if (photonView.isMine)
        {
            SceneManager.AddMy_Electricity(this);
            if (originalTurretData.ATK_Level != TurretData.myTowerAtkLevel || originalTurretData.DEF_Level != TurretData.myTowerDefLevel)
                originalTurretData = TurretData.instance.getTowerData(DataName);

            //取得第一波網格
            tmpCollider = Physics.OverlapBox(transform.position, rangeV3, Quaternion.identity, GridMask);
            if (tmpCollider.Length != 0)
            {
                for (int i = 0; i < tmpCollider.Length; i++)
                {
                    if (Vector3.SqrMagnitude(tmpCollider[i].transform.position - transform.position) <= range * range)
                    {
                        tmpCollider[i].gameObject.layer = 25;
                        gridMeshList.Add(tmpCollider[i].GetComponent<MeshRenderer>());
                    }
                }
            }
        }
        else
        {
            SceneManager.AddEnemy_Electricity(this);
            if (originalTurretData.ATK_Level != TurretData.enemyTowerAtkLevel || originalTurretData.DEF_Level != TurretData.enemyTowerDefLevel)
                originalTurretData = TurretData.instance.getEnemyTowerData(DataName);
        }
        turretData = originalTurretData;
        deadManager.ifDead(false);
        turretData.UI_Hp = turretData.UI_maxHp;
        turretData.Fad_thermalEnergy = 0;

        healthBar.fillAmount = 1;
        Fad_energyBar.fillAmount = 0.0f;
        resource_Electricity = (int)originalTurretData.Atk_Damage;
        electricity = resource_Electricity;
        _mytouch = -1;
    }

    public override void GoFormatData()
    {
        //myRender.material.SetFloat("Vector1_D655974D", 0);

        //(true物件池生成會先第一次執行一次)
        //false從物件池哪出後執行
        if (!firstGetData)
        {
            rangeV3 = new Vector3(range, 1, range);
            FormatData();
        }
        else
            FirstformatData();
    }
    #endregion

    public override void NeedToUpdate()
    {
        //相接的電力塔炸開
        if (myTouch.Count > _mytouch)
        {
            _mytouch = myTouch.Count;
            FindTower(SceneManager.myTowerObjs, this);
        }

        if (myTouch.Count != _mytouch)
            _mytouch = myTouch.Count;

        //電量有改
        if (firstE == this && electricity != resource_Electricity)
        {
            electricity = resource_Electricity;
            SynchronizeElectricity();

            changeGridColor(0);
            //Debug.Log("電量改變囉 " + resource_Electricity);
        }
    }

    #region 電力範圍
    void ShowElectricitRange(bool _open)
    {
        for (int i = 0; i < gridMeshList.Count; i++)
        {
            gridMeshList[i].gameObject.layer = (_open) ? 25 : 10;
        }
    }
    #endregion

    #region 找塔防
    private Vector3 tmpV3 = new Vector3(5.5f, 2, 5.5f);
    void FindTower(List<Turret_Manager> TurretList, Electricity _electricity)
    {
        for (int i = 0; i < TurretList.Count; i++)
        {
            if (_electricity.firstE.connectTowers.Contains(TurretList[i]))
                continue;

            if (Vector3.SqrMagnitude(TurretList[i].transform.position - _electricity.transform.position) <= range * range)
            {
                if (TurretList[i].GridNumber <= 1)
                {
                    _electricity.firstE.connectTowers.Add(TurretList[i]);
                    _electricity.firstE.Use_Electricit(-(TurretList[i].GetMyElectricity()));
                    TurretList[i].power = _electricity;
                    continue;
                }
                tmpCollider = Physics.OverlapBox(TurretList[i].transform.position, tmpV3, TurretList[i].transform.localRotation, TowerMask);
                if (tmpCollider.Length >= TurretList[i].GridNumber)
                {
                    _electricity.firstE.connectTowers.Add(TurretList[i]);
                    _electricity.firstE.Use_Electricit(-(TurretList[i].GetMyElectricity()));
                    TurretList[i].power = _electricity;
                }
                else
                    TurretList[i].power = null;
            }
        }
    }
    #endregion

    #region 改變網格顏色
    public void changeGridColor(int _electricity)
    {
        if (playerObtain.Check_ElectricityAmount(resource_Electricity, _electricity))
        {
            for (int i = 0; i < gridMeshList.Count; i++)
            {
                gridMeshList[i].gameObject.layer = 25;
                gridMeshList[i].material.SetColor("_EmissionColor", orininalColor);
            }
        }
        else
        {
            for (int i = 0; i < gridMeshList.Count; i++)
            {
                gridMeshList[i].gameObject.layer = 10;
                gridMeshList[i].material.SetColor("_EmissionColor", overHeatColor);
            }         
        }
    }
    #endregion

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

    #region 死亡
    protected override void Death()
    {
        if (photonView.isMine)
        {
            SceneManager.RemoveMy_Electricity(this);
            ShowElectricitRange(false);
            dead();
            firstE = null;
            connectElectricitys.Clear();
            connectTowers.Clear();
            myTouch.Clear();
            gridMeshList.Clear();
        }
        else
        {
            SceneManager.RemoveEnemy_Electricity(this);
        }

        Invoke("Return_ObjPool", 2f);
    }
    #endregion

    #region 死亡做的事
    public void dead()
    {
        List<Electricity> e = new List<Electricity>();
        List<Turret_Manager> o = new List<Turret_Manager>();

        for (int i = 0; i < myTouch.Count; i++)
        {
            myTouch[i].ShowElectricitRange(true);
            myTouch[i].myTouch.Remove(this);
        }

        if (firstE != this)
        {
            firstE.connectElectricitys.Remove(this);
            e.Add(firstE);
            firstE.resource_Electricity = (int)firstE.turretData.Atk_Damage;
        }

        for (int i = 0; i < firstE.connectTowers.Count; i++)
        {
            firstE.connectTowers[i].power = null;
            o.Add(firstE.connectTowers[i]);
        }
        
        for (int i = 0; i < firstE.connectElectricitys.Count; i++)
        {
            firstE.connectElectricitys[i].firstE = null;
            firstE.connectElectricitys[i].resource_Electricity = (int)firstE.connectElectricitys[i].turretData.Atk_Damage;
            e.Add(firstE.connectElectricitys[i]);
        }

        firstE.connectTowers.Clear();
        firstE.connectElectricitys.Clear();

        for (int i = 0; i < e.Count; i++)
        {
            //e[i].ShowElectricitRange(true);
            buildManager.FindfirstE(e, e[i]);
        }

        for (int i = 0; i < e.Count; i++)
        {
            FindTower(o, e[i]);
        }
    }
    #endregion
}