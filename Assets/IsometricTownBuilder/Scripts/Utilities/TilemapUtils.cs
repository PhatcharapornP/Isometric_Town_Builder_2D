using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using UnityEngine.Serialization;
using UnityEngine.Tilemaps;
using TouchPhase = UnityEngine.TouchPhase;

public class TilemapUtils : MonoBehaviour
{
    [SerializeField] private Tilemap tilemap;
    [SerializeField] private BoundsInt bounds;
    [SerializeField] private TileBase[] tiles;

    [Space] [SerializeField] private Vector2Int size = new Vector2Int(30, 30);
    [SerializeField] private Vector2Int origin = new Vector2Int(0, 0);
    [Space] 
    [SerializeField] private Vector3Int tilePos;

    [SerializeField] private TileBase selectedTile;
    [SerializeField] private bool hasSelected = false;

    [SerializeField] private Vector3 screenToWorldPos;
    [SerializeField] private Vector3 screenPos;
    [SerializeField] private Vector2 clickPos;

    private Color defaultColor = Color.white;
    private Color selectedColor = Color.red;

    private Camera camera;

    private void Start()
    {
        camera = Camera.main;
    }

    private void Update()
    {
        DetectTouch();
        DetectMouseClick();
    }

    private void DetectTouch()
    {
        bool androidTouched = Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began;
        if (androidTouched)
        {
            Touch touch = Input.touches[0];
            //camera z is -10
            screenPos = new Vector3(touch.position.x, touch.position.y, 10f);
            HandleOnClick();
        }
    }

    private void DetectMouseClick()
    {
        if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
        {
            clickPos = Mouse.current.position.ReadValue();
            //camera z is -10
            screenPos = new Vector3(clickPos.x, clickPos.y, 10f);
            HandleOnClick();
        }
    }

    private void HandleOnClick(
#if UNITY_EDITOR
        [CallerMemberName] string caller = ""
#endif
    )
    {
        screenToWorldPos = camera.ScreenToWorldPoint(screenPos);
        var newTilePos = tilemap.WorldToCell(screenToWorldPos);
#if UNITY_EDITOR
        Debug.Log(
            $"clicked at world pos {screenToWorldPos} from caller: {caller}".LogColor(new Color(0.42f, 0.76f, 1f)));
#endif

        if (tilePos != Vector3.zero)
        {
            //re-lock color for selected tile 
            tilemap.SetColor(tilePos, defaultColor);
        }

        //update tilePos
        tilePos = newTilePos;

        //by default this set flags as lock color, set as none and added selected color?
        tilemap.SetTileFlags(tilePos, TileFlags.None);
        tilemap.SetColor(tilePos, selectedColor);
        
        //update selectedTile
        selectedTile = tilemap.GetTile(tilePos);
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
        tiles = tilemap.GetTilesBlock(bounds);
        Dictionary<TileBase, int> counts = new Dictionary<TileBase, int>();

        foreach (var tileBase in tiles)
        {
            if (tileBase != null)
            {
                if (counts.ContainsKey(tileBase))
                {
                    counts[tileBase]++;
                }
                else
                {
                    counts[tileBase] = 1;
                }
            }
        }

        string stringDebug = Newtonsoft.Json.JsonConvert.SerializeObject(counts);
        Debug.Log($"{GetType().Name} -> stringDebug: {stringDebug}".LogColor(new Color(1f, 0.52f, 0.89f)));
    }

    private void OnDrawGizmos()
    {
    }
}