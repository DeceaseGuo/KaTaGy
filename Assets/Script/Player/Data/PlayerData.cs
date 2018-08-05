using System;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class PlayerData:MonoBehaviour
{
    public static PlayerData instance;

    private void Awake()
    {
        if (instance == null)
            instance = this;
    }

    [System.Serializable]
    public struct PlayerDataBase
    {
        public string objectName;
        public Sprite headImage;
        public float moveSpeed;
        public float rotSpeed;
        public float stoppingDst;
        public float GV;
        public float Dodget_Delay;
        public float ReBorn_CountDown;
        [Header("攻擊")]
        public float Atk_Damage;
        public float Atk_maxDamage;
        public int ATK_Level;
        [Header("防禦")]
        public float def_maxDEF;
        public float def_base;
        public int DEF_Level;
        [Header("血量、能量")]
        public float Hp_Max;
        public float Hp_original;
        public float Ap_Max;
        public float Ap_original;
        public float add_APValue;
        [Header("技能CD")]
        public float skillCD_Q;
        public float skillCD_W;
        public float skillCD_E;
        public float skillCD_R;
        [Header("升級相關")]
        public float Add_hp1;
        public float Add_hp2;
        public float Add_hp3;
        public float Add_ap1;
        public float Add_ap2;
        public float Add_ap3;
        public float Add_atk1;
        public float Add_atk2;
        public float Add_atk3;
        public float Add_def1;
        public float Add_def2;
        public float Add_def3;
    }

    public PlayerDataBase Allen;
    public PlayerDataBase Queen;

    public PlayerDataBase getPlayerData(GameManager.meIs _name)
    {
        switch (_name)
        {
            case (GameManager.meIs.Allen):
                return Allen;
            case (GameManager.meIs.Queen):
                return Queen;
            default:
                return new PlayerDataBase();
        }
    }
}
