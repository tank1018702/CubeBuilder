using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class CubeMesh : MonoBehaviour
{
    public List<CubeInfo> Allcubes;
    public CubeInfo CubePrefab;
    public bool UseCollider;


    Mesh cubeMesh;
    MeshCollider meshCollider;


    List<Vector3> vertices;
    List<int> triangles;
    List<Color> colors;
    List<Vector2> uvs;
    List<Vector3> tempCubeVertices;


    private void Awake()
    {
        Allcubes = new List<CubeInfo>();

        GetComponent<MeshFilter>().mesh = cubeMesh = new Mesh();
        if(UseCollider)
        {
            meshCollider = gameObject.AddComponent<MeshCollider>();
        }
        vertices = new List<Vector3>();
        triangles = new List<int>();
        colors = new List<Color>();
        uvs = new List<Vector2>();

        tempCubeVertices = new List<Vector3>();      
    }

    public void RemoveCube(Vector3 positon)
    {
        CubeInfo cube;
        if (GetCubeByPosition(positon, out cube))
        {

            RemoveNeighbor(cube);
            TriangulateAllCubes();
        }
    }

    public void AddCube(Vector3 position, M_CubeType type,CubeOrientate orientate)
    {
        CubeInfo cube = Instantiate(CubePrefab, position, Quaternion.identity, transform);
        cube.type = type;
        cube.Orientate = orientate;
        Debug.Log("传入坐标" + position + "||cube本地坐标" + cube.transform.localPosition+"type:"+(int)type);
       

        SetNeighbors(cube);

        TriangulateAllCubes();
    }

    CubeInfo preview;
    public void UpdateCube(M_CubeType type,CubeOrientate orientate)
    {
        if(!preview)
        {
            preview = Instantiate(CubePrefab, Vector3.zero, Quaternion.identity, transform);
            Allcubes.Add(preview);
        }
        DataClear();
       
        preview.transform.localPosition = Vector3.zero;
        preview.type = type;
        preview.Orientate = orientate;

        TriangulateAllCubes();
    }

    void TriangulateAllCubes()
    {
        Clear();
        foreach (var c in Allcubes)
        {
            TriaggulateCube(c);
        }
        Apply();
    }

    void DataClear()
    {
        vertices.Clear();
        triangles.Clear();
        uvs.Clear();
    }

    void RemoveNeighbor(CubeInfo cube)
    {
        Allcubes.Remove(cube);
        for (int i = 0; i < 14; i++)
        {
            if (cube.neighbors[i])
            {
                cube.neighbors[i].RemoveNeighbor(CubeMetrics.GetOppositeDirection((CubeNeighborDirection)i));
            }
        }
        Destroy(cube.gameObject);
    }

    void SetNeighbors(CubeInfo cube)
    {
        Allcubes.Add(cube);
        for (int i = 0; i < 14; i++)
        {
            CubeInfo neighbor;
            if (!cube.neighbors[i] && GetCubeByDirection(cube.Position, (CubeNeighborDirection)i, out neighbor))
            {
                cube.SetNeighbor((CubeNeighborDirection)i, neighbor);
            }
        }
    }

    bool GetCubeByPosition(Vector3 position, out CubeInfo resutCube)
    {
        for (int i = 0; i < Allcubes.Count; i++)
        {
            if (Allcubes[i].Position == position)
            {
                resutCube = Allcubes[i];
                return true;
            }

        }
        resutCube = null;
        return false;
    }

    bool GetCubeByDirection(Vector3 position, CubeNeighborDirection direction, out CubeInfo resutCube)
    {
        CubeInfo cube;
        if (GetCubeByPosition(CubeMetrics.GetCubePosByDirection(position, direction), out cube))
        {
            resutCube = cube;
            return true;
        }
        resutCube = cube;
        return false;
    }

    void TriaggulateCube(CubeInfo cube)
    {
        TransformToCubeVertices(cube);

        for (int i = 0; i < 6; i++)
        {
            if (!cube.CanHideSurface((CubeSurface)i))
            {
                AddCubeSurface(tempCubeVertices[0], tempCubeVertices[1], tempCubeVertices[2], tempCubeVertices[3],
                               tempCubeVertices[4], tempCubeVertices[5], tempCubeVertices[6], tempCubeVertices[7],
                              (CubeSurface)i, cube.type,6);
            }
        }
    }


    void TransformToCubeVertices(CubeInfo cube)
    {
        
        tempCubeVertices.Clear();
        Vector3 p = cube.Position;
        
        for (int i=0;i<8;i++)
        {
            tempCubeVertices.Add(p+cube.transform.InverseTransformVector(CubeMetrics.cubeVertex[i]));
        }
    }


    void AddCubeSurface(Vector3 v1, Vector3 v2, Vector3 v3, Vector3 v4,
                        Vector3 v5, Vector3 v6, Vector3 v7, Vector3 v8,
                        CubeSurface suface, M_CubeType type,int TypeCount)
    {
        //正方体为六个面,若使UV图为正方形,则暂设正方体的类型为n种
        //v坐标基点:0~5/6

        float uCoordinate = ((int)suface * 1.0f) / 6.0f;
        float vCoordinate=((int)type*1.0f)/TypeCount*1.0f;
     
        Vector2 uvBasePoint=new Vector2(uCoordinate,vCoordinate);

        switch (suface)
        {
            case CubeSurface.up:         
                AddSurfaceQuad(v1, v2, v3, v4,uvBasePoint,TypeCount);
                break;
            case CubeSurface.down:
                AddSurfaceQuad(v6, v5, v8, v7,uvBasePoint, TypeCount);
                break;
            case CubeSurface.left:
                AddSurfaceQuad(v1, v4, v8, v5,uvBasePoint, TypeCount);
                break;
            case CubeSurface.right:
                AddSurfaceQuad(v3, v2, v6, v7,uvBasePoint, TypeCount);
                break;
            case CubeSurface.front:
                AddSurfaceQuad(v2, v1, v5, v6,uvBasePoint, TypeCount);
                break;
            case CubeSurface.back:
                AddSurfaceQuad(v4, v3, v7, v8,uvBasePoint, TypeCount);
                break;
        }
    }

    void AddTriangle(Vector3 v1, Vector3 v2, Vector3 v3)
    {
        int vertexIndex = vertices.Count; 
        vertices.Add(v1); vertices.Add(v2); vertices.Add(v3);
        triangles.Add(vertexIndex); triangles.Add(vertexIndex + 1); triangles.Add(vertexIndex + 2);
    }
    void AddQuad(Vector3 v1,Vector3 v2,Vector3 v3,Vector3 v4)
    {
        int vertexIndex = vertices.Count;
        vertices.Add(v1); vertices.Add(v2); vertices.Add(v3); vertices.Add(v4);
        triangles.Add(vertexIndex); triangles.Add(vertexIndex + 1); triangles.Add(vertexIndex + 2);
        triangles.Add(vertexIndex); triangles.Add(vertexIndex + 2); triangles.Add(vertexIndex + 3);
    }

    void AddQuadUV(Vector2 uvBasePoint,int TypeCount)
    {
        float deltaU = 1f / 6.0f;
        float deltaV = 1f / TypeCount*1.0f;
        Vector2 uv1 = new Vector2(uvBasePoint.x, uvBasePoint.y + deltaV);
        Vector2 uv2 = new Vector2(uvBasePoint.x + deltaU, uvBasePoint.y + deltaV);
        Vector2 uv3 = new Vector2(uvBasePoint.x + deltaU, uvBasePoint.y);
        Vector2 uv4 = uvBasePoint;
        uvs.Add(uv1); uvs.Add(uv2); uvs.Add(uv3); uvs.Add(uv4);
    }

    void AddSurfaceQuad(Vector3 v1, Vector3 v2, Vector3 v3, Vector3 v4, Vector2 uvDp,int uvCount)
    {
        AddQuad(v1, v2, v3, v4);
        AddQuadUV(uvDp,uvCount);
    }

    void Apply()
    {
        cubeMesh.SetVertices(vertices);
        cubeMesh.SetTriangles(triangles, 0);
        cubeMesh.SetUVs(0,uvs);
        cubeMesh.RecalculateNormals();
        if(UseCollider)
        {
            meshCollider.sharedMesh = cubeMesh;
        }
        
    }

    public void Clear()
    {
        vertices.Clear();
        triangles.Clear();
        uvs.Clear();
        cubeMesh.Clear();
    }
















}
