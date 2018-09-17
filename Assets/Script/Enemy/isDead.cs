using UnityEngine;

public class isDead : MonoBehaviour
{
    public GameManager.NowTarget myAttributes;

    private bool dead;
    public bool checkDead { get { return dead; } }
    [SerializeField] bool noResetNoCC;
    public bool noCC;
    public bool noDamage;
    public bool notFeedBack;

    private void OnEnable()
    {
        dead = false;
        notFeedBack = false;
        noDamage = false;
        if (!noResetNoCC)
            noCC = false;
    }

    //改變死亡狀態
    public void ifDead(bool _dead)
    {
        dead = _dead;
    }

    //無敵
    public void NoDamage(bool _t)
    {
        noCC = _t;
        noDamage = _t;
    }
}
