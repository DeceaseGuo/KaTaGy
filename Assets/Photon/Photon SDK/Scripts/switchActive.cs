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
            Debug.LogWarning("You don't have pool");
            return;
        }
        parentPool = ObjectPooler.instance.transform;
    }

    [PunRPC]
    public void SetActiveRPC(bool active)
    {
        this.gameObject.SetActive(active);
    }

    [PunRPC]
    public void SetPoolparent()
    {
        if (parentPool != null)
        {
            this.gameObject.transform.SetParent(parentPool);
        }
    }

    [PunRPC]
    public void changePos(Vector3 _pos)
    {
        this.gameObject.transform.position = _pos;
    }

    [PunRPC]
    public void changeLayer(int n)
    {
        this.gameObject.layer = n;
    }
}
