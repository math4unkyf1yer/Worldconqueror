using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

public class PopulateTerritory : MonoBehaviour
{
    public Vector2 size = new Vector2 ();
    public float minSpacing = 1.5f;
    public int number = 30;

    public float displayRadius = 1f;
    List<Vector2> points;

    List<LevelData.VegetationData> vegetationData = new List<LevelData.VegetationData>();

    List<Vector2> allPoints = new List<Vector2>(); // for gizmos only
    private Dictionary<Sprite, int> currentCounts = new Dictionary<Sprite, int>();

    [SerializeField] LayerMask borderLayer;

    private const int MAX_PER_BATCH = 1023; // hard limit for DrawMeshInstanced
    private Mesh quadMesh;

    public bool IsReady { get; private set; }
    public bool testReady;
    public Shader shadertest;

    public bool menuTerritory;

    struct PlacedPoint
    {
        public Vector2 pos;
        public float radius;
    }


    public void Setup(List<LevelData.VegetationData> populatedArea)
    {

        vegetationData = populatedArea;

        currentCounts = vegetationData.ToDictionary(v => v.sprite, v => 0);

        BuildQuadMesh();
        BuildMaterials();

        if (menuTerritory)
        {
            StartCoroutine(WaiABit(0.1f));
        }
        else
        {
            StartCoroutine(WaiABit(0.1f));
        }
    }

    void BuildQuadMesh()
    {
        quadMesh = new Mesh();
        quadMesh.vertices = new Vector3[]
        {
        new Vector3(-0.5f, -0.5f, 0),
        new Vector3(0.5f, -0.5f, 0),
        new Vector3(0.5f, 0.5f, 0),
        new Vector3(-0.5f, 0.5f, 0)
        };
        quadMesh.uv = new Vector2[]
        {
        new Vector2(0, 0),
        new Vector2(1, 0),
        new Vector2(1, 1),
        new Vector2(0, 1)
        };
        quadMesh.triangles = new int[] { 0, 2, 1, 0, 3, 2 };
        quadMesh.RecalculateNormals();
        quadMesh.RecalculateBounds();

    }

    void BuildMaterials()
    {

        foreach (var veg in vegetationData)
        {
            Material mat = new Material(shadertest);

            // Assign texture
            if (mat.HasProperty("_BaseMap"))
                mat.SetTexture("_BaseMap", veg.sprite.texture);
            else
                mat.mainTexture = veg.sprite.texture;

            // Opaque surface
            mat.SetFloat("_Surface", 0f);
            mat.SetOverrideTag("RenderType", "Opaque");

            // Standard opaque blending
            mat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
            mat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);

            // Depth writing ON
            mat.SetInt("_ZWrite", 1);
            mat.renderQueue = (int)UnityEngine.Rendering.RenderQueue.Geometry;

            // Alpha clip
            mat.SetFloat("_AlphaClip", 1f);
            mat.SetFloat("_Cutoff", 0.5f);

            // ⭐ REQUIRED for proper cutout depth sorting
            mat.EnableKeyword("_ALPHATEST_ON");
            mat.EnableKeyword("_ALPHA_CLIP");

            mat.enableInstancing = true;

            veg.runtimeMaterial = mat;
        }
    }

    IEnumerator WaiABit(float time)
    {
        yield return new WaitForSeconds(time);
        PopulateTerritories();
        IsReady = true;
    }

    public void PopulateTerritories()
    {
        allPoints.Clear();
        List<PlacedPoint> placedPoints = new List<PlacedPoint>(); // all accepted points so far, across every type

        // giving them priority placement before smaller detail fills gaps
        var sortedVeg = vegetationData.OrderByDescending(v => v.radius).ToList();

        foreach (var veg in sortedVeg)
        {

            veg.matrices.Clear();
            // generate a candidate set using THIS prefab's own radius
            List<Vector2> candidates = PoissonDisk.GeneratePoints(veg.radius, size, number);

            foreach (Vector2 p in candidates)
            {
                if (currentCounts[veg.sprite] >= veg.maxCount)
                    break;

                Vector2 worldPos = transform.TransformPoint(p);

                if (Physics2D.OverlapPoint(worldPos, borderLayer) != null)
                    continue;

                // reject if too close to an already-placed point from ANY type
                bool tooClose = placedPoints.Any(other =>
                {
                    float minDist = Mathf.Max(veg.radius, other.radius);
                    return Vector2.Distance(p, other.pos) < minDist;
                });
                if (tooClose)
                    continue;

                float randomYRot = Random.Range(0f, 360f);


                Matrix4x4 matrix = Matrix4x4.TRS(new Vector3(worldPos.x, worldPos.y, 0f),Quaternion.identity, new Vector3(veg.scaleX, veg.scaleY, 1f));


                veg.matrices.Add(matrix);
                currentCounts[veg.sprite]++;
                // Store radius-aware point
                placedPoints.Add(new PlacedPoint
                {
                    pos = p,
                    radius = veg.radius
                });

                allPoints.Add(p);
            }

            // M42 = matrix.y position
            veg.matrices = veg.matrices.OrderByDescending(m => m.m13).ToList();
            veg.batches.Clear();
            for (int i = 0; i < veg.matrices.Count; i += MAX_PER_BATCH)
            {
                int count = Mathf.Min(MAX_PER_BATCH, veg.matrices.Count - i);
                veg.batches.Add(veg.matrices.GetRange(i, count).ToArray());
            }
        }
    }

    public void ClearTerritory()
    {
        allPoints.Clear();
        currentCounts.Clear();

        foreach (var veg in vegetationData)
        {
            veg.matrices.Clear();
            veg.batches.Clear();
        }

        IsReady = false;
    }

    private void Update()
    {
        foreach (var veg in vegetationData)
        {
            if (veg.runtimeMaterial == null) continue;

            foreach (var batch in veg.batches)
            {
              //  Debug.Log($"Drawing batch of {batch.Length} for {veg.sprite.name}");
                Graphics.DrawMeshInstanced(quadMesh, submeshIndex: 0, veg.runtimeMaterial, batch);
            }
        }
    }
    private void OnDrawGizmosSelected()
    {
        Vector2 bottomLeft = (Vector2)transform.position;
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(bottomLeft + size / 2f, size);

        if (allPoints != null)
        {
            foreach (Vector2 p in allPoints)
            {
                Vector2 worldPos = transform.TransformPoint(p);
                Gizmos.DrawSphere(worldPos, displayRadius);
            }
        }

    }
}
