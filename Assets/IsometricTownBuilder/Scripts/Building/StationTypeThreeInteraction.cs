using Cysharp.Threading.Tasks;
using UnityEngine;

public class StationTypeThreeInteraction : StationInteraction
{
    public override StationType StationType => StationType.TypeThree;
    private BuildingGameState buildingGameState; 

    protected override async UniTask InitializeAsync()
    {
        await base.InitializeAsync();
        buildingGameState = gameStateManager.GetGameStateFromType(GameStateType.Building) as BuildingGameState;
    }
    
    public override void OnInputReleased(Vector3 screenPos)
    {
        if (!IsInitialized)
        {
            return;
        }

        if (IsInteractionBlocked())
        {
            return;
        }

        if (!IsThisBuilding(screenPos))
        {
            return;
        }

        if (gameStateManager.CurrentState.StateType is not GameStateType.Idle)
        {
            return;
        }
       
        //TODO: go to building game state + new build state -> destroy   
        gamePlayManager.GamePlayUI.EnableExitDestroyModeButton(true);
        gameStateManager.GoToState(GameStateType.Building);
        buildingGameState.ChangeBuildState(BuildState.DestroyBuilding);
    }
}