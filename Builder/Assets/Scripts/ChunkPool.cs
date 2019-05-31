using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChunkPool : MonoBehaviour
{
    public static ChunkPool Instance;

    List<CubeChunk> chunkPool;

    public CubeChunk chunkPrefab;

    private void Awake()
    {
        Instance = this;

        chunkPool = new List<CubeChunk>();
    }


    public CubeChunk Get()
    {
        if(chunkPool.Count>0)
        {
            CubeChunk cubeChunk = chunkPool[chunkPool.Count - 1];
            chunkPool.RemoveAt(chunkPool.Count - 1);
            
            return cubeChunk;
        }
        else
        {
            return Instantiate(chunkPrefab);
        }
    }


    public void Set(CubeChunk chunk)
    {
        chunk.Init();
        chunk.GetComponent<Renderer>().enabled = false;
        chunkPool.Add(chunk);
    }

    

}
