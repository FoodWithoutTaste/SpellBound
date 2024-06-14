using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PL_Inventory : Inventory
{
    public PL_EquipSystem equipSystem;
    public MeleeWeaponItem _equipedMeleeWeapon;
    public GrimoireItem _equipedGrimoire;
    [SerializeField] DealDamage playerMelee;
    public MeleeWeaponItem equipedMeleeWeapon
    {
        get
        {
            return _equipedMeleeWeapon;
        }

        set
        {
            //#3
            _equipedMeleeWeapon = value;
         
            if(_equipedMeleeWeapon != null)
            {
                playerMelee.Damage = _equipedMeleeWeapon.dmg;
                equipSystem.changeImageSlot(equipSystem.RightHandImageSlot, _equipedMeleeWeapon.itemTexture);
            }
            else
            {
                equipSystem.changeImageSlot(equipSystem.RightHandImageSlot, equipSystem.transparentImage);
            }
           
        }
    }
    public GrimoireItem equipedGrimoire
    {
        get
        {
            return _equipedGrimoire;
        }

        set
        {
            //#3
            _equipedGrimoire = value;
            if (_equipedGrimoire != null)
            {
                equipSystem.changeImageSlot(equipSystem.LeftHandImageSlot, _equipedGrimoire.itemTexture);
            }
            else
            {
                equipSystem.changeImageSlot(equipSystem.LeftHandImageSlot, equipSystem.transparentImage);
            }

        }
    }


}
