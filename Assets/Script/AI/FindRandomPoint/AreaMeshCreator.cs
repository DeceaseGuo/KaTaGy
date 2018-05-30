using System;
using System.Collections.Generic;
using System.Linq;
using Core.Extensions;
using UnityEngine;
using Random = UnityEngine.Random;

namespace TowerDefense.MeshCreator
{
    /// <summary>
    /// 創建一個表示區域的網格
    /// </summary>
    [Serializable]
    public class AreaMeshCreator : MonoBehaviour
    {
        [HideInInspector]
        public MeshObject meshObject;

        public Transform outSidePointsParent;

        /// <summary>
        /// 將中心變為此物件的子物件
        /// </summary>
        public Transform pointsCenter
        {
            get
            {
                if (outSidePointsParent == null)
                {
                    var points = new GameObject("Points");
                    outSidePointsParent = points.transform;
                    outSidePointsParent.SetParent(transform, false);
                    outSidePointsParent.eulerAngles = new Vector3(90, 0, 0);
                }
#if UNITY_EDITOR
                outSidePointsParent.hideFlags = HideFlags.HideInHierarchy;
#endif
                return outSidePointsParent;
            }
        }

#if UNITY_EDITOR
        /// <summary>
        /// 獲取此網格中點的變換的數組 - 僅由編輯器腳本使用
        /// </summary>
        public Transform[] pointsTransforms
        {
            get
            {
                Transform[] childern = new Transform[pointsCenter.childCount];
                int length = pointsCenter.childCount;
                for (int i = 0; i < length; i++)
                {
                    childern[i] = pointsCenter.GetChild(i);
                }
                return childern;
            }
        }
#endif
        /// <summary>
        /// 獲取與此網格中點的位置相對應的Vector3的列表
        /// </summary>
        public List<Vector3> GetPoints()
        {
            return GetChildrenPositions(pointsCenter);
        }

        /// <summary>
        /// 取得一隨機點
        /// </summary>
        public Vector3 GetRandomPointInside()
        {
            return transform.TransformPoint(meshObject.RandomPointInMesh());
        }

        /// <summary>
        /// 強制所有點的局部“y”位置為0
        /// Makes them coplanar
        /// </summary>
        public void ForcePointsFlat()
        {
            int length = pointsCenter.childCount;
            for (int i = 0; i < length; i++)
            {
                Transform t = pointsCenter.GetChild(i);
                Vector3 position = t.localPosition;
                position.z = 0;
                t.localPosition = position;
            }
        }

        List<Vector3> GetChildrenPositions(Transform parent)
        {
            int length = parent.childCount;
            List<Vector3> points = new List<Vector3>();
            for (int i = 0; i < length; i++)
            {
                points.Add(parent.GetChild(i).position);
            }
            return points;
        }

#if UNITY_EDITOR
        void OnDrawGizmos()
        {
            int count = pointsCenter.childCount;
            for (int i = 0; i < count - 1; i++)
            {
                Vector3 from = pointsCenter.GetChild(i).position;
                Vector3 to = pointsCenter.GetChild(i + 1).position;
                Gizmos.DrawLine(from, to);
            }
            // last to first
            Vector3 last = pointsCenter.GetChild(count - 1).position;
            Vector3 first = pointsCenter.GetChild(0).position;
            Gizmos.DrawLine(last, first);
        }
#endif
    }

    [Serializable]
    public class Triangle
    {
        public Vector3 v0;

        public Vector3 v1;

        public Vector3 v2;

        public float area;

        /// <summary>
        /// Represents a Triangle in a mesh
        /// </summary>
        /// <param name="v0">First Point</param>
        /// <param name="v1">Second Point</param>
        /// <param name="v2">Third Point</param>
        public Triangle(Vector3 v0, Vector3 v1, Vector3 v2)
        {
            this.v0 = v0;
            this.v1 = v1;
            this.v2 = v2;

            // Precalculate area
            float a = Vector3.Distance(v0, v1), b = Vector3.Distance(v1, v2), c = Vector3.Distance(v2, v0);
            float s = (a + b + c) / 2;
            area = Mathf.Sqrt(s * (s - a) * (s - b) * (s - c));
        }
    }

    /// <summary>
    /// Contains the triangles of the mesh
    /// Calculates the mesh area
    /// Can get a random point within the mesh area
    /// </summary>
    [Serializable]
    public class MeshObject
    {
        public List<Triangle> triangles;

        public float completeArea;

        public MeshObject(List<Triangle> triangles)
        {
            this.triangles = triangles;
            completeArea = this.triangles.Sum(x => x.area);
        }

        /// <summary>
        /// Gets a random point in the mesh
        /// </summary>
        /// <returns>Random point</returns>
        public Vector3 RandomPointInMesh()
        {
            Triangle randomTriangle = triangles.WeightedSelection(completeArea, t => t.area);
            float x = Random.value, y = Random.value;
            if (x + y >= 1)
            {
                x = 1 - x;
                y = 1 - y;
            }
            float z = 1 - x - y;
            var randomBaryCentricPoint = new Vector3(x, y, z);
            Vector3 cartesianPoint = (randomBaryCentricPoint.x * randomTriangle.v0) + (randomBaryCentricPoint.y *
                                                                                       randomTriangle.v1) +
                                     (randomBaryCentricPoint.z * randomTriangle.v2);
            return cartesianPoint;
        }
    }
}