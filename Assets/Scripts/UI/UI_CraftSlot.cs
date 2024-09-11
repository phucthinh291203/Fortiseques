using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UI_CraftSlot : UI_ItemSlot
{

    protected override void Start()
    {
        base.Start();
    }


    public void SetUpCraftSlot(ItemData_Equipment _data)
    {
        if (_data == null) return;
        item.data = _data;
        itemImage.sprite = _data.icon;
        itemText.text = _data.name;

        if (itemText.text.Length > 12)
            itemText.fontSize = itemText.fontSize * 0.7f;
        else
            itemText.fontSize = 24;
    }

    
    public override void OnPointerDown(PointerEventData eventData) //Bam vao craft vu khi
    {
        ui.craftWindow.SetUpCraftWindow(item.data as ItemData_Equipment);
    }
}
