using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    private bool StopClick;

    public enum whichObject
    {
        None,
        Soldier_1,
        Soldier_2,
        SoldierIcon_1,
        SoldierIcon_2,
        Tower1_Cannon,
        Tower2_Wind,

        TowerDetect_Wind,

        HintText,
        Bullet_Normal,
        Bullet_Wind,
        popupText,
        TowerDetect_Cannon,
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

    public MyNowPlayer WhoMe = MyNowPlayer.Null;
    public MyNowPlayer getMyPlayer() { return WhoMe; }
    public MyNowPlayer getMyFirst() { return firstPlayer; }

    private LayerMask targetMask_Player1 = 1 << 29 | 1 << 31;
    private LayerMask targetMask_Player2 = 1 << 28 | 1 << 30;
    public LayerMask getPlayer1_Mask { get { return targetMask_Player1; } }
    public LayerMask getPlayer2_Mask { get { return targetMask_Player2; } }
    //投降選單
    private GameObject surrenderMessage;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        
       DontDestroyOnLoad(this.gameObject);
      //  changeCurrentPlayer(GameManager.MyNowPlayer.player_1);
    }

    private void Update()
    {
        if (BuildManager.instance != null && BuildManager.instance.nowSelect)
        {
            if (!StopClick)
            {
                if (Input.GetKeyDown(KeyCode.Tab))
                {
                    BuildManager.instance.BuildSwitch();
                    StopClick = true;
                }
            }
        }

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

    public void NowStop(bool _stop)
    {
        StopClick = _stop;
    }

    public void changeCurrentPlayer(string _Player)
    {
        switch (_Player)
        {
            case ("Player1"):
                WhoMe = MyNowPlayer.player_1;
                firstPlayer = MyNowPlayer.player_1;
                Meis = meIs.Allen;
                Debug.Log("p1");
                break;
            case ("Player2"):
                WhoMe = MyNowPlayer.player_2;
                firstPlayer = MyNowPlayer.player_2;
                Meis = meIs.Queen;
                Debug.Log("p2");
                break;
        }
    }
    public MyNowPlayer firstPlayer;
}