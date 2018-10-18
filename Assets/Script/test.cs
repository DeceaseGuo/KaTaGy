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
        public LinkedList<int> linkTest=new LinkedList<int>();
        public NavMeshAgent[] nav;
        public Transform[] p;

        [System.Serializable]
        public class testUse
        {
            public byte findIndex;
            public int nowIndex;

            public testUse(byte _findIndex, int _t)
            {
                findIndex = _findIndex;
                nowIndex = _t;
            }
        }
        public List<testUse> UseTest = new List<testUse>();
        testUse lll;
        private void Update()
        {

            if (Input.GetKeyDown(KeyCode.Alpha1))
            {

                // nav[0].SetDestination(p[0].position);
            }
            if (Input.GetKeyDown(KeyCode.Alpha2))
            {
            }
            if (Input.GetKeyDown(KeyCode.Alpha3))
            {
               // print(linkTest.AddLast)
                //nav[1].SetDestination(p[1].position);
            }
        }

        /* private void OnDrawGizmos()
         {
             Gizmos.DrawWireSphere(transform.position, testNum);
         }*/
        public byte modifyIndex;

        void ModifyThis(byte _a)
        {

            int a = UseTest.FindIndex(x => x.findIndex == _a);
            if (a != -1)
                UseTest[a].nowIndex = 99;
            // lll.nowIndex = 99;

        }

        void ClearThis(byte _a)
        {
            try
            {
                UseTest.Remove(UseTest.Find(x => x.findIndex == _a));
            }
            catch (IndexOutOfRangeException)
            {
                Debug.Log("Null");
                throw;
            }
            
        }


        void iii()
        {
            Invoke("uuu", 2.5f);
        }

        void uuu()
        {
            Debug.Log("123");
        }
    }
}
