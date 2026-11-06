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
    private List<int> stationIdList;
    
    public BoardRuleChecker(BoardRuleConfig config)
    {
        ruleConfig = config;
        stationIdList = new List<int>();
    }

    private  bool StillAbleToAffordStationCount()
    {
        bool able = stationIdList.Count < ruleConfig.StationLimit;
        return able;
    }

    public void AddToBuiltStationId(int buildingId)
    {
        stationIdList.Add(buildingId);
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
            bool containedId = stationIdList.Contains(buildingId);
            bool canBuildStation = containedId == false && stationAffordable;
            return  canBuildStation;
        }
    }

    public void RemoveStation(int buildingId)
    {
        stationIdList.Remove(buildingId);
    }
}
