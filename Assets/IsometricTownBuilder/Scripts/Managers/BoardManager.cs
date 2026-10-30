#nullable enable
using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class BoardManager 
{
    [SerializeField] private GetBuildPositionUtil  getBuildPositionUtil = new GetBuildPositionUtil();
    
    private Dictionary<Vector2Int,CellData> boardCellDict = new Dictionary<Vector2Int,CellData>();

    [field:SerializeField] public bool IsInitialized { get; private set; } = false;
    [field:SerializeField] public bool HasUpdatedFromSave { get; private set; } = false;

    public void SetupRuntimeBoardFromConfig(BoardConfig boardConfig)
    {
        boardCellDict.Clear();
        foreach (var cellConfigData in boardConfig.CellConfigDataList)
        {
            var newCellData = new CellData(CellOccupyType.Undefined,0,cellConfigData);
            var cellPos = new Vector2Int(newCellData.cellConfigData.X, newCellData.cellConfigData.Y);
            //if (!boardCellDict.ContainsKey(cellPos))
            {
                boardCellDict[cellPos] = newCellData;
            }
            
        }

        IsInitialized = true;
    }

    public void FillInRuntimeCellDataFromSave(IEnumerable<CellData> savedCellDataList)
    {
        foreach (var cellData in savedCellDataList)
        {
            var cellPos = new Vector2Int(cellData.cellConfigData.X, cellData.cellConfigData.Y);
            boardCellDict[cellPos] = cellData;

        }
    }

    private IEnumerable<CellData> GetUpdatedRuntimeCell()
    {
        List<CellData> cellDataList = new List<CellData>();
        foreach (var cellItem in boardCellDict)
        {
            if (cellItem.Value.OccupyType is not CellOccupyType.Undefined)
            {
                cellDataList.Add(cellItem.Value);
            }
        }

        return cellDataList;
    }

    public bool TryPlaceCellAt(
        Vector2Int position,
        PlaceableObjectConfigData objectSpec,
        out Vector2Int[]? cellPositionRequire
    )
    {
        if (objectSpec.PlaceableObjectType is PlaceableObjectType.Undefined)
        {
            //TODO: reject
            Debug.LogError($"{GetType().Name}.TryPlaceCellAt() -> ObjectSpec {objectSpec} doesn't has object type setup".LogColor(Color.red));
            cellPositionRequire = null;
            return false;
        }
        var objectSpecDimension = new Vector2Int(objectSpec.DimensionX, objectSpec.DimensionY);
        bool gotBuildPositionResult = getBuildPositionUtil.TryGetBuildPositions(
            targetGridPos: position,
            objectSpec: objectSpecDimension,
            gridPositionResult: out cellPositionRequire
        );

        if (!gotBuildPositionResult || cellPositionRequire == null)
        {
            return false;
        }
        
        //TODO: Check if any of the cell needed are occupied

        foreach (var cellPos in cellPositionRequire)
        {
            if (!boardCellDict.ContainsKey(cellPos))
            {
                //TODO: wtf are we doing here? why there are cell that out side the board? just reject the shit out of this?
                Debug.LogError($"{GetType().Name}.TryPlaceCellAt() -> Cell {cellPos} does not exist".LogColor(Color.red));
                return false;
            }

            if (boardCellDict[cellPos].OccupyType is CellOccupyType.Occupied)
            {
                return false;
            }
        }
        
        //TODO: update the grid
        
        //TODO: place the object visual

        return true;
    }
    
    public Vector2Int FlipObjectSpec(Vector2Int baseSpec)
    {
        return new Vector2Int(baseSpec.y,baseSpec.x);
    }
    
    
    [Serializable]
    private class PreviewCellRuntimeData
    {
        public Vector2Int CellPosition;
        public CellData CellData;
        public  PreviewCellRuntimeData(Vector2Int cellPosition, CellData cellData)
        {
            CellPosition = cellPosition;
            CellData = cellData;
        }
    }
}