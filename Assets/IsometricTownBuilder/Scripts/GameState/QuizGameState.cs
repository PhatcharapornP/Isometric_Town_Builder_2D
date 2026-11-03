using UnityEngine;

public class QuizGameState : BaseGameState
{
    public override GameStateType StateType => GameStateType.Building;
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

public class QuizHandler
{
    
}