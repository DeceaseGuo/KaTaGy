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
                {
                    print("取消建造");
                    buildManager.cancelSelect();
                }
            }
            if (Input.GetMouseButtonDown(1))
            {
                print("取消建造222");
                buildManager.cancelSelect();
                _start = false;
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

                if (builderDistance())
                {
                    if (buildManager.payment(true))
                    {
                        _pos = buildManager.builder.transform.position;
                        buildManager.playerScript.stopAnything_Switch(true);
                        buildManager.openScaffolding(NodePos);
                        buildManager.playerScript.switchScaffolding(true);
                        StartCoroutine("delayToBuildTurret");
                    }
                }
                else
                {
                    buildManager.creatTmpObj(NodePos);
                    buildManager.playerScript.getTatgetPoint(NodePos);
                    _start = true;

                    Debug.Log("距離過遠");
                }
            }
            else
            {
                hintManager.CreatHint("此處不能建造");
            }
        }
    }

    #region 判斷距離
    bool builderDistance()
    {
        if (Vector3.Distance(NodePos, buildManager.builder.transform.position) <= tmpTurretBlueprint.turret_buildDistance)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    #endregion

    #region 前往蓋塔防位置
    public Vector3 _pos;
    void goBuild()
    {
        if (!_start)
        {
            return;
        }

        if (CheckStopPos())
        {
            if (buildManager.payment(true))
            {
                _pos = buildManager.builder.transform.position;
                buildManager.playerScript.stopAnything_Switch(true);
                buildManager.openScaffolding(NodePos);
                buildManager.closeTmpObj();
                buildManager.playerScript.switchScaffolding(true);
                StartCoroutine("delayToBuildTurret");
                _start = false;
            }
        }
    }
    #endregion

    #region 確認是否到達目標位置了
    bool CheckStopPos()
    {
        Vector3 d = buildManager.builder.transform.position + buildManager.builder.transform.up * 2f;
        if (Physics.Linecast(d, d + buildManager.builder.transform.forward * 5f, stopMask))
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
            //buildManager.playerScript.switchScaffolding(true);

            #region 按下esc 中斷建造
            float d = Vector3.Distance(_pos, buildManager.builder.transform.position);
            //print("d" + d);
            //print("_pos" + _pos);
            //print("builder" + buildManager.builder.transform.position);
            if (buildManager.stopBuild || d > 1.5f || Input.GetKeyDown(KeyCode.Escape) || _dead.checkDead)
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