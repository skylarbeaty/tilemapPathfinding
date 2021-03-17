using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node 
{
    public bool walkable;
    public Vector2Int posGrid;
    public Vector3 posWorld;
    public Node(bool _walkable, Vector2Int _posGrid, Vector3 _posWorld){
        walkable = _walkable;
        posGrid = _posGrid;
        posWorld = _posWorld;
    }
}
