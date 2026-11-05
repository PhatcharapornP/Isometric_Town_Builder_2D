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
    
    /*
     * 1. Hold
     * 2. End hold -> pick up
     * 3. click -> place + show build option
     * 4. If the build option hasn't close it self after clicked any button. The build object must be able to be pick up again.
     * 5. end with build option
     */
   
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
            playerInput.OnStartHolding += OnStartSelecting;
            playerInput.WhileHolding += UpdateOnPlayerHolding;
            playerInput.InputPositionUpdated += OnInputPositionUpdated;
            playerInput.OnEndHolding += SelectingBuildObjectOnEndHold;
            playerInput.OnResetHolding += OnCancelSelecting;
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
            Debug.Log($"[BuildObjSelecter]  {GetType().Name}.OnFlip() rejected selectObject null".LogColor(Color.red));
            return;
        }

        selectingObject.SetFlipSpriteRenderer(!selectingObject.FlipSpriteRenderer);
        Debug.Log($"[BuildObjSelecter]  {GetType().Name}.OnFlip() to: {selectingObject.FlipSpriteRenderer}".LogColor(Color.greenYellow));
        
        //don't like this at all, need to refactor this and open for anyone to sub self according to one place again.
        OnSelectedObjFlip?.Invoke(selectingObject.FlipSpriteRenderer);
        
        //TODO: Check if it still build able, otherwise back to HoldingSelectedBuilding
    }

    private void OnBuildCancel()
    {
        Debug.Log($"[BuildObjSelecter]  {GetType().Name}.OnBuildCancel()");
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
            var tileWorldPos = tilemapManager.GetTileWorldPosition(selectingObject.OriginBuiltOnTile);
            selectingObject.SnapToTilePosition(tileWorldPos);
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
            Debug.LogError($"[BuildObjSelecter] BRUHHH!! {GetType().Name}.OnBuildAccept(): tilemapManager is null".LogColor(Color.red));
            OnAcceptBuildFailedAction?.Invoke();
            return;
        }

        Debug.Log($"[BuildObjSelecter]  {GetType().Name}.OnBuildAccept() buildable: {tilemapManager.IsCurrentSpotBuildable}".LogColor(new Color(1f, 0.48f, 0.09f)));
        
        if (!tilemapManager.IsCurrentSpotBuildable)
        {
            OnAcceptBuildFailedAction?.Invoke();
            return;
        }
        
        OnAcceptBuildAction?.Invoke();
        gameplayUI.BuildingOptionPanel.EnableOptionPanel(false);
        selectingObject.SetBuilt(true);
        selectingObject.SetBuiltOnTiles(tilemapManager.SelectedTiles);
        selectingObject.SetOriginBuiltOnTile(tilemapManager.TilePos);
        var tileWorldPos = tilemapManager.GetTileWorldPosition(selectingObject.OriginTileWhileHolding);
        selectingObject.SnapToTilePosition(tileWorldPos);
        selectingObject.SetBuiltPosition(selectingObject.transform.position);
        ResetBuildStateToIdle();
    }

    private void ResetBuildStateToIdle()
    {
        selectingObject = null;
        UpdateBuildState(BuildState.Undefined);
        gameStateManager?.GoToState(GameStateType.Idle);
        Debug.Log($"[BuildObjSelecter] {GetType().Name}.ResetBuildStateToIdle()");
    }

    private void UpdateOnPlayerHolding(float holdTimer)
    {
        if (buildingGameState == null)
        {
            return;
        }
        
        if (buildingGameState.CurrentBuildState is BuildState.Undefined && holdTimer >= minHoldTimer)
        {
            UpdateBuildState(BuildState.Selecting);
            Debug.Log($"[BuildObjSelecter] {GetType().Name}.OnStartSelecting()".LogColor(Color.greenYellow));
        }
    }

    private void OnStartSelecting()
    {
        if (buildingGameState == null)
        {
            return;
        }
      
        
        //comment out while fixing picking up building from catalog
        // if (buildingGameState.CurrentBuildState is BuildState.HoldingSelectedBuilding)
        // {
        //     UpdateBuildState(BuildState.Building);
        // }

        if (playerInput == null)
        {
            return;
        }
         
        if (buildingGameState.CurrentBuildState is BuildState.Undefined)
        {
            gameStateManager.GoToState(GameStateType.Building);
            UpdateBuildState(BuildState.Selecting);
            Debug.Log($"[BuildObjSelecter] {GetType().Name}.OnStartSelecting() | UpdateBuildState to Selecting".LogColor(Color.greenYellow));
            bool hasBuildingAtScreenPos = CheckForAvailableBuildingToPickup(playerInput.ScreenPos);
            if (!hasBuildingAtScreenPos)
            {
                Debug.Log($"[BuildObjSelecter] {GetType().Name}.OnStartSelecting() | hasBuildingAtScreenPos: false".LogColor(new Color(1f, 0.28f, 0.31f)));
                UpdateBuildState(BuildState.Undefined);
                gameStateManager.GoToState(GameStateType.Idle);
            }
        }
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
        
        if (targetCamera == null || gameStateManager == null)
        {
            return false;
        }
        
        if (buildingGameState == null)
        {
            return false;
        }

        if (buildingGameState.CurrentBuildState is not BuildState.Selecting)
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
        var world = targetCamera.ScreenToWorldPoint(screenPos);
        Collider2D resultCollider = Physics2D.OverlapPoint(world, mask);
        
        if (resultCollider == null)
        {
            Debug.Log($"[BuildObjSelecter] OnEndHold couldn't find any collider 2D rejected".LogColor(Color.softRed));
            
            return;
        }

        selectingObject = resultCollider.transform.GetComponent<BuildingObject>();
        if (selectingObject != null)
        {
            gameStateManager.GoToState(GameStateType.Building);
            UpdateBuildState(BuildState.HoldingSelectedBuilding);
            Debug.Log($"[BuildObjSelecter] current holding exceeded check found {resultCollider.transform.name}".LogColor(
                    Color.greenYellow), resultCollider.transform);
        }
        else
        {
            gameStateManager.GoToState(GameStateType.Idle);
            UpdateBuildState(BuildState.Undefined);
            
            Debug.Log($"[BuildObjSelecter] current holding exceeded check found {resultCollider.transform.name} but not building".LogColor(Color.softRed), resultCollider.transform);
        }
    }

    public void SetSelectingObject(BuildingObject targetBuildingObject)
    {
        selectingObject = targetBuildingObject;
    }

    private void OnCancelSelecting()
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

        if (gameStateManager.CurrentState.StateType is not GameStateType.Building)
        {
            return;
        }

        if (selectingObject == null)
        {
            return;
        }

        if (buildingGameState.CurrentBuildState is not BuildState.HoldingSelectedBuilding)
        {
            return;
        }
        
         Debug.Log($"[BuildObjSelecter] {GetType().Name}.OnCancelSelecting() set build state to building...".LogColor(new Color(1f, 0.42f, 0.15f)));
        // selectingObject = null;
        // gameStateManager?.GoToState(GameStateType.Idle);
        UpdateBuildState(BuildState.Building);
        //TODO: Trigger build option...
        
        gameplayUI.OpenBuildOption();
    }

    private void OnInputReleased(Vector3 screenPos)
    {
        if (selectingObject == null )
        {
            return;
        }

        if (buildingGameState == null)
        {
            return;
        }
        
        

        if (buildingGameState.CurrentBuildState is not BuildState.HoldingSelectedBuilding)
        {
            Debug.Log($"[BuildObjSelecter] {GetType().Name}.OnInputReleased() screenPos: {screenPos} -> OnCancelSelecting".LogColor(new Color(1f, 0.49f, 0.11f)));
            OnCancelSelecting();
        }
        else
        {
            Debug.Log($"[BuildObjSelecter] {GetType().Name}.OnInputReleased() screenPos: {screenPos}".LogColor(new Color(0.65f, 1f, 0.97f)));
        }
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