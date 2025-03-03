using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace HexGrid
{
    public class HexGridGenerator : MonoBehaviour
    {
        public int radius = 3;
        
        private HexGrid _hexGrid;

        private void Awake()
        {
            _hexGrid = this.AddComponent<HexGrid>();
            _hexGrid.spacing = 1;
            
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
                        _hexGrid.CreateCell(q, r);
                    }
                }
            }
        }
    }
}