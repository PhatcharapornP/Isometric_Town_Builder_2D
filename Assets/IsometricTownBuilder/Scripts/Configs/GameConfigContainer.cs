using UnityEngine;

[CreateAssetMenu(fileName = "GameConfigContainer", menuName = "Scriptable Objects/GameConfigContainer")]
public class GameConfigContainer : ScriptableObject
{
    [SerializeField] private BoardConfig boardConfig;
    public BoardConfig BoardConfig => boardConfig;
}
