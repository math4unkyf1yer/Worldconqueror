using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrow : Projectiles
{
    private Coroutine activeToLongRoutine;
    public override void ReachTarget()
    {
        base.ReachTarget();
        // no need for effect
        activeToLongRoutine = StartCoroutine(ActivetoLong());
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Unit")
        {
            UnitTroop troopDamaged = collision.gameObject.GetComponent<UnitTroop>();

            if(troopDamaged && troopDamaged.ownercl != owner)
            {
                Debug.Log("Arrow hit");
                troopDamaged.TakeDamage(damage);

                CloseArrowProjectile();
            }
        }
    }

    IEnumerator ActivetoLong()
    {
        yield return new WaitForSeconds(0.2f);
        if (gameObject.activeInHierarchy)
        {
            CloseArrowProjectile();
        }
    }
     void CloseArrowProjectile()
     {
        if(activeToLongRoutine != null)
        {
            StopCoroutine(activeToLongRoutine);
        }
        ArrowPool.Instance.AddArrow(gameObject);
     }
}
