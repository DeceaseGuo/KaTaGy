using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine;
using UnityEngine.UI;

public class CoreSort : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private MyCore coreManager;
    public GameManager.NowTarget whoUpdate;
    public UpdateManager.Myability abilityData;
    public GameManager.whichObject unLockObj;

    public byte myLevel;
    [SerializeField] string myName;
    [SerializeField] string myDescription;
    [Header("升級")]
    public int needMoney;
    public Sprite abilityImg;
    public float time_CD;
    [SerializeField] Image outFrame_Select;
    [SerializeField] Image outFrame_Update;
    public bool nowUpdate = false;
    [Header("結束")]
    public bool over;
    private Button myBtn;
    [SerializeField] Image outFrame_previous;
    [SerializeField] Button NextBtn;

    private void Start()
    {
        myBtn = GetComponent<Button>();
        coreManager = MyCore.instance;
    }

    public void ClickThisBtn()
    {
        if (!nowUpdate)
            coreManager.GetMySelect(this);
    }

    public void Color_select(bool _t)
    {
        outFrame_Select.enabled = _t;
    }

    public void Overto_UnLock()
    {
        over = true;
        myBtn.interactable = false;
        if (NextBtn != null)
            NextBtn.interactable = true;
        if (outFrame_previous != null)
            outFrame_previous.enabled = false;
        outFrame_Update.enabled = true;
    }

    #region 顯示資訊
    // 滑鼠進入範圍
    public void OnPointerEnter(PointerEventData eventData)
    {
        coreManager.Show_info(myName, myDescription, needMoney);
    }

    // 滑鼠離開範圍與點擊時
    public void OnPointerExit(PointerEventData eventData)
    {
        coreManager.Exit_info();
    }
    #endregion
}
