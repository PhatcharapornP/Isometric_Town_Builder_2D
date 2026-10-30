using System;
using UnityEngine;
using UnityEngine.Serialization;

[Serializable]
public class CellData
{
    public CellOccupyType OccupyType = CellOccupyType.Empty;
    public int OccupiedObjectUId;
    [FormerlySerializedAs("placeableConfigData")] [FormerlySerializedAs("CellConfigData")] public CellConfigData cellConfigData;

    public CellData(
        CellOccupyType occupyType,
        int occupiedObjectUId,
        CellConfigData configData
    )
    {
        OccupyType = occupyType;
        cellConfigData = configData;
    }
}

[Serializable]
public class CellConfigData
{
    public CellType CellType;
    public int X;
    public int Y;

    public CellConfigData(CellType cellType, int x, int y)
    {
        CellType = cellType;
        X = x;
        Y = y;
    }
}

[Serializable]
public class PlaceableObjectData
{
    public int ObjectUid;
    public Vector2Int OriginateCellPosition;
    public PlaceableObjectConfigData ConfigData;
}

[Serializable]
public class PlaceableObjectConfigData
{
    public PlaceableObjectType PlaceableObjectType = PlaceableObjectType.Undefined;
    public int DimensionX = 1;
    public int DimensionY = 1;
}

[Flags]
public enum CellType : byte
{
    Furniture = 1 << 1,
    Station = 1 << 2,
}

public enum CellOccupyType
{
    Undefined = 0,
    Occupied = 1,
    Empty = 2,
}

public enum PlaceableObjectType
{
    Undefined = 0,
    Furniture = 1,
    Station = 2,
}

