using UnityEngine;

public class ButtonManager_Solider : MonoBehaviour
{
    private SoldierStore soldierStore;
    private UIManager uiManager;
    private Prompt_SelectLocalPos prompt_localPos;

    [Header("攻擊模式")]
    [SerializeField] GameObject soliderMenu;

    private void Start()
    {
        soldierStore = GameObject.Find("EnemyManager").GetComponent<SoldierStore>();
        prompt_localPos = GameObject.Find("Prompt_SelectObj").GetComponent<Prompt_SelectLocalPos>();
        uiManager = UIManager.instance;
    }

    private void Update()
    {
        if (!uiManager.getTowerMenu())
        {
            clickButton_Solider();
        }
    }

    #region 快捷鍵
    private void clickButton_Solider()
    {
        if (soliderMenu.activeInHierarchy)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1))
                clickSolider(GameManager.whichObject.Soldier_1);
            if (Input.GetKeyDown(KeyCode.Alpha2))
                clickSolider(GameManager.whichObject.Soldier_2);
            if (Input.GetKeyDown(KeyCode.Alpha3))
                clickSolider(GameManager.whichObject.None);
            if (Input.GetKeyDown(KeyCode.Alpha4))
                clickSolider(GameManager.whichObject.None);
            if (Input.GetKeyDown(KeyCode.Alpha5))
                clickSolider(GameManager.whichObject.None);
            if (Input.GetKeyDown(KeyCode.Alpha6))
                clickSolider(GameManager.whichObject.None);
        }
    }
    #endregion

    #region 點擊執行
    public void clickSolider(GameManager.whichObject _name)
    {
        prompt_localPos.ClearPrompt();
        soldierStore.SelectSoldier(_name);
    }

    public void clickSolider(PromptScreen _name)
    {
        prompt_localPos.ClearPrompt();
        soldierStore.SelectSoldier(_name.DataName);
    }
    #endregion
}
