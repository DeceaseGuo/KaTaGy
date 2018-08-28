using UnityEngine;

public class UpdateDataBase
{
    [System.Serializable]
    public class PlayerUpdateData
    {
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
        public float moveSpeed;        
        //還有技能
    }

    [System.Serializable]
    public class SoldierUpdateData
    {
        [Header("升級相關")]
        public float Add_hp1;
        public float Add_hp2;
        public float Add_hp3;
        public float Add_atk1;
        public float Add_atk2;
        public float Add_atk3;
        public float Add_def1;
        public float Add_def2;
        public float Add_def3;
        public float moveSpeed;
    }

    [System.Serializable]
    public class TowerUpdateData
    {
        [Header("升級相關")]
        public float Add_hp1;
        public float Add_hp2;
        public float Add_hp3;
        public float Add_atk1;
        public float Add_atk2;
        public float Add_atk3;
        public float Add_def1;
        public float Add_def2;
        public float Add_def3;
    }
}
