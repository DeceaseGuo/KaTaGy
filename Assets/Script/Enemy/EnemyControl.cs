using MyCode.Timer;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;
using DG.Tweening;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(FieldOfView))]
[RequireComponent(typeof(isDead))]
public class EnemyControl : Photon.MonoBehaviour
{
    private SceneObjManager sceneObjManager;
    private SceneObjManager SceneManager { get { if (sceneObjManager == null) sceneObjManager = SceneObjManager.Instance; return sceneObjManager; } }

    //數據
    public GameManager.whichObject DataName;
    public MyEnemyData.Enemies enemyData;
    protected MyEnemyData.Enemies originalData;
    protected GameObject displayHpBarPos;
    [HideInInspector]
    public isDead deadManager;
    protected FieldOfView fieldOfView;    

    //尋找目標
    protected NavMeshAgent nav;
    protected NavMeshObstacle stopNav;
    protected RandomNodeManager randomNodeManager;

    //尋路
    public Node[] agentPoints;
    protected Transform targetPoint;
    protected int nowPoint;
    private int Find_PathPoint;
    public int NowPoint { get { return nowPoint; } }
    protected bool nextPos;
    //攻擊目標
    public Transform target;

    //血量
    private float maxValue;
    [SerializeField] Renderer myRender;
    [SerializeField] float height_Hpbar; //血條高度
    [SerializeField] CanvasGroup UI_HpObj;
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

    private void Awake()
    {
        displayHpBarPos = GameObject.Find("Display_HpBarPos");
        UI_HpObj.transform.SetParent(displayHpBarPos.transform, false);
    }

    private void Start()
    {
        randomNodeManager = GameObject.Find("RandomNodeManager").GetComponent<RandomNodeManager>();
        fieldOfView = GetComponent<FieldOfView>();        
        Net = GetComponent<PhotonView>();
        ani = GetComponent<Animator>();
        if (photonView.isMine)
        {            
            CreatPoints Cpoint = GetComponent<CreatPoints>();
            Cpoint.enabled = false;
            checkCurrentPlay();
        }
        else
        {            
            fieldOfView.enabled = false;
            nav.enabled = false;
            return;
        }
        nav.updateRotation = false;
        stopNav = GetComponent<NavMeshObstacle>();

        selectRoad();
        getNextPoint();
        nowState = states.Move;
    }

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
                fieldOfView.StartDetectT();
                stopNav.enabled = false;
                nav.enabled = true;
                nowState = states.Move;
                selectRoad();
                getNextPoint();
            }
        }
    }

    private void FixedUpdate()
    {
        displayHpBar();

        if (!photonView.isMine)
            return;

        if (!deadManager.checkDead)
        {
            if (nowState != states.Atk && nowState != states.Wait_Move)
                DetectState();

            if (haveHit && fieldOfView.targetDeadScript != null && !fieldOfView.targetDeadScript.checkDead)
                TouchTarget();
        }
    }

    #region 恢復初始數據
    void formatData()
    {
        if (deadManager == null)
        {
            deadManager = GetComponent<isDead>();
            soldierCollider = GetComponent<CapsuleCollider>();
            deadManager.ifDead(false);
        }
        else
        {
            soldierCollider.enabled = true;
            deadManager.ifDead(false);
            if (photonView.isMine)
            {
                SceneManager.AddMyList(gameObject, deadManager.myAttributes);
                nav.speed = enemyData.moveSpeed;
            }
            else
                SceneManager.AddEnemyList(gameObject, deadManager.myAttributes);
        }

        if (nav != null)
            nav.speed = enemyData.moveSpeed;

        originalData = MyEnemyData.instance.getEnemyData(DataName);
        enemyData = originalData;
        nowPoint = Find_PathPoint;
    }
    #endregion

    #region 血條跟隨怪物
    public void displayHpBar()
    {
      //  if (UI_HpObj.alpha == 1)
       // {
            Vector2 screenPos = Camera.main.WorldToScreenPoint(transform.position + new Vector3(0, height_Hpbar, 0));
            UI_HpObj.transform.position = screenPos;
       // }
    }
    #endregion

    #region 目前為玩家幾
    public void checkCurrentPlay()
    {
        if (GameManager.instance.getMyPlayer() == GameManager.MyNowPlayer.player_1)
        {
            Net.RPC("changeLayer", PhotonTargets.All, 28);
            fieldOfView.currentMask = GameManager.instance.getPlayer1_Mask;
            nowPoint = 0;
            Find_PathPoint = nowPoint;
        }
        else if (GameManager.instance.getMyPlayer() == GameManager.MyNowPlayer.player_2)
        {
            Net.RPC("changeLayer", PhotonTargets.All, 29);
            fieldOfView.currentMask = GameManager.instance.getPlayer2_Mask;
            nowPoint = 9;
            Find_PathPoint = nowPoint;
            
        }
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
                if (fieldOfView.targetDeadScript != null && !fieldOfView.targetDeadScript.checkDead)
                    enemyMove();
                break;
            case (states.Skill):

                break;
            case (states.AtkWait):
                if (!ani.GetBool("Stop"))
                    Net.RPC("TP_stopAni", PhotonTargets.All, true);
                if (fieldOfView.targetDeadScript != null && !fieldOfView.targetDeadScript.checkDead)
                {
                   // if (fieldOfView.points.CheckThisPointFull(fieldOfView.nowPoint) && !ifAtkMoveStop)
                        fieldOfView.goWaitAtkPos(enemyData.atk_Range);
                }
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
        stopNav.enabled = false;
        nav.enabled = true;

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
            if (!ifAtkMoveStop)
            {
                Transform ABC = fieldOfView.points.GoComparing(enemyData.atk_Range, this.transform, fieldOfView.nowPoint, true);
                if (ABC != null)
                {
                    fieldOfView.points.TestNext.Remove(fieldOfView.nowPoint);

                    fieldOfView.points.TestNext.Add(ABC);
                    fieldOfView.nowPoint = ABC;
                }

                if (fieldOfView.points != null && fieldOfView.nowPoint == null)
                {
                    //點不見了
                    nowState = states.AtkWait;
                    return;
                }
            }

            target = fieldOfView.currentTarget.transform;
             getTatgetPoint(fieldOfView.nowPoint.position);

            findNextPath();

            #region 判斷是否到攻擊目標點→否則執行移動
            if (ifReach(nav.destination))
            {
                rotToTarget();

                if (canAtking && !deadManager.checkDead)
                {
                    ifAtkMoveStop = true;
                    fieldOfView.points.AddPoint(enemyData.atk_Range, fieldOfView.nowPoint);
                    nowState = states.Atk;
                    nav.ResetPath();

                    fieldOfView.StopDetectT();
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
        tmpNextPos.y = 0;

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
            if (fieldOfView.currentTarget == null || deadManager.checkDead)
                yield break;

            float tmpNowDis_F = Vector3.Distance(this.transform.position, fieldOfView.currentTarget.transform.position);

            //攻擊延遲到了
            if (canAtking && !deadManager.checkDead)
            {
                if (!fieldOfView.points.IFDis(enemyData.atk_Range, tmpNowDis_F - enemyData.stoppingDst))
                {
                   // 不在攻擊區域→往攻擊移動                  
                    fieldOfView.points.RemovePoint(enemyData.atk_Range, fieldOfView.nowPoint);
                    ifAtkMoveStop = false;
                    nowState = states.AtkWait;
                   // fieldOfView.StartDetectT();
                    //fieldOfView.goWaitAtkPos(enemyData.atk_Range);
                    yield break;
                }
                else
                {
                    //在攻擊區內→進行攻擊
                    nowState = states.Atk;
                    fieldOfView.StopDetectT();
                    enemyAttack_time = StartCoroutine(enemyAttack());
                    yield break;
                }
            }
            else //攻擊延遲還沒到
            {
                //超過攻擊範圍時 →OverAtkDis=True
                if (!fieldOfView.points.IFDis(enemyData.atk_Range, tmpNowDis_F - enemyData.stoppingDst) && !OverAtkDis)
                {
                    shortPos = fieldOfView.nowPoint;
                    fieldOfView.points.RemovePoint(enemyData.atk_Range, fieldOfView.nowPoint);
                    stopNav.enabled = false;
                    nav.enabled = true;
                    OverAtkDis = true;
                }

                //超過攻擊範圍進行追趕
                if (OverAtkDis)
                {
                    Transform ABC = fieldOfView.points.GoComparing(enemyData.atk_Range, this.transform, fieldOfView.nowPoint, false);
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
                        fieldOfView.nowPoint = shortPos;
                        fieldOfView.points.AddPoint(enemyData.atk_Range, fieldOfView.nowPoint);
                        shortPos = null;
                    }
                }
                else //沒超過攻擊範圍 士兵不動自動換點
                {
                    nav.enabled = false;
                    stopNav.enabled = true;
                    rotToTarget();
                    if (!ani.GetBool("Stop"))
                        Net.RPC("TP_stopAni", PhotonTargets.All, true);

                    Transform ABC = fieldOfView.points.GoComparing(enemyData.atk_Range, this.transform, fieldOfView.nowPoint, false);
                    if (ABC != null)
                    {
                        //Debug.Log("我找到一個更近的囉");

                        fieldOfView.points.RemovePoint(enemyData.atk_Range, fieldOfView.nowPoint);
                        fieldOfView.nowPoint = ABC;
                        fieldOfView.points.AddPoint(enemyData.atk_Range, fieldOfView.nowPoint);
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
        ani.SetBool("Stop", t);
    }
    #endregion

    #region 面對目標
    protected void rotToTarget()
    {
        atkDir = target.position - transform.position;
        atkDir.y = 0;
        CharacterRot = Quaternion.LookRotation(atkDir.normalized);
        transform.rotation = CharacterRot;
    }
    #endregion

    #region 攻擊動畫判定開關
    public virtual void changeCanHit(int c)
    {

    }
    #endregion

    #region 攻擊是否打中
    public List<GameObject> alreadytakeDamage = new List<GameObject>();
    protected virtual void TouchTarget()
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
    protected virtual void GetDeBuff_Stun()
    {

    }
    //緩速
    protected virtual void GetDeBuff_Slow()
    {

    }
    //破甲
    protected virtual void GetDeBuff_DestoryDef()
    {

    }
    //燒傷
    protected virtual void GetDeBuff_Burn()
    {

    }
    //擊退
    [PunRPC]
    protected virtual void pushOtherTarget(Vector3 _dir, float _dis)
    {
        this.transform.DOMove(transform.localPosition + _dir.normalized * _dis, .8f).SetEase(Ease.OutBounce);
        Quaternion Rot = Quaternion.LookRotation(-_dir.normalized);
        this.transform.rotation = Rot;
    }
    #endregion

    #region 時間延遲
    //每次攻擊間隔
    protected void delayTimeToAtk()
    {
        StartCoroutine(Timer.StartRealtime(enemyData.atk_delay, () =>
        {
            canAtking = true;
        }));
    }
    //被攻擊時反應時間
    protected void beAttackStop()
    {
        StartCoroutine(Timer.StartRealtime(enemyData.beAtk_delay, () =>
        {
            nowState = states.AtkMove;
        }));
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

    //#region 取得傷害
    public bool firstAtk;
    public bool ifAtkMoveStop;

    #region 前往下個目標點
    public void getNextPoint()
    {
        nextPos = false;
        stopNav.enabled = false;
        nav.enabled = true;

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
            PhotonView _Photon = PhotonView.Find(_id);
            isDead _isdead = _Photon.gameObject.GetComponent<isDead>();

            if (_isdead.myAttributes != GameManager.NowTarget.Tower)
            {
                if (!firstAtk && !fieldOfView.ifFirstAtkTarget())
                {
                    CreatPoints tmpPoint = _isdead.gameObject.GetComponent<CreatPoints>();
                    firstAtk = true;
                    nowState = states.BeAtk;
                    fieldOfView.goAtkPos(tmpPoint, _isdead);
                    beAttackStop();
                    fieldOfView.resetChaseTime();
                }
            }
        }
        #endregion
        float tureDamage = CalculatorDamage(_damage);
        MyHelath(tureDamage);
    }

    void MyHelath(float _damage)
    {
        //血量顯示與消失
        if (UI_HpObj.alpha == 0 && HP_Time <= 0)
        {
            UI_HpObj.alpha = 1;
            HP_Time = 5f;
            StartCoroutine(closeHP());
        }
        else
            HP_Time = 5f;
        //扣血
        if (enemyData.UI_HP > 0)
        {
            enemyData.UI_HP -= _damage;
            if (enemyData.UI_HP <= 0)
            {
                deadManager.ifDead(true);
                Death();
                HP_Time = 0f;
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

    void BeHitChangeColor()
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

    private float HP_Time;
    protected IEnumerator closeHP()
    {
        while (true)
        {
            yield return new WaitForEndOfFrame();
            HP_Time -= Time.deltaTime;
            if (HP_Time <= 0 || deadManager.checkDead)
            {
                UI_HpObj.alpha = 0;
                HP_Time = 0f;
                yield break;
            }
        }
    }

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
    void Death()
    {
        StartCoroutine(Timer.Start_MoreFunction(0f, () =>
        {
            if (photonView.isMine)
            {
                haveHit = false;
                alreadytakeDamage.Clear();
                fieldOfView.cancelSelectTarget(true);
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
        }, 2.5f, () =>
        {
            if (photonView.isMine)
                ObjectPooler.instance.Repool(enemyData._soldierName, this.gameObject);
            else
                Net.RPC("SetActiveF", PhotonTargets.All);
        }));
    }
    #endregion
}