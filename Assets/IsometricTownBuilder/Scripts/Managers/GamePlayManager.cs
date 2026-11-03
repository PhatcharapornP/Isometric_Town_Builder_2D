#nullable enable
using Cysharp.Threading.Tasks;
using UnityEngine;

public class GamePlayManager : MonoBehaviour
{
    [Space,Header("Managers")] 
    [SerializeField] private GameStateManager? gameStateManager;
    [SerializeField] private BoardManager boardManager = new BoardManager();
    [SerializeField] private GamePlayUIManager gameplayUIManager;
    
    [Space,Header("GameConfigs")] 
    [SerializeField] private GameConfigContainer gameConfigContainer;
    
    public static GamePlayManager Instance;
    public GameStateManager GameStateManager => gameStateManager;
    public BoardManager BoardManager => boardManager;
    
    
    [Space,Header("testing input")]
    [SerializeField] private Vector2Int gridPosition = Vector2Int.zero;
    [SerializeField] private PlaceableObjectConfigData testingPlaceableObj;
    
    [Space,Header("Output")]
    [SerializeField] private bool placeable = false;
    [SerializeField] private Vector2Int[]? cellPositionOutput;
    
    private void Start()
    {
        Instance = this;
        gameStateManager?.Initialize();
        Debug.Log($"{GetType().Name}.Start() called",gameObject);
        
        GoToIdleWhenReadyAsync().Forget();
    }

    private async UniTask GoToIdleWhenReadyAsync()
    {
        await UniTask.WaitUntil(()=>gameStateManager != null && gameStateManager.IsInitialized);
        Debug.Log($"{GetType().Name}.GoToIdleWhenReady() called");
        //already check for not null
        gameStateManager.GoToState(GameStateType.Idle);
        LoadingCanvas.Instance?.FullScreenLoading.HideLoading();
    }

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
