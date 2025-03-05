using System.Linq;
using Events;
using HexGrid;
using UnityEngine;
using Utils;

namespace Buildings
{
    public class BuildingManager : MonoBehaviour
    {
        private GameObject _selectedBuilding;

        private static readonly Color BaseOverlay = new(0, 0, 0, 0.0f);
        private static readonly Color InvalidOverlay = new(1, 0, 0, 1f);
        private static readonly Color ValidOverlay = new(0, 1, 0, 1f);

        private void Start()
        {
            EventSystem.Instance.OnClickableClick += HandleBuildingClick;
            EventSystem.Instance.OnKeyR += HandleBuildingRotate;
        }

        private void Update()
        {
            if (!_selectedBuilding) return;

            var mouseToWorldPosition = MouseUtils.MouseToWorldPosition(Camera.main);

            if (mouseToWorldPosition is not { } mousePosition) return;

            mousePosition.y = 0;

            var cell = HexGridManager.HexGrid.GetNearestHexCell(mousePosition);
            _selectedBuilding.transform.position = cell.transform.position;

            // Check placement validity
            var currentHexCoordinate = cell.HexCoordinate;

            if (!_selectedBuilding.TryGetComponent<Building>(out var building))
            {
                return;
            }

            var footprint = building.Footprint;
            var adjacentHexCoordinates = footprint.Select(offset =>
                new HexCoordinate(currentHexCoordinate.Q + offset.Q, currentHexCoordinate.R + offset.R)).ToList();

            var isInvalidPlacement = adjacentHexCoordinates
                .Select(adjacentHex => HexGridManager.HexGrid.GetCell(adjacentHex))
                .Any(adjacentCell => adjacentCell is null || adjacentCell.Occupied);

            ColorBasedOnValidity(isInvalidPlacement, building);
        }

        private void OnDestroy()
        {
            EventSystem.Instance.OnClickableClick -= HandleBuildingClick;
            EventSystem.Instance.OnKeyR -= HandleBuildingRotate;
        }

        private void HandleBuildingRotate(GameObject obj)
        {
            if (_selectedBuilding is null) return;

            if (_selectedBuilding.TryGetComponent<Building>(out var building))
            {
                building.RotateBuilding();
            }
        }

        private void HandleBuildingClick(GameObject obj)
        {
            if (_selectedBuilding is not null)
            {
                EventSystem.Instance.InvokeBuildingPlaced(_selectedBuilding);
                _selectedBuilding = null;
            }
            else if (obj.TryGetComponent<Building>(out var newSelectedBuilding))
            {
                _selectedBuilding = newSelectedBuilding.gameObject;
            }
        }

        private static void ColorBasedOnValidity(bool isInvalid, Building building)
        {
            building.SetColor(isInvalid ? InvalidOverlay : BaseOverlay);
        }
    }
}