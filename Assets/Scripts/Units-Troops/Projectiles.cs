using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectiles : MonoBehaviour
{
    [SerializeField] protected Transform target;
    protected float speed;
    public float damage;
    protected Owner owner;
    protected bool attackOver;
    protected SpriteRenderer spriteRenderer;
    protected Vector3 lastKnownTargetPos;

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void SetUp(Owner ownerref, Transform targetCl , float damageG , float speedG)
    {
        if (targetCl != null)
        {
            damage = damageG;
            speed = speedG;
            attackOver = false;
            owner = ownerref;
            target = targetCl;
            StartCoroutine(MoveToTarget());
        }
    }

    IEnumerator MoveToTarget()
    {
        while (attackOver == false)
        {
            // If target still exists and is active, update last known position
            if (target != null && target.gameObject.activeInHierarchy)
            {
                lastKnownTargetPos = target.position;
            }

            // Move toward last known position
            Vector2 dir = (lastKnownTargetPos - transform.position).normalized;
            float angle = Mathf.Atan2(dir.x, dir.y) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
            transform.position = Vector3.MoveTowards(transform.position, lastKnownTargetPos, speed * Time.deltaTime);

            float dist = Vector3.Distance(transform.position, lastKnownTargetPos);
            // If reached last known position, explode
            if (dist <= 0.05f)
            {
                ReachTarget();
                yield break;
            }

            yield return null;
        }
    }
    public virtual void ReachTarget()
    {
        //others will override
    }

}
