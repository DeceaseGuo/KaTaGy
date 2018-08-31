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

        public pointData(Transform _point, Quaternion _Dir, float _atkDis)
        {
            point = _point;
            Dir = _Dir;
            atkDistance = _atkDis;
        }
    }
    /// <param name="_soldier">士兵本身</param>
    /// <param name="range">攻擊距離</param>
    [SerializeField] float extraRange = 0;
    //public Dictionary<float, List<Transform>> atkPoints = new Dictionary<float, List<Transform>>();
    public Dictionary<float, List<pointData>> atkPoints = new Dictionary<float, List<pointData>>();
    public List<Transform> TestNext;
    public List<Transform> alreadyFull;


    /// <param name="_range">攻擊距離</param>
    /// <param name="_soldier">士兵本身</param>
    /// <param name="_width">士兵寬度</param>
    /// <param name="_obsDet"></param>
    public Transform getPoint(float _range, Transform _soldier ,float _width,bool _obsDet)
    {
        if (!atkPoints.ContainsKey(_range))
        {
            CalculatePoint(_range, _width);
            return findPoint(_range, _soldier, _obsDet);
        }
        else
        {
            return findPoint(_range, _soldier, _obsDet);
        }
    }

    #region 找到離最近的空位
    Transform findPoint(float _range, Transform _soldierPos ,bool _obsDet)
    {
        int near = -1;
        float neardis = 10000000;

        for (int i = 0; i < atkPoints[_range].Count; i++)
        {
            if (!CheckThisPointFull(atkPoints[_range][i].point) && !checkDetectPos(atkPoints[_range][i].point))
            {
                if (CheckObstacle(atkPoints[_range][i].point, _obsDet))
                {
                    float maxDisGap = Vector3.Distance(atkPoints[_range][i].point.position, _soldierPos.position);
                    if (maxDisGap < neardis)
                    {
                        neardis = maxDisGap;
                        near = i;
                    }
                }
            }
        }
        if (near == -1)
            return null;

        return atkPoints[_range][near].point;
    }
    #endregion

    public Transform GoComparing(float _range, Transform _soldierPos, Transform _firstPoint ,bool _obsDet)
    {

        Transform tmpPos = findPoint(_range, _soldierPos, _obsDet);

        if (tmpPos == null)
            return null;

        if (_soldierPos == null || _firstPoint == null)
            return null;

        if (!TestNext.Contains(tmpPos) && !alreadyFull.Contains(tmpPos))
        {
            Vector3 maxDisGap = tmpPos.position - _soldierPos.position;
            float maxDis = maxDisGap.sqrMagnitude;

            Vector3 maxDisGap2 = _firstPoint.position - _soldierPos.position;
            float maxDis2 = maxDisGap2.sqrMagnitude;

            if (maxDis < maxDis2)
                return tmpPos;
            else
                return null;
        }
        else
            return null;
    }

    public bool CheckFull(float _dis)
    {
        if (!atkPoints.ContainsKey(_dis))
            return false;

        if (atkPoints[_dis].Count == alreadyFull.Count)
            return true;
        else
            return false;
    }

    public bool CheckThisPointFull(Transform _pos)
    {
        if (alreadyFull.Contains(_pos))
            return true;
        else
            return false;
    }

    public bool checkDetectPos(Transform _pos)
    {
        if (TestNext.Contains(_pos))
            return true;
        else
            return false;
    }

    public void AddPoint(float _range, Transform _node)
    {
      //  if (atkPoints[_range].Contains(_node))
        if(!CheckThisPointFull(_node))
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
        for (int i = 0; i < atkPoints.Keys.Count; i++)
        {
            for (int a = 0; a < atkPoints[i].Count; a++)
            {
                atkPoints[i][a].point.position = this.transform.position + atkPoints[i][a].Dir * Vector3.forward * atkPoints[i][a].atkDistance;
            }
        }
    }

    public bool IFDis(float _dis ,float _nowDis)
    {
        if (_nowDis <= atkPoints[_dis][0].atkDistance)
            return true;
        else
            return false;
    }

    void CalculatePoint(float _range ,float width)
    {
        float angle = (width * 180) / (Mathf.PI * _range);
        int count = (int)(360 / angle);
        List<pointData> _tmpLlist = new List<pointData>();
       Transform aaa = GameObject.Find("PointData").transform;
        for (int i = 0; i < count; i++)
        {            
            Transform _tmpPpoint = new GameObject(i+"Point").transform;
            _tmpPpoint.SetParent(/*transform*/aaa);
            _tmpPpoint.position = Vector3.forward * (_range + extraRange);
            Quaternion q = Quaternion.AngleAxis(angle * i, Vector3.up);
            _tmpPpoint.position = q * _tmpPpoint.position + this.transform.position;

            pointData tmpPointData = new pointData(_tmpPpoint, q, _range + extraRange);
            _tmpLlist.Add(tmpPointData);
        }
        atkPoints.Add(_range, _tmpLlist);
    }

    bool CheckObstacle(Transform _pos ,bool _obsDet)
    {
        if (!_obsDet)
            return true;

        bool walkable = (Physics.CheckBox(new Vector3(_pos.position.x, 0, _pos.position.z), new Vector3(.7f, 1f, .73f), Quaternion.identity, 1 << 30 | 1 << 31 | 1 << 29 | 1 << 28 | 1 << 14 | 1 << 8 | 1 << 11));
        if (!walkable)
        {
            bool walkableTwo = (Physics.CheckBox(new Vector3(_pos.position.x, 0, _pos.position.z), new Vector3(.7f, 1f, .7f), Quaternion.identity, 1 << 9));
            if (walkableTwo)
                return true;
        }
        return false;
    }

   /* private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        if (atkPoints.ContainsKey(2.7f))
        {
            foreach (var item in atkPoints[2.7f])
            {
                Gizmos.DrawSphere(item.point.position, .5f);

            }

            if (TestNext.Count != 0)
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
            }
        }
    }*/
}
