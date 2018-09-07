using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class FloatingText : MonoBehaviour
{
    [SerializeField] float timer;
    private Text damageText;
    //public Animator anim;
    //private Vector3 offset;

    private void OnEnable()
    {
        //AnimatorClipInfo[] clipInfo = animator.GetCurrentAnimatorClipInfo(0);
        // Destroy(gameObject, 0.8f/*clipInfo[0].clip.length*/);
        damageText = GetComponentInChildren<Text>();
    }


    IEnumerator returnPoolObject()
    {
        yield return new WaitForSeconds(timer);
        ObjectPooler.instance.Repool(GameManager.whichObject.popupText, this.gameObject);
    }
   /* private void Update()
    {
        if (enemyControl != null)
        {
            displayHpBar();
        }
    }*/

    /*#region 傷害數字跟隨敵人
    void displayHpBar()
    {
        Vector2 screenPos = Camera.main.WorldToScreenPoint(enemyControl.transform.position + offset);
        transform.position = screenPos;
    }
    #endregion*/

    #region 取得參數
    public void SetText(float _text)
    {
        if (damageText == null)
        {
            damageText = GetComponentInChildren<Text>();
        }

        damageText.text = _text.ToString("0.0");
        StartCoroutine("returnPoolObject");
    }
    #endregion
}
