using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EN_FollowState :BaseState
{
    public EN_MovementSm _sm;
    public EN_FollowState(EN_MovementSm stateMachine) : base("EN_FollowState", stateMachine) {
        _sm = (EN_MovementSm)stateMachine;

    }

    public override void Enter()
    {
        base.Enter();
        if (_sm.target != null)
        { 
        
        }

            _sm.aiAgent.enabled = true;
       
        _sm.aiAgent.speed = _sm.moveSpeed;
    }


    public override void LogicUpdate()
    {
        base.LogicUpdate();
        if (_sm.isInAttackRange)
        {
            if(_sm.timer < 0)
            {
                stateMachine.ChangeStates(_sm.en_AttackState);
              
            }
            else
            {
                _sm.aiAgent.enabled = false;
                _sm.anims.SetBool("Walk", false);

            }
         
        }
        else
        {
            _sm.aiAgent.enabled = true;
            _sm.anims.SetBool("Walk", true);
            if(_sm.target!= null)
            {
                _sm.aiAgent.destination = _sm.target.position;
            }
            
        }
        
        if (_sm.isInFollowRange == false)
        {
            stateMachine.ChangeStates(_sm.en_IdleState);
        }
       
        if (_sm.aiAgent.velocity.sqrMagnitude > Mathf.Epsilon)
        {
            _sm.visuals.transform.rotation = Quaternion.LookRotation(_sm.aiAgent.velocity.normalized);
        }
       
       
    }
    public override void Exit()
    {
        base.Exit();
        _sm.anims.SetBool("Walk", false);
        _sm.aiAgent.enabled = false;
    }
}
