using MyCode.Timer;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(isDead))]
public class EnemyControl : Photon.MonoBehaviour
{
    #region 取得單例
    private SceneObjManager sceneObjManager;
    private SceneObjManager SceneManager { get { if (sceneObjManager == null) sceneObjManager = SceneObjManager.Instance; return sceneObjManager; } }

    private MatchTimer matchTime;
    protected MatchTimer MatchTimeManager { get { if (matchTime == null) matchTime = MatchTimer.Instance; return matchTime; } }

    private EnemyManager enemyBornManager;
    protected EnemyManager EnemyBornScript { get { if (enemyBornManager == null) enemyBornManager = EnemyManager.instance; return enemyBornManager; } }
    #endregion

    #region 數據相關
    public GameManager.whichObject DataName;
    public MyEnemyData.Enemies enemyData;
    public MyEnemyData.Enemies originalData;
    private bool firstGetData = true;
    #endregion

    [HideInInspector]
    public isDead deadManager;  

    //尋找目標
    [SerializeField] protected GameManager.NowTarget firstPriority;
    protected GameManager.NowTarget nowTarget = GameManager.NowTarget.Null;

    [Tooltip("偵測半徑")]
    public float viewRadius;
    [Tooltip("追逐時間")]
    [SerializeField] protected float waitTime;
    [Tooltip("攻擊過後與敵人死亡僵直時間")]
    [SerializeField] protected float waitNextActionTime;
    //目前剩餘時間
    private float chaseTime = 0;
    //能攻擊對象layer
    public LayerMask currentMask;
    //正確目標
    protected GameObject currentTarget;
    //protected GameObject tmpTarget;
    protected SceneObjManager.myCorrectTarget myTmpTarget;
    //正確目標是否死亡腳本
    protected isDead targetDeadScript;

    private IEnumerator findVisible;
   // public List<GameObject> myTarget;
    //尋路
    protected NavMeshAgent nav;
   // protected RandomNodeManager randomNodeManager;
    public Node[] agentPoints;
  //  protected Transform targetPoint;
    private int Find_PathPoint;
    protected int nowPoint;
    public int NowPoint { get { return nowPoint; } }
    protected bool nextPos;
    //取得傷害
    protected bool firstAtk;
    protected bool ifAtkMoveStop;

    private bool nowCC;
    public bool NowCC { get { return nowCC; } set { nowCC = value; } }

    //血量
    private float maxValue;
    [SerializeField] Renderer myRender;
    [SerializeField] protected CanvasGroup UI_HpObj;
    [SerializeField] Image UI_HpBar;

    public enum states
    {
        Null,
        Move,
        AtkMove,
        Atk,
        AtkWait,
        Wait_Move,
        BeAtk,
        Wait_TargetDie,
    }
    public states nowState = states.Null;

    //自身的碰撞
    protected CapsuleCollider myCollider;
    protected Quaternion CharacterRot;
    protected Animator ani;
    protected bool haveHit;
    public Transform sword_1;
    protected Vector3 atkDir;
    //偵測可攻擊對象的容器
    protected Collider[] enemiesCon;

    //正確敵人位置腳本
    protected CreatPoints points;
    //正確敵人位置
    public Transform correctPos;

    public PhotonView Net;

    #region 巡找目標
    private isDead _attributes;
    //protected CreatPoints Cpoint;
    //找點閃控紀錄
    protected Vector3 lastTmpPos;
    //找點暫時紀錄
    protected Transform tmpPos;
    //尋路下個位子需求
    protected Vector3 tmpNextPos;
    #endregion

    #region 攻擊所需
    //攻擊偵測 
    [SerializeField]
    protected Vector3 checkEnemyBox;
    protected isDead atkTarget;
    protected PhotonView atkNet;
    #endregion

    //動畫雜湊值
    protected int[] aniHashValue;
    [SerializeField]
    protected int allHashAmount = 3;

    private void Awake()
    {
        SetAniHash();
        AtkDetectSet();
    }

    private void OnEnable()
    {
        myRender.material.SetFloat("Vector1_D655974D", 0);

        //(true物件池生成會先第一次執行一次)
        //false從物件池哪出後執行
        if (!firstGetData)
            FormatData();
        else
            FirstformatData();
    }

    public void NeedToUpdate()
    {
        //if (!photonView.isMine)
           // return;

        if (!deadManager.checkDead)
        {
            if (nowState == states.Move || nowState == states.AtkMove || nowState == states.AtkWait)
                DetectState();

            if (nowState == states.Wait_TargetDie || currentTarget != null)
                delayCancelTarget();
        }
    }

    #region 恢復初始數據
    void FirstformatData()
    {
        firstGetData = false;

        if (deadManager == null)
            deadManager = GetComponent<isDead>();

        if (photonView.isMine)
            originalData = MyEnemyData.instance.getMySoldierData(DataName);
        else
            originalData = MyEnemyData.instance.getEnemySoldierData(DataName);

        Net = GetComponent<PhotonView>();
        ani = GetComponent<Animator>();
        myCollider = GetComponent<CapsuleCollider>();
        nav = GetComponent<NavMeshAgent>();
        if (photonView.isMine)
        {
            GetComponent<CreatPoints>().enabled = false;
            checkCurrentPlay();
            SetCoroutine();
            nav.updateRotation = false;
        }
        else
            nav.enabled = false;
    }

    void FormatData()
    {
        if (photonView.isMine)
        {
            SceneManager.AddMy_SoldierList(this);
            if (originalData.ATK_Level != MyEnemyData.mySoldierAtkLevel || originalData.DEF_Level != MyEnemyData.mySoldierDefLevel)
                originalData = MyEnemyData.instance.getMySoldierData(DataName);
        }
        else
        {
            SceneManager.AddEnemy_SoldierList(this);
            if (originalData.ATK_Level != MyEnemyData.enemySoldierAtkLevel || originalData.DEF_Level != MyEnemyData.enemySoldierDefLevel)
                originalData = MyEnemyData.instance.getEnemySoldierData(DataName);
        }

        enemyData = originalData;
        ifAtkMoveStop = false;
        deadManager.ifDead(false);
        nav.speed = enemyData.moveSpeed;

        if (ani.GetBool(aniHashValue[1]))
            Net.RPC("TP_stopAni", PhotonTargets.All, false);

        if (myCollider != null)
            myCollider.enabled = true;

        if (photonView.isMine)
        {
            nowPoint = Find_PathPoint;
            if (nav != null)
                nav.enabled = true;
            StartDetectT();
        }
    }
    #endregion

    #region 取得動畫雜湊值
    protected virtual void SetAniHash()
    {
        if (allHashAmount <= 2)
            return;

        aniHashValue = new int[allHashAmount];
        //crossFade死亡
        aniHashValue[0] = Animator.StringToHash("Base Layer.dead");
        aniHashValue[1] = Animator.StringToHash("Stop");
        aniHashValue[2] = Animator.StringToHash("Hit");
    }
    #endregion

    protected virtual void AtkDetectSet()
    { }

    #region 尋找正確敵人(協成設定)
    //開始尋找目標
    public void StartDetectT()
    {
        if (photonView.isMine)
        {
            StopDetectT();
            StartCoroutine(findVisible);
        }
    }
    //停止偵測目標
    public void StopDetectT()
    {
        StopCoroutine(findVisible);
    }

    void SetCoroutine()
    {
        findVisible = Timer.Start(.65f, true, () =>
        {
            if (!firstAtk && !ifAtkMoveStop && !ifFirstAtkTarget())
            {
                getCurrentTarget();
            }
        });
    }
    //給其他小兵更改
   /* protected virtual void SetFindT()
    {
        myTarget = SceneManager.CalculationDis(gameObject, viewRadius, false, false);
    }*/
    #endregion

    #region 偵測攻擊優先順序
    /*void getCurrentTarget(isDead _inform)
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
                    //chaseTime = 9999;
                }
                else if (nowTarget != firstPriority && _inform.myAttributes == firstPriority)
                {
                    Cpoint = _inform.GetComponent<CreatPoints>();
                    if (!Cpoint.CheckFull(enemyData.atk_Range))
                    {
                        nowTarget = firstPriority;
                        goAtkPos(Cpoint, _inform);
                    }
                }
                else if (nowTarget != firstPriority && _inform.myAttributes == GameManager.NowTarget.Soldier)
                {
                   /* if (!Cpoint.CheckFull(enemyData.atk_Range))
                    {
                        nowTarget = GameManager.NowTarget.Soldier;
                        goAtkPos(Cpoint, _inform);
                    }*/
    /*          }
          }
          else
          {
              Cpoint = _inform.GetComponent<CreatPoints>();

              if (_inform.myAttributes == GameManager.NowTarget.Core)
              {
                  //未完成
                  nowTarget = GameManager.NowTarget.NoChange;
              }
              else if (_inform.myAttributes == firstPriority)
              {
                  if (!Cpoint.CheckFull(enemyData.atk_Range))
                  {
                      nowTarget = firstPriority;
                      goAtkPos(Cpoint, _inform);
                  }
              }
              else if (_inform.myAttributes == GameManager.NowTarget.Soldier)
              {
                  if (!Cpoint.CheckFull(enemyData.atk_Range))
                  {
                      nowTarget = GameManager.NowTarget.Soldier;
                      goAtkPos(Cpoint, _inform);
                  }
              }
          }
      }
  }*/


    void getCurrentTarget()
    {
        if (nowTarget != GameManager.NowTarget.Core)
        {
            switch (firstPriority)
            {
                case GameManager.NowTarget.Player:
                    myTmpTarget = SceneManager.CalculationDis_Soldier(this, true);
                    break;
                case GameManager.NowTarget.Soldier:
                    myTmpTarget = SceneManager.CalculationDis_Soldier(this, false);
                    break;
                case GameManager.NowTarget.Tower:
                    myTmpTarget = SceneManager.CalculationDis_Tower(this);
                    break;
                default:
                    break;
            }

            if (myTmpTarget.myTarget != null)
            {
                _attributes = myTmpTarget.myTarget.GetComponent<isDead>();
                if (!_attributes.checkDead)
                {
                    nowTarget = _attributes.myAttributes;
                    goAtkPos(myTmpTarget.nowPointScript, myTmpTarget.goPos, _attributes);
                }
            }
        }
    }
    #endregion

    #region 判斷並前往攻擊點
    public void goAtkPos(CreatPoints _tmpPoint,Transform firstPos ,isDead _isdaed)
    {
        if (deadManager.checkDead)
            return;

        points = _tmpPoint;
        targetDeadScript = _isdaed;
        currentTarget = _isdaed.gameObject;

        //前往找到的點
        points.AddWillGo_P(firstPos, correctPos);
        correctPos = firstPos;
        lastTmpPos = correctPos.position;
        resetChaseTime();

        //在距離內可直接攻擊
        if (IFRightNowAtk())
            return;

        nowState = states.AtkMove;
        getTatgetPoint(correctPos.position);
    }

    bool IFRightNowAtk()
    {
        if (!deadManager.checkDead)
        {
            tmpNowDis_F = Vector3.Distance(transform.position, currentTarget.transform.position);
            if (points.IFDis(enemyData.atk_Range, tmpNowDis_F - enemyData.stoppingDst))
            {
                if (Vector3.Distance(transform.position, correctPos.transform.position) < enemyData.width * 0.5f)
                {
                    if (canAtking)
                    {
                        if (!ani.GetBool(aniHashValue[1]))
                            Net.RPC("TP_stopAni", PhotonTargets.All, true);
                        ifAtkMoveStop = true;
                        StopDetectT();
                        rotToTarget();
                        resetChaseTime();
                        points.RemoveThisPoint(correctPos);
                        points.AddPoint(correctPos);
                        nowState = states.Atk;
                        nav.ResetPath();
                        enemyAttack_time = StartCoroutine(enemyAttack());
                        return true;
                    }
                }
            }
        }
        return false;
    }
    #endregion

    #region 前往攻擊點
    public void goWaitAtkPos(float _dis)
    {
        if (deadManager.checkDead)
            return;

        if (points.CheckFull(_dis))
        {
            cancelSelectTarget(false);
            return;
        }

        //在距離內可直接攻擊
       // IFRightNowAtk();
        //找一個點
        tmpPos = points.FindClosePoint(enemyData.atk_Range, this.transform, enemyData.width);
        if (tmpPos == null && chaseTime > 0)
        {
            Debug.Log("找點啦 耖");
            return;
        }
        else if (tmpPos == null && chaseTime <= 0)
        {
            Debug.Log("前往攻擊點這時間到  取消");
            cancelSelectTarget(false);
            return;
        }

        points.AddWillGo_P(tmpPos, correctPos);
        correctPos = tmpPos;
        lastTmpPos = correctPos.position;
        resetChaseTime();
        nowState = states.AtkMove;
        getTatgetPoint(correctPos.position);
    }

    public void ReturnState()
    {        
        if (photonView.isMine)
        {
            if (currentTarget != null && currentTarget.activeSelf)
            {
                nowState = states.AtkWait;
                resetChaseTime();
            }
            else
            {
                cancelSelectTarget(true);
                getNextPoint();
                nowState = states.Move;
            }
        }
    }
    #endregion

    #region 目前為玩家幾
    public void checkCurrentPlay()
    {
        if (GameManager.instance.getMyPlayer() == GameManager.MyNowPlayer.player_1)
        {
            Net.RPC("changeLayer", PhotonTargets.All, 28);
            Net.RPC("changeMask_1", PhotonTargets.All);
            nowPoint = 0;
            Find_PathPoint = nowPoint;
        }
        else if (GameManager.instance.getMyPlayer() == GameManager.MyNowPlayer.player_2)
        {
            Net.RPC("changeLayer", PhotonTargets.All, 29);
            Net.RPC("changeMask_2", PhotonTargets.All);
            nowPoint = 9;
            Find_PathPoint = nowPoint;          
        }
    }

    [PunRPC]
    public void changeMask_1()
    {
        currentMask = GameManager.instance.getPlayer1_Mask;
    }
    [PunRPC]
    public void changeMask_2()
    {
        currentMask = GameManager.instance.getPlayer2_Mask;
    }
    #endregion

    #region 士兵選擇路線
    public void selectRoad(bool _pathBool)
    {
        if (_pathBool)
            agentPoints = EnemyBornScript.nodePoints_1;
        else
            agentPoints = EnemyBornScript.nodePoints_2;

        nowState = states.Move;
        getNextPoint();
    }
    #endregion

    #region 偵測目前狀態
    void DetectState()
    {
        switch (nowState)
        {
            case (states.Move):
                enemyMove();
                break;
            case (states.AtkMove):
                if (targetDeadScript != null && !targetDeadScript.checkDead)
                    enemyMove();           
                break;
            case (states.AtkWait):
                if (!ani.GetBool(aniHashValue[1]))
                {
                    Net.RPC("TP_stopAni", PhotonTargets.All, true);
                }
                if (targetDeadScript != null && !targetDeadScript.checkDead)
                    goWaitAtkPos(enemyData.atk_Range);
                break;
            default:
                return;
        }
    }
    #endregion

    public void getTatgetPoint(Vector3 _targetPoint)
    {
        if (nav != null && nav.enabled != false)
        {
            if (ani.GetBool(aniHashValue[1]))
                Net.RPC("TP_stopAni", PhotonTargets.All, false);
            nav.SetDestination(_targetPoint);
        }
    }

    #region 士兵移動
    void enemyMove()
    {
        if (nowState == states.Move)
        {
            if (!nav.hasPath)
                getNextPoint();

            findNextPath();

            #region 判斷是否到最終目標點→否則執行移動
            if (nextPos && ifReach(nav.destination))
                getNextPoint();
            else            
                transform.rotation = Quaternion.Lerp(transform.rotation, CharacterRot, enemyData.rotSpeed);
            #endregion
        }

        if (nowState == states.AtkMove)
        {
            //if (!ifAtkMoveStop)
            if (lastTmpPos != correctPos.position)
            {
                tmpPos = points.GoComparing(enemyData.atk_Range, transform, correctPos, enemyData.width);
                if (tmpPos != null)
                {
                    points.AddWillGo_P(tmpPos, correctPos);
                    correctPos = tmpPos;                                       
                }

                lastTmpPos = correctPos.position;
                getTatgetPoint(correctPos.position);

              /*  if (points != null && correctPos == null)
                {
                    //點不見了
                    nowState = states.AtkWait;
                    return;
                }*/
            }
            
            findNextPath();
            //到達可攻擊區域直接攻擊
            if (IFRightNowAtk())
                return;

            #region 判斷是否到攻擊目標點→否則執行移動
            if (ifReach(nav.destination))
            {
                if (!ani.GetBool(aniHashValue[1]))
                    Net.RPC("TP_stopAni", PhotonTargets.All, true);

                rotToTarget();
                ifAtkMoveStop = true;

                if (canAtking && !deadManager.checkDead)
                {
                    points.RemoveThisPoint(correctPos);
                    points.AddPoint(correctPos);
                    nowState = states.Atk;
                    nav.ResetPath();

                    StopDetectT();
                    enemyAttack_time = StartCoroutine(enemyAttack());
                }
            }
            else  //士兵旋轉            
                transform.rotation = Quaternion.Lerp(transform.rotation, CharacterRot, enemyData.rotSpeed);
            #endregion
        }
    }
    #endregion

    #region 尋找下一個位置方向
    void findNextPath()
    {
        tmpNextPos = nav.steeringTarget - transform.position;
        tmpNextPos.y = transform.localPosition.y;
        CharacterRot = Quaternion.LookRotation(tmpNextPos);
    }
    #endregion

    #region 小兵攻擊
    protected Coroutine enemyAttack_time;
    [HideInInspector] public bool canAtking = true;
    protected virtual IEnumerator enemyAttack()
    {
        yield return null;
    }
    #endregion

    #region 攻擊後間隔
    protected Coroutine sotpWait_time;
    protected bool OverAtkDis = false;
    protected float tmpNowDis_F;
    protected IEnumerator stopWait()
    {
        while (nowState == states.Wait_Move)
        {
            if (currentTarget == null || deadManager.checkDead)
            {
                Debug.Log("StopWait敵人死亡");
                cancelSelectTarget(true);
                KillTarget();
                yield break;
            }

            tmpNowDis_F = Vector3.Distance(transform.position, currentTarget.transform.position);

            //攻擊延遲到了
            if (canAtking && !deadManager.checkDead)
            {
                if (!points.IFDis(enemyData.atk_Range, tmpNowDis_F - enemyData.stoppingDst))
                {
                    // 不在攻擊區域→往攻擊移動                  
                    points.RemovePoint(correctPos);
                    ifAtkMoveStop = false;
                    nowState = states.AtkWait;
                    yield break;
                }
                else
                {
                    //在攻擊區內→進行攻擊
                    nowState = states.Atk;
                    StopDetectT();
                    enemyAttack_time = StartCoroutine(enemyAttack());
                    yield break;
                }
            }
            else //攻擊延遲還沒到
            {
                //超過攻擊範圍時 →OverAtkDis=True
                if (!points.IFDis(enemyData.atk_Range, tmpNowDis_F - enemyData.stoppingDst) && !OverAtkDis)
                {
                    points.RemovePoint(correctPos);
                    points.AddWillGo_P(correctPos, correctPos);
                    OverAtkDis = true;
                }

                //超過攻擊範圍進行追趕
                if (OverAtkDis)
                {
                    if (lastTmpPos != correctPos.position)
                    {
                        tmpPos = points.GoComparing(enemyData.atk_Range, transform, correctPos, enemyData.width);
                        if (tmpPos != null)
                        {
                            points.RemoveWillGo_P(correctPos);

                            points.AddWillGo_P(tmpPos, correctPos);
                            correctPos = tmpPos;
                        }

                        lastTmpPos = correctPos.position;
                    }

                    //未到達範圍 →追趕
                    if (!ifReach(correctPos.position))
                    {
                        getTatgetPoint(correctPos.position);
                        findNextPath();
                        transform.rotation = Quaternion.Lerp(transform.rotation, CharacterRot, enemyData.rotSpeed);
                    }
                    else //到達範圍 →OverAtkDis=False
                    {
                        OverAtkDis = false;
                        points.RemoveThisPoint(correctPos);
                        points.AddPoint( correctPos);
                    }
                }
                else //沒超過攻擊範圍 士兵自動換點
                {
                    rotToTarget();
                    if (!ani.GetBool(aniHashValue[1]))
                        Net.RPC("TP_stopAni", PhotonTargets.All, true);

                    if (lastTmpPos != correctPos.position)
                    {
                        tmpPos = points.GoComparing(enemyData.atk_Range, this.transform, correctPos, enemyData.width);
                        if (tmpPos != null)
                        {
                            points.RemoveThisPoint(correctPos);
                            //Debug.Log("我找到一個更近的囉");
                            correctPos = tmpPos;
                            points.AddPoint(correctPos);
                        }
                        lastTmpPos = correctPos.position;
                    }
                }

            }
            yield return null;
        }
    }

    [PunRPC]
    public void TP_stopAni(bool _t)
    {
        if (ani == null)
            ani = GetComponent<Animator>();
       /* if (t)
            nav.avoidancePriority = 10;
        else
            nav.avoidancePriority = 40;*/
        ani.SetBool(aniHashValue[1], _t);
    }
    #endregion

    #region 面對目標
    protected void rotToTarget()
    {
        atkDir = currentTarget.transform.position - transform.position;
        atkDir.y = transform.position.y;
        CharacterRot = Quaternion.LookRotation(atkDir.normalized);
        transform.rotation = CharacterRot;
    }
    #endregion

    #region 攻擊動畫判定開關
    public List<GameObject> alreadytakeDamage = new List<GameObject>();
    public virtual void changeCanHit(int c)
    {

    }
    #endregion

    /// <summary>
    /// 觀看武器大小
    /// </summary>
    //public GameObject TTTTEEEE;
    public bool canLookCheckRadius;
    private void OnDrawGizmos()
    {
        if (canLookCheckRadius)
            Gizmos.DrawWireSphere(transform.position, viewRadius);
       // TTTTEEEE.transform.position = sword_1.position;
        //TTTTEEEE.transform.rotation = sword_1.rotation;
    }

    #region 給與正確目標傷害
    protected virtual void giveCurrentDamage()
    {

    }
    #endregion

    #region 負面效果
    //暈眩
    [PunRPC]
    protected virtual void GetDeBuff_Stun(float _time)
    { }
    //緩速
    protected virtual void GetDeBuff_Slow()
    { }
    //破甲
    protected virtual void GetDeBuff_DestoryDef()
    { }
    //燒傷
    protected virtual void GetDeBuff_Burn()
    { }
    //擊退
    [PunRPC]
    protected virtual void pushOtherTarget(Vector3 _dir)
    { }
    //往上擊飛
    [PunRPC]
    protected virtual void HitFlayUp()
    { }

    //負面狀態恢復
    protected void Recover_Stun()
    {
        NowCC = false;
        ReturnState();
    }
    #endregion

    #region 時間延遲
    //每次攻擊間隔
    protected void delayTimeToAtk()
    {
        StartCoroutine(MatchTimeManager.SetCountDown(atkIsOk, enemyData.atk_delay));
    }
    void atkIsOk()
    {
        canAtking = true;
    }
    //被攻擊時反應時間
    protected void beAttackStop()
    {
        correctPos = transform;
        StartCoroutine(MatchTimeManager.SetCountDown(waitToAtk, enemyData.beAtk_delay));
    }
    void waitToAtk()
    {
        if (IFRightNowAtk())
            return;

        ReturnState();
    }
    //殺死目標後延遲
    protected void KillTargetDelay()
    {
        if (!canAtking)
            StartCoroutine(MatchTimeManager.SetCountDown(GoNextAxtion, waitNextActionTime));
        else
            GoNextAxtion();
    }
    void GoNextAxtion()
    {
        chaseTime = 1.5f;
        nowState = states.Wait_TargetDie;
        StartDetectT();
    }
    //關閉一切時間延遲
    void CloseThis(Coroutine _coroutine)
    {
        if (_coroutine != null)
        {
            StopCoroutine(_coroutine);
            _coroutine = null;
        }
    }
    public void closeAllDelay()
    {
        CloseThis(enemyAttack_time);
        CloseThis(sotpWait_time);        
    }
    #endregion

    #region 前往下個目標點
    public void getNextPoint()
    {
        if (photonView.isMine)
        {
            getTatgetPoint(agentPoints[nowPoint].transform.position);
            nextPos = false;
        }
    }

    public void touchPoint(int _i)
    {
        nowPoint = _i;
        if (nowPoint == 10 || nowPoint == -1)
        {
            nowState = states.Null;
          //  Debug.Log("已到達終點");
            return;
        }
        nextPos = true;
    }
    #endregion

    #region 判斷是否到目標點
    protected bool ifReach(Vector3 _targetPoint)
    {
        return ((_targetPoint - transform.position).sqrMagnitude < (enemyData.stoppingDst * enemyData.stoppingDst)) ? true : false;
    }
    #endregion

    #region 傷害
    [PunRPC]
    public void takeDamage(int _id, float _damage)
    {
        if (deadManager.checkDead)
            return;

        #region 反擊判斷
        if (photonView.isMine && _id != 0)
        {
            if (!firstAtk && !ifFirstAtkTarget())
            {
                isDead _isdead = PhotonView.Find(_id).gameObject.GetComponent<isDead>();

                if (_isdead.myAttributes != GameManager.NowTarget.Tower)
                {
                    if (points != null && correctPos != null)
                        points.RemoveThisPoint(correctPos);

                    firstAtk = true;
                    resetChaseTime();
                    if (!NowCC)
                    {
                        nowState = states.BeAtk;
                        nav.ResetPath();
                        if (!ani.GetBool(aniHashValue[1]))
                            Net.RPC("TP_stopAni", PhotonTargets.All, true);

                        currentTarget = _isdead.gameObject;
                        points = _isdead.gameObject.GetComponent<CreatPoints>();
                        targetDeadScript = _isdead;

                        beAttackStop();
                    }
                    else
                    {
                        currentTarget = _isdead.gameObject;
                        points = _isdead.gameObject.GetComponent<CreatPoints>();
                        targetDeadScript = _isdead;
                    }
                }
            }
        }
        #endregion
        MyHelath(CalculatorDamage(_damage));
    }

    protected virtual void MyHelath(float _damage)
    {
        //血量顯示與消失
        UI_HpObj.alpha = 1;
        CloseHP();

        //扣血
        if (enemyData.UI_HP > 0)
        {
            enemyData.UI_HP -= _damage;
            if (enemyData.UI_HP <= 0)
            {
                deadManager.ifDead(true);
                Death();
                UI_HpObj.alpha = 0;
            }
            BeHitChangeColor();
            Feedback();
            openPopupObject(_damage);
        }
    }

    #region 打中效果
    protected virtual void Feedback()
    {

    }

    protected void BeHitChangeColor()
    {
        if (maxValue == 0)
        {
            maxValue = 10;
            myRender.material.SetColor("_EmissionColor", new Color(255, 0, 0, maxValue));
            myRender.material.EnableKeyword("_EMISSION");
            StartCoroutine(OriginalColor());
        }
        else
        {
            maxValue = 10;
            myRender.material.SetColor("_EmissionColor", new Color(255, 0, 0, maxValue));
        }
    }

    IEnumerator OriginalColor()
    {
        while (maxValue > 0)
        {
            maxValue -= Time.deltaTime * 70;
            myRender.material.SetColor("_EmissionColor", new Color(255, 0, 0, maxValue));
            if (maxValue <= 0)
            {
                maxValue = 0;
                myRender.material.DisableKeyword("_EMISSION");
                yield break;
            }
            yield return null;
        }
    }
    #endregion
    #endregion

    #region 關閉血量顯示
    private float HP_Time = 5.5f;
    private Coroutine hpCoroutine;
    protected void CloseHP()
    {
        if (hpCoroutine == null)
            hpCoroutine = StartCoroutine(MatchTimeManager.SetCountDown(closeHpBar, HP_Time));
        else
        {
            StopCoroutine(hpCoroutine);
            hpCoroutine = StartCoroutine(MatchTimeManager.SetCountDown(closeHpBar, HP_Time));
        }
    }
    void closeHpBar()
    {
        UI_HpObj.alpha = 0;
    }
    #endregion

    #region 傷害顯示
    public void openPopupObject(float _damage)
    {
        FloatingTextController.instance.CreateFloatingText(_damage, this.transform);
        UI_HpBar.fillAmount = enemyData.UI_HP / enemyData.UI_MaxHp;
    }
    #endregion

    #region 計算傷害
    protected virtual float CalculatorDamage(float _damage)
    {
        return _damage;
    }
    #endregion

    #region 死亡
    protected void Death()
    {
        if (photonView.isMine)
        {
            StopCoroutine(findVisible);
            haveHit = false;
            alreadytakeDamage.Clear();
            cancelSelectTarget(true);
            SceneManager.RemoveMy_SoldierList(this);
            ifAtkMoveStop = false;
            firstAtk = false;
            canAtking = true;
            nextPos = false;
            nav.enabled = false;
        }
        else
        {
            SceneManager.RemoveEnemy_SoldierList(this);
        }
        myCollider.enabled = false;
        ani.CrossFade(aniHashValue[0], 0.02f, 0);
        Invoke("Return_ObjPool", 2.5f);
    }

    void Return_ObjPool()
    {
        if (photonView.isMine)
            ObjectPooler.instance.Repool(enemyData._soldierName, this.gameObject);
        else
            Net.RPC("SetActiveF", PhotonTargets.All);
    }
    #endregion

    #region 取消目標偵測
    void delayCancelTarget()
    {
        //目標死亡時等待時間
        if (nowState == states.Wait_TargetDie)
        {
            if (chaseTime > 0)
            {
                chaseTime -= Time.deltaTime;
            }
            else
            {
                if (nowTarget == GameManager.NowTarget.Null)
                {
                    getNextPoint();
                    nowState = states.Move;
                }
            }
        }
        else
        {
            //有目標且未死亡時
            if (chaseTime > 0)
            {
                if (targetDeadScript.checkDead)
                {
                    cancelSelectTarget(true);
                    KillTarget();
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
    #endregion

    #region 取消目標(尋找敵人方面)
    public void cancelSelectTarget(bool _now)
    {
        if (correctPos != null)
        {
            points.RemoveThisPoint(correctPos);
        }
        correctPos = null;        
        points = null;
        targetDeadScript = null;
        currentTarget = null;

        chaseTime = 0;
        nowTarget = GameManager.NowTarget.Null;
        closeAllDelay();
        firstAtk = false;
        ifAtkMoveStop = false;
        if (!_now)
        {
            chaseTime = 1.5f;
            nowState = states.Wait_TargetDie;
            StartDetectT();
        }
    }

    public void KillTarget()
    {
        nowState = states.Null;
        KillTargetDelay();

    }
    #endregion

    protected void StopAll()
    {
        if (nav.hasPath)
            nav.ResetPath();
        nowState = states.Null;
        ani.SetBool(aniHashValue[1], true);
    }
    //回歸時間
    public void resetChaseTime()
    {
        chaseTime = waitTime;
    }
    //判斷是否為優先目標
    public bool ifFirstAtkTarget()
    {
        return (nowTarget == GameManager.NowTarget.Core || (nowTarget != GameManager.NowTarget.Soldier && nowTarget == firstPriority)) ? true : false;
    }
}