#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public class GetBuildPositionUtil
{
    [SerializeField] public int size = 0;
    [SerializeField] private int yCount = 0;
    [SerializeField] private int xCount = 0;
    public bool TryGetBuildPositions(
        Vector2Int? targetGridPos, 
        Vector2Int objectSpec,
        out Vector2Int[]? gridPositionResult 
        )
    {
        if (!targetGridPos.HasValue)
        {
            gridPositionResult = null;
            return false;
        }

        size = objectSpec.x * objectSpec.y;

        if (size <= 0)
        {
            gridPositionResult = null;
            return false;
        }
        
        yCount = objectSpec.x;
        xCount = objectSpec.y;
        
        var tmpCells = new List<Vector2Int>(size);
        for (int x = 0; x < xCount; x++)
        {
            for (int y = 0; y < yCount; y++)
            {
                tmpCells.Add(targetGridPos.Value + new Vector2Int(x, y));
            }
        }
        
        gridPositionResult = tmpCells.ToArray();
        Debug.Log($"gridPos: \n {string.Join(" ",gridPositionResult.ToList())}".LogColor(new Color(0.55f, 1f, 0.45f)));
        
        return true;
    }
}
