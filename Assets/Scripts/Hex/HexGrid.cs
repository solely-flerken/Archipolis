using System.Collections.Generic;
using System.Linq;
using Input;
using UnityEngine;
using Utils;

namespace Hex
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

        public HexCell GetCell(HexCoordinate coordinate)
        {
            return hexCells.Find(cell => cell.HexCoordinate.Q == coordinate.Q && cell.HexCoordinate.R == coordinate.R);
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
            cell.HexCoordinate = new HexCoordinate(q, r);

            hexCells.Add(cell);
        }

        public void RemoveCell(int q, int r)
        {
            var cellToRemove = hexCells.Find(cell => cell.HexCoordinate.Q == q && cell.HexCoordinate.R == r);

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
                var neighborQ = cell.HexCoordinate.Q + dq;
                var neighborR = cell.HexCoordinate.R + dr;

                // Find the neighboring cell
                var neighbor = hexCells.Find(c => c.HexCoordinate.Q == neighborQ && c.HexCoordinate.R == neighborR);

                if (neighbor != null)
                {
                    neighbors.Add(neighbor);
                }
            }

            return neighbors;
        }

        public HexCoordinate WorldToHex(Vector3 position)
        {
            // Convert world position to local space (if grid is not at origin)
            var localPosition = position - transform.position;

            // Compute axial coordinates
            var rFloat = localPosition.z / (HexConstants.OuterRadius * 1.5f + spacing);
            var qFloat = localPosition.x / (HexConstants.InnerRadius * 2f + spacing) - rFloat * 0.5f;

            // Convert to cube coordinates (q, r, s)
            var sFloat = -qFloat - rFloat;

            // Perform cube rounding
            var q = Mathf.RoundToInt(qFloat);
            var r = Mathf.RoundToInt(rFloat);
            var s = Mathf.RoundToInt(sFloat);

            // Fix rounding errors
            var qDiff = Mathf.Abs(q - qFloat);
            var rDiff = Mathf.Abs(r - rFloat);
            var sDiff = Mathf.Abs(s - sFloat);

            if (qDiff > rDiff && qDiff > sDiff)
            {
                q = -r - s;
            }
            else if (rDiff > sDiff)
            {
                r = -q - s;
            }
            else
            {
                // s = -q - r; Not needed directly but helps for clarity
            }

            return new HexCoordinate(q, r);
        }

        public HexCell GetNearestHexCell(Vector3 worldPosition)
        {
            var closestDistance = Mathf.Infinity;
            HexCell closestCell = null;

            // Iterate over all hex cells and find the one with the minimum distance
            foreach (var hexCell in hexCells)
            {
                var hexWorldPos = hexCell.transform.position;
                var distance = Vector3.Distance(worldPosition, hexWorldPos);

                if (distance >= closestDistance) continue;

                closestDistance = distance;
                closestCell = hexCell;
            }

            return closestCell;
        }

        public HexCell GetNearestHexCellToMousePosition()
        {
            var mouseToWorldPosition = MouseUtils.MouseToWorldPosition(Vector3.up, CameraController.Camera);

            mouseToWorldPosition.y = 0;

            var cell = GetNearestHexCell(mouseToWorldPosition);

            return cell;
        }

        public List<HexCell> GetTissue(HexCoordinate origin, HexCoordinate[] offsets)
        {
            var cellCoordinatesInTissue = offsets.Select(offset =>
                new HexCoordinate(origin.Q + offset.Q, origin.R + offset.R)).ToList();

            return cellCoordinatesInTissue.Select(GetCell).ToList();
        }

        public static HexCoordinate[] RotateHexesClockwise(HexCoordinate origin, HexCoordinate[] hexes)
        {
            var rotatedHexes = new HexCoordinate[hexes.Length];

            for (var i = 0; i < hexes.Length; i++)
            {
                var q = hexes[i].Q - origin.Q;
                var r = hexes[i].R - origin.R;
                var s = -q - r;

                // Apply clockwise hex rotation (60 degrees)
                var newQ = -s;
                var newR = -q;
                // var newS = -r; Not needed directly but helps for clarity

                rotatedHexes[i] = new HexCoordinate(newQ + origin.Q, newR + origin.R);
            }

            return rotatedHexes;
        }
    }
}