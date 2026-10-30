using UnityEngine;
using UnityEngine.Tilemaps;

namespace Mythryl
{
    /// <summary>A Rule Tile that counts a cell outside the map as itself, so shores stop at the edge of the map instead of running round it.</summary>
    public class EdgeRuleTile : RuleTile
    {
        public override bool RuleMatch(int neighbor, TileBase other)
        {
            return base.RuleMatch(neighbor, other == null ? this : other);
        }
    }
}
