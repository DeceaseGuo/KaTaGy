using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MipMapSyn : MonoBehaviour
{
    [Header("玩家")]
    public Transform player;
    [Header("大地圖")]
    public GameObject myTerrain;

    //大地圖寬高
    float widthMax;
    float heightMax;

    [Header("小地圖")]
    [SerializeField]RectTransform litMap;
    [Header("玩家icon")]
    [SerializeField]RectTransform palyerIcon;

    //寬高比例
    float widthRate;
    float heightRate;

    Vector3 tmpAngle;
    Vector2 tmpPos = Vector2.zero;

 //   Vector2 offsetPos;

    private void Awake()
    {
        getWidthHeight();
       /* offsetPos.x = litMap.position.x;
        offsetPos.y = litMap.position.y;
        Debug.Log(offsetPos);*/
    }

    void getWidthHeight()
    {
        widthMax = myTerrain.GetComponent<MeshFilter>().mesh.bounds.size.x;
        heightMax= myTerrain.GetComponent<MeshFilter>().mesh.bounds.size.z;

        float scal_z = myTerrain.transform.localScale.z;
        heightMax = heightMax * scal_z;
        //得到大地图高度缩放地理
        float scal_x = myTerrain.transform.localScale.x;
        widthMax = widthMax * scal_x;
    }

    void Update()
    {
        UpdatePos();
    }


    void UpdatePos()
    {
        //寬比例=玩家目前位置.x / 大地圖.x
        widthRate = player.transform.position.x / widthMax;
        //高比例=玩家目前位置.z / 大地圖.z
        heightRate = player.transform.position.z / heightMax;
        //玩家icon位置.x=小地圖.x *寬比例
        tmpPos.x = litMap.sizeDelta.x * widthRate ;
        //玩家icon位置.y=小地圖.y *寬比例
        tmpPos.y = litMap.sizeDelta.y * heightRate ;

        tmpAngle = palyerIcon.localEulerAngles;
        tmpAngle.z = 90 - player.localEulerAngles.y;
        palyerIcon.localEulerAngles = tmpAngle;
        palyerIcon.localPosition = tmpPos;
    }
}