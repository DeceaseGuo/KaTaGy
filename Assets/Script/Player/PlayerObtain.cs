using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerObtain : MonoBehaviour
{
    public static PlayerObtain instance;

    [Header("資源")]
    [SerializeField] int resource_Money;

    [Header("資源text")]
    [SerializeField] Text text_Money;


    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown("f"))
        {
            obtaniResource(100);
        }
    }

    public void obtaniResource(int _money)
    {
        resource_Money += _money;
        changeResourceAmount();
    }

    public void consumeResource(int _money)
    {
        if (Check_MoneyAmount(_money))
        {
            resource_Money -= _money;
            changeResourceAmount();
        }
        else
        {
            Debug.Log("資源不夠");
        }
    }

    #region 改變目前資源顯示
    void changeResourceAmount()
    {
        text_Money.text = resource_Money.ToString();
    }
    #endregion

    #region 檢查金錢夠不夠
    public bool Check_MoneyAmount(int _money)
    {
        return (resource_Money >= _money) ? true : false;
    }
    #endregion

    #region 檢查電力夠不夠
    public bool Check_ElectricityAmount(int resource_Electricity, int _electricity)
    {
        if (resource_Electricity <= 0)
        {
            return false;
        }

        return (resource_Electricity >= _electricity) ? true : false;
    }
    #endregion
}
