using System.Collections;
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
    public Dictionary<float, List<pointData>> atkPoints /*= new Dictionary<float, List<pointData>>()*/;
    public List<Transform> TestNext;
    public List<Transform> alreadyFull;

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

    private void Start()
    {
        pointParent = GameObject.Find("PointData").transform;
        atkPoints = new Dictionary<float, List<pointData>>(new WhichfloatComparer());
    }

    #region 找到離最近的空位
    public Transform FindClosePoint(float _range, Transform _soldierPos, float _width, bool _obsDet)
    {
        ////之後可將距離事先存好
        if (!atkPoints.ContainsKey(_range))
        {
            CalculatePoint(_range, _width);
        }
        ////

        near = -1;
        neardis = 1000000;
        tmpPointData = atkPoints[_range];
        for (int i = 0; i < tmpPointData.Count; i++)
        {
            if (!alreadyFull.Contains(tmpPointData[i].point) && !checkDetectPos(tmpPointData[i].point))
            {
                if (CheckObstacle(tmpPointData[i].point, _width, _obsDet))
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

    public Transform GoComparing(float _range, Transform _soldierPos, Transform _firstPoint, float _width, bool _obsDet)
    {

        comparPos = FindClosePoint(_range, _soldierPos, _width, _obsDet);

        if (comparPos == null || _soldierPos == null || _firstPoint == null)
            return null;

        if (!TestNext.Contains(comparPos) && !alreadyFull.Contains(comparPos))
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

    public bool checkDetectPos(Transform _pos)
    {
        return (TestNext.Contains(_pos)) ? true : false;
    }

    public void AddPoint(float _range, Transform _node)
    {
        if(!alreadyFull.Contains(_node))
        {
            //if(!CheckFull(_node))
            alreadyFull.Add(_node);

            TestNext.Remove(_node);
        }
    }

    public void RemovePoint( float _range, Transform _node)
    {
        if (alreadyFull.Contains(_node))
            alreadyFull.Remove(_node);      
    }

    private void LateUpdate()
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
        /*if (Input.GetKeyDown("f"))
        {
            CalculatePoint(atkRangeTest, atkWidthTest);
        }*/
    }

    public bool IFDis(float _dis ,float _nowDis)
    {
        return (_nowDis <= atkPoints[_dis][0].atkDistance) ? true : false;
    }

    void CalculatePoint(float _range, float width)
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
            _tmpPpoint.SetParent(/*transform*/pointParent);
            _tmpPpoint.position = Vector3.forward * (_range + extraRange);
            q = Quaternion.AngleAxis(angle * i, Vector3.up);
            _tmpPpoint.position = q * _tmpPpoint.position + transform.position;
            tmpData = new pointData(_tmpPpoint, q, _range + extraRange);
            _tmpLlist.Add(tmpData);
            keysList.Add(tmpData);
        }
        atkPoints.Add(_range, _tmpLlist);
    }

    bool CheckObstacle(Transform _pos, float _width, bool _obsDet)
    {
        if (!_obsDet)
            return true;

        if (_width != lastWidth)
        {
            lastWidth = _width;
            myCheckBoxV3 = new Vector3(_width / 2, 2, _width / 2);
        }
        if (!(Physics.CheckBox(_pos.position, myCheckBoxV3, Quaternion.identity, 1 << 30 | 1 << 31 | 1 << 29 | 1 << 28 | 1 << 14 | 1 << 8 | 1 << 11)))
        {
            if ((Physics.CheckBox(_pos.position, myCheckBoxV3, Quaternion.identity, 1 << 9)))
                return true;
        }
        return false;
    }

    public bool testLook;
    public float atkRangeTest;
    public float atkWidthTest;
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

                /*  if (TestNext.Count != 0)
                  {
                      Gizmos.color = Color.blue;
                      foreach (var item in TestNext)
                      {
                          Gizmos.DrawSphere(item.position, .5f);
                          Gizmos.color = Color.black;
                          Gizmos.DrawCube(item.position, new Vector3(.7f, 1f, .7f));
                      }
                  }

                  if (alreadyFull.Count != 0)
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
