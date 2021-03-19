using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TileMapData : MonoBehaviour //this script should sit on a tilemap that defines the walkable area
{
    [HideInInspector]
    public Tilemap tilemap;
    BoundsInt bounds;
    void Awake(){
        tilemap = GetComponent<Tilemap>();
        tilemap.CompressBounds();//cleans up bounds to furthest tiles (doesnt shirnk in editor changes)
        bounds = tilemap.cellBounds;
    }
    public Node[,] TileMapToGrid(){//creates a grid by reading over the tilemap and counting the tiles as floors
        //get tiles and bounds from tilemap
        TileBase[] tiles = tilemap.GetTilesBlock(bounds);//tilemaps cells are hard to index directly via cell, so this grabs them in a 1d array
        Node[,] grid = new Node[bounds.size.x, bounds.size.y];

        //loop through all the tiles and translate into output
        for(int x = 0; x < bounds.size.x; x++){
            for(int y = 0; y < bounds.size.y; y++){
                TileBase tile = tiles[x + y * bounds.size.x];//2d index into 1d array
                bool walkable = (tile != null);//if theres a tile here, its walkable
                Vector2Int posGrid = new Vector2Int(x,y);//tiles will be numbered starting in the bottom left
                Vector3 posWorld = tilemap.CellToWorld(GridPositionToCellPosition(posGrid)) + tilemap.tileAnchor;//finds the world position of the tile and adjusts for centering
                grid[x,y] = new Node(walkable, posGrid, posWorld);
            }
        }
        return grid;
    }

    // SPACE CONVERSIONS

    public Vector2Int WorldPositionToGridPosition(Vector3 posWorld){//take a world position (mouse click) and find what grid cell it should be on this tile map
        Vector3Int posCell = tilemap.WorldToCell(posWorld);
        Vector2Int posGrid = CellPositionToGridPosition(posCell);
        return posGrid;
    }

    Vector2Int CellPositionToGridPosition(Vector3Int posCell){//cell positions are x,y,layer and go out from a center to bounds, an array cannot have negative values, a conversion is needed
        //get the offset position
        Vector2Int ret = new Vector2Int(posCell.x - bounds.xMin, posCell.y - bounds.yMin);
        //clamp it avoid out of bounds (this does mean it it return the closest valid value sometimes, when clicking near where the map goes up to the edge)
        ret.x = Mathf.Clamp(ret.x, 0, bounds.size.x - 1);
        ret.y = Mathf.Clamp(ret.y, 0, bounds.size.y - 1);
        return ret;
    }

    Vector3Int GridPositionToCellPosition(Vector2Int posGrid){
        //get the offset position
        Vector3Int ret = new Vector3Int(posGrid.x + bounds.xMin, posGrid.y + bounds.yMin, 0);
        return ret;
    }
}
