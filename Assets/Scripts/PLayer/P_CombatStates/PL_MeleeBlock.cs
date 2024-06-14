using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
public class PL_MeleeBlock : BaseState
{
    protected PL_CombatSm _sm;
    public PL_MeleeBlock(string name, PL_CombatSm stateMachine) : base("PL_MeleeBlock", stateMachine)
    {
        _sm = (PL_CombatSm)stateMachine;

    }

    public override void Enter()
    {
        base.Enter();
        _sm.equipSystem.ChangeHandSlot(_sm.equipSystem.RightHandSlot, _sm.inventoryRefrence.equipedMeleeWeapon);
        GameObject.Find("CombatPlayerFollowMelee").GetComponent<CinemachineVirtualCamera>().Priority = 12;
        _sm.anims.SetBool("Combat", true);
        _sm.anims.SetBool("DrawnSword", true);
        _sm.anims.SetBool("MeleeBlock", true);
        _sm.animLayerController.ChangeLayers(1, 1);
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();
      
        if (!Input.GetMouseButton(1))
        {
           
        }
    }
    public override void Exit()
    {
        base.Exit();
        _sm.equipSystem.ChangeHandSlotNone(_sm.equipSystem.RightHandSlot);
        _sm.anims.SetBool("DrawnSword", false);
        _sm.anims.SetBool("Combat", false);
        _sm.anims.SetBool("MeleeBlock", false);

       
      


    }
}
