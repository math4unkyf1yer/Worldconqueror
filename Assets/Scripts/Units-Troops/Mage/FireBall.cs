using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.FilePathAttribute;

public class FireBall : Projectiles
{

    [SerializeField] GameObject outerEdge;
    [SerializeField] GameObject innerEdge;


    public override void ReachTarget()
    {
        base.ReachTarget();
        Explotion();
    }
    void Explotion()
    {
        //unhide both
        outerEdge.SetActive(true);
        FireBallHitBox fireBallHitBox1 = outerEdge.GetComponent<FireBallHitBox>();
        innerEdge.SetActive(true);
        FireBallHitBox fireBallHitBox2 = innerEdge.GetComponent<FireBallHitBox>();
        fireBallHitBox1.ExplotionHappen();
        attackOver = true;
        spriteRenderer.enabled = false;
    }
    public void CloseFireProjectile()
    {
        outerEdge.SetActive(false);
        innerEdge.SetActive(false);
        FireBallPool.Instance.AddFireBall(gameObject);
    }
}
