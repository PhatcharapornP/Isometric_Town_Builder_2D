using UnityEngine;

public class MixingGameState : BaseGameState
{
    public override GameStateType StateType => GameStateType.Mixing;
    public override void Initialize()
    {
        //nothing yet
        //open mixing panel
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