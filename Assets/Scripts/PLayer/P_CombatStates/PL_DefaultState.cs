using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PL_DefaultState : BaseState
{
    protected PL_CombatSm _sm;
    public float AttackCooldown;
    public PL_DefaultState(PL_CombatSm stateMachine) : base("PL_DefaultState", stateMachine) {
        _sm = (PL_CombatSm)stateMachine;
    }

    public override void Enter()
    {
       base.Enter();
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();
        if(AttackCooldown >= 0)
        {
            AttackCooldown -= Time.deltaTime;
        }

        if (Input.GetMouseButton(0))
        {
            //if (_sm.inventoryRefrence.equipedMeleeWeapon != null && AttackCooldown < 0 && _sm.movementSm.currentState is PL_GroundedState) 
            // {
            //    stateMachine.ChangeStates(((PL_CombatSm)stateMachine).pl_MeleeAttackState);
            // }

            if (_sm.inventoryRefrence.equipedGrimoire != null && GameManager.instance.gameState == GameManager.GameState.Playing)
            {
                if (_sm.inventoryRefrence.equipedGrimoire.spell is BeamSpell)
                {
                    stateMachine.ChangeStates(((PL_CombatSm)stateMachine).pl_spellAttackState1);
                }
                else
                {
                    if (_sm.stats.mana >= _sm.inventoryRefrence.equipedGrimoire.spell.manaCost)
                        stateMachine.ChangeStates(((PL_CombatSm)stateMachine).pl_spellAttackState);
                }
                   
            }
              



        }
         if (Input.GetMouseButton(1))
         {
            if (_sm.inventoryRefrence.equipedGrimoire != null)
                stateMachine.ChangeStates(((PL_CombatSm)stateMachine).pl_aimState);
         }
        
    }
    public override void Exit()
    {
        base.Exit();
    }

    
}
