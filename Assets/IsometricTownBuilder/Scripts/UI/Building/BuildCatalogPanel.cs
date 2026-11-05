#nullable enable
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class BuildCatalogPanel : MonoBehaviour
{
    [SerializeField] private GenericButton openCatalogButton;
    [SerializeField] private GenericButton closeCatalogButton;
    [SerializeField] private GenericButton furnitureTabButton;
    [SerializeField] private GenericButton stationTabButton;
    [SerializeField] private Transform furnitureContainer;
    [SerializeField] private Transform stationContainer;
    [SerializeField] private BuildingButton[] buildingButtons;
   
    public async UniTask Initialize()
    {
        var tasks = new List<UniTask>();
        foreach (var buildingButton in buildingButtons)
        {
            tasks.Add(buildingButton.Initialize());
        }
        
        await UniTask.WhenAll(tasks);
        
        openCatalogButton.AddToClickAction(OpenCatalog);
        closeCatalogButton.AddToClickAction(CloseCatalog);
        
        furnitureTabButton.AddToClickAction(ToggleOnFurnitureTab);
        stationTabButton.AddToClickAction(ToggleOnStationTab);
        
        //TODO: each build button has to close the catalog?
    }

    private void OpenCatalog()
    {
        gameObject.SetActive(true);
        openCatalogButton.gameObject.SetActive(false);
    }

    private void CloseCatalog()
    {
        gameObject.SetActive(false);
        openCatalogButton.gameObject.SetActive(true);
    }
    

    private void ToggleOnStationTab()
    {
        furnitureContainer.gameObject.SetActive(false);
        stationContainer.gameObject.SetActive(true);
    }

    private void ToggleOnFurnitureTab()
    {
        furnitureContainer.gameObject.SetActive(true);
        stationContainer.gameObject.SetActive(false);
    }

    public void Hide()
    {
        CloseCatalog();
        ToggleOnFurnitureTab();
    }
}
