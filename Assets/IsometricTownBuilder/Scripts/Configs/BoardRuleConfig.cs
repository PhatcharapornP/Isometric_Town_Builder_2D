using UnityEngine;

[CreateAssetMenu(fileName = "BoardRuleConfig", menuName = "Scriptable Objects/BoardRuleConfig")]
public class BoardRuleConfig : ScriptableObject
{
    [field: SerializeField] public int StationLimit { get; private set; }= 3;
    
    
}


//TODO: Need to find a away to comply with this rule, would love to implement system to support each type of the rule type later
//but will do the simplest way as much as possible
public class BoardRuleChecker
{
    private BoardRuleConfig ruleConfig;
    private int stationCount;
    
    
    public BoardRuleChecker(BoardRuleConfig config)
    {
        ruleConfig = config;
    }

    public bool StillAbleToAffordStationCount()
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
    
    
}
