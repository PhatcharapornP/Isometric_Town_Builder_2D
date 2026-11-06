#nullable enable
using System.Threading;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

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
    [SerializeField] private GenericButton deletePlayerPrefs;
    [SerializeField] private GenericButton reloadGame;

    [SerializeField] private GenericButton openDebugButton;
    [SerializeField] private TextMeshProUGUI debugText;
    [SerializeField] private GameObject debugPanel;
    private bool isDebugPanelShowing => debugPanel.activeInHierarchy;
    private bool isInitialized = false;

    private const string GAME_MANAGER_PATH = "GameManager";

    private void Start()
    {
        //This doesn't needed if navigated from 'Main' scene
        if (GameManager.Instance == null)
        {
            //TODO: Load GameManager
           var gameManagerPrefab = Resources.Load<GameObject>(GAME_MANAGER_PATH);
           Instantiate(gameManagerPrefab);
           
        }

        openDebugButton.AddToClickAction(OnClickDebugButton);

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
        string buildStateDebug = "";
        string debugInitialized = $" isInitialized: {isInitialized}";
        string inputDebug = $" InputState: {GamePlayManager.Instance.PlayerInput.InputState}";
        string screenPos = $" ScreenPos: {GamePlayManager.Instance.PlayerInput.ScreenPos}";
        string clickPos = $" ClickPos: {GamePlayManager.Instance.PlayerInput.ClickPos}";
        string touchNull = $" touch null?: {Touchscreen.current == null}";
        string mouseNull = $" mouse null?: {Mouse.current == null}";
        if (gameStateManager != null)
        {
            gameStateDebug = $" currentGameState: {gameStateManager.CurrentState}";
            var buildingGameState = gameStateManager?.GetGameStateFromType(GameStateType.Building) as BuildingGameState;
            buildStateDebug = $"buildState: {buildingGameState.CurrentBuildState}";
        }
        else
        {
            gameStateDebug = $" currentGameState: null";
            buildStateDebug =  $" buildState: ...";
        }
        

        debugText.SetText(debugInitialized + $" " + 
                          $" \n {gameStateDebug}" + 
                          $" \n {buildStateDebug}" +
                          $" \n {inputDebug}" +
                          $" \n {screenPos}" +
                          $" \n {clickPos}" +
                          $" \n {touchNull}" + 
                          $" \n {mouseNull}"
                          );
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
        SetupDeletePlayerPrefs();
        SetupReloadGame();
        isInitialized = true;
    }

    private void SetupDeletePlayerPrefs()
    {
        deletePlayerPrefs.AddToClickAction(DeletePlayerPrefs);
    }

    private void DeletePlayerPrefs()
    {
        PlayerPrefs.DeleteAll();
        PlayerPrefs.DeleteKey(SaveKeyGlobal.QuizSaveKey);
        PlayerPrefs.DeleteKey(SaveKeyGlobal.BuildObjectSaveKey);
        PlayerPrefs.Save();
    }

    private void SetupIdleDebug()
    {
        idleState.AddToClickAction(GoToIdleState);
    }

    private void GoToIdleState()
    {
        gameStateManager.GoToState(GameStateType.Idle);
    }

    private void SetupBuildingDebug()
    {
        buildingState.AddToClickAction(GoToBuildingState);
    }

    private void GoToBuildingState()
    {
        //for this to work, debug might have to give this state the target building item (furniture / station)
        gameStateManager.GoToState(GameStateType.Building);
    }

    private void SetupQuizDebug()
    {
        quizState.AddToClickAction(GoToQuizState);
    }

    private void GoToQuizState()
    {
        //just pull up the quiz panel, load quiz in and checking answer
        gameStateManager.GoToState(GameStateType.Quiz);
    }

    private void SetupMixingDebug()
    {
        mixingState.AddToClickAction(GoToMixingState);
    }

    private void GoToMixingState()
    {
        gameStateManager.GoToState(GameStateType.Mixing);
    }

    private void SetupSavingDebug()
    {
        savingState.AddToClickAction(SaveGame);
    }

    private void SaveGame()
    {
        saveLoadManager.SaveGame();
    }

    private void SetupLoadingState()
    {
    }

    private void SetupReloadGame()
    {
        reloadGame.AddToOnClickAsync(GoToMain);
    }

    private void OnDestroy()
    {
        reloadGame.RemoveFromOnClickAsync(GoToMain);
    }

    private async UniTask GoToMain(CancellationToken ct)
    {
        LoadingCanvas.Instance?.FullScreenLoading.ShowLoading();
        await LoadSceneUtil.GoToSceneAsync(
            targetSceneName: SceneNameKey.MainMenuSceneName,
            unloadSceneName: SceneNameKey.GameplaySceneName
        ).AttachExternalCancellation(ct);
    }

    private void SetupLoadingSaveState()
    {
        loadingSaveState.AddToClickAction(LoadGameSave);
    }

    private void LoadGameSave()
    {
        saveLoadManager.LoadGameSave();
    }
}