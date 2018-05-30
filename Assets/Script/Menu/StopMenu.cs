using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StopMenu : MonoBehaviour
{
    public static StopMenu instance;

    public GameObject stopMenuPrefab;

    private void Awake()
    {
        if (instance == null)
            instance = this;
    }

    public void SurrenderClick()
    {
        PhotonNetManager.instance.GetComponent<PhotonView>().RPC("OutGame", PhotonTargets.All);
        Destroy(GameManager.instance.gameObject);
        Destroy(PhotonNetManager.instance.gameObject);
    }
}
