using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class ItemDrag : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    public ItemData data;
    bool isDragging;

    private Vector3 placementPosition;

    [SerializeField] private Camera worldCamera;
    [SerializeField] private RectTransform parchment;
    [SerializeField] private GameObject itemPreview;

    [SerializeField] private TMP_Text amountText;

    private ItemManager itemManager;

    private void Start()
    {
        itemManager = AssignLevel.Instance.GetComponent<ItemManager>();

        UpdateAmountText();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (data == null || itemManager.GetAmount(data.itemType) <= 0)
            return;

        GameObject hit = eventData.pointerCurrentRaycast.gameObject;

        if (hit.tag == "Item")
        {
            isDragging = true;
            Debug.Log("Drag item");
        }
    }

    //dragging the mouse 
    public void OnDrag(PointerEventData eventData)
    {
        if (!isDragging)
            return;
        placementPosition = worldCamera.ScreenToWorldPoint(
            new Vector3(
                eventData.position.x,
                eventData.position.y,
                Mathf.Abs(worldCamera.transform.position.z)
            )
        );

        placementPosition.z = 0;
        itemPreview.transform.position = placementPosition;
        itemPreview.SetActive( true);
    }
    //on mouse release
    public void OnPointerUp(PointerEventData eventData)
    {
        if (!isDragging)
            return;

        GameObject hit = eventData.pointerCurrentRaycast.gameObject;

        if (hit.tag == "Map" || hit.tag == "Terretory")
        {
            PlaceItem();
        }

        isDragging = false;
        itemPreview.SetActive(false);
    }

    void PlaceItem()
    {
        itemManager.PlaceItem(data.itemType, data.radius, placementPosition);
        UpdateAmountText();
    }

    private void UpdateAmountText()
    {
        int amount = itemManager.GetAmount(data.itemType);

        amountText.text = "x" + amount;
    }
}
