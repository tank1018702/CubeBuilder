using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class CubeMesh : MonoBehaviour
{
    Mesh cubeMesh;
    MeshCollider meshCollider;
    MeshFilter filter;

    [NonSerialized]
    List<Vector3> vertices;
    [NonSerialized]
    List<int> triangles1;
    [NonSerialized]
    List<int> triangles2;
    [NonSerialized]
    List<Vector2> uvs;


    public bool useCollider;

    private void Awake()
    {
        filter = GetComponent<MeshFilter>();
        cubeMesh = new Mesh();
        cubeMesh = filter.mesh;
        cubeMesh.subMeshCount = 2;

        if (useCollider)
        {
            meshCollider = gameObject.AddComponent<MeshCollider>();
        }
    }

    public void Clear()
    {
        cubeMesh.Clear();
        vertices = ListPool<Vector3>.Get();
        triangles1 = ListPool<int>.Get();
        triangles2 = ListPool<int>.Get();
        uvs = ListPool<Vector2>.Get();
      

    }

    public void Apply()
    {

        cubeMesh.subMeshCount = 2;
        cubeMesh.SetVertices(vertices);
        ListPool<Vector3>.Add(vertices);

        cubeMesh.SetTriangles(triangles1, 0);
        ListPool<int>.Add(triangles1);

        cubeMesh.SetTriangles(triangles2, 1);
        ListPool<int>.Add(triangles2);

        cubeMesh.SetUVs(0, uvs);
        ListPool<Vector2>.Add(uvs);


        cubeMesh.RecalculateNormals();
        

        if (useCollider )
        {
            meshCollider.sharedMesh = null;
            meshCollider.sharedMesh = cubeMesh;
        }
    }

    public void AddTriangle(Vector3 v1, Vector3 v2, Vector3 v3,int subIndex=0)
    {
        int vertexIndex = vertices.Count;
        vertices.Add(v1); vertices.Add(v2); vertices.Add(v3);
        if(subIndex!=0)
        {
            triangles2.Add(vertexIndex); triangles2.Add(vertexIndex + 1); triangles2.Add(vertexIndex + 2);
        }
        else
        {
            triangles1.Add(vertexIndex); triangles1.Add(vertexIndex + 1); triangles1.Add(vertexIndex + 2);
        }
        
    }

    public void AddQuad(Vector3 v1, Vector3 v2, Vector3 v3, Vector3 v4,int subIndex=0)
    {
        int vertexIndex = vertices.Count;
        vertices.Add(v1); vertices.Add(v2); vertices.Add(v3); vertices.Add(v4);
        if(subIndex!=0)
        {
            triangles2.Add(vertexIndex); triangles2.Add(vertexIndex + 1); triangles2.Add(vertexIndex + 2);
            triangles2.Add(vertexIndex); triangles2.Add(vertexIndex + 2); triangles2.Add(vertexIndex + 3);
        }
        else
        {
            triangles1.Add(vertexIndex); triangles1.Add(vertexIndex + 1); triangles1.Add(vertexIndex + 2);
            triangles1.Add(vertexIndex); triangles1.Add(vertexIndex + 2); triangles1.Add(vertexIndex + 3);
        }
    }

    public void AddQuadUV(Vector2 uvBasePoint, int TypeCount)
    {
        float deltaU = 1f / 6.0f;
        float deltaV = 1f / TypeCount * 1.0f;
        Vector2 uv1 = new Vector2(uvBasePoint.x, uvBasePoint.y + deltaV);
        Vector2 uv2 = new Vector2(uvBasePoint.x + deltaU, uvBasePoint.y + deltaV);
        Vector2 uv3 = new Vector2(uvBasePoint.x + deltaU, uvBasePoint.y);
        Vector2 uv4 = uvBasePoint;
        uvs.Add(uv1); uvs.Add(uv2); uvs.Add(uv3); uvs.Add(uv4);
    }

    public void AddQuadUV(float uMin, float uMax, float vMin, float vMax)
    {
        uvs.Add(new Vector2(uMin, vMin));
        uvs.Add(new Vector2(uMax, vMin));
        uvs.Add(new Vector2(uMax, vMax));
        uvs.Add(new Vector2(uMin, vMax));
    }
}
