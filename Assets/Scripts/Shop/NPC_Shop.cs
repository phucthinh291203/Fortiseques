using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class NPC_Shop : MonoBehaviour
{
    [SerializeField] UI_InGame inGameUI;
    [SerializeField] public List<ItemData> itemListSell;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponent<Player>() != null)
        {
            inGameUI.interactButton.gameObject.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.GetComponent<Player>() != null)
            if(inGameUI != null)
            inGameUI.interactButton.gameObject.SetActive(false);
    }

}
