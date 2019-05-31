using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeGrid : MonoBehaviour
{
    int chunkCountX = 4, chunkCountY =2, chunkCountZ = 4;

    int cubeCellCountX, cubeCellCountY, cubeCellCountZ;

   

    public CubeChunk chunkPrefab;



    public  Dictionary<string, CubeChunk> chunks;

    public  Dictionary<string, byte[]> chunkDatas;

     public ChunkManager manager;



    private void Awake()
    {
        cubeCellCountX = chunkCountX * CubeMetrics.CHUNK_WIDTH;
        cubeCellCountY = chunkCountY * CubeMetrics.CHUNK_WIDTH;
        cubeCellCountZ = chunkCountZ * CubeMetrics.CHUNK_WIDTH;
        chunks = new Dictionary<string, CubeChunk>();
        chunkDatas = new Dictionary<string, byte[]>();
        //CreateChunks_test();
    }


    void CreateChunks_test()
    {
        
        for (int y = -2, i = 0;y<chunkCountY;y++)
        {
            for(int z=-2;z<chunkCountZ;z++)
            {
                for(int x=-2;x<chunkCountX;x++)
                {
                    CubeChunk chunk = ChunkPool.Instance.Get();
                    chunk.grid = this;
                    
                    chunk.transform.SetParent(transform);
                    chunk.transform.name = "Chunk" + x + "_" + y + "_" + z+"_index"+(i-1)+"C"+(x+y*chunkCountY*chunkCountY + z*chunkCountZ);
                   
                    chunk.ChunkCoordinate = new CubeCoordinate(x,y,z);
                    chunks[chunk.ChunkCoordinate.ToString()] = chunk;
                }
            }
        }

        foreach (var c in chunks)
        {
            manager.AddChunk(c.Value);
        }



    }



    private void Update()
    {
        //Debug.Log("chunk"+chunks.Count);
        //Debug.Log("chunkDatas" + chunkDatas.Count);
    }







    void AddCell(Vector3 position,int data)
    {
       
        
       
    }




   

    public void SetCubeData(Vector3 position, byte data)
    {
        Vector3 cubePosition = CubeMetrics.WorldPosition2CubePosition(position);
        CubeCoordinate chunkCoordinate = new CubeCoordinate(position, CubeCoordinate.CoordinateType.chunk);
        CubeCoordinate cubeCoordinate = new CubeCoordinate(position, CubeCoordinate.CoordinateType.cubeWorld);
        CubeChunk chunk = chunks[chunkCoordinate.ToString()];

        chunk.SetCubeData(cubePosition, data,true);
    }


    public CubeChunk GetAdjacentChunk(CubeChunk chunk,AdjacentDirection direction)
    {
        CubeCoordinate adjChunkCoordinate = chunk.ChunkCoordinate.GetAdjacentCoordinate(direction);
        if (chunks.ContainsKey(adjChunkCoordinate.ToString()))
        {
            return chunks[adjChunkCoordinate.ToString()];
        }
        else return null;  
    }

}
