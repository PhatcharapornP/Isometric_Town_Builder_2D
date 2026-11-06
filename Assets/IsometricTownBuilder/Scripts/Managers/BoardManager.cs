#nullable enable
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using UnityEngine;

public static class BoardManager 
{
    //for current complexity, tile just need to be crossed with buildingId to see that it's being occupied by what type of the building
    //and that should be it?
    private static Dictionary<TilePosition, BuildObjectSaveData>? buildDataDict = new Dictionary<TilePosition,BuildObjectSaveData>();
    private static Dictionary<TilePosition,int> runtimeOccupiedTiles = new Dictionary<TilePosition, int>();
    public static Dictionary<TilePosition,BuildObjectSaveData>? BuildDataDict => buildDataDict;
    
    public static void LoadSaveData()
    {
        PlayerPrefManager.TryGetString(SaveKeyGlobal.BuildObjectSaveKey,out string saveStringValue);
        Debug.Log($"[BoardManager] LoadSaveData() -> saveStringValue: {saveStringValue}".LogColor(new Color(1f, 0.63f, 0.92f)));
        var tmpLoadedList  = JsonConvert.DeserializeObject<List<BuildObjectSaveData>>(saveStringValue);
        if (tmpLoadedList != null)
        {
            foreach (var saveData in tmpLoadedList)
            {
                buildDataDict.Add(saveData.BuildData.OriginTilePosition, saveData);
                foreach (var tilePos in saveData.BuildData.BuiltOnTiles)
                {
                    if (runtimeOccupiedTiles.ContainsKey(tilePos))
                    {
                        continue;
                    }
                    runtimeOccupiedTiles.Add(tilePos,saveData.BuildingId);    
                }
            }
        }
        else
        {
            Debug.Log($"[BoardManager] LoadSaveData() -> buildDataDict null".LogColor(new Color(1f, 0.45f, 0.24f)));
        }
    }

    public static void UpdateBuildDataDict(int buildingId,BuildObjectRuntimeData toSaveData)
    {
        var newSaveData = new BuildObjectSaveData(buildingId,toSaveData);
        if (buildDataDict == null)
        {
            buildDataDict = new Dictionary<TilePosition, BuildObjectSaveData>();
        }
        buildDataDict[newSaveData.BuildData.OriginTilePosition] = newSaveData;
        runtimeOccupiedTiles.Add(newSaveData.BuildData.OriginTilePosition,buildingId);
        
        SaveBoard();
    }

    public static void SaveBoard()
    {
        var buildObjectSaveData = GetBuildObjectSaveData().ToList();
        string saveStringValue = JsonConvert.SerializeObject(buildObjectSaveData);  
        Debug.Log($"[BoardManager] SaveData() -> saveStringValue: {saveStringValue}".LogColor(new Color(0.82f, 1f, 0.52f)));
        PlayerPrefManager.SaveString(SaveKeyGlobal.BuildObjectSaveKey, saveStringValue);
    }
    
    public static IEnumerable<BuildObjectSaveData> GetBuildObjectSaveData()
    {
        foreach (var (_, buildData) in buildDataDict)
        {
            yield return buildData;
        }
    }

    public static bool IsTilePositionFree(Vector3Int tilePosition)
    {
        return runtimeOccupiedTiles.ContainsKey(new TilePosition(tilePosition.x, tilePosition.y, tilePosition.z)) == false;
    }

    public static bool AreTilesFree(IEnumerable<Vector3Int> tilePositions)
    {
        foreach (Vector3Int tilePosition in tilePositions)
        {
            if (!IsTilePositionFree(tilePosition))
            {
                return false;
            }
        }

        return true;
    }

    public static void RemoveBuildData(BuildObjectRuntimeData oldBuildData)
    {
        if (buildDataDict == null)
        {
            return;
        }
        
        var originTilePosition = oldBuildData.OriginTilePosition;
        
        if (!buildDataDict.ContainsKey(originTilePosition))
        {
            return;
        }
        
        var tempTiles = oldBuildData.BuiltOnTiles;
        foreach (var tilePosition in tempTiles)
        {
            runtimeOccupiedTiles.Remove(tilePosition);   
        }
        
        buildDataDict.Remove(originTilePosition);
    }
}