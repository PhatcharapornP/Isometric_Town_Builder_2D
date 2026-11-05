using System;
using UnityEngine;

public class BuildingObject : MonoBehaviour
{
   [SerializeField] private Vector3 positionOffset;
   [field:SerializeField] public bool FlipSpriteRenderer { get; private set; } = false; 
   [SerializeField] private SpriteRenderer spriteRenderer;
   [field:SerializeField] private BuildSpec[] normalSpec;
   [SerializeField] private BuildSpec[] flipSpec;

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
}

[Serializable]
public struct BuildSpec
{
   public int X;
   public int Y;
}
