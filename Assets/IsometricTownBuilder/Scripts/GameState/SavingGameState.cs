using UnityEngine;

public class SavingGameState : BaseGameState
{
    public override GameStateType StateType => GameStateType.Saving;
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