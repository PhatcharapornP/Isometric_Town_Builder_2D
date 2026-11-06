using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;

public class BuildingButton : GenericButton ,IPointerDownHandler
{
    [field:SerializeField] public int BuildingId { get; private set; }
    private GameStateManager gameStateManager => GamePlayManager.Instance.GameStateManager;
    private PlayerInputHandler playerInput => GamePlayManager.Instance.PlayerInput;
    private BuildingObjectSelecter buildObjectSelecter => GamePlayManager.Instance.BuildingObjSelecter;
    private BuildingCatalogConfig buildingCatalogConfig;
    private BuildingGameState buildingGameState => gameStateManager?.GetGameStateFromType(GameStateType.Building) as BuildingGameState;
    private GameplayUIManager gameplayUIManager => GamePlayManager.Instance.GamePlayUI;  
    private Camera targetCamera => Camera.main;

    private bool isInitialized = false;

    public async UniTask Initialize()
    {
        //TODO: want to refactor this dependencies later after things are settled
        GameConfigContainer gameConfigContainer = GamePlayManager.Instance.GameConfigContainer;
        await UniTask.WaitUntil(() => playerInput != null
                                      && gameConfigContainer != null
                                      && gameStateManager != null
                                      && buildObjectSelecter != null
                                      && buildingGameState != null
                                      && gameplayUIManager != null
        );

        buildingCatalogConfig = gameConfigContainer.BuildingCatalogConfig;
        isInitialized = true;
    }

    private void SpawnBuildableObjectAtMousePosition()
    {
        if (!isInitialized)
        {
            return;
        }

        var buildingData = buildingCatalogConfig.GetBuildingCatalogData(BuildingId);
        if (!buildingData.HasValue)
        {
            Debug.LogError($"[BuildButton] {GetType().Name}.SpawnBuildableObjectAtMousePosition() -> couldn't get building from id: {BuildingId}".LogColor(Color.red));
            return;
        }

        if (buildingData.Value.buildingPrefab == null)
        {
            Debug.LogError($"[BuildButton] {GetType().Name}.SpawnBuildableObjectAtMousePosition() -> building from id: {BuildingId} doesn't has a prefab".LogColor(Color.red));
            return;
        }
        
        var buildingObj = Instantiate(buildingData.Value.buildingPrefab);
        var screenToWorldPos = targetCamera.ScreenToWorldPoint(new Vector3(playerInput.ScreenPos.x, playerInput.ScreenPos.y, 0f));
        buildingObj.SetPosition(screenToWorldPos);
        buildObjectSelecter.SetSelectingObject(buildingObj);
        gameStateManager.GoToState(GameStateType.Building);
        buildingGameState.ChangeBuildState(BuildState.HoldingSelectedBuilding);
        gameplayUIManager.HideBuildingCatalogPanel();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        SpawnBuildableObjectAtMousePosition();
    }
}
