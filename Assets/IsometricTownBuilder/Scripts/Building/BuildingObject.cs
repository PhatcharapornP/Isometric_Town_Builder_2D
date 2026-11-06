#nullable enable
using System;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;

[RequireComponent(typeof(Collider2D))]
public class BuildingObject : MonoBehaviour
{
    [field: FormerlySerializedAs("<BulidingId>k__BackingField")] 
    [field: SerializeField] public int BuildingId { get; private set; }

    [field: SerializeField] public CellType CellType { get; private set; } 
    [SerializeField] private Vector3 positionOffset;
    [SerializeField] private Vector3 tilePosOffset;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private BuildSpec[] normalSpec;
    [SerializeField] private BuildSpec[] flipSpec;
    [Space]
    [SerializeField] private Collider2D collider2D;
    [SerializeField] private Vector2 colliderOffset;
    [SerializeField] private Vector2 flippedColliderOffset;
    
    [field: SerializeField] public bool HasBeenBuilt { get; private set; } = false;

    [field: SerializeField] public BuildObjectRuntimeData RuntimeBuildData { get; private set; } //to be used while building

    [field: SerializeField] public BuildObjectRuntimeData BuildData { get; private set; } //loaded / to be fallback to when cancel buliding
    
    private BuildingProtector? buildingProtector;
    private BuildingInteractionBlocker? interactionBlocker;
    
    public bool HasBuildingProtector()
    {
        if (buildingProtector == null)
        {
            buildingProtector = gameObject.GetComponent<BuildingProtector>();
        }
        
        return buildingProtector != null;
    }

    public bool HasInteractionBlocker()
    {
        if (interactionBlocker == null)
        {
            interactionBlocker = gameObject.GetComponent<BuildingInteractionBlocker>();
        }
        
        return interactionBlocker != null;
    }


    public void SetBuildData(BuildObjectRuntimeData buildData)
    {
        SetBuilt(true);
        BuildData = buildData;
        SetRendererFlipX(BuildData.FlipValue);
    }

    public void SetBuilt(bool value)
    {
        HasBeenBuilt = value;
    }

    public void SetFlipSpriteRenderer(bool value)
    {
        RuntimeBuildData.SetFlipSpriteRenderer(value);
        SetRendererFlipX(RuntimeBuildData.FlipValue);
    }

    private void SetRendererFlipX(bool value)
    {
        spriteRenderer.flipX = value;
        if (collider2D != null
            && colliderOffset != Vector2.zero
            && flippedColliderOffset != Vector2.zero
           )
        {
            if (value)
            {
                collider2D.offset = flippedColliderOffset;
            }
            else
            {
                collider2D.offset = colliderOffset;
            }
        }
    }

    public BuildSpec[] GetBuildSpecs()
    {
        return RuntimeBuildData.FlipValue ? flipSpec : normalSpec;
    }


    public void CacheBuildToRuntimeData()
    {
        RuntimeBuildData = new BuildObjectRuntimeData(
            originTilePosition: new Vector3Int(BuildData.OriginTilePosition.X, BuildData.OriginTilePosition.Y, BuildData.OriginTilePosition.Z),
            builtOnTiles: BuildData.GetBuiltOnTiles().ToList(),
            flipValue: BuildData.FlipValue
        );
    }

    public void ClearRunTimeData()
    {
        RuntimeBuildData = new BuildObjectRuntimeData(
            originTilePosition: default,
            builtOnTiles: default,
            flipValue: false
        );
    }

    public void OverwriteOldBuildDataWithNewOne(BuildObjectRuntimeData updatedData)
    {
        var oldBuildData = BuildData; 
        BoardManager.RemoveBuildData(oldBuildData);
        BuildData = updatedData;
        BoardManager.UpdateBuildDataDict(BuildingId,BuildData);
    }

    public void SetPosition(Vector3 screenToWorldPos)
    {
        var newPosition = new Vector3(screenToWorldPos.x, screenToWorldPos.y, 0f) + positionOffset;
        transform.position = newPosition;
    }

    public void SnapToTilePosition(Vector3 tileWorldPos)
    {
        var newPosition = new Vector3(tileWorldPos.x, tileWorldPos.y, transform.position.z) + tilePosOffset;
        transform.position = newPosition;
    }

    private void OnValidate()
    {
        if (collider2D == null)
        {
            collider2D = GetComponent<Collider2D>();
        }

        SetFlipSpriteRenderer(RuntimeBuildData.FlipValue);
        flipSpec = new BuildSpec[normalSpec.Length];
        for (int i = 0; i < normalSpec.Length; i++)
        {
            flipSpec[i].X = normalSpec[i].Y;
            flipSpec[i].Y = normalSpec[i].X;
        }
    }
}

[Serializable]
public struct BuildSpec
{
    public int X;
    public int Y;
}