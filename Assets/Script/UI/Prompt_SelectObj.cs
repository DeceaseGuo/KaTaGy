using UnityEngine;

public class Prompt_SelectObj : MonoBehaviour
{
    private Prompt_SelectLocalPos prompt_localPos;
    public bool isSelect;
    //[Header("使用面板")]
    //[SerializeField] int openMenuNumber;
    [Header("腳本")]
    [SerializeField]
    EnemyControl soldier;
    [SerializeField] isDead deadManager;
    [Header("數據")]
    [SerializeField]
    GameManager.whichObject DataName;
    //private PromptData DataBase;
    // private PromptData.PromptDataBase Data;
    public Prompt_SelectLocalPos.whois Iam;
    private MyEnemyData.Enemies SoldierData;
    private TurretData.TowerDataBase TowerData;
    private PlayerData.PlayerDataBase Player_Data;


    private void Awake()
    {
        prompt_localPos = GameObject.Find("Prompt_SelectObj").GetComponent<Prompt_SelectLocalPos>();
    }

    private void Start()
    {
        getData();
    }

    private void Update()
    {
        if (isSelect)
        {
            //  ObjectDeath();
            // prompt_localPos.setClickObj(enemy.enemyData.UI_MaxHp, enemy.enemyData.UI_HP, 0, 0);
        }
    }

    void getData()
    {
        switch (Iam)
        {
            case Prompt_SelectLocalPos.whois.Player:
                Player_Data = PlayerData.instance.getPlayerData(GameManager.instance.Meis);
                return;
            case Prompt_SelectLocalPos.whois.Soldier:
               // SoldierData = MyEnemyData.instance.getEnemyData(DataName);
                return;
            case Prompt_SelectLocalPos.whois.Tower:
                TowerData = TurretData.instance.getTowerData(DataName);
                return;
            case Prompt_SelectLocalPos.whois.Building:
                return;
            case Prompt_SelectLocalPos.whois.Core:
                return;
            default:
                break;
        }
    }

    #region 被點擊到時
    private void OnMouseDown()
    {
        prompt_localPos.ClearPrompt();
        isSelect = true;

        #region 傳送數據

        switch (Iam)
        {
            case (Prompt_SelectLocalPos.whois.Soldier):
                if (soldier == null)
                    return;
                prompt_localPos.setMoInBtMenu(SoldierData.headImage, SoldierData.firstAtk, SoldierData.objectName);
                prompt_localPos.setClickObj(soldier.enemyData.UI_MaxHp, soldier.enemyData.UI_HP, 0, 0);
                prompt_localPos.setClickObj(soldier.enemyData.ATK_Level, soldier.enemyData.DEF_Level);
                return;
            case (Prompt_SelectLocalPos.whois.Tower):
                prompt_localPos.setMoInBtMenu(TowerData.headImage, null, TowerData.objectName);
                prompt_localPos.setClickObj(TowerData.UI_maxHp, TowerData.UI_Hp, TowerData.Fad_maxThermalEnergy, TowerData.Fad_thermalEnergy);
                prompt_localPos.setClickObj(TowerData.ATK_Level, TowerData.DEF_Level);
                return;
            case (Prompt_SelectLocalPos.whois.Player):
                prompt_localPos.setMoInBtMenu(Player_Data.headImage, null, Player_Data.objectName);
                prompt_localPos.setClickObj(Player_Data.Hp_Max, Player_Data.Hp_original, Player_Data.Ap_Max, Player_Data.Ap_original);
                prompt_localPos.setClickObj(Player_Data.ATK_Level, Player_Data.DEF_Level);
                return;
        }
        #endregion

        prompt_localPos.openMenu(Prompt_SelectLocalPos.allMenu.Click_Obj);
    }
    #endregion

    #region 目標死亡時
    void ObjectDeath()
    {
        if (deadManager.checkDead)
        {
            prompt_localPos.ClearPrompt();
        }
    }
    #endregion
}