using System;
using UnityEngine;

namespace Player
{
    [ExecuteInEditMode]
    [RequireComponent(typeof(MeshRenderer))]
    [RequireComponent(typeof(MeshFilter))]
    public class CircularSectorMeshRenderer : MonoBehaviour
    {
        [SerializeField] private MeshFilter meshFilter;
        [SerializeField, Range(0, 360)] private float degree = 180;
        [SerializeField, Range(1, 360)] private float intervalDegree = 5;
        [SerializeField, Range(0f, 100f)] private float radius = 10;
        
        public void RegenerateMesh(float degree, float radius)
        {
            this.degree = degree;
            this.radius = radius;

            CreateMesh();
        }

#if UNITY_EDITOR
        private void LateUpdate()
        {
            if (!Application.isPlaying)
            {
                CreateMesh();
            }
        }
#endif

        private void CreateMesh()
        {
            // 全体の角度を割って分割数を算出
            int count = (int)(degree / intervalDegree);

            // 割り切れなかったら分割数を追加
            if (degree % intervalDegree != 0)
            {
                ++count;
            }

            Vector2 uvCenter = new Vector2(0.5f, 0.5f);

            Mesh mesh = new Mesh();
            Vector3[] vertices = new Vector3[count * 2 + 1];
            int[] triangles = new int [count * 3];
            Vector2[] uvs = new Vector2[count * 2 + 1];
            uvs[0] = uvCenter;

            float forwardAngle = Vector3.SignedAngle(transform.forward, Vector3.right, transform.up);

            float beginDegree = forwardAngle - degree * 0.5f;
            float limitDegree = forwardAngle + degree * 0.5f;

            int i = 0;
            while (i < count)
            {
                float endDegree = beginDegree + intervalDegree;

                // 限界角度にclamp
                if (endDegree > limitDegree)
                {
                    endDegree = limitDegree;
                }

                // ラジアンに変換
                float beginRadian = Mathf.Deg2Rad * beginDegree;
                float endRadian = Mathf.Deg2Rad * endDegree;

                float beginCos = Mathf.Cos(beginRadian);
                float beginSin = Mathf.Sin(beginRadian);
                float endCos = Mathf.Cos(endRadian);
                float endSin = Mathf.Sin(endRadian);

                int beginNumber = i * 2 + 1;
                int endNumber = i * 2 + 2;
                int triangleNumber = i * 3;

                // セグメントの頂点を計算する
                vertices[beginNumber] = new Vector2(beginCos * radius, beginSin * radius);
                vertices[endNumber] = new Vector2(endCos * radius, endSin * radius);

                // 三角形のインデックスを設定
                triangles[triangleNumber] = 0;
                triangles[triangleNumber + 1] = endNumber;
                triangles[triangleNumber + 2] = beginNumber;

                const float uvRadius = 0.5f;
                uvs[beginNumber].x = beginCos * uvRadius + uvCenter.x;
                uvs[beginNumber].y = beginSin * uvRadius + uvCenter.y;
                uvs[endNumber].x = endCos * uvRadius + uvCenter.x;
                uvs[endNumber].y = endSin * uvRadius + uvCenter.y;

                beginDegree += intervalDegree;
                ++i;
            }

            mesh.vertices = vertices;
            mesh.triangles = triangles;
            mesh.uv = uvs;

            mesh.RecalculateNormals();
            mesh.RecalculateBounds();

            mesh.name = "CircularSectorMesh";
            meshFilter.mesh = mesh;
        }
    }
}