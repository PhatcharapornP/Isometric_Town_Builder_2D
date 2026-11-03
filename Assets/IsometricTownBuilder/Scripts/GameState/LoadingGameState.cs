using UnityEngine;

public class LoadingGameState : BaseGameState
{
    public override GameStateType StateType => GameStateType.Loading;
    public override void Initialize()
    {
        //nothing yet
    }

    public override void Enter()
    {
        Debug.Log($"{GetType().Name}.OnEnter()");
    }

    public override void Exit()
    {
        Debug.Log($"{GetType().Name}.OnExit()");
    }
}