using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
namespace Mytest
{
    public class test : MonoBehaviour
    {
        public Text num;
        public Coroutine testaaa;
        public float coutnDown;


        public float A1;
        public float A2 = 1;
        public float width;
        [SerializeField] Projector kkk;

        private void Update()
        {
            /* if (Input.GetKeyDown(KeyCode.Alpha1))
             {
                 kkk.Invoke();
                 Debug.Log("click");
             }*/
            kkk.orthographicSize = A1;
            kkk.aspectRatio = width / A1;

            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
               
                kkk.orthographicSize = A1 / 2;

            }
            if (Input.GetKeyDown(KeyCode.Alpha2))
            {

                kkk.aspectRatio = A2;

            }
            if (Input.GetKeyDown(KeyCode.Alpha3))
            {

                kkk.aspectRatio = width / A1;

            }

        }
    }
}
