using Buildings;
using UnityEngine;

namespace Hex
{
    public class HexCell : MonoBehaviour, IClickable
    {
        public HexCoordinate HexCoordinate { get; set; }

        public GameObject OccupiedBy { get; set; }
        public bool Occupied => OccupiedBy;
        public bool Preview { get; set; }

        private void OnDrawGizmos()
        {
            UnityEditor.Handles.Label(transform.position, $"({HexCoordinate.Q}, {HexCoordinate.R}, {HexCoordinate.S})");
        }

        public void OnClick(GameObject obj)
        {
            Debug.Log($"HexCell ({HexCoordinate.Q}, {HexCoordinate.R}, {HexCoordinate.S})");
        }
    }
}