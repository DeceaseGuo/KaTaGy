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
    public UnityEvent kkk;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            TestA();
        }
    }

    void TestA()
    {
        if (testaaa != null)
        {
            StopCoroutine(testaaa);
            testaaa = StartCoroutine(MatchTimer.Instance.SetCountDown(isOk, coutnDown, num, null));
        }
        else
            testaaa = StartCoroutine(MatchTimer.Instance.SetCountDown(isOk, coutnDown, num, null));
    }

    void isOk()
    {
        Debug.Log("Test成功");
        testaaa = null;
    }
}
