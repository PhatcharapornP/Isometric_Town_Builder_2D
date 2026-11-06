public abstract class BaseGameState : IGameState
{
    public abstract GameStateType StateType { get; }
    
    public abstract void Initialize();

    public abstract void Enter();

    public abstract void Exit();
}