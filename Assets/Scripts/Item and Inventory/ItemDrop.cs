using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemDrop : MonoBehaviour
{
    [SerializeField] private int possibleItemDrop;
    [SerializeField] private ItemData[] possibleDrop;
    private List<ItemData> dropList = new List<ItemData>();

    [SerializeField] private GameObject dropPrefab;


    public virtual void GenerateDrop()
    {
        for (int i = 0; i < possibleDrop.Length; i++)
        {
            if (Random.Range(0, 100) <= possibleDrop[i].dropChance)
                dropList.Add(possibleDrop[i]);
        }


        for (int i = 0; i < possibleItemDrop; i++)
        {
            if (dropList.Count <= 0) return;

            ItemData randomItem = dropList[Random.Range(0, dropList.Count - 1)];
            dropList.Remove(randomItem);
            DropItem(randomItem);
        }
    }

    protected void DropItem(ItemData _itemData)
    {
        
        //Set up độ nảy (drop item tưng lên cho đẹp sau khi giết quái xong) (random)
        Vector2 randomVelocity = new Vector2(Random.Range(-5,5), Random.Range(15,20));


        GameObject newDrop = Instantiate(dropPrefab,transform.position,Quaternion.identity);
        newDrop.GetComponent<ItemObject>().SetUpItem(_itemData, randomVelocity);
    }
}
