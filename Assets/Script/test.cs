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
      
        [System.Serializable]
        public struct TestData
        {
            public float hp;
        }
        public byte aaa;
        public TestData originalData;
        public TestData nowData;

            public NavMeshAgent[] nav;
        public Transform[] p;

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
                nav[0].SetDestination(p[0].position);
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
                // nav[1].SetDestination(p[1].position);
                kkk();
            }
        }

        /* private void OnDrawGizmos()
         {
             Gizmos.DrawWireSphere(transform.position, testNum);
         }*/
        void Ttest1(byte j)
        {
            Debug.Log(j);
        }

        public bool a;


        bool hhh()
        {
            if (a)
                return true;
            else
                return false;
        }

        void kkk()
        {
            Debug.Log("1");
            Debug.Log("2");
            if (hhh())
                return;
            Debug.Log("3");
            Debug.Log("4");

        }
    }
}
