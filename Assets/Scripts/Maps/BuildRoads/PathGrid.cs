using System;
using System.Collections.Generic;
using UnityEngine;

[Flags]
public enum BlockType
{
    None = 0,
    Tree = 1 << 0, // 1
    Water = 1 << 1, // 2
    Rock = 1 << 2  // 4
}

public struct PathObstacle
{
    public Vector2 worldPos;
    public float worldRadius;   // physical footprint of the tree, NOT the Poisson spacing radius
    public BlockType type;
}

/// <summary>
/// Coarse grid + A* built straight from the population points.
/// Build once per level, after every PopulateTerritory.IsReady, then feed it obstacles.
/// Paths are cached per (from territory, to territory, troop block mask).
/// </summary>
public class PathGrid : MonoBehaviour
{
    public static PathGrid Instance { get; private set; }

    [SerializeField] float cellSize = 0.5f;     // roughly one troop wide
    [SerializeField] float agentRadius = 0.2f;  // extra clearance around every obstacle
    [SerializeField] bool drawGizmos = true;

    Vector2 origin;
    int width, height;
    BlockType[] cells;

    // A* scratch buffers, reused between searches
    float[] gScore;
    int[] cameFrom;
    bool[] closed;
    readonly MinHeap open = new MinHeap();

    readonly Dictionary<long, Vector2[]> cache = new Dictionary<long, Vector2[]>();
    static readonly Vector2[] NoWaypoints = new Vector2[0];

    static readonly Vector2Int[] Dirs =
    {
        new Vector2Int( 1, 0), new Vector2Int(-1, 0), new Vector2Int(0,  1), new Vector2Int(0, -1),
        new Vector2Int( 1, 1), new Vector2Int( 1,-1), new Vector2Int(-1, 1), new Vector2Int(-1,-1),
    };

    void Awake() { Instance = this; }

    // ---------- setup ----------

    public void Build(Rect worldBounds)
    {
        origin = worldBounds.min;
        width = Mathf.CeilToInt(worldBounds.width / cellSize);
        height = Mathf.CeilToInt(worldBounds.height / cellSize);
        cells = new BlockType[width * height];
        gScore = new float[cells.Length];
        cameFrom = new int[cells.Length];
        closed = new bool[cells.Length];
        cache.Clear();
    }

    public void AddObstacle(PathObstacle o)
    {
        if (cells == null) return;

        float r = o.worldRadius + agentRadius;
        Vector2Int lo = WorldToCell(o.worldPos - new Vector2(r, r));
        Vector2Int hi = WorldToCell(o.worldPos + new Vector2(r, r));

        for (int y = Mathf.Max(lo.y, 0); y <= Mathf.Min(hi.y, height - 1); y++)
        {
            for (int x = Mathf.Max(lo.x, 0); x <= Mathf.Min(hi.x, width - 1); x++)
            {
                if ((CellCenter(x, y) - o.worldPos).sqrMagnitude <= r * r)
                    cells[y * width + x] |= o.type;
            }
        }
        cache.Clear(); // obstacles changed, old paths may be invalid
    }

    // ---------- query ----------

    /// <summary>
    /// Interior waypoints between from and to (start and end not included).
    /// Empty array = walk straight. Always safe to use: if the target is walled off
    /// it also returns empty, so troops fall back to a straight line instead of freezing.
    /// </summary>
    public Vector2[] GetWaypoints(int fromId, int toId, Vector2 from, Vector2 to, BlockType mask)
    {
        if (cells == null) return NoWaypoints;

        long key = ((long)fromId << 32) | ((long)toId << 8) | (long)mask;
        Vector2[] cached;
        if (cache.TryGetValue(key, out cached)) return cached;

        Vector2[] result = LineClear(from, to, mask) ? NoWaypoints : FindPath(from, to, mask);
        cache[key] = result;
        return result;
    }

    public Vector2[] GetPathTo(Vector2 from, Vector2 to,BlockType mask)
    {
        if(cells == null) return NoWaypoints;
        return LineClear(from, to, mask) ? NoWaypoints : FindPath(from, to, mask);
    }

    // ---------- A* ----------

    Vector2[] FindPath(Vector2 from, Vector2 to, BlockType mask)
    {
        Vector2Int s, g;
        if (!TryGetOpenCell(from, mask, out s) || !TryGetOpenCell(to, mask, out g))
            return NoWaypoints;

        int start = s.y * width + s.x;
        int goal = g.y * width + g.x;

        for (int i = 0; i < gScore.Length; i++)
        {
            gScore[i] = float.MaxValue;
            cameFrom[i] = -1;
            closed[i] = false;
        }

        open.Clear();
        gScore[start] = 0f;
        open.Push(start, Heuristic(s, g));

        while (open.Count > 0)
        {
            int cur = open.Pop();
            if (closed[cur]) continue;
            closed[cur] = true;

            if (cur == goal)
                return Smooth(Reconstruct(goal), from, to, mask);

            int cx = cur % width;
            int cy = cur / width;

            for (int d = 0; d < Dirs.Length; d++)
            {
                int dx = Dirs[d].x;
                int dy = Dirs[d].y;
                int nx = cx + dx;
                int ny = cy + dy;

                if (Blocked(nx, ny, mask)) continue;

                bool diagonal = dx != 0 && dy != 0;
                // no squeezing diagonally between two blocked cells
                if (diagonal && (Blocked(cx + dx, cy, mask) || Blocked(cx, cy + dy, mask))) continue;

                int n = ny * width + nx;
                if (closed[n]) continue;

                float ng = gScore[cur] + (diagonal ? 1.4142f : 1f);
                if (ng >= gScore[n]) continue;

                gScore[n] = ng;
                cameFrom[n] = cur;
                open.Push(n, ng + Heuristic(new Vector2Int(nx, ny), g));
            }
        }

        return NoWaypoints; // fully walled off
    }

    static float Heuristic(Vector2Int a, Vector2Int b)
    {
        int dx = Mathf.Abs(a.x - b.x);
        int dy = Mathf.Abs(a.y - b.y);
        return (dx + dy) + (1.4142f - 2f) * Mathf.Min(dx, dy); // octile distance
    }

    List<Vector2> Reconstruct(int goal)
    {
        List<Vector2> pts = new List<Vector2>();
        for (int c = goal; c != -1; c = cameFrom[c])
            pts.Add(CellCenter(c % width, c / width));
        pts.Reverse();
        return pts;
    }

    // Greedy string-pulling: skip every point we can already see past, so troops don't zig-zag along the grid.
    Vector2[] Smooth(List<Vector2> raw, Vector2 from, Vector2 to, BlockType mask)
    {
        raw[0] = from;
        raw[raw.Count - 1] = to;

        List<Vector2> result = new List<Vector2>();

        int i = 0;
        while (i < raw.Count - 1)
        {
            int j = raw.Count - 1;
            while (j > i + 1 && !LineClear(raw[i], raw[j], mask)) j--;
            if (j < raw.Count - 1) result.Add(raw[j]);
            i = j;
        }
        return result.ToArray();
    }

    // ---------- grid helpers ----------

    Vector2Int WorldToCell(Vector2 p)
    {
        return new Vector2Int(
            Mathf.FloorToInt((p.x - origin.x) / cellSize),
            Mathf.FloorToInt((p.y - origin.y) / cellSize));
    }

    Vector2 CellCenter(int x, int y)
    {
        return origin + new Vector2((x + 0.5f) * cellSize, (y + 0.5f) * cellSize);
    }

    bool Blocked(int x, int y, BlockType mask)
    {
        if (x < 0 || y < 0 || x >= width || y >= height) return true;
        return (cells[y * width + x] & mask) != BlockType.None;
    }

    bool LineClear(Vector2 a, Vector2 b, BlockType mask)
    {
        Vector2 delta = b - a;
        int steps = Mathf.Max(1, Mathf.CeilToInt(delta.magnitude / (cellSize * 0.2f)));
        Vector2Int ca = WorldToCell(a);
        Vector2Int cb = WorldToCell(b);

        Vector2Int prev = ca;
        for (int k = 0; k <= steps; k++)
        {
            Vector2Int c = WorldToCell(a + delta * (k / (float)steps));

            if (c != ca && c != cb && Blocked(c.x, c.y, mask))
                return false;

            int dx = c.x - prev.x;
            int dy = c.y - prev.y;
            if (dx != 0 && dy != 0 &&
                (Blocked(prev.x + dx, prev.y, mask) || Blocked(prev.x, prev.y + dy, mask)))
                return false;

            prev = c;
        }
        return true;
    }

    // If a start/end point lands in a blocked cell, snap to the nearest open one.
    bool TryGetOpenCell(Vector2 p, BlockType mask, out Vector2Int cell)
    {
        Vector2Int c = WorldToCell(p);
        for (int r = 0; r <= 4; r++)
        {
            for (int dy = -r; dy <= r; dy++)
            {
                for (int dx = -r; dx <= r; dx++)
                {
                    if (Mathf.Max(Mathf.Abs(dx), Mathf.Abs(dy)) != r) continue; // ring only
                    if (!Blocked(c.x + dx, c.y + dy, mask))
                    {
                        cell = new Vector2Int(c.x + dx, c.y + dy);
                        return true;
                    }
                }
            }
        }
        cell = default(Vector2Int);
        return false;
    }

    //for ai 
    public float GetPathDistance(Vector2 from, Vector2 to, BlockType mask)
    {
        Vector2[] waypoints = GetPathTo(from, to, mask);

        // Straight path
        if (waypoints.Length == 0)
            return Vector2.Distance(from, to);

        float distance = 0f;
        Vector2 current = from;

        foreach (Vector2 point in waypoints)
        {
            distance += Vector2.Distance(current, point);
            current = point;
        }

        distance += Vector2.Distance(current, to);

        return distance;
    }

    // ---------- debug ----------

    void OnDrawGizmosSelected()
    {
        if (!drawGizmos || cells == null) return;
        Gizmos.color = new Color(1f, 0f, 0f, 0.35f);
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                if (cells[y * width + x] != BlockType.None)
                    Gizmos.DrawCube(CellCenter(x, y), Vector3.one * cellSize * 0.9f);
            }
        }
    }

    // ---------- tiny binary min-heap ----------

    class MinHeap
    {
        readonly List<int> items = new List<int>();
        readonly List<float> prios = new List<float>();

        public int Count { get { return items.Count; } }

        public void Clear() { items.Clear(); prios.Clear(); }

        public void Push(int item, float p)
        {
            items.Add(item);
            prios.Add(p);
            int i = items.Count - 1;
            while (i > 0)
            {
                int parent = (i - 1) / 2;
                if (prios[parent] <= prios[i]) break;
                Swap(i, parent);
                i = parent;
            }
        }

        public int Pop()
        {
            int top = items[0];
            int last = items.Count - 1;
            items[0] = items[last];
            prios[0] = prios[last];
            items.RemoveAt(last);
            prios.RemoveAt(last);

            int i = 0;
            while (true)
            {
                int l = 2 * i + 1;
                int r = l + 1;
                int m = i;
                if (l < items.Count && prios[l] < prios[m]) m = l;
                if (r < items.Count && prios[r] < prios[m]) m = r;
                if (m == i) break;
                Swap(i, m);
                i = m;
            }
            return top;
        }

        void Swap(int a, int b)
        {
            int ti = items[a]; items[a] = items[b]; items[b] = ti;
            float tp = prios[a]; prios[a] = prios[b]; prios[b] = tp;
        }
    }


}
