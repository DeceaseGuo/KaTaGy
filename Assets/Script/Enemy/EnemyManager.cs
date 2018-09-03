using MyCode.Timer;
using System.Collections;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    #region 單例
    public static EnemyManager instance;
    #endregion

    //出生點
    private Transform correctBornPoint;
    public Transform CorrectBornPoint { get { return correctBornPoint; } set { correctBornPoint = value; } }

    [SerializeField] float soldierGapTime = .4f; //產生每隻小兵間隔時間

    [SerializeField] ArraySoldier arraySoldier;
    private Sort_Soldier sort_Soldier;    
    private IEnumerator soldierBorn;
    int nowNum = 0;

    public bool bornSoldier;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    private void Start()
    {
        if (bornSoldier)
            SetCoroution();
        else
        {
            soldierBorn = Timer.Start(soldierGapTime, true, () =>
            {
                arraySoldier.sort_list[nowNum].BornSoldier(CorrectBornPoint);
                nowNum += 1;
                if (nowNum == 3)
                {
                    StopCoroutine(soldierBorn);
                    nowNum = 0;
                }
            });
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F7))
        {
            StartCoroutine(soldierBorn);
        }
    }


    #region 取得買到的士兵
    public void getEnemyQueue(MyEnemyData.Enemies solider)
    {
        sort_Soldier = arraySoldier.getSortPos(solider._soldierName);
        if (sort_Soldier != null)
        {
            sort_Soldier.ChangeAllAmount(1);
            sort_Soldier = null;
        }
    }
    #endregion

    #region 協成
    public void SetCoroution()
    {
        soldierBorn = Timer.FirstAction(soldierGapTime,() =>
        {
            arraySoldier.sort_list[nowNum].BornSoldier(CorrectBornPoint);
            nowNum += 1;
            if (nowNum == arraySoldier.MaxPopulation)
            {
                nowNum = 0;
                StopCoroutine(soldierBorn);
            }
        });
    }
    #endregion

    #region 出產士兵
    public void SpawnWave()
    {
        StopCoroutine(soldierBorn);
        StartCoroutine(soldierBorn);
    }
    #endregion
}
