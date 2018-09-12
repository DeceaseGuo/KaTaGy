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

            public NavMeshAgent nav;
        public Transform newPos;

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
                hhh(a);
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
                nowData = originalData;
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

        public int a;
        public List<int> ooo;
        public List<int> ccc;

        void hhh(int a)
        {
            for (int i = 0; i < ooo.Count; i++)
            {
                if (ooo[i] == a)
                    return;
                else
                    Debug.Log(ooo[i]);
            }

            for (int i = 0; i < ccc.Count; i++)
            {
                if (ccc[i] == a)
                    return;
                else
                    Debug.Log(ccc[i]);
            }
        }
    }
}
