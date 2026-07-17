using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowPool : MonoBehaviour
{
    [SerializeField] private List<GameObject> ArrowsList = new List<GameObject>();
    // Start is called before the first frame update

    public static ArrowPool Instance { get; private set; }
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public void AddArrow(GameObject arrow)
    {
        arrow.SetActive(false);
        ArrowsList.Add(arrow);
    }
    public void RemoveArrow(GameObject arrow)
    {
        arrow.SetActive(true);
        ArrowsList.Remove(arrow);
    }

    public GameObject GetFireBall()
    {
        if (ArrowsList.Count > 0)
        {
            GameObject arr = ArrowsList[0];
            RemoveArrow(arr);
            return arr;
        }
        return null;
    }
}
