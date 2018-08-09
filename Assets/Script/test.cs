using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
public class test : MonoBehaviour
{
    public Text num;
    public Coroutine testaaa;
    public float coutnDown;

    public List<int> aaa;


    private void Update()
    {
        /* if (Input.GetKeyDown(KeyCode.Alpha1))
         {
             kkk.Invoke();
             Debug.Log("click");
         }*/

         if (Input.GetKeyDown(KeyCode.Alpha2))
         {

            aaaTest();
         }

    }

    void aaaTest()
    {
        foreach (var item in aaa)
        {
            if (item == 3)
            {
                Debug.Log("中斷?");
                break;
            }

            if (item != 3)
                Debug.Log("Num  "+item);
        }
    }
}
