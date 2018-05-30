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
