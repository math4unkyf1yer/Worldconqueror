using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireStorm : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Unit")
        {
            UnitBuffs troopBuff = collision.gameObject.GetComponent<UnitBuffs>();

            troopBuff.AddDamage(2);
        }
    }

}
