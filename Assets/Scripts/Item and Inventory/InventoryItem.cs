using System;
using UnityEngine;

[Serializable]
public class InventoryItem
{

    public ItemData data;
    public int stackSize;       //Unity mac dinh la 0

    public InventoryItem(ItemData _newItemData)
    {
        data = _newItemData;
        AddStack(); //Khoi tao lan dau -> so luong = 1
    }

    public void AddStack() => stackSize++;
    public void RemoveStack() => stackSize--;

}
