using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerItemDrop : ItemDrop
{
    [Header("Player's drop ")]
    //[SerializeField] private float chanceToLoseItem;

    [SerializeField] private float chanceToLoseMaterial;
    public override void GenerateDrop()
    {


        Inventory inventory = Inventory.instance;
        // list of equipment

        List<InventoryItem> materialToLoose = new List<InventoryItem>();


        //List<InventoryItem> itemToUnequip = new List<InventoryItem>();
        ////foreach item we gonna check if should loose item
        //if (chanceToLoseItem > 0)
        //{

        //    foreach (InventoryItem item in inventory.GetEquipmentList())
        //    {
        //        if (Random.Range(0, 100) <= chanceToLoseItem)
        //        {
        //            DropItem(item.data);
        //            itemToUnequip.Add(item);
        //        }
        //    }

        //    for (int i = 0; i < itemToUnequip.Count; i++)
        //    {
        //        inventory.UnequipItem(itemToUnequip[i].data as ItemData_Equipment);

        //    }
        //}

        if (chanceToLoseMaterial > 0)
        {
            foreach (InventoryItem item in inventory.GetStashList())
            {
                if (Random.Range(0, 100) <= chanceToLoseMaterial)
                {
                    DropItem(item.data);
                    materialToLoose.Add(item);
                }
            }

            for (int i = 0; i < materialToLoose.Count; i++)
            {
                inventory.RemoveItem(materialToLoose[i].data);
            }
        }
    }
}
