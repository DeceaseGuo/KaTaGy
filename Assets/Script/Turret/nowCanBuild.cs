using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class nowCanBuild : MonoBehaviour
{
    public Building ifBuild;
    public Renderer render;
    public Renderer belowRender;
    public Color origonalColor;
    public Color notBuildColor;
    [SerializeField] Transform centerPos;
    [SerializeField] LayerMask DetectMask;
    [SerializeField] Vector3 DetectCube;
    public Collider[] nott;
    Vector3 pos;

    private void Start()
    {
        pos = transform.position;
        DetectCanBuild();
    }

    private void LateUpdate()
    {
        if (pos != transform.position)
        {
            pos = transform.position;

            DetectCanBuild();
        }
    }

    void DetectCanBuild()
    {
        nott = Physics.OverlapBox(transform.position, DetectCube, transform.localRotation, DetectMask);
        bool walkableTwo = (Physics.CheckBox(transform.position, DetectCube, transform.localRotation, DetectMask));
        if (!walkableTwo)
        {
            ifBuild.ifCanBuild = true;
            render.material.color = origonalColor;
            belowRender.material.color = origonalColor;
        }
        else
        {
            ifBuild.ifCanBuild = false;
            render.material.color = notBuildColor;
            belowRender.material.color = notBuildColor;
        }
    }

    /*private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(centerPos.position, DetectCube);
    }*/
}
