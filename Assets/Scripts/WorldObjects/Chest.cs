using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chest : InteractiableObject
{
    [SerializeField] Item item;
    [SerializeField] GameObject PhysicalItem;
    [SerializeField]  Animator anims;
    
    public override void Interact()
    {
        base.Interact();
        PhysicalItem.GetComponent<ItemInGame>().item = item;
        PhysicalItem.SetActive(true);
        anims.SetTrigger("chestOpen");
        this.transform.tag = "Untagged";

    }
}
