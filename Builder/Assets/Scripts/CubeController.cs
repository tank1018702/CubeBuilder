using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeController : MonoBehaviour
{

    public static CubeController Instence;

    private void Awake()
    {
        Instence = this;
    }

    public bool GetMouseRayPoint(Ray ray, out Vector3 addCubePosition, out Vector3 removeCubePosition)
    {
        Debug.DrawRay(ray.origin, ray.direction, Color.gray);

        if (Physics.Raycast(ray, out RaycastHit hitInfo))
        {

            Debug.DrawLine(ray.origin, hitInfo.point,Color.red);
            //Debug.Log(hitInfo.transform.name);

            addCubePosition = CubeMetrics.WorldPosition2CubePosition(hitInfo.point - ray.direction * 0.001f);
            removeCubePosition = CubeMetrics.WorldPosition2CubePosition(hitInfo.point + ray.direction * 0.001f);
            return true;
        }
        addCubePosition = Vector3.zero;
        removeCubePosition = Vector3.zero;
        return false;
    }

}
