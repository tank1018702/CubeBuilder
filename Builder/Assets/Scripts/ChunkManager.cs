using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[DefaultExecutionOrder(-100)]
public class ChunkManager : MonoBehaviour
{

    Stack<CubeChunk> pendingChunk;

    Dictionary<string, CubeChunk> chunks;

    List<string> removeList;

    public CubeGrid grid;


    Dictionary<string,CubeCoordinate> tempCoordinate;

    private void Awake()
    {
        pendingChunk = new Stack<CubeChunk>();
        chunks = new Dictionary<string, CubeChunk>();
        tempCoordinate = new Dictionary<string, CubeCoordinate>();
        removeList = new List<string>();
    }

    private void Start()
    {
        StartCoroutine(ChunkUpdate());
    }

    public void ProcessMark(CubeChunk chunk)
    {
        pendingChunk.Push(chunk);
    }



    Dictionary<string,CubeCoordinate> GetCoordinateByBounds(Bounds bounds)
    {
        tempCoordinate.Clear();
        float delta = CubeMetrics.CHUNK_WIDTH * CubeMetrics.CUBE_SIDE_LENGTH;
        //根据包围盒计算出需要显示的所有chunk的坐标

        for (float y = bounds.min.y; y <= bounds.max.y; y += delta)
        {
            for (float z = bounds.min.z; z <= bounds.max.z; z += delta)
            {
                for (float x = bounds.min.x; x <= bounds.max.x; x += delta)
                {
                    CubeCoordinate chunkCoordinate = new CubeCoordinate(new Vector3(x, y, z), CubeCoordinate.CoordinateType.chunk);
                    tempCoordinate.Add(chunkCoordinate.ToString(),chunkCoordinate);
                }
            }
        }

        return tempCoordinate;
      
    }


    void ProcessByCoordinate()
    { 
        foreach(var c in tempCoordinate)
        {
            CubeChunk chunk;

            if (grid.chunks.ContainsKey(c.Key))
            {  
                chunk = grid.chunks[c.Key];
            }
            else
            {
                chunk = ChunkPool.Instance.Get();
                grid.chunks.Add(c.Key, chunk);
                chunk.grid = grid;
                chunk.transform.SetParent(grid.transform);
                chunk.ChunkCoordinate = c.Value;
                chunk.ConnectNeighbors();

                //如果之前存有其数据
                if (grid.chunkDatas.ContainsKey(c.Key))
                {
                    chunk.SetAllCubeData(grid.chunkDatas[c.Key]);
                }
                else
                {
                    chunk.GeneratorTerrain();
                    grid.chunkDatas.Add(c.Key, chunk.GetAllCubeData());
                    
                }
                chunk.NeedRefresh = true;
                AddChunk(chunk);
            }
            chunk.isActiveNow = true;   
        }    
    }

  

    public void RefreshByBounds(Bounds bounds)
    { 
        GetCoordinateByBounds(bounds);
        removeList.Clear();
        foreach(var c in grid.chunks)
        {
            if(!tempCoordinate.ContainsKey(c.Key))
            {
                removeList.Add(c.Key);
            }
        }       
        for(int i=0;i<removeList.Count;i++)
        {
            CubeChunk chunk = grid.chunks[removeList[i]];
            
            if(grid.chunkDatas.ContainsKey(removeList[i]))
            {
                grid.chunkDatas[removeList[i]] = chunk.GetAllCubeData();
            }
            else
            {         
                grid.chunkDatas.Add(removeList[i], chunk.GetAllCubeData());
            }
            grid.chunks.Remove(removeList[i]);
            ChunkPool.Instance.Set(chunk);
        }
        
        ProcessByCoordinate();

    }

    public void AddChunk(CubeChunk chunk)
    {
        pendingChunk.Push(chunk);      
    }

    IEnumerator ChunkUpdate()
    {
        while (true)
        {

            if (pendingChunk.Count > 0)
            {
                CubeChunk chunk = pendingChunk.Pop();
                if (chunk.NeedRefresh)
                {
                    chunk.RefreshSelf();
                    chunk.SetVisible(true);
                    //chunk.GetComponent<Renderer>().enabled = true;
                }
                else
                {
                    continue;
                }
            }

            yield return null;
        }
    }


}
