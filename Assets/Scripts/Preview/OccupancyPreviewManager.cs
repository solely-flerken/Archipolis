using System.Collections.Generic;
using System.Linq;
using Hex;
using Terrain;
using UnityEngine;

namespace Preview
{
    // TODO: Maybe redo this. We use GPU instancing and need to draw every frame. Instead we could dynamically calculate the mesh by removing/adding parts (hexes) of the mesh
    public class OccupancyPreviewManager : MonoBehaviour
    {
        public static OccupancyPreviewManager Instance;

        private readonly Dictionary<HexCoordinate, Matrix4x4> _previewHexes = new();

        private Mesh _hexTemplate;

        [SerializeField] private Material occupancyPreviewMaterial;

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

            _hexTemplate = Util.GenerateHexagonMesh(MapGenerator.HexRadius, Color.white, MapGenerator.IsFlatTopped);

            // Position the preview slightly above the terrain to avoid z-fighting
            transform.position = new Vector3(0, 0.05f, 0);
        }

        private void Update()
        {
            Graphics.DrawMeshInstanced(_hexTemplate, 0, occupancyPreviewMaterial, _previewHexes.Values.ToArray(),
                _previewHexes.Count);
        }

        public void AddPreviewHex(HexCoordinate coordinate)
        {
            _previewHexes[coordinate] = GetMatrix(coordinate);
        }

        public void AddPreviewHexes(List<HexCoordinate> coordinates)
        {
            coordinates.ForEach(AddPreviewHex);
        }

        public void RemovePreviewHex(HexCoordinate coordinate)
        {
            _previewHexes.Remove(coordinate);
        }

        public void RemovePreviewHexes(List<HexCoordinate> coordinates)
        {
            coordinates.ForEach(RemovePreviewHex);
        }

        public void ClearPreviewHexes()
        {
            _previewHexes.Clear();
        }

        private static Matrix4x4 GetMatrix(HexCoordinate coordinate)
        {
            var position = HexMapManager.Instance.HexMap[coordinate].WorldPosition;
            var quaternion = Quaternion.identity;
            var scale = Vector3.one;

            return Matrix4x4.TRS(position, quaternion, scale);
        }
    }
}