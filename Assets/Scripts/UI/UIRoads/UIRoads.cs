using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIRoads : MonoBehaviour
{
    [Header("Road Prefab")]
    public GameObject road0;
    private GameObject roadObj;

    [Header("Settings")]
    public float segmentSpacing = 30f; // distance between UI segments

    List<Road> roadList = new List<Road>();

    [SerializeField] Material roadMaterial;
    [SerializeField] Material playerRoadMaterial;


    public void BuildRoad(Vector3 start, Vector3 end, Transform parent)
    {
        List<Vector3> points = null;

        points = GenerateCurveRoad(start, end);

         roadObj = Instantiate(road0, parent);

        Road road = roadObj.GetComponent<Road>();

        road.lr.sortingOrder = 1;
        road.lr.positionCount = points.Count;
        road.lr.SetPositions(points.ToArray());
        roadList.Add(road);
    }

    public void ReshapeRoad(Vector3 start, Vector3 end, int index)
    {
        if(index >= 0)
        {
            List<Vector3> points = null;
            points = GenerateCurveRoad(start, end);
            roadList[index].lr.positionCount = points.Count;
            roadList[index].lr.SetPositions(points.ToArray());
            ResetRoadTexture(roadList[index]);
        }
    }

    List<Vector3> GenerateCurveRoad(Vector3 start, Vector3 end)
    {
        List<Vector3> points = new List<Vector3>();
        int segments = 20;

        Vector3 dir = end - start;
        float curveStrength = dir.magnitude * 0.30f;

        Vector3 perp = new Vector3(-dir.y, dir.x, 0).normalized;

        // Push control points to OPPOSITE sides for an S-curve
        Vector3 controlA = start + dir * 0.33f + perp * curveStrength;
        Vector3 controlB = start + dir * 0.66f - perp * curveStrength;

        for (int i = 0; i <= segments; i++)
        {
            float t = i / (float)segments;
            float u = 1 - t;

            // Cubic Bezier: start, controlA, controlB, end
            Vector3 pos =
                u * u * u * start +
                3 * u * u * t * controlA +
                3 * u * t * t * controlB +
                t * t * t * end;

            points.Add(pos);
        }
        return points;
    }

    public void ChangeRoadTexture(int roadID)
    {
        roadList[roadID].lr.material = playerRoadMaterial;
    }
    public void ResetRoadTexture(Road road)
    {
        road.lr.material = roadMaterial;
    }
}
