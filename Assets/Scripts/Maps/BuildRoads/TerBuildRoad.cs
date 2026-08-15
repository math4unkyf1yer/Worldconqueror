using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using UnityEngine;

public class TerBuildRoad : MonoBehaviour
{
    private RoadManager roadManager;
    private TerAura terAura;
    [SerializeField] LayerMask territoryLayer;
    public Collider2D ourCollider;
    public List<Collider2D> alreadyCollided = new List<Collider2D>();
        
    public float range = 1.5f; 

    [SerializeField] float push = 50;

    public void SetUp()
    {
        roadManager = RoadManager.Instance;
        ourCollider = GetComponent<Collider2D>();
        terAura = GetComponent<TerAura>();

        StartCoroutine(CloseCollider());
    }

    IEnumerator CloseCollider()
    {
        yield return new WaitForSeconds(0.4f);
        terAura.SetCollider();
    }


    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, range);

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Only collide with border layer
        if (collision.gameObject.layer != 11)
            return;

        // Get the territory we collided with
        TerBuildRoad other = collision.GetComponent<TerBuildRoad>();
        if (other == null || other == this)
            return;

        // If we already collided with this territory, do nothing
        if (alreadyCollided.Contains(other.ourCollider))
            return;

        // Add each other to the lists
        alreadyCollided.Add(other.ourCollider);
        other.alreadyCollided.Add(ourCollider);

        if(roadManager != null) { roadManager.DrawRoadsStraight(transform.position, other.transform.position); }
        // Draw the road
    }


}
