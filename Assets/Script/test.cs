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

        [SerializeField] List<bool> sss;

        private void Update()
        {
            /* if (Input.GetKeyDown(KeyCode.Alpha1))
             {
                 kkk.Invoke();
                 Debug.Log("click");
             }*/


            if (Input.GetKeyDown(KeyCode.Alpha1))
            {

                int num = sss.FindIndex(x => x == true);
                Debug.Log(num);

            }
            if (Input.GetKeyDown(KeyCode.Alpha2))
            {

                

            }
            if (Input.GetKeyDown(KeyCode.Alpha3))
            {

                

            }

        }

        IEnumerator ggg()
        {
            yield return new WaitForSeconds(0.2f);
            if (A1 != 4)
            {
                Debug.Log(A1);
                A1++;
            }
        }
    }
}
