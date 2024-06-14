using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PL_EquipSystem : MonoBehaviour
{
   public PL_Inventory inventoryRefrence;

    public Transform LeftHandSlot;
    public Transform RightHandSlot;

    public Image LeftHandImageSlot;
    public Image RightHandImageSlot;


    public Sprite transparentImage;



    public void changeImageSlot(Image imageSlot,Sprite changeTexture)
    {
        imageSlot.sprite = changeTexture;
    }

    public void ChangeHandSlot(Transform slot , Item item)
    {

        if(slot.childCount > 0)
        {
            Destroy(slot.GetChild(0).gameObject);
        }
        Instantiate(item.prefab, slot);
    

    }
    public void ChangeHandSlotNone(Transform slot)
    {
        if (slot.childCount > 0)
        {
            Destroy(slot.GetChild(0).gameObject);
        }
       
    }


}
