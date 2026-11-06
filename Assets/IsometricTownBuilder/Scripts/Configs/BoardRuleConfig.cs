using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BoardRuleConfig", menuName = "Scriptable Objects/BoardRuleConfig")]
public class BoardRuleConfig : ScriptableObject
{
    [field: SerializeField] public int StationLimit { get; private set; }= 3;
    [field: SerializeField] public bool CanBuildSameStation { get; private set; } = false;
    
}


//TODO: Need to find a away to comply with this rule, would love to implement system to support each type of the rule type later
//but will do the simplest way as much as possible
public class BoardRuleChecker
{
    private BoardRuleConfig ruleConfig;
    private int stationCount;
    private List<int> stationId = new List<int>();
    
    public BoardRuleChecker(BoardRuleConfig config)
    {
        ruleConfig = config;
    }

    private  bool StillAbleToAffordStationCount()
    {
        bool able = stationCount < ruleConfig.StationLimit;
        // Debug.Log($"[BoardRuleChecker] StillAbleToAffordStationCount: stationCount {stationCount} | able?: {able}");
        return able;
    }

    public void IncreaseStationCount(int amount)
    {
        stationCount += amount;
        if (stationCount > ruleConfig.StationLimit)
        {
            stationCount = ruleConfig.StationLimit;
        }
        // Debug.Log($"[BoardRuleChecker] stationCount {stationCount}");
    }

    public void AddToBuiltStationId(int buildingId)
    {
        stationId.Add(buildingId);
    }

    public bool CanBuildStation(int buildingId)
    {
        bool stationAffordable = StillAbleToAffordStationCount();
        
        if (ruleConfig.CanBuildSameStation)
        {
            return stationAffordable;
        }
        else
        {
            return  stationId.Contains(buildingId) == false && stationAffordable;
        }
    }
}
