public interface IGameState
{
    public GameStateType StateType { get; }
    void Initialize();
    void Enter();
    void Exit();
}