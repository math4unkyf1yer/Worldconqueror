using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoadManager : MonoBehaviour
{
    public Vector3 start;
    public Vector3 end;

    [SerializeField] GameObject roadPrefab;
    public static RoadManager Instance { get; private set; }

    private void Awake()
    {
        // 1. If an instance already exists and it isn't this one, destroy this duplicate
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    List<Vector3> GenerateStraightRoad(Vector3 start, Vector3 end)
    {
        List<Vector3> points = new List<Vector3>();
        int segments = 6;

        for (int i = 0; i < segments; i++)
        {
            float t = i / (float)segments;
            Vector3 pos = Vector3.Lerp(start, end, t);

            pos.x += Random.Range(-0.10f, 0.3f);
            pos.y += Random.Range(-0.10f, 0.13f);

            points.Add(pos);
        }
        return points;
    }
    List<Vector3> GenerateCurveRoad(Vector3 start, Vector3 end)
    {
        List<Vector3> points = new List<Vector3>();
        int segments = 20;

        // Create two control points
        Vector3 controlA = start + new Vector3((end.x - start.x) * 0.3f, 2f, 0f);
        Vector3 controlB = start + new Vector3((end.x - start.x) * 0.6f, -2f, 0f);

        for (int i = 0; i <= segments; i++)
        {
            float t = i / (float)segments;

            // Quadratic Bezier formula
            Vector3 pos =
                Mathf.Pow(1 - t, 2) * start +
                2 * (1 - t) * t * controlA +
                Mathf.Pow(t, 2) * end;

            points.Add(pos);
        }

        return points;
    }

    public void DrawRoadsStraight(Vector3 start, Vector3 end)
    {
        List<Vector3> points = null;

        points = GenerateStraightRoad(start, end);

        GameObject roadObj = Instantiate(roadPrefab, transform);

        Road road = roadObj.GetComponent<Road>();

        road.lr.positionCount = points.Count;
        road.lr.SetPositions(points.ToArray());
    }

    public void DrawCurveLine(GameObject objects, Vector3 start, Vector3 end)
    {
        List<Vector3> points = GenerateCurveRoad(start, end);

        LineRenderer ln = objects.GetComponent<LineRenderer>();
        ln.positionCount = points.Count;
        ln.SetPositions(points.ToArray());
    }

}
