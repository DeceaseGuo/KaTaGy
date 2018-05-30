using UnityEngine;
using UnityEngine.UI;

public class EnemyIcon : MonoBehaviour
{
    private EnemyManager enemyManager;
    private EnemyBornPoint bornPoint;
    [SerializeField] GameManager.whichObject iconName;

    private void Start()
    {
        enemyManager = EnemyManager.instance;
    }

    public void OnClick()
    {
        if (bornPoint != null)
        {
            bornPoint.endColdDown();
            bornPoint = null;
        }
        else
            enemyManager.RemoveEnemyIcon(iconName, this.gameObject);
    }

    #region 增加與移除生成區
    public void addBornPoints(EnemyBornPoint _bornPos)
    {
        bornPoint = _bornPos;
    }
    #endregion
}