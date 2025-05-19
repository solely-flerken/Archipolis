using System.Collections.Generic;
using System.Linq;
using Hex;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEditor;
using UnityEngine;
using Random = Unity.Mathematics.Random;

namespace Terrain
{
    public class MapGenerator : MonoBehaviour
    {
        public static MapGenerator Instance;

        public const float HexRadius = 5f;
        public const bool IsFlatTopped = false;
        
        private ChunkPool _chunkPool;
        
        [SerializeField] private Material chunkMaterial;

        [Header("Chunk Settings")] 
        [SerializeField] private int initialPoolSize = 10;
        [SerializeField] private int expandAmount = 1;
        
        public MapGenerationParameters mapParameters = MapGenerationParameters.Default();

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
                return;
            }

            _chunkPool ??= new ChunkPool(chunkMaterial, transform, initialPoolSize, expandAmount);
        }

        private void OnValidate()
        {
            if (!Application.isPlaying)
            {
                // Defer mesh generation until the next editor frame to avoid conflicts with the Unity initialization.
                EditorApplication.delayCall += () =>
                {
                    if (this == null)
                    {
                        return;
                    }

                    _chunkPool ??= new ChunkPool(chunkMaterial, transform, initialPoolSize, expandAmount);
                    GenerateTerrainMesh();
                };
            }
        }

        public Dictionary<HexCoordinate, HexCellData> GenerateTerrainMesh()
        {
            _chunkPool.ReleaseAllChunks();

            var positions = Util.GenerateHexPositions(mapParameters.gridRadius, HexRadius,
                mapParameters.useCircularShape, mapParameters.circleFactor);
            var hexCount = positions.Count;

            // Prepare global data
            var worldPositions =
                new NativeArray<float3>(positions.Select(x => x.worldPos).ToArray(), Allocator.TempJob);
            var biomeMap = new NativeArray<BiomeType>(hexCount, Allocator.TempJob);
            var heightMap = new NativeArray<float>(hexCount, Allocator.TempJob);

            var mapCenter = Util.CalculateMapCenter(worldPositions);
            var mapRadius = Util.CalculateMapRadius(worldPositions, mapCenter);
            
            var random = new Random(mapParameters.seed);
            var octaveOffsets = new NativeArray<float2>(mapParameters.octaves, Allocator.TempJob);
            for (var i = 0; i < mapParameters.octaves; i++)
            {
                var offset = new float2(
                    random.NextFloat(-100000, 100000),
                    random.NextFloat(-100000, 100000)
                );
                octaveOffsets[i] = offset;
            }
            
            var generateTerrain =
                new MapBiomeGenerator(mapParameters, worldPositions, octaveOffsets, biomeMap, heightMap, mapCenter, mapRadius);
            generateTerrain.Schedule(hexCount, 64).Complete();

            // Prepare hex mesh template data
            var hexTemplate = Util.GenerateHexagonMesh(HexRadius, Color.white, IsFlatTopped);

            var baseVertices =
                new NativeArray<float3>(hexTemplate.vertices.Select(v => (float3)v).ToArray(), Allocator.TempJob);
            var baseTriangles = new NativeArray<int>(hexTemplate.triangles, Allocator.TempJob);

            var verticesPerHex = hexTemplate.vertexCount;
            var trianglesPerHex = hexTemplate.triangles.Length;

            // Create a lookup map for position indices
            var hexIndexLookup = new Dictionary<HexCoordinate, int>(hexCount);
            for (var i = 0; i < hexCount; i++)
            {
                hexIndexLookup[positions[i].hexPos] = i;
            }

            // Group all positions into chunks (for mesh only, because of Unity's vertex limit of 65,535 for one mesh renderer)
            var chunks = GroupHexesIntoChunks(positions, mapParameters.chunkSize);

            foreach (var hexList in chunks.Values)
            {
                var chunkHexCount = hexList.Count;

                if (chunkHexCount == 0)
                {
                    continue;
                }

                var finalVertices = new NativeArray<float3>(chunkHexCount * verticesPerHex, Allocator.TempJob);
                var finalTriangles = new NativeArray<int>(chunkHexCount * trianglesPerHex, Allocator.TempJob);
                var finalColors = new NativeArray<Color>(chunkHexCount * verticesPerHex, Allocator.TempJob);

                var hexIndices = new NativeArray<int>(chunkHexCount, Allocator.TempJob);
                for (var i = 0; i < chunkHexCount; i++)
                {
                    hexIndices[i] = hexIndexLookup[hexList[i].hexPos];
                }

                var generateMesh = new HexMeshJob
                {
                    BiomeMap = biomeMap,
                    Positions = worldPositions,
                    BaseVertices = baseVertices,
                    BaseTriangles = baseTriangles,
                    Vertices = finalVertices,
                    Triangles = finalTriangles,
                    Colors = finalColors,
                    HexIndices = hexIndices,
                };

                generateMesh.Schedule(chunkHexCount, 64).Complete();

                var mesh = generateMesh.ToMesh();

                _chunkPool.GetChunk(mesh);

                // Dispose chunk data
                hexIndices.Dispose();
                finalVertices.Dispose();
                finalTriangles.Dispose();
                finalColors.Dispose();
            }

            var hexMap = new Dictionary<HexCoordinate, HexCellData>(hexCount);
            for (var i = 0; i < hexCount; i++)
            {
                hexMap[positions[i].hexPos] = new HexCellData
                {
                    WorldPosition = positions[i].worldPos,
                    Height = heightMap[i],
                    Biome = biomeMap[i],
                };
            }

            // Dispose global data
            worldPositions.Dispose();
            octaveOffsets.Dispose();
            biomeMap.Dispose();
            heightMap.Dispose();
            baseVertices.Dispose();
            baseTriangles.Dispose();

            return hexMap;
        }

        private static Dictionary<string, List<(HexCoordinate hexPos, float3 worldPos)>> GroupHexesIntoChunks(
            List<(HexCoordinate hexPos, float3 worldPos)> hexList, int chunkSize)
        {
            var chunks = new Dictionary<string, List<(HexCoordinate, float3)>>();

            foreach (var (hex, world) in hexList)
            {
                // Calculate "chunk coordinate"
                var chunkQ = hex.Q / chunkSize;
                var chunkR = hex.R / chunkSize;

                var chunkKey = $"{chunkQ},{chunkR}";

                if (!chunks.TryGetValue(chunkKey, out var chunkList))
                {
                    chunkList = new List<(HexCoordinate, float3)>();
                    chunks[chunkKey] = chunkList;
                }

                chunkList.Add((hex, world));
            }

            return chunks;
        }
    }
}