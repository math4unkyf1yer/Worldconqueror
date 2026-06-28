using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HazardController : MonoBehaviour
{
    private HazardZone zone;

    //images 
    [SerializeField] SpriteRenderer hazardImage;
    //colors assign to show the UI which type of hazard it his 
    [SerializeField] Color slowColor;
    [SerializeField] Color speedColor;
    [SerializeField] Color damageColor;
    [SerializeField] Color FogColor;

    [SerializeField] SpriteRenderer imageSprite;
    [SerializeField] Sprite[] hazardSprites;

    private List<UnitTroop> troopsInDamage = new List<UnitTroop>();
    bool isrepeating;

    float oldSpeed;
    public void SetUp(HazardZone hazardZone)
    {
        zone = hazardZone;


        HazardType type = zone.Type;
        switch (type)
        {
            case HazardType.Slow:
                hazardImage.color = slowColor;
                imageSprite.sprite = hazardSprites[0];
                break;
            case HazardType.Speed:
                hazardImage.color = speedColor;
                imageSprite.sprite = hazardSprites[1];
                break;
            case HazardType.Damage:
                hazardImage.color = damageColor;
                imageSprite.sprite = hazardSprites[2];
                break;

        }
        
        // Optionally scale the visual size based on intensity
        transform.localScale = Vector3.one * Mathf.Lerp(0.3f, 2f, zone.intensity);
        imageSprite.size = new Vector2(1, 1);
    }

    public void DamageHazardActivated()
    {
        if(troopsInDamage.Count > 0)
        {
            int index = Random.Range(0, troopsInDamage.Count);
            zone.Damage(troopsInDamage[index]);
        }
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Unit")
        {
            UnitTroop troop = collision.gameObject.GetComponent<UnitTroop>();

            switch (zone.Type)
            {
                case HazardType.Slow:
                    oldSpeed = troop.speed;
                    float slowdown = troop.speed * zone.speedChange();
                    troop.speed -= slowdown;
                    break;
                case HazardType.Speed:
                    oldSpeed = troop.speed;
                    float speed = zone.speedChange();
                    troop.speed *= speed;
                    break;
                case HazardType.Damage:
                    //chance to damage
                    troopsInDamage.Add(troop);
                    if (!isrepeating) { InvokeRepeating("DamageHazardActivated", 0.5f, 1); isrepeating = true; }
                    break;

            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Unit")
        {
            UnitTroop troop = collision.gameObject.GetComponent<UnitTroop>();

            switch (zone.Type)
            {
                case HazardType.Slow:                
                    troop.speed = oldSpeed;
                    break;
                case HazardType.Speed:
                    troop.speed = oldSpeed;

                    break;
                case HazardType.Damage:
                    troopsInDamage.Remove(troop);
                    if(troopsInDamage.Count == 0) { CancelInvoke("DamageHazardActivated"); isrepeating = false; }
                    break;

            }
        }
    }
}
