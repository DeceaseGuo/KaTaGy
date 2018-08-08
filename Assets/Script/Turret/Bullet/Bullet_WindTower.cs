using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Bullet_WindTower : Photon.MonoBehaviour
{
    [SerializeField] GameManager.whichObject DataName;
    private TurretData.TowerDataBase Data;
    private Vector3 dir;
    private float distanceThisFrame;
    private PhotonView Net;
    private Transform nowTarget;

    [Header("碰撞區")]
    [SerializeField] Vector3 pushBox_Size;
    [SerializeField] Vector3 offset;
    [SerializeField] Vector3 pushBox_Offset;
    public LayerMask pushMask;

    [Header("飛行與傷害間隔時間")]
    [SerializeField] float flyTime;
    [SerializeField] float fireCd = 0;
    public List<GameObject> tmpNoDamage;

    protected Tweener myTweener;

    private void Awake()
    {
        Data = TurretData.instance.getTowerData(DataName);
        Debug.Log(Data);
        Net = GetComponent<PhotonView>();
        getMask();
    }

    private void Update()
    {
        //bulletMove();
        /*if (!photonView.isMine)
        {
            DetectTarget();
        }*/
        DetectTarget();
    }

    #region 目前為玩家幾
    public void getMask()
    {
        GameManager _gm = GameManager.instance;
        if (photonView.isMine)
        {
            if(_gm.getMyPlayer() == GameManager.MyNowPlayer.player_1)
            {
                pushMask = _gm.getPlayer1_Mask;
            }
            else
            {
                pushMask = _gm.getPlayer2_Mask;
            }
        }
        else
        {
            if (_gm.getMyPlayer() == GameManager.MyNowPlayer.player_1)
            {
                pushMask = _gm.getPlayer2_Mask;
            }
            else
            {
                pushMask = _gm.getPlayer1_Mask;
            }
        }
    }

    /*public void checkCurrentPlay()
    {
        pushMask = GameManager.instance.correctMask;
    }*/
    #endregion

    #region 找到目標
    public void getTarget(Transform _target)
    {
        int viewID = _target.GetComponent<PhotonView>().viewID;
        Net.RPC("TP_Data", PhotonTargets.All, viewID);

        if (photonView.isMine)
            StartCoroutine("DisappearThis");
    }
    #endregion

    [PunRPC]
    public void TP_Data(int _id)
    {
        PhotonView _Photon = PhotonView.Find(_id);
        nowTarget = _Photon.transform;
        transform.LookAt(nowTarget);
        dir = nowTarget.position - transform.position;
        bulletMove();
    }    

    #region 子彈移動
    void bulletMove()
    {
        //子彈移動距離
         /*distanceThisFrame = Data.bullet_Speed * Time.deltaTime;
        //子彈移動
          transform.Translate(dir.normalized * distanceThisFrame, Space.World);
          //子彈朝前
          if (transform.position.y < 5)
          {
              dir.y = 0;
              Quaternion tmpRot = Quaternion.Euler(0, transform.localEulerAngles.y, transform.localEulerAngles.z);
              transform.rotation = Quaternion.Lerp(transform.rotation, tmpRot, .2f);
          }*/
        Vector3 _targetPoint = dir.normalized * Data.bullet_Speed * flyTime;
        _targetPoint.y = nowTarget.localPosition.y;
        myTweener = transform.DOBlendableMoveBy(_targetPoint, flyTime+.5f).SetEase(/*Ease.InOutQuart*/ Ease.InOutCubic);
        myTweener.OnUpdate(Reset_Rot);
    }
    #endregion

    public void Reset_Rot()
    {
        Quaternion tmpRot = Quaternion.Euler(0, transform.localEulerAngles.y, transform.localEulerAngles.z);
        transform.rotation = Quaternion.Lerp(transform.rotation, tmpRot, .1f);
    }

    #region 將碰到的全部擊退
    void DetectTarget()
    {
        Collider[] colliders = Physics.OverlapBox(transform.position + offset, pushBox_Size , Quaternion.identity, pushMask);

        foreach (Collider bePush_Obj in colliders)
        {
            isDead _who = bePush_Obj.gameObject.GetComponent<isDead>();
            if (_who == null)
                return;

            if (photonView.isMine)
            {
                giveDamage(bePush_Obj, _who);
            }
            else
            {
                //giveDamage(bePush_Obj, _who);
                if (_who.myAttributes != GameManager.NowTarget.Tower && _who.myAttributes != GameManager.NowTarget.Core)
                    bePush_Obj.transform.localPosition += dir.normalized * /*distanceThisFrame*/1.7f;
            }
        }
    }
    #endregion

    //扣血
    void giveDamage(Collider bePush_Obj, isDead _who)
    {
        if (!checkIf(bePush_Obj.gameObject))
        {
            PhotonView _TargetNet = bePush_Obj.GetComponent<PhotonView>();
            switch (_who.myAttributes)
            {
                case GameManager.NowTarget.Player:
                    _TargetNet.RPC("takeDamage", PhotonTargets.All, CalculatorDamage(), Vector3.zero, false);
                    break;
                case GameManager.NowTarget.Soldier:
                    _TargetNet.RPC("takeDamage", PhotonTargets.All, 0, CalculatorDamage());
                    break;
                case GameManager.NowTarget.Tower:
                    {
                        _TargetNet.RPC("takeDamage", PhotonTargets.All, CalculatorDamage());
                    }
                    break;
                case GameManager.NowTarget.Core:
                    break;
                default:
                    break;
            }
            tmpNoDamage.Add(bePush_Obj.gameObject);
            StartCoroutine(DelayDamage(bePush_Obj.gameObject));
        }
        Debug.Log("攻擊");
    }
    //計算傷害
    float CalculatorDamage()
    {
        return Data.Atk_Damage;
    }

    #region 檢查敵人是否在無傷害間隔區 
    bool checkIf(GameObject _enemy)
    {
        if (tmpNoDamage.Contains(_enemy))
            return true;
        else
            return false;
    }
    #endregion

    #region 離開無傷害間隔區
    IEnumerator DelayDamage(GameObject _enemy)
    {
        yield return new WaitForSeconds(fireCd);
        if (checkIf(_enemy))
            tmpNoDamage.Remove(_enemy);
    }
    #endregion

    #region 過一段時間後消失
    IEnumerator DisappearThis()
    {
        yield return new WaitForSeconds(flyTime);
        tmpNoDamage.Clear();
        returnBulletPool();
    }
    #endregion

    //返回物件池
    void returnBulletPool()
    {
        if (photonView.isMine)
        {
            ObjectPooler.instance.Repool(Data.bullet_Name, this.gameObject);
        }
    }

   /* void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(transform.position+offset, pushBox_Size );       
    }*/
}
