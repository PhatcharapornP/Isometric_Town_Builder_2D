using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.Tilemaps;

public class TilemapManager : MonoBehaviour
{
    [field: SerializeField] public bool IsInitialized { get; private set; } = false;
    [SerializeField] private Tilemap tilemap;
    [SerializeField] private TileBase[] tiles;
    [SerializeField] private BuildableTileData[] furnitureTiles;
    [SerializeField] private BuildableTileData[] mixBuildingTiles;

    [Space] 
    [SerializeField] private Vector3Int tilePos;
    [SerializeField] private List<Vector3Int> allTilePosList = new List<Vector3Int>();
    [Space] 
    [SerializeField] private TileBase selectedTile;
    [SerializeField] private List<TileBase> selectedTiles = new List<TileBase>();
    [SerializeField] private bool hasSelected = false;
    [Space] 
    [SerializeField] private Vector3 screenToWorldPos;
    [SerializeField] private Vector3 screenPos;
    [SerializeField] private Vector2 clickPos;

    private Color defaultColor = Color.white;
    private Color buildableColor = Color.greenYellow;
    private Color noneBuildableColor = Color.red;
    // private Color neighbourPlusOneX = Color.orangeRed;
    // private Color neighbourMinusOneX = new(0.2f, 0.95f, 1f);
    // private Color neighbourPlusOneY = new(0.91f, 0.31f, 1f);
    // private Color neighbourMinusOneY = new(0.71f, 1f, 0.2f);

    private Camera camera;

    private Dictionary<Vector3Int, BuildableTileData> buildableTiles = new Dictionary<Vector3Int, BuildableTileData>();

    private GamePlayManager gamePlayManager => GamePlayManager.Instance;
    private BuildingObjectSelecter buildObjSelecter => gamePlayManager.BuildingObjSelecter;
    private GameStateManager gameStateManager => gamePlayManager.GameStateManager;

    private PlayerInputHandler playerInput => GamePlayManager.Instance.PlayerInput;
    private BuildingGameState? buildingGameState => gameStateManager?.GetGameStateFromType(GameStateType.Building) as BuildingGameState;
    

    private void Start()
    {
        Initialize().Forget();
        
    }

    private async UniTask Initialize()
    {
        await UniTask.WaitUntil(() => playerInput != null
                                      && gamePlayManager != null
                                      && gamePlayManager.BuildingObjSelecter != null
                                      && gamePlayManager.BuildingObjSelecter != null);
        camera = Camera.main;
        SetupBuildableTileDict();
        IsInitialized = true;
        playerInput.InputPositionUpdated += UpdateTileColorOnHover;
        playerInput.OnResetHolding += OnInputResetHolding;
        //TODO: Sup to on accept and cancel build
    }

    private void OnInputResetHolding()
    {
        if (selectedTile == null)
        {
            return;
        }

        if (tilePos == Vector3.zero)
        {
            return;
        }
        
        ResetAllCachedTileColorToDefault();
    }

    private void ResetAllCachedTileColorToDefault()
    {
        tilemap.SetColor(tilePos, defaultColor);
        foreach (var neighbourPos in allTilePosList)
        {
            tilemap.SetColor(neighbourPos, defaultColor);
        }
        allTilePosList.Clear();
        selectedTiles.Clear();
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

        if (gameStateManager.CurrentState.StateType is not GameStateType.Building)
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
        
        ResetTilePosAndNeighbourToDefaultColor();
        
        screenToWorldPos = camera.ScreenToWorldPoint(screenPos);
        var newTilePos = tilemap.WorldToCell(screenToWorldPos);
        
        var selectedBuilding = buildObjSelecter.SelectedObject;
        if (selectedBuilding == null)
        {
            return;
        }

        bool isBuildSpecPass = IsBuildableSpec(newTilePos, selectedBuilding.GetBuildSpecs(), out var tilePosResult);
        allTilePosList = tilePosResult.ToList();
        foreach (var neighbourPos in tilePosResult)
        {
            tilemap.SetTileFlags(neighbourPos, TileFlags.None);
            tilemap.SetColor(neighbourPos, isBuildSpecPass? buildableColor:noneBuildableColor);
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
                Debug.LogError($"{GetType().Name}.IsBuildableSpec at pos: {checkTilePos} isn't one of the buildables".LogColor(Color.red));
                areBuildable = false;
            }
        }

        tilePosResult = tilePosResultList;
        return areBuildable;
    }

    public void ResetTilePosAndNeighbourToDefaultColor()
    {
        if (tilePos != Vector3.zero)
        {
            //re-lock color for selected tile 
            tilemap.SetColor(tilePos, defaultColor);
        }
        
        foreach (var neighbourPos in allTilePosList)
        {
            tilemap.SetColor(neighbourPos, defaultColor);
        }
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