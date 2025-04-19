using System.Collections.Generic;
using System.Linq;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEditor;
using UnityEngine;

namespace Terrain
{
    [RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
    public class TerrainMeshGenerator : MonoBehaviour
    {
        public static TerrainMeshGenerator Instance;

        [SerializeField] private float hexRadius = 1f;
        [SerializeField] private int gridRadius = 10;
        [SerializeField] private bool useCircularShape;

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
                EditorApplication.delayCall += GenerateTerrainMesh;
            }
        }

        private void GenerateTerrainMesh()
        {
            var hexPositions = GenerateHexPositions();
            var hexCount = hexPositions.Count;

            var hexTemplate = GenerateHexMesh(hexRadius, Color.white);
            var verticesPerHex = hexTemplate.vertexCount;
            var trianglesPerHex = hexTemplate.triangles.Length;

            // Base data
            var baseVertices =
                new NativeArray<float3>(hexTemplate.vertices.Select(v => (float3)v).ToArray(), Allocator.TempJob);
            var baseTriangles = new NativeArray<int>(hexTemplate.triangles, Allocator.TempJob);
            var baseColors = new NativeArray<Color>(hexTemplate.colors.ToArray(), Allocator.TempJob);

            // Positions of individual hex cells
            var positions = new NativeArray<float3>(hexPositions.ToArray(), Allocator.TempJob);

            // Final mesh data
            var finalVertices = new NativeArray<float3>(hexCount * verticesPerHex, Allocator.TempJob);
            var finalTriangles = new NativeArray<int>(hexCount * trianglesPerHex, Allocator.TempJob);
            var finalColors = new NativeArray<Color>(hexCount * verticesPerHex, Allocator.TempJob);

            var meshJob = new HexMeshJob
            {
                Positions = positions,
                BaseVertices = baseVertices,
                BaseTriangles = baseTriangles,
                BaseColors = baseColors,
                Vertices = finalVertices,
                Triangles = finalTriangles,
                Colors = finalColors,
            };

            meshJob.Schedule(hexCount, 64).Complete();

            GetComponent<MeshFilter>().mesh = meshJob.DisposeAndGetMesh();
        }

        [BurstCompile]
        private struct HexMeshJob : IJobParallelFor
        {
            [ReadOnly] public NativeArray<float3> Positions;

            [ReadOnly] public NativeArray<float3> BaseVertices;
            [ReadOnly] public NativeArray<int> BaseTriangles;
            [ReadOnly] public NativeArray<Color> BaseColors;

            [NativeDisableParallelForRestriction] public NativeArray<float3> Vertices;
            [NativeDisableParallelForRestriction] public NativeArray<int> Triangles;
            [NativeDisableParallelForRestriction] public NativeArray<Color> Colors;

            public void Execute(int index)
            {
                var hexOffset = Positions[index];

                var vertStart = index * BaseVertices.Length;
                var triStart = index * BaseTriangles.Length;

                for (var i = 0; i < BaseVertices.Length; i++)
                {
                    Vertices[vertStart + i] = BaseVertices[i] + hexOffset;
                    Colors[vertStart + i] = BaseColors[i];
                }

                for (var i = 0; i < BaseTriangles.Length; i++)
                {
                    Triangles[triStart + i] = BaseTriangles[i] + vertStart;
                }
            }

            public Mesh DisposeAndGetMesh()
            {
                var mesh = new Mesh();

                mesh.SetVertices(Vertices.Reinterpret<Vector3>().ToArray());
                mesh.SetTriangles(Triangles.ToArray(), 0);
                mesh.SetColors(Colors.ToArray());
                mesh.RecalculateNormals();

                Positions.Dispose();
                BaseVertices.Dispose();
                BaseTriangles.Dispose();
                BaseColors.Dispose();
                Vertices.Dispose();
                Triangles.Dispose();
                Colors.Dispose();

                return mesh;
            }
        }

        private List<float3> GenerateHexPositions()
        {
            var hexPositions = new List<float3>();

            for (var q = -gridRadius; q <= gridRadius; q++)
            {
                var minR = Mathf.Max(-gridRadius, -q - gridRadius);
                var maxR = Mathf.Min(gridRadius, -q + gridRadius);

                for (var r = minR; r <= maxR; r++)
                {
                    var pos = HexToWorldPosition(hexRadius, q, r);

                    if (useCircularShape)
                    {
                        if (math.length(pos) <= gridRadius)
                        {
                            hexPositions.Add(pos);
                        }
                    }
                    else
                    {
                        var s = -q - r;
                        var hexDistance = (math.abs(q) + math.abs(r) + math.abs(s)) / 2;

                        if (hexDistance * hexRadius <= gridRadius * 1.1f)
                        {
                            hexPositions.Add(pos);
                        }
                    }
                }
            }

            return hexPositions;
        }

        private static float3 HexToWorldPosition(float radius, int q, int r)
        {
            var x = radius * 1.5f * q;
            var z = radius * math.sqrt(3f) * (r + q * 0.5f);
            return new float3(x, 0f, z);
        }

        private static Mesh GenerateHexMesh(float radius, Color color)
        {
            var mesh = new Mesh();

            var vertices = new List<Vector3> { Vector3.zero };
            var triangles = new List<int>();
            var colors = new List<Color> { color };

            for (var i = 0; i < 6; i++)
            {
                float angleDeg = 60 * i;
                var angleRad = Mathf.Deg2Rad * angleDeg;
                vertices.Add(new Vector3(Mathf.Cos(angleRad), 0, Mathf.Sin(angleRad)) * radius);
                colors.Add(color);
            }

            for (var i = 0; i < 6; i++)
            {
                triangles.Add(0);
                triangles.Add(i + 2 > 6 ? 1 : i + 2);
                triangles.Add(i + 1);
            }

            mesh.vertices = vertices.ToArray();
            mesh.triangles = triangles.ToArray();
            mesh.colors = colors.ToArray();
            mesh.RecalculateNormals();

            return mesh;
        }
    }
}