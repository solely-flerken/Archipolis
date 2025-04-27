using System;
using UnityEngine;

namespace Hex 
{
    [Serializable]
    public struct HexCoordinate : IEquatable<HexCoordinate>
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

        public bool Equals(HexCoordinate other)
        {
            return q == other.q && r == other.r;
        }

        public override bool Equals(object obj)
        {
            return obj is HexCoordinate other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(q, r);
        }
    }
}