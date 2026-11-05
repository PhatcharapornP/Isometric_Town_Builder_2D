#nullable enable
using UnityEngine;

/// <summary>
/// Will be initializer for managers, keeping references
/// Would love to refactor this system to be used by DI instead of this
/// </summary>
public class GameManager : MonoBehaviour
{
    [Header("SaveLoadManager container here.")]
    public static GameManager Instance;
    private SaveLoadManager saveLoadManager = new SaveLoadManager();
    public SaveLoadManager SaveLoadManager => saveLoadManager;

    private void Awake()
    {
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

   
}
