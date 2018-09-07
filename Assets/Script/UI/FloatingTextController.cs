using UnityEngine;

public class FloatingTextController : MonoBehaviour
{
    public static FloatingTextController instance;
    private GameObject displayDamageText;
    private Vector2 screenPos;

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
    public void CreateFloatingText(float _text, Transform _location)
    {
        GameObject _obj = ObjectPooler.instance.getPoolObject(GameManager.whichObject.popupText, _location.position, Quaternion.identity);

        _obj.transform.localScale = new Vector3(1, 1, 1);
        //彈出位置=將當地位置轉為螢幕位置
        screenPos = Camera.main.WorldToScreenPoint(_location.position);

        //設置實例化出來的ui的父母位置
        _obj.transform.SetParent(displayDamageText.transform, false);
        //彈出位置                              ////隨機一個位置
        _obj.transform.position = screenPos + (new Vector2(Random.Range(-1.2f, 1.2f), 0));
        //將哪到的text(數值)傳到實例化出來的腳本
        _obj.SendMessage("SetText", _text);
    }
    #endregion
}
