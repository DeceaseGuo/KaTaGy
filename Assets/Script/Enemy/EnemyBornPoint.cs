using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyBornPoint : MonoBehaviour
{
    private EnemyManager enemyManager;
    private float CD = 0;
    private Image cdBar;

    Coroutine timer;

    private MyEnemyData.Enemies tmpSoldier;
    //生成怪物icon位子
    [SerializeField] GameObject creatPos;
    private GameObject enemyIcon;


    private void Start()
    {
        enemyManager = EnemyManager.instance;
    }

    #region 計算生怪冷卻
    public IEnumerator CalculateTime()
    {
        for (CD = tmpSoldier.soldier_CountDown; CD > 0; CD -= Time.deltaTime)
        {
            cdBar.fillAmount = CD / tmpSoldier.soldier_CountDown;
            yield return 0;
        }

        if (CD <= 0)
        {
            CD = 0;
            BornEnemy();
            enemyManager.RemoveSoldier();
            if (this.timer != null)
            {
                StopCoroutine(this.timer);
                this.timer = null;
            }
            cdBar.enabled = false;
            cdBar.fillAmount = 1;
            enemyManager.nextSoldier();
        }
    }
    #endregion

    #region 前往cd等待區
    public void goToBornArea(GameObject _icon, MyEnemyData.Enemies _soldier)
    {
        tmpSoldier = _soldier;
        enemyManager.decreaseWaitBornIcon();
        _icon.transform.SetParent(creatPos.transform, false);
        enemyIcon = _icon.gameObject;
        EnemyIcon cancel = enemyIcon.GetComponent<EnemyIcon>();
        cancel.addBornPoints(this);
        Transform tmpGameObj = enemyIcon.transform.GetChild(0);
        cdBar = tmpGameObj.GetComponent<Image>();
        cdBar.enabled = true;

        timer = StartCoroutine("CalculateTime");
    }
    #endregion

    #region 生成怪物
    void BornEnemy()
    {
        GameObject enemy = ObjectPooler.instance.getPoolObject(tmpSoldier._soldierName, transform.localPosition, Quaternion.LookRotation(transform.forward));
        EnemyControl enemyControl = enemy.GetComponent<EnemyControl>();
        enemyManager.enemies.Add(enemyControl);
        ObjectPooler.instance.Repool(tmpSoldier.UI_Name, enemyIcon);
    }
    #endregion

    #region 取消生成
    public void endColdDown()
    {
        if (this.timer != null)
        {
            StopCoroutine(this.timer);
            this.timer = null;

            PlayerObtain.instance.obtaniResource(tmpSoldier.cost_Ore, tmpSoldier.cost_Money);
            enemyManager.RemoveSoldier();
            cdBar.enabled = false;
            cdBar.fillAmount = 1;
            enemyManager.returnPoolObject(tmpSoldier.UI_Name, enemyIcon);
            enemyManager.nextSoldier();
        }
    }
    #endregion
}
