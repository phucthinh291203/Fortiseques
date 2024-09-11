using TMPro;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// Script nay chiu trach nhiem cho viec chuyen du lieu cua 1 item thanh UI len inventory
/// </summary>
public class UI_ItemSlot : MonoBehaviour, IPointerDownHandler, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] protected Image itemImage;
    [SerializeField] protected TextMeshProUGUI itemText;

    public InventoryItem item;
    protected UI ui;

    protected virtual void Start()
    {
        ui = GetComponentInParent<UI>();
    }

    public void UpdateSlot(InventoryItem _newItem)
    {
        item = _newItem;
        itemImage.color = Color.white;  // chinh color cho UI de nhin
        if (item != null)
        {
            itemImage.sprite = item.data.icon;

            if (item.stackSize > 0)
            {
                itemText.text = item.stackSize.ToString();
            }
            else
                itemText.text = "";
        }
    }

    public void CleanUpSlot()
    {
        item = null;
        itemImage.sprite = null;
        itemImage.color = Color.clear;
        itemText.text = "";
    }

    public virtual void OnPointerDown(PointerEventData eventData)
    {
        if (item == null) return;

        if(Input.GetKey(KeyCode.LeftControl) || PlayerInputManager.instance.isPressDrop)
        {
            Inventory.instance.RemoveItem(item.data);
            return;
        }

        if (item.data.itemType == ItemType.Equipment)
        {
            Inventory.instance.EquipItem(item.data);
            return;
        }
        
        ui.itemToolTip.HideToolTip();
    }

    

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (item == null) return;

        ui.itemToolTip.ShowToolTip(item.data as ItemData_Equipment);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (item == null) return;
        ui.itemToolTip.HideToolTip();
    }
}
