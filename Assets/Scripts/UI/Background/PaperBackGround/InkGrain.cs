using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(RawImage))]
public class InkGrain : MonoBehaviour
{
    public float opacity = 0.04f;
    public int resolution = 256;
    public int refreshRate = 3; // update every N frames

    Texture2D _tex;
    Color32[] _pixels;
    int _frame;
    // Start is called before the first frame update
    void Start()
    {
        _tex = new Texture2D(resolution, resolution, TextureFormat.RGBA32, false);
        _pixels = new Color32[resolution * resolution];
        GetComponent<RawImage>().texture = _tex;
        Refresh();
    }

    // Update is called once per frame
    void Update()
    {
        if (++_frame % refreshRate == 0) Refresh();
    }

    void Refresh()
    {
        byte a = (byte)(opacity * 255);
        for (int i = 0; i < _pixels.Length; i++)
        {
            byte v = (byte)Random.Range(60, 160);
            _pixels[i] = new Color32(v, (byte)(v * 0.8f), (byte)(v * 0.5f), a);
        }
        _tex.SetPixels32(_pixels);
        _tex.Apply();
    }
}
