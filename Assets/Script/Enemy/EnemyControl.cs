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
    #endregion
    //數據
    public GameManager.whichObject DataName;
    public MyEnemyData.Enemies enemyData;
    public MyEnemyData.Enemies originalData;
    [HideInInspector]
    public isDead deadManager;  

    //尋找目標
    [SerializeField] protected GameManager.NowTarget firstPriority;
    protected GameManager.NowTarget nowTarget = GameManager.NowTarget.Null;

    [Tooltip("偵測半徑")]
    [SerializeField] protected float viewRadius;
    [Tooltip("追逐時間")]
    [SerializeField] protected float waitTime;
    //目前剩餘時間
    private float chaseTime = 0;
    //能攻擊對象layer
    public LayerMask currentMask;
    //正確目標
    protected GameObject currentTarget;
    //正確目標是否死亡腳本
    protected isDead targetDeadScript;

    private IEnumerator findVisible;
    public List<GameObject> myTarget;
    //尋路
    protected NavMeshAgent nav;
    protected RandomNodeManager randomNodeManager;
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
        Skill
    }
    public states nowState = states.Null;

    protected Quaternion CharacterRot;
    protected CapsuleCollider soldierCollider;
    protected Animator ani;
    protected bool haveHit;
    public Transform sword_1;
    protected Vector3 atkDir;

    //正確敵人位置腳本
    protected CreatPoints points;
    //正確敵人位置
    public Transform correctPos;

    public PhotonView Net;
    private void OnEnable()
    {
        myRender.material.SetFloat("Vector1_D655974D", 0);
        if (nav == null)
            nav = GetComponent<NavMeshAgent>();

        formatData();

        if (Net != null)
        {
            if (ani.GetBool("Stop"))
                Net.RPC("TP_stopAni", PhotonTargets.All, false);
            if (photonView.isMine)
            {
                StartDetectT();
                nav.enabled = true;
                nowState = states.Move;
                selectRoad();
                getNextPoint();
            }
        }
    }

    private void Start()
    {
        AtkDetectSet();
        randomNodeManager = GameObject.Find("RandomNodeManager").GetComponent<RandomNodeManager>();    
        Net = GetComponent<PhotonView>();
        ani = GetComponent<Animator>();
        if (photonView.isMine)
        {
            CreatPoints Cpoint = GetComponent<CreatPoints>();
            Cpoint.enabled = false;
            checkCurrentPlay();
            SetCoroutine();
        }
        nav.updateRotation = false;

        selectRoad();
        getNextPoint();
        nowState = states.Move;
    }

    private void Update()
    {
        if (!photonView.isMine)
            return;

        if (!deadManager.checkDead)
        {
            if (nowState != states.Atk && nowState != states.Wait_Move && nowState != states.Wait_TargetDie)
                DetectState();

            if ((nowState == states.Wait_TargetDie && nowTarget == GameManager.NowTarget.Null) || currentTarget != null)
                delayCancelTarget();
        }
    }

    #region 恢復初始數據
    void formatData()
    {
        if (deadManager == null)
        {
            deadManager = GetComponent<isDead>();
            soldierCollider = GetComponent<CapsuleCollider>();
        }
        else
        {
            if (photonView.isMine)
            {
                originalData = MyEnemyData.instance.getMySoldierData(DataName);
                SceneManager.AddMyList(gameObject, deadManager.myAttributes);
            }
            else
            {
                originalData = MyEnemyData.instance.getEnemySoldierData(DataName);
                SceneManager.AddEnemyList(gameObject, deadManager.myAttributes);
            }

            soldierCollider.enabled = true;
            deadManager.ifDead(false);
            enemyData = originalData;
        }
        if (nav != null)
            nav.speed = enemyData.moveSpeed;
        nowPoint = Find_PathPoint;
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
            if (deadManager.checkDead)
                StopCoroutine(findVisible);

            if (!firstAtk && !ifAtkMoveStop && !ifFirstAtkTarget())
            {
                SetFindT();
                if (myTarget.Count != 0)
                {
                    for (int i = 0; i < myTarget.Count; i++)
                    {
                        isDead _attributes = myTarget[i].GetComponent<isDead>();
                        if (!_attributes.checkDead)
                        {
                            getCurrentTarget(_attributes);
                        }
                    }
                }
            }
        });

        StartDetectT();
    }
    //給其他小兵更改
    protected virtual void SetFindT()
    {
        myTarget = SceneManager.CalculationDis(gameObject, viewRadius, false, false);
    }
    #endregion

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
                    //chaseTime = 9999;
                    return;
                }
                else if (nowTarget != firstPriority && _inform.myAttributes == firstPriority)
                {
                    CreatPoints tmpPoint = _inform.GetComponent<CreatPoints>();
                    if (!tmpPoint.CheckFull(enemyData.atk_Range))
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
                    if (!tmpPoint.CheckFull(enemyData.atk_Range))
                    {
                        nowTarget = firstPriority;
                        goAtkPos(tmpPoint, _inform);
                    }
                }
                else if (_inform.myAttributes == GameManager.NowTarget.Soldier)
                {
                    if (!tmpPoint.CheckFull(enemyData.atk_Range))
                    {
                        nowTarget = GameManager.NowTarget.Soldier;
                        goAtkPos(tmpPoint, _inform);
                    }
                }
            }
        }
    }
    #endregion

    #region 判斷並前往攻擊點
    public void goAtkPos(CreatPoints _tmpPoint, isDead _isdaed)
    {
        if (deadManager.checkDead)
            return;

        points = _tmpPoint;
        targetDeadScript = _isdaed;
        currentTarget = _isdaed.gameObject;
        if (nowTarget == GameManager.NowTarget.Null)
            nowTarget = _isdaed.myAttributes;

        Transform tmpPos = points.getPoint(enemyData.atk_Range, this.transform, enemyData.width, true);
        if (tmpPos == null)
        {
            cancelSelectTarget(true);
            return;
        }
        correctPos = tmpPos;
        points.TestNext.Add(correctPos);
        resetChaseTime();
        nowState = states.AtkMove;
    }

    /*protected void goAtkPos(isDead _isdaed)
    {
        if (deadManager.checkDead)
            return;
        CreatPoints tmpScript = _isdaed.GetComponent<CreatPoints>();
        goAtkPos(tmpScript, _isdaed);
    }*/
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

        Transform tmpPos = points.getPoint(enemyData.atk_Range, this.transform, enemyData.width, true);
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

        correctPos = tmpPos;
        points.TestNext.Add(correctPos);
        resetChaseTime();
        nowState = states.AtkMove;
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
    void selectRoad()
    {
        int i = Random.Range(0, 100);
        if (i < 50)
            agentPoints = randomNodeManager.getNodePoints_1;
        else
            agentPoints = randomNodeManager.getNodePoints_2;
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
                else                
                    cancelSelectTarget(false);                
                break;
            case (states.Skill):

                break;
            case (states.AtkWait):
                if (!ani.GetBool("Stop"))
                    Net.RPC("TP_stopAni", PhotonTargets.All, true);
                if (targetDeadScript != null && !targetDeadScript.checkDead)
                    goWaitAtkPos(enemyData.atk_Range);
                else
                    cancelSelectTarget(false);
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
            if (ani.GetBool("Stop"))
                Net.RPC("TP_stopAni", PhotonTargets.All, false);
            nav.SetDestination(_targetPoint);
        }
    }

    #region 士兵移動
    void enemyMove()
    {
       // stopNav.enabled = false;
        //nav.enabled = true;

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
           // if (!ifAtkMoveStop)
            {
                Transform ABC = points.GoComparing(enemyData.atk_Range, transform, correctPos, true);
                if (ABC != null)
                {
                    points.TestNext.Remove(correctPos);

                    points.TestNext.Add(ABC);
                    correctPos = ABC;
                }

                if (points != null && correctPos == null)
                {
                    //點不見了
                    nowState = states.AtkWait;
                    return;
                }
            }

            getTatgetPoint(correctPos.position);
            findNextPath();

            #region 判斷是否到攻擊目標點→否則執行移動
            if (ifReach(nav.destination))
            {
                if (!ani.GetBool("Stop"))
                    Net.RPC("TP_stopAni", PhotonTargets.All, true);

                rotToTarget();
                ifAtkMoveStop = true;

                if (canAtking && !deadManager.checkDead)
                {
                    points.AddPoint(enemyData.atk_Range, correctPos);
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
        Vector3 tmpNextPos = nav.steeringTarget - transform.position;
        tmpNextPos.y = transform.localPosition.y;

        if (tmpNextPos != Vector3.zero)
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
    protected Transform shortPos = null;
    protected IEnumerator stopWait()
    {
        while (nowState == states.Wait_Move)
        {
            if (currentTarget == null || deadManager.checkDead)
            {
                cancelSelectTarget(false);
                yield break;
            }

            float tmpNowDis_F = Vector3.Distance(this.transform.position, currentTarget.transform.position);

            //攻擊延遲到了
            if (canAtking && !deadManager.checkDead)
            {
                if (!points.IFDis(enemyData.atk_Range, tmpNowDis_F - enemyData.stoppingDst))
                {
                   // 不在攻擊區域→往攻擊移動                  
                    points.RemovePoint(enemyData.atk_Range, correctPos);
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
                    shortPos = correctPos;
                    points.RemovePoint(enemyData.atk_Range, correctPos);
                    OverAtkDis = true;
                }

                //超過攻擊範圍進行追趕
                if (OverAtkDis)
                {
                    Transform ABC = points.GoComparing(enemyData.atk_Range, this.transform, correctPos, false);
                    if (ABC != null)
                        shortPos = ABC;
                    //未到達範圍 →追趕
                    if (!ifReach(shortPos.position))
                    {
                        getTatgetPoint(shortPos.position);
                        findNextPath();
                        transform.rotation = Quaternion.Lerp(transform.rotation, CharacterRot, enemyData.rotSpeed);
                    }
                    else //到達範圍 →OverAtkDis=False
                    {
                        OverAtkDis = false;
                        correctPos = shortPos;
                        points.AddPoint(enemyData.atk_Range, correctPos);
                        shortPos = null;
                    }
                }
                else //沒超過攻擊範圍 士兵自動換點
                {
                    rotToTarget();
                    if (!ani.GetBool("Stop"))
                        Net.RPC("TP_stopAni", PhotonTargets.All, true);

                    Transform ABC = points.GoComparing(enemyData.atk_Range, this.transform, correctPos, false);
                    if (ABC != null)
                    {
                        //Debug.Log("我找到一個更近的囉");

                        points.RemovePoint(enemyData.atk_Range, correctPos);
                        correctPos = ABC;
                        points.AddPoint(enemyData.atk_Range, correctPos);
                    }
                }

            }
            yield return null;
        }
    }

    [PunRPC]
    public void TP_stopAni(bool t)
    {
        if (ani == null)
            ani = GetComponent<Animator>();
        if (t)
            nav.avoidancePriority = 10;
        else
            nav.avoidancePriority = 40;
        ani.SetBool("Stop", t);
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
    /*public GameObject TTTTEEEE;
    private void OnDrawGizmos()
    {
        TTTTEEEE.transform.position = sword_1.position;
        TTTTEEEE.transform.rotation = sword_1.rotation;
    }*/

    #region 給與正確目標傷害
    protected virtual void giveCurrentDamage(isDead _target)
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
    protected virtual void pushOtherTarget(Vector3 _dir, float _dis)
    { }
    //往上擊飛
    [PunRPC]
    protected virtual void HitFlayUp()
    { }

    //負面狀態恢復
    protected void Recover_Stun()
    {
        ani.SetBool("StunRock", false);
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
        StartCoroutine(MatchTimeManager.SetCountDown(waitToAtk, enemyData.beAtk_delay));
    }
    void waitToAtk()
    {
        nowState = states.AtkMove;
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
        nextPos = false;

        if (photonView.isMine)
        {
            Vector3 tmpPos = agentPoints[nowPoint].GetRandomPointInNodeArea();
            getTatgetPoint(tmpPos);
        }
    }

    public void touchPoint(int _i, bool _canMove)
    {
        nowPoint = _i;
        if (nowPoint == 10 || nowPoint == 0)
        {
            nowState = states.Null;
          //  Debug.Log("已到達終點");
            return;
        }
        nextPos = _canMove;
    }
    #endregion

    #region 判斷是否到目標點
    protected bool ifReach(Vector3 _targetPoint)
    {
        Vector3 maxDisGap = _targetPoint - transform.position;
        float maxDis = maxDisGap.sqrMagnitude;

        if (maxDis < Mathf.Pow(enemyData.stoppingDst, 2))
            return true;
        else
            return false;
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
                PhotonView _Photon = PhotonView.Find(_id);
                isDead _isdead = _Photon.gameObject.GetComponent<isDead>();

                if (_isdead.myAttributes != GameManager.NowTarget.Tower)
                {
                    CreatPoints tmpPoint = _isdead.gameObject.GetComponent<CreatPoints>();
                    firstAtk = true;
                    resetChaseTime();
                    if (!NowCC)
                    {
                        nowState = states.BeAtk;
                        goAtkPos(tmpPoint, _isdead);
                        beAttackStop();
                    }
                    else
                    {
                        currentTarget = _isdead.gameObject;
                        points = tmpPoint;
                        targetDeadScript = _isdead;
                    }
                }
            }
        }
        #endregion
        float tureDamage = CalculatorDamage(_damage);
        MyHelath(tureDamage);
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
        FloatingTextController.instance.CreateFloatingText(_damage.ToString("0.0"), this.transform);
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
            haveHit = false;
            alreadytakeDamage.Clear();
            cancelSelectTarget(true);
            SceneManager.RemoveMyList(gameObject, deadManager.myAttributes);
            ifAtkMoveStop = false;
            firstAtk = false;
            canAtking = true;

            nextPos = false;
            nav.enabled = false;
        }
        else
        {
            SceneManager.RemoveEnemyList(gameObject, deadManager.myAttributes);
        }
        soldierCollider.enabled = false;
        ani.SetTrigger("Death");

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
    #endregion

    #region 取消目標(尋找敵人方面)
    public void cancelSelectTarget(bool _now)
    {
        if (correctPos != null)
        {
            points.RemovePoint(enemyData.atk_Range, correctPos);
            points.TestNext.Remove(correctPos);
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
            chaseTime = 1f;
            nowState = states.Wait_TargetDie;
            StartDetectT();
        }
    }
    #endregion

    protected void StopAll()
    {
        nav.ResetPath();
        nowState = states.Null;
        ani.SetBool("Stop", false);
    }
    //回歸時間
    public void resetChaseTime()
    {
        chaseTime = waitTime;
    }
    //判斷是否為優先目標
    public bool ifFirstAtkTarget()
    {
        if (nowTarget == GameManager.NowTarget.Core || nowTarget == firstPriority)
            return true;
        else
            return false;
    }
}