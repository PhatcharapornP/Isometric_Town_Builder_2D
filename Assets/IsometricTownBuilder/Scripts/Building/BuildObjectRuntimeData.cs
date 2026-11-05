#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


[Serializable]
public struct TilePosition
{
    public int X;
    public int Y;
    public int Z;

    public TilePosition(int x, int y, int z)
    {
        X = x;
        Y = y;
        Z = z;
    }
}

[Serializable]
public class BuildObjectSaveData
{
    public int BuildingId;
    public BuildObjectRuntimeData  BuildData;

    public BuildObjectSaveData(int buildingId, BuildObjectRuntimeData buildData)
    {
        BuildingId = buildingId;
        BuildData = buildData;
    }
}

[Serializable]
public class BuildObjectRuntimeData
{
    [field: SerializeField] public TilePosition OriginTilePosition { get; private set; }
    [field: SerializeField] public List<TilePosition> BuiltOnTiles { get; private set; }
    [field: SerializeField] public bool FlipValue { get; private set; }

    public BuildObjectRuntimeData(
        Vector3Int originTilePosition,
        List<Vector3Int> builtOnTiles,
        bool flipValue
    )
    {
        OriginTilePosition = new TilePosition(originTilePosition.x, originTilePosition.y, originTilePosition.z);
        if (builtOnTiles != null)
        {
            BuiltOnTiles = GetBuildOnTiles(builtOnTiles).ToList();    
        }
        
        FlipValue = flipValue;
    }

    public Vector3Int GetVector3OriginTile()
    {
        return new Vector3Int(OriginTilePosition.X, OriginTilePosition.Y, OriginTilePosition.Z);
    }

    public void SetOriginalTilePosition(Vector3Int originTilePosition)
    {
        OriginTilePosition = new TilePosition(originTilePosition.x, originTilePosition.y, originTilePosition.z);
    }

    public void SetBuiltOnTiles(List<Vector3Int> builtOnTiles)
    {
        foreach (var position in builtOnTiles)
        {
            BuiltOnTiles.Add(new TilePosition(position.x, position.y, position.z));    
        }
    }

    private IEnumerable<TilePosition> GetBuildOnTiles(List<Vector3Int> builtOnTiles)
    {
        foreach (var position in builtOnTiles)
        {
            yield return new TilePosition(position.x, position.y, position.z);
        }
    }

    public IEnumerable<Vector3Int> GetBuiltOnTiles()
    {
        foreach (var position in BuiltOnTiles)
        {
            yield return new Vector3Int(position.X, position.Y, position.Z);
        }
    }
   
    public void SetFlipSpriteRenderer(bool flipValue)
    {
        FlipValue = flipValue;
    }
}