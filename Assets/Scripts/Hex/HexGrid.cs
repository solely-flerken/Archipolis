using System;
using System.Collections.Generic;
using System.Linq;
using Input;
using Terrain;
using Unity.Mathematics;
using UnityEngine;
using Utils;

namespace Hex
{
    public class HexGrid : MonoBehaviour
    {
        public float spacing;

        [Obsolete("Restructured the HexGrid to use a data driven architecture")]
        public HexCell hexCellPrefab;

        [Obsolete("Restructured the HexGrid to use a data driven architecture")]
        public List<HexCell> hexCells = new();

        public Dictionary<HexCoordinate, HexCellData> HexMap { get; set; } = new();

        private static readonly (int q, int r)[] NeighborOffsets =
        {
            (1, 0), (0, 1), (-1, 1),
            (-1, 0), (0, -1), (1, -1)
        };

        [Obsolete("Restructured the HexGrid to use a data driven architecture")]
        public HexCell GetCell(HexCoordinate coordinate)
        {
            return hexCells.Find(cell => cell.HexCoordinate.Q == coordinate.Q && cell.HexCoordinate.R == coordinate.R);
        }    

        [Obsolete("Restructured the HexGrid to use a data driven architecture")]
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

        [Obsolete("Restructured the HexGrid to use a data driven architecture")]
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

        [Obsolete("Restructured the HexGrid to use a data driven architecture")]
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

        public List<HexCoordinate> GetNeighbors(HexCoordinate coordinate)
        {
            var neighbors = new List<HexCoordinate>();

            foreach (var (dq, dr) in NeighborOffsets)
            {
                var neighborCoordinate = new HexCoordinate(coordinate.Q + dq, coordinate.R + dr);
                if (HexMap.ContainsKey(neighborCoordinate))
                {
                    neighbors.Add(neighborCoordinate);
                }
            }

            return neighbors;
        }

        [Obsolete("Restructured the HexGrid to use a data driven architecture")]
        public Vector3 HexToWorld(HexCoordinate coordinate)
        {
            var cell = GetCell(coordinate);
            return cell.gameObject.transform.position;
        }

        // TODO: Refactor: Convert worldPosition into Hex Pos and then look for that in HexMap
        public HexCoordinate? GetNearestHexCoordinate(Vector3 worldPosition)
        {
            var closestDistance = float.PositiveInfinity;
            HexCoordinate? closestCoordinate = null;

            foreach (var (coordinate, cellData) in HexMap)
            {
                var distance = math.distance(worldPosition, cellData.WorldPosition);

                if (distance >= closestDistance) continue;

                closestDistance = distance;
                closestCoordinate = coordinate;
            }

            return closestCoordinate;
        }

        [Obsolete("Restructured the HexGrid to use a data driven architecture")]
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

        [Obsolete("Restructured the HexGrid to use a data driven architecture")]
        public HexCell GetNearestHexCellToMousePosition()
        {
            var mouseToWorldPosition = MouseUtils.MouseToWorldPosition(Vector3.up, CameraController.Camera);
        
            mouseToWorldPosition.y = 0;
        
            var cell = GetNearestHexCell(mouseToWorldPosition);
        
            return cell;
        }

        public HexCoordinate? GetNearestHexCoordinateToMousePosition()
        {
            var mouseWorldPosition = MouseUtils.MouseToWorldPosition(Vector3.up, CameraController.Camera);
            mouseWorldPosition.y = 0f;

            return GetNearestHexCoordinate(mouseWorldPosition);
        }

        [Obsolete("Restructured the HexGrid to use a data driven architecture")]
        public List<HexCell> GetTissueObsolete(HexCoordinate origin, HexCoordinate[] offsets)
        {
            var cellCoordinatesInTissue = offsets.Select(offset =>
                new HexCoordinate(origin.Q + offset.Q, origin.R + offset.R)).ToList();
        
            return cellCoordinatesInTissue.Select(GetCell).ToList();
        }

        public static List<HexCoordinate> GetTissue(HexCoordinate origin, HexCoordinate[] offsets)
        {
            var tissueCoordinates = new List<HexCoordinate>(offsets.Length);
            tissueCoordinates.AddRange(offsets
                .Select(offset => new HexCoordinate(origin.Q + offset.Q, origin.R + offset.R)));
                // .Where(coordinate => HexMap.ContainsKey(coordinate)));

            return tissueCoordinates;
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

                rotatedHexes[i] = new HexCoordinate(newQ + origin.Q, newR + origin.R);
            }

            return rotatedHexes;
        }
    }
}