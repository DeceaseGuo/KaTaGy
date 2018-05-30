using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet_WindTower : Photon.MonoBehaviour
{
    private TurretData.TowerDataBase Data;
    private Vector3 dir;
    private float distanceThisFrame;

    [Header("碰撞區")]
    [SerializeField] Vector3 pushBox_Size;
    [SerializeField] Vector3 offset;
    [SerializeField] Vector3 pushBox_Offset;
    [SerializeField] LayerMask pushMask;
    [Header("飛行與傷害間隔時間")]
    [SerializeField] float flyTime;
    [SerializeField] float fireCd = 0;
    public List<GameObject> tmpNoDamage;

    private void OnEnable()
    {
        if(photonView.isMine)
            checkCurrentPlay();
    }

    private void Update()
    {
        bulletMove();
        DetectTarget();
    }

    #region 目前為玩家幾
    public void checkCurrentPlay()
    {
        if (GameManager.instance.getMyPlayer() == GameManager.MyNowPlayer.player_1)
            pushMask = GameManager.instance.getPlayer1_Mask;
        else if (GameManager.instance.getMyPlayer() == GameManager.MyNowPlayer.player_2)
            pushMask = GameManager.instance.getPlayer2_Mask;
    }
    #endregion

    #region 找到目標
    public void getTarget(Transform _target,TurretData.TowerDataBase _turretData)
    {
        Data = _turretData;
        transform.LookAt(_target);        
        dir = _target.position - transform.position;
        StartCoroutine("DisappearThis");
    }
    #endregion

    #region 子彈移動
    void bulletMove()
    {
        //子彈移動距離
        distanceThisFrame = Data.bullet_Speed * Time.deltaTime;
        //子彈移動
        transform.Translate(dir.normalized * distanceThisFrame, Space.World);
        //子彈朝前
        if (transform.position.y < 5)
        {
            dir.y = 0;
            Quaternion tmpRot = Quaternion.Euler(0, transform.localEulerAngles.y, transform.localEulerAngles.z);
            transform.rotation = Quaternion.Lerp(transform.rotation, tmpRot, .2f);
        }
    }
    #endregion

    #region 將碰到的全部擊退
    void DetectTarget()
    {
        Collider[] colliders = Physics.OverlapBox(transform.position + offset, pushBox_Size , Quaternion.identity, pushMask);

        foreach (Collider bePush_Obj in colliders)
        {
            isDead _who = bePush_Obj.gameObject.GetComponent<isDead>();
            if (_who == null)
                return;

            if (_who.myAttributes != GameManager.NowTarget.Tower && _who.myAttributes != GameManager.NowTarget.Core)
                bePush_Obj.gameObject.transform.position += dir.normalized * distanceThisFrame * .7f;

            if (!checkIf(bePush_Obj.gameObject))
            {
                switch (_who.myAttributes)
                {
                    case GameManager.NowTarget.Player:
                        bePush_Obj.gameObject.GetComponent<PhotonView>().RPC("takeDamage", PhotonTargets.All, Data.Atk_Damage);
                        break;
                    case GameManager.NowTarget.Soldier:
                        bePush_Obj.gameObject.GetComponent<PhotonView>().RPC("takeDamage", PhotonTargets.All, 0, Data.Atk_Damage);
                        break;
                    case GameManager.NowTarget.Tower:
                        break;
                    case GameManager.NowTarget.Core:
                        break;
                    default:
                        break;
                }
                tmpNoDamage.Add(bePush_Obj.gameObject);
                StartCoroutine(DelayDamage(bePush_Obj.gameObject));
            }
        }
    }
    #endregion

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
        if(checkIf(_enemy))
            tmpNoDamage.Remove(_enemy);
    }
    #endregion

    #region 過一段時間後消失
    IEnumerator DisappearThis()
    {
        yield return new WaitForSeconds(flyTime);
        returnBulletPool();
    }
    #endregion

    //返回物件池
    void returnBulletPool()
    {
        ObjectPooler.instance.Repool(Data.bullet_Name, this.gameObject);
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(transform.position+offset, pushBox_Size );       
    }
}
