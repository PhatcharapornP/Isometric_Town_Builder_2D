#nullable enable
using System;
using System.Runtime.CompilerServices;
using UnityEngine;

public class BuildingOptionPanel : MonoBehaviour
{
    [SerializeField] private GenericButton acceptButton;
    [SerializeField] private GenericButton cancelButton;
    [SerializeField] private GenericButton flipButton;
    private Canvas canvas;
    private Camera? targetCamera => Camera.main;

    public event Action? OnAcceptBuild;
    public event Action? OnCancelBuild;
    public event Action? OnFlipBuild;

    [field: SerializeField] public bool IsInitialized { get; private set; } = false;
    public bool IsShowing => gameObject.activeInHierarchy;

    public void Initialize(Canvas canvas)
    {
        this.canvas = canvas;
        
        IsInitialized = true;
    }

    private void OnEnable()
    {
        Debug.Log($"{GetType().Name}.OnEnable OnAcceptBuild null?: {OnAcceptBuild == null}" +
                  $"\n OnCancelBuild null?: {OnCancelBuild == null}" +
                  $"\n OnFlipBuild null?: {OnFlipBuild == null}");
        acceptButton.AddToClickAction(OnAcceptBuild);
        cancelButton.AddToClickAction(OnCancelBuild);
        flipButton.AddToClickAction(OnFlipBuild);
    }

    private void OnDestroy()
    {
        acceptButton.RemoveFromClickAction(OnAcceptBuild);
        cancelButton.RemoveFromClickAction(OnCancelBuild);
        flipButton.RemoveFromClickAction(OnFlipBuild);
    }

    public void EnableOptionPanel(bool enable
#if UNITY_EDITOR
    ,[CallerMemberName] string caller = ""
#endif
    )
    {
        if (gameObject.activeInHierarchy == enable)
        {
            return;
        }
        
        gameObject.SetActive(enable);
        
#if UNITY_EDITOR
        Debug.Log($"====>>> {GetType().Name}.EnableOptionPanel({enable}) from: {caller}".LogColor(Color.cornflowerBlue));
#endif
    }

    public void SetPanelPositionFromScreenPosition(Vector2 screenPos)
    {
        bool posIsInScreenRect = RectTransformUtility.ScreenPointToWorldPointInRectangle(
            (RectTransform)canvas.transform,
            screenPos, targetCamera,
            out Vector3 worldPos
        );
        if (posIsInScreenRect)
        {
            transform.position = worldPos;
        }
    }
}
