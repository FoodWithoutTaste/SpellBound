using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PL_GroundedState : BaseState
{
    protected PL_MovementSm _sm;
    public string GroundedState;
    public PL_GroundedState(string name ,PL_MovementSm stateMachine) : base(name, stateMachine) {
        _sm = (PL_MovementSm)stateMachine;
    }

    public override void Enter()
    {
        base.Enter();
      
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();
        if (!_sm.isGrounded && _sm.jumpCoolDown < 0.1f)
        {
            stateMachine.ChangeStates(_sm.pl_fallState);
        }
        if (Input.GetKeyDown(KeyCode.Space) &&_sm.isGrounded)
        {
            stateMachine.ChangeStates(_sm.pl_JumpingState);
        }
      
          

    }
       
}
