using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    private Transform enemyBtnPos;
    private Transform towerBtPos;
    public List<Transform> tmpObj;
    private bool isTowerMenu;

    private Prompt_SelectLocalPos prompt_localPos;

    #region 單例
    public static UIManager instance;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }

        enemyBtnPos = GameObject.Find("Monster_BtnPos").transform;
        towerBtPos = GameObject.Find("Tower_BtnPos").transform;
        prompt_localPos = GameObject.Find("Prompt_SelectObj").GetComponent<Prompt_SelectLocalPos>();
    }
    #endregion

    private void Start()
    {
        produceButton();
        getTowerList();
        CloseTowerMenu();
    }

    //確認目前玩家為誰
    //生成對應按鈕 (怪物與塔防)

    #region  生成按鈕區
    void produceButton()
    {
        RectTransform _enemyBtnObj = Instantiate(Resources.Load("Prefabs/UI/Monster_Btn", typeof(RectTransform))) as RectTransform;
        RectTransform _towerBtnObj = Instantiate(Resources.Load("Prefabs/UI/Tower_Btn", typeof(RectTransform))) as RectTransform;

        _enemyBtnObj.transform.SetParent(enemyBtnPos);
        _towerBtnObj.transform.SetParent(towerBtPos);
        resetPos(_enemyBtnObj);
        resetPos(_towerBtnObj);
    }
    #endregion

    #region 生成歸零
    void resetPos(RectTransform _pos)
    {
        _pos.offsetMax = new Vector2(0, 0);
        _pos.offsetMin = new Vector2(0, 0);
        _pos.sizeDelta = new Vector2(0, 0);

        _pos.localScale = new Vector3(1, 1, 1);
    }
    #endregion

    #region 蓋塔區陣列
    void getTowerList()
    {
        Transform tmp = towerBtPos.GetChild(0);
        tmpObj.Add(tmp.GetChild(0));
        tmpObj.Add(tmp.GetChild(1));
        tmpObj.Add(tmp.GetChild(2));
        tmpObj.Add(tmp.GetChild(3));
    }
    #endregion

    #region 切換塔防與生怪畫面
    public void OpenTowerMenu()
    {
        isTowerMenu = true;
        prompt_localPos.ClearPrompt();
        towerBtPos.gameObject.SetActive(true);
        enemyBtnPos.gameObject.SetActive(false);        
    }

    public void CloseTowerMenu()
    {
        isTowerMenu = false;
        prompt_localPos.ClearPrompt();
        foreach (var item in tmpObj)
        {
            if (item.name == "Tower_TopMenu")
                item.gameObject.SetActive(true);
            else
                item.gameObject.SetActive(false);
        }
        towerBtPos.gameObject.SetActive(false);
        enemyBtnPos.gameObject.SetActive(true);
    }
    #endregion

    public bool getTowerMenu()
    {
        return isTowerMenu;
    }
}   
