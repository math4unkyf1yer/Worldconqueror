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

    private List<UnitBuffs> troopsInDamage = new List<UnitBuffs>();
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
        troopsInDamage.RemoveAll(t => t == null || t.troop.health <= 0);

        if (troopsInDamage.Count == 0)
        {
            CancelInvoke(nameof(DamageHazardActivated));
            isrepeating = false;
            return;
        }

        int index = Random.Range(0, troopsInDamage.Count);
        troopsInDamage[index].AddDamage(1);
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Unit")
        {
            UnitBuffs troopBuff = collision.gameObject.GetComponent<UnitBuffs>();

            switch (zone.Type)
            {
                case HazardType.Slow:
                    troopBuff.AddGlobalSpeed(0.65f);
                    break;
                case HazardType.Speed:
                    troopBuff.AddGlobalSpeed(1.35f);
                    break;
                case HazardType.Damage:
                    //chance to damage
                    troopsInDamage.Add(troopBuff);
                    if (!isrepeating) { InvokeRepeating("DamageHazardActivated", 0.8f, 1); isrepeating = true; }
                    break;

            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Unit")
        {
            UnitBuffs troopBuff = collision.gameObject.GetComponent<UnitBuffs>();

            switch (zone.Type)
            {
                case HazardType.Slow:
                    troopBuff.RemoveGlobalSpeed(0.65f);
                    break;
                case HazardType.Speed:
                    troopBuff.RemoveGlobalSpeed(1.35f);

                    break;
                case HazardType.Damage:
                    troopsInDamage.Remove(troopBuff);
                    if(troopsInDamage.Count == 0) { CancelInvoke("DamageHazardActivated"); isrepeating = false; }
                    break;

            }
        }
    }
}
