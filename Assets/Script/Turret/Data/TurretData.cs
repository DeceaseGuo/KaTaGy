using UnityEngine;
using System.Collections.Generic;

public class TurretData : MonoBehaviour
{
    public static TurretData instance;
    public static byte myTowerAtkLevel = 0;
    public static byte enemyTowerAtkLevel = 0;
    public static byte myTowerDefLevel = 0;
    public static byte enemyTowerDefLevel = 0;
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
        public float def_base;
        public int DEF_Level;
        [Header("子彈")]
        public GameManager.whichObject bullet_Name;
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
        public UpdateDataBase.TowerUpdateData updateData;
    }

    GameManager.WhichObjectEnumComparer myEnumComparer = new GameManager.WhichObjectEnumComparer();
    public Dictionary<GameManager.whichObject, TowerDataBase> myDataBase;
    public Dictionary<GameManager.whichObject, TowerDataBase> enemyDataBase;
    public List<TowerDataBase> Towers;

    private List<GameManager.whichObject> towerKey = new List<GameManager.whichObject>();
    private TowerDataBase tmpUpdateData;

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
            towerKey.Add(Towers[i].TurretName);
            myDataBase.Add(Towers[i].TurretName, Towers[i]);
            enemyDataBase.Add(Towers[i].TurretName, Towers[i]);
        }
    }

    #region 改變塔防數據
    /*//我方
    public void changeMyData(GameManager.whichObject _name, TowerDataBase _data)
    {
        myDataBase[_name] = _data;
    }
    //敵方
    public void changeEnemyData(GameManager.whichObject _name, TowerDataBase _data)
    {
        enemyDataBase[_name] = _data;
    }*/

    //全體功
    public void ChangeMyAtkData(byte _level)
    {
        myTowerAtkLevel = _level;
        for (int i = 0; i < towerKey.Count; i++)
        {
            tmpUpdateData = myDataBase[towerKey[i]];

            switch (_level)
            {
                case (1):
                    if (tmpUpdateData.ATK_Level != _level)
                    {
                        tmpUpdateData.ATK_Level = _level;
                        tmpUpdateData.Atk_maxDamage += tmpUpdateData.updateData.Add_atk1;
                        tmpUpdateData.Atk_Damage += tmpUpdateData.updateData.Add_atk1;
                    }
                    break;
                case (2):
                    if (tmpUpdateData.ATK_Level != _level)
                    {
                        tmpUpdateData.ATK_Level = _level;
                        tmpUpdateData.Atk_maxDamage += tmpUpdateData.updateData.Add_atk2;
                        tmpUpdateData.Atk_Damage += tmpUpdateData.updateData.Add_atk2;
                    }
                    break;
                case (3):
                    if (tmpUpdateData.ATK_Level != _level)
                    {
                        tmpUpdateData.ATK_Level = _level;
                        tmpUpdateData.Atk_maxDamage += tmpUpdateData.updateData.Add_atk3;
                        tmpUpdateData.Atk_Damage += tmpUpdateData.updateData.Add_atk3;
                    }
                    break;
                default:
                    break;
            }
            myDataBase[towerKey[i]] = tmpUpdateData;
        }
    }
    public void ChangeEnemyAtkData(byte _level)
    {
        enemyTowerAtkLevel = _level;
        for (int i = 0; i < towerKey.Count; i++)
        {
            tmpUpdateData = enemyDataBase[towerKey[i]];

            switch (_level)
            {
                case (1):
                    if (tmpUpdateData.ATK_Level != _level)
                    {
                        tmpUpdateData.ATK_Level = _level;
                        tmpUpdateData.Atk_maxDamage += tmpUpdateData.updateData.Add_atk1;
                        tmpUpdateData.Atk_Damage += tmpUpdateData.updateData.Add_atk1;
                    }
                    break;
                case (2):
                    if (tmpUpdateData.ATK_Level != _level)
                    {
                        tmpUpdateData.ATK_Level = _level;
                        tmpUpdateData.Atk_maxDamage += tmpUpdateData.updateData.Add_atk2;
                        tmpUpdateData.Atk_Damage += tmpUpdateData.updateData.Add_atk2;
                    }
                    break;
                case (3):
                    if (tmpUpdateData.ATK_Level != _level)
                    {
                        tmpUpdateData.ATK_Level = _level;
                        tmpUpdateData.Atk_maxDamage += tmpUpdateData.updateData.Add_atk3;
                        tmpUpdateData.Atk_Damage += tmpUpdateData.updateData.Add_atk3;
                    }
                    break;
                default:
                    break;
            }
            enemyDataBase[towerKey[i]] = tmpUpdateData;
        }
    }

    //全體防
    public void ChangeMyDefData(byte _level)
    {
        myTowerDefLevel = _level;
        for (int i = 0; i < towerKey.Count; i++)
        {
            tmpUpdateData = myDataBase[towerKey[i]];
            switch (_level)
            {
                case (1):
                    if (tmpUpdateData.DEF_Level != _level)
                    {
                        tmpUpdateData.DEF_Level = _level;
                        tmpUpdateData.def_base += tmpUpdateData.updateData.Add_def1;
                        tmpUpdateData.UI_Hp += tmpUpdateData.updateData.Add_hp1;
                        tmpUpdateData.UI_maxHp += tmpUpdateData.updateData.Add_hp1;
                    }
                    break;
                case (2):
                    if (tmpUpdateData.DEF_Level != _level)
                    {
                        tmpUpdateData.DEF_Level = _level;
                        tmpUpdateData.def_base += tmpUpdateData.updateData.Add_def2;
                        tmpUpdateData.UI_Hp += tmpUpdateData.updateData.Add_hp2;
                        tmpUpdateData.UI_maxHp += tmpUpdateData.updateData.Add_hp2;
                    }
                    break;
                case (3):
                    if (tmpUpdateData.DEF_Level != _level)
                    {
                        tmpUpdateData.DEF_Level = _level;
                        tmpUpdateData.def_base += tmpUpdateData.updateData.Add_def3;
                        tmpUpdateData.UI_Hp += tmpUpdateData.updateData.Add_hp3;
                        tmpUpdateData.UI_maxHp += tmpUpdateData.updateData.Add_hp3;
                    }
                    break;
                default:
                    break;
            }
            myDataBase[towerKey[i]] = tmpUpdateData;
        }
    }
    public void ChangeEnemyDefData(byte _level)
    {
        enemyTowerDefLevel = _level;
        for (int i = 0; i < towerKey.Count; i++)
        {
            tmpUpdateData = enemyDataBase[towerKey[i]];
            switch (_level)
            {
                case (1):
                    if (tmpUpdateData.DEF_Level != _level)
                    {
                        tmpUpdateData.DEF_Level = _level;
                        tmpUpdateData.def_base += tmpUpdateData.updateData.Add_def1;
                        tmpUpdateData.UI_Hp += tmpUpdateData.updateData.Add_hp1;
                        tmpUpdateData.UI_maxHp += tmpUpdateData.updateData.Add_hp1;
                    }
                    break;
                case (2):
                    if (tmpUpdateData.DEF_Level != _level)
                    {
                        tmpUpdateData.DEF_Level = _level;
                        tmpUpdateData.def_base += tmpUpdateData.updateData.Add_def2;
                        tmpUpdateData.UI_Hp += tmpUpdateData.updateData.Add_hp2;
                        tmpUpdateData.UI_maxHp += tmpUpdateData.updateData.Add_hp2;
                    }
                    break;
                case (3):
                    if (tmpUpdateData.DEF_Level != _level)
                    {
                        tmpUpdateData.DEF_Level = _level;
                        tmpUpdateData.def_base += tmpUpdateData.updateData.Add_def3;
                        tmpUpdateData.UI_Hp += tmpUpdateData.updateData.Add_hp3;
                        tmpUpdateData.UI_maxHp += tmpUpdateData.updateData.Add_hp3;
                    }
                    break;
                default:
                    break;
            }
            enemyDataBase[towerKey[i]] = tmpUpdateData;
        }
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
