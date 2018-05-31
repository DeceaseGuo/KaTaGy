using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class PlayerAni : Photon.MonoBehaviour
{
    protected SmoothFollow cameraControl;
    protected Player player;
    protected Animator anim;

    [Header("武器")]
    public GameObject weapon;
    public Transform swordRecyclePos;
    public Transform pullSwordPos;
    public Transform weapon_Detect;
    public Transform weapon_Detect_Hand;
    public LayerMask canAtkMask;

    [Header("Combo")]    
    public bool canClick = true;
    public bool nextComboBool;

    protected int comboIndex;
    protected bool startDetect_1 = false;
    protected bool startDetect_2 = false;
    protected bool startDetect_3 = false;
    public Transform effectPos;
    public GameObject[] swordLight = new GameObject[3];
    public List<GameObject> alreadyDamage;

    protected Vector3 currentAtkDir;

    private void Start()
    { 
        anim = GetComponent<Animator>();
        player = GetComponent<Player>();

        cameraControl = SmoothFollow.instance;

        if (photonView.isMine)
        {
            checkCurrentPlay();
        }
       /* else if (!photonView.isMine)
        {
            this.enabled = false;
        }*/
    }

    #region 改變正確玩家
    void checkCurrentPlay()
    {
        if (GameManager.instance.getMyPlayer() == GameManager.MyNowPlayer.player_1)
        {
            player.Net.RPC("changeLayer", PhotonTargets.All, 30);
            canAtkMask = GameManager.instance.getPlayer1_Mask;
            farDistance += GameManager.instance.getPlayer1_Mask;
            player.Net.RPC("changeMask_1", PhotonTargets.Others);
        }
        else if (GameManager.instance.getMyPlayer() == GameManager.MyNowPlayer.player_2)
        {
            player.Net.RPC("changeLayer", PhotonTargets.All, 31);
            canAtkMask = GameManager.instance.getPlayer2_Mask;
            farDistance += GameManager.instance.getPlayer2_Mask;
            player.Net.RPC("changeMask_2", PhotonTargets.Others);
        }
    }
    [PunRPC]
    public void changeMask_1()
    {
        canAtkMask = GameManager.instance.getPlayer1_Mask;
        farDistance += GameManager.instance.getPlayer1_Mask;
    }
    [PunRPC]
    public void changeMask_2()
    {
        canAtkMask = GameManager.instance.getPlayer2_Mask;
        farDistance += GameManager.instance.getPlayer2_Mask;
    }
    #endregion

    #region 武器切換
    public void switchWeapon_Pattren(bool _change)
    {
        GetComponent<PhotonView>().RPC("weaponOC", PhotonTargets.All, _change);
    }

    [PunRPC]
    public void weaponOC(bool _t)
    {
        anim.SetBool("NowBuild", _t);
        anim.SetTrigger("Switch");
    }

    public void WeaponChangePos(int _n)
    {
        switch (_n)
        {
            //武器回背上
            case(0):                
                weapon.transform.SetParent(swordRecyclePos);                
                break;
            //武器回手上
            case (1):
                weapon.transform.SetParent(pullSwordPos);                
                break;
            //玩家不可移動
            case (2):
                player.stopAnything_Switch(true);
                break;
            //玩家可移動,模式可在切換
            case (3):
                player.stopAnything_Switch(false);
                GameManager.instance.NowStop(false);
                break;
        }
    }
    #endregion

    #region Combo
    //按下判斷
    public virtual void TypeCombo(Vector3 atkDir)
    {
        
    }
    //動畫播放間判定
    public virtual void comboCheck(int _n)
    {

    }
    //角色攻擊位移
    public virtual void GoMovePos()
    {

    }
    #endregion

    #region 偵測攻擊最遠範圍
    public Transform detectStartPos;
    public float maxDistance;
    public LayerMask farDistance;
    protected  Vector3 DetectAtkDistance(Vector3 _point)
    {        
        RaycastHit hit;
        if (Physics.BoxCast(detectStartPos.position, new Vector3(2f, 4, 0.2f), detectStartPos.forward, out hit, detectStartPos.rotation, maxDistance, farDistance))
        {
            Vector3 dir = hit.point - transform.position;
            dir.y = 0;
            Vector3 tmpPos = new Vector3(hit.point.x, .2f, hit.point.z) - dir.normalized * 4.5f;

            Vector3 maxDisGap = player.gameObject.transform.position - tmpPos;
            float maxDis = maxDisGap.sqrMagnitude;

            Vector3 willDisGap = player.gameObject.transform.position - _point;
            float willDis = willDisGap.sqrMagnitude;
            if (maxDis > willDis)
            {
                return _point;
            }
            else
            {
                return tmpPos;
            }
        }
        else
            return _point;
    }
    #endregion

    //攻擊區間傷害判斷
    public virtual void DetectAtkRanage()
    {

    }
    //給予正確目標傷害
    protected virtual void GetCurrentTarget(Collider[] _enemies)
    {
        
    }
    //目前傷害判定區及刀光特效
    public virtual void SwitchAtkRange(int _n)
    {

    }

    #region 檢查敵人是否已得到傷害 
    protected bool checkIf(GameObject _enemy)
    {
        if (alreadyDamage.Contains(_enemy))
            return true;
        else
            return false;
    }
    #endregion


    //觀看傷害碰撞
    public GameObject test;
    public GameObject test2;
    public GameObject test888;
    public GameObject testFinal;
    private void OnDrawGizmos()
    {
        test.transform.position = weapon_Detect.position;
        test.transform.rotation = weapon_Detect.rotation;

        testFinal.transform.position = weapon_Detect.position;
        testFinal.transform.rotation = weapon_Detect.rotation;

        test2.transform.position = weapon_Detect_Hand.position;
        test2.transform.rotation = weapon_Detect_Hand.rotation;

        test888.transform.position = detectStartPos.position;
        test888.transform.rotation = detectStartPos.rotation;

        /*Gizmos.color = Color.red;
        Gizmos.DrawLine(detectStartPos.position, detectStartPos.position+ detectStartPos.forward* maxDistance);*/
       
    }

    public void Ani_Run(bool isRun)
    {
        anim.SetBool("Run", isRun);        
    }
}
