using Unity.VisualScripting;
using UnityEngine;

namespace HexGrid
{
    public class HexGridManager : MonoBehaviour
    {
        public int radius = 3;
        public int spacing = 1;
        
        public static HexGrid HexGrid;

        private void Awake()
        {
            HexGrid = this.AddComponent<HexGrid>();
            HexGrid.spacing = spacing;
            
            GenerateRoundHexGrid();
        }

        private void GenerateRoundHexGrid()
        {
            for (var q = -radius; q <= radius; q++)
            {
                for (var r = -radius; r <= radius; r++)
                {
                    var s = -q - r;
                    if (Mathf.Abs(s) <= radius)
                    {
                        HexGrid.CreateCell(q, r);
                    }
                }
            }
        }
    }
}