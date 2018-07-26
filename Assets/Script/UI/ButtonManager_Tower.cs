using UnityEngine;

public class ButtonManager_Tower : MonoBehaviour
{
    private TurretStore turretStore;
    private BuildManager buildManager;
    private UIManager uiManager;
    private Prompt_SelectLocalPos prompt_localPos;

    [Header("建造模式所有選單")]
    private CanvasGroup currentMenu;
    [SerializeField] CanvasGroup topMenu;
    [SerializeField] CanvasGroup towerAtk_1_Cannon;
    [SerializeField] CanvasGroup towerAtk_2_Wind;
    [SerializeField] CanvasGroup towerAtk_3;
    [SerializeField] CanvasGroup towerAtk_4;
    [SerializeField] CanvasGroup towerAtk_5;
    [SerializeField] CanvasGroup towerAtk_6;

    enum NowMenu
    {
        None,
        topMenu,
        towerAtk_1_Cannon,
        towerAtk_2_Wind,
        towerAtk_3,
        towerAtk_4,
        towerAtk_5,
        towerAtk_6
    }
    private NowMenu nowMenu = NowMenu.None;

    private void Start()
    {
        turretStore = GameObject.Find("TurretManager").GetComponent<TurretStore>();
        prompt_localPos = GameObject.Find("Prompt_SelectObj").GetComponent<Prompt_SelectLocalPos>();
        uiManager = UIManager.instance;
        buildManager = BuildManager.instance;
    }

    private void Update()
    {
        if (uiManager.IsTowerMenu && buildManager.nowSelect && nowMenu != NowMenu.None)
        {
            clickButton_Tower();
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
    private void clickButton_Tower()
    {
        switch (nowMenu)
        {
            //主畫面
            case NowMenu.topMenu:
                if (Input.GetKeyDown(KeyCode.Alpha1))
                {
                    Debug.Log("電力塔");
                    clickAction(GameManager.whichObject.Tower_Electricity);
                }
                if (Input.GetKeyDown(KeyCode.Alpha2))
                    Key_Atk_T1();
                if (Input.GetKeyDown(KeyCode.Alpha3))
                    Key_Atk_T2();
                if (Input.GetKeyDown(KeyCode.Alpha4))
                    Key_Atk_T3();
                if (Input.GetKeyDown(KeyCode.Alpha5))
                    Key_Atk_T4();
                if (Input.GetKeyDown(KeyCode.Alpha6))
                    Key_Atk_T5();
                if (Input.GetKeyDown(KeyCode.Alpha7))
                    Key_Atk_T6();
                break;
            //攻擊塔1
            case NowMenu.towerAtk_1_Cannon:
                if (Input.GetKeyDown(KeyCode.Alpha1))
                    clickAction(GameManager.whichObject.Tower1_Cannon);
                if (Input.GetKeyDown(KeyCode.Alpha2))
                    Debug.Log("塔1-2");
                //clickAction(GameManager.whichObject.Tower2_Wind);
                if (Input.GetKeyDown(KeyCode.Alpha3))
                    Debug.Log("塔1-3");
                // clickAction(GameManager.whichObject.None);
                if (Input.GetKeyDown(KeyCode.R))
                    returnBtn_Atk();
                break;
            //攻擊塔2
            case NowMenu.towerAtk_2_Wind:
                if (Input.GetKeyDown(KeyCode.Alpha1))
                    clickAction(GameManager.whichObject.Tower2_Wind);
                if (Input.GetKeyDown(KeyCode.Alpha2))
                    Debug.Log("塔2-2");
                //clickAction(GameManager.whichObject.Tower2_Wind);
                if (Input.GetKeyDown(KeyCode.Alpha3))
                    Debug.Log("塔2-3");
                // clickAction(GameManager.whichObject.None);
                if (Input.GetKeyDown(KeyCode.R))
                    returnBtn_Atk();
                break;
            //攻擊塔3
            case NowMenu.towerAtk_3:
                if (currentMenu == towerAtk_3)
                {
                    if (Input.GetKeyDown(KeyCode.Alpha1))
                        clickAction(GameManager.whichObject.None);
                    if (Input.GetKeyDown(KeyCode.Alpha2))
                        Debug.Log("塔3-2");
                    //clickAction(GameManager.whichObject.Tower2_Wind);
                    if (Input.GetKeyDown(KeyCode.Alpha3))
                        Debug.Log("塔3-3");
                    // clickAction(GameManager.whichObject.None);
                    if (Input.GetKeyDown(KeyCode.R))
                        returnBtn_Atk();
                }
                break;
            //攻擊塔4
            case NowMenu.towerAtk_4:
                if (currentMenu == towerAtk_4)
                {
                    if (Input.GetKeyDown(KeyCode.Alpha1))
                        clickAction(GameManager.whichObject.None);
                    if (Input.GetKeyDown(KeyCode.Alpha2))
                        Debug.Log("塔4-2");
                    //clickAction(GameManager.whichObject.Tower2_Wind);
                    if (Input.GetKeyDown(KeyCode.Alpha3))
                        Debug.Log("塔4-3");
                    // clickAction(GameManager.whichObject.None);
                    if (Input.GetKeyDown(KeyCode.R))
                        returnBtn_Atk();
                }
                break;
            //攻擊塔5
            case NowMenu.towerAtk_5:
                if (currentMenu == towerAtk_5)
                {
                    if (Input.GetKeyDown(KeyCode.Alpha1))
                        clickAction(GameManager.whichObject.None);
                    if (Input.GetKeyDown(KeyCode.Alpha2))
                        Debug.Log("塔5-2");
                    //clickAction(GameManager.whichObject.Tower2_Wind);
                    if (Input.GetKeyDown(KeyCode.Alpha3))
                        Debug.Log("塔5-3");
                    // clickAction(GameManager.whichObject.None);
                    if (Input.GetKeyDown(KeyCode.R))
                        returnBtn_Atk();
                }
                break;
            //攻擊塔6
            case NowMenu.towerAtk_6:
                if (currentMenu == towerAtk_6)
                {
                    if (Input.GetKeyDown(KeyCode.Alpha1))
                        clickAction(GameManager.whichObject.None);
                    if (Input.GetKeyDown(KeyCode.Alpha2))
                        Debug.Log("塔6-2");
                    //clickAction(GameManager.whichObject.Tower2_Wind);
                    if (Input.GetKeyDown(KeyCode.Alpha3))
                        Debug.Log("塔6-3");
                    // clickAction(GameManager.whichObject.None);
                    if (Input.GetKeyDown(KeyCode.R))
                        returnBtn_Atk();
                }
                break;
        }
    }
    #endregion

    #region 按鈕列
    public void Key_Atk_T1()
    {
        nowMenu = NowMenu.towerAtk_1_Cannon;
        uiManager.MenuClose(topMenu);
        uiManager.MenuOpen(towerAtk_1_Cannon);
        currentMenu = towerAtk_1_Cannon;
        prompt_localPos.ClearPrompt();
    }

    public void Key_Atk_T2()
    {
        nowMenu = NowMenu.towerAtk_2_Wind;
        uiManager.MenuClose(topMenu);
        uiManager.MenuOpen(towerAtk_2_Wind);
        currentMenu = towerAtk_2_Wind;
        prompt_localPos.ClearPrompt();
    }

    public void Key_Atk_T3()
    {
        nowMenu = NowMenu.towerAtk_3;
        uiManager.MenuClose(topMenu);
        uiManager.MenuOpen(towerAtk_3);
        currentMenu = towerAtk_3;
        prompt_localPos.ClearPrompt();
    }

    public void Key_Atk_T4()
    {
        nowMenu = NowMenu.towerAtk_4;
        uiManager.MenuClose(topMenu);
        uiManager.MenuOpen(towerAtk_4);
        currentMenu = towerAtk_4;
        prompt_localPos.ClearPrompt();
    }

    public void Key_Atk_T5()
    {
        nowMenu = NowMenu.towerAtk_5;
        uiManager.MenuClose(topMenu);
        uiManager.MenuOpen(towerAtk_5);
        currentMenu = towerAtk_5;
        prompt_localPos.ClearPrompt();
    }

    public void Key_Atk_T6()
    {
        nowMenu = NowMenu.towerAtk_6;
        uiManager.MenuClose(topMenu);
        uiManager.MenuOpen(towerAtk_6);
        currentMenu = towerAtk_6;
        prompt_localPos.ClearPrompt();
    }

    public void returnBtn_Atk()
    {
        nowMenu = NowMenu.topMenu;
        if (buildManager.nowSelect)
        {
            switchTowerMenu(true);
            cancleSelect();
            prompt_localPos.ClearPrompt();
        }
        else
            HintManager.instance.CreatHint("目前正在前往蓋塔防");
    }
    #endregion

    #region 取消選擇的塔防
    public void cancleSelect()
    {
        buildManager.cancelSelect();
    }
    #endregion

    #region 點擊執行
    public void clickAction(GameManager.whichObject _name)
    {
        prompt_localPos.ClearPrompt();
        turretStore.SelectNowTurret(_name);
    }

    public void clickAction(PromptScreen _name)
    {
        prompt_localPos.ClearPrompt();
        turretStore.SelectNowTurret(_name.DataName);
    }
    #endregion
}
