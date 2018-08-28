using UnityEngine.EventSystems;
using UnityEngine;
using UnityEngine.UI;

public class Sort_Soldier : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public GameManager.whichObject DataName;
    public bool isLowSoldier;
    public int maxAmount;
    private Button soldierBtn;
    [SerializeField] Image IconImage;
    [SerializeField] Text amountText;
    [HideInInspector]
    public MyEnemyData.Enemies SoldierData;
    private UIManager uiManager;

    private void Start()
    {
        uiManager = UIManager.instance;
        soldierBtn = GetComponent<Button>();
    }

    void SwitchImage(bool _t)
    {
        if (_t)
            soldierBtn.interactable = _t;
        else
            soldierBtn.interactable = _t;

        IconImage.enabled = _t;
    }

    public void ChangeAllAmount(int _amount)
    {
        if (isLowSoldier)
            return;

        maxAmount += _amount;
        amountText.text = maxAmount.ToString();
        if (maxAmount == 0)
            SwitchImage(false);
        else if (!IconImage.isActiveAndEnabled)
            SwitchImage(true);
    }

    public void ResetSoldierData()
    {
        if (DataName != GameManager.whichObject.None)
            SoldierData = MyEnemyData.instance.getMySoldierData(DataName);
    }

    #region 顯示資訊
    // 滑鼠進入範圍
    public void OnPointerEnter(PointerEventData eventData)
    {
        uiManager.Info_MouseIn(SoldierData, SoldierData.population_need);
    }

    // 滑鼠離開範圍與點擊時
    public void OnPointerExit(PointerEventData eventData)
    {
        uiManager.Info_Exit();
    }
    #endregion
}
