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

  /*  private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.layer == 8 || other.gameObject.tag == "Player")
        {
            ifBuild.ifCanBuild = false;
            render.material.color = notBuildColor;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == 8 || other.gameObject.tag == "Player")
        {
            ifBuild.ifCanBuild = false;
            render.material.color = notBuildColor;
        }
        else
        {
            ifBuild.ifCanBuild = true;
            render.material.color = origonalColor;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == 8 || other.gameObject.tag == "Player")
        {
            render.material.color = origonalColor;
        }
        ifBuild.ifCanBuild = true;
    }*/

    private void LateUpdate()
    {
        DetectCanBuild();
    }

    void DetectCanBuild()
    {        
        bool walkableTwo = (Physics.CheckBox(centerPos.position, DetectCube, centerPos.rotation, DetectMask));
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

   /* public GameObject lllll;
    private void OnDrawGizmos()
    {
        lllll.transform.position = centerPos.position;
        lllll.transform.rotation = centerPos.rotation;
    }*/
}
