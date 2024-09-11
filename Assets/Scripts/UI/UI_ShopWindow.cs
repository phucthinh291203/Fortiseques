using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class UI_ShopWindow : MonoBehaviour
{
    [SerializeField] public TextMeshProUGUI itemName;
    [SerializeField] public TextMeshProUGUI itemDescription;
    [SerializeField] public TextMeshProUGUI itemPrice;

    [SerializeField] Button buyButton;

    public event Action<bool, string> OnBuyActionCompleted;
    public void SetupShopWindow(ItemData _item)
    {
        buyButton.onClick.RemoveAllListeners();
        itemName.text = _item.itemName;
        itemDescription.text = _item.GetDescription();
        itemPrice.text = "Price: " + _item.price.ToString() + " souls ";

        buyButton.onClick.AddListener(() => {
            bool result = Inventory.instance.CanBuy(_item);
            OnBuyActionCompleted?.Invoke(result, result ? "Purchase successfully: " + _item.name : "Not enough currency or inventory full.");
        });
    }
}
