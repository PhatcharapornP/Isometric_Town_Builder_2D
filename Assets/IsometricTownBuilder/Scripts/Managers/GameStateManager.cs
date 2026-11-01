#nullable enable
using UnityEngine;

public class GameStateManager : MonoBehaviour
{
    private IGameState? Idle;
    private IGameState? Building;
    private IGameState? Quiz;
    private IGameState? Mixing;
    private IGameState? Saving;
    private IGameState? Loading;
    private IGameState? LoadingSave;

    public void Initialize()
    {
        Idle?.Initialize();
        Building?.Initialize();
        Quiz?.Initialize();
        Mixing?.Initialize();
        Saving?.Initialize();
        Loading?.Initialize();
        LoadingSave?.Initialize();
        Loading?.Initialize();
    }
}