using UnityEngine;

[CreateAssetMenu(fileName = "GameConfigContainer", menuName = "Scriptable Objects/GameConfigContainer")]
public class GameConfigContainer : ScriptableObject
{
    [SerializeField] private BoardConfig boardConfig;
    [SerializeField] private BuildingCatalogConfig buildingCatalogConfig;
    [SerializeField] private BoardRuleConfig boardRuleConfig;
    [SerializeField] private QuizQuestions quizQuestions;
    public BoardConfig BoardConfig => boardConfig;
    public BuildingCatalogConfig BuildingCatalogConfig => buildingCatalogConfig;
    public BoardRuleConfig BoardRuleConfig => boardRuleConfig;
    public QuizQuestions QuizQuestions => quizQuestions;
    
}
