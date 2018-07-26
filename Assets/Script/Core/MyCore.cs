using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MyCore : MonoBehaviour
{
    public static MyCore instance;
    private HintManager hintManager;
    private MatchTimer matchTime;
    public MatchTimer MatchTimeManager { get { if (matchTime == null) matchTime = MatchTimer.Instance; return matchTime; } }

    private bool coreOpen = false;
    public bool CoreOpen { get { return coreOpen; } private set { coreOpen = value; } }



    [SerializeField] CanvasGroup coreObj;
    [SerializeField] CanvasGroup player_Group;
    private CanvasGroup nowThis_Group;

    [SerializeField] Scrollbar scrollbar;

    //顯示選單
    [SerializeField] Text nameText;
    [SerializeField] Text descriptionText;
    [SerializeField] Text needMoneyText;

    //升級
    [SerializeField] CoreSort selectSort;
    [SerializeField] Transform hidePos;
    [SerializeField] Transform updateArrayPos;
    [SerializeField] List<WaitPosition> update_canUse;

    //  public delegate void playerUpdate(Myability _whatIs, int level);
    //  public static event playerUpdate P_Update;

    private UpdateManager.Myability myState = UpdateManager.Myability.None;
    private void Awake()
    {
        if (instance == null)
            instance = this;
    }

    private void Start()
    {
        hintManager = HintManager.instance;
    }

    #region 開關主選單
    public void OpenCoreMenu()
    {
        if (!CoreOpen)
        {
            coreObj.alpha = 1;
            MatchTimeManager.FirstOpen();
            SelectThis(player_Group);
            coreObj.interactable = true;
            coreObj.blocksRaycasts = true;
            CoreOpen = true;
        }
        else
        {
            coreObj.alpha = 0;
            MatchTimeManager.TotalTimeShow = false;
            coreObj.interactable = false;
            coreObj.blocksRaycasts = false;
            CoreOpen = false;
            if (selectSort != null)
            {
                selectSort.Color_select(false);
                selectSort = null;
            }
            myState = UpdateManager.Myability.None;
        }
    }

    private void CloseThisGroup()
    {
        if (nowThis_Group != null)
        {
            nowThis_Group.alpha = 0;
            nowThis_Group.interactable = false;
            nowThis_Group.blocksRaycasts = false;
        }
    }

    public void SelectThis(CanvasGroup _group)
    {
        CloseThisGroup();
        scrollbar.value = 0;
        _group.alpha = 1;
        _group.interactable = true;
        _group.blocksRaycasts = true;
        nowThis_Group = _group;
    }
    #endregion

    void CloseColor()
    {
        selectSort.Color_select(false);
    }

    public void GetMySelect(CoreSort _sort)
    {
        if (selectSort != null)
            selectSort.Color_select(false);
        selectSort = _sort;
        selectSort.Color_select(true);
        myState = _sort.abilityData;
    }

    public void CorrectMySelect()
    {
        if (myState != UpdateManager.Myability.None)
        {
            if (!selectSort.nowUpdate)
                GoWaitToUpdate();
            else
                hintManager.CreatHint("正在升級中");
        }
        else
            hintManager.CreatHint("選擇一個能力進行升級");
    }

    void GoWaitToUpdate()
    {
        Debug.Log("可使用目前數量" + update_canUse.Count);
        if (update_canUse.Count != 0)
        {
            update_canUse[0].SetData(selectSort);
            selectSort.nowUpdate = true;
            update_canUse[0].transform.SetParent(updateArrayPos);
            update_canUse.RemoveAt(0);
            selectSort.Color_select(false);
            selectSort = null;
            myState = UpdateManager.Myability.None;

        }
        else
            hintManager.CreatHint("沒有更多的空間升級");
    }

    public void ReturnCanUse(WaitPosition _pos)
    {
        _pos.transform.SetParent(hidePos);
        update_canUse.Add(_pos);
    }

    #region 顯示資訊
    public void Show_info(string _name, string _description, int _money)
    {
        if (_name != null)
            nameText.text = _name;
        if (_description != null)
            descriptionText.text = _description;
        needMoneyText.text = _money.ToString();
    }

    public void Exit_info()
    {
        nameText.text = "";
        descriptionText.text = "";
        needMoneyText.text = 0.ToString();
    }
    #endregion
}
