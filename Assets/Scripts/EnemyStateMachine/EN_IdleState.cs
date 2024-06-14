using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EN_IdleState : BaseState
{
    public EN_MovementSm _sm;
    public EN_IdleState(EN_MovementSm stateMachine) : base("PL_IdleState", stateMachine) {
        _sm = (EN_MovementSm)stateMachine;
    }

    public override void Enter()
    {
        base.Enter();
        
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        if (_sm.isInFollowRange)
        {
            stateMachine.ChangeStates(_sm.en_FollowState);
        }
        if (_sm.isInAttackRange && _sm.timer < 0)
        {
            stateMachine.ChangeStates(_sm.en_AttackState);
        }
    }
}
