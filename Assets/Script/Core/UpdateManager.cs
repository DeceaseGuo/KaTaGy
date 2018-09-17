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
        Skill_R_Player
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

   /* private void Update()
    {
        if (Input.GetKeyDown("v"))
        {
            Update_ThisAbility(GameManager.NowTarget.Soldier, Myability.Soldier_ATK, 1, GameManager.whichObject.None);
        }
    }*/

    public void Update_ThisAbility(GameManager.NowTarget _whoUpdate, Myability _state, byte _level, GameManager.whichObject _whoIs)
    {
        switch (_whoUpdate)
        {
            case GameManager.NowTarget.Null:
                FindAnd_UnLock(_whoIs);
                break;
            case GameManager.NowTarget.Player:
                myPlayer.Net.RPC("UpdataData", PhotonTargets.All, _level, (int)_state);
                break;
            case GameManager.NowTarget.Soldier:
                myPlayer.Net.RPC("UpdataSoldier", PhotonTargets.All, _level, (int)_state);
                break;
            case GameManager.NowTarget.Tower:
                myPlayer.Net.RPC("UpdataTower", PhotonTargets.All, _level, (int)_state);
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
