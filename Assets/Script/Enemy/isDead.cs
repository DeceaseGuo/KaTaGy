using System.Collections.Generic;
using UnityEngine;

public class isDead : MonoBehaviour
{
    public GameManager.NowTarget myAttributes;

    private bool dead = false;
    public bool checkDead { get { return dead; } }

    public bool notFeedBack = false;

    private void OnEnable()
    {
        dead = false;
    }

    public void ifDead(bool _dead)
    {
        dead = _dead;
    }
}
