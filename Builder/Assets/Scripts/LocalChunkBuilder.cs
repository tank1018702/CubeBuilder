using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocalChunkBuilder : MonoBehaviour
{

    public Transform Center;

    public ChunkManager manager;

    [HideInInspector]
    public Vector3 M_centerPoint
    {
        get
        {
            if(Center)
            {
                return Center.position;
            }
            return transform.position;
        }
    }

    Vector3 MaxBoundSize
    {
        get
        {
            return new Vector3(sizeX, sizeY, sizeZ) * sideLength;
        }
    }

    Vector3 CurBoundSize
    {
        get
        {
            uint x = (uint)Mathf.FloorToInt(sizeX * updateAmount);
            uint y = (uint)Mathf.FloorToInt(sizeY * updateAmount);
            uint z = (uint)Mathf.FloorToInt(sizeZ * updateAmount);
            return new Vector3(Mathf.Max(1,x),Mathf.Max(1, y),Mathf.Max(1, z)) * sideLength;
        }
    }

    Vector3 curMaxBoundsCenter;

    public uint sizeX, sizeY, sizeZ;

    [Range(0f,1f)]
    public float updateAmount;

    float sideLength = CubeMetrics.CHUNK_WIDTH * CubeMetrics.CUBE_SIDE_LENGTH;

    void Start()
    {
        curMaxBoundsCenter = CubeMetrics.WorldPositon2ChunkPosition(M_centerPoint);
        manager.RefreshByBounds(new Bounds(curMaxBoundsCenter, MaxBoundSize));
    }

    void CenterUpdata()
    {      
        Bounds b = new Bounds(M_centerPoint, CurBoundSize);
        Bounds b2 = new Bounds(curMaxBoundsCenter, MaxBoundSize);
        
        if(b2.Contains(b.max)&&b2.Contains(b.min))
        {   
            return ;     
        }        
        curMaxBoundsCenter = CubeMetrics.WorldPositon2ChunkPosition(M_centerPoint);
        manager.RefreshByBounds(new Bounds(curMaxBoundsCenter, MaxBoundSize));
    }
    
    void Update()
    {
        CenterUpdata();
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(curMaxBoundsCenter, MaxBoundSize);

        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(M_centerPoint, CurBoundSize);
    }
}
