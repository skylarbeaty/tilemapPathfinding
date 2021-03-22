using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathfindingFollower : MonoBehaviour
{
    Transform follow;

    Pathfinder pathfinder;
    Stack<Vector3> path = new Stack<Vector3>();
    Vector3 target;
    bool moving = false;
    float speed = 4f, stopDistance = 0.8f;
    float updateLast = 0, updateTiming = 0.2f;

    void Start(){
        pathfinder = FindObjectOfType<Pathfinder>();
        follow = pathfinder.transform;
        // UpdatePath();
    }

    void Update(){
        if (Time.time - updateLast > updateTiming){//refresh every so often
            UpdatePath();
            updateLast = Time.time;
        }
        if (moving){
            //debug: draw line
            Vector3 lastPos = transform.position;
            foreach(Vector3 pos in path){//iterating through a stack is not ideal, but this is for testing only
                Debug.DrawLine(lastPos, pos, Color.magenta);
                lastPos = pos;
            }
            //move a little towards target
            Vector3 dir = (target - transform.position).normalized;
            Vector3 displacement = dir * speed * Time.deltaTime;
            transform.Translate(displacement);
            //check if at the target
            if (Vector3.Distance(transform.position, target) < stopDistance){
                if (path.Count > 0)//if there is more on the path, set the next target
                    target = path.Pop();
                else
                    moving = false;//if the path is complete, stop moving
            }
        }
    }
    
    void UpdatePath() => pathfinder.AStar(transform.position, follow.position, PathCallback);

    public void PathCallback(Stack<Vector3> _path){
        path = _path;
        moving = true;
        target = path.Pop();
    }
}
