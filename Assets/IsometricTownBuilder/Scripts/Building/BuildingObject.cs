using System;
using System.Collections.Generic;
using UnityEngine;

//TODO: Create runtime data for building obj for save / load and revert to when cancel building

public class BuildingObject : MonoBehaviour
{
   [SerializeField] private Vector3 positionOffset;
   [SerializeField] private Vector3 tilePosOffset;
   [SerializeField] private SpriteRenderer spriteRenderer;
   [field:SerializeField] private BuildSpec[] normalSpec;
   [SerializeField] private BuildSpec[] flipSpec;
   [field:SerializeField] public bool FlipSpriteRenderer { get; private set; } = false;
   [field:SerializeField] public bool HasBeenBuilt { get; private set; } = false;
   [field:SerializeField] public List<Vector3Int> BuiltOnTiles { get; private set; }
   [field:SerializeField] public Vector3 BuiltOnPos { get; private set; }
   [field:SerializeField] public Vector3Int OriginBuiltOnTile { get; private set; }
   [field:SerializeField] public Vector3Int OriginTileWhileHolding { get; private set; }

   private void OnValidate()
   {
      spriteRenderer.flipX = FlipSpriteRenderer;
      flipSpec = new BuildSpec[normalSpec.Length];
      for(int i =0 ; i < normalSpec.Length; i++)
      {
         flipSpec[i].X =  normalSpec[i].Y;
         flipSpec[i].Y =  normalSpec[i].X;
      }
   }

   public void SetBuilt(bool value)
   {
      HasBeenBuilt = value;
   }

   public void SetBuiltOnTiles(List<Vector3Int> originTilePos)
   {
      BuiltOnTiles = originTilePos;
   }
   
   public void SetFlipSpriteRenderer(bool value)
   {
      FlipSpriteRenderer = value;
      spriteRenderer.flipX = FlipSpriteRenderer;
   }

   public BuildSpec[] GetBuildSpecs()
   {
      return FlipSpriteRenderer ? flipSpec : normalSpec; 
   }

   public void SetPosition(Vector3 screenToWorldPos)
   {
      var newPosition = new Vector3(screenToWorldPos.x,screenToWorldPos.y,0f) + positionOffset;
      transform.position = newPosition;
   }

   public void SetBuiltPosition(Vector3 targetPos)
   {
      BuiltOnPos = new Vector3(targetPos.x,targetPos.y,0f) + tilePosOffset;
   }

   public void SetOriginTileWhileHolding(Vector3Int tilePos)
   {
      OriginTileWhileHolding = tilePos;
   }

   public void SetOriginBuiltOnTile(Vector3Int tilePos)
   {
      OriginBuiltOnTile = tilePos;
   }

   public void SnapToTilePosition(Vector3 tileWorldPos)
   {
      transform.position = new Vector3(tileWorldPos.x,tileWorldPos.y,transform.position.z) + tilePosOffset;
   }
}

[Serializable]
public struct BuildSpec
{
   public int X;
   public int Y;
}
