using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.FilePathAttribute;

public class FireBall : MonoBehaviour
{

    [SerializeField] Transform target;
    private float speed = 1.5f;
    public float damage = 0.5f;
    Owner owner;
    [SerializeField] GameObject outerEdge;
    [SerializeField] GameObject innerEdge;
    bool attackOver;
    private SpriteRenderer spriteRenderer;
    private Vector3 lastKnownTargetPos;

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void SetUp(Owner ownerref, Transform targetCl)
    {
        if(targetCl != null)
        {
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

            // If reached last known position, explode
            if (Vector3.Distance(transform.position, lastKnownTargetPos) <= 0.15f)
            {
                Explotion();
                yield break;
            }

            yield return null;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Unit" && attackOver == false)
        {
            UnitTroop enemyTroop = collision.gameObject.GetComponent<UnitTroop>();

            if (enemyTroop.ownercl != owner)
            {
                Explotion();
            }
        }
    }
    void Explotion()
    {
        StopCoroutine(MoveToTarget());

        //unhide both
        outerEdge.SetActive(true);
        FireBallHitBox fireBallHitBox1 = outerEdge.GetComponent<FireBallHitBox>();
        innerEdge.SetActive(true);
        FireBallHitBox fireBallHitBox2 = innerEdge.GetComponent<FireBallHitBox>();
        fireBallHitBox1.ExplotionHappen();
        attackOver = true;
        spriteRenderer.enabled = false;
    }
    public void CloseProjectile()
    {
        outerEdge.SetActive(false);
        innerEdge.SetActive(false);
        FireBallPool.Instance.AddFireBall(gameObject);
    }
}
