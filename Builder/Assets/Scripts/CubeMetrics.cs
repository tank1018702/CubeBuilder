using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeMetrics
{
    public const float CUBE_SIDE_LENGTH = 1f;

    public const int CHUNK_WIDTH = 16;

    public static Vector3[] cubeVertex =
  { 
        //上面四个顶点
        //左上
        new Vector3(-0.5f,0.5f,0.5f),
        //右上
        new Vector3(0.5f,0.5f,0.5f),
        //右下
        new Vector3 (0.5f,0.5f,-0.5f),
        //左下
        new Vector3(-0.5f,0.5f,-0.5f),
        //下面四个顶点
        //左上
        new Vector3(-0.5f,-0.5f,0.5f),
        //右上
        new Vector3(0.5f,-0.5f,0.5f),
        //右下
        new Vector3(0.5f,-0.5f,-0.5f),
        //左下
        new Vector3(-0.5f,-0.5f,-0.5f)
    };

    public static Vector3 WorldPosition2CubePosition(Vector3 position)
    {
        Vector3 resut = Vector3.zero;
        resut.x = position.x > 0 ?
            (int)(position.x / CUBE_SIDE_LENGTH) * CUBE_SIDE_LENGTH + 0.5f * CUBE_SIDE_LENGTH :
            (int)(position.x / CUBE_SIDE_LENGTH) * CUBE_SIDE_LENGTH - 0.5f * CUBE_SIDE_LENGTH;
        resut.y = position.y > 0 ?
            (int)(position.y / CUBE_SIDE_LENGTH) * CUBE_SIDE_LENGTH + 0.5f * CUBE_SIDE_LENGTH :
            (int)(position.y / CUBE_SIDE_LENGTH) * CUBE_SIDE_LENGTH - 0.5f * CUBE_SIDE_LENGTH;
        resut.z = position.z > 0 ?
            (int)(position.z / CUBE_SIDE_LENGTH) * CUBE_SIDE_LENGTH + 0.5f * CUBE_SIDE_LENGTH :
            (int)(position.z / CUBE_SIDE_LENGTH) * CUBE_SIDE_LENGTH - 0.5f * CUBE_SIDE_LENGTH;
        return resut;
    }

    public static Vector3 WorldPositon2ChunkPosition(Vector3 position)
    {
        float x = position.x > 0 ? ((int)(position.x / (CUBE_SIDE_LENGTH * CHUNK_WIDTH)) + 0.5f) * CUBE_SIDE_LENGTH * CHUNK_WIDTH :
                                  ((int)(position.x / (CUBE_SIDE_LENGTH * CHUNK_WIDTH)) - 0.5f) * CUBE_SIDE_LENGTH * CHUNK_WIDTH;
        float y = position.y > 0 ? ((int)(position.y / (CUBE_SIDE_LENGTH * CHUNK_WIDTH)) + 0.5f) * CUBE_SIDE_LENGTH * CHUNK_WIDTH :
                                   ((int)(position.y / (CUBE_SIDE_LENGTH * CHUNK_WIDTH)) - 0.5f) * CUBE_SIDE_LENGTH * CHUNK_WIDTH;
        float z = position.z > 0 ? ((int)(position.z / (CUBE_SIDE_LENGTH * CHUNK_WIDTH)) + 0.5f) * CUBE_SIDE_LENGTH * CHUNK_WIDTH :
                                   ((int)(position.z / (CUBE_SIDE_LENGTH * CHUNK_WIDTH)) - 0.5f) * CUBE_SIDE_LENGTH * CHUNK_WIDTH;
        return new Vector3(x, y, z);
    }

    public const int MAX_HEIGHT = 20;

    public const float TerrainSeed = 896.72f;

    public static bool GetProbability(float vaule)
    {
        vaule = Mathf.Clamp01(vaule);
        float r = Random.value;
        return r < vaule ? true : false;
    }

    public static byte[] GetTerrainData(CubeChunk chunk)
    {
        byte[] data = new byte[CHUNK_WIDTH * CHUNK_WIDTH * CHUNK_WIDTH];



        for (int y = 0; y < CHUNK_WIDTH; y++)
        {
            for (int z = 0; z < CHUNK_WIDTH; z++)
            {
                for (int x = 0; x < CHUNK_WIDTH; x++)
                {
                    float cubeX = chunk.transform.position.x + x * CUBE_SIDE_LENGTH + CUBE_SIDE_LENGTH / 2f;
                    float cubeY = chunk.transform.position.y + y * CUBE_SIDE_LENGTH + CUBE_SIDE_LENGTH / 2f;
                    float cubeZ = chunk.transform.position.z + z * CUBE_SIDE_LENGTH + CUBE_SIDE_LENGTH / 2f;

                    Vector3 p = new Vector3(cubeX, cubeY, cubeZ);

                    float perlin = Mathf.PerlinNoise((p.x + TerrainSeed) * 0.010f, (p.z + TerrainSeed) * 0.010f) * 37f;
                    float currentHeight = p.y;
                    float heightDiff = Mathf.Abs(perlin - currentHeight);

                    if (perlin > currentHeight)
                    {
                        CubeType temp;
                        if (heightDiff < CUBE_SIDE_LENGTH)
                        {
                            temp = CubeType.grass;
                        }
                        else
                        {
                            if (currentHeight > 0)
                            {
                                temp = GetProbability(0.8f) ? CubeType.clay : CubeType.stone;
                            }
                            else if (currentHeight > -5 * CUBE_SIDE_LENGTH)
                            {
                                temp = GetProbability(0.5f) ? CubeType.stone : CubeType.clay;
                            }
                            else if (currentHeight > -10 * CUBE_SIDE_LENGTH)
                            {
                                temp = GetProbability(0.4f) ? CubeType.coal : CubeType.clay;
                            }
                            else if (currentHeight > -20 * CUBE_SIDE_LENGTH)
                            {
                                temp = GetProbability(0.2f) ? CubeType.copper : CubeType.stone;
                            }
                            else if (currentHeight > -30 * CUBE_SIDE_LENGTH)
                            {
                                temp = GetProbability(0.05f) ? CubeType.gold : CubeType.stone;
                            }
                            else
                            {
                                temp = GetProbability(0.01f) ? CubeType.diamond : CubeType.magma;
                            }
                        }
                        CubeData d = new CubeData(true, false, CubeOrientate.front, temp);
                        data[x + y * CHUNK_WIDTH * CHUNK_WIDTH + z * CHUNK_WIDTH] = d.ToByte();
                    }
                    else
                    {
                        data[x + y * CHUNK_WIDTH * CHUNK_WIDTH + z * CHUNK_WIDTH] = byte.MinValue;
                    }

                }
            }
        }

        return data;
    }

    public static Vector3 GetCubeVertexByOrientate(int index, CubeOrientate orientate)
    {
        switch (orientate)
        {
            case CubeOrientate.front:

                return Matrix3x3Operation(Vector3.right, Vector3.up, Vector3.forward, cubeVertex[index]);
            case CubeOrientate.right:
                return Matrix3x3Operation(Vector3.back, Vector3.up, Vector3.right, cubeVertex[index]);

            case CubeOrientate.back:
                return Matrix3x3Operation(Vector3.left, Vector3.up, Vector3.back, cubeVertex[index]);

            case CubeOrientate.left:
                return Matrix3x3Operation(Vector3.forward, Vector3.up, Vector3.left, cubeVertex[index]);
            default:
                return cubeVertex[index];
        }
    }

    static Vector3 Matrix3x3Operation(Vector3 m1, Vector3 m2, Vector3 m3, Vector3 v)
    {
        float x = v.x * m1.x + v.y * m2.x + v.z * m3.x;
        float y = v.x * m1.y + v.y * m2.y + v.z * m3.y;
        float z = v.x * m1.z + v.y * m2.z + v.z * m3.z;
        return new Vector3(x, y, z) * CUBE_SIDE_LENGTH;
    }

    public static int GetCubeIndexToChunk(int x, int y, int z)
    {
        return x + y * CHUNK_WIDTH * CHUNK_WIDTH + z * CHUNK_WIDTH;
    }

    public static int GetCubeIndex_FromWorldToLocal(int cubeX, int cubeY, int cubeZ, int chunkX, int chunkY, int chunkZ)
    {
        int x = CHUNK_WIDTH / 2 + cubeX - chunkX * CHUNK_WIDTH;
        int y = CHUNK_WIDTH / 2 + cubeY - chunkY * CHUNK_WIDTH;
        int z = CHUNK_WIDTH / 2 + cubeZ - chunkZ * CHUNK_WIDTH;
        return GetCubeIndexToChunk(x, y, z);
    }
}
public struct CubeCoordinate
{
    public enum CoordinateType
    {
        cubeLocal, cubeWorld, chunk
    }

    public int x;
    public int y;
    public int z;


    public CubeCoordinate(int x, int y, int z)
    {
        this.x = x;
        this.y = y;
        this.z = z;
    }

    public CubeCoordinate(Vector3 position, CoordinateType type)
    {
        if (type == CoordinateType.cubeWorld)
        {
            x = position.x > 0 ?
                Mathf.RoundToInt(((position.x - CubeMetrics.CUBE_SIDE_LENGTH * 0.5f) / CubeMetrics.CUBE_SIDE_LENGTH)) :
                Mathf.RoundToInt(((position.x + CubeMetrics.CUBE_SIDE_LENGTH * 0.5f) / CubeMetrics.CUBE_SIDE_LENGTH)) - 1;
            y = position.y > 0 ?
                Mathf.RoundToInt(((position.y - CubeMetrics.CUBE_SIDE_LENGTH * 0.5f) / CubeMetrics.CUBE_SIDE_LENGTH)) :
                Mathf.RoundToInt(((position.y + CubeMetrics.CUBE_SIDE_LENGTH * 0.5f) / CubeMetrics.CUBE_SIDE_LENGTH)) - 1;
            z = position.z > 0 ?
                Mathf.RoundToInt(((position.z - CubeMetrics.CUBE_SIDE_LENGTH * 0.5f) / CubeMetrics.CUBE_SIDE_LENGTH)) :
                Mathf.RoundToInt(((position.z + CubeMetrics.CUBE_SIDE_LENGTH * 0.5f) / CubeMetrics.CUBE_SIDE_LENGTH)) - 1;
        }
        else if (type == CoordinateType.chunk)
        {
            x = Mathf.RoundToInt(position.x / (CubeMetrics.CHUNK_WIDTH * CubeMetrics.CUBE_SIDE_LENGTH));
            y = Mathf.RoundToInt(position.y / (CubeMetrics.CHUNK_WIDTH * CubeMetrics.CUBE_SIDE_LENGTH));
            z = Mathf.RoundToInt(position.z / (CubeMetrics.CHUNK_WIDTH * CubeMetrics.CUBE_SIDE_LENGTH));
        }
        else
        {
            float tempx = position.x % (CubeMetrics.CHUNK_WIDTH * CubeMetrics.CUBE_SIDE_LENGTH);
            float tempy = position.y % (CubeMetrics.CHUNK_WIDTH * CubeMetrics.CUBE_SIDE_LENGTH);
            float tempz = position.z % (CubeMetrics.CHUNK_WIDTH * CubeMetrics.CUBE_SIDE_LENGTH);
            x = tempx > 0 ?
                Mathf.RoundToInt(((tempx - CubeMetrics.CUBE_SIDE_LENGTH * 0.5f) / CubeMetrics.CUBE_SIDE_LENGTH)) :
                Mathf.RoundToInt(((tempx + CubeMetrics.CUBE_SIDE_LENGTH * 0.5f) / CubeMetrics.CUBE_SIDE_LENGTH)) - 1;
            y = tempy > 0 ?
               Mathf.RoundToInt(((tempy - CubeMetrics.CUBE_SIDE_LENGTH * 0.5f) / CubeMetrics.CUBE_SIDE_LENGTH)) :
               Mathf.RoundToInt(((tempy + CubeMetrics.CUBE_SIDE_LENGTH * 0.5f) / CubeMetrics.CUBE_SIDE_LENGTH)) - 1;
            z = tempz > 0 ?
               Mathf.RoundToInt(((tempz - CubeMetrics.CUBE_SIDE_LENGTH * 0.5f) / CubeMetrics.CUBE_SIDE_LENGTH)) :
               Mathf.RoundToInt(((tempz + CubeMetrics.CUBE_SIDE_LENGTH * 0.5f) / CubeMetrics.CUBE_SIDE_LENGTH)) - 1;
        }
    }



    public bool Equal(CubeCoordinate c)
    {
        return x == c.x && y == c.y && z == c.z;
    }

    public override string ToString()
    {
        return "(" + x + "," + y + "," + z + ")";
    }

    public CubeCoordinate GetAdjacentCoordinate(AdjacentDirection dir)
    {
        switch (dir)
        {
            case AdjacentDirection.front: return new CubeCoordinate(x, y, z + 1);
            case AdjacentDirection.back: return new CubeCoordinate(x, y, z - 1);
            case AdjacentDirection.left: return new CubeCoordinate(x - 1, y, z);
            case AdjacentDirection.right: return new CubeCoordinate(x + 1, y, z);
            case AdjacentDirection.up: return new CubeCoordinate(x, y + 1, z);
            case AdjacentDirection.down: return new CubeCoordinate(x, y - 1, z);
            default: return this;
        }
    }

    public CubeCoordinate GetAdjacentCoordinate(params AdjacentDirection[] dirs)
    {
        CubeCoordinate temp = new CubeCoordinate(x, y, z);
        for(int i=0;i<dirs.Length;i++)
        {
            temp = temp.GetAdjacentCoordinate(dirs[i]);
        }
        return temp;
    }

    public static int GetDistance(CubeCoordinate c1,CubeCoordinate c2)
    {
        int x = Mathf.Abs(c1.x - c2.x);
        int y = Mathf.Abs(c1.y - c2.y);
        int z = Mathf.Abs(c1.z - c2.z);

        return x + y + z;
    }

    //public static CubeCoordinate ToCoordinate(Vector3 position,CoordinateType type)
    //{
    //    int x = position.x > 0 ? 
    //        Mathf.RoundToInt(((position.x - CubeMetrics.CUBE_SIDE_LENGTH * 0.5f) / CubeMetrics.CUBE_SIDE_LENGTH)) : 
    //        Mathf.RoundToInt(((position.x + CubeMetrics.CUBE_SIDE_LENGTH * 0.5f) / CubeMetrics.CUBE_SIDE_LENGTH)) - 1;
    //    int y = position.y > 0 ? 
    //        Mathf.RoundToInt(((position.y - CubeMetrics.CUBE_SIDE_LENGTH * 0.5f) / CubeMetrics.CUBE_SIDE_LENGTH)) : 
    //        Mathf.RoundToInt(((position.y + CubeMetrics.CUBE_SIDE_LENGTH * 0.5f) / CubeMetrics.CUBE_SIDE_LENGTH)) - 1;
    //    int z = position.z > 0 ? 
    //        Mathf.RoundToInt(((position.z - CubeMetrics.CUBE_SIDE_LENGTH * 0.5f) / CubeMetrics.CUBE_SIDE_LENGTH)) : 
    //        Mathf.RoundToInt(((position.z + CubeMetrics.CUBE_SIDE_LENGTH * 0.5f) / CubeMetrics.CUBE_SIDE_LENGTH)) - 1;
    //    return new CubeCoordinate(x, y, z);
    //}

    //只有本地方块坐标无法直接转换
    public Vector3 ToVector3(CoordinateType type)
    {
        float x, y, z;
        if (type == CoordinateType.cubeWorld)
        {
            x = this.x >= 0 ?
            this.x * CubeMetrics.CUBE_SIDE_LENGTH * 1.0f + CubeMetrics.CUBE_SIDE_LENGTH * 0.5f :
                     CubeMetrics.CUBE_SIDE_LENGTH * (this.x * 1.0f + 0.5f);
            y = this.y >= 0 ?
               this.y * CubeMetrics.CUBE_SIDE_LENGTH * 1.0f + CubeMetrics.CUBE_SIDE_LENGTH * 0.5f :
                        CubeMetrics.CUBE_SIDE_LENGTH * (this.y * 1.0f + 0.5f);
            z = this.z >= 0 ?
                this.z * CubeMetrics.CUBE_SIDE_LENGTH * 1.0f + CubeMetrics.CUBE_SIDE_LENGTH * 0.5f :
                         CubeMetrics.CUBE_SIDE_LENGTH * (this.z * 1.0f + 0.5f);
        }
        else if (type == CoordinateType.chunk)
        {
            x = this.x * CubeMetrics.CHUNK_WIDTH * CubeMetrics.CUBE_SIDE_LENGTH;
            y = this.y * CubeMetrics.CHUNK_WIDTH * CubeMetrics.CUBE_SIDE_LENGTH;
            z = this.z * CubeMetrics.CHUNK_WIDTH * CubeMetrics.CUBE_SIDE_LENGTH;
        }
        else
        {
            x = 0f;
            y = 0f;
            z = 0f;
        }
        return new Vector3(x, y, z);
    }
}

public enum AdjacentDirection
{
    front,
    back,
    left,
    right,
    up,
    down
}

public static class ChunkAdjacentDirectionExtensions
{
    public static AdjacentDirection ChunkOpposite(this AdjacentDirection direction)
    {
        return (int)direction % 2 == 0 ?
            (AdjacentDirection)((int)direction + 1) :
            (AdjacentDirection)((int)direction - 1);
    }
}


public enum CubeType
{
    coal, stone, diamond, gold,
    copper, magma, clay, grass,
    TNT, leaves, wood, WoodenBox,
    stoneWall, stoneWall_old, brickWall, debug

}

public enum CubeSurface
{
    front, right, back, left, up, down
}




public enum CubeOrientate
{
    front, right, back, left
}

public static class CubeOrientateExtensions
{
    public static CubeOrientate AddOrientate(this CubeOrientate orientate,CubeOrientate target)
    {
        return (CubeOrientate)(((int)orientate + (int)target) % 4);
    }
    public static CubeOrientate SubOrientate(this CubeOrientate orientate,CubeOrientate target)
    {
        int temp= ((int)orientate - (int)target) % 4;
        return temp < 0 ? (CubeOrientate)(temp + 4) : (CubeOrientate)temp;
    }
}

public enum KeyDirection
{
    up, down, left, right
}
