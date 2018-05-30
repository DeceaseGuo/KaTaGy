using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FieldOfView : MonoBehaviour
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
  //  public LayerMask obstacleMask;

    public GameObject currentTarget;
    public isDead targetDeadScript;

    void Start()
    {
        enemyControl = GetComponent<EnemyControl>();
        startDetectT();
    }

    private void FixedUpdate()
    {
        delayCancelTarget();
    }

    public void startDetectT()
    {
        StartCoroutine("FindVisibleTargets", .5f);
    }

    IEnumerator FindVisibleTargets(float delay)
    {
        while (true)
        {
            yield return new WaitForSeconds(delay);

            if (enemyControl.nowState == EnemyControl.states.Die)
                yield return null;

            if (!enemyControl.firstAtk && !enemyControl.ifAtkMoveStop)
            {
                Collider[] targetsInViewRadius = Physics.OverlapSphere(transform.position, viewRadius, currentMask);

                foreach (var soldier in targetsInViewRadius)
                {
                    GameObject target = soldier.transform.gameObject;
                    isDead _attributes = target.GetComponent<isDead>();
                    CreatPoints test = soldier.GetComponent<CreatPoints>();

                    if (!_attributes.checkDead && !test.CheckFull(enemyControl.enemyData.atk_Range))
                    {
                        getCurrentTarget(_attributes.myAttributes, target);
                    }
                }
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, viewRadius);
    }

    #region 偵測攻擊優先順序
    void getCurrentTarget(GameManager.NowTarget _inform, GameObject _pos)
    {
        if (nowTarget != GameManager.NowTarget.NoChange)
        {
            if (currentTarget != null)
            {
                if (_inform == GameManager.NowTarget.Core)
                {
                    //未完成
                    currentTarget = _pos;
                    nowTarget = GameManager.NowTarget.NoChange;
                    chaseTime = 9999;
                    return;
                }
                else if (nowTarget != firstPriority && _inform == firstPriority)
                {
                    nowTarget = firstPriority;
                    goAtkPos(_pos);

                    return;
                }
                return;
            }
            else
            {
                if (_inform == GameManager.NowTarget.Core)
                {
                    //未完成
                    nowTarget = GameManager.NowTarget.NoChange;
                }
                else if (_inform == firstPriority)
                {
                    nowTarget = firstPriority;
                    goAtkPos(_pos);
                }
                else if (_inform == GameManager.NowTarget.Soldier)
                {
                    nowTarget = GameManager.NowTarget.Soldier;
                    goAtkPos(_pos);
                }
            }
        }
    }
    #endregion
    public CreatPoints points;
    public Transform nowPoint;
    public void goAtkPos(GameObject _pos)
    {
        if (enemyControl.nowState == EnemyControl.states.Die)
            return;

        points = _pos.GetComponent<CreatPoints>();
        Transform tmpPos = points.getPoint(enemyControl.enemyData.atk_Range, this.transform, enemyControl.enemyData.width, true);
        if (tmpPos == null)
        {
            nowTarget = GameManager.NowTarget.Null;
            nowPoint = null;
            return;
        }
        currentTarget = _pos;
        nowPoint = tmpPos;
        points.TestNext.Add(nowPoint);
        Debug.Log("test加111111111111");

        //enemyControl.getNowTarget(tmpPos);
        targetDeadScript = _pos.GetComponent<isDead>();
        resetChaseTime();
        enemyControl.nowState = EnemyControl.states.AtkMove;
    }

    public void goWaitAtkPos(float _dis)
    {
        if (enemyControl.nowState == EnemyControl.states.Die)
            return;

        if (points.CheckFull(_dis))
        {
            cancelSelectTarget();
            return;
        }

        Transform tmpPos = points.getPoint(enemyControl.enemyData.atk_Range, this.transform, enemyControl.enemyData.width, true);
        if (tmpPos == null)
        {
            Debug.Log("找點啦 耖");
            return;
        }

        nowPoint = tmpPos;
        points.TestNext.Add(nowPoint);
        Debug.Log("test加2222222222222222222");
        resetChaseTime();
        enemyControl.nowState = EnemyControl.states.AtkMove;
    }

    void delayCancelTarget()
    {
        if (currentTarget != null )
        {
            if (chaseTime > 0)
            {
                if (targetDeadScript.checkDead)
                {
                    cancelSelectTarget();
                    return;
                }

                if (!targetDeadScript.checkDead && points.CheckThisPointFull(nowPoint) && !enemyControl.ifAtkMoveStop)
                {
                    goWaitAtkPos(enemyControl.enemyData.atk_Range);
                    Debug.Log("field取消這");
                }
                    //cancelSelectTarget();

                chaseTime -= Time.deltaTime;
            }
            else
            {
                cancelSelectTarget();
            }
        }

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
    }

    #region 取消目標
    public void cancelSelectTarget()
    {
        points.RemovePoint(enemyControl.enemyData.atk_Range, nowPoint);
        points.TestNext.Remove(nowPoint);
        currentTarget = null;
        nowPoint = null;
        points = null;
        targetDeadScript = null;

        //chaseTime = 0;
        chaseTime = 1.5f;
        nowTarget = GameManager.NowTarget.Null;
        enemyControl.closeAllDelay();
        enemyControl.firstAtk = false;
        enemyControl.ifAtkMoveStop = false;
        enemyControl.canAtking = true;

        enemyControl.nowState = EnemyControl.states.Wait_TargetDie;
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
