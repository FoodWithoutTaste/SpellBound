using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PL_GlidingState : BaseState
{
    protected PL_MovementSm _sm;
    public PL_GlidingState(PL_MovementSm stateMachine) : base("PL_GlidingState", stateMachine) {
        _sm = (PL_MovementSm)stateMachine;
    }

    public override void Enter()
    {
        base.Enter();
        _sm.anims.SetBool("Glide", true);
        _sm.gravity = 0;
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();
        if (_sm.isGrounded)
        {
            stateMachine.ChangeStates(_sm.pl_IdleState);
        }
        if (Input.GetKeyUp(KeyCode.Space))
        {
            stateMachine.ChangeStates(_sm.pl_fallState);
        }

        _sm.rb.AddForce(_sm.movementInput.normalized * _sm.MoveSpeed  * Time.deltaTime, ForceMode.Acceleration);
    }

    public override void Exit()
    {
        base.Exit();
        _sm.anims.SetBool("Glide", false);
        _sm.gravity = 70;
        GameObject.Find("LandEffect").GetComponent<ParticleSystem>().Play();
    }
}
