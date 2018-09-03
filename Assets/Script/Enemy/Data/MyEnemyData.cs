using UnityEngine;
using System.Collections.Generic;

public class MyEnemyData : MonoBehaviour
{
    public static MyEnemyData instance;

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
        public int ATK_Level;
        [Header("防禦")]
        public float def_base;
        public int DEF_Level;
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
            myDataBase.Add(Soldiers[i]._soldierName, Soldiers[i]);
            enemyDataBase.Add(Soldiers[i]._soldierName, Soldiers[i]);
        }
    }

    #region 改變士兵數據
    //我方
    public void changeMyData(GameManager.whichObject _name, Enemies _data)
    {
        myDataBase[_name] = _data;
    }
    //敵方
    public void changeEnemyData(GameManager.whichObject _name, Enemies _data)
    {
        enemyDataBase[_name] = _data;
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
