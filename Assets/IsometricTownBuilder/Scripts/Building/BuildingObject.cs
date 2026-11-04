using UnityEngine;

public class BuildingObject : MonoBehaviour
{
   [SerializeField] private Vector3 positionOffset;
   [field:SerializeField] public bool FlipSpriteRenderer { get; private set; } = false; 
   [SerializeField] private SpriteRenderer spriteRenderer;
   [SerializeField] private Transform[] buildingQuads;

   private void OnValidate()
   {
      spriteRenderer.flipX = FlipSpriteRenderer;
   }
   
   public void SetFlipSpriteRenderer(bool value)
   {
      FlipSpriteRenderer = value;
      spriteRenderer.flipX = FlipSpriteRenderer;
      
   }

   public void SetPosition(Vector3 screenToWorldPos)
   {
      var newPosition = new Vector3(screenToWorldPos.x,screenToWorldPos.y,0f) + positionOffset;
      transform.position = newPosition;
   }
}
