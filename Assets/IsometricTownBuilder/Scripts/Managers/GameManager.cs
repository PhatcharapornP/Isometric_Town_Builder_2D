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
        Debug.Log("GameManager Awake".LogColor(new Color(0.35f, 1f, 0.6f)),gameObject);
        DontDestroyOnLoad(gameObject);
    }

   
}
