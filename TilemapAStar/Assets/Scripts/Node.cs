using UnityEngine;
using System;
using System.Collections.Generic;

public class Node : IComparable<Node>
{
    public bool walkable;
    public Vector2Int posGrid;
    public Vector3 posWorld;
    public float fCost = Mathf.Infinity;//real high number default
    public List<Node> neighbors = new List<Node>();
    public Node(bool _walkable, Vector2Int _posGrid, Vector3 _posWorld){
        walkable = _walkable;
        posGrid = _posGrid;
        posWorld = _posWorld;
    }

    public int CompareTo(Node other){//needed for IComparable, used for Heap
        return (int) (10f * (this.fCost - other.fCost));//multiplied by 10 to solve fractional costs (should only be 0.0 or 0.5)
    }

    public override string ToString(){//for proper print for Debug
        return "Node pos: " + posGrid.x + ", " + posGrid.y + ", fcost: " + fCost;
    }
    
    public void SetNeighbors(Node[,] grid){//creates references to the neighbors so they dont need to be calculated each time
        neighbors.Clear();//dont want duplicates. This should only be called once, unless the grid has updated
        Vector2Int pos = posGrid;

        //add in each cardinal direction with bounds and walkable checks
        if (pos.x - 1 >= 0 && grid[pos.x - 1, pos.y].walkable) 
            neighbors.Add(grid[pos.x - 1, pos.y]);//left
        if (pos.x + 1 < grid.GetLength(0) && grid[pos.x + 1, pos.y].walkable) //GetLength allows for checking length on a specific dimension
            neighbors.Add(grid[pos.x + 1, pos.y]);//right
        if (pos.y - 1 >= 0 && grid[pos.x, pos.y - 1].walkable) 
            neighbors.Add(grid[pos.x, pos.y - 1]);//down
        if (pos.y + 1 < grid.GetLength(1) && grid[pos.x, pos.y + 1].walkable)
            neighbors.Add(grid[pos.x, pos.y + 1]);//up

        //add oridinal directions when there is no corner blocking, to avoid warping through corners
        //need to bounds check both directions and walkable checks at the location and both flanks.
        if (pos.x - 1 >= 0 && pos.y - 1 >= 0 && grid[pos.x - 1, pos.y - 1].walkable && grid[pos.x, pos.y - 1].walkable && grid[pos.x - 1, pos.y].walkable)
            neighbors.Add(grid[pos.x - 1, pos.y - 1]);//bottom left
        if (pos.x - 1 >= 0 && pos.y + 1 < grid.GetLength(1) && grid[pos.x - 1, pos.y + 1].walkable && grid[pos.x, pos.y + 1].walkable && grid[pos.x - 1, pos.y].walkable)
            neighbors.Add(grid[pos.x - 1, pos.y + 1]);//top left
        if (pos.x + 1 < grid.GetLength(0) && pos.y + 1 < grid.GetLength(1) && grid[pos.x + 1, pos.y + 1].walkable && grid[pos.x, pos.y + 1].walkable && grid[pos.x + 1, pos.y].walkable)
            neighbors.Add(grid[pos.x + 1, pos.y + 1]);//top right
        if (pos.x + 1 < grid.GetLength(0) && pos.y - 1 >= 0 && grid[pos.x + 1, pos.y - 1].walkable && grid[pos.x, pos.y - 1].walkable && grid[pos.x + 1, pos.y].walkable)//bottom left
            neighbors.Add(grid[pos.x + 1, pos.y - 1]);//bottom left
    }
}
