using UnityEngine;

public class SnapGrid_Pos : MonoBehaviour
{
    private Transform myCachedTransform;
    [SerializeField] Grid_Snap grid2;
    private BuildManager buildManager;

    //網格中心與大小
  //  Vector2 gridOffset = Vector2.zero;
  //  Vector2 gridSize = Vector2.one;
    //網格偵測
    [SerializeField] LayerMask gridMask;
    //物件變大中心位移量
    private Vector3 offsetPos;
   // [SerializeField] Transform testPos;

    Vector3 mousePos;
    [SerializeField]float tmpY;

    [Header("判斷是否可蓋塔防")]
    //public Building ifBuild;
    public Renderer render;
    public Renderer belowRender;
    public Color origonalColor;
    public Color notBuildColor;
    public LayerMask DetectMask;
    public Vector3 DetectCube;

    #region 緩存
    private Camera myCamera;
    //限制區XY
    private float maxXPos;
    private float maxYPos;
    private Ray detectRay;
    private RaycastHit hit;
    private Vector3 tmpPos;
    #endregion

    public Vector3 nodePos()
    {
        return myCachedTransform.position;
    }

    private void Start()
    {
        myCachedTransform = this.transform;
        myCamera = Camera.main;
        UpdateGridData();
        buildManager = BuildManager.instance;
    }

    public void NeedToUpdate()
    {
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
        offsetPos = myCachedTransform.localPosition;
        offsetPos.x = (myCachedTransform.localScale.x / 2f) - grid2.nodeRadius;
        offsetPos.y = -((myCachedTransform.localScale.y / 2f) - grid2.nodeRadius);
        myCachedTransform.localPosition = offsetPos;

       // gridSize = grid2.gridSize;
        //gridOffset = grid2.GetGridOffset();
    }
    #endregion

    #region 偵測網格正確位子
    void DetectPos()
    {
        //Debug.Log("偵測網格正確位子");
        detectRay = myCamera.ScreenPointToRay(Input.mousePosition);
        
        if (Physics.Raycast(detectRay, out hit, 300, gridMask))
        {
            tmpPos = hit.transform.position;
            tmpPos.y += tmpY;
            myCachedTransform.parent.position = tmpPos;
            DetectCanBuild();
        }
    }
    #endregion

    #region 偵測是否可蓋塔防
    void DetectCanBuild()
    {
        if (!Physics.CheckBox(myCachedTransform.position, DetectCube, myCachedTransform.localRotation, DetectMask))
        {
            buildManager.ifCanBuild = true;
            render.material.color = origonalColor;
            belowRender.material.color = origonalColor;
        }
        else
        {
            buildManager.ifCanBuild = false;
            render.material.color = notBuildColor;
            belowRender.material.color = notBuildColor;
        }
    }
    #endregion

    #region 觀看大小
    /*private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(centerPos.position, DetectCube);
    }*/
    #endregion

    // [Header("Restrictions")]
    // public bool considerScale = true;
    // [Header("移動格數")]
    // public Vector2 dragScale = new Vector2(1, 1);
    #region 取得網格正確位置
    /*Vector3 SnapToGrid(Vector3 dragPos)
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

        maxXPos = ((gridSize.x - 1) * 0.5f) + gridOffset.x;
        maxYPos = ((gridSize.y - 1) * 0.5f) + gridOffset.y;

        if (considerScale)
        {
            //右
            if (dragPos.x > maxXPos - myCachedTransform.localScale.x + .5f)
            {
                dragPos.x = maxXPos - myCachedTransform.localScale.x + grid2.nodeRadius;
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
            if (dragPos.z < (-maxYPos + gridOffset.y + gridOffset.y) + myCachedTransform.localScale.y - 1)
            {
                dragPos.z = -maxYPos + gridOffset.y + gridOffset.y + myCachedTransform.localScale.y - grid2.nodeRadius;
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
    }*/
    #endregion
}