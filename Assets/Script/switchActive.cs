using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class switchActive : Photon.PunBehaviour
{
    Transform parentPool;

    private void Awake()
    {
        if (ObjectPooler.instance == null)
        {
            Debug.LogWarning("You don't have pool" + name);
            return;
        }
        parentPool = ObjectPooler.instance.transform;
    }

    #region 開關gameObj
    [PunRPC]
    public void SetActiveT()
    {
        gameObject.SetActive(true);
    }
    [PunRPC]
    public void SetActiveT(Vector3 _pos)
    {
        transform.position = _pos;
        gameObject.SetActive(true);
    }
    [PunRPC]
    public void SetActiveF()
    {
        gameObject.SetActive(false);

        if (parentPool != null)
            gameObject.transform.SetParent(parentPool);
    }
    #endregion

    [PunRPC]
    public void changeLayer(int n)
    {
        this.gameObject.layer = n;
    }
}
