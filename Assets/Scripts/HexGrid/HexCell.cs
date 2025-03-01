using UnityEngine;

namespace HexGrid
{
    public class HexCell : MonoBehaviour
    {
        public int Q { get; set; }
        public int R { get; set; }
        private int S => -Q - R;
        
        private void OnDrawGizmos()
        {
            UnityEditor.Handles.Label(transform.position, $"({Q}, {R}, {S})");
        }
    }
}