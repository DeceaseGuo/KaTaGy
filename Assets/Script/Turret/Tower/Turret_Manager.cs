using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
namespace AtkTower
{
    [RequireComponent(typeof(isDead))]
    public class Turret_Manager : Photon.MonoBehaviour
    {
        #region 取得單例
        private FloatingTextController floatTextCon;
        protected FloatingTextController FloatTextCon { get { if (floatTextCon == null) floatTextCon = FloatingTextController.instance; return floatTextCon; } }

        private MatchTimer matchTime;
        protected MatchTimer MatchTimeManager { get { if (matchTime == null) matchTime = MatchTimer.Instance; return matchTime; } }

        private SceneObjManager sceneObjManager;
        protected SceneObjManager SceneManager { get { if (sceneObjManager == null) sceneObjManager = SceneObjManager.Instance; return sceneObjManager; } }

        private ObjectPooler poolManager;
        protected ObjectPooler PoolManager { get { if (poolManager == null) poolManager = ObjectPooler.instance; return poolManager; } }
        #endregion

        //數據
        public GameManager.whichObject DataName;
        protected TurretData.TowerDataBase turretData;
        protected TurretData.TowerDataBase originalTurretData;
        private CreatPoints MyCreatPoints;
        protected Transform myCachedTransform;
        private bool firstGetData = true;
        public int GridNumber;
        public Electricity power;
        //242 235 0
        //255 142 81
        public Color overHeatColor;
        public Color orininalColor;
        //是否能開火
        private bool canFire = true;

        //正確目標
        private GameObject tmpTarget;
        protected Transform target;
        protected isDead targetDeadScript;
        [Header("位置")]
        public Transform Pos_rotation;
        public Transform Pos_attack;

        //旋轉方向所需
        private Quaternion lookRotation;
        private Vector3 rotationEuler;
        private float tmpAngle;

        //是否超過塔防的距離
        protected float distanceToEnemy;

        [Header("UI部分")]
        public Image Fad_energyBar;

        protected isDead deadManager;
        protected PhotonView Net;

        private void Update()
        {
            //死亡
            if (deadManager.checkDead)
                return;

            //減少熱能
            if (turretData.Fad_thermalEnergy > 0)
                overHeat();            

            //沒電、過熱
            if (turretData.Fad_overHeat || power == null || power.resource_Electricity < 0)
                return;            

            if (target == null)
            {
                FindEnemy();
            }
            else
            {
                LockOnTarget();
            }
        }

        #region 恢復初始數據
        protected void FirstformatData()
        {
            firstGetData = false;
            Net = GetComponent<PhotonView>();
            myCachedTransform = this.transform;
            MyCreatPoints = GetComponent<CreatPoints>();

            if (deadManager == null)
                deadManager = GetComponent<isDead>();

            if (photonView.isMine)
            {
                MyCreatPoints.enabled = false;
                originalTurretData = TurretData.instance.getTowerData(DataName);
                checkCurrentPlay();
            }
            else
            {
                MyCreatPoints.ProdecePoints(myCachedTransform);
                originalTurretData = TurretData.instance.getEnemyTowerData(DataName);
                this.enabled = false;
            }
        }

        protected void FormatData()
        {
            if (photonView.isMine)
            {
                SceneManager.AddMy_TowerList(gameObject);
                if (originalTurretData.ATK_Level != TurretData.myTowerAtkLevel || originalTurretData.DEF_Level != TurretData.myTowerDefLevel)
                    originalTurretData = TurretData.instance.getTowerData(DataName);
            }
            else
            {
                SceneManager.AddEnemy_TowerList(gameObject);
                if (originalTurretData.ATK_Level != TurretData.enemyTowerAtkLevel || originalTurretData.DEF_Level != TurretData.enemyTowerDefLevel)
                    originalTurretData = TurretData.instance.getEnemyTowerData(DataName);
            }

            turretData = originalTurretData;
            deadManager.ifDead(false);
            turretData.UI_Hp = turretData.UI_maxHp;
            turretData.Fad_thermalEnergy = 0;

            healthBar.fillAmount = 1;
            Fad_energyBar.fillAmount = 0.0f;
        }

        public void GoFormatData()
        {
            //myRender.material.SetFloat("Vector1_D655974D", 0);

            //(true物件池生成會先第一次執行一次)
            //false從物件池哪出後執行
            if (!firstGetData)
                FormatData();
            else
                FirstformatData();
        }
        #endregion

        #region 目前為玩家幾
        public void checkCurrentPlay()
        {
            if (GameManager.instance.getMyPlayer() == GameManager.MyNowPlayer.player_1)
            {
                Net.RPC("changeLayer", PhotonTargets.All, 30);
            }
            else if (GameManager.instance.getMyPlayer() == GameManager.MyNowPlayer.player_2)
            {
                Net.RPC("changeLayer", PhotonTargets.All, 31);
            }
        }
        #endregion

        public int GetMyElectricity()
        {
            return originalTurretData.cost_Electricity;
        }

        #region 尋找敵人
        public void FindEnemy()
        {
            tmpTarget = SceneManager.CalculationDis(myCachedTransform, turretData.Atk_Range, turretData.Atk_MinRange);

            if (tmpTarget != null)
            {
                target = tmpTarget.transform;
                targetDeadScript = target.GetComponent<isDead>();
            }
        }
        #endregion

        #region 偵測是否死亡與超出攻擊範圍
        bool DetectTarget()
        {
            if (targetDeadScript.checkDead)
            {
                target = null;
                return false;
            }
            else
            {
                distanceToEnemy = Vector3.SqrMagnitude(target.position - myCachedTransform.position);

                if (distanceToEnemy > turretData.Atk_Range * turretData.Atk_Range || distanceToEnemy < turretData.Atk_MinRange * turretData.Atk_MinRange)
                {
                    target = null;
                    return false;
                }
            }
            return true;
        }
        #endregion

        #region 朝向敵方目標與偵測開火
        void LockOnTarget()
        {
            //轉向
            lookRotation = Quaternion.LookRotation(target.position - Pos_attack.position);
            rotationEuler = Quaternion.Lerp(Pos_rotation.rotation, lookRotation, Time.deltaTime * 10).eulerAngles;
            Pos_rotation.rotation = Quaternion.Euler(0, rotationEuler.y, 0f);
            
            // Debug.Log("角度" + tmpAngle);            
            if (canFire)
            {
                tmpAngle = Quaternion.Angle(Pos_rotation.rotation, lookRotation);
                if (DetectTarget() && tmpAngle < 30)
                {
                    canFire = false;
                    Tower_shoot();
                    Invoke("EndCountDown", turretData.Atk_Gap);
                }
            }
        }

        public void EndCountDown()
        {
            canFire = true;
        }
        #endregion

        #region 攻擊函式(射擊生成子彈)
        protected virtual void Tower_shoot()
        {
            addHeat(1.0f);
            Fad_energyBar.fillAmount = turretData.Fad_thermalEnergy / turretData.Fad_maxThermalEnergy;
            //物件池生成子彈                                                                               //取得子彈管理            //傳送目標與傷害
            PoolManager.getPoolObject(turretData.bullet_Name, Pos_attack.position, Pos_attack.rotation).GetComponent<BulletManager>().getTarget(target, turretData.Atk_Damage);
        }
        #endregion

        #region 熱能處理
        void overHeat()
        {
            reduceHeat((!turretData.Fad_overHeat) ? 1.0f : turretData.Over_downSpd);
            Fad_energyBar.fillAmount = turretData.Fad_thermalEnergy / turretData.Fad_maxThermalEnergy;
        }
        #endregion

        [PunRPC]
        public void OverheatChange(bool _nowOverHeat)
        {
            if (_nowOverHeat)
                Fad_energyBar.color = overHeatColor;
            else
                Fad_energyBar.color = orininalColor;
        }

        #region 增加減少熱能
        //減少
        public void reduceHeat(float _speed)
        {
            turretData.Fad_thermalEnergy -= (turretData.Fad_decreaseRate * Time.deltaTime * _speed);

            if (turretData.Fad_thermalEnergy <= 0)
            {
                turretData.Fad_thermalEnergy = 0;
                turretData.Fad_overHeat = false;
                Net.RPC("OverheatChange", PhotonTargets.All, turretData.Fad_overHeat);
            }
        }
        //增加
        public void addHeat(float _speed)
        {
            turretData.Fad_thermalEnergy += (turretData.Fad_oneEnergy * _speed);

            if (turretData.Fad_thermalEnergy >= turretData.Fad_maxThermalEnergy)
            {
                turretData.Fad_thermalEnergy = turretData.Fad_maxThermalEnergy;
                turretData.Fad_overHeat = true;
                Net.RPC("OverheatChange", PhotonTargets.All, turretData.Fad_overHeat);
            }
        }
        #endregion

        #region 傷害
        public Image healthBar;
        [PunRPC]
        public void takeDamage(float _damage)
        {
            if (deadManager.checkDead)
                return;

            MyHelath(CalculatorDamage(_damage));
        }

        private void MyHelath(float _damage)
        {
            if (turretData.UI_Hp > 0)
            {
                turretData.UI_Hp -= _damage;
                if (turretData.UI_Hp <= 0)
                {
                    deadManager.ifDead(true);
                    Death();
                }
                openPopupObject(_damage);
            }
        }

        #endregion

        #region 傷害顯示
        void openPopupObject(float _damage)
        {
            FloatTextCon.CreateFloatingText(_damage, myCachedTransform);
            healthBar.fillAmount = turretData.UI_Hp / turretData.UI_maxHp;
        }
        #endregion

        #region 計算傷害
        protected  float CalculatorDamage(float _damage)
        {
            return _damage;
        }
        #endregion

        #region 死亡
        protected void Death()
        {
            if (photonView.isMine)
            {
                SceneManager.RemoveMy_TowerList(gameObject);
                BuildManager.instance.obtaniElectricity(this);
            }
            else
            {
                SceneManager.RemoveEnemy_TowerList(gameObject);
            }

            Invoke("Return_ObjPool", 1.5f);
        }
        #endregion

        #region 返回物件池
        protected void Return_ObjPool()
        {
            if (photonView.isMine)
                poolManager.Repool(DataName, this.gameObject);
            else
                gameObject.SetActive(false);
        }
        #endregion

        #region 過熱同步
        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            if (stream.isWriting)
            {
                stream.SendNext(Fad_energyBar.fillAmount);
              //  stream.SendNext(Pos_rotation.localEulerAngles);
            }
            else
            {
                Fad_energyBar.fillAmount = (float)stream.ReceiveNext();
                //Pos_rotation.localEulerAngles = (Vector3)stream.ReceiveNext();
            }
        }
        #endregion

        /*  public void OnDrawGizmos()
          {
              Gizmos.color = Color.red;
              Gizmos.DrawWireSphere(transform.position, turretData.Atk_Range);
              if (target != null)
              {
                  Gizmos.DrawLine(Pos_attack.position, target.position);
              }
              Gizmos.color = Color.blue;
              Gizmos.DrawWireSphere(transform.position, turretData.Atk_MinRange);
          }*/
    }
}
