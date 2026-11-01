#nullable enable
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuUIManager : MonoBehaviour
{
    //There's nothing for main menu right now but let's put a button for loading to next scene

    [SerializeField] private GenericButton goToGameplayScene;
    public GenericButton goToGameplaySceneButton => goToGameplayScene;
}

public static class LoadSceneUtil
{
    public static async UniTask GoToSceneAsync(string targetSceneName,string? unloadSceneName)
    {
        await SceneManager.LoadSceneAsync(targetSceneName);
        SceneManager.SetActiveScene(SceneManager.GetSceneByName(targetSceneName));
        if (!string.IsNullOrEmpty(unloadSceneName))
        {
            await SceneManager.UnloadSceneAsync(unloadSceneName);    
        }
        
        await Resources.UnloadUnusedAssets();
    }
}