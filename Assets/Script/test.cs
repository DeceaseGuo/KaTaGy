using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using MyCode.Timer;
namespace Mytest
{
    public class test : MonoBehaviour
    {

        bool A = true;
        bool B = false;
        public int atk;
        public float d;

        private void Start()
        {
            for (int i = 0; i < 100; i++)
            {
                kkk(atk);
            }
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

                B = true;
            }
            if (Input.GetKeyDown(KeyCode.Alpha2))
            {

            }
            if (Input.GetKeyDown(KeyCode.Alpha3))
            {

                

            }

        }


        void kkk(int a)
        {
            float s = a * Mathf.Pow((1 - 0.06f), d);
            Debug.Log(s);
        }
    }
}
