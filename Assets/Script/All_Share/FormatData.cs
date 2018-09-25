using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class FormatData : MonoBehaviour
{
    public UnityEvent formatScript;

    private void OnEnable()
    {
        formatScript.Invoke();
    }
}
