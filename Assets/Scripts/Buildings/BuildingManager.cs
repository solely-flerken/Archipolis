using System.Linq;
using Events;
using HexGrid;
using UnityEngine;

namespace Buildings
{
    public class BuildingManager : MonoBehaviour
    {
        public static BuildingManager Instance { get; private set; }

        private GameObject _selectedObject;

        private static readonly Color BaseOverlay = new(0, 0, 0, 0.0f);
        private static readonly Color InvalidOverlay = new(1, 0, 0, 1f);
        private static readonly Color ValidOverlay = new(0, 1, 0, 1f);

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

        private void Start()
        {
            EventSystem.Instance.OnBuildingClick += HandleBuildingClick;
            EventSystem.Instance.OnBuildingPlaced += HandleBuildingPlaced;
            EventSystem.Instance.OnKeyR += HandleBuildingRotate;
        }

        private void Update()
        {
            if (!_selectedObject) return;

            var cell = HexGridManager.Instance.HexGrid.GetNearestHexCellToMousePosition();
            if (cell is null) return;

            // Snap to grid
            _selectedObject.transform.position = cell.transform.position;

            if (!_selectedObject.TryGetComponent<Building>(out var building))
            {
                return;
            }

            // Check placement validity
            var isValidPlacement = IsPlacementValid(building);

            // Overlay color based on placement validity
            ColorBasedOnValidity(isValidPlacement, building);
        }

        private void OnDestroy()
        {
            EventSystem.Instance.OnBuildingClick -= HandleBuildingClick;
            EventSystem.Instance.OnBuildingPlaced -= HandleBuildingPlaced;
            EventSystem.Instance.OnKeyR -= HandleBuildingRotate;
        }

        private void HandleBuildingClick(GameObject obj)
        {
            if (_selectedObject is not null)
            {
                EventSystem.Instance.InvokeBuildingPlaced(_selectedObject);
            }
            else
            {
                _selectedObject = obj;
            }
        }

        private void HandleBuildingPlaced(GameObject obj)
        {
            if (!_selectedObject.TryGetComponent<Building>(out var building)) return;


            if (!IsPlacementValid(building)) return;

            // Only place the building if placement is valid
            _selectedObject = null;
        }

        private void HandleBuildingRotate(GameObject obj)
        {
            if (_selectedObject is null) return;

            if (_selectedObject.TryGetComponent<Building>(out var building))
            {
                building.RotateBuilding();
            }
        }

        private static void ColorBasedOnValidity(bool isValid, Building building)
        {
            building.SetColor(isValid ? BaseOverlay : InvalidOverlay);
        }

        private static bool IsPlacementValid(Building building)
        {
            var cell = HexGridManager.Instance.HexGrid.GetNearestHexCell(building.transform.position);
            if (cell is null) return false;

            var coordinate = cell.HexCoordinate;
            var footprint = building.Footprint;
            var adjacentHexCoordinates = footprint.Select(offset =>
                new HexCoordinate(coordinate.Q + offset.Q, coordinate.R + offset.R)).ToList();

            var isInvalidPlacement = adjacentHexCoordinates
                .Select(adjacentHex => HexGridManager.Instance.HexGrid.GetCell(adjacentHex))
                .Any(adjacentCell => adjacentCell is null || adjacentCell.Occupied);

            return !isInvalidPlacement;
        }
    }
}