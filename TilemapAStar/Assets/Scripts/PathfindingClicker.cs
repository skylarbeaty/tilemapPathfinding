using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PathfindingClicker : MonoBehaviour//goes on an agent to pathfind to mouse clicks
{
    public TileMapData tileMapData;
    Node[,] grid;
    Queue<Vector3> path = new Queue<Vector3>();
    Vector3 target;
    float speed = 5, stopDistance = 0.01f;
    bool moving = false;
    void Start(){
        grid = tileMapData.TileMapToGrid();
    }
    void Update()
    {
        if(Input.GetMouseButtonDown(0)){
            SetTarget();
        }
        if (moving){
            //move a little towards target
            Vector3 posNew = Vector3.Lerp(transform.position, target, speed * Time.deltaTime);
            transform.Translate(posNew - transform.position);
            //check if at the target
            if (Vector3.Distance(transform.position, target) < stopDistance){
                if (path.Count > 0)//if there is more on the path, set the next target
                    target = path.Dequeue();
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
            // print("clicked at: " + posWorldM + ", target at: " + goal.posWorld);
            if (moving){
                path.Enqueue(goal.posWorld);
            }else{
                moving = true;
                target = goal.posWorld;
            }
        }else{
            print("clicked node not walkable");
        }
    }
}
