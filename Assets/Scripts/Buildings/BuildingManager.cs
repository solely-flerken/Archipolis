using System.Linq;
using Events;
using Hex;
using Input;
using UnityEngine;
using Utils;

namespace Buildings
{
    public class BuildingManager : MonoBehaviour
    {
        public static BuildingManager Instance { get; private set; }
        
        private GameObject _selectedObject;
        private Building SelectedBuilding => _selectedObject?.GetComponent<Building>();

        // States
        private bool _isBulldozing;
        
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
            EventSystem.Instance.OnCancel += HandleCancel;
            EventSystem.Instance.OnBuildingPlaced += HandleBuildingPlaced;
            EventSystem.Instance.OnPlaceBuildingUI += HandlePlaceBuilding;
            EventSystem.Instance.OnBulldoze += HandleBulldoze;
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
            EventSystem.Instance.OnCancel -= HandleCancel;
            EventSystem.Instance.OnBuildingPlaced -= HandleBuildingPlaced;
            EventSystem.Instance.OnPlaceBuildingUI -= HandlePlaceBuilding;
            EventSystem.Instance.OnBulldoze -= HandleBulldoze;
        }

        private void HandleBuildingPlaced(GameObject obj)
        {
            // Free occupied cells
            HexGridManager.Instance.hexGrid.hexCells
                .Where(x => x.OccupiedBy == obj)
                .ToList()
                .ForEach(x => x.OccupiedBy = null);

            // Occupy new cells
            var newTissue = HexGridManager.Instance.hexGrid.GetTissue(SelectedBuilding.Origin, SelectedBuilding.Footprint);
            foreach (var cell in newTissue)
            {
                cell.OccupiedBy = obj;
            }

            _selectedObject = null;
        }

        private void HandleCancel()
        {
            // TODO: Refactor this
            if (!HexGridManager.Instance.hexGrid.hexCells.Exists(cell => cell.OccupiedBy == _selectedObject))
            {
                Destroy(_selectedObject);
            }
            
            SelectedBuilding.Origin = SelectedBuilding.InitialPosition;

            var cell = HexGridManager.Instance.hexGrid.GetCell(SelectedBuilding.Origin);
            _selectedObject.transform.position = cell.transform.position;
            
            // We need this here since it won't be called in update(), because we set _selectedObject to null
            var isValidPlacement = IsPlacementValid(SelectedBuilding);
            ColorBasedOnValidity(isValidPlacement, SelectedBuilding);
            
            _selectedObject = null;
        }

        private void HandleBuildingClick(GameObject obj)
        {
            if (_selectedObject is not null)
            {
                // Try place object. Only place the building if placement is valid
                var originCell = HexGridManager.Instance.hexGrid.GetNearestHexCell(obj.transform.position);

                if (SelectedBuilding is null) return;

                SelectedBuilding.Origin = originCell.HexCoordinate;

                if (!IsPlacementValid(SelectedBuilding)) return;

                EventSystem.Instance.InvokeBuildingPlaced(_selectedObject);
            }
            else
            {
                // Pick object 
                _selectedObject = obj;

                var objectHexCell =
                    HexGridManager.Instance.hexGrid.GetNearestHexCell(_selectedObject.transform.position);

                if (objectHexCell is null) return;
                if (SelectedBuilding is null) return;

                SelectedBuilding.InitialPosition = objectHexCell.HexCoordinate;
            }
        }

        private void HandleBuildingRotate(GameObject obj)
        {
            if (_selectedObject is null) return;

            if (!_selectedObject.TryGetComponent<Building>(out var building)) return;

            building.RotateBuilding();
            building.RotateFootprint();
        }

        private void HandlePlaceBuilding(string identifier)
        {
            var building = BuildingDatabase.GetBuildingByID(identifier);

            if (!building.Prefab)
            {
                return;
            }
            
            var position = MouseUtils.MouseToWorldPosition(Vector3.up, CameraController.Camera);
            var newBuilding = Instantiate(building.Prefab, position, Quaternion.identity);
            EventSystem.Instance.InvokeBuildingClick(newBuilding);
        }
        
        private void HandleBulldoze()
        {
            _isBulldozing = !_isBulldozing;
            Debug.Log($"Bulldoze is {_isBulldozing}");
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
                .Any(adjacentCell => adjacentCell is null ||
                                     (adjacentCell.Occupied && adjacentCell.OccupiedBy != building.gameObject));

            return !isInvalidPlacement;
        }
    }
}