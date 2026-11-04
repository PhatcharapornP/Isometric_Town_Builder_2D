#nullable enable
using System.Collections.Generic;
using UnityEngine;

public class GameStateManager : MonoBehaviour
{
    private IGameState idle = new IdleGameState();
    private IGameState building = new BuildingGameState();
    private IGameState quiz = new QuizGameState();
    private IGameState mixing = new MixingGameState();
    private IGameState saving = new SavingGameState();
    private IGameState loading = new LoadingGameState();
    private IGameState loadingSave = new LoadingSaveGameState();
    private IGameState? currentState;
    public IGameState? CurrentState => currentState;

    private Dictionary<GameStateType, IGameState> gameStateDict = new();
    public bool IsInitialized { get; private set; } = false;

    public void Initialize()
    {
        InitializeGameStates();
        SetupGameStateDict();
        IsInitialized = true;
    }

    private void InitializeGameStates()
    {
        idle.Initialize();
        building.Initialize();
        quiz.Initialize();
        mixing.Initialize();
        saving.Initialize();
        loading.Initialize();
        loadingSave.Initialize();
    }

    private void SetupGameStateDict()
    {
        gameStateDict.Add(GameStateType.Idle, idle);
        gameStateDict.Add(GameStateType.Building, building);
        gameStateDict.Add(GameStateType.Quiz, quiz);
        gameStateDict.Add(GameStateType.Mixing, mixing);
        gameStateDict.Add(GameStateType.Saving, saving);
        gameStateDict.Add(GameStateType.Loading, loading);
        gameStateDict.Add(GameStateType.LoadingSave, loadingSave);
    }

    public void GoToState(GameStateType goToState)
    {
        if (currentState != null)
        {
            if (currentState.StateType == goToState)
            {
                return;
            }
            
            currentState.Exit();
        }

        currentState = GetGameStateFromType(goToState);
        if (currentState == null)
        {
            //Go to idle
            currentState = idle;
            Debug.LogError(
                $"{GetType().Name}.EnterState() called with {goToState.ToString()} but no current state exists."
                    .LogColor(Color.red));
        }

        currentState.Enter();
    }

    public IGameState? GetGameStateFromType(GameStateType targetState)
    {
        return gameStateDict[targetState];
    }
}