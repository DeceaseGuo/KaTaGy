using UnityEngine;
using UnityEngine.UI;

public class FloatingText : MonoBehaviour
{
    private ObjectPooler poolManager;
    private ObjectPooler PoolManager { get { if (poolManager == null) poolManager = ObjectPooler.instance; return poolManager; } }

    private float timer = 1.2f;
    private Text damageText;
    private GameManager.whichObject popText;

    private void Start()
    {
        damageText = GetComponentInChildren<Text>();
        popText = GameManager.whichObject.popupText;
    }

    void returnPoolObject()
    {
        PoolManager.Repool(popText, this.gameObject);
    }

    #region 取得參數
    public void SetText(float _text)
    {
        if (damageText == null)
        {
            damageText = GetComponentInChildren<Text>();
        }

        damageText.text = _text.ToString("0.0");
        Invoke("returnPoolObject",timer);
    }
    #endregion
}
