using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PL_IdleState : PL_GroundedState
{
    public PL_IdleState(PL_MovementSm stateMachine) : base("PL_IdleState", stateMachine) { }

    public override void Enter()
    {
        base.Enter();
        
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate(); 

        if(Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.S) && ((PL_MovementSm)stateMachine).combatSm.currentState != null)
        {
           if(((PL_MovementSm)stateMachine).combatSm.currentState.GetType() == typeof(PL_MeleeAttackState))
            {
               
            }
            else
            {
                stateMachine.ChangeStates(((PL_MovementSm)stateMachine).pl_MoveState);
            }
          

        }
    }
}
