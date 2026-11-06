#nullable enable
using UnityEngine;
using UnityEngine.EventSystems;

public class HideCatalogOnPointerDownInBlockerArea : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField] private BuildCatalogPanel buildCatalogPanel;
    [SerializeField] private bool isPointerDown = false;
    public void OnPointerDown(PointerEventData eventData)
    {
        buildCatalogPanel.Hide();
        isPointerDown = true;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        isPointerDown = false;
    }
}