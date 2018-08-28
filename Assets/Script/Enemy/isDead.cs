using System.Collections.Generic;
using UnityEngine;

public class isDead : MonoBehaviour
{
    public GameManager.NowTarget myAttributes;

    private bool dead;
    public bool checkDead { get { return dead; } }
    public bool noCC;
    public bool notFeedBack;

    private void OnEnable()
    {
        dead = false;
        notFeedBack = false;
        noCC = false;
    }

    public void ifDead(bool _dead)
    {
        dead = _dead;
    }
}
