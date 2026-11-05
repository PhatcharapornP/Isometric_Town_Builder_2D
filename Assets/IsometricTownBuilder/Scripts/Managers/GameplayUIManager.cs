#nullable enable
using Cysharp.Threading.Tasks;
using UnityEngine;

public class GameplayUIManager : MonoBehaviour
{
    [SerializeField] private float minHoldingTime = .2f;
    [field: SerializeField] public bool IsInitialized { get; private set; } = false;
    
    private GameplayCanvas gameplayCanvas;
    private PlayerInputHandler? playerInput => GamePlayManager.Instance.PlayerInput;
    private GamePlayManager? gameplayManager => GamePlayManager.Instance;
    private BuildingGameState? buildingGameState => gameplayManager?.GameStateManager?.GetGameStateFromType(GameStateType.Building) as BuildingGameState;
    
    public BuildingOptionPanel? BuildingOptionPanel { get; private set; }
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Initialize().Forget();
    }

    private async UniTask Initialize()
    {
        await UniTask.WaitUntil(() => gameplayCanvas != null
                                      && playerInput != null
                                      && gameplayCanvas.BuildingOptionPanel != null);

        BuildingOptionPanel = gameplayCanvas.BuildingOptionPanel;

        if (playerInput != null)
        {
            playerInput.OnStartPressed += OnInputPressed;
            playerInput.OnStartHolding += OnInputStartHolding;
            playerInput.WhileHolding += UpdateUIWhilePlayerHoldingInput;
            playerInput.OnResetHolding += OnInputResetHolding;
            playerInput.OnEndHolding += OnInputEndHolding;
            playerInput.InputPositionUpdated += OnInputPositionUpdate;
        }
        
        IsInitialized = true;
    }

    /*
     * 1. Hold
     * 2. End hold -> pick up
     * 3. click -> place + show build option
     * 4. end with build option
     */

    private void OnInputPressed()
    {
        
    }
    
    private void OnInputStartHolding()
    {
        if (buildingGameState == null)
        {
            return;
        }
        // if (buildingGameState.CurrentBuildState is BuildState.Building)
        // {
        //     //TODO: show build options
        //     OpenBuildOptionsOnPressed();
        // }
    }

    private void UpdateUIWhilePlayerHoldingInput(float timer)
    {
        if (timer < minHoldingTime)
        {
            return;
        }

        HandleHoldingImage(timer);
    }
    
    private void OnInputResetHolding()
    {
        gameplayCanvas.EnableHoldingImage(false);
        Debug.Log($"------> {GetType().Name}.OnInputResetHolding() -> buildstate: {buildingGameState.CurrentBuildState}".LogColor(new Color(1f, 0.46f, 0.81f)));
    }
    
    private void OnInputEndHolding(Vector3 screenPos)
    {
        gameplayCanvas.EnableHoldingImage(false);
    }

    private void OnInputPositionUpdate(Vector3 screenPos)
    {
    }

    public void OpenBuildOption()
    {
        if (gameplayManager == null)
        {
            Debug.LogError($"{GetType().Name}.OpenBuildOptionsOnPressed rejected gameplay manager null".LogColor(Color.red));
            return;
        }

        if (playerInput == null)
        {
            Debug.LogError($"{GetType().Name}.OpenBuildOptionsOnPressed rejected player input null".LogColor(Color.red));
            return;
        }

        if (gameplayManager.BuildingObjSelecter == null)
        {
            Debug.LogError($"{GetType().Name}.OpenBuildOptionsOnPressed rejected on build obj selector null".LogColor(Color.red));
            return;
        }

        if (gameplayManager.BuildingObjSelecter.SelectedObject == null)
        {
            Debug.LogError($"{GetType().Name}.OpenBuildOptionsOnPressed rejected SelectedObject null".LogColor(Color.red));
            return;
        }

        if (!gameplayCanvas.BuildingOptionPanel.IsShowing)
        {
            gameplayCanvas.BuildingOptionPanel.SetPanelPositionFromScreenPosition(playerInput.ScreenPos);    
        }
        
        
        gameplayCanvas.BuildingOptionPanel.EnableOptionPanel(true);
    }

    private void HandleBuildOptionPanelOnMove()
    {
        if (gameplayManager == null)
        {
            return;
        }

        if (gameplayManager.BuildingObjSelecter == null)
        {
            return;
        }

        if (gameplayManager.BuildingObjSelecter.SelectedObject == null)
        {
            return;
        }

        if (playerInput == null)
        {
            return;
        }
        
        gameplayCanvas.BuildingOptionPanel.SetPanelPositionFromScreenPosition(playerInput.ScreenPos);
    }

    private void HandleHoldingImage(float timer)
    {
        if (playerInput == null)
        {
            return;
        }
        
        gameplayCanvas.EnableHoldingImage(true);
        gameplayCanvas.SetHoldingPosition(playerInput.ScreenPos);
        gameplayCanvas.SetHoldingImageValue(timer);
    }

    public void SetGamePlayUI(GameplayCanvas gameplayCanvas)
    {
        this.gameplayCanvas = gameplayCanvas;
    }

    
}
