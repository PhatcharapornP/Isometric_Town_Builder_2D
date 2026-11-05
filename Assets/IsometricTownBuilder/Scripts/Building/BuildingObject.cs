using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

//TODO: Create runtime data for building obj for save / load and revert to when cancel building

[Serializable]
public class BuildObjectRuntimeData
{
   [field: SerializeField] public Vector3Int OriginTilePosition { get; private set; }
   [field: SerializeField] public List<Vector3Int> BuiltOnTiles { get; private set; }
   [field: SerializeField] public bool FlipValue { get; private set; }

   public BuildObjectRuntimeData(
      Vector3Int originTilePosition,
      List<Vector3Int> builtOnTiles,
      bool flipValue
   )
   {
      OriginTilePosition = originTilePosition;
   }

   public void SetOriginalTilePosition(Vector3Int originTilePosition)
   {
      OriginTilePosition = originTilePosition;
   }

   public void SetBuiltOnTiles(List<Vector3Int> builtOnTiles)
   {
      BuiltOnTiles = builtOnTiles;
   }
   
   public void SetFlipSpriteRenderer(bool flipValue)
   {
      FlipValue = flipValue;
   }
}

public class BuildingObject : MonoBehaviour
{
   [field:SerializeField] public int BulidingId { get; private set; }
   [SerializeField] private Vector3 positionOffset;
   [SerializeField] private Vector3 tilePosOffset;
   [SerializeField] private SpriteRenderer spriteRenderer;
   [field:SerializeField] private BuildSpec[] normalSpec;
   [SerializeField] private BuildSpec[] flipSpec;
   // [field:SerializeField] public bool FlipSpriteRenderer { get; private set; } = false;
   [field:SerializeField] public bool HasBeenBuilt { get; private set; } = false;
   // [field:SerializeField] public List<Vector3Int> BuiltOnTiles { get; private set; }
   // [field:SerializeField] public Vector3 BuiltOnPos { get; private set; }
   // [field:SerializeField] public Vector3Int OriginBuiltOnTile { get; private set; }
   // [field:SerializeField] public Vector3Int OriginTileWhileHolding { get; private set; }

   [field: SerializeField] public BuildObjectRuntimeData BuildingRuntimeData { get; private set; } //to be used while building
   [field: SerializeField] public BuildObjectRuntimeData BuiltData { get; private set; } //loaded / to be revert back to when cancel buliding

   public void SetBuilt(bool value)
   {
      HasBeenBuilt = value;
   }
   
   public void SetFlipSpriteRenderer(bool value)
   {
      BuildingRuntimeData.SetFlipSpriteRenderer(value);
      spriteRenderer.flipX = BuildingRuntimeData.FlipValue;
   }

   public BuildSpec[] GetBuildSpecs()
   {
      return BuildingRuntimeData.FlipValue ? flipSpec : normalSpec; 
   }

   public void UpdateRuntimeData(BuildObjectRuntimeData runtimeData)
   {
      BuildingRuntimeData =  runtimeData;
   }

   public void OverwriteBuiltDataWithRuntimeData()
   {
      UpdateBuiltOnData(BuildingRuntimeData);
   }

   public void UpdateBuiltOnData(BuildObjectRuntimeData updatedData)
   {
      BuiltData = updatedData;
      //TODO: everytime this got updated, the game will save, by boardManager
      
   }
   
   public void SetPosition(Vector3 screenToWorldPos)
   {
      var newPosition = new Vector3(screenToWorldPos.x,screenToWorldPos.y,0f) + positionOffset;
      transform.position = newPosition;
   }

   public void SnapToTilePosition(Vector3 tileWorldPos)
   {
      var newPosition = new Vector3(tileWorldPos.x, tileWorldPos.y, transform.position.z) + tilePosOffset;
      transform.position = newPosition;
   }
   
   
   // public void SetBuiltOnTiles(List<Vector3Int> originTilePos)
   // {
   //    BuiltOnTiles = originTilePos;
   // }

   // public void SetBuiltPosition(Vector3 targetPos)
   // {
   //    BuiltOnPos = new Vector3(targetPos.x,targetPos.y,0f) + tilePosOffset;
   // }
   //
   // public void SetRuntimeOriginTile(Vector3Int tilePos)
   // {
   //    buildingRuntimeData.OriginTile = tilePos;
   // }
   //
   // public void SetBuiltOnOriginTile(Vector3Int tilePos)
   // {
   //    builtRuntimeData.OriginTile = tilePos;
   // }
   
   private void OnValidate()
   {
      SetFlipSpriteRenderer(BuildingRuntimeData.FlipValue);
      flipSpec = new BuildSpec[normalSpec.Length];
      for(int i =0 ; i < normalSpec.Length; i++)
      {
         flipSpec[i].X =  normalSpec[i].Y;
         flipSpec[i].Y =  normalSpec[i].X;
      }
   }
}

[Serializable]
public struct BuildSpec
{
   public int X;
   public int Y;
}
