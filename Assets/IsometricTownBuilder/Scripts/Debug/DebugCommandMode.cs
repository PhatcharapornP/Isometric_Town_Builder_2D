#nullable enable
using System;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;

public class DebugCommandMode : MonoBehaviour
{
    private GameStateManager gameStateManager;
    private SaveLoadManager saveLoadManager;
    [SerializeField] private GenericButton idleState;
    [SerializeField] private GenericButton buildingState;
    [SerializeField] private GenericButton quizState;
    [SerializeField] private GenericButton mixingState;
    [SerializeField] private GenericButton savingState;
    [SerializeField] private GenericButton loadingState;
    [SerializeField] private GenericButton loadingSaveState;

    [SerializeField] private GenericButton openDebugButton;
    [SerializeField] private TextMeshProUGUI debugText;
    [SerializeField] private GameObject debugPanel;
    private bool isDebugPanelShowing => debugPanel.activeInHierarchy;
    private bool isInitialized = false;

//Assets/Resources/GameManager.prefab
    private const string GAME_MANAGER_PATH = "GameManager";

    private void Start()
    {
        if (GameManager.Instance == null)
        {
            //TODO: Load GameManager
           var prefab = Resources.Load<GameObject>(GAME_MANAGER_PATH);
           var gameManager = Instantiate(prefab);
        }

        openDebugButton.ClickedAction += OnClickDebugButton;

        InitializeDebugAsync().Forget();

    }

    private void Update()
    {
        if (!isDebugPanelShowing)
        {
            return;
        }

        UpdateDebugString();
    }

    private void UpdateDebugString()
    {
        string gameStateDebug = "";
        string debugInitialized = $" isInitialized: {isInitialized}";
        if (gameStateManager != null)
        {
            gameStateDebug = $" currentGameState: {gameStateManager.CurrentState}";
        }
        else
        {
            gameStateDebug = $" currentGameState: null";
        }

        debugText.SetText(debugInitialized + $" " +
                          $" \n  {gameStateDebug}");
    }

    private void OnClickDebugButton()
    {
        debugPanel.SetActive(!isDebugPanelShowing);
    }

    private async UniTask InitializeDebugAsync()
    {
        await UniTask.WaitUntil(() => GamePlayManager.Instance != null
                                      && GamePlayManager.Instance.GameStateManager != null
                                      && GamePlayManager.Instance.GameStateManager.IsInitialized
                                      && GameManager.Instance != null);
        gameStateManager = GamePlayManager.Instance.GameStateManager;
        saveLoadManager = GameManager.Instance.SaveLoadManager;
        SetupIdleDebug();
        SetupBuildingDebug();
        SetupQuizDebug();
        SetupMixingDebug();
        SetupSavingDebug();
        SetupLoadingState();
        SetupLoadingSaveState();
        isInitialized = true;
    }

    private void SetupIdleDebug()
    {
        idleState.ClickedAction += GoToIdleState;
    }

    private void GoToIdleState()
    {
        gameStateManager.GoToState(GameStateType.Idle);
    }

    private void SetupBuildingDebug()
    {
        buildingState.ClickedAction += GoToBuildingState;
    }

    private void GoToBuildingState()
    {
        //for this to work, debug might have to give this state the target building item (furniture / station)
        gameStateManager.GoToState(GameStateType.Building);
    }

    private void SetupQuizDebug()
    {
        quizState.ClickedAction += GoToQuizState;
    }

    private void GoToQuizState()
    {
        //just pull up the quiz panel, load quiz in and checking answer
        gameStateManager.GoToState(GameStateType.Quiz);
    }

    private void SetupMixingDebug()
    {
        mixingState.ClickedAction += GoToMixingState;
    }

    private void GoToMixingState()
    {
        gameStateManager.GoToState(GameStateType.Mixing);
    }

    private void SetupSavingDebug()
    {
        savingState.ClickedAction += SaveGame;
    }

    private void SaveGame()
    {
        saveLoadManager.SaveGame();
    }

    private void SetupLoadingState()
    {
        loadingState.ClickedAction += ReloadGame;
    }

    private void ReloadGame()
    {
        //TODO: reload the gameplay scene
    }

    private void SetupLoadingSaveState()
    {
        loadingSaveState.ClickedAction += LoadGameSave;
    }

    private void LoadGameSave()
    {
        saveLoadManager.LoadGameSave();
    }
}