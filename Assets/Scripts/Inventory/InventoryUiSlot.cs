using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryUiSlot : MonoBehaviour
{
    public Item item;
    public Inventory inventoryReference;
    public void EquipTheItem()
    {
        if (item is MeleeWeaponItem && inventoryReference is PL_Inventory)
        {
          
            PL_Inventory playerInventory = (PL_Inventory)inventoryReference;

           
            playerInventory.equipedMeleeWeapon = (MeleeWeaponItem)item;
            GameManager.instance.ResumeGame();
        }
        if (item is GrimoireItem && inventoryReference is PL_Inventory)
        {
            
            PL_Inventory playerInventory = (PL_Inventory)inventoryReference;

         
            playerInventory.equipedGrimoire = (GrimoireItem)item;
            GameManager.instance.ResumeGame();
        }
        if (item is PotionItem && inventoryReference is PL_Inventory)
        {
           
            PL_Inventory playerInventory = (PL_Inventory)inventoryReference;
            PotionItem ITEM = (PotionItem)item;
            
            playerInventory.gameObject.GetComponent<Stats>().hp += ITEM.hpToAdd;
            playerInventory.RemoveItem(ITEM);
            GameManager.instance.ResumeGame();
            Destroy(this.gameObject);
        }
    }
    public void inspect()
    {
        FindObjectOfType<InstantiateAndCenter>().InstantiateInspectItem(item,this);
    }
}
