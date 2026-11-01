#nullable enable
using UnityEngine;
using UnityEngine.Serialization;

public class GamePlayManager : MonoBehaviour
{
    
    
    [Space,Header("testing input")]
    [SerializeField] private Vector2Int gridPosition = Vector2Int.zero;
    [SerializeField] private PlaceableObjectConfigData testingPlaceableObj;
    
    [Space,Header("Output")]
    [SerializeField] private bool placeable = false;
    [SerializeField] private Vector2Int[]? cellPositionOutput;

#if false
    
    //for testing only with out playing only [for now]
    private void OnValidate()
    {
       // if (!boardManager.IsInitialized)
        {
            boardManager.SetupRuntimeBoardFromConfig(gameConfigContainer.BoardConfig);
        }

        placeable = boardManager.TryPlaceCellAt(
            position: gridPosition,
            objectSpec: testingPlaceableObj,
            cellPositionRequire: out cellPositionOutput
        );
        
        //test edit the grid
        
        
        //testing save edited grid into player pref (string)
        //testing load grid data into runtime cache
        
    }
#endif

    
}
