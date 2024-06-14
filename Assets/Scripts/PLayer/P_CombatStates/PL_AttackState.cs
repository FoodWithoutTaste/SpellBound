using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
public class PL_AttackState : BaseState
{
    protected float timer = 0;
    protected float maxTimer;
    protected bool started = false;
    protected PL_CombatSm _sm;
    public PL_AttackState(string name, PL_CombatSm stateMachine) : base(name, stateMachine)
    {
        _sm = (PL_CombatSm)stateMachine;
    }

    public override void Enter()
    {
        base.Enter();
        timer = 1f;
        maxTimer = timer;
        started = true;
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        if (timer >= 0 && started)
        {
            timer -= Time.deltaTime;
        }
        if (timer < 0)
        {
            stateMachine.ChangeStates(((PL_CombatSm)stateMachine).pl_defaultState);
        }

    }
    public override void Exit()
    {
        base.Exit();
        started = false;
        timer = 0;


    }
}
