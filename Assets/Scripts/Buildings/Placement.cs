using Hex;
using UnityEngine;

namespace Buildings
{
    public class Placement : MonoBehaviour
    {
        private static readonly Color CellOccupied = new(0.8f, 0.3f, 0.5f);
        private static readonly Color CellPreview = new(0f, 1f, 0f, 0.5f);
        private static readonly Color CellBase = new(0f, 0f, 0f, 0f);

        private void Update()
        {
            var cells = HexGridManager.Instance.HexGrid.hexCells;

            foreach (var cell in cells)
            {
                if (!cell.TryGetComponent<HexMesh>(out var mesh)) continue;

                if (cell.Occupied)
                {
                    mesh.ChangeColor(CellOccupied);
                }
                else if (cell.Preview)
                {
                    mesh.ChangeColor(CellPreview);
                }
                else
                {
                    mesh.ChangeColor(CellBase);
                }
            }
        }
    }
}