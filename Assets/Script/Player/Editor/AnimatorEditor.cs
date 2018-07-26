using System.Collections;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Allen_Ani))]
public class AnimatorEditor :Editor 
{
    void OnSceneGUI()
    {
        Allen_Ani ani = (Allen_Ani)target;
        Handles.color = Color.white;
        Handles.DrawWireArc(ani.transform.position, Vector3.up, Vector3.forward, 360, ani.viewRadius);
        Vector3 viewAngleA = ani.DirFromAngle(-ani.viewAngle / 2, false);
        Vector3 viewAngleB = ani.DirFromAngle(ani.viewAngle / 2, false);

        Handles.DrawLine(ani.transform.position, ani.transform.position + viewAngleA * ani.viewRadius);
        Handles.DrawLine(ani.transform.position, ani.transform.position + viewAngleB * ani.viewRadius);
    }
}
