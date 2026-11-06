public class BuildingGameState : BaseGameState
{
    public override GameStateType StateType => GameStateType.Building;
    public BuildState CurrentBuildState { get; private set; } =  BuildState.Undefined;
    public override void Initialize()
    {
    }

    public override void Enter()
    {
    }

    public void ChangeBuildState(BuildState buildState)
    {
        if (CurrentBuildState == buildState)
        {
            return;
        }

        CurrentBuildState = buildState;
    }

    public override void Exit()
    {
        ChangeBuildState(BuildState.Undefined);
    }
}


public enum BuildState
{
    Undefined,
    Selecting,
    Building,
    HoldingSelectedBuilding,
    DestroyBuilding,
}