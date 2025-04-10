using UnityEngine;

namespace Hex 
{
    [System.Serializable]
    public struct HexCoordinate 
    {
        [SerializeField] private int q;
        [SerializeField] private int r;
        
        public int Q { get => q; set => q = value; }
        public int R { get => r; set => r = value;  }
        public int S => -q - r; // S is always computed
        
        public HexCoordinate(int q, int r)
        {
            this.q = q;
            this.r = r;
        }
    }
}