using UnityEngine;
using System.Collections.Generic;

public class MyEnemyData : MonoBehaviour
{
    public static MyEnemyData instance;
    public static byte mySoldierAtkLevel = 0;
    public static byte enemySoldierAtkLevel = 0;
    public static byte mySoldierDefLevel = 0;
    public static byte enemySoldierDefLevel = 0;
    [System.Serializable]
    public struct Enemies
    {
        public string objectName;
        public GameManager.whichObject _soldierName;
        public Sprite headImage;
        public Sprite firstAtk;
        public float width;
        public float moveSpeed;
        public float rotSpeed;
        public float stoppingDst;
        [Header("UI參數")]
        public float UI_MaxHp;
        public float UI_HP;

        [Header("攻擊")]
        public float atk_maxDamage;
        public float atk_Damage;
        public float atk_Range;
        public float atk_delay;
        public float beAtk_delay;
        public byte ATK_Level;
        [Header("防禦")]
        public float def_base;
        public byte DEF_Level;
        [Header("生產所需")]
        public int population_need;
        public float soldier_CountDown;
        [Header("花費")]
        public int cost_Money;
        public UpdateDataBase.SoldierUpdateData updateData;
    }

    GameManager.WhichObjectEnumComparer myEnumComparer = new GameManager.WhichObjectEnumComparer();
    public Dictionary<GameManager.whichObject, Enemies> myDataBase;
    public Dictionary<GameManager.whichObject, Enemies> enemyDataBase;
    public List<Enemies> Soldiers;
    //升級數據用
    private List<GameManager.whichObject> soldierKey = new List<GameManager.whichObject>();
    private Enemies tmpUpdateData;

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
        myDataBase = new Dictionary<GameManager.whichObject, Enemies>(myEnumComparer);
        enemyDataBase = new Dictionary<GameManager.whichObject, Enemies>(myEnumComparer);

        for (int i = 0; i < Soldiers.Count; i++)
        {
            soldierKey.Add(Soldiers[i]._soldierName);
            myDataBase.Add(Soldiers[i]._soldierName, Soldiers[i]);
            enemyDataBase.Add(Soldiers[i]._soldierName, Soldiers[i]);
        }
    }

    #region 改變士兵數據
    /*  單體
      //我方
      public void changeMyData(GameManager.whichObject _name, Enemies _data)
      {
          myDataBase[_name] = _data;
      }
      //敵方
      public void changeEnemyData(GameManager.whichObject _name, Enemies _data)
      {
          enemyDataBase[_name] = _data;
      }*/

    //全體功
    public void ChangeMyAtkData(byte _level)
    {
        mySoldierAtkLevel = _level;
        for (int i = 0; i < soldierKey.Count; i++)
        {
            tmpUpdateData = myDataBase[soldierKey[i]];

            switch (_level)
            {
                case (1):
                    if (tmpUpdateData.ATK_Level != _level)
                    {
                        tmpUpdateData.ATK_Level = _level;
                        tmpUpdateData.atk_maxDamage += tmpUpdateData.updateData.Add_atk1;
                        tmpUpdateData.atk_Damage += tmpUpdateData.updateData.Add_atk1;
                    }
                    break;
                case (2):
                    if (tmpUpdateData.ATK_Level != _level)
                    {
                        tmpUpdateData.ATK_Level = _level;
                        tmpUpdateData.atk_maxDamage += tmpUpdateData.updateData.Add_atk2;
                        tmpUpdateData.atk_Damage += tmpUpdateData.updateData.Add_atk2;
                    }
                    break;
                case (3):
                    if (tmpUpdateData.ATK_Level != _level)
                    {
                        tmpUpdateData.ATK_Level = _level;
                        tmpUpdateData.atk_maxDamage += tmpUpdateData.updateData.Add_atk3;
                        tmpUpdateData.atk_Damage += tmpUpdateData.updateData.Add_atk3;
                    }
                    break;
                default:
                    break;
            }
            myDataBase[soldierKey[i]] = tmpUpdateData;
        }
    }
    public void ChangeEnemyAtkData(byte _level)
    {
        enemySoldierAtkLevel = _level;
        for (int i = 0; i < soldierKey.Count; i++)
        {
            tmpUpdateData = enemyDataBase[soldierKey[i]];

            switch (_level)
            {
                case (1):
                    if (tmpUpdateData.ATK_Level != _level)
                    {
                        tmpUpdateData.ATK_Level = _level;
                        tmpUpdateData.atk_maxDamage += tmpUpdateData.updateData.Add_atk1;
                        tmpUpdateData.atk_Damage += tmpUpdateData.updateData.Add_atk1;
                    }
                    break;
                case (2):
                    if (tmpUpdateData.ATK_Level != _level)
                    {
                        tmpUpdateData.ATK_Level = _level;
                        tmpUpdateData.atk_maxDamage += tmpUpdateData.updateData.Add_atk2;
                        tmpUpdateData.atk_Damage += tmpUpdateData.updateData.Add_atk2;
                    }
                    break;
                case (3):
                    if (tmpUpdateData.ATK_Level != _level)
                    {
                        tmpUpdateData.ATK_Level = _level;
                        tmpUpdateData.atk_maxDamage += tmpUpdateData.updateData.Add_atk3;
                        tmpUpdateData.atk_Damage += tmpUpdateData.updateData.Add_atk3;
                    }
                    break;
                default:
                    break;
            }
            enemyDataBase[soldierKey[i]] = tmpUpdateData;
        }
    }

    //全體防
    public void ChangeMyDefData(byte _level)
    {
        mySoldierDefLevel = _level;
        for (int i = 0; i < soldierKey.Count; i++)
        {
            tmpUpdateData = myDataBase[soldierKey[i]];
            switch (_level)
            {
                case (1):
                    if (tmpUpdateData.DEF_Level != _level)
                    {
                        tmpUpdateData.DEF_Level = _level;
                        tmpUpdateData.def_base += tmpUpdateData.updateData.Add_def1;
                        tmpUpdateData.UI_HP += tmpUpdateData.updateData.Add_hp1;
                        tmpUpdateData.UI_MaxHp += tmpUpdateData.updateData.Add_hp1;
                    }
                    break;
                case (2):
                    if (tmpUpdateData.DEF_Level != _level)
                    {
                        tmpUpdateData.DEF_Level = _level;
                        tmpUpdateData.def_base += tmpUpdateData.updateData.Add_def2;
                        tmpUpdateData.UI_HP += tmpUpdateData.updateData.Add_hp2;
                        tmpUpdateData.UI_MaxHp += tmpUpdateData.updateData.Add_hp2;
                    }
                    break;
                case (3):
                    if (tmpUpdateData.DEF_Level != _level)
                    {
                        tmpUpdateData.DEF_Level = _level;
                        tmpUpdateData.def_base += tmpUpdateData.updateData.Add_def3;
                        tmpUpdateData.UI_HP += tmpUpdateData.updateData.Add_hp3;
                        tmpUpdateData.UI_MaxHp += tmpUpdateData.updateData.Add_hp3;
                    }
                    break;
                default:
                    break;
            }
            myDataBase[soldierKey[i]] = tmpUpdateData;
        }
    }
    public void ChangeEnemyDefData(byte _level)
    {
        enemySoldierDefLevel = _level;
        for (int i = 0; i < soldierKey.Count; i++)
        {
            tmpUpdateData = enemyDataBase[soldierKey[i]];
            switch (_level)
            {
                case (1):
                    if (tmpUpdateData.DEF_Level != _level)
                    {
                        tmpUpdateData.DEF_Level = _level;
                        tmpUpdateData.def_base += tmpUpdateData.updateData.Add_def1;
                        tmpUpdateData.UI_HP += tmpUpdateData.updateData.Add_hp1;
                        tmpUpdateData.UI_MaxHp += tmpUpdateData.updateData.Add_hp1;
                    }
                    break;
                case (2):
                    if (tmpUpdateData.DEF_Level != _level)
                    {
                        tmpUpdateData.DEF_Level = _level;
                        tmpUpdateData.def_base += tmpUpdateData.updateData.Add_def2;
                        tmpUpdateData.UI_HP += tmpUpdateData.updateData.Add_hp2;
                        tmpUpdateData.UI_MaxHp += tmpUpdateData.updateData.Add_hp2;
                    }
                    break;
                case (3):
                    if (tmpUpdateData.DEF_Level != _level)
                    {
                        tmpUpdateData.DEF_Level = _level;
                        tmpUpdateData.def_base += tmpUpdateData.updateData.Add_def3;
                        tmpUpdateData.UI_HP += tmpUpdateData.updateData.Add_hp3;
                        tmpUpdateData.UI_MaxHp += tmpUpdateData.updateData.Add_hp3;
                    }
                    break;
                default:
                    break;
            }
            enemyDataBase[soldierKey[i]] = tmpUpdateData;
        }
    }
    #endregion

    #region 取得士兵數據
    //我方
    public Enemies getMySoldierData(GameManager.whichObject _name)
    {
        Enemies tmpData = new Enemies();
        myDataBase.TryGetValue(_name, out tmpData);
        return tmpData;
    }
    //敵方
    public Enemies getEnemySoldierData(GameManager.whichObject _name)
    {
        Enemies tmpData = new Enemies();
        enemyDataBase.TryGetValue(_name, out tmpData);
        return tmpData;
    }
    #endregion
}
