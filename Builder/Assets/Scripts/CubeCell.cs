using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeCell : MonoBehaviour
{

    public CubeChunk chunk;

    public CubeCoordinate coordinate;

    public CubeType type;

    public CubeOrientate orientate;




    [SerializeField]
    CubeCell[] neighbors;

    private void Awake()
    {
        neighbors = new CubeCell[14];
    }

    //public void SetNeighbor(CubeAdjacentDirection direction,CubeCell cell)
    //{
    //    neighbors[(int)direction] = cell;
    //    cell.neighbors[(int)direction.CubeOpposite()] = this;
    //}

    //public CubeCell GetNeighbor(CubeAdjacentDirection direction)
    //{
    //    return neighbors[(int)direction];
    //}


    //void Refresh()
    //{
    //    if(chunk)
    //    {
    //        chunk.Refresh();
    //        for(int i=0;i<neighbors.Length;i++)
    //        {
    //            CubeCell neighbor = neighbors[i];
    //            if(neighbor&&neighbor.chunk!=chunk)
    //            {
    //                neighbor.chunk.Refresh();
    //            }
    //        }
    //    }
    //}

    public bool CanHideSurface(CubeSurface surface)
    {
        //前后左右
        if((int)surface<4)
        {
            int tempOrentate = ((int)surface - (int)orientate)<0? 
                ((int)surface - (int)orientate)+4:
                ((int)surface - (int)orientate);
            switch(tempOrentate)
            {
                case 0:
                    return neighbors[0];
                case 1:
                    return neighbors[3];
                case 2:
                    return neighbors[6];
                case 3:
                    return neighbors[9];
                default:
                    return false;
            }
        }//上
        else if((int) surface==4)
        {
            return neighbors[12];
        }
        else//下
        {
            return neighbors[13];
        }
        
    }
}

public struct CubeData
{
    public bool active;

    public bool isTransparent;

    public CubeType type;

    public CubeOrientate orientate;

    public CubeData(bool active,bool isTransparent ,CubeOrientate orientate,CubeType type)
    {
        this.active = active;
        this.isTransparent = isTransparent;
        this.orientate = orientate;
        this.type = type;
    }

    public static CubeData ToCubeData(byte data)
    {
        bool _active = (data & 1 << 0) != 0;
        bool _isTransparent = (data & 1 << 1) != 0;

        CubeOrientate _orientate = (CubeOrientate)((data & 3 << 2) >> 2);

        CubeType _type = (CubeType)((data & 15 << 4) >> 4);

        return new CubeData(_active, _isTransparent, _orientate, _type);
    }


    public byte ToByte()
    {
        byte data = Byte.MinValue;

        data = active ?(byte)(data | 1 << 0):data;

        data = isTransparent ? (byte)(data | 1 << 1) : data;

        data |= (byte)((int)orientate << 2);

        data |= (byte)((int)type << 4);

        return data;
    }


    public bool HasCube(bool currentIsTransparent)
    {
        if(currentIsTransparent)
        {
            return active && isTransparent;
        }
        else
        {
            return active && !isTransparent;
        }

  
    }


    //public bool HasCube
    //{
    //    get
    //    {
    //        return active && !isTransparent;
    //    }
    //}
    public override string ToString()
    {
        return active.ToString() + "," + isTransparent.ToString() + "," + orientate.ToString() + "," + type.ToString();
    }


}

