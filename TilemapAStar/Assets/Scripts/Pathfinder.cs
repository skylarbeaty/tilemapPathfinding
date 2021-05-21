using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pathfinder : MonoBehaviour
{
    TileMapData tileMapData;
    TileMapDebug tileMapDebug;
    Node[,] grid;
    public Node[,] Grid {get { return grid; } }
    Transform player { get { return transform; } }//this script is meant to sit on the player
    Vector2Int lastDijkstraPos;
    void Start(){
        tileMapDebug = FindObjectOfType<TileMapDebug>();
        tileMapData = FindObjectOfType<TileMapData>();
        SetGrid();
    }

    void SetGrid(){// O(n) sets up the grid, and can be used to reset if the tilemap changes in runtime
        grid = tileMapData.TileMapToGrid();//get the tilemap data into nodes
        foreach(Node n in grid)//set the neighbors for all the nodes on the grid ahead of time, once
            n.SetNeighbors(grid);
    }

    // A* pathfinding
    // optimized for random start and goal
    #region A*
    public void AStar(Node start, Node goal, System.Action<Stack<Vector3>> callback){//return a path on the grid to the callback, from start to goal
        MinHeap<Node> openSet = new MinHeap<Node>(30);//nodes that are to be looked at
        //per node variable that are not needed after search
        Dictionary<Node, float> gCost = new Dictionary<Node, float>();//cost of the current best path to the node. gcost = parent.gcost + distance(parent, node)
        Dictionary<Node, Node> parent = new Dictionary<Node, Node>();//parent with the smallest gCost
        
        //initialize start node
        gCost[start] = 0;
        start.cost = Heuristic(start, goal);//fCost = gCost (actual Path) + hCost (estimated distance to goal)
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
                float dist = Vector3.Distance(current.posWorld, neighbor.posWorld);//using will result in 1 for straight and 1.5 for corners moves. Tile weight could be added here
                float gCostTest = gCost[current] + dist;//find the gcost through current
                if (!gCost.ContainsKey(neighbor) || gCostTest < gCost[neighbor]){//if this is a better path than this node has, make this its parent
                    parent[neighbor] = current;
                    gCost[neighbor] = gCostTest;
                    neighbor.cost = gCost[neighbor] + Heuristic(neighbor,goal);//fCost
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

    #endregion

    // Dijkstra's Algorithm pathfinding
    // optimized for many paths to/from the same source (the player)
    // can not perform paths where the the source is not one of the endpoints
    #region Dijkstra

    void Update(){
        if (Input.GetKeyDown(KeyCode.Space)){//test running dijkstras
            //run once
            float timeStart = Time.realtimeSinceStartup;
            Node start = RandomWalkable();
            PathToPlayer(start.posWorld, TestCallback);

            float time = Time.realtimeSinceStartup - timeStart;
            float avg = time / 1;
            print("Paths traced: 1, Total Time: " + time + " seconds, Average Time: " + avg);
            
            //run 100 times
            timeStart = Time.realtimeSinceStartup;
            // player.position = playerStartPos;
            for (int i = 0; i < 100000; i++){
                start = RandomWalkable();
                PathToPlayer(start.posWorld, TestCallback);
            }

            time = Time.realtimeSinceStartup - timeStart;
            avg = time / 100000;
            print("Paths traced: 100,000, Total Time: " + time + " seconds, Average Time: " + avg);
            
            //run 1000 times
            timeStart = Time.realtimeSinceStartup;
            // player.position = player.position + Vector3.right;
            for (int i = 0; i < 1000000; i++){
                start = RandomWalkable();
                PathToPlayer(start.posWorld, TestCallback);
            }

            time = Time.realtimeSinceStartup - timeStart;
            avg = time / 1000000;
            print("Paths traced: 1,000,000, Total Time: " + time + " seconds, Average Time: " + avg);

            // player.position = playerStartPos;//return to original position
        }
    }

    void TestCallback(Stack<Vector3> blah){
        return;
    }

    Node RandomWalkable(){//gets a random walkable space for testing random inputs
        Node ret;
        do{
            int ix = Mathf.FloorToInt(Random.Range(0, Grid.GetLength(0) - 0.001f));
            int iy = Mathf.FloorToInt(Random.Range(0, Grid.GetLength(1) - 0.001f));
            ret = Grid[ix,iy];
        }while (!ret.walkable);
        return ret;
    }
    public void PathToPlayer(Vector3 startWorld, System.Action<Stack<Vector3>> callback){
        Vector2Int playerPos = tileMapData.WorldPositionToGridPosition(player.position);
        Vector2Int startGrid = tileMapData.WorldPositionToGridPosition(startWorld);
        
        if (lastDijkstraPos != playerPos){//does dijstra need to be updated: is the player on a new node?
            lastDijkstraPos = playerPos;
            Dijkstra(grid[playerPos.x, playerPos.y]);//this can be costly (more than A*) but will only happen a couple times a second when the player is moving
        }

        callback(TraceDijkstraPath(grid[startGrid.x, startGrid.y]));
    }

    Stack<Vector3> TraceDijkstraPath(Node startNode){
        Stack<Vector3> forwardTrace = new Stack<Vector3>();
        Node current = startNode;
        forwardTrace.Push(current.posWorld);//first node would be skipped otherwise

        while(current.parent != null){//trace back the parents to the player
            current = current.parent;//go to next parent
            forwardTrace.Push(current.posWorld);//add that parent
        }

        Stack<Vector3> reversedTrace = new Stack<Vector3>();
        while (forwardTrace.Count != 0){
            reversedTrace.Push(forwardTrace.Pop());//reverse the stack
        }

        return reversedTrace;
    }

    void Dijkstra(Node source){//creates a parent structure that can be traced to find a path to one location (the player)
        MinHeap<Node> openSet = new MinHeap<Node>(30);//nodes that are to be looked at
        HashSet<Node> touched = new HashSet<Node>();//nodes contained have had their costs set. Alternative to resetting all the costs

        //initialize start node
        source.cost = 0;//cost of the current best path to the node. gcost = parent.gcost + distance(parent, node)
        source.parent = null;
        touched.Add(source);
        openSet.Insert(source);

        while(openSet.Count != 0){
            //find the node with the lowest cost
            Node current = openSet.Peek();//using a heap here gives O(1) look up
            openSet.Remove();

            //loop thorugh neighbors of the current node
            foreach(Node neighbor in current.neighbors){
                //see what the cost to get to this neighbor through current would be. Current cost plus cost to get to neighbor. 
                float dist = Vector3.Distance(current.posWorld, neighbor.posWorld);//using will result in 1 for straight and 1.5 for corners moves. Tile weight could be added here
                float costTest = current.cost + dist;//find the gcost through current
                if (!touched.Contains(neighbor) || costTest < neighbor.cost){//if this is a better path than this node has, make this its parent
                    neighbor.parent = current;
                    neighbor.cost = costTest;
                    touched.Add(neighbor);
                    if (!openSet.Contains(neighbor))
                        openSet.Insert(neighbor);//new node will be inserted and sorted up
                    else
                        openSet.UpdateUp(neighbor);//update the node position in the heap since its compare value changed
                }
            }
        }
    }

    #endregion

}
