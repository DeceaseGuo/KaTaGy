using System.Collections.Generic;
using UnityEngine;

public class CreatPoints : MonoBehaviour
{
    public class pointData
    {
        public Transform point;
        public Quaternion Dir;
        public float atkDistance;
        public Vector3 lastPoint;

        public pointData(Transform _point, Quaternion _Dir, float _atkDis)
        {
            point = _point;
            Dir = _Dir;
            atkDistance = _atkDis;
        }
    }
    public List<pointData> keysList = new List<pointData>();
    
    [SerializeField] float extraRange = 0;
    public Dictionary<float, List<pointData>> atkPoints;
    public List<Transform> willGoNext;
    public List<Transform> alreadyFull;
    private LayerMask obstacleMask;

    private float lastWidth;
    private Vector3 myCheckBoxV3;
        
    class WhichfloatComparer : IEqualityComparer<float>
    {
        public bool Equals(float x, float y)
        {
            return x == y;
        }

        public int GetHashCode(float x)
        {
            return (int)x;
        }
    }

    #region 找最近點
    private int near = -1;
    private float neardis = 1000000;
    private float compareCon;
    private List<pointData> tmpPointData;

    //比較位子
    private Transform comparPos;
    //生點放置位子
    private Transform pointParent;
    #endregion

    #region 其他腳本初始化數據
    public void ProdecePoints ()
    {
        GetBaseData();
        CalculatePoint(4f, 2.5f, null);
        CalculatePoint(11, 4.8f, null);
    }

    public void ProdecePoints(Transform _Pos)
    {
        GetBaseData();
        CalculatePoint(4f, 2.5f, _Pos);
        CalculatePoint(11, 4.8f, _Pos);
    }

    void GetBaseData()
    {
        obstacleMask = 1 << 30 | 1 << 31 | 1 << 29 | 1 << 28 | 1 << 14 | 1 << 8;
        pointParent = GameObject.Find("PointData").transform;
        atkPoints = new Dictionary<float, List<pointData>>(new WhichfloatComparer());
    }
    #endregion

    public void NeedToLateUpdate()
    {
        for (int i = 0; i < keysList.Count; i++)
        {
            if (transform.position != keysList[i].lastPoint)
            {
                keysList[i].lastPoint = transform.position;
                keysList[i].point.position = transform.position + keysList[i].Dir * Vector3.forward * keysList[i].atkDistance;
            }
        }
        //觀看生成點用
        /*   if (Input.GetKeyDown("f"))
           {
             //  CalculatePoint(atkRangeTest, atkWidthTest);
           }*/
    }

    #region 找到離最近的空位
    public Transform FindClosePoint(float _range, Transform _soldierPos, float _width)
    {
        near = -1;
        neardis = 1000000;
        tmpPointData = atkPoints[_range];
        for (int i = 0; i < tmpPointData.Count; i++)
        {
            if (!alreadyFull.Contains(tmpPointData[i].point) && !willGoNext.Contains(tmpPointData[i].point))
            {
                if (CheckObstacle(tmpPointData[i].point, _width))
                {
                    compareCon = Vector3.Distance(tmpPointData[i].point.position, _soldierPos.position);
                    if (compareCon < neardis)
                    {
                        neardis = compareCon;
                        near = i;
                    }
                }
            }
        }
        if (near == -1)
            return null;

        return tmpPointData[near].point;
    }
    #endregion

    //比較距離
    public Transform GoComparing(float _range, Transform _soldierPos, Transform _firstPoint, float _width)
    {

        comparPos = FindClosePoint(_range, _soldierPos, _width);

        if (comparPos == null || _soldierPos == null || _firstPoint == null)
            return null;

        if (!willGoNext.Contains(comparPos) && !alreadyFull.Contains(comparPos))
        {
            return ((comparPos.position - _soldierPos.position).magnitude < (_firstPoint.position - _soldierPos.position).magnitude) ? comparPos : null;
        }
        else
            return null;
    }

    public bool CheckFull(float _dis)
    {
        if (!atkPoints.ContainsKey(_dis))
            return false;

        return (atkPoints[_dis].Count == alreadyFull.Count) ? true : false;
    }

    #region 新增移除Point
    //佔據點
    public void AddPoint(Transform _node)
    {
        if (!alreadyFull.Contains(_node))
        {
            alreadyFull.Add(_node);            
        }
    }
    public void RemoveForAddPoint(Transform _node)
    {
        if (willGoNext.Contains(_node))
            willGoNext.Remove(_node);

        if (!alreadyFull.Contains(_node))
            alreadyFull.Add(_node);
    }

    //WillGo點
    public void AddWillGo_P(Transform _node ,Transform _lastNode)
    {
        if (_lastNode != null && willGoNext.Contains(_lastNode))
            willGoNext.Remove(_lastNode);

        if (!willGoNext.Contains(_node))
            willGoNext.Add(_node);
    }

    //移除任何
    public void RemoveThisPoint(Transform _node)
    {
        if (_node == null)
            return;

        if (alreadyFull.Contains(_node))
            alreadyFull.Remove(_node);

        if (willGoNext.Contains(_node))
            willGoNext.Remove(_node);
    }
    public void RemoveAllPoint()
    {
        alreadyFull.Clear();
        willGoNext.Clear();
    }
    #endregion

    public bool IFDis(float _dis ,float _nowDis)
    {
        return (_nowDis <= atkPoints[_dis][0].atkDistance) ? true : false;
    }

    void CalculatePoint(float _range, float width ,Transform _myPos)
    {
        float angle = (width * 180) / (Mathf.PI * _range);
        int count = (int)(360 / angle);
        Transform _tmpPpoint = null;
        Quaternion q = Quaternion.identity;
        List<pointData> _tmpLlist = new List<pointData>();
        pointData tmpData = null;
        for (int i = 0; i < count; i++)
        {
            _tmpPpoint = new GameObject(i + "Point").transform;
            if (_myPos == null)
                _tmpPpoint.SetParent(pointParent);
            else
                _tmpPpoint.SetParent(_myPos);
            _tmpPpoint.position = Vector3.forward * (_range + extraRange);
            q = Quaternion.AngleAxis(angle * i, Vector3.up);
            _tmpPpoint.position = q * _tmpPpoint.position + transform.position;
            tmpData = new pointData(_tmpPpoint, q, _range + extraRange);
            _tmpLlist.Add(tmpData);
            keysList.Add(tmpData);
        }
        atkPoints.Add(_range, _tmpLlist);
    }

    //檢查障礙物
    bool CheckObstacle(Transform _pos, float _width)
    {
        if (_width != lastWidth)
        {
            lastWidth = _width;
            myCheckBoxV3 = new Vector3(_width, 2, _width);
        }
        if (!(Physics.CheckBox(_pos.position, myCheckBoxV3, Quaternion.identity, obstacleMask)))
        {
            if ((Physics.CheckBox(_pos.position, myCheckBoxV3, Quaternion.identity, 1 << 9)))
                return true;
        }
        return false;
    }


    //觀看點用
    public bool testLook;    
    /*public float atkRangeTest;
    public float atkWidthTest;

    private void Update()
    {
        if (Input.GetKeyDown("a"))
        {
            CalculatePoint(atkRangeTest, atkWidthTest);
        }
    }*/
    private void OnDrawGizmos()
    {
        if (testLook)
        {
            Gizmos.color = Color.red;

            if (keysList.Count != 0)
            {
                foreach (var item in keysList)
                {
                    Gizmos.DrawSphere(item.point.position, .3f);

                }
               /*   if (alreadyFull.Count != 0)
                  {
                      Gizmos.color = Color.yellow;
                      foreach (var item in alreadyFull)
                      {
                          Gizmos.DrawSphere(item.position, .5f);
                      }
                  }*/
            }
        }
    }
}
