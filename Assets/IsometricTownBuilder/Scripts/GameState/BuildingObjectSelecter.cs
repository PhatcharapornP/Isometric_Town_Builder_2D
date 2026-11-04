#nullable enable
using System;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class BuildingObjectSelecter : MonoBehaviour
{
    private GamePlayManager? gamePlayManager => GamePlayManager.Instance;
    private GameStateManager? gameStateManager => GamePlayManager.Instance.GameStateManager;
    private PlayerInputHandler? playerInput => GamePlayManager.Instance.PlayerInput;
    private GameplayUIManager? gameplayUI => GamePlayManager.Instance.GamePlayUI;
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
    public event Action? OnCancelBuildAction;
    public event Action? OnFlipAction;
    
    /*
     * 1. Hold
     * 2. End hold -> pick up
     * 3. click -> place + show build option
     * 4. end with build option
     */

    private void Start()
    {
        InitializeAsync().Forget();
    }
    
#if UNITY_EDITOR
    private void Update()
    {
        currentBuildState = buildingGameState.CurrentBuildState;
    }
#endif

    private async UniTask InitializeAsync()
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
            gameplayUI.BuildingOptionPanel.OnFlipBuild += OnFlip;
        }
        isInitialized = true;
    }

    private void OnFlip()
    {
        //TODO: Flip the build obj
        if (selectingObject == null)
        {
            Debug.Log($"====>>> {GetType().Name}.OnFlip() rejected selectObject null".LogColor(Color.red));
            return;
        }

        OnFlipAction?.Invoke();
        selectingObject.SetFlipSpriteRenderer(!selectingObject.FlipSpriteRenderer);
        Debug.Log($"====>>> {GetType().Name}.OnFlip() to: {selectingObject.FlipSpriteRenderer}".LogColor(Color.greenYellow));
        //TODO: Check if it still build able, otherwise back to HoldingSelectedBuilding
    }

    private void OnBuildCancel()
    {
        Debug.Log($"====>>> {GetType().Name}.OnBuildCancel()");
        OnCancelBuildAction?.Invoke();
        ResetBuildStateToIdle();
        gameplayUI.BuildingOptionPanel.EnableOptionPanel(false);
        //TODO: Reset build object to previous pos
    }

    private void OnBuildAccept()
    {
        Debug.Log($"====>>> {GetType().Name}.OnBuildAccept()");
        OnAcceptBuildAction?.Invoke();
        ResetBuildStateToIdle();
        gameplayUI.BuildingOptionPanel.EnableOptionPanel(false);
        //TODO: Save
        
    }

    private void ResetBuildStateToIdle()
    {
        UpdateBuildState(BuildState.Undefined);
        selectingObject = null;
        gameStateManager?.GoToState(GameStateType.Idle);
        Debug.Log($"{GetType().Name}.ResetBuildStateToIdle()");
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
            Debug.Log($"{GetType().Name}.OnStartSelecting()".LogColor(Color.greenYellow));
        }
    }

    private void OnStartSelecting()
    {
        if (buildingGameState == null)
        {
            return;
        }
        
        if (buildingGameState.CurrentBuildState is BuildState.HoldingSelectedBuilding)
        {
            UpdateBuildState(BuildState.Building);
        }
         
        // if (buildingGameState.CurrentBuildState is BuildState.Undefined)
        // {
        //     UpdateBuildState(BuildState.Selecting);
        //     Debug.Log($"{GetType().Name}.OnStartSelecting()".LogColor(Color.greenYellow));
        //     return;
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

        if (gameStateManager.CurrentState.StateType is not GameStateType.Building)
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

        if (buildingGameState.CurrentBuildState is not BuildState.HoldingSelectedBuilding)
        {
            return;
        }
        
        //TODO: update selecting object position
        screenToWorldPos = targetCamera.ScreenToWorldPoint(new Vector3(screenPos.x, screenPos.y, 0f));
        selectingObject.SetPosition(screenToWorldPos);
        
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
            return;
        }
        
        //do the pick up selected
        var world = targetCamera.ScreenToWorldPoint(screenPos);
        Collider2D resultCollider = Physics2D.OverlapPoint(world, mask);
        
        if (resultCollider == null)
        {
            gameStateManager.GoToState(GameStateType.Idle);
            Debug.Log($"{GetType().Name}.SelectingBuildObject() -> Selecting building object found none -> go back to idle game state");
            return;
        }

        selectingObject = resultCollider.transform.GetComponent<BuildingObject>();
        if (selectingObject != null)
        {
            gameStateManager.GoToState(GameStateType.Building);
            UpdateBuildState(BuildState.HoldingSelectedBuilding);
            Debug.Log($"current holding exceeded check found {resultCollider.transform.name}".LogColor(
                    Color.greenYellow), resultCollider.transform);
        }
        // else
        // {
        //     Debug.Log($"current holding exceeded check found {resultCollider.transform.name}".LogColor(Color.yellowNice), resultCollider.transform);
        // }
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
        
         Debug.Log($"{GetType().Name}.OnCancelSelecting() ".LogColor(new Color(1f, 0.42f, 0.15f)));
        // selectingObject = null;
        // UpdateBuildState(BuildState.Undefined);
        // gameStateManager?.GoToState(GameStateType.Idle);
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
        
        Debug.Log($"{GetType().Name}.LetGoBuildingObject() screenPos: {screenPos}".LogColor(new Color(0.65f, 1f, 0.97f)));

        if (buildingGameState.CurrentBuildState is not BuildState.HoldingSelectedBuilding)
        {
            OnCancelSelecting();
        }
    }
    
    private void UpdateBuildState( BuildState targetBuildState)
    {
        if (gameStateManager == null)
        {
            return;
        }

        if (buildingGameState == null)
        {
            return;
        }
        
        buildingGameState.ChangeBuildState(targetBuildState);
    }
}