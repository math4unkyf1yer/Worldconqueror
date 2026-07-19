using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerBuildRoad : MonoBehaviour
{
    private RoadManager roadManager;
    [SerializeField] LayerMask territoryLayer;
    float range = 3;
    private void Start()
    {

    }

    public void SetUp()
    {
        roadManager = RoadManager.Instance;

        //check if we bild roads if we do get out transform and target transform
        //no enemy see if there is any 
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, range, territoryLayer);

        foreach (Collider2D h in hits)
        {
            TerretoryController found = h.GetComponentInParent<TerretoryController>();

            if (found != null)
            {
                roadManager.DrawRoads(transform.position, found.transform.position);
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, range);
    }
}
