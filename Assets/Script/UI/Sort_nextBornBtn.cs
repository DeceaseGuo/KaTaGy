using UnityEngine.EventSystems;
using UnityEngine;
using UnityEngine.UI;

public class Sort_nextBornBtn : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler
{
    public int nowPopulation = 0;
    public int order;
    public bool isChose = false;
    public Sort_Soldier Data;
    private UIManager uiManager;

    [Header("ICON")]
    [SerializeField] Image icon_Pos;
    [SerializeField] Image showBornImage;
    [SerializeField] Image frame_Pos;
    [SerializeField] Image Lock_Pos;
    [SerializeField] Sprite originalImg;
    
    private void Start()
    {
        uiManager = UIManager.instance;
    }

    public void OpenSelectImage(bool _t)
    {
        frame_Pos.enabled = _t;
    }

    //改變此位置
    public void changeSoldier(Sort_Soldier _data)
    {
        isChose = true;
        Data = _data;
        Data.ChangeAllAmount(-1);
        icon_Pos.sprite = Data.SoldierData.headImage;
        showBornImage.sprite = icon_Pos.sprite;
        nowPopulation = Data.SoldierData.population_need;
        OpenSelectImage(false);
    }

    //移除此位置
    public void removeSoldier()
    {
        if (Data != null)
        {
            if (!Data.isLowSoldier)
                Data.ChangeAllAmount(1);
            isChose = false;
            icon_Pos.sprite = originalImg;
            showBornImage.sprite = icon_Pos.sprite;
            nowPopulation = 0;
            Data = null;
        }
    }

    public void LockState(bool _t)
    {
        if (Lock_Pos != null)
            Lock_Pos.enabled = _t;
    }

    //生出小兵
    public void BornSoldier(Transform _pos)
    {
        if (isChose)
        {
            ObjectPooler.instance.getPoolObject(Data.SoldierData._soldierName, _pos.localPosition, Quaternion.LookRotation(_pos.forward));
        }
    }

    #region 顯示資訊
    // 滑鼠進入範圍
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (Data != null)
            uiManager.Info_MouseIn(Data.SoldierData, nowPopulation);
    }

    // 滑鼠離開範圍與點擊時
    public void OnPointerExit(PointerEventData eventData)
    {
        if (Data != null)
            uiManager.Info_Exit();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (Data != null)
            uiManager.Info_Exit();
    }
    #endregion
}
