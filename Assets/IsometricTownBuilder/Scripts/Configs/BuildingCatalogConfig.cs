#nullable enable
using System;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "BuildingCatalogConfig", menuName = "Scriptable Objects/BuildingCatalogConfig")]
public class BuildingCatalogConfig : ScriptableObject
{
    [field:SerializeField] public BuildingCatalogData[] Buildings { get; private set; }

    public BuildingCatalogData? GetBuildingCatalogData(int buildingId)
    {
        var result =Buildings.ToList().Find(x => x.BuildingId == buildingId);
        return result;
    }
}

[Serializable]
public struct BuildingCatalogData
{
    public int BuildingId;
    public BuildingObject? buildingPrefab;
}
