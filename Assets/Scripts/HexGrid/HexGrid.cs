using System.Collections.Generic;
using UnityEngine;

namespace HexGrid
{
    public class HexGrid : MonoBehaviour
    {
        public float spacing;

        public HexCell hexCellPrefab;

        public List<HexCell> hexCells = new();

        private static readonly (int q, int r)[] NeighborOffsets =
        {
            (1, 0), (0, 1), (-1, 1),
            (-1, 0), (0, -1), (1, -1)
        };

        public HexCell GetCell(int q, int r)
        {
            return hexCells.Find(cell => cell.Q == q && cell.R == r);
        }

        public void CreateCell(int q, int r)
        {
            // Get position in world space
            var position = new Vector3(
                (HexConstants.InnerRadius * 2f + spacing) * (q + r * 0.5f),
                0,
                (HexConstants.OuterRadius * 1.5f + spacing) * r
            );

            var cell = Instantiate(hexCellPrefab, transform, false);
            cell.transform.localPosition = position;

            // Grid coordinates
            cell.Q = q;
            cell.R = r;

            hexCells.Add(cell);
        }

        public void RemoveCell(int q, int r)
        {
            var cellToRemove = hexCells.Find(cell => cell.Q == q && cell.R == r);

            if (cellToRemove != null)
            {
                hexCells.Remove(cellToRemove);
                Destroy(cellToRemove.gameObject);
            }
            else
            {
                Debug.LogWarning($"No HexCell found at ({q}, {r}) to remove.");
            }
        }

        public List<HexCell> GetNeighbors(HexCell cell)
        {
            List<HexCell> neighbors = new();

            foreach (var (dq, dr) in NeighborOffsets)
            {
                var neighborQ = cell.Q + dq;
                var neighborR = cell.R + dr;

                // Find the neighboring cell
                var neighbor = hexCells.Find(c => c.Q == neighborQ && c.R == neighborR);

                if (neighbor != null)
                {
                    neighbors.Add(neighbor);
                }
            }

            return neighbors;
        }
    }
}