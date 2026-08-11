using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class PoissonDisk
{
    public static List<Vector2> GeneratePoints(float radius,Vector2 regionSize,int numSampleBeforeRejection)
    {
        //calculate size of grid 
        float cellSize = radius / Mathf.Sqrt(2);

        //compute how many cells fit in the region
        int gridWidth = Mathf.CeilToInt(regionSize.x / cellSize);
        int gridHeight = Mathf.CeilToInt(regionSize.y / cellSize);

        //creade a 2d grid storing index
        int[,] grid = new int[gridWidth, gridHeight];

        //final accepted position
        List<Vector2> points = new List<Vector2>();
        List<Vector2> spawnPoints = new List<Vector2>();

        //start with one point in the center
        spawnPoints.Add(regionSize / 2);

        //keep going until no more spawn points 
        while (spawnPoints.Count > 0)
        {
            //randomly piock spawn point
            int spawnIndex = Random.Range(0, spawnPoints.Count);
            Vector2 spawnCenter = spawnPoints[spawnIndex];
            //to track if we found a canditate point
            bool accepted = false;

            //tried a lot of times if not found one give up on it
            for (int i = 0; i < numSampleBeforeRejection; i++)
            {
                //generate ring for direction and at least a radius away from the point 
                float angle = Random.value * Mathf.PI * 2;
                float distance = Random.Range(radius, 2 * radius);

                //creates actual canditate
                Vector2 candidate = spawnCenter + new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * distance;

                //check if canditate is inside region and far enough from all other points
                if (IsValid(candidate, regionSize,cellSize,radius,points,grid))
                {
                    points.Add(candidate);
                    spawnPoints.Add(candidate);
                    grid[(int)(candidate.x / cellSize), (int)(candidate.y / cellSize)] = points.Count;
                    accepted = true;
                    break;
                }
            }
            if (!accepted)
                spawnPoints.RemoveAt(spawnIndex);
        }
        return points;
    }

    private static bool IsValid(Vector2 candidate, Vector2 regionSize, float cellSize, float radius, List<Vector2> points, int[,] grid)
    {
        //points need to be in region 
        if(candidate.x < 0 || candidate.x >= regionSize.x || candidate.y < 0 || candidate.y >= regionSize.y)
           return false;

        int cellX = (int)(candidate.x / cellSize);
        int cellY = (int)(candidate.y / cellSize);

        //make the search around the point
        int searchStartX = Mathf.Max(0, cellX - 2);
        int searchEndX = Mathf.Min(grid.GetLength(0) - 1, cellX + 2);
        int searchStartY = Mathf.Max(0, cellY - 2);
        int searchEndY = Mathf.Min(grid.GetLength(1) - 1, cellY + 2);

        for (int x = searchStartX; x <= searchEndX; x++)
        {
            for (int y = searchStartY; y <= searchEndY; y++)
            {
                int pointIndex = grid[x, y] - 1;
                if (pointIndex != -1)
                {
                    float sqrDist = (candidate - points[pointIndex]).sqrMagnitude;
                    if (sqrDist < radius * radius)
                        return false;
                }
            }
        }
        return true;
    }
}
