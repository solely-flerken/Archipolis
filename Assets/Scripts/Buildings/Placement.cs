using HexGrid;
using UnityEngine;
using Utils;

namespace Buildings
{
    public class Placement : MonoBehaviour
    {
        private HexGrid.HexGrid _hexGrid;

        private void Start()
        {
            _hexGrid = HexGridManager.HexGrid;
        }

        private void Update()
        {
            var worldPosition = MouseUtils.MouseToWorldPosition(Camera.main);
            if (!worldPosition.HasValue) return;

            var hexCoordinate = _hexGrid.WorldToHex(worldPosition.Value);
            var cell = _hexGrid.GetCell(hexCoordinate);
            if (!cell) return;

            var mesh = cell.GetComponent<HexMesh>();
            mesh.ChangeColor(new Color(0.2f, 0.8f, 0.4f));
        }
    }
}