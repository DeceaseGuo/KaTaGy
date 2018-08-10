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

    [SerializeField] int a;
    private void Update()
    {
        /* if (Input.GetKeyDown(KeyCode.Alpha1))
         {
             kkk.Invoke();
             Debug.Log("click");
         }*/

         if (Input.GetKeyDown(KeyCode.Alpha2))
         {

            aaaTest(a, true);
         }

    }

    void aaaTest(int _a, bool _T)
    {

        switch (_a)
        {
            case (0):
                Debug.Log("0");
                break;
            case (1):
                if (_T)
                {
                    Debug.Log(1);
                }
                return;

            default:
                break;
        }
        Debug.Log("fuck");

    }
}
