using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PathfindingClicker : MonoBehaviour//goes on an agent to pathfind to mouse clicks
{
    public TileMapData tileMapData;
    Pathfinder pathfinder;
    Node[,] grid;
    Stack<Vector3> path = new Stack<Vector3>();
    Vector3 target;
    float speed = 5, stopDistance = 0.01f;
    bool moving = false;
    void Start(){
        grid = tileMapData.TileMapToGrid();
        pathfinder = GetComponent<Pathfinder>();
    }
    void Update()
    {
        if(Input.GetMouseButtonDown(0)){
            SetTarget();
        }
        if (moving){
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
    void SetTarget(){
        //find the mouse on the grid
        Vector3 posWorldM = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2Int posGridM = tileMapData.WorldPointToGridPosition(posWorldM);
        //find own position on the grid
        Vector2Int posGrid = tileMapData.WorldPointToGridPosition(transform.position);
        //set up nodes
        Node start = grid[posGrid.x, posGrid.y];
        Node goal = grid[posGridM.x, posGridM.y];

        if (!start.walkable){
            Debug.Log("Not starting from a walkable node");
        }
        else if(goal.walkable){//this part needs updated when a* is in
            pathfinder.AStar(start, goal, grid, PathCallback);
            // if (moving){
            //     path.Enqueue(goal.posWorld);
            // }else{
            //     moving = true;
            //     target = goal.posWorld;
            // }
        }else{
            print("clicked node not walkable");
        }
    }

    public void PathCallback(Stack<Vector3> _path){
        path = _path;
        moving = true;
        target = path.Pop();
    }
}
