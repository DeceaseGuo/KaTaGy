using UnityEngine.EventSystems;
using UnityEngine;
using UnityEngine.UI;

public class PromptScreen : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler
{
    [Header("數據")]
    public GameManager.whichObject DataName;

    public Prompt_SelectLocalPos.whois Iam; 
    private MyEnemyData.Enemies SoldierData;
    private TurretData.TowerDataBase TowerData;

    [SerializeField] Image lockImage;
    private Button localBtn;
    private Prompt_SelectLocalPos prompt_localPos;

    private void Awake()
    {
        prompt_localPos = GameObject.Find("Prompt_SelectObj").GetComponent<Prompt_SelectLocalPos>();
        localBtn = GetComponent<Button>();
    }

    private void Start()
    {
        SoldierData = MyEnemyData.instance.getEnemyData(DataName);
        TowerData = TurretData.instance.getTowerData(DataName);
    }

    #region 滑鼠進入範圍
    public void OnPointerEnter(PointerEventData eventData)
    {
        prompt_localPos.ClearPrompt();

        #region 傳送數據        
        switch (Iam)
        {
            case (Prompt_SelectLocalPos.whois.Building):

                prompt_localPos.openMenu(Prompt_SelectLocalPos.allMenu.MoinB_build);
                return;
            case (Prompt_SelectLocalPos.whois.Core):

                return;
            case (Prompt_SelectLocalPos.whois.Soldier):
                prompt_localPos.setMoInBtMenu(SoldierData.headImage, SoldierData.firstAtk, SoldierData.objectName);
                prompt_localPos.setMoInBtMenu_Need(SoldierData.cost_Ore, SoldierData.cost_Money, 0, SoldierData.soldier_CountDown);
                prompt_localPos.setMoInBtMenu_Bar(SoldierData.atk_Damage, SoldierData.atk_delay, SoldierData.def_base, SoldierData.moveSpeed);
                break;
            case (Prompt_SelectLocalPos.whois.Tower):
                prompt_localPos.setMoInBtMenu(TowerData.headImage, null, TowerData.objectName);
                prompt_localPos.setMoInBtMenu_Need(TowerData.cost_Ore, TowerData.cost_Money, TowerData.cost_Electricity, TowerData.turret_delayTime);
                prompt_localPos.setMoInBtMenu_Bar(TowerData.Atk_Damage, TowerData.Atk_Gap, TowerData.def_base, 0);
                break;
        }
        #endregion

        prompt_localPos.openMenu(Prompt_SelectLocalPos.allMenu.MoinB_atk);
    }
    #endregion

    public void UnLock()
    {
        if (lockImage != null)
            DestroyImmediate(lockImage);
        localBtn.interactable = true;
    }

    #region 滑鼠離開範圍與點擊時
    public void OnPointerExit(PointerEventData eventData)
    {
        prompt_localPos.ClearPrompt();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (localBtn.interactable)
        {
            prompt_localPos.ClearPrompt();
        }
    }
    #endregion
}
