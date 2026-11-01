#nullable enable
using System;
using UnityEngine;

/// <summary>
/// Will be initializer for managers, keeping references
/// Would love to refactor this system to be used by DI instead of this
/// </summary>
public class GameManager : MonoBehaviour
{
    [Space,Header("Managers")] 
    [SerializeField] private GameStateManager? gameStateManager;
    [SerializeField] private BoardManager boardManager = new BoardManager();
    private SaveLoadManager saveLoadManager = new SaveLoadManager();
    
    [Space,Header("GameConfigs")] 
    [SerializeField] private GameConfigContainer gameConfigContainer;
    
    public static GameManager instance;
    public GameStateManager GameStateManager => gameStateManager;
    public BoardManager BoardManager => boardManager;
    public SaveLoadManager SaveLoadManager => saveLoadManager;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        gameStateManager?.Initialize();
    }
}
