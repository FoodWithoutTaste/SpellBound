using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateMachine : MonoBehaviour
{
    [SerializeField] public BaseState currentState;
    [Header("CurrentState")]
    public string currStateName;

    void Start()
    {
        currentState = GetInitialState();
        if (currentState != null)
            currentState.Enter();
    }
    public virtual void Update()
    {

        if (currentState != null)
        {
            currentState.LogicUpdate();
            currStateName = currentState.name;
        }
            
    }
    void LateUpdate()
    {
        if (currentState != null)
            currentState.PhysicsUpdate();
    }
    public void ChangeStates(BaseState newState)
    {
        currentState.Exit();
        currentState = newState;
        currentState.Enter();
    }

    protected virtual BaseState GetInitialState()
    {
        return null;
    }
}
