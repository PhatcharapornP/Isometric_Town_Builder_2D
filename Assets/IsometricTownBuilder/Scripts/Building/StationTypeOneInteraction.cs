using UnityEngine;

public class StationTypeOneInteraction : StationInteraction
{
    public override StationType StationType => StationType.TypeOne;
    public override void OnInputReleased(Vector3 screenPos)
    {
        //TODO: go to mixing state    
    }
}