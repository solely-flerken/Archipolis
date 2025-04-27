using System.Collections.Generic;
using Hex;
using Terrain;
using UnityEngine;

namespace Preview
{
    public class PreviewManager : MonoBehaviour
    {
        public static PreviewManager Instance;

        private MeshFilter _meshFilter;
        private MeshRenderer _meshRenderer;
        private Mesh _previewMesh;
        private Mesh _hexTemplate;

        [SerializeField] private Material previewMaterial;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
            }

            _meshFilter = gameObject.AddComponent<MeshFilter>();
            _meshRenderer = gameObject.AddComponent<MeshRenderer>();

            _hexTemplate = Util.GenerateHexagonMesh(MapGenerator.HexRadius, Color.white, MapGenerator.IsFlatTopped);

            _previewMesh = new Mesh();
            _meshFilter.mesh = _previewMesh;

            _meshRenderer.material = previewMaterial;

            // Position the preview slightly above the terrain to avoid z-fighting
            transform.position = new Vector3(0, 0.05f, 0);
        }

        public void SetPreview(List<HexCoordinate> previewTiles)
        {
            _previewMesh = GeneratePreviewMesh(previewTiles, _hexTemplate);
            _meshFilter.mesh = _previewMesh;
        }

        public void ClearPreview()
        {
            _previewMesh.Clear();
        }

        private static Mesh GeneratePreviewMesh(List<HexCoordinate> previewTiles, Mesh hexTemplate)
        {
            var vertices = new List<Vector3>();
            var triangles = new List<int>();

            var indexOffset = 0;

            foreach (var tile in previewTiles)
            {
                var position = HexMapManager.Instance.HexMap[tile].WorldPosition;
                AddHexagonToMesh(hexTemplate, position, ref vertices, ref triangles, indexOffset);
                indexOffset += hexTemplate.vertexCount;
            }

            var mesh = new Mesh();
            mesh.SetVertices(vertices);
            mesh.SetTriangles(triangles, 0);
            mesh.RecalculateNormals();

            return mesh;
        }

        private static void AddHexagonToMesh(Mesh hexTemplate, Vector3 position, ref List<Vector3> vertices,
            ref List<int> triangles,
            int indexOffset)
        {
            for (var i = 0; i < hexTemplate.vertexCount; i++)
            {
                vertices.Add(hexTemplate.vertices[i] + position);
            }

            for (var i = 0; i < hexTemplate.triangles.Length; i++)
            {
                triangles.Add(hexTemplate.triangles[i] + indexOffset);
            }
        }
    }
}