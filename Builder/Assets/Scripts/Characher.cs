using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Characher : MonoBehaviour
{
    public float moveSpeed;

    public float gravity;

    public float jumpSpeed;

    float movementY;

    public float rotateSpeed;

    //CharacherCubeControlType ControlType
    //{
    //    get
    //    {
    //        return controlType;
    //    }
    //    set
    //    {
    //        if(controlType!=value)
    //        {
    //            controlType = value;
                
    //        }
    //    }
    //}
    CharacherCubeControlType controlType;

    CubeType cubeType;

    PreviewMode previewMode;

    CubeOrientate Orientate
    {
        get
        {
            return orientate;
        }
        set
        {
            if(orientate!=value)
            {
                orientate = value;
                //Debug.Log("orientate change");
                RefreshViewBox();
            }
        }
    }
    CubeOrientate orientate;

   

    float moveAmount;


    //Rigidbody rigid;

    CharacterController cc;
    Animator animator;
 
    public Transform previewCube;

    public PreviewBox previewCube2;

    public Transform camTransform;

    CubeController c;

    public  CubeGrid grid;


    float h,v;

    Vector3 va, vr;

    float turnAmount, forwardAmount;


   public  ParticleSystem destroy, build;




    private void Awake()
    {
        //rigid = GetComponent<Rigidbody>();
        cc = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
        previewCube.localScale = Vector3.one * (CubeMetrics.CUBE_SIDE_LENGTH + 0.01f);
        controlType = CharacherCubeControlType.reomve;
        cubeType = CubeType.debug;
    }


    void Start()
    {
       c= CubeController.Instence;


    }


    private void FixedUpdate()
    {
        h = Input.GetAxis("Horizontal");
        v = Input.GetAxis("Vertical");

        Vector3 move = v * Vector3.ProjectOnPlane(camTransform.forward, Vector3.up).normalized + h * Vector3.ProjectOnPlane(camTransform.right, Vector3.up).normalized;
     
        if(move.magnitude>1f)
        {
            move.Normalize();
        }
        
        move = transform.InverseTransformDirection(move);      
        move = Vector3.ProjectOnPlane(move, Vector3.up);
        turnAmount = Mathf.Atan2(move.x, move.z);
        forwardAmount = move.z;

        GravityUpdate();
        MovementUpdate();

    }

    void GravityUpdate()
    {
        if(!CheckGround())
        {
            movementY -= gravity * Time.fixedDeltaTime;
        }
    
       
    }

    void MovementUpdate()
    {
        transform.Rotate(0, turnAmount * rotateSpeed * Time.deltaTime, 0);
        Vector3 velocity = Vector3.ProjectOnPlane(transform.forward * forwardAmount * moveSpeed, Vector3.up);
        //rigid.MovePosition(rigid.position+ new Vector3(velocity.x, 0, velocity.z) * Time.deltaTime);

        cc.Move(new Vector3(velocity.x, movementY, velocity.z) * Time.deltaTime);

    }

    void CubeControlUpdate()
    {
        if(Input.GetMouseButtonDown(0)&&!EventSystem.current.IsPointerOverGameObject()&&CheckGround())
        {
            Debug.Log(va);
            animator.SetTrigger("Build");
            ParticleSystem p = Instantiate(destroy);
        
            if(controlType==CharacherCubeControlType.add)
            {
                p.transform.position = va;                
            }
            else
            {
                p.transform.position = vr;            
            }
            CubeData data = new CubeData(true, false, Orientate, cubeType);
            previewCube2.PreviewAction(data);
            p.Play();
        }
    }

    void CubeOrientateUpdate()
    {
        int orientTemp = (int)Orientate;

        if(Input.GetKeyDown(KeyCode.Q))
        {
            //Debug.Log("q");
            Orientate = orientTemp - 1 < 0 ? (CubeOrientate)(orientTemp + 3) : (CubeOrientate)(orientTemp - 1);
        }
        else if(Input.GetKeyDown(KeyCode.E))
        {
            //Debug.Log("e");
            Orientate = orientTemp + 1 > 3 ? (CubeOrientate)(orientTemp - 3) : (CubeOrientate)(orientTemp + 1);
        }

    }

    void CubePoslUpdate()
    {     
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (c.GetMouseRayPoint(ray, out Vector3 addPoint, out Vector3 removePoint))
        {
            va = addPoint;
            vr = removePoint;
        }
        else
        {
           va=vr = transform.position + transform.forward * CubeMetrics.CUBE_SIDE_LENGTH+transform.up;
        }

        if (controlType == CharacherCubeControlType.reomve)
        {
            //previewCube.position = CubeMetrics.WorldPosition2CubePosition(va);
            previewCube2.transform.position = CubeMetrics.WorldPosition2CubePosition(vr);
        }
        else
        {
            //previewCube.position = CubeMetrics.WorldPosition2CubePosition(vr);
            previewCube2.transform.position = CubeMetrics.WorldPosition2CubePosition(va);
        }
    }

    void AnimationUpdate()
    {
        animator.SetBool("IsGround", CheckGround());
        animator.SetFloat("Velocity",forwardAmount);
    }


    void Jump()
    {
        //rigid.velocity += new Vector3(0, jumpSpeed, 0);
        //cc.Move(new Vector3(0, jumpSpeed, 0));
        movementY = jumpSpeed;
        animator.SetTrigger("Jump");
    }


    bool CheckGround()
    {
        Vector3 p = transform.position - new Vector3(0, 0.05f, 0);
        if (Physics.CheckBox(p, new Vector3(0.35f, 0.15f, 0.35f), Quaternion.identity, ~(1 << 9)))
        {
            return true;
        }
        return false;
    }

    void Update()
    {
        CubeOrientateUpdate();
        CubePoslUpdate();
        CubeControlUpdate();
        AnimationUpdate(); 
        if(Input.GetKeyDown(KeyCode.Space)&&CheckGround())
        {  
            Jump();
        }
    }

    public void Selected(int index)
    {
        switch(index)
        {
            case 0:
                controlType = CharacherCubeControlType.reomve;
                cubeType = CubeType.debug;
                previewMode = PreviewMode.cube;
                break;
            case 1:
                controlType = CharacherCubeControlType.add;
                cubeType = CubeType.debug;
                previewMode = PreviewMode.cube;
                break;
            case 2:
                controlType = CharacherCubeControlType.add;
                cubeType = CubeType.brickWall;
                previewMode = PreviewMode.cube;
                break;
            case 3:
                controlType = CharacherCubeControlType.add;
                cubeType = CubeType.stoneWall_old;
                previewMode = PreviewMode.cube;
                break;
            case 4:
                controlType = CharacherCubeControlType.add;
                cubeType = CubeType.stoneWall;
                previewMode = PreviewMode.cube;
                break;
            case 5:
                controlType = CharacherCubeControlType.add;
                cubeType= CubeType.WoodenBox;
                previewMode = PreviewMode.cube;
                break;
            case 6:
                controlType = CharacherCubeControlType.add;
                cubeType = CubeType.wood;
                previewMode = PreviewMode.cube;
                break;
            case 7:
                controlType = CharacherCubeControlType.add;
                cubeType = CubeType.leaves;
                previewMode = PreviewMode.cube;
                break;
            case 8:
                controlType = CharacherCubeControlType.add;
                cubeType = CubeType.TNT;
                previewMode = PreviewMode.cube;
                break;
            case 9:
                controlType = CharacherCubeControlType.add;
                previewMode = PreviewMode.planBox;
                break;
            case 10:
                previewMode = PreviewMode.planBox;
                controlType = CharacherCubeControlType.copy;
                break;
        }
        RefreshViewBox();
    }

    void RefreshViewBox()
    {
        CubeData data = new CubeData(true, true, orientate, cubeType);
        //previewCube2.TriangulatePreviewCube(data);  
        previewCube2.TriangualtePreview(previewMode,orientate,data,controlType);
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawCube(transform.position - new Vector3(0, 0.05f, 0), new Vector3(0.35f, 0.15f, 0.35f));
    }
}

public enum CharacherCubeControlType
{
    add,reomve,copy
}