using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using AtkTower;

public class Building : MonoBehaviour
{
    private BuildManager buildManager;
    private TurretData.TowerDataBase tmpTurretBlueprint;
    private HintManager hintManager;
    private Vector3 NodePos;
    bool _start;

    public LayerMask canBuild;
    public bool ifCanBuild;

    [Header("stop")]
    [SerializeField] LayerMask stopMask;

    float CD;

    private void Start()
    {
        buildManager = BuildManager.instance;
        hintManager = HintManager.instance;
    }

    private void Update()
    {
        if (buildManager.HaveTower)
        {
            if (buildManager.nowSelect)
            {
                findPos();
                if (Input.GetMouseButtonDown(1))
                    buildManager.cancelSelect();
            }

            if (!buildManager.nowSelect)
            {
                goBuild();
            }
        }
    }

    void findPos()
    {
        if (EventSystem.current.IsPointerOverGameObject())
            return;

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Input.GetMouseButtonDown(0) && Physics.Raycast(ray, out hit, 100f, canBuild))
        {
            if (ifCanBuild)
            {
                tmpTurretBlueprint = buildManager.GetTurretToBuild();
                //取得建造位置
                NodePos = tmpTurretBlueprint.detectObjPrefab.GetComponentInChildren<SnapGrid_Pos>().nodePos();
                NodePos.y = hit.transform.position.y;
                
                buildManager.nowNotSelectSwitch(false);

                if (builderDistance(NodePos, tmpTurretBlueprint.turret_buildDistance))
                {
                    if (buildManager.payment(true))
                    {
                        buildManager.playerScript.stopAnything_Switch(true);
                        buildManager.openScaffolding(NodePos);
                        StartCoroutine("delayToBuildTurret");
                    }
                }
                else
                {
                    buildManager.creatTmpObj(NodePos);
                }
            }
            else
            {
                hintManager.CreatHint("此處不能建造");
            }
        }
    }

    #region 判斷距離
    bool builderDistance(Vector3 _buildPos, float _distance)
    {
        if (Vector3.Distance(_buildPos, buildManager.builder.transform.position) < _distance)
        {
            return true;
        }
        else
        {
            buildManager.playerScript.getTatgetPoint(_buildPos);
            _start = true;
            Debug.Log("距離過遠");
            return false;
        }
    }
    #endregion

    #region 前往蓋塔防位置
    void goBuild()
    {
        if (!_start)
        {
            return;
        }

        if (CheckStopPos())//抵達建造位置
        {
            buildManager.currentPlayerPos = buildManager.builder.transform.position;
            if (buildManager.payment(true))
            {
                buildManager.playerScript.stopAnything_Switch(true);
                buildManager.openScaffolding(NodePos);
                buildManager.closeTmpObj();
                StartCoroutine("delayToBuildTurret");
                _start = false;
            }
        }

        if (Input.GetMouseButtonDown(1))
        {
            _start = false;
            buildManager.closeTmpObj();
            buildManager.closeTurretToBuild();
        }
    }
    #endregion

    #region 確認是否到達目標位置了
    bool CheckStopPos()
    {
        if (Physics.Linecast(buildManager.builder.transform.position + buildManager.builder.transform.up * 2f, buildManager.builder.transform.position + buildManager.builder.transform.forward * 3f + buildManager.builder.transform.up * 2f, stopMask))
        {
            buildManager.playerScript.isStop();
            return true;
        }
        else
        {
            if (!buildManager.playerScript.getNavPath())
                buildManager.playerScript.getTatgetPoint(NodePos);
            return false;
        }
    }
    #endregion

   /* private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(builder.transform.position + builder.transform.up * 2f, builder.transform.position + builder.transform.forward * 3f + builder.transform.up * 2f);
    }*/

    #region 開始建造(延遲)
    IEnumerator delayToBuildTurret()
    {
        //bool suspend = false;
        isDead _dead = buildManager.playerScript.GetComponent<isDead>();
        for (CD = 0; CD < tmpTurretBlueprint.turret_delayTime; CD += Time.deltaTime)
        {
            buildManager.build_countDown(CD);
            buildManager.playerScript.switchScaffolding(true);
                
            #region 按下esc 中斷建造
            if (buildManager.StopBuild() || Input.GetKeyDown(KeyCode.Escape) || _dead.checkDead)
            {
                buildManager.playerScript.switchScaffolding(false);
                buildManager.cancelPunish(0.8f);
                buildManager.closeScaffolding();
                buildManager.closeTurretToBuild();
                buildManager.playerScript.stopAnything_Switch(false);
                hintManager.CreatHint("中斷建造");
                break;
            }
            #endregion
            yield return 0;

        }
        if (CD >= tmpTurretBlueprint.turret_delayTime/* && !suspend*/)
        {
            //Debug.Log("蓋");
            buildManager.playerScript.switchScaffolding(false);
            BuildTurret(NodePos);
            buildManager.playerScript.stopAnything_Switch(false);
        }
    }
    #endregion

    #region 蓋塔防
    void BuildTurret(Vector3 _pos)
    {
        buildManager.closeScaffolding();

        GameObject obj = buildManager.creatTower(tmpTurretBlueprint.TurretName, _pos, Quaternion.identity);

        if (tmpTurretBlueprint.TurretName != GameManager.whichObject.Tower_Electricity)
        {
            Turret_Manager tur_manager = obj.GetComponent<Turret_Manager>();
            buildManager.consumeElectricity(buildManager.SceneManager.myElectricityObjs, tur_manager);
        }
        else
        {
            Electricity e = obj.GetComponent<Electricity>();
            buildManager.FindfirstE(buildManager.SceneManager.myElectricityObjs, e);
        }
        buildManager.closeTurretToBuild();
    }
    #endregion
}