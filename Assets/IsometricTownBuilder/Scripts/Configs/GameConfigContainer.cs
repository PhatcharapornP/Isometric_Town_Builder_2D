using UnityEngine;

[CreateAssetMenu(fileName = "GameConfigContainer", menuName = "Scriptable Objects/GameConfigContainer")]
public class GameConfigContainer : ScriptableObject
{
    [SerializeField] private BoardConfig boardConfig;
    [SerializeField] private BuildingCatalogConfig buildingCatalogConfig;
    public BoardConfig BoardConfig => boardConfig;
    public BuildingCatalogConfig BuildingCatalogConfig => buildingCatalogConfig;
    
}
