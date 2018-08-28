using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoldierStore : MonoBehaviour
{
    public void SelectSoldier(GameManager.whichObject _name)
    {
        if (!BuildManager.instance.nowBuilding)
        {
            MyEnemyData.Enemies tmpSoldier = MyEnemyData.instance.getMySoldierData(_name);
            if (tmpSoldier._soldierName != GameManager.whichObject.None)
            {
                if (PlayerObtain.instance.Check_MoneyAmount(tmpSoldier.cost_Money))
                {
                    PlayerObtain.instance.consumeResource(tmpSoldier.cost_Money);
                    EnemyManager.instance.getEnemyQueue(tmpSoldier);
                    
                }
                else
                {
                    HintManager.instance.CreatHint("資源不足");
                }
            }
        }
        else
        {
            HintManager.instance.CreatHint("目前為建造模式");
        }
    }
}
