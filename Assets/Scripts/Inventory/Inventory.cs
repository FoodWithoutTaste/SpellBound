using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class Inventory : MonoBehaviour
{
    public List<ItemHolder> inventoryItems;

    public void AddItem(Item item,int quantity)
    {
        if (InventoryHasItem(item))
        {
            inventoryItems[returnItemId(item)].quantity += quantity;
        }
        else
        {
            inventoryItems.Add(new ItemHolder(item, quantity));
        }
      
    }
    public void RemoveItemByQantity(Item item, int quantity)
    {
        if (InventoryHasItem(item))
        {
           
            if(inventoryItems[returnItemId(item)].quantity > quantity)
            {
                inventoryItems[returnItemId(item)].quantity -= quantity;
            }
            else
            {
                RemoveItem(item);
            }
        }
    }
    public void RemoveItem(Item item)
    {
        inventoryItems.Remove(ReturnFirstItemHolder(item));
    }
    public bool InventoryHasItem(Item item)
    {
        bool x = false;
        for (int i = 0; i < inventoryItems.Count; i++)
        {
            if(inventoryItems[i].item == item)
            {
                x = true;
            }
        }
        return x;
    }
    public ItemHolder ReturnFirstItemHolder(Item item)
    {
        for (int i = 0; i < inventoryItems.Count; i++)
        {
            if (inventoryItems[i].item == item)
            {
                return inventoryItems[i];
            }
           
        }
        return null;
    }
    public int returnItemId(Item item)
    {
        int x = -1;
        if (InventoryHasItem(item))
        {
           
            for (int i = 0; i < inventoryItems.Count; i++)
            {
                if (inventoryItems[i].item == item)
                {
                    x = i;
                }
            }
           
        }
        return x;

    }


}
