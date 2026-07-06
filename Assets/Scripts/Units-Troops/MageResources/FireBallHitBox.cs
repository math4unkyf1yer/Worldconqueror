using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireBallHitBox : MonoBehaviour
{
    private FireBall fireBallRef;
    int testingHit;
    private void Start()
    {
        fireBallRef = GetComponentInParent<FireBall>();
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Unit")
        {
            UnitTroop troopDamaged = collision.gameObject.GetComponent<UnitTroop>();

            troopDamaged.TakeDamage(fireBallRef.damage); 
        }
    }

    public void ExplotionHappen()
    {
        StartCoroutine(CloseDamageRing());
    }

    public IEnumerator CloseDamageRing()
    {
        yield return new WaitForSeconds(0.5f);
        fireBallRef.CloseProjectile();
    }
}
