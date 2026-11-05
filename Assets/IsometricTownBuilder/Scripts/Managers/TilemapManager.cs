using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TilemapManager : MonoBehaviour
{
    [field: SerializeField] public bool IsInitialized { get; private set; } = false;
    [field: SerializeField] public bool IsCurrentSpotBuildable { get; private set; } = false;
    [SerializeField] private Tilemap tilemap;
    [SerializeField] private TileBase[] tiles;
    [SerializeField] private BuildableTileData[] furnitureTiles;
    [SerializeField] private BuildableTileData[] mixBuildingTiles;

    [field: SerializeField] public Vector3Int TilePos { get; private set; } = Vector3Int.zero;
    [SerializeField] private List<Vector3Int> allTilePosList = new List<Vector3Int>();
    public List<Vector3Int> SelectedTiles => allTilePosList;
            
    [Space] 
    [SerializeField] private TileBase selectedTile;
    [SerializeField] private bool hasSelected = false;
    [Space] 
    [SerializeField] private Vector3 screenToWorldPos;
    [SerializeField] private Vector3 screenPos;
    [SerializeField] private Vector2 clickPos;

    private Color defaultColor = Color.white;
    private Color buildableColor = Color.greenYellow;
    private Color noneBuildableColor = Color.red;

    private Camera camera;

    private Dictionary<Vector3Int, BuildableTileData> buildableTiles = new Dictionary<Vector3Int, BuildableTileData>();

    private GamePlayManager gamePlayManager => GamePlayManager.Instance;
    private GameplayUIManager gameplayUI => gamePlayManager.GamePlayUI;
    private BuildingObjectSelecter buildObjSelecter => gamePlayManager.BuildingObjSelecter;
    private GameStateManager gameStateManager => gamePlayManager.GameStateManager;

    private PlayerInputHandler playerInput => GamePlayManager.Instance.PlayerInput;
    private BuildingGameState? buildingGameState => gameStateManager?.GetGameStateFromType(GameStateType.Building) as BuildingGameState;

    public async UniTask InitializeAsync()
    {
        await UniTask.WaitUntil(() => playerInput != null
                                      && gamePlayManager != null
                                      && gamePlayManager.BuildingObjSelecter != null
                                      && gameplayUI != null
                                      && gameplayUI.BuildingOptionPanel != null);
        camera = Camera.main;
        SetupBuildableTileDict();
        playerInput.InputPositionUpdated += UpdateTileColorOnHover;
        playerInput.OnResetHolding += OnInputResetHolding;
        
        if (gameplayUI != null && gameplayUI.BuildingOptionPanel != null)
        {
            gameplayUI.BuildingOptionPanel.OnAcceptBuild += OnBuildAccept;
            gameplayUI.BuildingOptionPanel.OnCancelBuild += OnBuildCancel;
        }

        buildObjSelecter.OnSelectedObjFlip += OnFlip;
        
        IsInitialized = true;
    }

    private void OnFlip(bool flipped)
    {
        Debug.Log($"[TileMap] {GetType().Name}.OnFlip -> flipped: {flipped}".LogColor(new Color(1f, 0.7f, 0.61f)));
        TilePos = buildObjSelecter.SelectedObject.OriginTileWhileHolding;
        GetNewBuildSpecTilesAndColorThem(buildObjSelecter.SelectedObject.OriginTileWhileHolding,buildObjSelecter.SelectedObject);
    }

    private void OnBuildCancel()
    {
        Debug.Log($"[TileMap] {GetType().Name}.OnBuildCancel".LogColor(new Color(1f, 0.19f, 0.38f)));
        ResetAllCachedTileColorToDefault();
    }

    private void OnBuildAccept()
    {
        Debug.Log($"[TileMap] {GetType().Name}.OnBuildAccept".LogColor(new Color(0.81f, 1f, 0.36f)));
        ResetAllCachedTileColorToDefault();
    }

    private void OnInputResetHolding()
    {
        if (selectedTile == null)
        {
            return;
        }

        if (TilePos == Vector3.zero)
        {
            return;
        }
        
        ResetAllCachedTileColorToDefault();
    }

    private void ResetAllCachedTileColorToDefault()
    {
        tilemap.SetColor(TilePos, defaultColor);
        foreach (var neighbourPos in allTilePosList)
        {
            tilemap.SetColor(neighbourPos, defaultColor);
        }
        allTilePosList.Clear();
    }

    private void SetupBuildableTileDict()
    {
        var allBuildableTiles = mixBuildingTiles.ToList();
        allBuildableTiles.AddRange(furnitureTiles);
        foreach (var buildableTileData in allBuildableTiles)
        {
            buildableTiles[buildableTileData.TilePosition] = buildableTileData;
        }
    }

    private void UpdateTileColorOnHover(Vector3 screenPos)
    {
        if (gameStateManager == null)
        {
            return;
        }

        if (gameStateManager.CurrentState == null)
        {
            return;
        }

        if (gameStateManager.CurrentState.StateType != GameStateType.Building)
        {
            return;
        }

        if (buildingGameState == null)
        {
            return;
        }

        if (buildingGameState.CurrentBuildState is BuildState.Undefined
            || buildingGameState.CurrentBuildState is BuildState.Selecting
           )
        {
            return;
        }


        if (buildingGameState.CurrentBuildState is BuildState.HoldingSelectedBuilding)
        {
            screenToWorldPos = camera.ScreenToWorldPoint(screenPos);
            TilePos = tilemap.WorldToCell(screenToWorldPos);
            buildObjSelecter.SelectedObject.SetOriginTileWhileHolding(TilePos);    
        }
        
        GetNewBuildSpecTilesAndColorThem(TilePos,buildObjSelecter.SelectedObject);
    }

    private void GetNewBuildSpecTilesAndColorThem(Vector3Int tilePos,BuildingObject selectedBuilding)
    {
        if (selectedBuilding == null)
        {
            return;
        }
        ResetTilePosAndNeighbourToDefaultColor();
        IsCurrentSpotBuildable = IsBuildableSpec(tilePos, selectedBuilding.GetBuildSpecs(), out var tilePosResult);
        allTilePosList = tilePosResult.ToList();
        foreach (var neighbourPos in tilePosResult)
        {
            tilemap.SetTileFlags(neighbourPos, TileFlags.None);
            tilemap.SetColor(neighbourPos, IsCurrentSpotBuildable? buildableColor:noneBuildableColor);
        }
    }

    public bool IsBuildableSpec(Vector3Int origin,IEnumerable<BuildSpec> buildSpecs,out IEnumerable<Vector3Int> tilePosResult)
    {
        bool areBuildable = true;
        var tilePosResultList = new List<Vector3Int>();
        foreach (var buildSpec in buildSpecs)
        {
            var checkTilePos = new Vector3Int(origin.x + buildSpec.X, origin.y + buildSpec.Y, origin.z);
            tilePosResultList.Add(checkTilePos);
            if (!buildableTiles.ContainsKey(checkTilePos) && areBuildable)
            {
                // Debug.LogError($"{GetType().Name}.IsBuildableSpec at pos: {checkTilePos} isn't one of the buildables".LogColor(Color.red));
                areBuildable = false;
            }
        }

        tilePosResult = tilePosResultList;
        return areBuildable;
    }

    public void ResetTilePosAndNeighbourToDefaultColor()
    {
        if (TilePos != Vector3.zero)
        {
            //re-lock color for selected tile 
            tilemap.SetColor(TilePos, defaultColor);
        }
        
        foreach (var neighbourPos in allTilePosList)
        {
            tilemap.SetColor(neighbourPos, defaultColor);
        }
    }
    
    public Vector3 GetTileWorldPosition(Vector3Int tilePosition)
    {
        return tilemap.GetCellCenterWorld(tilePosition);
    }
    

#if UNITY_EDITOR && false
    private void OnValidate()
    {
        GetTileCounts();
    }


    private void GetTileCounts()
    {
        if (tilemap == null)
        {
            return;
        }

        var tileBaseFilterList = new List<TileBase>();
        var mixingBuildingTileList = new List<BuildableTileData>();
        var furnitureTileList = new List<BuildableTileData>();

        BoundsInt.PositionEnumerator tileMapPosition = tilemap.cellBounds.allPositionsWithin;
        foreach (var tilePos in tileMapPosition)
        {
            var tileBase = tilemap.GetTile(tilePos);
            if (tileBase == null)
            {
                continue;
            }

            tileBaseFilterList.Add(tileBase);
            var tileData = new TileData();
            tileBase.GetTileData(tilePos, null, ref tileData);

            if (tileData.sprite == null)
            {
                continue;
            }

            if (!tileData.sprite.name.Contains("dirt"))
            {
                continue;
            }

            var buildableTile = new BuildableTileData()
            {
                TilePosition = tilePos,
                TilebaseObject = tileBase
            };

            if (!tileData.sprite.name.Contains("grass"))
            {
                buildableTile.CellType = CellType.Furniture;
                //mix
                furnitureTileList.Add(buildableTile);
            }
            else
            {
                buildableTile.CellType = CellType.Furniture | CellType.Station;
                //
                mixingBuildingTileList.Add(buildableTile);
            }
        }

        furnitureTiles = furnitureTileList.ToArray();
        mixBuildingTiles = mixingBuildingTileList.ToArray();
        tiles = tileBaseFilterList.ToArray();
    }
#endif
}