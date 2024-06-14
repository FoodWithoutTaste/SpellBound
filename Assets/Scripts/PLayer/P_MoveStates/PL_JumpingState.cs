using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PL_JumpingState : BaseState
{
    protected PL_MovementSm _sm;

    public PL_JumpingState(PL_MovementSm stateMachine) : base("PL_JumpingState", stateMachine) {
        _sm = (PL_MovementSm)stateMachine;
    }

    public override void Enter()
    {
        base.Enter();
        Debug.Log("StartJumop");
        _sm.anims.SetTrigger("Jump");
        float a = _sm.rb.drag;
        _sm.rb.drag = 6;
        _sm.rb.AddForce(_sm.gravityFaux.fauxDirection * -1 * _sm.jumpSpeed * 100 , ForceMode.Impulse);
          _sm.rb.drag = 6;
        _sm.jumpCoolDown = 0.89f;
      
    }
    public override void LogicUpdate()
    {
        base.LogicUpdate();
        
    }
    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
        if (_sm.isGrounded)
        {
  
            stateMachine.ChangeStates(((PL_MovementSm)stateMachine).pl_IdleState);
        }
    }

}
