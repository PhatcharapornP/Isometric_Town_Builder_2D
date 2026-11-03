using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;

public class TilemapUtils : MonoBehaviour
{
    [SerializeField] private Tilemap tilemap;
    [SerializeField] private BoundsInt bounds;
    [SerializeField] private TileBase[] tiles;
    [SerializeField] private BuildableTileData[] furnitureTiles;
    [SerializeField] private BuildableTileData[] mixBuildingTiles;

    [Space] [SerializeField] private Vector2Int size = new Vector2Int(30, 30);
    [SerializeField] private Vector2Int origin = new Vector2Int(0, 0);
    [Space] [SerializeField] private Vector3Int tilePos;
    [Space] [SerializeField] private TileBase selectedTile;
    [SerializeField] private string tileString;
    [SerializeField] private bool hasSelected = false;
    [Space] [SerializeField] private Vector3 screenToWorldPos;
    [SerializeField] private Vector3 screenPos;
    [SerializeField] private Vector2 clickPos;

    private Color defaultColor = Color.white;
    private Color selectedColor = Color.red;

    private Camera camera;

    private Dictionary<Vector3Int, BuildableTileData> buildableTiles = new Dictionary<Vector3Int, BuildableTileData>();

    private void Start()
    {
        camera = Camera.main;
        SetupBuildableTileDict();
    }


    private void Update()
    {
        DetectTouch();
        DetectMouseClick();
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


    private void DetectTouch()
    {
        var touchscreen = Touchscreen.current;
        if (touchscreen == null)
        {
            return;
        }

        var hasTouched = touchscreen.touches.Count > 0 && touchscreen.touches[0].press.wasPressedThisFrame;
        // bool androidTouched = Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began;
        if (hasTouched /*androidTouched*/)
        {
            // Touch touch = Input.touches[0];
            //camera z is -10
            // screenPos = new Vector3(touch.position.x, touch.position.y, 10f);

            var screenPosition = touchscreen.touches[0].position.ReadValue();
            screenPos = new Vector3(screenPosition.x, screenPosition.y, 10f);
            HandleOnClick();
        }
    }

    private void DetectMouseClick()
    {
        if (Mouse.current == null)
        {
            return;
        }

        if (Mouse.current.leftButton.wasPressedThisFrame)
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

        //by default this set flags as lock color, set as none and added selected color?


        //update selectedTile
        selectedTile = tilemap.GetTile(tilePos);
        if (selectedTile != null)
        {
            var data = new TileData();
            selectedTile.GetTileData(tilePos, null, ref data);
            Debug.Log($"sprite: {data.sprite.name}" +
                      $"\n color: {data.color}" +
                      $"\n flags: {data.flags}", data.gameObject);

            // if (data.sprite.name.Contains("dirt"))
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

#if false

    private void OnValidate()
    {
        GetTileCounts();
    }

#endif

    private bool InBounds(Vector3Int cell)
    {
        return cell.x >= origin.x
               && cell.x < origin.x + size.x
               && cell.y >= origin.y
               && cell.y < origin.y + size.y;
    }

    private bool TrySetTile(Vector3Int cell, TileBase tileBase)
    {
        if (!InBounds(cell))
        {
            return false;
        }

        tilemap.SetTile(cell, tileBase);
        return true;
    }

    private void GetTileCounts()
    {
        if (tilemap == null)
        {
            return;
        }

        bounds = tilemap.cellBounds;
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
}