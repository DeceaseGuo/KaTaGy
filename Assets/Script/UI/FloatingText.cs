using System.Collections;
using System.Collections.Generic;
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
    public void SetText(string text /*,EnemyControl _enemy,Vector3 _offset*/)
    {
        if (damageText == null)
        {
            damageText = GetComponentInChildren<Text>();
        }

        damageText.text = text;
        StartCoroutine("returnPoolObject");
        //offset = _offset;
        //enemyControl = _enemy;
    }
    #endregion
}
