#nullable enable
using System;
using Cysharp.Threading.Tasks;
using UnityEngine;

[RequireComponent(typeof(BuildingObject))]
public abstract class StationInteraction : MonoBehaviour
{
    public abstract StationType StationType { get; }
    public bool IsInitialized { get; protected set; } = false;
    protected GamePlayManager gamePlayManager;
    protected GameStateManager gameStateManager;
    protected PlayerInputHandler playerInput;
    [SerializeField] private BuildingObject? buildingObject;

    private void OnValidate()
    {
        if (buildingObject == null)
        {
            buildingObject = GetComponent<BuildingObject>();    
        }
    }

    public virtual void Start()
    {
        InitializeAsync().Forget();
    }

    protected virtual async UniTask InitializeAsync()
    {
        if (buildingObject == null)
        {
            buildingObject = GetComponent<BuildingObject>();    
        }
        await UniTask.WaitUntil(() => GamePlayManager.Instance != null
                                      && GamePlayManager.Instance.PlayerInput != null
                                      && GamePlayManager.Instance.GameStateManager != null);
        gamePlayManager =  GamePlayManager.Instance;
        playerInput =  gamePlayManager.PlayerInput;
        gameStateManager =  gamePlayManager.GameStateManager;
        playerInput.OnReleased += OnInputReleased;
        
        IsInitialized = true;
    }

    private void OnDestroy()
    {
        playerInput.OnReleased -= OnInputReleased;
    }

    public abstract void OnInputReleased(Vector3 screenPos);

    protected bool IsThisBuilding(Vector3 screenPos)
    {
        bool gotBuildingObj = gamePlayManager.BuildingObjSelecter.TryGetBuildingObject(screenPos, out var buildingObject);
        bool isThisBuilding = gotBuildingObj
                               && buildingObject != null
                               && buildingObject == this.buildingObject;

        return gotBuildingObj && isThisBuilding;
    }

    protected bool IsInteractionBlocked()
    {
        return buildingObject.HasInteractionBlocker();
    }
}
