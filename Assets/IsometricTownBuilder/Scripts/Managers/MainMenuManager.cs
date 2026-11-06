#nullable enable
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

public static class SceneNameKey
{
    public const string GameplaySceneName = "BoardGameplay";
    public const string MainMenuSceneName = "Main";
}

public class MainMenuManager : MonoBehaviour
{
    [SerializeField] private MainMenuUIManager uiManager;

    private void Start()
    {
        uiManager.goToGameplaySceneButton.AddToOnClickAsync(GoToGameplayScene);
        LoadingCanvas.Instance?.FullScreenLoading.HideLoading();
    }

    private void OnDestroy()
    {
        uiManager.goToGameplaySceneButton.RemoveFromOnClickAsync(GoToGameplayScene);
    }

    private async UniTask GoToGameplayScene(CancellationToken ct)
    {
        LoadingCanvas.Instance?.FullScreenLoading.ShowLoading();
        await LoadSceneUtil.GoToSceneAsync(
            targetSceneName: SceneNameKey.GameplaySceneName,
            unloadSceneName: SceneNameKey.MainMenuSceneName
        ).AttachExternalCancellation(ct);
    }

}
