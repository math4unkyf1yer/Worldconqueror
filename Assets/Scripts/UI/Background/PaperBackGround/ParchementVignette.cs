using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(RawImage))]
public class ParchementVignette : MonoBehaviour
{
    [Range(0f, 1f)] public float strength = 0.72f;
    [Range(0.1f, 1f)] public float radius = 0.68f;
    public Color edgeColor = new Color(0.08f, 0.04f, 0.01f, 1f);
    public int resolution = 256;
    // Start is called before the first frame update
    void Start()
    {
        var tex = new Texture2D(resolution, resolution, TextureFormat.RGBA32, false);
        var pixels = new Color32[resolution * resolution];
        float half = resolution * 0.5f;

        for (int y = 0; y < resolution; y++)
            for (int x = 0; x < resolution; x++)
            {
                float dx = (x - half) / half;
                float dy = (y - half) / half;
                float dist = Mathf.Sqrt(dx * dx + dy * dy) / radius;
                float alpha = Mathf.Clamp01(dist * dist) * strength;

                pixels[y * resolution + x] = new Color32(
                    (byte)(edgeColor.r * 255),
                    (byte)(edgeColor.g * 255),
                    (byte)(edgeColor.b * 255),
                    (byte)(alpha * 255)
                );
            }

        tex.SetPixels32(pixels);
        tex.Apply();
        GetComponent<RawImage>().texture = tex;
    }

}
