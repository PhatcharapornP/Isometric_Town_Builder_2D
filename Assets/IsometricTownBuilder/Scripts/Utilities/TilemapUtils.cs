using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;
using UnityEngine.Tilemaps;

public class TilemapUtils : MonoBehaviour
{
    [SerializeField] private Tilemap tilemap;
    [SerializeField] private TileBase[] tiles;
    [SerializeField] private BuildableTileData[] furnitureTiles;
    [SerializeField] private BuildableTileData[] mixBuildingTiles;
    
    [Space]
    [SerializeField] private Vector3Int tilePos;
    [Space]
    [SerializeField] private TileBase selectedTile;
    [SerializeField] private bool hasSelected = false;
    [Space] 
    [SerializeField] private Vector3 screenToWorldPos;
    [SerializeField] private Vector3 screenPos;
    [SerializeField] private Vector2 clickPos;

    private Color defaultColor = Color.white;
    private Color selectedColor = Color.red;

    private Camera camera;

    private Dictionary<Vector3Int, BuildableTileData> buildableTiles = new Dictionary<Vector3Int, BuildableTileData>();

    [Space] 
    [SerializeField] private BuildingObject debugFurniture1Tile;
    [SerializeField] private BuildingObject debugFurniture2Tiles;
    [SerializeField] private bool testing1Tile = true;

    private GamePlayManager gamePlayManager => GamePlayManager.Instance;
    private GameStateManager gameStateManager => gamePlayManager.GameStateManager;

    private void Start()
    {
        camera = Camera.main;
        SetupBuildableTileDict();
    }


    private void Update()
    {
        //TODO: Detect game state is in build state
        if (gamePlayManager == null && gameStateManager == null)
        {
            return;
        }

        if (gameStateManager.CurrentState is not BuildingGameState)
        {
            return;
        }
        
        DetectTouch(false);
        DetectMouseClick(false);

        
        //TODO: Move target building with the moving screen pos
        {
            screenToWorldPos = camera.ScreenToWorldPoint(new Vector3(screenPos.x, screenPos.y, 0f));
            if (testing1Tile)
            {
                debugFurniture1Tile.SetPosition(screenToWorldPos);
            }
            else
            {
                debugFurniture2Tiles.SetPosition(screenToWorldPos);
            }
        }
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


    private void DetectTouch(bool detectTouch)
    {
        var touchscreen = Touchscreen.current;
        if (touchscreen == null)
        {
            return;
        }

        var hasTouched = touchscreen.touches.Count > 0 && touchscreen.touches[0].press.wasPressedThisFrame;
        if (!detectTouch)
        {
            hasTouched = true;
        }
        // bool androidTouched = Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began;
        if (hasTouched /*androidTouched*/)
        {
            // Touch touch = Input.touches[0];
            //camera z is -10
            // screenPos = new Vector3(touch.position.x, touch.position.y, 10f);

            clickPos = touchscreen.touches[0].position.ReadValue();
            screenPos = new Vector3(clickPos.x, clickPos.y, 10f);
            HandleOnClick();
        }
    }

    private void DetectMouseClick(bool detectClick)
    {
        if (Mouse.current == null)
        {
            return;
        }

        bool hasClicked = Mouse.current.leftButton.wasPressedThisFrame;

        if (!detectClick)
        {
            hasClicked = true;
        }
        
        if (hasClicked)
        {
            clickPos = Mouse.current.position.ReadValue();
            //camera z is -10
            screenPos = new Vector3(clickPos.x, clickPos.y, 10f);
            HandleOnClick();
        }
    }

    private void HandleOnClick()
    {
        screenToWorldPos = camera.ScreenToWorldPoint(screenPos);
        var newTilePos = tilemap.WorldToCell(screenToWorldPos);

        if (tilePos != Vector3.zero)
        {
            //re-lock color for selected tile 
            tilemap.SetColor(tilePos, defaultColor);
        }

        //update tilePos
        tilePos = newTilePos;

        //update selectedTile
        selectedTile = tilemap.GetTile(tilePos);
        if (selectedTile != null)
        {
            var data = new TileData();
            selectedTile.GetTileData(tilePos, null, ref data);

            if (buildableTiles.ContainsKey(tilePos))
            {
                tilemap.SetTileFlags(tilePos, TileFlags.None);
                tilemap.SetColor(tilePos, selectedColor);
                Debug.Log(
                    $"{GetType().Name}.HandleOnClick() found buildableTile at {tilePos} | cell type: {buildableTiles[tilePos].CellType}"
                        .LogColor(Color.greenYellow));
            }
            else
            {
                Debug.Log(
                    $"{GetType().Name}.HandleOnClick() none buildable tiles at {tilePos}".LogColor(Color.chocolate));
            }
        }
    }

#if true
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