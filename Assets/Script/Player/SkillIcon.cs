using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkillIcon : MonoBehaviour
{
    public static SkillIcon instance;

    [SerializeField] Transform hintArea;
    [SerializeField] Transform hideArea;

    [SerializeField] GameObject myStatesPrefab;
    [System.Serializable]
    public class MyStates
    {
        public bool isUse;
        public int listNum;
        public GameObject statePrefab;
        public Image stateImg;
        public Image cdBar; //1
        [Tooltip("右上數字")]
        public Text nowAmount;  //2
    }

    [System.Serializable]
    public struct SkillContainer
    {
        public Image skillImg;
        public Image cdBar;
        public Text nowTime;
        public Text nowLevel;
    }

    public List<SkillContainer> skillContainer;
    public List<MyStates> myStatesCT;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    private void Start()
    {
        SetStatesCT();
    }

    #region 清除技能顯示的CD
    public void ClearSkillCD(int _i)
    {
        skillContainer[_i].nowTime.text = "";
        skillContainer[_i].cdBar.fillAmount = 0;
    }
    #endregion

    #region 左上顯示狀態列
    //初始容器
    void SetStatesCT()
    {
        for (int i = 0; i < 8; i++)
        {
            AddNewStateCT();
        }
    }
    //新增一個容器
    void AddNewStateCT()
    {
        GameObject container = Instantiate(myStatesPrefab);
        GoHideArea(container);
        MyStates newStateCT = new MyStates();
        newStateCT.stateImg = container.GetComponent<Image>();
        newStateCT.cdBar = container.transform.GetChild(1).GetComponent<Image>();
        newStateCT.nowAmount = container.transform.GetChild(2).GetComponent<Text>();
        newStateCT.statePrefab = container;
        newStateCT.listNum = myStatesCT.Count;
        myStatesCT.Add(newStateCT);
    }
    //外部取得一個空的容器
    public MyStates GetNewStateCT()
    {
        return myStatesCT[GetNullCT()];
    }

    //取得一個空的容器陣列號
    public int GetNullCT()
    {
        int num = myStatesCT.FindIndex(x => x.isUse == false);
        if (num != -1)
        {
            myStatesCT[num].isUse = true;
            return num;
        }
        else
        {
            AddNewStateCT();
            myStatesCT[myStatesCT.Count - 1].isUse = true;
            return myStatesCT.Count - 1;
        }
    }

    public void ClearThisCT(int _num)
    {
        GoHideArea(myStatesCT[_num].statePrefab);
        myStatesCT[_num].isUse = false;
        myStatesCT[_num].stateImg.sprite = null;
        myStatesCT[_num].cdBar.fillAmount = 0;
        myStatesCT[_num].nowAmount.text = "";
    }
    #endregion

    public void SetSkillIcon(List<Sprite> _iconList)
    {
        for (int i = 0; i < skillContainer.Count; i++)
        {
            if (_iconList[i] != null)
                skillContainer[i].skillImg.sprite = _iconList[i];
        }
    }

    public void GoHintArea(GameObject _icon)
    {
        _icon.transform.SetParent(hintArea);
    }

    public void GoHideArea(GameObject _icon)
    {
        _icon.transform.SetParent(hideArea);
    }
}