using UnityEngine;
using UnityEngine.UI;

public class OreObject : Photon.MonoBehaviour
{
    #region 取得單例
    private MatchTimer matchTime;
    public MatchTimer MatchTimeManager { get { if (matchTime == null) matchTime = MatchTimer.Instance; return matchTime; } }

    private FloatingTextController floatTextCon;
    protected FloatingTextController FloatTextCon { get { if (floatTextCon == null) floatTextCon = FloatingTextController.instance; return floatTextCon; } }
    #endregion

    //數據
    private isDead deadManager;
    private Transform oreBornPoint;
    public int ownMoney;
    public int delayToReBorn;
    public float maxHealth;
    private float health;

    //血量UI
    private byte modifyIndex;
    public CanvasGroup canvasGroup; 
    public Image hpBar;

    public Renderer originalObj;
    public Renderer destoryObj;

    private void Start()
    {
        deadManager = GetComponent<isDead>();
        oreBornPoint = this.transform;
        health = maxHealth;
    }

    #region 傷害
    //重攻擊bool
    [PunRPC]
    public void takeDamage(bool _BigDamage, int _ID)
    {
        if (deadManager.checkDead)
            return;

        if (health > 0)
        {
            CloseHP();
            if (!_BigDamage)
            {
                health -= 1;
                openPopupObject(1);
            }
            else
            {
                health -= 1.5f;
                openPopupObject(1.5f);
            }
            if (health <= 0)
            {
                PhotonView.Find(_ID).GetComponent<Player>().GetSomeMoney(ownMoney);
                Death();
            }
        }
    }
    //顯示傷害
    void openPopupObject(float _damage)
    {
        FloatTextCon.CreateFloatingText(_damage, oreBornPoint);
        hpBar.fillAmount = health / maxHealth;
    }
    #endregion

    #region 血量顯示與關閉
    protected void CloseHP()
    {
        if (canvasGroup.alpha != 1)
        {
            canvasGroup.alpha = 1;
            modifyIndex = MatchTimeManager.SetCountDown(closeHpBar, 6);
        }
        else
        {
            MatchTimeManager.ModifyTime(modifyIndex, 6);
        }
    }
    void closeHpBar()
    {
        canvasGroup.alpha = 0;
        modifyIndex = 0;
    }
    #endregion

    void Death()
    {
        //被破壞特效(未寫)-----------------
        originalObj.enabled = false;
        destoryObj.enabled = true;
        deadManager.ifDead(true);
        MatchTimeManager.SetCountDownNoCancel(ReBorn, delayToReBorn);
        MatchTimeManager.ClearThisTask(modifyIndex);
        closeHpBar();
    }

    void ReBorn()
    {
        destoryObj.enabled = false;
        originalObj.enabled = true;
        deadManager.ifDead(false);
        health = maxHealth;
    }
}