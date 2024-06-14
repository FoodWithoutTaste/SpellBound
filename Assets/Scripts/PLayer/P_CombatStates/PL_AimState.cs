using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
public class PL_AimState : BaseState
{
    protected PL_CombatSm _sm;
    public PL_AimState(string name ,PL_CombatSm stateMachine) : base("PL_AimState", stateMachine) {
        _sm = (PL_CombatSm)stateMachine;

    }

    public override void Enter()
    {
        base.Enter();
        _sm.equipSystem.ChangeHandSlot(_sm.equipSystem.LeftHandSlot, _sm.inventoryRefrence.equipedGrimoire);
        GameObject.Find("CombatPlayerFollowMagic").GetComponent<CinemachineVirtualCamera>().Priority = 12;
        _sm.anims.SetBool("Combat", true);
        _sm.animLayerController.ChangeLayers(1, 1);
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();
       
        if (Input.GetMouseButton(0) && GameManager.instance.gameState == GameManager.GameState.Playing)
        {
            if(_sm.inventoryRefrence.equipedGrimoire.spell is BeamSpell)
            {

                stateMachine.ChangeStates(_sm.pl_spellAttackState1);
            }
            else
            {
                if(_sm.stats.mana >= _sm.inventoryRefrence.equipedGrimoire.spell.manaCost)
                stateMachine.ChangeStates(_sm.pl_spellAttackState);
            }





        }




        if (!Input.GetMouseButton(0) && !Input.GetMouseButton(1))
        {
            stateMachine.ChangeStates(((PL_CombatSm)stateMachine).pl_defaultState);
        }
    }
    public override void Exit()
    {
        base.Exit();
        _sm.equipSystem.ChangeHandSlotNone(_sm.equipSystem.LeftHandSlot);
        _sm.anims.SetBool("Combat", false);
        _sm.animLayerController.ChangeLayers(1, 0);
        GameObject.Find("CombatPlayerFollowMagic").GetComponent<CinemachineVirtualCamera>().Priority = 9;
    }
}
