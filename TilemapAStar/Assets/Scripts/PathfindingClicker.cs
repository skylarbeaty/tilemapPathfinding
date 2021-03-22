using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PathfindingClicker : MonoBehaviour//goes on an agent to pathfind to mouse clicks
{
    Pathfinder pathfinder;
    Stack<Vector3> path = new Stack<Vector3>();
    Vector3 target;
    float speed = 5f, stopDistance = 0.05f;
    bool moving = false;
    public bool drawDebugPath = true;
    // int testingWalkableLoops = 0;
    void Start(){
        pathfinder = GetComponent<Pathfinder>();
    }
    void Update()
    {
        if(Input.GetMouseButtonDown(0)){
            SetTarget();
        }
        // if (Input.GetKeyDown(KeyCode.T)){//TESTING: run a* a lot and see how long it takes
        //     TestAstar(1);
        //     TestAstar(100);
        //     TestAstar(1000);
        // }
        if (moving){
            //debug: draw line
            if (drawDebugPath){
                Vector3 lastPos = transform.position;
                foreach(Vector3 pos in path){//iterating through a stack is not ideal, but this is for testing only
                    Debug.DrawLine(lastPos, pos, Color.green);
                    lastPos = pos;
                }
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
    void SetTarget(){//tell the pathfinding system to give a new path
        //find the mouse on the grid
        Vector3 posWorldM = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        //send positions to pathfinder 
        pathfinder.AStar(transform.position, posWorldM, PathCallback);
    }

    public void PathCallback(Stack<Vector3> _path){//setup the new path
        path = _path;
        moving = true;
        target = path.Pop();
    }

    // void TestAstar(int iterations){//TESTING: run a* a lot and see how long it takes
    //     testingWalkableLoops= 0;
    //     float timeStart = Time.realtimeSinceStartup;
    //     int numIter = iterations;
    //     for (int i = 0; i < numIter; i++){
    //         Node start = RandomWalkable();
    //         Node end;
    //         do{
    //             end = RandomWalkable();
    //         }while(start == end);

    //         pathfinder.AStar(start, end, TestingCallback);
    //     }

    //     float time = Time.realtimeSinceStartup - timeStart;
    //     float avg = time / numIter;

    //     print("A* Iterations: " + numIter + "Total Time: " + time + " seconds, Average Time: " + avg + ", with " + testingWalkableLoops + " calls to RandomWalkable");
    // }

    // Node RandomWalkable(){//gets a random walkable space for testing random inputs
    //     Node ret;
    //     do{
    //         int ix = Mathf.FloorToInt(Random.Range(0, pathfinder.Grid.GetLength(0) - 0.001f));
    //         int iy = Mathf.FloorToInt(Random.Range(0, pathfinder.Grid.GetLength(1) - 0.001f));
    //         ret = pathfinder.Grid[ix,iy];
    //         testingWalkableLoops++;
    //     }while (!ret.walkable);
    //     return ret;
    // }

    // public void TestingCallback(Stack<Vector3> _path){
    //     // float length = _path.Count;
    //     // Vector3 lastPos = _path.Pop();
    //     // Vector3 pos;
    //     // while(_path.Count != 0){//iterating through a stack is not ideal, but this is for testing only
    //     //     pos = _path.Pop();
    //     //     Debug.DrawLine(lastPos, pos, Color.Lerp(Color.green, Color.blue, _path.Count / length), 5f);
    //     //     lastPos = pos;
    //     // }
    // }
}
