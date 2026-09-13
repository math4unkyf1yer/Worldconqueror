using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LandscapeCameraScaler : MonoBehaviour
{
    // Your PC aspect ratio (1920 / 1080 = 1.777...)
    public float referenceAspect = 16f / 9f;

    // Your current orthographic size on PC
    public float referenceSize = 5f;

    public float mobileScale = 1.1f;   // 1.05 = 5% smaller, adjust as needed

    void Start()
    {
        float currentAspect = (float)Screen.width / Screen.height;

        float size = referenceSize * (referenceAspect / currentAspect);

        // Apply slight shrink only on mobile
          #if UNITY_ANDROID || UNITY_IOS
            size *= mobileScale;
            #endif

        // Adjust camera size so territories look identical on all landscape screens
        Camera.main.orthographicSize = size;
    }
}
