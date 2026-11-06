using UnityEngine;

public class IdleGameState : BaseGameState
{
    public override GameStateType StateType => GameStateType.Idle;

    public override void Initialize()
    {
        //Nothing yet
        
    }

    public override void Enter()
    {
        // Debug.Log($"{GetType().Name}.OnEnter()");
    }

    public override void Exit()
    {
        // Debug.Log($"{GetType().Name}.OnExit()");
    }
}