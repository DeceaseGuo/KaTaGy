using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Prompt_SelectLocalPos : MonoBehaviour
{
    public enum whois
    {
        Player,
        Soldier,
        Tower,
        Building,
        Core
    }

    public enum allMenu
    {
        Null,
        MoinB_atk,  //塔防,士兵
        MoinB_build,
        Click_Obj,
        Click_Core
    }
    [System.Serializable]
    public class PromptMenuDataBase
    {
        [Header("基本0_0")]
        public Image headImage;
        public Text objectName;
        public Image firstATK;
        [Header("點擊時血量 攻防 2_0")]
        public Image Bar_HP;
        public Image Bar_AP;
        public Text level_ATK;
        public Text level_DEF;
        [Header("所需表0_1")]
        public Text need_ore;
        public Text need_money;
        public Text need_electricity;
        public Text need_time;
        [Header("屬性Bar0_2")]
        public float Max_Att_ATK;
        public float Max_Att_ASPD;
        public float Max_Att_DEF;
        public float Max_Att_MoveSPD;
        public Image Bar_ATK;
        public Image Bar_ASPD;
        public Image Bar_DEF;
        public Image Bar_MoveSPD;
        [Header("描述文字1_0")]
        public Text depictFrame;
    }

    public PromptMenuDataBase DataBase;
    [SerializeField] Sprite null_Image;

    [Header("面板類")]
    [SerializeField] CanvasGroup Menu_0_0; //頭像,名稱
    [SerializeField] CanvasGroup Menu_0_1; //需求
    [SerializeField] CanvasGroup Menu_0_2; //屬性條
    [SerializeField] CanvasGroup Menu_1_0; //文字框
    [SerializeField] CanvasGroup Menu_2; //Click顯示

    private Prompt_SelectObj selectObj;

    private void Start()
    {
        ClearPrompt();
    }

    #region 變換正確面板數據
    public void openMenu(allMenu _nowMenu)
    {
        switch (_nowMenu)
        {
            case (allMenu.MoinB_atk):
                Menu_0_0.alpha = 1;
                Menu_0_1.alpha = 1;
                Menu_0_2.alpha = 1;
                return;
            case (allMenu.MoinB_build):
                Menu_0_0.alpha = 1;
                Menu_0_1.alpha = 1;
                Menu_1_0.alpha = 1;
                return;
            case (allMenu.Click_Obj):
                Menu_0_0.alpha = 1;
                Menu_2.alpha = 1;
                return;
        }
    }
    #endregion

    #region 取得數據

    #region 滑鼠移入塔怪面板-0
    //取得頭像,名子-0
    public void setMoInBtMenu(Sprite _head, Sprite _firstAtk, string _name)
    {
        if (_head == null)
            DataBase.headImage.sprite = null_Image;
        else
            DataBase.headImage.sprite = _head;

        if (_firstAtk == null)
            DataBase.firstATK.sprite = null_Image;
        else
            DataBase.firstATK.sprite = _firstAtk;

        if (_name == "")
            DataBase.objectName.text = "no Name";
        else
            DataBase.objectName.text = _name;

    }

    //取得需求表-1
    public void setMoInBtMenu_Need(/*float _ore,*/ float _money, float _elect, float _needTime)
    {
       // DataBase.need_ore.text = _ore.ToString();
        DataBase.need_money.text = _money.ToString();
        DataBase.need_electricity.text = _elect.ToString();
        DataBase.need_time.text = _needTime.ToString();
    }

    //取得Bar條數值-2
    public void setMoInBtMenu_Bar(float _ATK, float _ASPD, float _DEF, float _MoveSPD)
    {
        DataBase.Bar_ATK.fillAmount = _ATK / DataBase.Max_Att_ATK /*←最大值*/;
        DataBase.Bar_ASPD.fillAmount = _ASPD / DataBase.Max_Att_ASPD /*←最大值*/;
        DataBase.Bar_DEF.fillAmount = _DEF / DataBase.Max_Att_DEF /*←最大值*/;
        DataBase.Bar_MoveSPD.fillAmount = _MoveSPD / DataBase.Max_Att_MoveSPD /*←最大值*/;
    }
    #endregion

    #region 滑鼠移入建築面板-1
    //需0-0, 0-1
    //文字描述框-0
    public void setDepictMenu(string _text)
    {
        if (_text == null)
            DataBase.depictFrame.text = "無資訊";

        DataBase.depictFrame.text = _text;
    }
    #endregion

    #region 點擊所有物體-2
    //需0-0
    //hp bar 過熱或魔力bar -0
    public void setClickObj(float _maxHp, float _hp ,float _maxAnyBar, float _anyBar)
    {
        DataBase.Bar_HP.fillAmount = _hp / _maxHp;
        DataBase.Bar_AP.fillAmount = _anyBar / _maxAnyBar;
    }

    //攻防-1
    public void setClickObj(int _nowATK, int _nowDEF)
    {
        DataBase.level_ATK.text = _nowATK.ToString();
        DataBase.level_DEF.text = _nowDEF.ToString();
    }
    #endregion

    #region 點擊核心-3_0
    // 描述框
    public void setClickCore(Text _text)
    {

    }
    #endregion




    //取得血量
    /*public void getHPBar(float _hpBarAmount, float _hpMax, Prompt_SelectObj _selectObj)
    {
        if (DataBase.hp_Bar != null)
        {
            DataBase.hp_Amount.text = _hpBarAmount.ToString("#0");
            DataBase.hp_Max.text = _hpMax.ToString("#0");
            DataBase.hp_Bar.fillAmount = _hpBarAmount / _hpMax;
        }
        selectObj = _selectObj;
    }

    //取得金錢,礦石 或攻擊,防禦 圖像
    public void getMidImage(Sprite _midImg1, Sprite _midImg2)
    {
        if (_midImg1 != null)
            DataBase.atkImage.sprite = _midImg1;
        if (_midImg2 != null)
            DataBase.defImage.sprite = _midImg2;
    }
    //取得金錢,礦石 或攻擊,防禦 文字
    public void getDynamicAmount(string _midText1, string _midText2)
    {
        DataBase.atkText.text = _midText1;
        DataBase.defText.text = _midText2;
    }

    //取得屬性圖像與文字
    public void getAttributes(Sprite _image0, Sprite _image1, Sprite _image2, Sprite _image3, Sprite _image4,
                                string _text0, string _text1, string _text2, string _text3, string _text4)
    {
        if (_image0 != null)
            DataBase.attribute_Images[0].sprite = _image0;
        if (_image1 != null)
            DataBase.attribute_Images[1].sprite = _image1;
        if (_image2 != null)
            DataBase.attribute_Images[2].sprite = _image2;
        if (_image3 != null)
            DataBase.attribute_Images[3].sprite = _image3;
        if (_image4 != null)
            DataBase.attribute_Images[4].sprite = _image4;

        if (_text0 != null)
            DataBase.attribute_Texts[0].text = _text0;
        if (_text1 != null)
            DataBase.attribute_Texts[1].text = _text1;
        if (_text1 != null)
            DataBase.attribute_Texts[2].text = _text2;
        if (_text1 != null)
            DataBase.attribute_Texts[3].text = _text3;
        if (_text1 != null)
            DataBase.attribute_Texts[4].text = _text4;
    }
    
    //取得電力或採礦量圖像與數量
    public void getCollectionMenu(Sprite _powerImage,string _powerText)
    {
        DataBase.powerImage.sprite = _powerImage;
        DataBase.powerText.text = _powerText;
    }
    //取得文字描述框
    public void getDialogueText(string _depict)
    {
        DataBase.depictFrame.text = _depict;
    }*/
    #endregion

    #region 關閉一切畫面
    public void ClearPrompt()
    {
        Menu_0_0.alpha = 0;
        Menu_0_1.alpha = 0;
        Menu_0_2.alpha = 0;
        Menu_1_0.alpha = 0;
        Menu_2.alpha = 0;
    }
    #endregion
}
