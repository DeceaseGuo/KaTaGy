using UnityEngine;

public class FloatingTextController : MonoBehaviour
{
    public static FloatingTextController instance;
    private GameObject displayDamageText;
        
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }               
    }

    private void Start()
    {
        displayDamageText = GameObject.Find("Display_DamageText");
    }

    #region 創建傷害Text
    public void CreateFloatingText(string text, Transform _location/*,EnemyControl _enemy*/)
    {
           GameObject _obj = ObjectPooler.instance.getPoolObject(GameManager.whichObject.popupText, _location.position, Quaternion.identity);
        //實例化顯示text的ui
        //FloatingText _folatObj = Instantiate(Resources.Load("PopupTextParent", typeof(FloatingText))) as FloatingText;
        //設置實例化出來的ui的父母位置
        _obj.transform.SetParent(displayDamageText.transform, false);
        //彈出位置=將當地位置轉為螢幕位置
        Vector2 screenPos = Camera.main.WorldToScreenPoint(_location.position);
        //隨機一個位置
        Vector2 offsetPos = new Vector3(Random.Range(-1f, 1f),0, /*Random.Range(5f, 15f)*/0);
        //彈出位置
        _obj.transform.position = screenPos+ offsetPos;
        //將哪到的text(數值)傳到實例化出來的腳本
        FloatingText floatText = _obj.GetComponent<FloatingText>();
        floatText.SetText(text/*, _enemy, offsetPos*/);
    }
    #endregion
}
