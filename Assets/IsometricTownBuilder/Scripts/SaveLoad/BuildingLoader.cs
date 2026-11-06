using System.Linq;
using UnityEngine;

public class BuildingLoader : MonoBehaviour
{
    //will need building catalogConfig
    [SerializeField] private BuildingCatalogConfig buildingCatalogConfig;
    private BoardRuleChecker boardRuleChecker;
    private TilemapManager tilemapManager;

    public void Initialize(
        BuildingCatalogConfig catalogConfig,
        TilemapManager tileMap,
        BoardRuleChecker ruleChecker
        )
    {
        buildingCatalogConfig = catalogConfig;
        tilemapManager = tileMap;
        boardRuleChecker = ruleChecker;
    }

    public void SpawnSavedBuildingIntoGame()
    {
        if (buildingCatalogConfig == null)
        {
            return;
        }
        var buildDataDict = BoardManager.BuildDataDict;
        if (buildDataDict == null)
        {
            return;
        }
        
        foreach (var (tilePos, buildData) in buildDataDict)
        {
            var buildingData = buildingCatalogConfig.GetBuildingCatalogData(buildData.BuildingId);
            if (buildingData == null)
            {
                continue;
            }
            
            var buildingObj = Instantiate(buildingData.Value.buildingPrefab);
            
            var originPos = buildData.BuildData.OriginTilePosition;
            var originVec3 = new Vector3Int(originPos.X, originPos.Y, originPos.Z);
            var runtimeData = new BuildObjectRuntimeData(
                originTilePosition: originVec3,
                builtOnTiles: buildData.BuildData.GetBuiltOnTiles().ToList(),
                flipValue: buildData.BuildData.FlipValue
            );
            
            buildingObj.SetBuildData(runtimeData);
            
            var tileWorldPos = tilemapManager.GetTileWorldPosition(new Vector3Int(tilePos.X, tilePos.Y, tilePos.Z));
            buildingObj.SnapToTilePosition(tileWorldPos);
            if (buildingObj.CellType.HasFlag(CellType.Station))
            {
                boardRuleChecker.AddToBuiltStationId(buildingObj.BuildingId);
            }
        }
    }
}
