using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class tex :Photon.PunBehaviour {
    Transform parent;
    Text te;

    private void Awake()//active = true都會作用
    {
        parent = GameObject.Find("Canvas").transform;
        te = GetComponent<Text>();
    }

    void Start () {
        gameObject.transform.SetParent(parent);
        gameObject.transform.localPosition = Vector3.zero;
        //gameObject.SetActive(false);
    }
	
    void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.isWriting)
        {
            //我们拥有这个玩家：把我们的数据发送给别的玩家
            stream.SendNext(te.text);
        }
        else
        {
            //网络玩家，接收数据
            te.text = (string)stream.ReceiveNext();
        }
    }
}
