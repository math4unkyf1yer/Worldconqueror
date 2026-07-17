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
        int segments = 8;

        for (int i = 0; i < segments; i++)
        {
            float t = i / (float)segments;
            Vector3 pos = Vector3.Lerp(start, end, t);

            pos.x += Random.Range(-0.13f, 0.3f);
            pos.y += Random.Range(-0.13f, 0.13f);

            points.Add(pos);
        }
        return points;
    }

    public void DrawRoads(Vector3 start, Vector3 end)
    {
        List<Vector3> points = null;

        points = GenerateStraightRoad(start, end);

        GameObject roadObj = Instantiate(roadPrefab, transform);

        Road road = roadObj.GetComponent<Road>();

        road.lr.positionCount = points.Count;
        road.lr.SetPositions(points.ToArray());
    }
}
