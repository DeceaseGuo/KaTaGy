using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretStore : MonoBehaviour
{
    private TurretData Data;
    private BuildManager buildManager;
    private PlayerObtain playerObtain;
    private HintManager hintManager;

    private void Start()
    {
        Data = TurretData.instance;
        buildManager = BuildManager.instance;
        playerObtain = PlayerObtain.instance;
        hintManager = HintManager.instance;
    }

    public void SelectNowTurret(GameManager.whichObject _name)
    {
        if (!buildManager.nowBuilding)
        {
            Debug.Log("沒有開啟建築模式");
            return;
        }

        if (!buildManager.nowSelect)
        {
            hintManager.CreatHint("目前正在前往蓋塔防");
            return;
        }

        TurretData.TowerDataBase tmpTurret = Data.getTowerData(_name);

        if (tmpTurret.TurretName != GameManager.whichObject.None)
        {
            if (playerObtain.Check_MoneyAmount(tmpTurret.cost_Money))
            {
                buildManager.SelectToBuild(tmpTurret, tmpTurret.detectObjPrefab);
            }
            else
            {
                hintManager.CreatHint("資源不足");
            }
        }
    }
}
