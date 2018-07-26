using MyCode.Timer;
using System.Collections;
using UnityEngine;

public class FieldOfView : Photon.MonoBehaviour
{
    [HideInInspector]
    public GameManager.NowTarget firstPriority;
    private GameManager.NowTarget nowTarget = GameManager.NowTarget.Null;
    private EnemyControl enemyControl;
    private float chaseTime = 0;
    [SerializeField] float waitTime;
    public float viewRadius;
    [Range(0, 360)]
    public float viewAngle;

    public LayerMask currentMask;

    public GameObject currentTarget;
    public isDead targetDeadScript;

    private IEnumerator findVisible;

    void Start()
    {
        enemyControl = GetComponent<EnemyControl>();
        SetCoroutine();
    }

    private void Update()
    {
        if (photonView.isMine && !enemyControl.deadManager.checkDead)
            if (enemyControl.nowState == EnemyControl.states.Wait_TargetDie || currentTarget != null)
                delayCancelTarget();
    }

    #region 尋找附近敵人
    public void StartDetectT()
    {
        if (photonView.isMine)
        {
            StopDetectT();
            StartCoroutine(findVisible);
        }
    }

    public void StopDetectT()
    {
        StopCoroutine(findVisible);
    }

    void SetCoroutine()
    {
        findVisible = Timer.Start(.6f, true, () =>
        {
            if (enemyControl.deadManager.checkDead)
                StopCoroutine(findVisible);

            if (!enemyControl.firstAtk && !enemyControl.ifAtkMoveStop && !ifFirstAtkTarget())
            {
                Collider[] targetsInViewRadius = Physics.OverlapSphere(transform.position, viewRadius, currentMask);
                foreach (var soldier in targetsInViewRadius)
                {
                    isDead _attributes = soldier.GetComponent<isDead>();

                    if (!_attributes.checkDead)
                    {
                        getCurrentTarget(_attributes);
                    }
                }
            }
        });

        StartDetectT();
    }
    #endregion

    /* private void OnDrawGizmos()
     {
         Gizmos.color = Color.blue;
         Gizmos.DrawWireSphere(transform.position, viewRadius);
     }*/

    #region 偵測攻擊優先順序
    void getCurrentTarget(isDead _inform)
    {
        if (nowTarget != GameManager.NowTarget.NoChange)
        {
            if (currentTarget != null)
            {
                if (_inform.myAttributes == GameManager.NowTarget.Core)
                {
                    //未完成
                   // currentTarget = _pos;
                    nowTarget = GameManager.NowTarget.NoChange;
                    chaseTime = 9999;
                    return;
                }
                else if (nowTarget != firstPriority && _inform.myAttributes == firstPriority)
                {
                    CreatPoints tmpPoint = _inform.GetComponent<CreatPoints>();
                    if (!tmpPoint.CheckFull(enemyControl.enemyData.atk_Range))
                    {
                        nowTarget = firstPriority;
                        goAtkPos(tmpPoint, _inform);
                    }

                    return;
                }
                return;
            }
            else
            {
                CreatPoints tmpPoint = _inform.GetComponent<CreatPoints>();

                if (_inform.myAttributes == GameManager.NowTarget.Core)
                {
                    //未完成
                    nowTarget = GameManager.NowTarget.NoChange;
                }
                else if (_inform.myAttributes == firstPriority)
                {
                    if (!tmpPoint.CheckFull(enemyControl.enemyData.atk_Range))
                    {
                        nowTarget = firstPriority;
                        goAtkPos(tmpPoint, _inform);
                    }
                }
                else if (_inform.myAttributes == GameManager.NowTarget.Soldier)
                {
                    if (!tmpPoint.CheckFull(enemyControl.enemyData.atk_Range))
                    {
                        nowTarget = GameManager.NowTarget.Soldier;
                        goAtkPos(tmpPoint, _inform);
                    }
                }
            }
        }
    }
    #endregion

    public CreatPoints points;
    public Transform nowPoint;
    public void goAtkPos(CreatPoints _tmpPoint,isDead _isdaed)
    {
        if (enemyControl.deadManager.checkDead)
            return;

        points = _tmpPoint;
        targetDeadScript = _isdaed;
        currentTarget = _isdaed.gameObject;
        if (nowTarget == GameManager.NowTarget.Null)
            nowTarget = _isdaed.myAttributes;

        Transform tmpPos = points.getPoint(enemyControl.enemyData.atk_Range, this.transform, enemyControl.enemyData.width, true);
        if (tmpPos == null)
        {
            cancelSelectTarget(true);
            return;
        }
        nowPoint = tmpPos;
        points.TestNext.Add(nowPoint);
        resetChaseTime();
        enemyControl.nowState = EnemyControl.states.AtkMove;
    }

    public void goWaitAtkPos(float _dis)
    {
        if (enemyControl.deadManager.checkDead)
            return;

        if (points.CheckFull(_dis))
        {
            cancelSelectTarget(false);
            return;
        }

        Transform tmpPos = points.getPoint(enemyControl.enemyData.atk_Range, this.transform, enemyControl.enemyData.width, true);
        if (tmpPos == null && chaseTime > 0)
        {
            //Debug.Log("找點啦 耖");
            return;
        }
        else if (tmpPos == null && chaseTime <= 0)
        {
            cancelSelectTarget(true);
            return;
        }

        nowPoint = tmpPos;
        points.TestNext.Add(nowPoint);
        resetChaseTime();
        enemyControl.nowState = EnemyControl.states.AtkMove;
    }

    void delayCancelTarget()
    {
        //目標死亡時等待時間
        if (enemyControl.nowState == EnemyControl.states.Wait_TargetDie)
        {
            if (chaseTime > 0)
            {
                chaseTime -= Time.deltaTime;
            }
            else
            {
                if (currentTarget != null)
                    return;                

                enemyControl.getNextPoint();
                enemyControl.nowState = EnemyControl.states.Move;
            }
        }
        else
        {
            //有目標且未死亡時
            if (chaseTime > 0)
            {
                if (targetDeadScript.checkDead)
                {
                    cancelSelectTarget(false);
                    return;
                }
                chaseTime -= Time.deltaTime;
            }
            else
            {
                cancelSelectTarget(false);
            }
        }
    }

    #region 取消目標
    public void cancelSelectTarget(bool _now)
    {
        if (nowPoint != null)
        {
            points.RemovePoint(enemyControl.enemyData.atk_Range, nowPoint);
            points.TestNext.Remove(nowPoint);
        }
        nowPoint = null;
        points = null;
        targetDeadScript = null;
        currentTarget = null;

        chaseTime = 0;
        nowTarget = GameManager.NowTarget.Null;
        enemyControl.closeAllDelay();
        enemyControl.firstAtk = false;
        enemyControl.ifAtkMoveStop = false;
        enemyControl.canAtking = true;
        if (!_now)
        {
            chaseTime = 1.5f;
            enemyControl.nowState = EnemyControl.states.Wait_TargetDie;
            StartDetectT();
        }
    }
    #endregion

    //回歸時間
    public void resetChaseTime()
    {
        chaseTime = waitTime;
    }

    public bool ifFirstAtkTarget()
    {
        if (nowTarget == GameManager.NowTarget.Core || nowTarget == firstPriority)
            return true;
        else
            return false;
    }

    public Vector3 DirFromAngle(float angleInDegrees, bool angleIsGlobal)
    {
        if (!angleIsGlobal)
        {
            angleInDegrees += transform.eulerAngles.y;
        }
        return new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), 0, Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));
    }
}
