using Cysharp.Threading.Tasks;
using UnityEngine;

public class StationTypeTwoInteraction : StationInteraction
{
    public override StationType StationType => StationType.TypeTwo;
    
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
        
        gameStateManager.GoToState(GameStateType.Quiz);
    }
}