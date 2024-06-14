using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PL_FallState : BaseState
{
    protected PL_MovementSm _sm;
    public PL_FallState(PL_MovementSm stateMachine) : base("PL_FallState", stateMachine) {
        _sm = (PL_MovementSm)stateMachine;
    }

    public override void Enter()
    {
        base.Enter();
        _sm.anims.SetBool("Fall", true);
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();
        _sm.anims.SetBool("Fall", true);
        if (_sm.isGrounded)
        {
            stateMachine.ChangeStates(_sm.pl_IdleState);
        }
        else
        {
            if (Input.GetKey(KeyCode.Space))
            {
                stateMachine.ChangeStates(_sm.pl_glidingState);
            }
        }
    }
    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
     
    }

    public override void Exit()
    {
        base.Exit();
        _sm.anims.SetBool("Fall", false);
        GameObject.Find("LandEffect").GetComponent<ParticleSystem>().Play();
    }
}
