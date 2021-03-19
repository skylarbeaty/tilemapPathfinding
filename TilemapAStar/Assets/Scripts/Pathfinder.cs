﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Pathfinder : MonoBehaviour
{
    public TileMapData tileMapData;
    Node[,] grid;
    public Node[,] Grid {get { return grid; } }
    void Awake(){
        SetGrid();
    }

    void SetGrid(){// O(n) sets up the grid, and can be used to reset if the tilemap changes in runtime
        grid = tileMapData.TileMapToGrid();//get the tilemap data into nodes
        foreach(Node n in grid)//set the neighbors for all the nodes on the grid ahead of time, once
            n.SetNeighbors(grid);
        //run scan for paths to player
    }

    public void AStar(Node start, Node goal, System.Action<Stack<Vector3>> callback){//return a path on the grid to the callback, from start to goal
        MinHeap<Node> openSet = new MinHeap<Node>(30);//nodes that are to be looked at
        //per node variable that are not needed after search
        Dictionary<Node, float> gCost = new Dictionary<Node, float>();//cost of the current best path to the node. gcost = parent.gcost + distance(parent, node)
        Dictionary<Node, Node> parent = new Dictionary<Node, Node>();//parent with the smallest gCost
        
        //initialize start node
        gCost[start] = 0;
        start.fCost = Heuristic(start, goal);
        openSet.Insert(start);

        while(openSet.Count != 0){
            //find the node with the lowest fcost
            Node current = openSet.Peek();//using a heap here gives O(1) look up
            
            if (current == goal){//if we've reached the end
                callback(TracePath(current, parent));//return the path
                return;
            }
            
            openSet.Remove();
            //loop thorugh neighbors of the current node
            foreach(Node neighbor in current.neighbors){
                //see what the cost to get to this neighbor through current would be. Current cost plus cost to get to neighbor. 
                float gCostTest = gCost[current] + Vector3.Distance(current.posWorld, neighbor.posWorld);//using will result in 1 for straight and 1.5 for corners moves
                if (!gCost.ContainsKey(neighbor) || gCostTest < gCost[neighbor]){//if this is a better path than this node has, make this its parent
                    parent[neighbor] = current;
                    gCost[neighbor] = gCostTest;
                    // fCost[neighbor] = gCost[neighbor] + Heuristic(neighbor,goal);
                    neighbor.fCost = gCost[neighbor] + Heuristic(neighbor,goal);
                    if (!openSet.Contains(neighbor))
                        openSet.Insert(neighbor);//new node will be inserted and sorted up
                    else
                        openSet.UpdateUp(neighbor);//update the node position in the heap since its compare value changed
                }
            }
        }
        Debug.Log("No path between start and end");
    }

    public void AStar(Vector3 startWorld, Vector3 goalWorld, System.Action<Stack<Vector3>> callback){//accessor with that finds the nodes at world positions to call into A*
        //find the nodes that (best) match those world positions
        Vector2Int startPosGrid = tileMapData.WorldPositionToGridPosition(startWorld);
        Vector2Int goalPosGrid = tileMapData.WorldPositionToGridPosition(goalWorld);
        Node start = Grid[startPosGrid.x, startPosGrid.y];
        Node goal = Grid[goalPosGrid.x, goalPosGrid.y];

        if (start.walkable && goal.walkable && start.posGrid != goal.posGrid)
            AStar(start, goal, callback);
        else {
            if (!start.walkable) Debug.Log("Start not walkable");
            if (!goal.walkable) Debug.Log("Goal not walkable");
            // if (start == goal) Debug.Log("Start is Goal");
        }
    }

    public float Heuristic(Node from, Node to){// O(1) use world space distance to calculate the heuristic, the estimation of cost
        return Vector3.Distance(from.posWorld, to.posWorld);//return the straight line distance 
    }

    Stack<Vector3> TracePath(Node end, Dictionary<Node,Node> parent){// O(n) creates a stack of positions by tracing parents of each node
        Stack<Vector3> path = new Stack<Vector3>();//pushing end to start will pop start to end
        Node n = end;
        while(parent.ContainsKey(n)){//cycle back through the nodes, not including start node (which has no parent)
            path.Push(n.posWorld);//capturing the actual world pos to move to
            n = parent[n];//look at the preceding node
        }
        return path;
    }
}
