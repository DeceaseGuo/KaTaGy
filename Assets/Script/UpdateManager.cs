using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpdateManager : MonoBehaviour
{
    public static UpdateManager instance;

    public List<PromptScreen> myLockPos;
    private Player myPlayer;

    public enum Myability
    {
        None,
        Player_ATK,
        Player_DEF,
        Soldier_ATK,
        Soldier_DEF,
        Tower_ATK,
        Tower_DEF,
        Skill_Q_Player,
        Skill_W_Player,
        Skill_E_Player,
        Skill_R_Player,
        UnLock_Obj
    }

    private void Awake()
    {
        if (instance == null)
            instance = this;
    }

    private void Start()
    {
        myPlayer = Creatplayer.instance.Player_Script;
    }

    private void Update()
    {
        if (Input.GetKeyDown("v"))
        {
            Update_ThisAbility(Myability.Player_ATK, 1, GameManager.whichObject.None);
        }
    }

    public void Update_ThisAbility(Myability _state, int _level, GameManager.whichObject _whoIs)
    {
        switch (_state)
        {
            case Myability.Player_ATK:
                myPlayer.UpdateMyData(_level, true, false, false, false, false, false);
                break;
            case Myability.Player_DEF:
                myPlayer.UpdateMyData(_level, false, true, false, false, false, false);
                break;
            case Myability.Soldier_ATK:
                break;
            case Myability.Soldier_DEF:
                break;
            case Myability.Tower_ATK:
                break;
            case Myability.Tower_DEF:
                break;
            case Myability.Skill_Q_Player:
                myPlayer.UpdateMyData(_level, false, false, true, false, false, false);
                break;
            case Myability.Skill_W_Player:
                myPlayer.UpdateMyData(_level, false, false, false, true, false, false);
                break;
            case Myability.Skill_E_Player:
                myPlayer.UpdateMyData(_level, false, false, false, false, true, false);
                break;
            case Myability.Skill_R_Player:
                myPlayer.UpdateMyData(_level, false, false, false, false, false, true);
                break;
            case Myability.UnLock_Obj:
                FindAnd_UnLock(_whoIs);
                break;
            default:
                break;
        }
    }

    void FindAnd_UnLock(GameManager.whichObject _whoIs)
    {
        myLockPos.Find(x => x.DataName == _whoIs).UnLock();
    }
}
