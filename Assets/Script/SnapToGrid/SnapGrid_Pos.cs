using UnityEngine;
using System.Collections;

public class SnapGrid_Pos : MonoBehaviour
{
    [SerializeField] Grid_Snap grid2;
    private BuildManager buildManager;
    [Header("Restrictions")]
    public bool considerScale = true;

    [Header("移動格數")]
    public Vector2 dragScale = new Vector2(1, 1);

    //網格中心與大小
    Vector2 gridOffset = Vector2.zero;
    Vector2 gridSize = Vector2.one;
    //網格偵測
    [SerializeField] LayerMask gridMask;
    //物件變大中心位移量
    private Vector3 offsetPos;
    [SerializeField] Transform testPos;

    Vector3 mousePos;
    [SerializeField]float tmpY;

    public Vector3 nodePos()
    {
        Vector3 nowNodePos = /*transform.parent.position + offsetPos*/testPos.position;
        return nowNodePos;
    }

    private void Start()
    {
        UpdateGridData();
        buildManager = BuildManager.instance;
    }
    private void Update()
    {
        if (!buildManager.nowBuilding)
            return;

        if (mousePos != Input.mousePosition)
        {
            mousePos = Input.mousePosition;
            DetectPos();
        }
    }

    #region 歸零並改變此網格中心
    void UpdateGridData()
    {
        //將中心變到最左
        offsetPos = transform.localPosition;
        offsetPos.x = (transform.localScale.x / 2f) - grid2.nodeRadius;
        offsetPos.y = -((transform.localScale.y / 2f) - grid2.nodeRadius);
        transform.localPosition = offsetPos;

        gridSize = grid2.gridSize;
        gridOffset = grid2.GetGridOffset();
    }
    #endregion

    #region 偵測網格正確位子
    void DetectPos()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 500, gridMask))
        {
            Vector3 tmpPos = hit.transform.position;
            tmpPos.y += tmpY;
            transform.parent.position = tmpPos;
        }
    }
    #endregion

    #region 取得網格正確位置
    Vector3 SnapToGrid(Vector3 dragPos)
    {
        if (gridSize.x % 2 == 0)
        {
            dragPos.x = (Mathf.Round(dragPos.x / dragScale.x) * dragScale.x);
        }
        else
        {
            dragPos.x = (Mathf.Round(dragPos.x / dragScale.x) * dragScale.x);
        }

        if (gridSize.y % 2 == 0)
        {
            dragPos.z = (Mathf.Round(dragPos.z / dragScale.y) * dragScale.y);
        }
        else
        {
            dragPos.z = (Mathf.Round(dragPos.z / dragScale.y) * dragScale.y);
        }

        #region 限制區域

        var maxXPos = ((gridSize.x - 1) * 0.5f) + gridOffset.x;
        var maxYPos = ((gridSize.y - 1) * 0.5f) + gridOffset.y;

        if (considerScale)
        {
            //右
            if (dragPos.x > maxXPos - transform.localScale.x + .5f)
            {
                dragPos.x = maxXPos - transform.localScale.x + grid2.nodeRadius;
            }
            //左
            if (dragPos.x < -maxXPos + gridOffset.x + gridOffset.x)
            {
                dragPos.x = -maxXPos + gridOffset.x + gridOffset.x;
            }
            //上
            if (dragPos.z > maxYPos)
            {
                dragPos.z = maxYPos;
            }
            //下
            if (dragPos.z < (-maxYPos + gridOffset.y + gridOffset.y) + transform.localScale.y - 1)
            {
                dragPos.z = -maxYPos + gridOffset.y + gridOffset.y + transform.localScale.y - grid2.nodeRadius;
            }
        }

        else
        {

            if (dragPos.x > maxXPos)
            {
                dragPos.x = maxXPos;
            }

            if (dragPos.x < -maxXPos + gridOffset.x + gridOffset.x)
            {
                dragPos.x = -maxXPos + gridOffset.x + gridOffset.x;
            }

            if (dragPos.z > maxYPos)
            {
                dragPos.z = maxYPos;
            }

            if (dragPos.z < -maxYPos + gridOffset.y + gridOffset.y)
            {
                dragPos.z = -maxYPos + gridOffset.y + gridOffset.y;
            }
        }

        #endregion
        dragPos.y = 0;
        return dragPos;
    }
    #endregion
}



