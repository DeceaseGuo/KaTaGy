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
    //數據
    public GameManager.whichObject DataName;
    public MyEnemyData.Enemies enemyData;
    protected MyEnemyData.Enemies originalData;
    protected GameObject displayHpBarPos;
    protected isDead deadManager;
    protected FieldOfView fieldOfView;

    //尋找目標
    protected NavMeshAgent nav;
    protected NavMeshObstacle stopNav;
    protected RandomNodeManager randomNodeManager;

    //尋路
    public Node[] agentPoints;
    protected Transform targetPoint;
    protected int nowPoint;
    protected bool nextPos;

    //攻擊目標
    public Transform target;

    //血量
    public float height_Hpbar;
    public GameObject UI_HpObj;
    public Image UI_HpBar;

    public enum states
    {
        Null,
        Move,
        AtkMove,
        Atk,
        AtkWait,
        Wait_Move,
        BeAtk,
        Die,
        Wait_TargetDie,
        Skill
    }
    public states nowState = states.Null;

    protected CharacterController Chara;
    protected Vector3 MoveDir = Vector3.zero;
    protected Transform nextTargetRot;
    protected Quaternion CharacterRot;

    protected Animator ani;
    protected bool haveHit;
    public Transform sword_1;

    private void Awake()
    {
        displayHpBarPos = GameObject.Find("Display_HpBarPos");
        UI_HpObj.transform.SetParent(displayHpBarPos.transform, false);
    }

    private void Start()
    {        
        formatData();
        randomNodeManager = GameObject.Find("RandomNodeManager").GetComponent<RandomNodeManager>();
        nav = GetComponent<NavMeshAgent>();
        stopNav = GetComponent<NavMeshObstacle>();

        nav.updateRotation = false;

        Chara = GetComponent<CharacterController>();
        ani = GetComponent<Animator>();
        MoveDir = transform.forward;
        nextTargetRot = this.transform;
        checkCurrentPlay();
        selectRoad();
        getNextPoint();
        nowState = states.Move;
    }

    protected PhotonView Net;
    private void OnEnable()
    {
        deadManager = GetComponent<isDead>();
        Net = GetComponent<PhotonView>();
        Net.RPC("NowDeath", PhotonTargets.All, false);
        fieldOfView = GetComponent<FieldOfView>();

        if (!photonView.isMine)
        {
            fieldOfView.enabled = false;
           // this.enabled = false;
            return;
        }

        if (enemyData._soldierName != GameManager.whichObject.None)
        {
            fieldOfView.startDetectT();
            nowState = states.Move;
            ani.SetBool("Stop", false);
            stopNav.enabled = false;
            nav.enabled = true;
            selectRoad();
            getNextPoint();
        }
    }

    private void FixedUpdate()
    {
        displayHpBar();
        if (!photonView.isMine)
            return;
        
       // Net.RPC("displayHpBar", PhotonTargets.All);

        if (!deadManager.checkDead)
        {

            DetectState();
            if (fieldOfView.targetDeadScript != null && !fieldOfView.targetDeadScript.checkDead)
                TouchTarget();
        }
    }

    #region 恢復初始數據
    void formatData()
    {
        originalData = MyEnemyData.instance.getEnemyData(DataName);
        enemyData = originalData;
        Net.RPC("show_HPBar", PhotonTargets.All, 1.0f);
    }
    #endregion

    #region 血條跟隨怪物
   // [PunRPC]
    public void displayHpBar()
    {
        if (UI_HpObj.activeInHierarchy)
        {
            Vector2 screenPos = Camera.main.WorldToScreenPoint(transform.position + new Vector3(0, height_Hpbar, 0));
            UI_HpObj.transform.position = screenPos;
        }
    }
    #endregion

    #region 目前為玩家幾
    public void checkCurrentPlay()
    {
        if (!photonView.isMine)
            return;

        if (GameManager.instance.getMyPlayer() == GameManager.MyNowPlayer.player_1)
        {
            Net.RPC("changeLayer", PhotonTargets.All, 28);
            fieldOfView.currentMask = GameManager.instance.getPlayer1_Mask;
            nowPoint = 0;
        }
        else if (GameManager.instance.getMyPlayer() == GameManager.MyNowPlayer.player_2)
        {
            Net.RPC("changeLayer", PhotonTargets.All, 29);
            fieldOfView.currentMask = GameManager.instance.getPlayer2_Mask;
            nowPoint = 9;
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
        if (nowState == states.Die)
            return;

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
                Debug.Log("怪物這找點啦  西瓜");
                ani.SetBool("Stop", true);
                if (fieldOfView.targetDeadScript != null && !fieldOfView.targetDeadScript.checkDead)
                    fieldOfView.goWaitAtkPos(enemyData.atk_Range);
                break;
            default:
                return;
        }
    }
    #endregion

    public void getTatgetPoint(Vector3 _targetPoint)
    {
        if (nav != null && nav.enabled != false)
            nav.SetDestination(_targetPoint);
    }

    #region 士兵移動
    void enemyMove()
    {
        stopNav.enabled = false;
        nav.enabled = true;

        if (nowState == states.Move)
        {
            findNextPath();

            #region 判斷是否到最終目標點→否則執行移動
            if (nextPos && ifReach(nav.destination))
            {
                getNextPoint();
            }
            else
            {
                transform.rotation = Quaternion.Lerp(transform.rotation, CharacterRot, enemyData.rotSpeed);
                Chara.Move(MoveDir * enemyData.moveSpeed * Time.deltaTime);
            }
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

                if (fieldOfView.nowPoint == null)
                {
                    Debug.Log("點被消掉了啦 媽b");

                    fieldOfView.goWaitAtkPos(enemyData.atk_Range);
                    return;
                }
            }

            target = fieldOfView.currentTarget.transform;
            getTatgetPoint(/*target.position*/fieldOfView.nowPoint.position);


            findNextPath();

            #region 判斷是否到攻擊目標點→否則執行移動
            if (ifReach(nav.destination))
            {
                rotToTarget();
                if (canAtking)
                {
                    ifAtkMoveStop = true;
                    nowState = states.Atk;
                    nav.ResetPath();
                    fieldOfView.points.AddPoint(enemyData.atk_Range, fieldOfView.nowPoint);
                    enemyAttack_time = StartCoroutine(enemyAttack());
                }
            }
            else
            {
                transform.rotation = Quaternion.Lerp(transform.rotation, CharacterRot, enemyData.rotSpeed);
                Chara.Move(MoveDir * enemyData.moveSpeed * Time.deltaTime);

                /*  float tmpNowDis_F = Vector3.Distance(this.transform.position, fieldOfView.currentTarget.transform.position);
                  if (fieldOfView.points.IFDis(enemyData.atk_Range, tmpNowDis_F - enemyData.stoppingDst) && canAtking)
                  {
                      ifAtkMoveStop = true;
                      nowState = states.Atk;
                      nav.ResetPath();
                      fieldOfView.points.AddPoint(enemyData.atk_Range, fieldOfView.nowPoint);
                      StartCoroutine(enemyAttack());
                  }*/
            }
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
        {
            CharacterRot = Quaternion.LookRotation(tmpNextPos);
            nextTargetRot.rotation = CharacterRot;
            MoveDir = nextTargetRot.forward;
        }
    }
    #endregion

    #region 小兵攻擊
    protected Coroutine enemyAttack_time;
    protected virtual IEnumerator enemyAttack()
    {
        yield return null;
    }
    #endregion

    [PunRPC]
    public void getAtkAnimator()
    {
        ani= GetComponent<Animator>();
        ani.SetTrigger("Atk");
    }

    #region 攻擊後間隔
    protected bool OverAtkDis = false;
    protected Transform shortPos = null;
    protected Coroutine stopWait_time;
    protected IEnumerator stopWait()
    {
        while (nowState == states.Wait_Move && nowState != states.Die)
        {
            float tmpNowDis_F = Vector3.Distance(this.transform.position, fieldOfView.currentTarget.transform.position);

            if (canAtking)
            {
                if (!fieldOfView.points.IFDis(enemyData.atk_Range, tmpNowDis_F - enemyData.stoppingDst))
                {
                    Debug.Log("你就他媽不在攻擊區");
                    ani.SetBool("Stop", false);
                    fieldOfView.points.RemovePoint(enemyData.atk_Range, fieldOfView.nowPoint);
                    ifAtkMoveStop = false;
                    nowState = states.AtkWait; 
                    fieldOfView.goWaitAtkPos(enemyData.atk_Range);
                }
                else
                {
                    nowState = states.Atk;
                    enemyAttack_time = StartCoroutine(enemyAttack());
                }
            }
            else
            {
                //超過攻擊範圍test=true
                if (!fieldOfView.points.IFDis(enemyData.atk_Range, tmpNowDis_F - enemyData.stoppingDst) && !OverAtkDis)
                {
                    shortPos = fieldOfView.nowPoint;
                    fieldOfView.points.RemovePoint(enemyData.atk_Range, fieldOfView.nowPoint);
                    ani.SetBool("Stop", false);
                    stopNav.enabled = false;
                    nav.enabled = true;
                    OverAtkDis = true;
                }

                if (OverAtkDis)
                {
                    Transform ABC = fieldOfView.points.GoComparing(enemyData.atk_Range, this.transform, fieldOfView.nowPoint, false);
                    if (ABC != null)
                        shortPos = ABC;


                    if (!ifReach(shortPos.position))
                    {
                        getTatgetPoint(shortPos.position);
                        findNextPath();
                        transform.rotation = Quaternion.Lerp(transform.rotation, CharacterRot, enemyData.rotSpeed);
                        Chara.Move(MoveDir * enemyData.moveSpeed * Time.deltaTime);

                    }
                    else
                    {
                        OverAtkDis = false;
                        fieldOfView.nowPoint = shortPos;
                        fieldOfView.points.AddPoint(enemyData.atk_Range, fieldOfView.nowPoint);
                        shortPos = null;
                    }
                }
                else
                {
                    nav.enabled = false;
                    stopNav.enabled = true;
                    rotToTarget();
                    ani.SetBool("Stop", true);

                    Transform ABC = fieldOfView.points.GoComparing(enemyData.atk_Range, this.transform, fieldOfView.nowPoint, false);
                    if (ABC != null)
                    {
                        Debug.Log("我找到一個更近的囉");

                        fieldOfView.points.RemovePoint(enemyData.atk_Range, fieldOfView.nowPoint);
                        fieldOfView.nowPoint = ABC;
                        fieldOfView.points.AddPoint(enemyData.atk_Range, fieldOfView.nowPoint);
                    }
                }

            }
            yield return null;
        }
    }
    #endregion

    #region 面對目標
    protected void rotToTarget()
    {
        Vector3 AtkDir = target.position - transform.position;
        AtkDir.y = 0;
        CharacterRot = Quaternion.LookRotation(AtkDir.normalized);
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
    }
    #endregion

    protected Coroutine delayToAtk_time;
    #region 每次攻擊間隔
    [HideInInspector] public bool canAtking = true;
    protected IEnumerator delayTimeToAtk(float _time)
    {
        yield return new WaitForSeconds(_time);
        canAtking = true;
    }
    #endregion

    protected Coroutine beAttackStop_time;
    #region 被攻擊時延遲
    protected IEnumerator beAttackStop(float _time)
    {
        yield return new WaitForSeconds(_time);
        nowState = states.AtkMove;
    }
    #endregion

    //#region 取得傷害
    public bool firstAtk;
    public bool ifAtkMoveStop;
    //被攻擊後延遲
    public void beAtkTo()
    {
        beAttackStop_time = StartCoroutine(beAttackStop(enemyData.beAtk_delay));
    }

    #region 前往下個目標點
    public void getNextPoint()
    {
        ani.SetBool("Stop", false);
        nextPos = false;
        stopNav.enabled = false;
        nav.enabled = true;

        Vector3 tmpPos = agentPoints[nowPoint].GetRandomPointInNodeArea();
        getTatgetPoint(tmpPos);
    }

    public void touchPoint(int _i, bool _canMove)
    {
        nowPoint = _i;
        if (nowPoint == 10)
        {
            Debug.Log("已到達終點");
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

    #region 關閉一切時間延遲   
    protected void isTimeToClose(Coroutine _time)
    {
        if (_time != null)
        {
            StopCoroutine(_time);
            _time = null;
        }
    }

    public void closeAllDelay()
    {
        isTimeToClose(stopWait_time);
        isTimeToClose(enemyAttack_time);
        isTimeToClose(delayToAtk_time);
        isTimeToClose(beAttackStop_time);
    }
    #endregion

    #region 傷害
    [PunRPC]
    public void takeDamage(int _id, float _damage)
    {
        if (nowState == states.Die)
            return;

        #region 反擊判斷
        if (_id != 0 && photonView.isMine)
        {
            PhotonView _Photon = PhotonView.Find(_id);
            isDead _isdead = _Photon.gameObject.GetComponent<isDead>();


            if (_isdead.myAttributes != GameManager.NowTarget.Tower)
            {
                if (!firstAtk && !fieldOfView.ifFirstAtkTarget())
                {
                    firstAtk = true;

                    nowState = states.BeAtk;
                    fieldOfView.goAtkPos(_Photon.gameObject);
                    beAtkTo();
                    fieldOfView.resetChaseTime();
                }
            }
        }
        #endregion
        float tureDamage = CalculatorDamage(_damage);
        if (photonView.isMine)
        {
            Net.RPC("switch_HPObj", PhotonTargets.All, true);
            if (enemyData.UI_HP > 0)
            {
                enemyData.UI_HP -= tureDamage;
            }
            if (enemyData.UI_HP <= 0)
            {
                Net.RPC("NowDeath", PhotonTargets.All, true);
                Net.RPC("switch_HPObj", PhotonTargets.All, false);
            }
        }

        //UI_HpObj.SetActive(true);
        openPopupObject(tureDamage);
    }
    #endregion

    #region 傷害顯示
    void openPopupObject(float _damage)
    {
        FloatingTextController.instance.CreateFloatingText(_damage.ToString("0.0"), this.transform);
        if (!photonView.isMine)
            return;
        float _value = enemyData.UI_HP / enemyData.UI_MaxHp;
        Net.RPC("show_HPBar", PhotonTargets.All, _value);
    }
    #endregion

    #region 計算傷害
    protected virtual float CalculatorDamage(float _damage)
    {
        return _damage;
    }
    #endregion

    //同步血量
    [PunRPC]
    public void show_HPBar(float _value)
    {
        UI_HpBar.fillAmount = _value;
    }
    //同步死亡
    [PunRPC]
    public void NowDeath(bool _t)
    {
        //UI_HpObj.transform.SetParent(this.transform, false);
        deadManager.ifDead(_t);
        if (_t)
            StartCoroutine(Death());
    }

    [PunRPC]
    public void switch_HPObj(bool _t)
    {
        UI_HpObj.SetActive(_t);
    }

    #region 死亡
    IEnumerator Death()
    {
        ani.StopPlayback();
        if (photonView.isMine)
        {
            haveHit = false;
            alreadytakeDamage.Clear();
            fieldOfView.cancelSelectTarget();
            EnemyManager.instance.RemoveEnemy(this);
            nowState = states.Die;
            closeAllDelay();
        }

        ifAtkMoveStop = false;
        firstAtk = false;
        canAtking = true;
        nextPos = false;
        ani.SetTrigger("Death");
        yield return new WaitForSeconds(1.3f);
        formatData();
        EnemyManager.instance.returnPoolObject(enemyData._soldierName, this.gameObject);
    }
    #endregion
}