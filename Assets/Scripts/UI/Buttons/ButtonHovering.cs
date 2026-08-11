using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;

public class ButtonHovering : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private GameObject image;
    [SerializeField] private TextMeshProUGUI buttonText;
    private ButtonLock scriptlock;

    public bool canHover = true;


    private void Start()
    {
        scriptlock = GetComponent<ButtonLock>();
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (canHover)
        {
            // hovering image appears
            image.SetActive(true);
            buttonText.text = "Unlock at Level "+ scriptlock.GetLevel().ToString(); 
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (canHover)
        {
            image.SetActive(false);
            // stop hovering image disappears
            Debug.Log("Hover End");
        }
    }

}
