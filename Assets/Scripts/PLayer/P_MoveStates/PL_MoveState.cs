using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PL_MoveState : PL_GroundedState
{
  
    public PL_MoveState(PL_MovementSm stateMachine) : base("PL_MoveState", stateMachine) {}

    public override void Enter()
    {
        base.Enter();
        _sm.anims.SetBool("Walk", true);
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();
        if (!Input.GetKey(KeyCode.A) && !Input.GetKey(KeyCode.D) && !Input.GetKey(KeyCode.W) && !Input.GetKey(KeyCode.S))
        {
            stateMachine.ChangeStates(_sm.pl_IdleState);
        }
        if (Input.GetKey(KeyCode.LeftShift))
        {
            stateMachine.ChangeStates(_sm.pl_runState);
        }
     
    }
    public void MovePlayer()
    {
       
            _sm.rb.AddForce(_sm.movementInput.normalized * _sm.MoveSpeed * Time.deltaTime, ForceMode.Acceleration);
        
       
    }
    public override void Exit()
    {
        base.Exit();
        _sm.anims.SetBool("Walk", false);
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
        MovePlayer();
    }
}
