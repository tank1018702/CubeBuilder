using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{
    public CubeGrid grid;
    
    void Start()
    {
        
    }

    
    void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if(CubeController.Instence.GetMouseRayPoint(ray,out Vector3 pa,out Vector3 pb))
            {
                grid.SetCubeData(pb, 0);               
            }
        }
        
    }
}
