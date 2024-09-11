using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_ItemShopSlot : MonoBehaviour, IPointerDownHandler
{
    private ItemData item;
    private Image icon;
    UI ui;

    private void Awake()
    {
        ui = GetComponentInParent<UI>();
        icon = GetComponent<Image>();
    }

    public void SetupShopSlot(ItemData _item)
    {
        item = _item;
        icon.sprite = item.icon;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        ui.shopWindow.SetupShopWindow(item);
    }
}
