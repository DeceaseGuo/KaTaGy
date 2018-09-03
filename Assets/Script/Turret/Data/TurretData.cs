using UnityEngine;
using System.Collections.Generic;

public class TurretData : MonoBehaviour
{
    public static TurretData instance;

    [System.Serializable]
    public struct TowerDataBase
    {
        public string objectName;
        public GameManager.whichObject TurretName;
        public Sprite headImage;
        [Header("攻擊")]
        public float Atk_Damage;
        public float Atk_maxDamage;
        public float Atk_Gap;
        public float Atk_Range;
        public float Atk_MinRange;
        public int ATK_Level;
        [Header("防禦")]
        public float def_maxDEF;
        public float def_base;
        public int DEF_Level;
        [Header("子彈")]
        public GameManager.whichObject bullet_Name;
        public float bullet_Speed;
        [Header("UI參數")]
        public float UI_maxHp;
        public float UI_Hp;
        [Header("過熱")]
        public float Fad_oneEnergy;     //砲台每次使用增加量
        public float Fad_decreaseRate;  //每秒減少的熱能
        public float Fad_thermalEnergy;  //當前熱能
        public float Fad_maxThermalEnergy; //最大上限
        public float Over_downSpd; //過熱時降速
        public bool Fad_overHeat;

        [Header("偵測")]
        public float turret_buildDistance;
        public GameObject detectObjPrefab;
        public GameManager.whichObject tspObject_Name;
        [Header("花費")]
        public float turret_delayTime;
        public int cost_Money;
        public int cost_Electricity;
    }

    GameManager.WhichObjectEnumComparer myEnumComparer = new GameManager.WhichObjectEnumComparer();
    public Dictionary<GameManager.whichObject, TowerDataBase> myDataBase;
    public Dictionary<GameManager.whichObject, TowerDataBase> enemyDataBase;
    public List<TowerDataBase> Towers;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }

        addToDictionary();
    }

    void addToDictionary()
    {
        myDataBase = new Dictionary<GameManager.whichObject, TowerDataBase>(myEnumComparer);
        enemyDataBase = new Dictionary<GameManager.whichObject, TowerDataBase>(myEnumComparer);
        for (int i = 0; i < Towers.Count; i++)
        {
            myDataBase.Add(Towers[i].TurretName, Towers[i]);
            enemyDataBase.Add(Towers[i].TurretName, Towers[i]);
        }
    }

    #region 改變塔防數據
    //我方
    public void changeMyData(GameManager.whichObject _name, TowerDataBase _data)
    {
        myDataBase[_name] = _data;
    }
    //敵方
    public void changeEnemyData(GameManager.whichObject _name, TowerDataBase _data)
    {
        enemyDataBase[_name] = _data;
    }
    #endregion

    #region 取得塔防數據
    //我方
    public TowerDataBase getTowerData(GameManager.whichObject _name)
    {
        TowerDataBase tmpData = new TowerDataBase();
        myDataBase.TryGetValue(_name, out tmpData);
        return tmpData;
    }
    //敵方
    public TowerDataBase getEnemyTowerData(GameManager.whichObject _name)
    {
        TowerDataBase tmpData = new TowerDataBase();
        enemyDataBase.TryGetValue(_name, out tmpData);
        return tmpData;
    }
    #endregion
}
