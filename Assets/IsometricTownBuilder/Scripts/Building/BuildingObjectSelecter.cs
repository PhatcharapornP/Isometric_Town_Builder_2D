#nullable enable
using System;
using System.Runtime.CompilerServices;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class BuildingObjectSelecter : MonoBehaviour
{
    private GamePlayManager? gamePlayManager => GamePlayManager.Instance;
    private GameStateManager? gameStateManager => GamePlayManager.Instance.GameStateManager;
    private PlayerInputHandler? playerInput => GamePlayManager.Instance.PlayerInput;
    private GameplayUIManager? gameplayUI => GamePlayManager.Instance.GamePlayUI;
    private TilemapManager? tilemapManager => GamePlayManager.Instance.TilemapManager;
    private BoardRuleChecker boardRuleChecker => GamePlayManager.Instance.BoardRuleChecker;
    
    
    private BuildingGameState? buildingGameState => gameStateManager?.GetGameStateFromType(GameStateType.Building) as BuildingGameState;

    private Camera? targetCamera => Camera.main;

    [SerializeField] private bool isInitialized = false;
    
    [SerializeField] private BuildingObject selectingObject = null;
    [SerializeField] private LayerMask mask = 0;
    [Space] 
    [SerializeField] private Vector3 screenToWorldPos;
    [Space]
    [SerializeField] private BuildState currentBuildState;
    
    public BuildingObject SelectedObject => selectingObject;
    public Vector3 ScreenToWorldPos => screenToWorldPos;

    private float minHoldTimer = .2f;

    public event Action? OnAcceptBuildAction;
    public event Action? OnAcceptBuildFailedAction;
    public event Action? OnCancelBuildAction;
    public event Action<bool>? OnSelectedObjFlip;
    
   
#if UNITY_EDITOR
    private void Update()
    {
        currentBuildState = buildingGameState.CurrentBuildState;
    }
#endif

    public async UniTask InitializeAsync()
    {
        await UniTask.WaitUntil(() => gamePlayManager != null
                                      && gameStateManager != null
                                      && playerInput != null
                                      && gameplayUI != null
                                      && gameplayUI.BuildingOptionPanel != null);

        if (playerInput != null)
        {
            playerInput.OnStartHolding += OnStartHolding;
            playerInput.WhileHolding += WhilePlayerHolding;
            playerInput.InputPositionUpdated += OnInputPositionUpdated;
            playerInput.OnEndHolding += SelectingBuildObjectOnEndHold;
            playerInput.OnResetHolding += OnResetHolding;
            playerInput.OnReleased += OnInputReleased;
        }

        if (gameplayUI != null && gameplayUI.BuildingOptionPanel != null)
        {
            gameplayUI.BuildingOptionPanel.OnAcceptBuild += OnBuildAccept;
            gameplayUI.BuildingOptionPanel.OnCancelBuild += OnBuildCancel;
            gameplayUI.BuildingOptionPanel.OnFlipBuildAction += OnFlip;
        }
        isInitialized = true;
    }

    private void OnFlip()
    {
        //TODO: Flip the build obj
        if (selectingObject == null)
        {
            // Debug.Log($"[BuildObjSelecter]  {GetType().Name}.OnFlip() rejected selectObject null".LogColor(Color.red));
            return;
        }

        selectingObject.SetFlipSpriteRenderer(!selectingObject.RuntimeBuildData.FlipValue);
        // Debug.Log($"[BuildObjSelecter]  {GetType().Name}.OnFlip() to: {selectingObject.RuntimeBuildData.FlipValue}".LogColor(Color.greenYellow));
        
        //don't like this at all, need to refactor this and open for anyone to sub self according to one place again.
        OnSelectedObjFlip?.Invoke(selectingObject.RuntimeBuildData.FlipValue);
    }

    private void OnBuildCancel()
    {
        // Debug.Log($"[BuildObjSelecter]  {GetType().Name}.OnBuildCancel()");
        OnCancelBuildAction?.Invoke();
        
        gameplayUI.BuildingOptionPanel.EnableOptionPanel(false);

        if (selectingObject == null)
        {
            return;
        }
        
        //TODO: or destroy the buildingobj
        if (!selectingObject.HasBeenBuilt)
        {
            Destroy(selectingObject.gameObject);
        }
        else //TODO: Reset build object to previous pos
        {
            selectingObject.SetBuilt(true);
            selectingObject.SetFlipSpriteRenderer(selectingObject.BuildData.FlipValue);
            var tileWorldPos = tilemapManager.GetTileWorldPosition(selectingObject.BuildData.GetVector3OriginTile());
            selectingObject.SnapToTilePosition(tileWorldPos);
            selectingObject.ClearRunTimeData();
            var interactionBlocker = selectingObject.gameObject.AddComponent<BuildingInteractionBlocker>();
            interactionBlocker.StartTimer().Forget();
        }
        
        ResetBuildStateToIdle();
        
    }

    private void OnBuildAccept()
    {
        if (gameplayUI == null || gameplayUI.BuildingOptionPanel == null)
        {
            return;
        }
        
        if (tilemapManager == null)
        {
            OnAcceptBuildFailedAction?.Invoke();
            return;
        }

        // Debug.Log($"[BuildObjSelecter]  {GetType().Name}.OnBuildAccept() buildable: {tilemapManager.IsCurrentSpotBuildable}".LogColor(new Color(1f, 0.48f, 0.09f)));
        if (!tilemapManager.IsCurrentSpotBuildable)
        {
            OnAcceptBuildFailedAction?.Invoke();
            return;
        }
        
        OnAcceptBuildAction?.Invoke();
        gameplayUI.BuildingOptionPanel.EnableOptionPanel(false);

        if (!selectingObject.HasBeenBuilt && selectingObject.CellType.HasFlag(CellType.Station))
        {
            boardRuleChecker.AddToBuiltStationId(selectingObject.BuildingId);    
        }
        
        selectingObject.SetBuilt(true);

        var newBuildData = new BuildObjectRuntimeData(
            originTilePosition: tilemapManager.TilePos,
            builtOnTiles: tilemapManager.AllTilePosList,
            flipValue: selectingObject.RuntimeBuildData.FlipValue
        );
        
        selectingObject.OverwriteOldBuildDataWithNewOne(newBuildData);
        
        var tileWorldPos = tilemapManager.GetTileWorldPosition(selectingObject.RuntimeBuildData.GetVector3OriginTile());
        selectingObject.SnapToTilePosition(tileWorldPos);
        selectingObject.ClearRunTimeData();
        
        var interactionBlocker = selectingObject.gameObject.AddComponent<BuildingInteractionBlocker>();
        interactionBlocker.StartTimer().Forget();
        ResetBuildStateToIdle();
        tilemapManager.ResetTilePosAndNeighbourToDefaultColor();
    }

    private void ResetBuildStateToIdle()
    {
        selectingObject = null;
        gameStateManager?.GoToState(GameStateType.Idle);
        Debug.Log($"[BuildObjSelecter] {GetType().Name}.ResetBuildStateToIdle()");
    }

    private void WhilePlayerHolding(float holdTimer)
    {
        if (buildingGameState == null)
        {
            return;
        }
        
        if (buildingGameState.CurrentBuildState is BuildState.Undefined && holdTimer >= minHoldTimer)
        {
            bool hasBuildingAtScreenPos = CheckForAvailableBuildingToPickup(playerInput.ScreenPos);
            if (hasBuildingAtScreenPos)
            {
                gameStateManager.GoToState(GameStateType.Building);
                UpdateBuildState(BuildState.Selecting);
                Debug.Log($"[BuildObjSelecter] UpdateOnPlayerHolding holdTimer: {holdTimer}".LogColor(Color.greenYellow));
            }
        }
    }

    private void OnStartHolding()
    {
        if (buildingGameState == null)
        {
            return;
        }

        if (playerInput == null)
        {
            return;
        }
         
        // if (buildingGameState.CurrentBuildState is BuildState.Undefined)
        // {
        //     bool hasBuildingAtScreenPos = CheckForAvailableBuildingToPickup(playerInput.ScreenPos);
        //     if (!hasBuildingAtScreenPos)
        //     {
        //         Debug.Log($"[BuildObjSelecter] {GetType().Name}.OnStartSelecting() | hasBuildingAtScreenPos: false".LogColor(new Color(1f, 0.28f, 0.31f)));
        //         gameStateManager.GoToState(GameStateType.Idle);
        //     }
        //     else
        //     {
        //         gameStateManager.GoToState(GameStateType.Building);
        //         UpdateBuildState(BuildState.Selecting);
        //         Debug.Log($"[BuildObjSelecter] {GetType().Name}.OnStartSelecting() | UpdateBuildState to Selecting".LogColor(Color.greenYellow));
        //     }
        // }
    }
    
    private void OnInputPositionUpdated(Vector3 screenPos)
    {
        if (!isInitialized)
        {
            return;
        }

        if (targetCamera == null)
        {
            return;
        }
        
        if (gameStateManager == null)
        {
            return;
        }

        if (gameStateManager.CurrentState == null)
        {
            return;
        }
        
        if (selectingObject == null )
        {
            return;
        }

        if (buildingGameState == null)
        {
            return;
        }

        if (buildingGameState.CurrentBuildState != BuildState.HoldingSelectedBuilding)
        {
            return;
        }
        
        //update selecting object position
        screenToWorldPos = targetCamera.ScreenToWorldPoint(new Vector3(screenPos.x, screenPos.y, 0f));
        selectingObject.SetPosition(screenToWorldPos);
        
    }

    public bool CheckForAvailableBuildingToPickup(Vector3 screenPos)
    {
        if (!isInitialized)
        {
            return false;
        }
        
        if (targetCamera == null)
        {
            return false;
        }
        
        if (buildingGameState == null)
        {
            return false;
        }
        
        //do the pick up selected
        var world = targetCamera.ScreenToWorldPoint(screenPos);
        Collider2D resultCollider = Physics2D.OverlapPoint(world, mask);

        if (resultCollider == null)
        {
            return false;
        }

        var availableObj = resultCollider.transform.GetComponent<BuildingObject>();
        return availableObj != null;
    }
    
    private void SelectingBuildObjectOnEndHold(Vector3 screenPos)
    {
        if (!isInitialized)
        {
            return;
        }

        if (targetCamera == null || gameStateManager == null)
        {
            return;
        }
        
        if (buildingGameState == null)
        {
            return;
        }

        if (buildingGameState.CurrentBuildState is not BuildState.Selecting)
        {
            Debug.Log($"[BuildObjSelecter] OnEndHold CurrentBuildState is not selecting: {buildingGameState.CurrentBuildState} rejected".LogColor(Color.softRed));
            return;
        }
        
        //do the pick up selected
        // var world = targetCamera.ScreenToWorldPoint(screenPos);
        // Collider2D resultCollider = Physics2D.OverlapPoint(world, mask);
        //
        // if (resultCollider == null)
        // {
        //     Debug.Log($"[BuildObjSelecter] OnEndHold couldn't find any collider 2D rejected".LogColor(Color.softRed));
        //     
        //     return;
        // }
        //
        // selectingObject = resultCollider.transform.GetComponent<BuildingObject>();

        bool gotBuildingObject = TryGetBuildingObject(screenPos, out selectingObject);

        if (!gotBuildingObject)
        {
            Debug.Log($"[BuildObjSelecter] OnEndHold couldn't find any collider 2D rejected".LogColor(Color.softRed));
            return;
        }
        
        if (selectingObject != null)
        {
            selectingObject.CacheBuildToRuntimeData();
            
            BoardManager.RemoveBuildData(selectingObject.RuntimeBuildData);
            if (selectingObject.CellType is CellType.Station)
            {
                boardRuleChecker.RemoveStation(selectingObject.BuildingId);    
            }
            
            gameStateManager.GoToState(GameStateType.Building);
            UpdateBuildState(BuildState.HoldingSelectedBuilding);
            Debug.Log($"[BuildObjSelecter] current holding exceeded check found {selectingObject.name}".LogColor(
                    Color.greenYellow), selectingObject.transform);
        }
        else
        {
            gameStateManager.GoToState(GameStateType.Idle);
            UpdateBuildState(BuildState.Undefined);
            
            Debug.Log($"[BuildObjSelecter] current holding exceeded check found {selectingObject.name} but not building".LogColor(Color.softRed), selectingObject.transform);
        }
    }

    public void SetSelectingObject(BuildingObject targetBuildingObject)
    {
        selectingObject = targetBuildingObject;
    }

    private void OnResetHolding()
    {
        if (buildingGameState == null)
        {
            return;
        }

        if (gameStateManager == null)
        {
            return;
        }

        if (gameStateManager.CurrentState == null)
        {
            return;
        }
        
        if (selectingObject == null)
        {
            return;
        }

        if (gameStateManager.CurrentState.StateType is GameStateType.Building 
            && buildingGameState.CurrentBuildState is BuildState.HoldingSelectedBuilding)
        {
            Debug.Log($"[BuildObjSelecter] {GetType().Name}.OnCancelSelecting() set build state to building...".LogColor(new Color(1f, 0.42f, 0.15f)));
            UpdateBuildState(BuildState.Building);
            gameplayUI.OpenBuildOption();
        }
    }

    private void OnInputReleased(Vector3 screenPos)
    {
        if (buildingGameState == null)
        {
            return;
        }
        
        if (buildingGameState.CurrentBuildState is BuildState.DestroyBuilding)
        {
            DestroySelectingBuilding(screenPos);
            return;
        }

        if (selectingObject == null)
        {
            return;
        }

        if (buildingGameState.CurrentBuildState is not BuildState.HoldingSelectedBuilding)
        {
            Debug.Log($"[BuildObjSelecter] {GetType().Name}.OnInputReleased() screenPos: {screenPos} -> OnCancelSelecting".LogColor(new Color(1f, 0.49f, 0.11f)));
            OnResetHolding();
        }
    }

    private void DestroySelectingBuilding(Vector3 screenPos)
    {
        bool gotBuildingObject = TryGetBuildingObject(screenPos, out var buildingResult);
        if (!gotBuildingObject)
        {
            Debug.Log($"[BuildObjSelecter] DestroySelectingBuilding couldn't find any collider 2D rejected".LogColor(Color.softRed));
            return;
        }

        if (buildingResult == null )
        {
            Debug.Log($"[BuildObjSelecter] DestroySelectingBuilding couldn't find buildingResult rejected".LogColor(Color.softRed));
            return;
        }

        if (buildingResult.HasBuildingProtector())
        {
            Debug.Log($"[BuildObjSelecter] DestroySelectingBuilding building {buildingResult.name} has protection rejected".LogColor(Color.softRed));
            return;
        }
        
        //Remove this building from board
        BoardManager.RemoveBuildData(buildingResult.BuildData);
        BoardManager.SaveBoard();
        
        //Remove from board rule checker
        if (buildingResult.CellType is CellType.Station)
        {
            boardRuleChecker.RemoveStation(buildingResult.BuildingId);
        }
        
        //Proceed to destroy this building
        Destroy(buildingResult.gameObject);
        
    }

    public bool TryGetBuildingObject(Vector3 screenPos, out BuildingObject? buildingObjectResult)
    {
        var world = targetCamera.ScreenToWorldPoint(screenPos);
        Collider2D resultCollider = Physics2D.OverlapPoint(world, mask);
        
        if (resultCollider == null)
        {
            // Debug.Log($"[BuildObjSelectker] TryGetBuildingOBject couldn't find any collider 2D rejected".LogColor(Color.softRed));
            buildingObjectResult = null;
            return false;
        }

        buildingObjectResult = resultCollider.transform.GetComponent<BuildingObject>();
        return true;
    }

    private void UpdateBuildState(BuildState targetBuildState
#if UNITY_EDITOR
        , [CallerMemberName] string caller = ""
#endif
    )
    {
        if (gameStateManager == null)
        {
            return;
        }

        if (buildingGameState == null)
        {
            return;
        }

#if UNITY_EDITOR
        Debug.Log($"[BuildObjSelecter] {GetType().Name}.UpdateBuildState({targetBuildState}) from: {caller}".LogColor(new Color(1f, 0.83f, 0.27f)));
#endif  
        
        buildingGameState.ChangeBuildState(targetBuildState);
    }
}