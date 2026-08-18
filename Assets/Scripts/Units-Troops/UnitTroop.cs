using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class UnitTroop : MonoBehaviour
{
    //class beahavior
    private IUnitBehavior behavior;

    public float speed;
    public float strenght;
    public float vigor;
    public float range;
    public float fireRate;
    public float critRate;
    public float noDeathChange;
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
    [SerializeField] float combatCooldown = 0.1f;

    [SerializeField] SpriteRenderer spriteRenderer;
    Rigidbody2D rb;
    bool isReturned = false;

    //look 
    [SerializeField] Sprite[] troopSprites;
    private Sprite unitSprite;

    public void SetUp(UnitStats stats,Transform targetLocation,int ID,Owner owner)
    {
        //get buff without changing the actuals stats of the troops
        isReturned = false;
        hasFought = false;
        StopAllCoroutines();
        
        unitType = stats.unitType;
        speed = stats.moveSpeed;
        strenght = stats.strenght;
        vigor = stats.vigor;
        range = stats.attackRange;
        fireRate = stats.fireRate;
        noDeathChange = stats.noDeathChances;
        critRate = stats.critChances;
        releasedRadius = range + 0.5f;
        location = targetLocation;
        territoryLocation = location;
        index = ID;
        ownercl = owner;

        UnitBuffs troopBuff = GetComponent<UnitBuffs>();
        troopBuff.SetUp();
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

    public void TakeDamage(float damage,bool lastStandActivate)
    {
        bool lastStand = false;
        if(noDeathChange > 0 && lastStandActivate)
        {
            float roll = Random.value;//0-1

            if (roll <= noDeathChange)
            {
                Debug.Log("live again");
                lastStand = true;
            }
        }

        vigor -= damage;
        if(vigor <= 0)
        {
            if (!lastStand) // not soldier or did not it the percentage
            {
                gameObject.transform.position = new Vector3(0, 0, 0);
                ReturnToPool();
                return;
            }
            else
            {
                vigor = 0.1f;
            }
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
        if (isAlive)
        {
            hasFought = false;
        }
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
                    TakeDamage(vigor,false);
                }
            }
        }
    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.tag != "Unit") return;

        UnitTroop enemyTroop = collision.gameObject.GetComponent<UnitTroop>();
        if (enemyTroop == null) return;

        if (enemyTroop.ownercl == ownercl) return;

        // Prevent double combat
        if (hasFought || enemyTroop.hasFought) return;

        hasFought = true;
        enemyTroop.hasFought = true;

        // -------------------------
        // YOUR DAMAGE TO ENEMY
        // -------------------------
        float damageToEnemy = vigor;

        if (critRate > 0f && Random.value <= critRate)
        {
            damageToEnemy = vigor * 2f; // YOUR CRIT
        }


        // ENEMY DAMAGE TO YOU
        // -------------------------
        float damageToSelf = enemyTroop.vigor;

        if (enemyTroop.critRate > 0f && Random.value <= enemyTroop.critRate)
        {
            damageToSelf = enemyTroop.vigor * 2f; // ENEMY CRIT
        }

        enemyTroop.TakeDamage(damageToEnemy, true);
        TakeDamage(damageToSelf, true);
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
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, range);
    }

}
