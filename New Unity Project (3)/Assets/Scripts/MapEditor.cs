using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapEditor : MonoBehaviour
{
    public CubeMesh cubeMesh,preview;

   

    M_CubeType cubeType = M_CubeType.test1;

    CubeOrientate Orientate;



    void Start()
    {
        preview.UpdateCube(cubeType,Orientate);
    }


    public void TypeSelect(int type)
    {
        cubeType = (M_CubeType)type;
        preview.UpdateCube(cubeType,Orientate);
    }


    void Update()
    {
        PreviewControl();
        if(OrientateControl())
        {
            preview.UpdateCube(cubeType, Orientate);
        }
       
    }

    bool GetMouseRayPoint(out Vector3 addCubePosition, out Vector3 removeCubePosition)
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hitInfo;
        if (Physics.Raycast(ray, out hitInfo))
        {

            Debug.DrawRay(hitInfo.point, Vector3.up, Color.red);

            addCubePosition = CubeMetrics.FromWorldPositionToCubePosition(hitInfo.point - ray.direction * 0.001f);
            removeCubePosition = CubeMetrics.FromWorldPositionToCubePosition(hitInfo.point + ray.direction * 0.001f);
            return true;
        }
        addCubePosition = Vector3.zero;
        removeCubePosition = Vector3.zero;
        return false;
    }

    bool OrientateControl()
    {
        CubeOrientate temp = Orientate;
        if (Input.GetKeyDown(KeyCode.Q))
        {
            Orientate = (int)Orientate == 0 ? (CubeOrientate)3 : (CubeOrientate)Orientate - 1;
        }
        else if (Input.GetKeyDown(KeyCode.E))
        {
            Orientate = (int)Orientate == 3 ? (CubeOrientate)0 : (CubeOrientate)Orientate + 1;
        }

        if(temp!=Orientate)
        {
            return true;
        }

        return false;
    }

    void PreviewControl()
    {
        Vector3 addCubePosition;
        Vector3 removeCubePosition;
        if (GetMouseRayPoint(out addCubePosition, out removeCubePosition))
        {
            if (Input.GetMouseButtonDown(0))
            {

                cubeMesh.AddCube(addCubePosition, cubeType,Orientate);
            }
            else if (Input.GetMouseButtonDown(1))
            {
                Debug.DrawRay(removeCubePosition, Vector3.up, Color.blue, 1f);
                cubeMesh.RemoveCube(removeCubePosition);
            }
            else
            {
                preview.transform.position = addCubePosition;
            }
        }
        else
        {
            preview.transform.position = Vector3.down;
        }

    }


    private void OnDrawGizmos()
    {
        
        if (GetMouseRayPoint(out Vector3 cubePosition1,out Vector3 cubePosition2))
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireCube(cubePosition2, Vector3.one);
            Gizmos.color = Color.white;
            Gizmos.DrawWireCube(cubePosition1, Vector3.one);
        }
        
    }



}
