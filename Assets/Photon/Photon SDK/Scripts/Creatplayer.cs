using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Creatplayer : Photon.MonoBehaviour
{
    [SerializeField] Transform pos_1;
    [SerializeField] Transform pos_2;

    [SerializeField] Transform MyPlayer;

    string player_Allen = "Player_Allen";
    string player_Queen = "Player_Queen";
    string pool_Allen1 = "PoolManager_Allen1";
    string pool_Allen2 = "PoolManager_Allen2";
    string pool_Queen1 = "PoolManager_Queen1";
    string pool_Queen2 = "PoolManager_Queen2";


    private void Start()
    {
        if (PhotonNetwork.isMasterClient)
        {
            Debug.Log("本機端");
            MasterClient();
        }
        else
        {
            Debug.Log("客戶端");
            SecondClient();
        }
    }

    #region 目前為玩家幾
    public void MasterClient()
    {
         if (GameManager.instance.getMyPlayer() == GameManager.MyNowPlayer.player_1)
         {
            //第一區
            born_P(player_Allen, pos_1.position, pool_Allen1);
         }
         else if (GameManager.instance.getMyPlayer() == GameManager.MyNowPlayer.player_2)
         {
            //第二區
            born_P(player_Queen, /*pos_2.position*/pos_1.position, pool_Queen1);
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
                born_P(player_Allen,/*pos_2.position*/pos_1.position, pool_Allen2);
            }
            else
            {
                //第一區
                born_P(player_Allen,pos_1.position, pool_Allen1);
            }

        }
        else if (GameManager.instance.getMyPlayer() == GameManager.MyNowPlayer.player_2)
        {
            if (GameManager.instance.getMyFirst() == GameManager.MyNowPlayer.player_1)
            {
                //第二區
                born_P(player_Queen,pos_2.position, pool_Queen2);
            }
            else
            {
                //第一區
                GameManager.instance.WhoMe = GameManager.MyNowPlayer.player_1;
                born_P(player_Queen, pos_1.position, pool_Queen1);
            }
        }
    }
    #endregion

    void born_P(string player, Vector3 _pos, string poolNumber)
    {
        GameObject _player = PhotonNetwork.Instantiate("Prefabs/Player/" + player, _pos, Quaternion.identity, 0);
        Instantiate(Resources.Load("Prefabs/ObjectPool/" + poolNumber), Vector3.zero, Quaternion.identity);
        _player.transform.SetParent(MyPlayer);
    }
}
