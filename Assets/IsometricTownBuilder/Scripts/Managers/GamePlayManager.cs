#nullable enable
using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class GamePlayManager : MonoBehaviour
{
    [Space, Header("Managers")]
    [SerializeField] private GameStateManager? gameStateManager;
    [SerializeField] private GameplayUIManager? gameplayUIManager;
    [SerializeField] private PlayerInputHandler? playerInput;
    [SerializeField] private BuildingObjectSelecter? buildingObjSelecter;
    [SerializeField] private TilemapManager tilemapManager;
    [Space, Header("GameConfigs")] 
    [SerializeField] private GameConfigContainer gameConfigContainer;
    [SerializeField] private PlaceableObjectConfigData testingPlaceableObj;
    private BuildingLoader buildingLoader;
    private BoardRuleChecker boardRuleChecker;

    public GameStateManager? GameStateManager => gameStateManager;
    public PlayerInputHandler? PlayerInput => playerInput;

    public GameplayUIManager? GamePlayUI => gameplayUIManager;
    public BuildingObjectSelecter? BuildingObjSelecter => buildingObjSelecter;
    public GameConfigContainer? GameConfigContainer => gameConfigContainer;
    
    public TilemapManager? TilemapManager => tilemapManager;
    
    public BoardRuleChecker BoardRuleChecker => boardRuleChecker;

    public static GamePlayManager Instance;
    public bool IsInitialized { get; private set; } = false;
    

    private void Start()
    {
        Instance = this;
        BoardManager.LoadSaveData();
        gameStateManager?.Initialize();
        InitializeAllManagers().Forget();
    }

    private async UniTask InitializeAllManagers()
    {
        var initTasks = new List<UniTask>();
        initTasks.Add(gameplayUIManager.InitializeAsync());
        initTasks.Add(buildingObjSelecter.InitializeAsync());
        initTasks.Add(tilemapManager.InitializeAsync());
        
        await UniTask.WhenAll(initTasks);
        boardRuleChecker = new BoardRuleChecker(gameConfigContainer.BoardRuleConfig);
        buildingLoader = gameObject.AddComponent<BuildingLoader>();
        buildingLoader.Initialize(
            catalogConfig: gameConfigContainer.BuildingCatalogConfig,
            tileMap: tilemapManager,
            ruleChecker: boardRuleChecker
        );
        buildingLoader.SpawnSavedBuildingIntoGame();
        IsInitialized = true;
        await GoToIdleWhenReadyAsync();
    }

    private async UniTask GoToIdleWhenReadyAsync()
    {
        gameStateManager?.GoToState(GameStateType.Idle);
        LoadingCanvas.Instance?.FullScreenLoading.HideLoading();
    }
}