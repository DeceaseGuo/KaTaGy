using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

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
        if (buildManager.nowBuilding)
        {
            if (buildManager.CheckDetectTurret())
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
    }

    void findPos()
    {
        if (EventSystem.current.IsPointerOverGameObject())
            return;

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 100f, canBuild))
        {
            if (Input.GetMouseButtonDown(0))
            {
                if (ifCanBuild)
                {
                    tmpTurretBlueprint = buildManager.GetTurretToBuild();
                    NodePos = tmpTurretBlueprint.detectObjPrefab.GetComponentInChildren<SnapGrid_Pos>().nodePos();
                    NodePos.y = hit.transform.position.y;
                    buildManager.creatTmpObj(NodePos);
                    buildManager.nowNotSelectSwitch(false);
                    if (builderDistance(NodePos, tmpTurretBlueprint.turret_buildDistance))
                    {
                        if (buildManager.payment(true))
                        {
                            buildManager.playerScript.stopAnything_Switch(true);
                            buildManager.openScaffolding(NodePos);
                            buildManager.closeTmpObj();
                            StartCoroutine("delayToBuildTurret");
                        }
                    }
                }
                else
                {
                    hintManager.CreatHint("此處不能建造");
                }
            }
        }
    }

    #region 判斷距離
    bool builderDistance(Vector3 _buildPos, float _distance)
    {
        float distance = Vector3.Distance(_buildPos, buildManager.builder.transform.position);
        if (distance < _distance)
        {
            return true;
        }
        else
        {
            if (buildManager.payment(false))
            {
                // Vector3 canBuildPos = _buildPos + (builder.transform.position - _buildPos).normalized * (_distance - 1.5f);
                //buildManager.playerScript.getTatgetPoint(_buildPos);
                buildManager.playerScript.Net.RPC("getTatgetPoint", PhotonTargets.All, _buildPos);
                _start = true;
            }
            Debug.Log("距離過遠");
            return false;
        }
    }
    #endregion

    #region 前往蓋塔防位置
    void goBuild()
    {
        if (_start)
        {
            if(CheckStopPos())
            {
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
                buildManager.nowNotSelectSwitch(true);
                buildManager.closeTurretToBuild();
            }
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
        return false;
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
        bool suspend = false;
        for (CD = 0; CD < tmpTurretBlueprint.turret_delayTime; CD += Time.deltaTime)
        {
            Debug.Log("cd");
            buildManager.build_countDown(CD);

            #region 按下esc 中斷建造
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                suspend = true;
                CD = tmpTurretBlueprint.turret_delayTime;
                buildManager.cancelPunish(0.8f);
                buildManager.closeScaffolding();
                buildManager.nowNotSelectSwitch(true);
                buildManager.closeTurretToBuild();
                buildManager.playerScript.stopAnything_Switch(false);
                hintManager.CreatHint("中斷建造");
            }
            #endregion
            yield return 0;

        }
        if (CD >= tmpTurretBlueprint.turret_delayTime && !suspend)
        {
            Debug.Log("蓋");            
            BuildTurret(NodePos);
            buildManager.playerScript.stopAnything_Switch(false);
        }
    }
    #endregion

    #region 蓋塔防
    void BuildTurret(Vector3 _pos)
    {
        buildManager.closeScaffolding();
        buildManager.nowNotSelectSwitch(true);
        buildManager.closeTurretToBuild();
        buildManager.creatTower(tmpTurretBlueprint.TurretName, _pos, Quaternion.identity);
    }
    #endregion
}