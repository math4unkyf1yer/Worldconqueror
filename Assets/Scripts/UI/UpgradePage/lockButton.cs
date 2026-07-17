
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class lockButton : MonoBehaviour
{
    AssignLevel levelScript;

    public UnitType buttonType;
    [SerializeField] Image buttonImage;

    private void Start()
    {
        if(levelScript == null)
        {
            levelScript = AssignLevel.Instance;
        }
    }
}
