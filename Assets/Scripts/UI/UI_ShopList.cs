using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;


public class UI_ShopList : MonoBehaviour
{
    [SerializeField] private Transform itemShopParent;
    [SerializeField] private GameObject itemShopPrefab;
    public TextMeshProUGUI currentSouls;
    public TextMeshProUGUI buyActionText;

    UI ui;
    UI_ShopWindow shopWindow;

    [SerializeField] NPC_Shop npc;
    List<ItemData> itemList;
    private void Awake()
    {
        itemList = npc.GetComponent<NPC_Shop>().itemListSell;
        ui = GetComponentInParent<UI>();
        shopWindow = FindObjectOfType<UI_ShopWindow>();
        ui.ResetText += OpenShopUI;
        shopWindow.OnBuyActionCompleted += ShowActionText;
    }
    void Start()
    {
        SetUpShop();
        SetUpDefaultShopWindow();
    }


    private void Update()
    {
       currentSouls.text = "Your current souls: " + PlayerManager.instance.GetCurrentCurrency().ToString();
    }

    private void SetUpShop()
    {

        for(int i=0;i< itemShopParent.childCount;i++)
        {
            Destroy(itemShopParent.GetChild(i).gameObject);
        }

        for(int i = 0;i < itemList.Count;i++)
        {
            GameObject newShopItem = Instantiate(itemShopPrefab, itemShopParent);
            newShopItem.GetComponent<UI_ItemShopSlot>().SetupShopSlot(itemList[i]);
        }

        buyActionText.text = "";
    }


    private void SetUpDefaultShopWindow()
    {
        ui.shopWindow.SetupShopWindow(itemList[0]);
    }

    public void ShowActionText(bool _action,string buyAction)
    {   
        buyActionText.color = _action ? Color.green : Color.red;

        buyActionText.text = buyAction.ToString();
    }
    

    public void OpenShopUI()
    {
        buyActionText.text = "";
        shopWindow.itemName.text = "";
        shopWindow.itemDescription.text = "";
        shopWindow.itemPrice.text = "";
    }
}
