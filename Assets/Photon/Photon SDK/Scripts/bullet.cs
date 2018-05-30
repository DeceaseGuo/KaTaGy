using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bullet : Photon.MonoBehaviour {
    public int speed;
    public float diedTime;

    //ObjectPooler1 objectpool;
    bool Isobjactive;

    private void Awake()
    {
     //   objectpool = ObjectPooler1.instance;
    }

    void Start()
    {
        gameObject.SetActive(false);
        if (!photonView.isMine)
        {
            gameObject.name += "notMine";
            return;
        }
    }

    void Update()
    {
        
        if (!photonView.isMine)
        {
            return;
        }

        gameObject.transform.Translate(transform.forward * Time.deltaTime * speed);
    }
    
    void Dead()
    {
        //if (photonView.isMine)
           // objectpool.Repool("Sphere", gameObject);
    }

    #region 訊息傳遞
    void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.isWriting)
        {
            //我们拥有这个玩家：把我们的数据发送给别的玩家
            //stream.SendNext(gameObject.activeSelf);
            //stream.SendNext(gameObject.activeSelf);
        }
        else
        {
            //网络玩家，接收数据
            //Isobjactive = (bool)stream.ReceiveNext();
        }
    }
    #endregion
}
