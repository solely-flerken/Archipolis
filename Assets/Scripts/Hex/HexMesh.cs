using System.Collections.Generic;
using UnityEngine;

namespace Hex
{
    [RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
    public class HexMesh : MonoBehaviour
    {
        private Mesh _hexMesh;
        private List<Vector3> _vertices;
        private List<int> _triangles;

        private void Awake()
        {
            Initialize();
            GenerateHexMesh();
            ApplyMesh();
            AddMeshCollider();
        }

        private void Initialize()
        {
            _hexMesh = new Mesh
            {
                name = "HexMesh"
            };
            _vertices = new List<Vector3>();
            _triangles = new List<int>();

            // Assign to MeshFilter
            GetComponent<MeshFilter>().mesh = _hexMesh;
        }

        private void GenerateHexMesh()
        {
            _vertices.Add(Vector3.zero);

            // Generate vertices for a pointy-topped hexagon
            for (var i = 0; i < 6; i++)
            {
                // Calculate vertex positions
                var angleDeg = 30 + 60 * i;
                var angleRad = Mathf.Deg2Rad * angleDeg;

                _vertices.Add(new Vector3(
                    HexConstants.OuterRadius * Mathf.Cos(angleRad),
                    0,
                    HexConstants.OuterRadius * Mathf.Sin(angleRad)
                ));
            }

            // Create triangles in counter-clockwise order, because of backface culling
            for (var i = 0; i < 6; i++)
            {
                _triangles.Add(0);
                _triangles.Add(i + 2 > 6 ? 1 : i + 2);
                _triangles.Add(i + 1);
            }
        }

        private void ApplyMesh()
        {
            _hexMesh.Clear();

            _hexMesh.vertices = _vertices.ToArray();
            _hexMesh.triangles = _triangles.ToArray();

            _hexMesh.RecalculateNormals();
            _hexMesh.RecalculateBounds();
        }

        private void AddMeshCollider()
        {
            var meshCollider = GetComponent<MeshCollider>();
            if (meshCollider == null)
            {
                meshCollider = gameObject.AddComponent<MeshCollider>();
            }
            meshCollider.sharedMesh = _hexMesh; 
        }
        
        public void ChangeColor(Color newColor)
        {
            var meshRenderer = GetComponent<MeshRenderer>();
            if (meshRenderer)
            {
                meshRenderer.material.color = newColor;
            }
        }
    }
}