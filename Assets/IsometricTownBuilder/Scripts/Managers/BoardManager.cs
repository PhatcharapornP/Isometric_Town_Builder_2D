#nullable enable
using System.Collections.Generic;
using UnityEngine;

public static class BoardManager 
{
    private static Dictionary<Vector3Int,CellData> boardCellDict = new Dictionary<Vector3Int,CellData>();
    //TODO: be used by SaveLoadManager to init saved build objects in the board
}