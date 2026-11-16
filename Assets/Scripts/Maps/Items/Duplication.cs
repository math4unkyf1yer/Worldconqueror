using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Duplication : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Unit")
        {
            UnitTroop troop = collision.GetComponent<UnitTroop>();

            if (troop != null)
            {
                troop.DuplicateTroop();
            }
        }
    }
}
