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
        public int cost_Ore;
        public int cost_Money;
        public int cost_Electricity;
    }

    public Dictionary<GameManager.whichObject, TowerDataBase> DataBase = new Dictionary<GameManager.whichObject, TowerDataBase>();
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
        for (int i = 0; i < Towers.Count; i++)
        {
            DataBase.Add(Towers[i].TurretName, Towers[i]);
        }
    }

    public TowerDataBase getTowerData(GameManager.whichObject _name)
    {
        if (!DataBase.ContainsKey(_name))
        {
            Debug.Log("沒有此塔防數據");
            return new TowerDataBase();
        }
        return DataBase[_name];
    }
}
