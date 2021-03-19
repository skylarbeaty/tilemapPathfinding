using System.Collections;
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
    void SetGrid(){//sets up the grid, and resets if needed
        grid = tileMapData.TileMapToGrid();//get the tilemap data into nodes
        //set the neighbors on the nodes
        //run scan for paths to player
    }
    public void AStar(Node start, Node goal, System.Action<Stack<Vector3>> callback){//return a path on the grid to the callback, from start to goal
        List<Node> openSet = new List<Node>();//nodes that are to be looked at
        //per node variable that are not needed after search
        Dictionary<Node, float> gCost = new Dictionary<Node, float>();//cost of the current best path to the node. gcost = parent.gcost + distance(parent, node)
        Dictionary<Node, float> fCost = new Dictionary<Node, float>();//combines gcost and heuristic to tell what nodes are most likely on path
        Dictionary<Node, Node> parent = new Dictionary<Node, Node>();//parent with the smallest gCost
        //initialize start node
        openSet.Add(start);
        gCost[start] = 0;
        fCost[start] = Heuristic(start, goal);

        while(openSet.Count != 0){
            //find the node with the lowest fcost
            Node current = LowestCostNode(openSet, fCost);//will swap for min-heap or prioritry queue
            
            if (current == goal)//if we've reached the end
                callback(TracePath(current, parent));//return the path
            
            openSet.Remove(current);
            //loop thorugh neighbors of the current node
            foreach(Node neighbor in GetNeighbors(current, grid)){
                //see what the cost to get to this neighbor through current would be. Current cost plus cost to get to neighbor. 
                float gCostTest = gCost[current] + Vector3.Distance(current.posWorld, neighbor.posWorld);//using will result in 1 for straight and 1.5 for corners moves
                if (!gCost.ContainsKey(neighbor) || gCostTest < gCost[neighbor]){//if this is a better path than this node has, make this its parent
                    parent[neighbor] = current;
                    gCost[neighbor] = gCostTest;
                    fCost[neighbor] = gCost[neighbor] + Heuristic(neighbor,goal);
                    if (!openSet.Contains(neighbor))
                        openSet.Add(neighbor);
                }

            }
        }
    }

    public float Heuristic(Node from, Node to){//use world space distance to calculate the heuristic, the estimation of cost
        return Vector3.Distance(from.posWorld, to.posWorld);//return the straight line distance 
    }

    Node LowestCostNode(List<Node> list, Dictionary<Node, float> costs){//this method finds the lowest cost node O(n), wont be needed with a min heap O(1)
        float lowestValue = Mathf.Infinity;
        Node lowest = list[0];
        foreach (Node n in list){
            if (costs[n] < lowestValue){
                lowestValue = costs[n];
                lowest = n;
            }
        }
        return lowest;
    }

    List<Node> GetNeighbors(Node node, Node[,] grid){//will be replaced by information on the node, checks in 8 directions for neigghbors
        List<Node> neighbors = new List<Node>();
        Vector2Int pos = node.posGrid;

        //add in each direction with bounds and walkable checks
        if (pos.x - 1 >= 0 && grid[pos.x - 1, pos.y].walkable) 
            neighbors.Add(grid[pos.x - 1, pos.y]);//left
        if (pos.x + 1 < grid.GetLength(0) && grid[pos.x + 1, pos.y].walkable) //GetLength allows for checking length on a specific dimension
            neighbors.Add(grid[pos.x + 1, pos.y]);//right
        if (pos.y - 1 >= 0 && grid[pos.x, pos.y - 1].walkable) 
            neighbors.Add(grid[pos.x, pos.y - 1]);//down
        if (pos.y + 1 < grid.GetLength(1) && grid[pos.x, pos.y + 1].walkable)
            neighbors.Add(grid[pos.x, pos.y + 1]);//up

        //add side directions when there is no corner blocking
        //need to bounds check both directions and walkable checks at the location and both flanks. This is to not warp through corners
        if (pos.x - 1 >= 0 && pos.y - 1 >= 0 && grid[pos.x - 1, pos.y - 1].walkable && grid[pos.x, pos.y - 1].walkable && grid[pos.x - 1, pos.y].walkable)
            neighbors.Add(grid[pos.x - 1, pos.y - 1]);//bottom left
        if (pos.x - 1 >= 0 && pos.y + 1 < grid.GetLength(1) && grid[pos.x - 1, pos.y + 1].walkable && grid[pos.x, pos.y + 1].walkable && grid[pos.x - 1, pos.y].walkable)
            neighbors.Add(grid[pos.x - 1, pos.y + 1]);//top left
        if (pos.x + 1 < grid.GetLength(0) && pos.y + 1 < grid.GetLength(1) && grid[pos.x + 1, pos.y + 1].walkable && grid[pos.x, pos.y + 1].walkable && grid[pos.x + 1, pos.y].walkable)
            neighbors.Add(grid[pos.x + 1, pos.y + 1]);//top right
        if (pos.x + 1 < grid.GetLength(0) && pos.y - 1 >= 0 && grid[pos.x + 1, pos.y - 1].walkable && grid[pos.x, pos.y - 1].walkable && grid[pos.x + 1, pos.y].walkable)//bottom left
            neighbors.Add(grid[pos.x + 1, pos.y - 1]);//bottom left

        return neighbors;
    }

    Stack<Vector3> TracePath(Node end, Dictionary<Node,Node> parent){//creates a stack of positions by tracing parents of each node
        Stack<Vector3> path = new Stack<Vector3>();//pushing end to start will pop start to end
        Node n = end;
        while(parent.ContainsKey(n)){//cycle back through the nodes, not including start node (which has no parent)
            path.Push(n.posWorld);//capturing the actual world pos to move to
            n = parent[n];//look at the preceding node
        }
        return path;
    }
}
