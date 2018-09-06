using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using MyCode.Timer;
using UnityEngine.AI;
using System;

namespace Mytest
{
    public class test : MonoBehaviour
    {

        public enum kkk :int
        {
            A=0,
            B=1,
            C=2,
            D=3
        }

        public kkk nowTest;
        public float qqq=12;
        public Mini_Soldier jjj;

        [SerializeField] int[] pos;
        private void Start()
        {
        }

        private void Update()
        {
            /* if (Input.GetKeyDown(KeyCode.Alpha1))
             {
                 kkk.Invoke();
                 Debug.Log("click");
             }*/

            
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
            }
            if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                if (pos.Length != 0)
                    Debug.Log("不等於0");
                else
                    Debug.Log("等於0");
            }
            if (Input.GetKeyDown(KeyCode.Alpha3))
            { 
            }
        }

        /* private void OnDrawGizmos()
         {
             Gizmos.DrawWireSphere(transform.position, testNum);
         }*/
        void Ttest1()
        {
        }

    }
}
