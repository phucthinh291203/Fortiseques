using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
///  Script nay chiu trach nhiem cho viec chuyen du lieu cua 1 item vao object
/// </summary>
public class ItemObject : MonoBehaviour
{
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private Vector2 velocity;
    [SerializeField] private ItemData itemData;

    

    private void SetUpVisual()
    {
        if (itemData == null)
            return;

        GetComponent<SpriteRenderer>().sprite = itemData.icon;
        gameObject.name = "item object - " + itemData.name;
    }

    public void SetUpItem(ItemData _itemData, Vector2 _velocity)
    {
        itemData = _itemData;
        rb.velocity = _velocity;

        SetUpVisual();
    }

    //Add thêm điều kiện này nếu vẫn muốn cộng dồn vào túi khi nhặt ( full slot)
    //inventoryDictionary.TryGetValue(_itemToCraft,out InventoryItem value) == false
    public void PickUpItem()
    {
        if (!Inventory.instance.CanAddItem() && itemData.itemType == ItemType.Equipment)
        {
            rb.velocity = new Vector2(0, 7);
            PlayerManager.instance.player.fx.CreatePopUpText("Inventory is full");
            return;
        }

        AudioManager.instance.PlaySFX(16, transform);
        Inventory.instance.AddItem(itemData);
        Destroy(gameObject);
    }
}
