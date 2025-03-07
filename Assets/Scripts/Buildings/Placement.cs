using System;
using Events;
using Hex;
using UnityEngine;

namespace Buildings
{
    public class Placement : MonoBehaviour
    {
        private void Start()
        {
            EventSystem.Instance.OnBuildingPlaced += CheckHexGrid;
        }

        public void OnDestroy()
        {
            EventSystem.Instance.OnBuildingPlaced -= CheckHexGrid;
        }
        
        private static void CheckHexGrid(GameObject obj)
        {
            var cells = HexGridManager.Instance.HexGrid.hexCells;

            foreach (var cell in cells)
            {
                if(!cell.TryGetComponent<HexMesh>(out var mesh)) continue;
                mesh.ChangeColor(cell.Occupied ? new Color(0.8f, 0.3f, 0.5f) : new Color(0f, 0f, 0f, 0f));
            }
        }
    }
}