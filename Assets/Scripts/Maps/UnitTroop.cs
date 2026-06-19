using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitTroop : MonoBehaviour
{
    public float speed;
    float strenght;
    int index;
    Owner ownercl;
    Transform location;
    public bool hasFought = false;
    [SerializeField] float cobatCooldown = 0.2f;

    [SerializeField] SpriteRenderer spriteRenderer;
    Rigidbody2D rb;
    bool isReturned = false;


    public void SetUp(UnitStats stats,Transform targetLocation,int ID,Owner owner,Sprite unitSprite)
    {
        isReturned = false;
        hasFought = false;
        StopAllCoroutines();


        speed = stats.moveSpeed;
        strenght = stats.strenght;
        location = targetLocation;
        index = ID;
        ownercl = owner;
        spriteRenderer.sprite = unitSprite;

        if (ownercl == Owner.Player)
        {
            spriteRenderer.color = Color.blue;
        }
        else if(ownercl == Owner.AI1) 
        {
            spriteRenderer.color = Color.red;
        }else if (ownercl == Owner.AI2)
        {
            spriteRenderer.color = Color.yellow;
        }
        else
        {
            spriteRenderer.color = Color.green;
        }

            rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.velocity = Vector2.zero;
            rb.angularVelocity = 0f;
        }
        StartCoroutine(MoveToTarget());
    }

    //move to position 
    IEnumerator MoveToTarget()
    {
        while (Vector3.Distance(transform.position, location.position) > 0.05f)
        {
            transform.position = Vector3.MoveTowards( transform.position, location.position, speed * Time.deltaTime);
           // rb.MovePosition(Vector2.MoveTowards(rb.position,    location.position,   speed * Time.deltaTime));   //need to change the speed for the units
            yield return null;
        }

        transform.position = location.position;
    }

    public void TakeDamage(float damage)
    {
        strenght -= damage;
        if( strenght <= 0)
        {
            StopAllCoroutines();
            gameObject.transform.position = new Vector3(0, 0, 0);
            ReturnToPool();
            return;
        }

        if (gameObject.activeInHierarchy)
        {
            StartCoroutine(ReadyToFight());
        }

    }
    public void ReturnToPool()
    {
        if (isReturned) return;
        isReturned = true;
        StopAllCoroutines();
        BulletPool.Instance.ReceiveTroop(gameObject);
    }
    IEnumerator ReadyToFight()
    {
        yield return new WaitForSeconds(cobatCooldown);
        hasFought = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {

        if(collision.gameObject.tag == "Terretory")
        {
            TerretoryController terretory = collision.gameObject.GetComponentInParent<TerretoryController>();
            if (terretory != null)
            {
                if (terretory.terretoryIndex == index)
                {

                    if (terretory.owner == ownercl)
                    {
                        //get more troops
                        terretory.ReceiveTroops(strenght);
                    }
                    else
                    {
                        //lose troops
                        terretory.TakeDamage(strenght, ownercl);
                    }
                    TakeDamage(strenght);

                }
            }
        }else if(collision.gameObject.tag == "Unit")
        {
            if (hasFought) return;
            UnitTroop enemyTroop = collision.gameObject.GetComponent<UnitTroop>();

            if (enemyTroop.ownercl != ownercl)
            {
                hasFought = true;
                enemyTroop.hasFought = true;
                float strenghtSave = enemyTroop.strenght;
                enemyTroop.TakeDamage(strenght);
                TakeDamage(strenghtSave);
            }

        }
    }


}
