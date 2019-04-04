using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeMetrics : MonoBehaviour
{
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

    public static Vector3 GetCubePosByDirection(Vector3 pos,CubeNeighborDirection direction)
    {  
        switch (direction)
        {
            case CubeNeighborDirection.front:
                pos += Vector3.forward;
                break;
            case CubeNeighborDirection.frontUp:
                pos += Vector3.forward + Vector3.up;
                break;
            case CubeNeighborDirection.frontDown:
                pos += Vector3.forward + Vector3.down;
                break;
            case CubeNeighborDirection.back:
                pos += Vector3.back;
                break;
            case CubeNeighborDirection.backUp:
                pos += Vector3.back + Vector3.up;
                break;
            case CubeNeighborDirection.backDown:
                pos += Vector3.back + Vector3.down;
                break;
            case CubeNeighborDirection.left:
                pos += Vector3.left;
                break;
            case CubeNeighborDirection.leftUp:
                pos += Vector3.left + Vector3.up;
                break;
            case CubeNeighborDirection.leftDown:
                pos += Vector3.left + Vector3.down;
                break;
            case CubeNeighborDirection.right:
                pos += Vector3.right;
                break;
            case CubeNeighborDirection.rightUp:
                pos += Vector3.right + Vector3.up;
                break;
            case CubeNeighborDirection.rightDown:
                pos += Vector3.right + Vector3.down;
                break;
            case CubeNeighborDirection.up:
                pos += Vector3.up;
                break;
            case CubeNeighborDirection.dowm:
                pos += Vector3.down;
                break;
                
        }
        return pos;
    }

    public static CubeNeighborDirection GetOppositeDirection(CubeNeighborDirection direction)
    {
        switch(direction)
        {
            case CubeNeighborDirection.front:
                return CubeNeighborDirection.back;
            case CubeNeighborDirection.frontUp:
                return CubeNeighborDirection.backDown;
            case CubeNeighborDirection.frontDown:
                return CubeNeighborDirection.backUp;

            case CubeNeighborDirection.back:
                return CubeNeighborDirection.front;
            case CubeNeighborDirection.backUp:
                return CubeNeighborDirection.frontDown;
            case CubeNeighborDirection.backDown:
                return CubeNeighborDirection.frontUp;

            case CubeNeighborDirection.left:
                return CubeNeighborDirection.right;
            case CubeNeighborDirection.leftUp:
                return CubeNeighborDirection.rightDown;
            case CubeNeighborDirection.leftDown:
                return CubeNeighborDirection.rightUp;

            case CubeNeighborDirection.right:
                return CubeNeighborDirection.left;
            case CubeNeighborDirection.rightUp:
                return CubeNeighborDirection.leftDown;
            case CubeNeighborDirection.rightDown:
                return CubeNeighborDirection.leftUp;

            case CubeNeighborDirection.up:
                return CubeNeighborDirection.dowm;
            case CubeNeighborDirection.dowm:
                return CubeNeighborDirection.up;

            default:
                return direction;
        }
    }

    public static Vector3 FromWorldPositionToCubePosition(Vector3 position)
    {
        Vector3 resut = Vector3.zero;
        resut.x = position.x > 0 ? (int)position.x * 1f + 0.5f : (int)position.x * 1f - 0.5f;
        resut.y = position.y > 0 ? (int)position.y * 1f + 0.5f : (int)position.y * 1f - 0.5f;
        resut.z = position.z > 0 ? (int)position.z * 1f + 0.5f : (int)position.z * 1f - 0.5f;
        return resut;
    }


}
public enum CubeNeighborDirection
{
    front,
    frontUp,
    frontDown,

    back,
    backUp,
    backDown,

    left,
    leftUp,
    leftDown,

    right,
    rightUp,
    rightDown,

    up,
    dowm,
}

public enum M_CubeType
{
    test1,
    test2,
    test3,
    test4,
    test5,
    test6
}

public enum CubeSurface
{
    front, right, back, left, up, down
}

public enum CubeOrientate
{
    front, right, back, left
}

public enum KeyDirection
{
    up, down, left, right
}


