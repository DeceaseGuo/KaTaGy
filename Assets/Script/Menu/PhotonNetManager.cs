using System.Collections;
using System.Collections.Generic;
using ExitGames.Client.Photon;
using UnityEngine;
using UnityEngine.UI;

public class PhotonNetManager : Photon.PunBehaviour
{
    [PunRPC]
    public void getFirestPlayer(GameManager.MyNowPlayer _player)
    {
        GameManager.instance.firstPlayer = _player;
    }

    #region Public Variables
    public static PhotonNetManager instance;
    public PhotonLogLevel Loglevel = PhotonLogLevel.Informational;
    public byte MaxPlayersPerRoom;//房間最大人數
    public int GoGameNumber;//開始遊戲所需人數
    public float reciprocalTime;//遊戲倒數時間
    public GameObject HostI;
    public GameObject matchBtn_obj;
    public GameObject SignIn;
    #endregion

    #region Private Varlables
    string _gameVersion = "1";
    Button _matchBtn;
    Text _matchTxt;
    GameObject StartGametimer;
    #endregion

    #region Photon.PunBehaviour CallBacks
    public override void OnConnectedToPhoton()
    {
        SignIn.SetActive(true);
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

        //match按鈕初始
        _matchTxt.text = "隨機配對";
        if (GameManager.instance.getMyPlayer() == GameManager.MyNowPlayer.Null)
        {
            _matchBtn.interactable = false;
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

    public override void OnCreatedRoom()
    {
        Debug.Log("創建房間");
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
        /* if (PhotonNetwork.playerList.Length >= GoGameNumber)
         {
             Debug.Log("玩家數量到達");
             _matchTxt.text = "遊戲倒數";

             if (PhotonNetwork.isMasterClient)
             {
                 GetComponent<PhotonView>().RPC("getFirestPlayer", PhotonTargets.All, GameManager.instance.getMyFirst());

                 if (StartGametimer == null)
                 {
                     StartGametimer = PhotonNetwork.Instantiate("StartGametimer", Vector3.zero, Quaternion.identity, 0, null);
                 }
                 else
                 {
                     StartGametimer.GetComponent<PhotonView>().RPC("SetactiveRPC", PhotonTargets.All, true);
                 }
                 timer = StartCoroutine(ReciprocalTimer());
             }
         }*/
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
                GetComponent<PhotonView>().RPC("getFirestPlayer", PhotonTargets.All, GameManager.instance.getMyFirst());

                if (StartGametimer == null)
                {
                    StartGametimer = PhotonNetwork.Instantiate("StartGametimer", Vector3.zero, Quaternion.identity, 0, null);
                }
                else
                {
                    StartGametimer.GetComponent<PhotonView>().RPC("SetActiveRPC", PhotonTargets.All, true);
                }

                timer = StartCoroutine(ReciprocalTimer());
            }
        }
    }

    public override void OnPhotonPlayerDisconnected(PhotonPlayer otherPlayer)//有人離開，除了離開的都會呼叫
    {
        if (GameManager.instance != null)
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

        StopMenu.instance.SurrenderClick();
        Application.Quit();
    }

    public override void OnLeftRoom()
    {
        HostI.SetActive(false);
        Debug.Log("取消配對");
    }
    #endregion

    #region Public Method
    public void Match()
    {
        if (!PhotonNetwork.inRoom)
        {
            PhotonNetwork.JoinRandomRoom();
            _matchBtn.interactable = false;
        }
    }

    public void CancelMatch()
    {
        if (PhotonNetwork.inRoom)
        {
            ReciprocalTimeEnd();
            PhotonNetwork.LeaveRoom();
        }
    }

    public void matchIn()
    {
        if (GameManager.instance.getMyPlayer() == GameManager.MyNowPlayer.Null)
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
            time -= Time.deltaTime;
            StartGametimer.GetComponent<Text>().text = time.ToString("0");

            if (time <= 0)
            {
                ReciprocalTimeEnd();
                if (PhotonNetwork.playerList.Length >= GoGameNumber)
                {
                    PhotonNetwork.LoadLevel(1);
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
            StartGametimer.GetComponent<PhotonView>().RPC("SetActiveRPC", PhotonTargets.All, false);
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
        Destroy(GameManager.instance.gameObject);
        Destroy(PhotonNetManager.instance.gameObject);
    }
    #endregion
}
