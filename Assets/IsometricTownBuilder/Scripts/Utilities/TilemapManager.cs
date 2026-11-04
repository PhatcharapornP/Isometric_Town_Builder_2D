using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using UnityEngine;
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
    [SerializeField] private List<Vector3Int> neighbourTilePosList = new List<Vector3Int>();
    [Space] 
    [SerializeField] private TileBase selectedTile;
    [SerializeField] private List<TileBase> selectedNeighbourTiles = new List<TileBase>();
    [SerializeField] private bool hasSelected = false;
    [Space] 
    [SerializeField] private Vector3 screenToWorldPos;
    [SerializeField] private Vector3 screenPos;
    [SerializeField] private Vector2 clickPos;

    private Color defaultColor = Color.white;
    private Color buildableColor = Color.green;
    private Color noneBuildableColor = Color.red;
    private Color neighbour = Color.orangeRed;

    private Camera camera;

    private Dictionary<Vector3Int, BuildableTileData> buildableTiles = new Dictionary<Vector3Int, BuildableTileData>();

    private GamePlayManager gamePlayManager => GamePlayManager.Instance;
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
        
        tilemap.SetColor(tilePos, defaultColor);
        neighbourTilePosList.Clear();
        selectedNeighbourTiles.Clear();
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
        
        screenToWorldPos = camera.ScreenToWorldPoint(screenPos);
        var newTilePos = tilemap.WorldToCell(screenToWorldPos);

        ResetTilePosAndNeighbourToDefaultColor();

        //update tilePos
        tilePos = newTilePos;
        
        neighbourTilePosList.Clear();
        selectedNeighbourTiles.Clear();
        //TODO: find neighbour on x and -x
        {
            var xPos = new  Vector3Int(tilePos.x+1, tilePos.y, tilePos.z);
            var minXPos = new  Vector3Int(tilePos.x-1, tilePos.y, tilePos.z);
            var neighbourXPos = tilemap.GetTile(xPos);
            var neighbourMinXPos = tilemap.GetTile(minXPos);
            if (neighbourXPos != null&& buildableTiles.ContainsKey(xPos))
            {
                neighbourTilePosList.Add(xPos);
                selectedNeighbourTiles.Add(neighbourXPos);
            }
        
            if (neighbourMinXPos != null&& buildableTiles.ContainsKey(minXPos))
            {
                neighbourTilePosList.Add(minXPos);
                selectedNeighbourTiles.Add(neighbourMinXPos);
            }    
        }
        
        //TODO: find neighbour on y and -y
        {
            var yPos = new  Vector3Int(tilePos.x, tilePos.y+1, tilePos.z);
            var minYPos = new  Vector3Int(tilePos.x, tilePos.y-1, tilePos.z);
            var neighbourYPos = tilemap.GetTile(yPos);
            var neighbourMinYPos = tilemap.GetTile(minYPos);
            if (neighbourYPos != null && buildableTiles.ContainsKey(yPos))
            {
                neighbourTilePosList.Add(yPos);
                selectedNeighbourTiles.Add(neighbourYPos);
            }
        
            if (neighbourMinYPos != null && buildableTiles.ContainsKey(minYPos))
            {
                neighbourTilePosList.Add(minYPos);
                selectedNeighbourTiles.Add(neighbourMinYPos);
            }    
        }

        //update selectedTile
        selectedTile = tilemap.GetTile(tilePos);
        if (selectedTile != null)
        {
            var data = new TileData();
            selectedTile.GetTileData(tilePos, null, ref data);

            if (buildableTiles.ContainsKey(tilePos))
            {
                tilemap.SetTileFlags(tilePos, TileFlags.None);

                //TODO: check if this tile has already something build onto it.
                bool isBuildable = true;

                if (isBuildable)
                {
                    tilemap.SetColor(tilePos, buildableColor);
                    foreach (var neighbourPos in neighbourTilePosList) //todo: need to make the spec of each building and search for the next tile using the spec
                    {
                        tilemap.SetColor(neighbourPos, neighbour); 
                    }
                }
                else
                {
                    tilemap.SetColor(tilePos, noneBuildableColor);
                    foreach (var neighbourPos in neighbourTilePosList)
                    {
                        tilemap.SetColor(neighbourPos, noneBuildableColor);
                    }
                }
                
                // Debug.Log(
                //     $"{GetType().Name}.HandleOnClick() found buildableTile at {tilePos} | cell type: {buildableTiles[tilePos].CellType}"
                //         .LogColor(Color.greenYellow));
            }
            else
            {
                // Debug.Log(
                //     $"{GetType().Name}.HandleOnClick() none buildable tiles at {tilePos}".LogColor(Color.chocolate));
            }
        }
    }

    public void ResetTilePosAndNeighbourToDefaultColor()
    {
        if (tilePos != Vector3.zero)
        {
            //re-lock color for selected tile 
            tilemap.SetColor(tilePos, defaultColor);

            foreach (var neighbourPos in neighbourTilePosList)
            {
                tilemap.SetColor(neighbourPos, defaultColor);
            }
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