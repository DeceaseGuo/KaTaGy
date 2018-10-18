using UnityEngine;

public class BulletManager : Photon.MonoBehaviour
{
    private MatchTimer matchTime;
    protected MatchTimer MatchTimeManager { get { if (matchTime == null) matchTime = MatchTimer.Instance; return matchTime; } }

    public GameManager.whichObject bulletName;
    public float bullet_Speed;
    protected float atkDamage;
    protected PhotonView Net;
    protected LayerMask atkMask;
    protected bool hit;
    public bool Isfllow = false;
    //目標
    protected isDead targetDead;
    protected PhotonView targetNet;

    //移動所需
    protected Vector3 targetPos;
    protected Vector3 dir;
    public float targetOffsetY;
    protected float distanceThisFrame;

    protected Transform enemyCachedTransform;
    protected Transform myCachedTransform;

    private void Awake()
    {
        Net = GetComponent<PhotonView>();        
        distanceThisFrame = bullet_Speed * Time.deltaTime;
        myCachedTransform = this.transform;

        if (photonView.isMine)
            checkCurrentPlay();
    }

    #region 目前為玩家幾
    public void checkCurrentPlay()
    {
        if (GameManager.instance.getMyPlayer() == GameManager.MyNowPlayer.player_1)
        {
            atkMask = GameManager.instance.getPlayer1_Mask;
        }
        else if (GameManager.instance.getMyPlayer() == GameManager.MyNowPlayer.player_2)
        {
            atkMask = GameManager.instance.getPlayer2_Mask;
        }
    }
    #endregion

    #region 取得目標，外部使用
    public void getTarget(Transform _target ,float _damage)
    {        
        hit = false;
       /* if (_target == null)
        {
            print("沒有目標");
            returnBulletPool();
            return;
        }*/
        atkDamage = _damage;
        int viewID = _target.GetComponent<PhotonView>().viewID;
        Net.RPC("TP_Data", PhotonTargets.All, viewID);
    }
    #endregion

    [PunRPC]
    public virtual void TP_Data(int _id)
    {
        targetNet = PhotonView.Find(_id);
        targetDead = targetNet.GetComponent<isDead>();
        enemyCachedTransform = targetDead.transform;
        targetPos = enemyCachedTransform.position;
        targetPos.y += targetOffsetY;
        dir = targetPos - myCachedTransform.position;
    }

    #region 子彈移動
    protected virtual void BulletMove()
    {
        if (Isfllow)
        {
            targetPos = enemyCachedTransform.position;
            targetPos.y = enemyCachedTransform.position.y + targetOffsetY;
            dir = targetPos - myCachedTransform.position;
            myCachedTransform.LookAt(targetPos);
        }

        myCachedTransform.Translate(dir.normalized * distanceThisFrame, Space.World);
    }
    #endregion

    #region 給予傷害
    protected virtual void GiveDamage()
    {
        if (targetDead.checkDead)
            return;

        switch (targetDead.myAttributes)
        {
            case (GameManager.NowTarget.Soldier):
                targetNet.RPC("takeDamage", PhotonTargets.All, 0, atkDamage);
                break;
            case (GameManager.NowTarget.Player):
                targetNet.RPC("takeDamage", PhotonTargets.All, atkDamage, Vector3.zero, false);
                break;
            case (GameManager.NowTarget.Tower):
                targetNet.RPC("takeDamage", PhotonTargets.All, atkDamage);
                break;
            case (GameManager.NowTarget.Core):
                break;
            default:
                return;
        }
    }
    #endregion

    #region 風砲位移
    protected virtual void MoveTarget()
    {
        print("位移囉");
    }
    #endregion

    #region 返回物件池
    protected void returnBulletPool()
    {
        if (photonView.isMine)
        {
            ObjectPooler.instance.Repool(bulletName, this.gameObject);
        }
        else
        {
            gameObject.SetActive(false);
        }
    }
    #endregion
}