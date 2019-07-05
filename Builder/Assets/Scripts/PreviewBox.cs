using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PreviewBox : MonoBehaviour
{
    PreviewMode mode;

    CharacherCubeControlType controlType;

    CubeOrientate PreviewOrientate;

    public CubeGrid grid;

    CubeMesh mesh;

    public int localScale;

    byte[] datas;

    Vector3[] allCubePoint;

    Vector3[] viewWireBoxVertices;

    Vector3[] tempCubeVertices;



    private void Awake()
    {
       
        datas = new byte[localScale * localScale * localScale];
        allCubePoint = new Vector3[localScale * localScale * localScale];
        viewWireBoxVertices = new Vector3[8];
        tempCubeVertices = new Vector3[8];
        mesh = GetComponent<CubeMesh>();
        
    }


    void Clear()
    {
        mesh.Clear();
    }

    void Apply()
    {
        mesh.Apply();
    }

   

    public void TriangulatePreviewCube(CubeData data)
    {
        Clear();
        TriangulateCube(transform.position, data);
        Apply();
    }

    void TriangulateCube(Vector3 position, CubeData data)
    {
        Vector3[] cubeVertices = GetCubeVertices(position, data.orientate);

        for (int i = 0; i < 6; i++)
        {
            TriangulateCubeSurface(cubeVertices, (CubeSurface)i, data);
        }
    }

    void TriangulateCube(int index)
    {
        Vector3 positon = allCubePoint[index];
        CubeData data = CubeData.ToCubeData( datas[index]);
        data.orientate = data.orientate.AddOrientate(PreviewOrientate);


        Vector3[] cubeVertices = GetCubeVertices(positon, data.orientate);

        

        for (int i=0;i<6;i++)
        {
            CubeOrientate temp = (CubeOrientate)i;
            if (i < 4)
            {
                temp = temp.SubOrientate(PreviewOrientate);
                temp = temp.AddOrientate(data.orientate);
            }
            if (!CheckAdjacent(index,(CubeSurface)temp))
            {
                TriangulateCubeSurface(cubeVertices, (CubeSurface)i, data);
            }
        }
    }



    public void TriangualtePreview(PreviewMode previewMode, CubeOrientate orientate, CubeData data,
        CharacherCubeControlType type)
    {

        mode = previewMode;
        PreviewOrientate = orientate;
        controlType = type;

        Clear();
        if (mode == PreviewMode.cube)
        {
            TriangulateCube(transform.position, data);
        }
        else
        {
            TriangulatePreviewWire(orientate);
            if (controlType == CharacherCubeControlType.add)
            {
                TriangualteAllCubes(orientate);
            }
        }
        Apply();
    }

    public void PreviewAction(CubeData data)
    {
        if(mode==PreviewMode.cube)
        {
            byte d;
            if(controlType==CharacherCubeControlType.add)
            {
                d = data.ToByte();
            }
            else
            {
                d = 0;
            }
            grid.SetCubeData(transform.position, data.ToByte());

        }
        else
        {
            if(controlType==CharacherCubeControlType.copy)
            {
                Debug.Log("getorientate:" + PreviewOrientate);
                GetDataFromPreviewBox(PreviewOrientate);

            }
            else
            {

                SetDataFromPreviewBox(PreviewOrientate);
            }
        }

    }

    void TriangualteAllCubes(CubeOrientate orientate)
    {
        GetCurrentCubePoints(orientate);
        for(int i=0;i<datas.Length;i++)
        {
            CubeData data = CubeData.ToCubeData(datas[i]);
            if(data.active)
            {
                TriangulateCube(i);
            }     
        }
    }


    void TriangulatePreviewWire(CubeOrientate orientate)
    {
        GetViewWireBoxVertices(orientate);
        mesh.AddQuad(viewWireBoxVertices[6], viewWireBoxVertices[7], viewWireBoxVertices[4], viewWireBoxVertices[5], 1);
        mesh.AddQuadUV(0f, 1f, 0f, 1f);
    }

    public Vector3[] GetViewWireBoxVertices(CubeOrientate orientate)
    {
        Vector3 dir = transform.forward;
        switch(orientate)
        {
            case CubeOrientate.back:
                dir = -transform.forward;
                break;
            case CubeOrientate.left:
                dir = -transform.right;
                break;
            case CubeOrientate.right:
                dir = transform.right;
                break;
        }
        Vector3 center = transform.position + dir * (1 + (localScale - 1) / 2) + transform.up * ((localScale - 1) / 2);
        for(int i=0;i<viewWireBoxVertices.Length;i++)
        {
            viewWireBoxVertices[i] = transform.InverseTransformPoint(center + CubeMetrics.cubeVertex[i]*localScale);
        }
        return viewWireBoxVertices;

    }

    public  Vector3 [] GetCurrentCubePoints(CubeOrientate orientate)
    {
        Vector3 dirX = transform.right;
        Vector3 dirZ = transform.forward;
        switch (orientate)
        {
            case CubeOrientate.back:
                dirX = -transform.right;
                dirZ = -transform.forward ;
                break;
            case CubeOrientate.left:
                dirX = transform.forward;
                dirZ = -transform.right ;
      
                break;
            case CubeOrientate.right:
                dirX = -transform.forward ;
                dirZ = transform.right;  
                break;
        }
        Vector3 deltaX = dirX * CubeMetrics.CUBE_SIDE_LENGTH;
        Vector3 deltaY = Vector3.up * CubeMetrics.CUBE_SIDE_LENGTH;
        Vector3 deltaZ = dirZ * CubeMetrics.CUBE_SIDE_LENGTH;
        Vector3 startPoint = transform.position + deltaZ
            - (deltaX * (localScale / 2));

        for(int y=0;y<localScale;y++)
        {
            for(int z=0;z<localScale;z++)
            {
                for(int x=0;x<localScale;x++)
                {
                    int index = x + y * localScale * localScale + z * localScale;
                    try
                    {   
                        allCubePoint[index] = startPoint + (x * deltaX) + (y * deltaY) + (z * deltaZ);
                    }
                    catch
                    {
                        Debug.LogError("index error :" + index + "(" + x + "," + y + "," + z + ")");
                    }     
                }
            }       
        }
        return allCubePoint;
    }

    bool CheckAdjacent(int index,CubeSurface surfaceTo)
    {
        
        int tempIndex=index;
        int y = tempIndex / (localScale *localScale);
        int z = (tempIndex - (y * localScale * localScale)) / localScale;
        int x = tempIndex - (y * localScale * localScale) - (z * localScale);


        switch(surfaceTo)
        {
            case CubeSurface.front:
                if(z+1>6)
                {
                    return false;
                }
                z += 1;
                break;
            case CubeSurface.back:
                if(z-1<0)
                {
                    return false;
                }
                z -= 1;
                break;
            case CubeSurface.left:
                if(x-1<0)
                {
                    return false;
                }
                x -= 1;
                break;
            case CubeSurface.right:
                if(x+1>6)
                {
                    return false;
                }
                x += 1;
                break;
            case CubeSurface.up:
                if(y+1>6)
                {
                    return false;
                }
                y += 1;
                break;
            case CubeSurface.down:
                if(y-1<0)
                {
                    return false;
                }
                y -= 1;
                break;
        }

        int i = x + y * localScale * localScale + z * localScale;
        CubeData data = CubeData.ToCubeData(datas[i]);
        return data.active;


    }



    Vector3[] GetCubeVertices(Vector3 position, CubeOrientate orientate)
    {
        for (int i = 0; i < tempCubeVertices.Length; i++)
        {
            tempCubeVertices[i] = transform.InverseTransformPoint(position + CubeMetrics.GetCubeVertexByOrientate(i, orientate));
        }
        return tempCubeVertices;
    }


    void GetDataFromPreviewBox(CubeOrientate orientate)
    {
        var allPoint = GetCurrentCubePoints(orientate);

        for (int i = 0; i < allPoint.Length; i++)
        {      
            CubeData data = CubeData.ToCubeData(grid.GetCubeData(allPoint[i]));
            if(!data.isTransparent&&data.active)
            {
                data.orientate = data.orientate.SubOrientate(orientate);
                datas[i] = data.ToByte();
            }
            else
            {
                datas[i] = byte.MinValue;
            }
        }
    }

    void SetDataFromPreviewBox(CubeOrientate orientate)
    {
        var allPoint = GetCurrentCubePoints(orientate);
        for (int i = 0; i < datas.Length; i++)
        {
            CubeData data = CubeData.ToCubeData(datas[i]);
            if(data.active)
            {
                data.orientate = data.orientate.AddOrientate(orientate);
                data.isTransparent = true;
                grid.SetCubeData(allPoint[i], data.ToByte());
                BuilderAI.pendingPosList.Enqueue(allPoint[i]);
            }
          
        }
    }


    void TriangulateCubeSurface(Vector3[] temp,CubeSurface surfaceTo,CubeData data )
    {
        float uCoordinate = ((int)surfaceTo * 1.0f) / 6.0f;
        float vCoordinate = ((int)data.type * 1.0f) / 16f * 1.0f;
        Vector2 uvBasePoint = new Vector2(uCoordinate, vCoordinate);

        switch (surfaceTo)
        {
            case CubeSurface.up:
                mesh.AddQuad(temp[0], temp[1], temp[2], temp[3]);        
                break;
            case CubeSurface.down:
                mesh.AddQuad(temp[5], temp[4], temp[7], temp[6]);
                break;
            case CubeSurface.left:
                mesh.AddQuad(temp[0], temp[3], temp[7], temp[4]);         
                break;
            case CubeSurface.right:
                mesh.AddQuad(temp[2], temp[1], temp[5], temp[6]);           
                break;
            case CubeSurface.front:
                mesh.AddQuad(temp[1], temp[0], temp[4], temp[5]);   
                break;
            case CubeSurface.back:
                mesh.AddQuad(temp[3], temp[2], temp[6], temp[7]);             
                break;
        }
        mesh.AddQuadUV(uvBasePoint, 16);
    }

    


    private void OnDrawGizmosSelected()
    {
        //Gizmos.color = Color.white;
        //if (allCubePoint == null)
        //{
        //    return;
        //}
        //for (int i = 0; i < allCubePoint.Length; i++)
        //{
        //    Gizmos.DrawWireCube(allCubePoint[i], Vector3.one * CubeMetrics.CUBE_SIDE_LENGTH);
        //}

        //Gizmos.color = Color.red;
        //Gizmos.DrawSphere(allCubePoint[0], CubeMetrics.CUBE_SIDE_LENGTH / 2f);
        //Gizmos.color = Color.yellow;
        //Gizmos.DrawSphere(allCubePoint[1], CubeMetrics.CUBE_SIDE_LENGTH / 2f);
        //Gizmos.color = Color.blue;
        //Gizmos.DrawSphere(allCubePoint[2], CubeMetrics.CUBE_SIDE_LENGTH / 2f);

        ////-------------------------------------------------------------------------------------
        //Gizmos.color = Color.red;
        //Gizmos.DrawLine(viewWireBoxVertices[0], viewWireBoxVertices[1]);
        //Gizmos.DrawLine(viewWireBoxVertices[1], viewWireBoxVertices[2]);
        //Gizmos.DrawLine(viewWireBoxVertices[2], viewWireBoxVertices[3]);
        //Gizmos.DrawLine(viewWireBoxVertices[3], viewWireBoxVertices[0]);

        //Gizmos.color = Color.yellow;
        //Gizmos.DrawLine(viewWireBoxVertices[5], viewWireBoxVertices[4]);
        //Gizmos.DrawLine(viewWireBoxVertices[4], viewWireBoxVertices[7]);
        //Gizmos.DrawLine(viewWireBoxVertices[7], viewWireBoxVertices[6]);
        //Gizmos.DrawLine(viewWireBoxVertices[6], viewWireBoxVertices[5]);

        //Gizmos.color = Color.green;
        //Gizmos.DrawLine(viewWireBoxVertices[0], viewWireBoxVertices[3]);
        //Gizmos.DrawLine(viewWireBoxVertices[3], viewWireBoxVertices[7]);
        //Gizmos.DrawLine(viewWireBoxVertices[7], viewWireBoxVertices[4]);
        //Gizmos.DrawLine(viewWireBoxVertices[4], viewWireBoxVertices[0]);

        //Gizmos.color = Color.cyan;
        //Gizmos.DrawLine(viewWireBoxVertices[2], viewWireBoxVertices[1]);
        //Gizmos.DrawLine(viewWireBoxVertices[1], viewWireBoxVertices[5]);
        //Gizmos.DrawLine(viewWireBoxVertices[5], viewWireBoxVertices[6]);
        //Gizmos.DrawLine(viewWireBoxVertices[6], viewWireBoxVertices[2]);

        //Gizmos.color = Color.blue;
        //Gizmos.DrawLine(viewWireBoxVertices[1], viewWireBoxVertices[0]);
        //Gizmos.DrawLine(viewWireBoxVertices[0], viewWireBoxVertices[4]);
        //Gizmos.DrawLine(viewWireBoxVertices[4], viewWireBoxVertices[5]);
        //Gizmos.DrawLine(viewWireBoxVertices[5], viewWireBoxVertices[1]);

        //Gizmos.color = Color.magenta;
        //Gizmos.DrawLine(viewWireBoxVertices[3], viewWireBoxVertices[2]);
        //Gizmos.DrawLine(viewWireBoxVertices[2], viewWireBoxVertices[6]);
        //Gizmos.DrawLine(viewWireBoxVertices[6], viewWireBoxVertices[7]);
        //Gizmos.DrawLine(viewWireBoxVertices[7], viewWireBoxVertices[3]);

    }
}

public enum PreviewMode
{
    cube,
    planBox
}
