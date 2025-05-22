using System.Collections.Generic;
using System.Linq;
using Hex;

namespace Terrain
{
    public static class PostProcessingUtil
    {
        public static List<HexCoordinate> GetBiomeTissue(this Dictionary<HexCoordinate, HexCellData> hexMap, HexCoordinate coordinate)
        {
            var cellsToProcess = new Queue<HexCoordinate>();
            var cellsProcessed = new HashSet<HexCoordinate>();

            if (!hexMap.TryGetValue(coordinate, out var initialCell))
                return new List<HexCoordinate>();

            var initialBiome = initialCell.Biome;
            cellsToProcess.Enqueue(coordinate);

            while (cellsToProcess.Count > 0)
            {
                var current = cellsToProcess.Dequeue();
                if (!cellsProcessed.Add(current))
                    continue;

                var neighbors = hexMap.GetNeighbors(current);
                foreach (var neighbor in neighbors)
                {
                    var exists = hexMap.TryGetValue(neighbor, out var data);
                    
                    if (!exists) continue;
                    if (data.Biome != initialBiome) continue;
                    if (cellsProcessed.Contains(neighbor)) continue;

                    cellsToProcess.Enqueue(neighbor);
                }
            }

            return cellsProcessed.ToList();
        }

        public static BiomeType GetDominantNeighboringBiomeType(this Dictionary<HexCoordinate, HexCellData> hexMap, HexCoordinate coordinate)
        {
            return BiomeType.Unknown;
        }
    }
}