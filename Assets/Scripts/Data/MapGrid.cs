using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class GridCell
{
    public bool walkable;
    public int cost;
    public Vector2 worldPos;
}
public class MapGrid 
{
    public int width;
    public int height;
    public float cellSize;
    public GridCell[,] cells;

    public MapGrid(int width, int height, float cellSize)
    {
        this.width = width;
        this.height = height;
        this.cellSize = cellSize;

        cells = new GridCell[width, height];

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                cells[x, y] = new GridCell()
                {
                    walkable = true,
                    cost = 1,
                    worldPos = new Vector2(x * cellSize, y * cellSize)
                };
            }
        }
    }
}
