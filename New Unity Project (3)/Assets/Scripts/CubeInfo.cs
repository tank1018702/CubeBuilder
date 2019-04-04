using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeInfo : MonoBehaviour
{
    public string cubeName;
    public CubeInfo[] neighbors;

    public M_CubeType type;


    CubeOrientate orientate;

    public CubeOrientate Orientate
    {
        get
        {
            return orientate;
        }
        set
        {
            
            switch(value)
            {
                case CubeOrientate.front:
                    transform.forward = Vector3.forward;
                    break;
                case CubeOrientate.back:
                    transform.forward = Vector3.back;
                    break;
                case CubeOrientate.left:
                    transform.forward = Vector3.left;
                    break;
                case CubeOrientate.right:
                    transform.forward = Vector3.right;
                    break;
            }
            orientate = value;
        }
    }


    

    public Vector3 Position
    {
        get
        {
            return transform.localPosition;
        }
    }

    public Vector3 PathPoint
    {
        get
        {
            return transform.position + Vector3.up;
        }
    }

    

    public void SetNeighbor(CubeNeighborDirection direction,CubeInfo cube)
    {
        neighbors[(int)direction] = cube;
        cube.neighbors[(int)CubeMetrics.GetOppositeDirection(direction)] = this;
    }
    
    public void RemoveNeighbor(CubeNeighborDirection direction)
    {
        neighbors[(int)direction] = null;
    }

    public CubeNeighborDirection GetDirectionByCube(CubeInfo Targetcube)
    {
        Vector3 dir = (Targetcube.Position - this.Position).normalized;

        if (dir.z > 0.6f)//前
        {
            if (dir.z > 0.8f)
            {
                return CubeNeighborDirection.front;
            }
            else
            {
                if (dir.y > 0.6f)
                {
                    return CubeNeighborDirection.frontUp;
                }
                return CubeNeighborDirection.frontDown;
            }
        }
        else if (dir.z < -0.6f)//后
        {
            if (dir.z < -0.8f)
            {
                return CubeNeighborDirection.back;
            }
            else
            {
                if (dir.y > 0.6f)
                {
                    return CubeNeighborDirection.backUp;
                }
                return CubeNeighborDirection.backDown;
            }
        }
        else if (dir.x > 0.6f)
        {
            if (dir.x > 0.8f)
            {
                return CubeNeighborDirection.right;
            }
            else
            {
                if (dir.y > 0.6f)
                {
                    return CubeNeighborDirection.rightUp;
                }
                return CubeNeighborDirection.rightDown;
            }
        }
        else
        {
            if (dir.x < -0.8f)
            {
                return CubeNeighborDirection.left;
            }
            else
            {
                if (dir.y > 0.6f)
                {
                    return CubeNeighborDirection.leftUp;
                }
                return CubeNeighborDirection.leftDown;
            }
        }
    }

    public bool CanHideSurface(CubeSurface surface)
    {
        if((int)surface<4)
        {
            int temp = (int)surface -(int)orientate;
            if(temp<0)
            {
                temp += 4;
            }
            

            switch((CubeOrientate)temp)
            {
                case CubeOrientate.front:
                    return neighbors[0];
                case CubeOrientate.back:
                    return neighbors[3];
                case CubeOrientate.left:
                    return neighbors[6];
                case CubeOrientate.right:
                    return neighbors[9];
                default:
                    return false;
            }
        }
        else if((int)surface == 4)
        {
            return neighbors[12];
        }
        else
        {
            return neighbors[13];
        }
        
    }
}


