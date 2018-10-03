using UnityEngine;

public class FloatingTextController : MonoBehaviour
{
    public static FloatingTextController instance;

    private ObjectPooler poolManager;
    private ObjectPooler PoolManager { get { if (poolManager == null) poolManager = ObjectPooler.instance; return poolManager; } }

    #region 緩存
    private GameObject displayDamageText;
    private Camera mainCamera;
    private Transform tmpTansform;
    private FloatingText tmpFloatText;
    private GameManager.whichObject popText;
    #endregion

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
        mainCamera = Camera.main;
        popText = GameManager.whichObject.popupText;
    }

    #region 創建傷害Text
    public void CreateFloatingText(float _text, Transform _location)
    {
        tmpFloatText = PoolManager.getPoolObject(popText, _location.position, Quaternion.identity).GetComponent<FloatingText>();
        tmpTansform = tmpFloatText.transform;

        tmpTansform.localScale = new Vector3(1, 1, 1);

        //設置實例化出來的ui的父母位置
        tmpTansform.SetParent(displayDamageText.transform, false);
        //彈出位置                                    ///將當地位置轉為螢幕位置     +  隨機一個x位置
        tmpTansform.position = mainCamera.WorldToScreenPoint(_location.position) + (new Vector3(Random.Range(-1.2f, 1.2f), 0, 0));
        //將哪到的text(數值)傳到實例化出來的腳本
        tmpFloatText.SetText(_text);
    }
    #endregion
}
