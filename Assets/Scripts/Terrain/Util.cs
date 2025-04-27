using System.Collections.Generic;
using Hex;
using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;

namespace Terrain
{
    public static class Util
    {
        public static float3 HexToWorldPosition(float radius, int q, int r, bool flatTopped)
        {
            if (flatTopped)
            {
                // Flat-topped (X axis wider)
                var x = radius * 1.5f * q;
                var z = radius * math.sqrt(3f) * (r + q * 0.5f);
                return new float3(x, 0f, z);
            }
            else
            {
                // Pointy-topped (Z axis wider)
                var x = radius * math.sqrt(3f) * (q + r * 0.5f);
                var z = radius * 1.5f * r;
                return new float3(x, 0f, z);
            }
        }

        public static float3 WorldToHexPosition(float radius, float3 position)
        {
            var innerRadius = radius * 0.866025404f;

            // Scale factors for conversion
            var q = position.x / (1.5f * radius);
            var r = position.z / (innerRadius * 2f) - q * 0.5f;
            var s = -q - r;

            // Round to get grid coordinates
            var qi = (int)math.round(q);
            var ri = (int)math.round(r);
            var si = (int)math.round(s);

            // Fix any rounding errors
            var qDiff = math.abs(qi - q);
            var rDiff = math.abs(ri - r);
            var sDiff = math.abs(si - s);

            if (qDiff > rDiff && qDiff > sDiff)
            {
                qi = -ri - si;
            }
            else if (rDiff > sDiff)
            {
                ri = -qi - si;
            }

            return new float3(qi, ri, si);
        }

        public static float2 CalculateMapCenter(NativeArray<float3> positions)
        {
            var sum = float2.zero;
            for (var i = 0; i < positions.Length; i++)
            {
                sum += new float2(positions[i].x, positions[i].z);
            }

            return sum / positions.Length;
        }

        public static float CalculateMapRadius(NativeArray<float3> positions, float2 center)
        {
            var maxDistSq = 0f;
            for (var i = 0; i < positions.Length; i++)
            {
                var pos = new float2(positions[i].x, positions[i].z);
                var distSq = math.distancesq(pos, center);
                maxDistSq = math.max(maxDistSq, distSq);
            }

            return math.sqrt(maxDistSq);
        }

        public static List<(HexCoordinate hexPos, float3 worldPos)> GenerateHexPositions(int gridRadius,
            float hexRadius, bool useCircularShape, float circleFactor)
        {
            var hexPositions = new List<(HexCoordinate hexPos, float3 worldPos)>();

            for (var q = -gridRadius; q <= gridRadius; q++)
            {
                var minR = Mathf.Max(-gridRadius, -q - gridRadius);
                var maxR = Mathf.Min(gridRadius, -q + gridRadius);

                for (var r = minR; r <= maxR; r++)
                {
                    var hexPos = new HexCoordinate(q, r);
                    var worldPos = HexToWorldPosition(hexRadius, q, r, MapGenerator.IsFlatTopped);

                    if (useCircularShape)
                    {
                        var maxDistance = gridRadius * hexRadius * circleFactor;
                        var maxDistanceSq = maxDistance * maxDistance;

                        if (math.lengthsq(worldPos) <= maxDistanceSq)
                        {
                            hexPositions.Add((hexPos, worldPos));
                        }
                    }
                    else
                    {
                        var s = -q - r;
                        var hexDistance = (math.abs(q) + math.abs(r) + math.abs(s)) / 2;

                        if (hexDistance <= gridRadius)
                        {
                            hexPositions.Add((hexPos, worldPos));
                        }
                    }
                }
            }

            return hexPositions;
        }

        public static Mesh GenerateHexagonMesh(float radius, Color color, bool flatTopped)
        {
            var mesh = new Mesh();

            var vertices = new List<Vector3> { Vector3.zero };
            var triangles = new List<int>();
            var colors = new List<Color> { color };

            var angleOffset = flatTopped ? 0f : 30f; // 0° for flat, 30° for pointy

            for (var i = 0; i < 6; i++)
            {
                var angleDeg = 60 * i + angleOffset;
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