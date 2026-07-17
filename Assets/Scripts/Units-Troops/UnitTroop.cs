using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class UnitTroop : MonoBehaviour
{
    //class beahavior
    private IUnitBehavior behavior;
    BuffHolder buffScript;

    public float speed;
    float strenght;
    float attackPower;
    public float health;
    public float range;
    int index;
    UnitType unitType;
    public Owner ownercl;
    public LayerMask troopLayer;

    UnitTroop CurrentEnemy;
    public Transform location;
    public Transform territoryLocation;
    public bool chasingEnemy;// for attackers enemy in range
    public TroopState State = TroopState.Objective;
    public bool isAlive => gameObject.activeInHierarchy;
    public bool isTargeted = false;//use to tell not to go after someone is already going fot him
    public float releasedRadius;
    public bool hasFought = false;
    [SerializeField] float combatCooldown = 0.2f;

    [SerializeField] SpriteRenderer spriteRenderer;
    Rigidbody2D rb;
    bool isReturned = false;

    //look 
    [SerializeField] Sprite[] troopSprites;
    private Sprite unitSprite;

    public void SetUp(UnitStats stats,Transform targetLocation,int ID,Owner owner)
    {
        //get buff without changing the actuals stats of the troops
        buffScript = BuffHolder.Instance;
        isReturned = false;
        hasFought = false;
        StopAllCoroutines();

        
        unitType = stats.unitType;
        speed = stats.moveSpeed * (buffScript.moveSpeedBuff + 1);
        strenght = stats.strenght;
        attackPower = stats.attackPower * (buffScript.attackBuff + 1);
        health = stats.health * (buffScript.healthBuff + 1);
        range = stats.attackRange;
        releasedRadius = range + 0.5f;
        location = targetLocation;
        index = ID;
        ownercl = owner;

        AssignSpritesAndBeahviors();

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
        StartCoroutine(behavior.Move(this));
    }

    public void EnemyGone()
    {
        if (CurrentEnemy != null)
        {
            CurrentEnemy.isTargeted = false;
        }
        chasingEnemy = false;
        location = territoryLocation;
        CurrentEnemy = null;
    }

    public void MoveTroop()
    {
        transform.position = Vector3.MoveTowards(transform.position, location.position, speed * Time.deltaTime);
    }

    public void TakeDamage(float damage)
    {
        health -= damage;
        if( health <= 0)
        {
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
        if (CurrentEnemy) { CurrentEnemy.isTargeted = false; CurrentEnemy = null; }
        isReturned = true;
        StopAllCoroutines();
        if(ownercl == Owner.Player) { TroopConter.Instance.UnregisterTroop(true); }
        else { TroopConter.Instance.UnregisterTroop(false); }
            BulletPool.Instance.ReceiveTroop(gameObject);
    }
    IEnumerator ReadyToFight()
    {
        yield return new WaitForSeconds(combatCooldown);
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
                    TakeDamage(health);
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
                float strenghtSave = enemyTroop.attackPower;
                enemyTroop.TakeDamage(attackPower);
                TakeDamage(strenghtSave);
            }

        }
    }

    void AssignSpritesAndBeahviors()
    {
        switch (unitType)
        {
            case UnitType.Soldier:
                unitSprite = troopSprites[0];
                behavior = new SoldierBehavior();
                break;

            case UnitType.Dwarf:
                unitSprite = troopSprites[1];
                behavior = new DwarfBehavior();
                break;

            case UnitType.Assassin:
                unitSprite = troopSprites[2];
                behavior = new AssassinBehavior();
                break;

            case UnitType.Mage:
                unitSprite = troopSprites[3];
                behavior = new MageBehavior();
                break;
            case UnitType.Ranger:
                unitSprite = troopSprites[4];
                behavior = new RangerBehavior();
                break;
        }
        spriteRenderer.sprite = unitSprite;
    }

    public UnitTroop GetCurrentEnemy()
    {
        return CurrentEnemy;
    }
    public void SetCurrentEnemy(UnitTroop currentEnemy)
    {
        CurrentEnemy = currentEnemy;
    }
    public float GetSpeed()
    {
        return speed;
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, range);
    }
}
