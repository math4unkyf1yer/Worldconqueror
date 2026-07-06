using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IUnitBehavior
{
    IEnumerator Move(UnitTroop troop);
    void Attack(UnitTroop troop);
}
