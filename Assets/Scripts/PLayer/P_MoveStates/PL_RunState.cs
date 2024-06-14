using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class PL_RunState : PL_GroundedState
{
  
    public PL_RunState(PL_MovementSm stateMachine) : base("PL_RunState", stateMachine) {}

    public override void Enter()
    {
        base.Enter();
        _sm.anims.SetBool("Run", true);
        _sm.anims.SetBool("Walk", true);
       
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();
        if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            stateMachine.ChangeStates(_sm.pl_MoveState);
        }

    }
    public void MovePlayer()
    {
        _sm.rb.AddForce(_sm.movementInput.normalized * _sm.RunSpeed * Time.deltaTime, ForceMode.Acceleration);
    }
    public override void Exit()
    {
        base.Exit();
      
        _sm.anims.SetBool("Run", false);
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
        MovePlayer();
    }
}
