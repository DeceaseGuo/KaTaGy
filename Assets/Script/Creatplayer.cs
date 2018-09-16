using UnityEngine;
using UnityEngine.UI;

public class Creatplayer : Photon.MonoBehaviour
{
    #region 單例模式與取得單例
    public static Creatplayer instance;

    private EnemyManager enemyManager;
    private EnemyManager EnemyManagerScript { get { if (enemyManager == null) enemyManager = EnemyManager.instance; return enemyManager; } }
    #endregion

    private MatchTimer matchTime;
    private MatchTimer MatchTimeManager { get { if (matchTime == null) matchTime = MatchTimer.Instance; return matchTime; } }

    [SerializeField] Transform pos_1;
    [SerializeField] Transform pos_2;

    [SerializeField] Transform MyPlayer;
    [SerializeField] Text dieCD_Obj;

    //玩家控制腳本
    private Player player_Script;
    public Player Player_Script { get { return player_Script; } private set { player_Script = value; } }

    //出生位置
    private Vector3 myPosition;

    string player_Allen = "Player_Allen";
    string player_Queen = "Player_Queen";
    string pool_Allen1 = "PoolManager_Allen1";
    string pool_Allen2 = "PoolManager_Allen2";
    string pool_Queen1 = "PoolManager_Queen1";
    string pool_Queen2 = "PoolManager_Queen2";

    private void Awake()
    {
        PhotonNetwork.isMessageQueueRunning = true;
        if (instance == null)
            instance = this;
    }

    private void Start()
    {

        if (PhotonNetwork.isMasterClient)
        {
            // Debug.Log("本機端");
            MasterClient();
        }
        else
        {
            // Debug.Log("客戶端");
            SecondClient();
        }
    }

    #region 目前為玩家幾
    public void MasterClient()
    {
         if (GameManager.instance.getMyPlayer() == GameManager.MyNowPlayer.player_1)
         {
            //第一區
            born_P(player_Allen, pos_1, pool_Allen1);
         }
         else if (GameManager.instance.getMyPlayer() == GameManager.MyNowPlayer.player_2)
         {
            //第二區
            born_P(player_Queen, pos_2/*pos_1.position*/, pool_Queen2);
         }
    }

    public void SecondClient()
    {
        if (GameManager.instance.getMyPlayer() == GameManager.MyNowPlayer.player_1)
        {
            if (GameManager.instance.getMyFirst() == GameManager.MyNowPlayer.player_1)
            {
                //第二區
                GameManager.instance.WhoMe = GameManager.MyNowPlayer.player_2;
                born_P(player_Allen,pos_2/*pos_1.position*/, pool_Allen2);
            }
            else
            {
                //第一區
                born_P(player_Allen,pos_1, pool_Allen1);
            }

        }
        else if (GameManager.instance.getMyPlayer() == GameManager.MyNowPlayer.player_2)
        {
            if (GameManager.instance.getMyFirst() == GameManager.MyNowPlayer.player_1)
            {
                //第二區
                born_P(player_Queen,pos_2, pool_Queen2);
            }
            else
            {
                //第一區
                GameManager.instance.WhoMe = GameManager.MyNowPlayer.player_1;
                born_P(player_Queen, pos_1, pool_Queen1);
            }
        }
    }
    #endregion

    void born_P(string player, Transform _pos, string poolNumber)
    {
        myPosition = _pos.localPosition;
        EnemyManagerScript.CorrectBornPoint = _pos;
        GameObject myNowPlayer = PhotonNetwork.Instantiate("Prefabs/Player/" + player, _pos.localPosition, Quaternion.identity, 0);
        Instantiate(Resources.Load("Prefabs/ObjectPool/" + poolNumber), Vector3.zero, Quaternion.identity);
        myNowPlayer.transform.SetParent(MyPlayer);
        //GameManager.instance.changeNowMask();
        Player_Script = myNowPlayer.GetComponent<Player>();
    }

    #region 玩家重生
    public void player_ReBorn(float _countDown)
    {
        if (!dieCD_Obj.gameObject.activeInHierarchy)
            dieCD_Obj.gameObject.SetActive(true);

        StartCoroutine(MatchTimeManager.SetCountDown(ReBorn, _countDown, dieCD_Obj, null));
    }

    void ReBorn()
    {
        CameraEffect.instance.nowDie(false);
        dieCD_Obj.gameObject.SetActive(false);
        Player_Script.Net.RPC("SetActiveT", PhotonTargets.All, myPosition);
    }
    #endregion
}
