using System.Collections.Generic;
using UnityEngine;

public class isDead : MonoBehaviour
{
    public GameManager.NowTarget myAttributes;

    public bool dead = false;
    public bool checkDead { get { return dead; } }

    private void OnEnable()
    {
        dead = false;
    }

    public void ifDead(bool _dead)
    {
        dead = _dead;
    }
}
