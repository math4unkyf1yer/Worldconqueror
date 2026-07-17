using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Road : MonoBehaviour
{
    public LineRenderer lr;

    void Awake()
    {
        lr = GetComponent<LineRenderer>();
    }
}
