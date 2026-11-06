#nullable enable
using System.Runtime.CompilerServices;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class GameplayUIManager : MonoBehaviour
{
    [SerializeField] private float minHoldingTime = .2f;
    [SerializeField] private float midScreenTexTimer = .5f;
    
    [field: SerializeField] public bool IsInitialized { get; private set; } = false;
    
    private GameplayCanvas gameplayCanvas;
    private PlayerInputHandler? playerInput => GamePlayManager.Instance.PlayerInput;
    private GamePlayManager? gameplayManager => GamePlayManager.Instance;
    private BuildingGameState? buildingGameState => gameplayManager?.GameStateManager?.GetGameStateFromType(GameStateType.Building) as BuildingGameState;
    
    public BuildingOptionPanel? BuildingOptionPanel { get; private set; }

    public QuizPopup? QuizPopup;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    public async UniTask InitializeAsync()
    {
        await UniTask.WaitUntil(() => gameplayCanvas != null
                                      && gameplayCanvas.IsInitialized
                                      && playerInput != null
                                      && gameplayCanvas.BuildingOptionPanel != null);

        BuildingOptionPanel = gameplayCanvas.BuildingOptionPanel;
        gameplayCanvas.ExitDestroyModeButton.AddToClickAction(GoToIdleFromDestroyBuildingState);
        QuizPopup = gameplayCanvas.QuizPopup;

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
        // Debug.Log($"[GameplayUI] OnInputResetHolding() -> buildstate: {buildingGameState.CurrentBuildState}".LogColor(new Color(1f, 0.46f, 0.81f)));
        gameplayCanvas.EnableHoldingImage(false);
    }
    
    private void OnInputEndHolding(Vector3 screenPos)
    {
        // Debug.Log($"[GameplayUI] OnInputEndHolding() -> buildstate: {buildingGameState.CurrentBuildState}".LogColor(new Color(0.68f, 0.78f, 1f)));
        gameplayCanvas.EnableHoldingImage(false);
    }

    private void OnInputPositionUpdate(Vector3 screenPos)
    {
    }

    public void OpenBuildOption()
    {
        if (gameplayManager == null)
        {
            Debug.LogError($"[GameplayUI] OpenBuildOptionsOnPressed rejected gameplay manager null".LogColor(Color.red));
            return;
        }

        if (playerInput == null)
        {
            Debug.LogError($"[GameplayUI] OpenBuildOptionsOnPressed rejected player input null".LogColor(Color.red));
            return;
        }

        if (gameplayManager.BuildingObjSelecter == null)
        {
            Debug.LogError($"[GameplayUI] OpenBuildOptionsOnPressed rejected on build obj selector null".LogColor(Color.red));
            return;
        }

        if (gameplayManager.BuildingObjSelecter.SelectedObject == null)
        {
            Debug.LogError($"[GameplayUI] OpenBuildOptionsOnPressed rejected SelectedObject null".LogColor(Color.red));
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

        // bool hasAvailableBuildingToPickup = gameplayManager.BuildingObjSelecter.CheckForAvailableBuildingToPickup(playerInput.ScreenPos);
        if (gameplayManager.GameStateManager.CurrentState.StateType is not GameStateType.Building)
        {
            return;
        }

        var buildGameState = gameplayManager.GameStateManager.CurrentState as BuildingGameState;

        if (buildGameState == null)
        {
            return;
        }

        if (buildGameState.CurrentBuildState is BuildState.HoldingSelectedBuilding)
        {
            return;
        }
        
        {
            gameplayCanvas.EnableHoldingImage(true);
            gameplayCanvas.SetHoldingPosition(playerInput.ScreenPos);
            gameplayCanvas.SetHoldingImageValue(timer);
        }
    }

    

    public void SetGamePlayUI(GameplayCanvas gameplayCanvas)
    {
        this.gameplayCanvas = gameplayCanvas;
    }

    public void HideBuildingCatalogPanel()
    {
        gameplayCanvas.BuildCatalogPanel.Hide();
    }

    private void GoToIdleFromDestroyBuildingState()
    {
        gameplayManager?.GameStateManager?.GoToState(GameStateType.Idle);
        EnableExitDestroyModeButton(false);
    }

    public void EnableExitDestroyModeButton(bool enable
    #if UNITY_EDITOR
        ,[CallerMemberName] string caller = ""
    #endif
    )
    {
        #if UNITY_EDITOR
        Debug.Log($"[GamePlayUI] EnableExitDestroyModeButton {enable} from: {caller}".LogColor(new Color(1f, 0.8f, 0.22f)));
        #endif
        gameplayCanvas.EnableExitDestroyMode(enable);
    }

    public void EnableQuizPopup(bool enable)
    {
        gameplayCanvas.QuizPopup.EnableQuizPopup(enable);
    }

    public void ShowQuizPopup(QuizPopupDetail detail)
    {
        gameplayCanvas.QuizPopup.ShowQuizPopup(detail);
    }
    
    public async UniTask ShowMidScreenText(string text)
    {
        gameplayCanvas.SetMidScreenText(text);
        gameplayCanvas.EnableMidScreenText(true);
        await UniTask.WaitForSeconds(midScreenTexTimer);
        gameplayCanvas.EnableMidScreenText(false);
    }
}
