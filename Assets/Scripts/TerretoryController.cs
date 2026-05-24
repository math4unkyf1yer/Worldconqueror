using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerretoryController : MonoBehaviour
{
    private TerretoryData TerretoryData;
    

    public void GetData(TerretoryData data)
    {
        TerretoryData = data;

        Debug.Log(TerretoryData.Owner);
    }

    //on capture
    //OnUnitCountUp--- will need the units and how strong 

}
