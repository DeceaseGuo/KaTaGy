using UnityEngine.EventSystems;
using UnityEngine;
using UnityEngine.UI;

public class Sort_nextBornBtn : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler
{
    private UIManager uiManager;
    private ObjectPooler poolManager;
    private ObjectPooler PoolManager { get { if (poolManager == null) poolManager = ObjectPooler.instance; return poolManager; } }
    public int nowPopulation = 0;
    public int order;
    public bool isChose = false;
    public Sort_Soldier Data;

    [Header("ICON")]
    [SerializeField] Image icon_Pos;
    [SerializeField] Image showBornImage;
    [SerializeField] CanvasGroup frame_Pos;
    [SerializeField] Image Lock_Pos;
    [SerializeField] Sprite originalImg;

    private EnemyControl tmpObj;
    
    private void Start()
    {
        uiManager = UIManager.instance;
        poolManager = ObjectPooler.instance;
    }

    public void OpenSelectImage(bool _t)
    {
        if (_t)
            frame_Pos.alpha = 1;
        else
            frame_Pos.alpha = 0;
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

    void ClearThis()
    {
        isChose = false;
        icon_Pos.sprite = originalImg;
        showBornImage.sprite = icon_Pos.sprite;
        uiManager.ChangeNowP(nowPopulation);
        nowPopulation = 0;
        Data = null;
    }

    public void LockState(bool _t)
    {
        if (Lock_Pos != null)
            Lock_Pos.enabled = _t;
    }

    //生出小兵
    public void BornSoldier(Transform _pos, bool _pathBool)
    {
        if (isChose)
        {
            tmpObj = PoolManager.getPoolObject(Data.SoldierData._soldierName, _pos.localPosition, Quaternion.LookRotation(_pos.forward)).GetComponent<EnemyControl>();
            tmpObj.selectRoad(_pathBool);

            if (!Data.isLowSoldier)
            {
                if (!Data.CheckAmountToClear(-1))
                    Data.ChangeAllAmount(-1);
                else
                    ClearThis();
            }
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
