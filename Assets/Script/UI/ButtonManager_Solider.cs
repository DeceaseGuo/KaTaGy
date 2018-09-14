using UnityEngine;

public class ButtonManager_Solider : MonoBehaviour
{
    private SoldierStore soldierStore;
    private UIManager uiManager;
    private Prompt_SelectLocalPos prompt_localPos;

    [Header("建造模式所有選單")]
    private CanvasGroup currentMenu;
    [SerializeField] CanvasGroup topMenu;
    [SerializeField] CanvasGroup soldier_1;
    [SerializeField] CanvasGroup soldier_2;
    [SerializeField] CanvasGroup soldier_3;
    [SerializeField] CanvasGroup soldier_4;
    [SerializeField] CanvasGroup soldier_5;
    [SerializeField] CanvasGroup soldier_6;

    enum NowMenu
    {
        None,
        topMenu,
        soldier_1,
        soldier_2,
        soldier_3,
        soldier_4,
        soldier_5,
        soldier_6
    }
    private NowMenu nowMenu = NowMenu.None;

    private void Start()
    {
        soldierStore = GameObject.Find("EnemyManager").GetComponent<SoldierStore>();
        prompt_localPos = GameObject.Find("Prompt_SelectObj").GetComponent<Prompt_SelectLocalPos>();
        uiManager = UIManager.instance;
    }

    private void Update()
    {
        if (!uiManager.IsTowerMenu && nowMenu != NowMenu.None)
        {
            clickButton_Solider();
        }
    }

    #region 開關
    public void switchTowerMenu(bool _t)
    {
        if (uiManager == null)
            uiManager = UIManager.instance;

        uiManager.MenuClose(currentMenu);
        uiManager.MenuOpen(topMenu);

        if (_t)
            nowMenu = NowMenu.topMenu;
        else
            nowMenu = NowMenu.None;
    }
    #endregion

    #region 快捷鍵
    private void clickButton_Solider()
    {
        switch (nowMenu)
        {
            case NowMenu.topMenu:
                if (Input.GetKeyDown(KeyCode.Alpha1))
                    Key_Atk_S1();
                if (Input.GetKeyDown(KeyCode.Alpha2))
                    Key_Atk_S2();
                if (Input.GetKeyDown(KeyCode.Alpha3))
                    Key_Atk_S3();
                if (Input.GetKeyDown(KeyCode.Alpha4))
                    Key_Atk_S4();
                if (Input.GetKeyDown(KeyCode.Alpha5))
                    Key_Atk_S5();
                if (Input.GetKeyDown(KeyCode.Alpha6))
                    Key_Atk_S6();
                break;
            case NowMenu.soldier_1:
                if (Input.GetKeyDown(KeyCode.Alpha1))
                    clickSolider(GameManager.whichObject.soldier_Test);
                if (Input.GetKeyDown(KeyCode.Alpha2))
                    clickSolider(GameManager.whichObject.None);
                if (Input.GetKeyDown(KeyCode.Alpha3))
                    clickSolider(GameManager.whichObject.None);
                if (Input.GetKeyDown(KeyCode.R))
                    returnBtn();
                break;
            case NowMenu.soldier_2:
                if (Input.GetKeyDown(KeyCode.Alpha1))
                    clickSolider(GameManager.whichObject.None);
                if (Input.GetKeyDown(KeyCode.Alpha2))
                    clickSolider(GameManager.whichObject.None);
                if (Input.GetKeyDown(KeyCode.Alpha3))
                    clickSolider(GameManager.whichObject.None);
                if (Input.GetKeyDown(KeyCode.R))
                    returnBtn();
                break;
            case NowMenu.soldier_3:
                if (Input.GetKeyDown(KeyCode.Alpha1))
                    clickSolider(GameManager.whichObject.None);
                if (Input.GetKeyDown(KeyCode.Alpha2))
                    clickSolider(GameManager.whichObject.None);
                if (Input.GetKeyDown(KeyCode.Alpha3))
                    clickSolider(GameManager.whichObject.None);
                if (Input.GetKeyDown(KeyCode.R))
                    returnBtn();
                break;
            case NowMenu.soldier_4:
                if (Input.GetKeyDown(KeyCode.Alpha1))
                    clickSolider(GameManager.whichObject.None);
                if (Input.GetKeyDown(KeyCode.Alpha2))
                    clickSolider(GameManager.whichObject.None);
                if (Input.GetKeyDown(KeyCode.Alpha3))
                    clickSolider(GameManager.whichObject.None);
                if (Input.GetKeyDown(KeyCode.R))
                    returnBtn();
                break;
            case NowMenu.soldier_5:
                if (Input.GetKeyDown(KeyCode.Alpha1))
                    clickSolider(GameManager.whichObject.None);
                if (Input.GetKeyDown(KeyCode.Alpha2))
                    clickSolider(GameManager.whichObject.None);
                if (Input.GetKeyDown(KeyCode.Alpha3))
                    clickSolider(GameManager.whichObject.None);
                if (Input.GetKeyDown(KeyCode.R))
                    returnBtn();
                break;
            case NowMenu.soldier_6:
                if (Input.GetKeyDown(KeyCode.Alpha1))
                    clickSolider(GameManager.whichObject.Soldier_2_Siege);
                if (Input.GetKeyDown(KeyCode.Alpha2))
                    clickSolider(GameManager.whichObject.None);
                if (Input.GetKeyDown(KeyCode.Alpha3))
                    clickSolider(GameManager.whichObject.None);
                if (Input.GetKeyDown(KeyCode.R))
                    returnBtn();
                break;
        }
    }
    #endregion
    #region 按鈕列
    public void Key_Atk_S1()
    {
        nowMenu = NowMenu.soldier_1;
        uiManager.MenuClose(topMenu);
        uiManager.MenuOpen(soldier_1);
        currentMenu = soldier_1;
        prompt_localPos.ClearPrompt();
    }

    public void Key_Atk_S2()
    {
        nowMenu = NowMenu.soldier_2;
        uiManager.MenuClose(topMenu);
        uiManager.MenuOpen(soldier_2);
        currentMenu = soldier_2;
        prompt_localPos.ClearPrompt();
    }

    public void Key_Atk_S3()
    {
        nowMenu = NowMenu.soldier_3;
        uiManager.MenuClose(topMenu);
        uiManager.MenuOpen(soldier_3);
        currentMenu = soldier_3;
        prompt_localPos.ClearPrompt();
    }

    public void Key_Atk_S4()
    {
        nowMenu = NowMenu.soldier_4;
        uiManager.MenuClose(topMenu);
        uiManager.MenuOpen(soldier_4);
        currentMenu = soldier_4;
        prompt_localPos.ClearPrompt();
    }

    public void Key_Atk_S5()
    {
        nowMenu = NowMenu.soldier_5;
        uiManager.MenuClose(topMenu);
        uiManager.MenuOpen(soldier_5);
        currentMenu = soldier_5;
        prompt_localPos.ClearPrompt();
    }

    public void Key_Atk_S6()
    {
        nowMenu = NowMenu.soldier_6;
        uiManager.MenuClose(topMenu);
        uiManager.MenuOpen(soldier_6);
        currentMenu = soldier_6;
        prompt_localPos.ClearPrompt();
    }

    public void returnBtn()
    {        
        if (nowMenu != NowMenu.topMenu)
        {
            switchTowerMenu(true);
            prompt_localPos.ClearPrompt();
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
