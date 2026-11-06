#nullable enable
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    private SaveLoadManager saveLoadManager = new SaveLoadManager();
    public SaveLoadManager SaveLoadManager => saveLoadManager;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;    
        }
        else
        {
            Destroy(gameObject);
        }
        
       
        DontDestroyOnLoad(gameObject);
    }

   
}
