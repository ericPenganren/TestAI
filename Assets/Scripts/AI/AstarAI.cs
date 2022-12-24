using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class AstarAI : MonoBehaviour
{

    public Transform target;
    private Seeker seeker;
    private CharacterController cc;
    ///用来表示路径
    public Path path;
    //角色每秒的速度
    public float speed = 100;
    //当角色与第一个航点写的距离小于这个值时，角色便可转向路径的下一个导航点
    public float nextWayPointDistance = 3;
    //角色正操其行进的航点
    private int currentWayPoint = 0;


    void Start()
    {
        seeker = GetComponent<Seeker>();
        cc = GetComponent<CharacterController>();
        //注册回调函数，在Astar Path 完成寻路后调用该函数
        seeker.pathCallback += OnPathComplete;
        //调用StarPath函数，开始到目标的寻路
        seeker.StartPath(transform.position,target.position);

    }

    void FixedUpdate()
    {
        if(path == null)
        {
            Debug.LogError("没有找到对应的路径！！！");
            return;
        }
        //如果当前路点的编号大于这条路径上路点的总数，那么已经到达路径的终点
        Debug.LogError(currentWayPoint +":"+path.vectorPath.Count);
        if(currentWayPoint >=path.vectorPath.Count )
        {
            Debug.LogError("寻路完成！！！");
            return;
        }
        //计算去往当前路点所需的行进方向和距离，控制觉得移动
        Vector3 dir = (path.vectorPath[currentWayPoint] = transform.position).normalized;
        dir*=speed*Time.fixedDeltaTime;
        cc.SimpleMove(dir);
        //角色转向目标
        Quaternion targetRotation = Quaternion.LookRotation(dir);
        transform.rotation = Quaternion.Slerp(transform.rotation,targetRotation,Time.deltaTime * speed);
        //如果当前位置与当前路点的距离小于一个给定值，可以转向下一个路点
        if(Vector3.Distance(transform.position,path.vectorPath[currentWayPoint]) < nextWayPointDistance)
        {
            currentWayPoint++;
            return;
        }

   }

   //当寻路结束后调用这个函数
   public void OnPathComplete(Path p)
   {
       Debug.Log("Find the path"+p.error);
       //如果找到了一条路径，那么保存，并且把第一个路点设置为当前路点
       if(!p.error)
       {
           path = p;
           currentWayPoint = 0;
       }
   }

   void OnDisable()
   {
       seeker.pathCallback-=OnPathComplete;
   }

    // Update is called once per frame
    void Update()
    {
        
    }
}
