using System.Linq;
using Events;
using Hex;
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
            EventSystem.Instance.OnKeyR += HandleBuildingRotate;
        }

        private void Update()
        {
            if (!_selectedObject) return;

            var cell = HexGridManager.Instance.hexGrid.GetNearestHexCellToMousePosition();
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
            EventSystem.Instance.OnKeyR -= HandleBuildingRotate;
        }

        // TODO: Refactor this
        private void HandleBuildingClick(GameObject obj)
        {
            if (_selectedObject is not null)
            {
                // Try place object. Only place the building if placement is valid
                if (!_selectedObject.TryGetComponent<Building>(out var building)) return;
                
                if (!IsPlacementValid(building)) return;

                var originCell = HexGridManager.Instance.hexGrid.GetNearestHexCell(building.transform.position);
                if (originCell is null) return;
                
                var tissue = HexGridManager.Instance.hexGrid.GetTissue(originCell.HexCoordinate, building.Footprint);
                foreach (var cell in tissue)
                {
                    cell.OccupiedBy = obj;
                }
            
                _selectedObject = null;
                
                EventSystem.Instance.InvokeBuildingPlaced(_selectedObject);
            }
            else
            {
                // Pick object up
                _selectedObject = obj;
                
                if (!_selectedObject.TryGetComponent<Building>(out var building)) return;
                
                var originCell = HexGridManager.Instance.hexGrid.GetNearestHexCell(building.transform.position);
                if (originCell is null) return;
            
                var tissue = HexGridManager.Instance.hexGrid.GetTissue(originCell.HexCoordinate, building.Footprint);
                foreach (var cell in tissue)
                {
                    // TODO: We should only reset the occupancy when we have placed it. So we could cancel the placement
                    cell.OccupiedBy = null;
                }
            }
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
            var cell = HexGridManager.Instance.hexGrid.GetNearestHexCell(building.transform.position);
            if (cell is null) return false;

            var coordinate = cell.HexCoordinate;
            var footprint = building.Footprint;
            var adjacentHexCoordinates = footprint.Select(offset =>
                new HexCoordinate(coordinate.Q + offset.Q, coordinate.R + offset.R)).ToList();

            var isInvalidPlacement = adjacentHexCoordinates
                .Select(adjacentHex => HexGridManager.Instance.hexGrid.GetCell(adjacentHex))
                .Any(adjacentCell => adjacentCell is null || adjacentCell.Occupied);

            return !isInvalidPlacement;
        }
    }
}