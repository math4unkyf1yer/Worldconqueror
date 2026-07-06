using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class FireBallPool : MonoBehaviour
{
    [SerializeField] private List<GameObject> fireBallList = new List<GameObject>();
    // Start is called before the first frame update

    public static FireBallPool Instance { get; private set; }
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public void AddFireBall(GameObject fireBall)
    {
        fireBall.SetActive(false);
        fireBallList.Add(fireBall);
    }
    public void RemoveFireBall(GameObject fireBall)
    {
        fireBall.SetActive(true);
        fireBallList.Remove(fireBall);
    }

    public GameObject GetFireBall()
    {
        if (fireBallList.Count > 0)
        {
            GameObject fire = fireBallList[0];
            RemoveFireBall(fire);
            return fire;
        }
        return null;
    }

}
