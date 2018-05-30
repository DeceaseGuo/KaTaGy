using UnityEngine;

public class ButtonManager_Tower : MonoBehaviour
{
    private TurretStore turretStore;
    private BuildManager buildManager;
    private UIManager uiManager;
    private Prompt_SelectLocalPos prompt_localPos;

    [Header("建造模式下選單")]
    [SerializeField] GameObject topMenu;
    [SerializeField] GameObject otherTowerMenu;
    [SerializeField] GameObject atkTowerMenu;
    [SerializeField] GameObject researchMenu;

    private void Start()
    {
        turretStore = GameObject.Find("TurretManager").GetComponent<TurretStore>();
        prompt_localPos = GameObject.Find("Prompt_SelectObj").GetComponent<Prompt_SelectLocalPos>();
        uiManager = UIManager.instance;
        buildManager = BuildManager.instance;
    }

    private void Update()
    {
        if (uiManager.getTowerMenu())
        {
            clickButton_Tower();
        }
    }

    #region 快捷鍵
    private void clickButton_Tower()
    {
        #region 主畫面
        if (topMenu.activeInHierarchy && !atkTowerMenu.activeInHierarchy && !otherTowerMenu.activeInHierarchy)
        {
            if (Input.GetKeyDown(KeyCode.Q))
                Key_Q();
            if (Input.GetKeyDown(KeyCode.W))
                Key_W();
            if (Input.GetKeyDown(KeyCode.E))
                Key_E();
        }
        #endregion

        #region 攻擊型
        if (!topMenu.activeInHierarchy && atkTowerMenu.activeInHierarchy)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1))
                clickAction(GameManager.whichObject.Tower1_Cannon);
            if (Input.GetKeyDown(KeyCode.Alpha2))
                clickAction(GameManager.whichObject.Tower2_Wind);
            if (Input.GetKeyDown(KeyCode.Alpha3))
                clickAction(GameManager.whichObject.None);
            if (Input.GetKeyDown(KeyCode.Alpha4))
                clickAction(GameManager.whichObject.None);
            if (Input.GetKeyDown(KeyCode.Alpha5))
                clickAction(GameManager.whichObject.None);
            if (Input.GetKeyDown(KeyCode.Alpha6))
                clickAction(GameManager.whichObject.None);
            if (Input.GetKeyDown(KeyCode.R))
                returnBtn_Atk();
        }
        #endregion

        #region 資源型
        if (!topMenu.activeInHierarchy && otherTowerMenu.activeInHierarchy)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1))
                clickAction(GameManager.whichObject.None); //"採集器"
            if (Input.GetKeyDown(KeyCode.Alpha2))
                clickAction(GameManager.whichObject.None);//"電力塔"
            if (Input.GetKeyDown(KeyCode.R))
                returnBtn_Other();
        }
        #endregion

        #region 研究類
        if (!topMenu.activeInHierarchy && researchMenu.activeInHierarchy)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1))
                clickAction(GameManager.whichObject.None);//"兵站"
            if (Input.GetKeyDown(KeyCode.Alpha2))
                clickAction(GameManager.whichObject.None);//"兵營1"
            if (Input.GetKeyDown(KeyCode.Alpha3))
                clickAction(GameManager.whichObject.None);//"兵營2"
            if (Input.GetKeyDown(KeyCode.Alpha4))
                clickAction(GameManager.whichObject.None);//"兵營3"
            if (Input.GetKeyDown(KeyCode.Alpha5))
                clickAction(GameManager.whichObject.None);//"兵營4"
            if (Input.GetKeyDown(KeyCode.Alpha6))
                clickAction(GameManager.whichObject.None);//"兵營5
            if (Input.GetKeyDown(KeyCode.R))
                returnBtn_reseach();
        }
        #endregion
    }
    #endregion

    #region 按鈕列
    public void Key_Q()
    {
        topMenu.SetActive(false);
        otherTowerMenu.SetActive(true);
        prompt_localPos.ClearPrompt();
    }

    public void Key_W()
    {
        topMenu.SetActive(false);
        atkTowerMenu.SetActive(true);
        prompt_localPos.ClearPrompt();
    }

    public void Key_E()
    {
        topMenu.SetActive(false);
        researchMenu.SetActive(true);
        prompt_localPos.ClearPrompt();
    }

    public void returnBtn_Atk()
    {
        if (buildManager.nowSelect)
        {
            atkTowerMenu.SetActive(false);
            topMenu.SetActive(true);
            cancleSelect();
            prompt_localPos.ClearPrompt();
        }
        else
            HintManager.instance.CreatHint("目前正在前往蓋塔防");
    }

    public void returnBtn_Other()
    {
        if (buildManager.nowSelect)
        {
            otherTowerMenu.SetActive(false);
            topMenu.SetActive(true);
            cancleSelect();
            prompt_localPos.ClearPrompt();
        }
        else
            HintManager.instance.CreatHint("目前正在前往蓋塔防");
    }

    public void returnBtn_reseach()
    {
        if (buildManager.nowSelect)
        {
            researchMenu.SetActive(false);
            topMenu.SetActive(true);
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
