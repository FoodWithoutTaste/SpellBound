using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class InventoryMenuUI : MonoBehaviour
{
    public PL_Inventory inventoryRefrence;
    public GameObject inventorySlot;
    public Transform content;

    public void RefreshMenu()
    {
        if(content.childCount > 0)
        {
            for (int i = 0; i < content.childCount; i++)
            {
                Destroy(content.GetChild(i).gameObject);
            }
        }
        for (int i = 0; i < inventoryRefrence.inventoryItems.Count; i++)
        {
            GameObject a = Instantiate(inventorySlot, content);
            a.transform.GetChild(0).GetChild(0).GetComponent<Image>().sprite = inventoryRefrence.inventoryItems[i].item.itemTexture;
           
            a.GetComponent<InventoryUiSlot>().item = inventoryRefrence.inventoryItems[i].item;
            a.GetComponent<InventoryUiSlot>().inventoryReference = inventoryRefrence;
        }
    }
}
