using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThirdPesonCamera : MonoBehaviour
{
    private float Y_ANGLE_MIN = -80.0f;
    private float Y_ANGLE_MAX = 60.0f;

    public Transform lookAt;
    public Transform camTransform;
    public float distance = 10.0f;
    float currentDistance = 0f;
    public float height = 5.0f;

    private float currentX = 0.0f;
    private float currentY = 0.0f;
    private float lastX = 0.0f;
    private float lastY = 0.0f;
    [SerializeField]
    [Range(0f, 1f)]
    private float sensitivityX = 1f;
    [SerializeField]
    [Range(0f, 1f)]
    private float sensitivityY = 1f;
    [SerializeField]
    LayerMask CameraCheck;

    Vector3 MousePosition_start;
    Vector3 MousePosition_end;

    Quaternion CurrentRotation;



    private void Start()
    {
        camTransform = transform;

        
        CurrentRotation = lookAt.rotation;


    }

    private void Update()
    {

        if (Input.GetMouseButton(1))
        {
            MousePosition_start = Input.mousePosition;
            Vector3 MouseDir = MousePosition_start - MousePosition_end;
            currentX = (MouseDir.x / (Camera.main.pixelWidth * 0.5f)) * 180 * sensitivityX + lastX;
            currentY = (-MouseDir.y / (Camera.main.pixelHeight * 0.5f)) * 180 * sensitivityY + lastY;
            currentY = Mathf.Clamp(currentY, Y_ANGLE_MIN, Y_ANGLE_MAX);

        }
        else
        {
            lastX = currentX;
            lastY = currentY;
            MousePosition_end = Input.mousePosition;
        }

        //CharacherRayCast();

        //currentX += Input.GetAxis("Mouse X")*sensitivityX;
        //currentY -= Input.GetAxis("Mouse Y")*sensitivityY;

    }

    void CharacherRayCast(Vector3 charaPos, Quaternion rotation)
    {
        //Ray ray = new Ray(lookAt.transform.position, (transform.position - lookAt.transform.position).normalized);
        RaycastHit hit;
        Debug.DrawRay(charaPos, rotation * new Vector3(0, 0, -10f));
        if (Physics.Raycast(charaPos, rotation * new Vector3(0, 0, -10f), out hit, 100f, CameraCheck))
        {
            //Debug.Log(hit.transform.name);
            float d = (hit.point - lookAt.transform.position).magnitude;

            if (d < distance)
            {
                currentDistance = Mathf.Lerp(currentDistance, d, 0.1f);
            }
        }
        else
        {
            currentDistance = Mathf.Lerp(currentDistance, distance, 0.1f);
        }

    }

    private void LateUpdate()
    {

        Vector3 lookAtP = lookAt.position+new Vector3(0,0.45f,0);

        //Vector3 dir =-new Vector3(0,0, lookAt.forward.z* distance)  ;
        //Debug.Log(lookAt.forward.x + ":" + lookAt.forward.y+":"+ lookAt.forward.z);
        Quaternion rotation;
        if (Input.GetMouseButton(1))
        {
            rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(currentY, currentX, 0), 0.1f);

            CurrentRotation = rotation;

        }
        else
        {
            rotation = CurrentRotation;
        }
        CharacherRayCast(lookAtP, rotation);
        Vector3 dir = new Vector3(0, 0, -currentDistance);
        camTransform.position = lookAtP + new Vector3(0, height, 0) + rotation * dir;

        camTransform.LookAt(lookAtP);
    }
}
