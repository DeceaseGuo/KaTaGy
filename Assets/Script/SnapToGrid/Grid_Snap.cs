using UnityEngine;

public class Grid_Snap : MonoBehaviour
{
    //整個網格大小
    public Vector2 gridSize = new Vector2(1, 1);
    public float nodeRadius;  //每個點半徑
    private float nodeDiameter; //每個點直徑
    int gridSizeX, gridSizeY;   //節點x與y在整張圖中有多少個
    [SerializeField] LayerMask mask;
    [SerializeField] LayerMask mask2;
    [SerializeField] GameObject meshGridPrefab;
    private GameObject showGridSoace;


    //初始位置 (網格原點)
    Vector2 gridOffset;

    private void Awake()
    {
        nodeDiameter = nodeRadius * 2;
        gridSizeX = Mathf.FloorToInt(Mathf.Abs(gridSize.x / nodeDiameter));
        gridSizeY = Mathf.FloorToInt(Mathf.Abs(gridSize.y / nodeDiameter));
        showGridSoace = transform.Find("_Grid").gameObject;
    }

    private void Start()
    {
        CreatGrid();
        closGrid();
    }

    #region 自動生成捕捉用網格
    void CreatGrid()
    {
        Vector3 firstPos = transform.position - Vector3.right * gridSize.x / 2 - Vector3.right * 1f + Vector3.forward * gridSize.y / 2 + Vector3.forward * nodeRadius * 1.5f;
        for (int z = gridSizeY; z > 0; z--)
        {
            Vector3 nodePos;
            for (int x = 0; x < gridSizeX; x++)
            {
                nodePos = firstPos + Vector3.right * (x * nodeDiameter + nodeRadius) - Vector3.forward * (z * nodeDiameter + nodeRadius);
                
                if (!(Physics.CheckBox(nodePos, new Vector3(nodeRadius, nodeRadius, nodeRadius), Quaternion.identity, mask)))
                {
                    if (Physics.CheckBox(nodePos, new Vector3(nodeRadius, nodeRadius, nodeRadius), Quaternion.identity, mask2))
                    {
                        GameObject gridPrefab = Instantiate(meshGridPrefab);
                        gridPrefab.transform.position = nodePos;
                        gridPrefab.transform.SetParent(showGridSoace.transform);
                    }
                }
            }
        }
    }
    #endregion

    #region 開啟 和 關閉 網格 
    public void openGrid()
    {
        showGridSoace.SetActive(true);
    }

    public void closGrid()
    {
        showGridSoace.SetActive(false);
    }
    #endregion

   /* public Vector2 GetGridOffset()
    {
        gridOffset.x = transform.localPosition.x;
        gridOffset.y = transform.localPosition.y;
        return gridOffset;
    }/*

   /* private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position, new Vector3(gridSize.x, nodeRadius, gridSize.y));
    }*/
}