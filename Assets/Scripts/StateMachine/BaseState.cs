using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseState 
{
    public string name;
    public StateMachine stateMachine;

    public BaseState(string name , StateMachine stateMachine)
    {
        this.name = name;
        this.stateMachine = stateMachine;
    }

    public virtual void Enter(){}
    public virtual void LogicUpdate(){}
    public virtual void PhysicsUpdate(){}
    public virtual void Exit(){}
}
