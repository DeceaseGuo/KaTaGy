using UnityEngine.EventSystems;
using UnityEngine;
using UnityEngine.UI;

public class Sort_Soldier : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public GameManager.whichObject DataName;
    public bool isLowSoldier;
    public int maxAmount;
    private Button soldierBtn;
    [SerializeField] CanvasGroup IconImage;
    [SerializeField] Text amountText;
    [HideInInspector]
    public MyEnemyData.Enemies SoldierData;
    private UIManager uiManager;

    private void Start()
    {
        uiManager = UIManager.instance;
        soldierBtn = GetComponent<Button>();
    }

    public void ChangeAllAmount(int _amount)
    {
        if (isLowSoldier)
            return;

        if (maxAmount + _amount >= 0)
            maxAmount += _amount;
        amountText.text = maxAmount.ToString();

        if (maxAmount == 0)
        {
            soldierBtn.interactable = false;
            IconImage.alpha = 0.5f;
        }
        else if (!soldierBtn.interactable)
        {
            soldierBtn.interactable = true;
            IconImage.alpha = 1;
        }
    }

    public bool CheckAmountToClear(int _amount)
    {
        if (maxAmount + _amount == -1)
            return true;
        else
            return false;
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
