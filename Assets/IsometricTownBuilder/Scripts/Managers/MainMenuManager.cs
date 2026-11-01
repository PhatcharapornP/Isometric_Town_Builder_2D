#nullable enable
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class MainMenuManager : MonoBehaviour
{
    [SerializeField] private MainMenuUIManager uiManager;
    
    private const string gameplaySceneName = "BoardGameplay";
    private const string mainMenuSceneName = "Main";

    private void Start()
    {
        uiManager.goToGameplaySceneButton.AddToOnClickAsync(GoToGameplayScene);
    }

    private void OnDestroy()
    {
        uiManager.goToGameplaySceneButton.RemoveFromOnClickAsync(GoToGameplayScene);
    }

    private async UniTask GoToGameplayScene(CancellationToken ct)
    {
        await LoadSceneUtil.GoToSceneAsync(
            targetSceneName: gameplaySceneName,
            unloadSceneName: mainMenuSceneName
        ).AttachExternalCancellation(ct);
    }

}
