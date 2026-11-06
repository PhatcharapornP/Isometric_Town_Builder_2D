#nullable enable
using System;
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
    public event Action? OnFlipBuildAction;

    [field: SerializeField] public bool IsInitialized { get; private set; } = false;
    public bool IsShowing => gameObject.activeInHierarchy;

    public void Initialize(Canvas canvas)
    {
        this.canvas = canvas;
        IsInitialized = true;
    }

    private void OnEnable()
    {
        if (OnAcceptBuild != null)
        {
            acceptButton.AddToClickAction(OnAcceptBuild);
        }

        if (OnCancelBuild != null)
        {
            cancelButton.AddToClickAction(OnCancelBuild);
        }

        if (OnFlipBuildAction != null)
        {
            flipButton.AddToClickAction(OnFlipBuildAction);
        }
    }

    private void OnDisable()
    {
        if (OnAcceptBuild != null)
        {
            acceptButton.RemoveFromClickAction(OnAcceptBuild);
        }

        if (OnCancelBuild != null)
        {
            cancelButton.RemoveFromClickAction(OnCancelBuild);
        }

        if (OnFlipBuildAction != null)
        {
            flipButton.RemoveFromClickAction(OnFlipBuildAction);
        }
    }
   
    public void EnableOptionPanel(bool enable)
    {
        if (gameObject.activeInHierarchy == enable)
        {
            return;
        }
        
        gameObject.SetActive(enable);
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
