using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

namespace Terrain
{
    [BurstCompile]
    public struct HexMeshJob : IJobParallelFor
    {
        [ReadOnly] public NativeArray<float3> Positions;
        [ReadOnly] public NativeArray<BiomeType> BiomeMap;
        [ReadOnly] public NativeArray<float> HeightMap;

        [ReadOnly] public NativeArray<float3> BaseVertices;
        [ReadOnly] public NativeArray<int> BaseTriangles;
        [ReadOnly] public NativeArray<Color> BaseColors;

        [NativeDisableParallelForRestriction] public NativeArray<float3> Vertices;
        [NativeDisableParallelForRestriction] public NativeArray<int> Triangles;
        [NativeDisableParallelForRestriction] public NativeArray<Color> Colors;

        // Color mapping for visualization
        private static readonly Color DeepOceanColor = new(0.1f, 0.1f, 0.4f);
        private static readonly Color ShallowOceanColor = new(0.2f, 0.2f, 0.6f);
        private static readonly Color BeachColor = new(0.9f, 0.9f, 0.6f);
        private static readonly Color PlainsColor = new(0.5f, 0.8f, 0.5f);
        private static readonly Color ForestColor = new(0.2f, 0.6f, 0.2f);
        private static readonly Color HillsColor = new(0.5f, 0.5f, 0.3f);
        private static readonly Color MountainColor = new(0.7f, 0.7f, 0.7f);
        private static readonly Color PeakColor = new(0.9f, 0.9f, 0.9f);

        public void Execute(int index)
        {
            var position = Positions[index];
            var color = GetBiomeColor(BiomeMap[index]);

            var vertStart = index * BaseVertices.Length;
            var triStart = index * BaseTriangles.Length;

            for (var i = 0; i < BaseVertices.Length; i++)
            {
                Vertices[vertStart + i] = BaseVertices[i] + position;

                Colors[vertStart + i] = color;
            }

            for (var i = 0; i < BaseTriangles.Length; i++)
            {
                Triangles[triStart + i] = BaseTriangles[i] + vertStart;
            }
        }

        private static Color GetBiomeColor(BiomeType biome)
        {
            return biome switch
            {
                BiomeType.DeepOcean => DeepOceanColor,
                BiomeType.ShallowOcean => ShallowOceanColor,
                BiomeType.Beach => BeachColor,
                BiomeType.Plains => PlainsColor,
                BiomeType.Forest => ForestColor,
                BiomeType.Hills => HillsColor,
                BiomeType.Mountains => MountainColor,
                BiomeType.Peaks => PeakColor,
                _ => Color.white
            };
        }

        public Mesh ToMesh()
        {
            var mesh = new Mesh();
            mesh.SetVertices(Vertices.Reinterpret<Vector3>().ToArray());
            mesh.SetTriangles(Triangles.ToArray(), 0);
            mesh.SetColors(Colors.ToArray());
            mesh.RecalculateNormals();
            return mesh;
        }
    }
}