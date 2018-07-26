using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class switchActive : Photon.PunBehaviour
{
    Transform parentPool;
    PhotonTransformView transformView;

    private void Awake()
    {
        if (ObjectPooler.instance == null)
        {
            Debug.LogWarning("You don't have pool" + name);
            return;
        }
        parentPool = ObjectPooler.instance.transform;
        transformView = GetComponent<PhotonTransformView>();
    }

    #region 開關gameObj
    [PunRPC]
    public void SetActiveT()
    {
        gameObject.SetActive(true);

        if (transformView != null)
            transformView.enabled = true;
    }
    [PunRPC]
    public void SetActiveT(Vector3 _pos)
    {
        if (_pos != null)
            transform.position = _pos;

        gameObject.SetActive(true);

        if (transformView != null)
            transformView.enabled = true;
    }
    [PunRPC]
    public void SetActiveF()
    {
        gameObject.SetActive(false);

        if (transformView != null)
            transformView.enabled = false;
    }
    #endregion

    [PunRPC]
    public void SetPoolparent()
    {
        if (parentPool != null)
        {
            this.gameObject.transform.SetParent(parentPool);
        }
    }

    [PunRPC]
    public void changeLayer(int n)
    {
        this.gameObject.layer = n;
    }
}
