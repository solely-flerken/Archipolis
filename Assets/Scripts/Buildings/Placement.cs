using HexGrid;
using UnityEngine;
using Utils;

namespace Buildings
{
    public class Placement : MonoBehaviour
    {
        private void Update()
        {
            var worldPosition = MouseUtils.MouseToWorldPosition(Camera.main);
            if (!worldPosition.HasValue) return;

            var hexCoordinate = HexGridManager.Instance.HexGrid.WorldToHex(worldPosition.Value);
            var cell = HexGridManager.Instance.HexGrid.GetCell(hexCoordinate);
            if (!cell) return;

            var mesh = cell.GetComponent<HexMesh>();
            mesh.ChangeColor(new Color(0.2f, 0.8f, 0.4f));
        }
    }
}