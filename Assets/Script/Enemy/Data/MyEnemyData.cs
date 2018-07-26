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
        public float def_maxDEF;
        public float def_base;
        public int DEF_Level;
        [Header("生產所需")]
        public int population_need;
        public float soldier_CountDown;
        public GameManager.whichObject UI_Name;
        [Header("花費")]
        public int cost_Ore;
        public int cost_Money;
    }
    [SerializeField]
    public Dictionary<GameManager.whichObject, Enemies> DataBase = new Dictionary<GameManager.whichObject, Enemies>();
    public List<Enemies> Soldiers;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        addToDictionary();
    }

    private void Update()
    {
        if (Input.GetKeyDown("z"))
        {
            Enemies aaa= getEnemyData(GameManager.whichObject.Soldier_1);
            aaa.atk_Damage += 5;
            changeData(GameManager.whichObject.Soldier_1, aaa);         
        }
    }

    void addToDictionary()
    {
        for (int i = 0; i < Soldiers.Count; i++)
        {
            DataBase.Add(Soldiers[i]._soldierName, Soldiers[i]);
        }
    }

    public void changeData(GameManager.whichObject _name, Enemies _data)
    {
        DataBase[_name] = _data;
    }  

    public Enemies getEnemyData(GameManager.whichObject _name)
    {
        Enemies tmpData = new Enemies();
        DataBase.TryGetValue(_name, out tmpData);
        return tmpData;
    }
}
