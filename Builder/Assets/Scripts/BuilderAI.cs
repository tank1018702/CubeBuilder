using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuilderAI : MonoBehaviour
{

    public CubeGrid grid;

    public static Queue<Vector3> pendingPosList;

    public static List<BuilderAI> allAI;


    public  Vector3 currentTarget;

    Dictionary<string, bool> map;

    Queue<PathNode> tempNodes;

    public float moveSpeed;

    public  bool needRefresh;

    Stack<Vector3> path;

    Animator animator;

    public ParticleSystem p;

    private void Awake()
    {
        if(pendingPosList==null)
        {
            pendingPosList = new Queue<Vector3>();
        }
        if(allAI==null)
        {
            allAI = new List<BuilderAI>();
        }
        map = new Dictionary<string, bool>();
        path = new Stack<Vector3>();
        tempNodes = new Queue<PathNode>();
        animator = transform.GetComponent<Animator>();
        needRefresh = false;
    }

    private void Start()
    {
        allAI.Add(this);
        StartCoroutine(AIFlow());
    }
    
    [ContextMenu("test")]
    void test()
    {
        pendingPosList.Enqueue(new Vector3(1.5f, 5.5f, 10.5f));
        Debug.Log("pos push");
        StartCoroutine(AIFlow());
    }


    public static void SetInfo()
    {
        for(int i=0; i<allAI.Count;i++)
        {
            allAI[i].needRefresh = true;
        }
    }


    IEnumerator AIFlow()
    {
        while(true)
        {
            if(pendingPosList.Count>0)
            {
                currentTarget = pendingPosList.Dequeue();
                CubeCoordinate start = new CubeCoordinate(transform.position, CubeCoordinate.CoordinateType.cubeWorld);
                CubeCoordinate end = new CubeCoordinate(currentTarget, CubeCoordinate.CoordinateType.cubeWorld).GetAdjacentCoordinate(AdjacentDirection.down);
               
                var curPath= GetPath(start, end);
                while (curPath.Count > 0)
                {   if(needRefresh)
                    {
                        start = new CubeCoordinate(transform.position, CubeCoordinate.CoordinateType.cubeWorld);
                        curPath = GetPath(start, end);
                        needRefresh = false;
                        if (curPath.Count == 0)
                        {
                            yield return null;
                            break;
                        }
                        
                    }
                    yield return MoveTo(curPath.Pop());
                }
                yield return Building(currentTarget);
             
            }
            yield return null;
        }
    }

    void PathRefresh()
    {
        needRefresh = true;
    }



    PathNode SearchRoad(CubeCoordinate start,CubeCoordinate target)
    {
        int d = CubeCoordinate.GetDistance(start, target);
        PathNode startNode = new PathNode(start, null, d); 
        PathNode nearestNode = startNode;
        tempNodes.Enqueue(startNode);
        map.Add(start.ToString(), false);
        
        while (tempNodes.Count>0)
        {
            PathNode curNode = tempNodes.Dequeue();

            if (curNode.coordinate.Equal(target))
            {
                Debug.DrawRay(target.ToVector3(CubeCoordinate.CoordinateType.cubeWorld), Vector3.up * 10f, Color.magenta, 1f);   
                return curNode;
            }
            for (int i = 0; i < 4; i++)
            {
                if (GetAdjacentPath(curNode.coordinate, (CubeOrientate)i, out CubeCoordinate result) && !map.ContainsKey(result.ToString()))
                {   
                    map.Add(result.ToString(), false);  
                    int distance = CubeCoordinate.GetDistance(result, target);

                    PathNode resNode = new PathNode(result, curNode, distance);
                    tempNodes.Enqueue(resNode);
                    Debug.DrawRay(result.ToVector3(CubeCoordinate.CoordinateType.cubeWorld), Vector3.up, Color.white, 1f);
                }
            }
            nearestNode = curNode.distance <= nearestNode.distance ? curNode : nearestNode;
        }
        return nearestNode;
    }


    bool GetAdjacentPath(CubeCoordinate coordinate, CubeOrientate orientate,out CubeCoordinate result)
    {
        AdjacentDirection temp=AdjacentDirection.front;
        switch (orientate)
        {
            case CubeOrientate.back:
                temp = AdjacentDirection.back;
                break;
            case CubeOrientate.left:
                temp = AdjacentDirection.left;
                break;
            case CubeOrientate.right:
                temp = AdjacentDirection.right;
                break;
        }
        if (HasCube(coordinate.GetAdjacentCoordinate(temp, AdjacentDirection.up,AdjacentDirection.up)))
        {
            result = coordinate.GetAdjacentCoordinate(temp, AdjacentDirection.up, AdjacentDirection.up);
            return false;
        }
        if(HasCube(coordinate.GetAdjacentCoordinate(temp, AdjacentDirection.up)))
        {
            result = coordinate.GetAdjacentCoordinate(temp, AdjacentDirection.up);
            return true;
        }
        if(HasCube(coordinate.GetAdjacentCoordinate(temp)))
        {
            result = coordinate.GetAdjacentCoordinate(temp);
            return true;
        }
        if(HasCube(coordinate.GetAdjacentCoordinate(temp,AdjacentDirection.down)))
        {
            result= coordinate.GetAdjacentCoordinate(temp, AdjacentDirection.down);
            return true;
        }
        result = new CubeCoordinate(0, 0, 0);
        return false;
    }

    IEnumerator MoveTo(Vector3 target)
    {
        Debug.DrawLine(transform.position, target, Color.blue, 1f);
        Vector3 dir = (target - transform.position).normalized;
        dir.y = 0;
        transform.forward = dir;
        float distance = (transform.position - target).magnitude;

        float offsetY = Mathf.Abs(transform.position.y - target.y);
        Debug.Log(offsetY);
        if (offsetY > 0.1f)
        {
            animator.SetTrigger("Jump");
        }
        else
        {
            animator.SetBool("IsMove", true);
        }

        while (distance>float.Epsilon)
        {
            Vector3 newPostion = Vector3.MoveTowards(transform.position, target, moveSpeed * Time.deltaTime);
            transform.position = newPostion;
            distance = (newPostion - target).magnitude;
            yield return null;
        }
        if(offsetY>0.1f)
        {
            yield return new WaitForSeconds(0.2f);
        }
        
        animator.SetBool("IsMove", false);
    }

    IEnumerator Building(Vector3 target)
    {
        Vector3 dir = (target - transform.position).normalized;
        dir.y = 0;
        transform.forward = dir;

        animator.SetTrigger("Build");
        ParticleSystem partic = Instantiate(p);
        partic.transform.position = target;
        partic.Play();

        CubeData data = CubeData.ToCubeData(grid.GetCubeData(target));
        data.isTransparent = false;
        grid.SetCubeData(target, data.ToByte());

        SetInfo();

        yield return new WaitForSeconds(0.5f);

    }

    bool HasCube(CubeCoordinate coordinate)
    {
        byte cubeData = grid.GetCubeData(coordinate.ToVector3(CubeCoordinate.CoordinateType.cubeWorld));
        CubeData data = CubeData.ToCubeData(cubeData);
        return data.HasCube(false);
    }

    Stack<Vector3> GetPath(CubeCoordinate start, CubeCoordinate end)
    {
        path.Clear();
        map.Clear();
        tempNodes.Clear();
        PathNode final = SearchRoad(start, end);
        while ( final.previous != null)
        {
            final = final.previous;
            path.Push(final.coordinate.ToVector3(CubeCoordinate.CoordinateType.cubeWorld)+(Vector3.up*CubeMetrics.CUBE_SIDE_LENGTH*0.5f));
        }
        if(path.Count>0)
        {
            path.Pop();//第一个坐标就是自己的位置 ,出栈不要.
        }
       
        
        return path;
    }
}



public class PathNode
{
    public CubeCoordinate coordinate;

    public PathNode previous;

    public int distance;
    
    public PathNode(CubeCoordinate c,PathNode prev,int d)
    {
        coordinate = c;
        previous = prev;
        distance = d;
    }
}


public enum CoordinateState
{
    Explored,



}