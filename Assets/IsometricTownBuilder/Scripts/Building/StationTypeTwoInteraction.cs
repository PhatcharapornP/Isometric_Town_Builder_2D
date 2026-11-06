using UnityEngine;

public class StationTypeTwoInteraction : StationInteraction
{
    public override StationType StationType => StationType.TypeTwo;
    public override void OnInputReleased(Vector3 screenPos)
    {
        //TODO: go to quiz state    
    }
}