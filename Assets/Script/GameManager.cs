using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : Photon.MonoBehaviour
{
    public static GameManager instance;
    
    //單人
    public Toggle singleToggle;
    public void IsSingleToggle()
    {
        PhotonNetManager manager = PhotonNetManager.instance;
        if (singleToggle.isOn)
        {
            manager.singlePeople = true;
            manager.MaxPlayersPerRoom = 1;
            manager.GoGameNumber = 1;
        }
        else
        {
            manager.singlePeople = false;
            manager.MaxPlayersPerRoom = 2;
            manager.GoGameNumber = 2;
        }
    }
    //
    private bool gameOver = false;
    public bool GameOver { get { return gameOver; } set { gameOver = value; } }


    public enum whichObject
    {
        None=0,
        Soldier_1=1,
        Soldier_2=2,
        //SoldierIcon_1=3,
        //SoldierIcon_2=4,
        soldier_Test = 13,

        Tower1_Cannon =5,
        Tower2_Wind=6,

        TowerDetect_Wind=7,

        HintText=8,
        Bullet_Normal=9,
        Bullet_Wind=10,
        popupText=11,
        TowerDetect_Cannon=12,
        Tower_Electricity=14,
        TowerDetect_Electricity=15,
        Tower1_Missile = 16,
        Bullet_Missile = 17,
        EIcon = 18,
        TowerIcon = 19,
        SoldierIcon = 20
    }

    public enum NowTarget
    {
        Null,
        Player,
        Soldier,
        Tower,
        Core,
        NoChange
    }

    public enum MyNowPlayer
    {
        Null,
        player_1,
        player_2
    }

    public enum meIs
    {
        Allen,
        Queen
    }
    public meIs Meis;
    public MyNowPlayer firstPlayer;
    public MyNowPlayer WhoMe = MyNowPlayer.Null;
    public MyNowPlayer getMyPlayer() { return WhoMe; }
    public MyNowPlayer getMyFirst() { return firstPlayer; }

    #region Mask
    private LayerMask targetMask_Player1 = 1 << 29 | 1 << 31;
    private LayerMask targetMask_Player2 = 1 << 28 | 1 << 30;
    public LayerMask getPlayer1_Mask { get { return targetMask_Player1; } }
    public LayerMask getPlayer2_Mask { get { return targetMask_Player2; } }

    public LayerMask correctMask(bool ismine)
    {
        if (ismine)
        {
            if (WhoMe == MyNowPlayer.player_1)
                return targetMask_Player1;
            else
                return targetMask_Player2;
        }
        else
        {
            if (WhoMe == MyNowPlayer.player_1)
                return targetMask_Player2;
            else
                return targetMask_Player1;
        }
    }
    #endregion
    //投降選單
    private GameObject surrenderMessage;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(this);
        
       DontDestroyOnLoad(this.gameObject);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F1))
        {
            if (StopMenu.instance == null)
            {
                Debug.LogWarning("沒有暫停選單啦 媽b");
            }
            else
            {
                if (surrenderMessage == null)
                    surrenderMessage = StopMenu.instance.stopMenuPrefab;
                else
                    surrenderMessage.SetActive(!surrenderMessage.activeInHierarchy);
            }
        }
    }

    [SerializeField]
    public void changeCurrentPlayer(string _Player)
    {
        switch (_Player)
        {
            case ("Player1"):
                WhoMe = MyNowPlayer.player_1;
                firstPlayer = MyNowPlayer.player_1;
                Meis = meIs.Allen;
                PhotonNetManager.instance.changeSelectColor(firstPlayer);
                break;
            case ("Player2"):
                WhoMe = MyNowPlayer.player_2;
                firstPlayer = MyNowPlayer.player_2;
                Meis = meIs.Queen;
                PhotonNetManager.instance.changeSelectColor(firstPlayer);
                break;
        }
    }
}