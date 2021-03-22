using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TileMapDebug : MonoBehaviour
{
    Tilemap tilemap;
    TileMapData walkableMap;
    public TileBase[] arrows;
    void Start(){
        //set up the tilemap to match the walkable tilemap
        tilemap = GetComponent<Tilemap>();
        walkableMap = FindObjectOfType<TileMapData>();
        tilemap.size = walkableMap.tilemap.size;
        tilemap.origin = walkableMap.tilemap.origin;
        tilemap.ResizeBounds();
    }
    public void DiplayParentPaths(Node[,] grid){
        tilemap.ClearAllTiles();

        //set the tile for the correct arrow on all grid cells
        for(int x = 0; x > grid.GetLength(0); x++){
            for(int y = 0; y < grid.GetLength(1); y++){
                Node current = grid[x,y];
                Node parent = current.parent;
                Vector2Int dir = parent.posGrid - current.posGrid;
                TileBase tile = TileFromDirection(dir);
                Vector3Int position = Vector3Int.zero;//walkableMap.GridPositionToCellPosition(current.posGrid);
                tilemap.SetTile(position, tile);
            }
        }

        // tilemap.RefreshAllTiles();
    }
    TileBase TileFromDirection(Vector2Int dir){
        if (dir.x != 0 && dir.y != 0){//corners
            if (dir.x == 1 && dir.y == 1)
                return arrows[4];
            if (dir.x == 1 && dir.y == -1)
                return arrows[5];
            if (dir.x == -1 && dir.y == 1)
                return arrows[6];
            if (dir.x == -1 && dir.y == -1)
                return arrows[6];
        }
        if (dir.x == 1)
            return arrows[1];
        if (dir.x == -1)
            return arrows[2];
        if (dir.y == 1)
            return arrows[0];
        return arrows[3];
    }
}
