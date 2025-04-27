using System.Collections.Generic;
using System.Linq;
using Hex;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEditor;
using UnityEngine;

namespace Terrain
{
    [RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
    public class MapGenerator : MonoBehaviour
    {
        public static MapGenerator Instance;

        public Dictionary<HexCoordinate, HexCellData> HexMap { get; private set; } = new();

        public const float HexRadius = 5f;
        public const bool IsFlatTopped = false;
        [SerializeField] private int gridRadius = 10;
        [SerializeField] private bool useCircularShape;
        [SerializeField, Range(1f, 2f)] private float circleFactor = 1.5f;

        public IslandGenerationParameters islandParameters;

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
        }

        private void OnValidate()
        {
            if (!Application.isPlaying)
            {
                // Defer mesh generation until the next editor frame to avoid conflicts with the Unity initialization.
                EditorApplication.delayCall += () =>
                {
                    GenerateTerrainMesh();
                };
            }
        }

        public Dictionary<HexCoordinate, HexCellData> GenerateTerrainMesh()
        {
            var positions = Util.GenerateHexPositions(gridRadius, HexRadius, useCircularShape, circleFactor);
            var hexCount = positions.Count;

            var worldPositions =
                new NativeArray<float3>(positions.Select(x => x.worldPos).ToArray(), Allocator.TempJob);
            var biomeMap = new NativeArray<BiomeType>(hexCount, Allocator.TempJob);
            var heightMap = new NativeArray<float>(hexCount, Allocator.TempJob);

            var mapCenter = Util.CalculateMapCenter(worldPositions);
            var mapRadius = Util.CalculateMapRadius(worldPositions, mapCenter);

            var generateTerrain =
                new IslandBiomeGenerator(islandParameters, worldPositions, biomeMap, heightMap, mapCenter, mapRadius);
            generateTerrain.Schedule(hexCount, 64).Complete();

            // Template data
            var hexTemplate = Util.GenerateHexagonMesh(HexRadius, Color.white, IsFlatTopped);
            var verticesPerHex = hexTemplate.vertexCount;
            var trianglesPerHex = hexTemplate.triangles.Length;

            // Base data
            var baseVertices =
                new NativeArray<float3>(hexTemplate.vertices.Select(v => (float3)v).ToArray(), Allocator.TempJob);
            var baseTriangles = new NativeArray<int>(hexTemplate.triangles, Allocator.TempJob);
            var baseColors = new NativeArray<Color>(hexTemplate.colors.ToArray(), Allocator.TempJob);

            // Final mesh data
            var finalVertices = new NativeArray<float3>(hexCount * verticesPerHex, Allocator.TempJob);
            var finalTriangles = new NativeArray<int>(hexCount * trianglesPerHex, Allocator.TempJob);
            var finalColors = new NativeArray<Color>(hexCount * verticesPerHex, Allocator.TempJob);

            var meshJob = new HexMeshJob
            {
                BiomeMap = biomeMap,
                Positions = worldPositions,
                HeightMap = heightMap,
                BaseVertices = baseVertices,
                BaseTriangles = baseTriangles,
                BaseColors = baseColors,
                Vertices = finalVertices,
                Triangles = finalTriangles,
                Colors = finalColors,
            };

            meshJob.Schedule(hexCount, 64).Complete();
            var mesh = meshJob.ToMesh();
            GetComponent<MeshFilter>().mesh = mesh;

            var hexMap = new Dictionary<HexCoordinate, HexCellData>();
            for (var i = 0; i < hexCount; i++)
            {
                var hexCoordinate = positions[i].hexPos;
                hexMap[hexCoordinate] = new HexCellData
                {
                    WorldPosition = positions[i].worldPos,
                    Height = heightMap[i],
                    Biome = biomeMap[i],
                };
            }

            // Dispose all
            worldPositions.Dispose();
            biomeMap.Dispose();
            heightMap.Dispose();
            baseVertices.Dispose();
            baseTriangles.Dispose();
            baseColors.Dispose();
            finalVertices.Dispose();
            finalTriangles.Dispose();
            finalColors.Dispose();

            return hexMap;
        }
    }
}