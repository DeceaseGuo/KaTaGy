using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class UIManager : MonoBehaviour
{
    [SerializeField] ButtonManager_Solider soldierBtnPos;
    [SerializeField] ButtonManager_Tower towerBtPos;
    private CanvasGroup soldier_CG;
    private CanvasGroup tower_CG;

    private bool isTowerMenu;
    public bool IsTowerMenu { get { return isTowerMenu; } private set { isTowerMenu = value; } }
    //倉庫區
    [SerializeField] GameObject warehouseObj;
    private bool isOpen = false;
    private Tweener myTweener;

    private Prompt_SelectLocalPos prompt_localPos;
    [SerializeField] Text populationText;
    private ArraySoldier arraySoldier;

    #region 單例
    public static UIManager instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }

        soldier_CG = soldierBtnPos.GetComponent<CanvasGroup>();
        tower_CG = towerBtPos.GetComponent<CanvasGroup>();
        prompt_localPos = GameObject.Find("Prompt_SelectObj").GetComponent<Prompt_SelectLocalPos>();
        arraySoldier = GetComponent<ArraySoldier>();
    }
    #endregion

    private void Start()
    {
        CloseTowerMenu();
        ReSetTween();
    }

    #region 初始化
    //倉庫彈出
    void ReSetTween()
    {
        myTweener = warehouseObj.transform.DOLocalMoveX(739.5f, .2f).SetEase(Ease.OutBack);
        myTweener.SetAutoKill(false);
        myTweener.Pause();
    }
    #endregion

    #region 切換塔防與生怪畫面
    public void OpenTowerMenu()
    {
        IsTowerMenu = true;
        prompt_localPos.ClearPrompt();

        MenuOpen(tower_CG);
        MenuClose(soldier_CG);

        towerBtPos.switchTowerMenu(true);
        soldierBtnPos.switchTowerMenu(false);

    }

    public void CloseTowerMenu()
    {
        IsTowerMenu = false;
        prompt_localPos.ClearPrompt();

        MenuOpen(soldier_CG);
        MenuClose(tower_CG);

        soldierBtnPos.switchTowerMenu(true);
        towerBtPos.switchTowerMenu(false);
    }
    #endregion

    #region 開啟關閉canvasGroup
    public void MenuOpen(CanvasGroup _CG)
    {
        if (_CG != null)
        {
            _CG.alpha = 1;
            _CG.blocksRaycasts = true;
            _CG.interactable = true;
        }
    }

    public void MenuClose(CanvasGroup _CG)
    {
        if (_CG != null)
        {
            _CG.alpha = 0;
            _CG.blocksRaycasts = false;
            _CG.interactable = false;
        }
    }
    #endregion

    #region 開啟倉庫與交換區按鈕
    private void click_Warehouse()
    {
        //快捷鍵
        //if (Input.GetKeyDown(KeyCode.RightShift))
    }

    public void switch_Warehouse()
    {
        if (!isOpen)
        {
            isOpen = true;
            myTweener.Play();
            myTweener.PlayForward();
            arraySoldier.MenuOpen();
        }
        else
        {
            isOpen = false;
            myTweener.PlayBackwards();
            Info_Exit();
            arraySoldier.MenuClose();
        }
    }
    #endregion

    #region 顯示倉庫怪物資訊
    public void Info_MouseIn(MyEnemyData.Enemies _data , int _population)
    {
        prompt_localPos.ClearPrompt();

        prompt_localPos.setMoInBtMenu(_data.headImage, _data.firstAtk, _data.objectName);
        prompt_localPos.setMoInBtMenu_Need(_data.cost_Ore, _data.cost_Money, 0, _data.soldier_CountDown);
        prompt_localPos.setMoInBtMenu_Bar(_data.atk_Damage, _data.atk_delay, _data.def_base, _data.moveSpeed);

        prompt_localPos.openMenu(Prompt_SelectLocalPos.allMenu.MoinB_atk);

        if (populationText != null)
            populationText.text = _population.ToString();
    }

    public void Info_Exit()
    {
        prompt_localPos.ClearPrompt();
        populationText.text = 0.ToString();
    }
    #endregion
}   
