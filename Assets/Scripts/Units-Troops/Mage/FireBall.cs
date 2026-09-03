using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class FireBall : Projectiles
{

    [SerializeField] GameObject outerEdge;
    [SerializeField] GameObject innerEdge;

    private AudioSource audioS;

    
    public override void ReachTarget()
    {
        base.ReachTarget();
        if (!audioS)
        {
            audioS = gameObject.GetComponent<AudioSource>();
        }
        Explotion();
    }
    void Explotion()
    {
        //unhide both
        outerEdge.SetActive(true);
        FireBallHitBox fireBallHitBox1 = outerEdge.GetComponent<FireBallHitBox>();
        innerEdge.SetActive(true);
        FireBallHitBox fireBallHitBox2 = innerEdge.GetComponent<FireBallHitBox>();
        //explotion
        FireBallSound();
        fireBallHitBox1.ExplotionHappen();
        attackOver = true;
        spriteRenderer.enabled = false;
    }
    public void CloseFireProjectile()
    {
        spriteRenderer.enabled = true;
        outerEdge.SetActive(false);
        innerEdge.SetActive(false);
        FireBallPool.Instance.AddFireBall(gameObject);
    }

    void FireBallSound()
    {
        if(audioM != null)
        {
            if (audioM.GetSfx())
            {
                audioS.PlayOneShot(audioM.explosion);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        UnitTroop troopCl = collision.GetComponent<UnitTroop>();

        if (troopCl != null)
        {
            if (troopCl.ownercl != owner)
            {
                if (!audioS)
                {
                    audioS = gameObject.GetComponent<AudioSource>();
                }
                Explotion();
            }
        }
    }
    
}
