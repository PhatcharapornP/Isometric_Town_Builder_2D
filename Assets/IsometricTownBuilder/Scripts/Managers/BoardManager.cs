#nullable enable
using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class BoardManager 
{
    [SerializeField] private GetBuildPositionUtil  getBuildPositionUtil = new GetBuildPositionUtil();
    
    private Dictionary<Vector3Int,CellData> boardCellDict = new Dictionary<Vector3Int,CellData>();

    [field:SerializeField] public bool IsInitialized { get; private set; } = false;
    [field:SerializeField] public bool HasUpdatedFromSave { get; private set; } = false;

    public void Initialize()
    {
        IsInitialized = true;
    }
    
    //TODO: handle build onto cell
}