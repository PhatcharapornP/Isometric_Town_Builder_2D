#nullable enable
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class GamePlayManager : MonoBehaviour
{
    [Space, Header("Managers")]
    [SerializeField] private GameStateManager? gameStateManager;
    [SerializeField] private BoardManager boardManager = new BoardManager();
    [SerializeField] private GameplayUIManager? gameplayUIManager;
    [SerializeField] private PlayerInputHandler? playerInput;
    [SerializeField] private BuildingObjectSelecter? buildingObjSelecter;
    [SerializeField] private TilemapManager tilemapManager;
    [Space, Header("GameConfigs")] 
    [SerializeField] private GameConfigContainer gameConfigContainer;

    [SerializeField] private PlaceableObjectConfigData testingPlaceableObj;

    public GameStateManager? GameStateManager => gameStateManager;
    public BoardManager? BoardManager => boardManager;
    public PlayerInputHandler? PlayerInput => playerInput;

    public GameplayUIManager? GamePlayUI => gameplayUIManager;
    public BuildingObjectSelecter? BuildingObjSelecter => buildingObjSelecter;
    public GameConfigContainer? GameConfigContainer => gameConfigContainer;
    
    public TilemapManager? TilemapManager => tilemapManager;

    public static GamePlayManager Instance;

    private void Start()
    {
        Instance = this;
        gameStateManager?.Initialize();
        boardManager?.Initialize();
        InitializeAllManagers().Forget();
    }

    private async UniTask InitializeAllManagers()
    {
        var initTasks = new List<UniTask>();
        initTasks.Add(gameplayUIManager.InitializeAsync());
        initTasks.Add(buildingObjSelecter.InitializeAsync());
        initTasks.Add(tilemapManager.InitializeAsync());
        
        await UniTask.WhenAll(initTasks);
        await GoToIdleWhenReadyAsync();
    }

    private async UniTask GoToIdleWhenReadyAsync()
    {
        await UniTask.WaitUntil(() => gameStateManager != null && gameStateManager.IsInitialized);
        gameStateManager?.GoToState(GameStateType.Idle);
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