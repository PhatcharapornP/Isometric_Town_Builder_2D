public class BuildingGameState : BaseGameState
{
    public override GameStateType StateType => GameStateType.Building;
    public BuildState CurrentBuildState { get; private set; } =  BuildState.Undefined;
    public override void Initialize()
    {
        //nothing yet
    }

    public override void Enter()
    {
    }

    public void ChangeBuildState(BuildState buildState
// #if UNITY_EDITOR
//         ,[CallerMemberName] string caller = ""
// #endif
    )
    {
        if (CurrentBuildState == buildState)
        {
            return;
        }

        CurrentBuildState = buildState;
// #if UNITY_EDITOR
//         Debug.Log($"{GetType().Name}.ChangeBuildState({buildState}) from: {caller}".LogColor(Color.yellowGreen));
// #endif
        
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