using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using System;

public class PhotonNetManager : Photon.PunBehaviour
{
    
    private ExitGames.Client.Photon.Hashtable ddd;

    [PunRPC]
    public void getFirestPlayer(GameManager.MyNowPlayer _player)
    {
        gm.firstPlayer = _player;
    }

    #region Public Variables
    public static PhotonNetManager instance;
    [SerializeField] PhotonLogLevel Loglevel = PhotonLogLevel.Informational;
    [HideInInspector]
    public bool singlePeople = false;
    [HideInInspector]
    public byte MaxPlayersPerRoom = 2;//房間最大人數
    [HideInInspector]
    public int GoGameNumber = 2;//開始遊戲所需人數
    [SerializeField] float reciprocalTime;//遊戲倒數時間
    [SerializeField] GameObject HostI;
    [SerializeField] GameObject matchBtn_obj;
    [SerializeField] Button cancel_btn;
    [SerializeField] GameObject SignIn;
    #endregion

    #region Private Varlables
    string _gameVersion = "1";
    Button _matchBtn;
    Text _matchTxt;
    GameObject StartGametimer;
    GameManager gm;
    #endregion

    #region Photon.PunBehaviour CallBacks
    public override void OnConnectedToPhoton()
    {
        PhotonNetwork.AuthValues = new AuthenticationValues(Guid.NewGuid().ToString());
        Debug.Log("給我看看" + Guid.NewGuid());
        //SignIn.SetActive(true);
    }

    //換房主
    public override void OnMasterClientSwitched(PhotonPlayer newMasterClient)
    {
        Debug.Log("OnMasterClientSwitched");
        if (PhotonNetwork.isMasterClient && HostI != null)
            HostI.SetActive(true);
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("已連線");
        cancel_btn.interactable = true;
        //match按鈕初始
        _matchTxt.text = "隨機配對";
        if (gm.getMyPlayer() == GameManager.MyNowPlayer.Null)
        {
            _matchBtn.interactable = false;
            rock.SetActive(true);
        }
        else
        {
            _matchBtn.interactable = true;
        }
    }

    public override void OnPhotonRandomJoinFailed(object[] codeAndMsg)
    {
        Debug.Log("隨機加入房間失敗");

        PhotonNetwork.CreateRoom(null, new RoomOptions() { MaxPlayers = MaxPlayersPerRoom }, null);
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("開始配對");

        if (PhotonNetwork.isMasterClient)
            HostI.SetActive(true);

        if (PhotonNetwork.playerList.Length == GoGameNumber)
        {
            _matchTxt.text = "倒數中...";
        }
        else if (PhotonNetwork.playerList.Length < GoGameNumber)
        {
            _matchTxt.text = "等待玩家...";
        }
        else
        {
            PhotonNetwork.LeaveRoom();
        }

        //////////////////////////////////////////////////////
        if (singlePeople)
        {
            Debug.Log("玩家數量到達");
            _matchTxt.text = "遊戲倒數";

            if (StartGametimer == null)
            {
                StartGametimer = PhotonNetwork.Instantiate("StartGametimer", Vector3.zero, Quaternion.identity, 0, null);
            }
            else
            {
                StartGametimer.GetComponent<PhotonView>().RPC("SetActiveT", PhotonTargets.All);
            }
            timer = StartCoroutine(ReciprocalTimer());
        }
        ///////////////////////////////////////////////////
    }

    public override void OnPhotonMaxCccuReached()
    {
        PhotonNetwork.LeaveRoom();
    }

    public override void OnPhotonPlayerConnected(PhotonPlayer newPlayer)//除了新加入的都會呼叫
    {
        Debug.Log(newPlayer.NickName + "加入房間，目前人數" + PhotonNetwork.playerList.Length + "/" + GoGameNumber);

        if (PhotonNetwork.playerList.Length == GoGameNumber)
        {
            Debug.Log("玩家數量到達");

            _matchTxt.text = "倒數中...";

            if (PhotonNetwork.isMasterClient)
            {
                GetComponent<PhotonView>().RPC("getFirestPlayer", PhotonTargets.All, gm.getMyFirst());

                if (StartGametimer == null)
                {
                    StartGametimer = PhotonNetwork.Instantiate("StartGametimer", Vector3.zero, Quaternion.identity, 0, null);
                }
                else
                {
                    StartGametimer.GetComponent<PhotonView>().RPC("SetActiveT", PhotonTargets.All);
                }

                PhotonNetwork.automaticallySyncScene = true;
                timer = StartCoroutine(ReciprocalTimer());
            }
        }
    }

    public override void OnPhotonPlayerDisconnected(PhotonPlayer otherPlayer)//有人離開，除了離開的都會呼叫
    {
        if (StopMenu.instance != null)
        {
            StopMenu.instance.SurrenderClick();
            Debug.Log("SurrenderClick();");
        }

        Debug.Log(otherPlayer.NickName + "離開房間，目前人數" + PhotonNetwork.playerList.Length + "/" + GoGameNumber);

        if (_matchTxt != null)
        {
            _matchTxt.text = "等待玩家...";
        }
        ReciprocalTimeEnd();
    }

    public override void OnDisconnectedFromPhoton()
    {
        Debug.Log("伺服器連線已中斷");
        if (StopMenu.instance)
        {
            StopMenu.instance.SurrenderClick();
        }
        Application.Quit();
    }

    public override void OnLeftRoom()
    {
        HostI.SetActive(false);
        Debug.Log("取消配對");
    }
    #endregion

    #region Public Method
    [Header("按鈕變色")]
    [SerializeField] Button P1;
    [SerializeField] Button P2;
    [SerializeField] GameObject rock;
    public ColorBlock nowBtn = new ColorBlock();
    public ColorBlock orBtn = new ColorBlock();
    public void changeSelectColor(GameManager.MyNowPlayer _player)
    {
        rock.SetActive(false);
        if (_player == GameManager.MyNowPlayer.player_1)
        {
            P1.colors = nowBtn;
            P2.colors = orBtn;
        }
        else if (_player == GameManager.MyNowPlayer.player_2)
        {
            P1.colors = orBtn;
            P2.colors = nowBtn;
        }
    }

    public void Match()
    {
        if (!PhotonNetwork.inRoom && PhotonNetwork.connected)
        {
             PhotonNetwork.JoinRandomRoom();

            _matchBtn.interactable = false;
            if (gm.getMyFirst() == GameManager.MyNowPlayer.player_1)
            {
                P1.colors = nowBtn;
                P2.colors = orBtn;
            }
            else
            {
                P1.colors = orBtn;
                P2.colors = nowBtn;
            }
            P1.interactable = false;
            P2.interactable = false;
        }
        else
        {
            Debug.Log("我是基德，請稍等");
        }
    }

    public void CancelMatch()
    {
        if (PhotonNetwork.inRoom)
        {
            ReciprocalTimeEnd();
            PhotonNetwork.LeaveRoom();
            
            P1.interactable = true;
            P2.interactable = true;
        }
    }

    public void matchIn()
    {
        if (gm.getMyPlayer() == GameManager.MyNowPlayer.Null)
        {
            _matchBtn.interactable = false;
        }
        else
        {
            _matchBtn.interactable = true;
        }
    }
    #endregion

    #region Private Method
    private void Awake()
    {
        if (instance == null)
            instance = this;

        PhotonNetwork.automaticallySyncScene = true;
        PhotonNetwork.logLevel = Loglevel;

        DontDestroyOnLoad(gameObject);

        if (matchBtn_obj != null)
        {
            _matchTxt = matchBtn_obj.transform.Find("matchTxt").GetComponent<Text>();
            _matchBtn = matchBtn_obj.GetComponent<Button>();
        }
    }

    private void Start()
    {
        Connect();
        gm = GameManager.instance;
    }

    void Connect()
    {
        //執行連線
        if (PhotonNetwork.connected)
        {
            Debug.Log("PhotonNetwork.connected");
            if (PhotonNetwork.inRoom)
            {
                ReciprocalTimeEnd();
                PhotonNetwork.LeaveRoom();
            }
        }
        else
        {
            Debug.Log("no PhotonNetwork.connected");
            PhotonNetwork.ConnectUsingSettings(_gameVersion);
        }
    }

    Coroutine timer;
    IEnumerator ReciprocalTimer()
    {
        StartGametimer.GetComponent<Text>().text = reciprocalTime.ToString("0");
        float time = reciprocalTime;

        while (true)
        {
            yield return new WaitForEndOfFrame();
            time -= Time.fixedDeltaTime;
            StartGametimer.GetComponent<Text>().text = time.ToString("0");

            if (time <= 0)
            {
                cancel_btn.interactable = false;
                ReciprocalTimeEnd();
                if (singlePeople)
                {
                    PhotonNetwork.LoadLevelAsync(1);
                }
                else if (PhotonNetwork.playerList.Length >= GoGameNumber)
                {
                   // PhotonNetwork.LoadLevel(1);
                    PhotonNetwork.LoadLevelAsync(1);
                }
                yield break;
            }
        }
    }

void ReciprocalTimeEnd()//結束遊戲計時
    {
        if (timer != null)
        {
            StopCoroutine(timer);
            timer = null;
        }

        if (StartGametimer != null)
        {
            StartGametimer.GetComponent<Text>().text = reciprocalTime.ToString("0");
            StartGametimer.GetComponent<PhotonView>().RPC("SetActiveF", PhotonTargets.All);
        }
    }
    #endregion

    #region 回到主畫面
    [PunRPC]
    void OutGame()
    {
        if (PhotonNetwork.isMasterClient)
        {
            PhotonNetwork.LoadLevel(0);
        }
        Destroy(gm.gameObject);
        Destroy(this.gameObject);
    }
    #endregion
}
