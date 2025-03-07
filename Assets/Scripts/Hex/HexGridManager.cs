using Unity.VisualScripting;
using UnityEngine;

namespace Hex
{
    public class HexGridManager : MonoBehaviour
    {
        public static HexGridManager Instance { get; private set; }

        public int radius = 3;
        public int spacing = 1;
        
        public HexGrid hexGrid;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
            
            hexGrid = this.AddComponent<HexGrid>();
            hexGrid.spacing = spacing;
            
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
                        hexGrid.CreateCell(q, r);
                    }
                }
            }
        }
    }
}