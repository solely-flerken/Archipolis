using System;
using System.Linq;
using Events;
using Hex;
using Input;
using State;
using UI;
using UnityEngine;
using Utils;

namespace Buildings
{
    public class BuildingManager : MonoBehaviour
    {
        public static BuildingManager Instance { get; private set; }

        private GameObject _selectedObject;
        private Building _selectedBuilding;
        private HexCoordinate _previousPosition;

        private static readonly Color BaseOverlay = new(0, 0, 0, 0.0f);
        private static readonly Color InvalidOverlay = new(1, 0, 0, 1f);

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
            EventSystem.Instance.OnPlaceBuildingUI += HandleCreateBuilding;
            EventSystem.Instance.OnBuildingPlaced += HandleBuildingPlaced;
            EventSystem.Instance.OnCancel += HandleCancel;
            EventSystem.Instance.OnKeyR += HandleBuildingRotate;
        }

        private void Update()
        {
            if (!_selectedObject || !_selectedBuilding ||
                ModeStateManager.Instance.ModeState != Mode.Building) return;

            // Snap to grid
            var cell = HexGridManager.Instance.HexGrid.GetNearestHexCellToMousePosition();
            if (cell is null) return;
            _selectedObject.transform.position = cell.transform.position;

            // Check placement validity
            var isValidPlacement = IsPlacementValid(_selectedBuilding);

            // Overlay color based on placement validity
            ColorBasedOnValidity(isValidPlacement, _selectedBuilding);

            // TODO: Refactor this (expensive)
            // Set preview of occupied cells
            HexGridManager.Instance.HexGrid.hexCells.ForEach(x => x.Preview = false);
            var adjacentHexCells =
                HexGridManager.Instance.HexGrid.GetTissue(cell.HexCoordinate, _selectedBuilding.Footprint);
            adjacentHexCells.Where(x => x is not null).ToList().ForEach(x => x.Preview = true);
        }

        private void OnDestroy()
        {
            EventSystem.Instance.OnBuildingClick -= HandleBuildingClick;
            EventSystem.Instance.OnPlaceBuildingUI -= HandleCreateBuilding;
            EventSystem.Instance.OnBuildingPlaced -= HandleBuildingPlaced;
            EventSystem.Instance.OnCancel -= HandleCancel;
            EventSystem.Instance.OnKeyR -= HandleBuildingRotate;
        }

        #region Event subscriptions

        private void HandleBuildingClick(GameObject obj)
        {
            _selectedObject = obj;
            _selectedBuilding = _selectedObject.GetComponent<Building>();

            switch (ModeStateManager.Instance.ModeState)
            {
                // Delete building
                case Mode.Bulldozing:
                    ConfirmationDialog.Show("Are you sure to delete this building?", () =>
                    {
                        foreach (var cell in HexGridManager.Instance.HexGrid.hexCells.Where(cell =>
                                     cell.OccupiedBy == _selectedObject))
                        {
                            cell.OccupiedBy = null;
                            cell.Preview = false;
                        }

                        Destroy(obj);
                    });
                    break;
                // Place building
                case Mode.Building:
                {
                    var nearestHexCell =
                        HexGridManager.Instance.HexGrid.GetNearestHexCell(_selectedObject.transform.position);

                    _selectedBuilding.Origin = nearestHexCell.HexCoordinate;

                    if (!IsPlacementValid(_selectedBuilding)) return;

                    EventSystem.Instance.InvokeBuildingPlaced(_selectedObject);
                    break;
                }
                // Pick up building
                case Mode.Idle:
                {
                    var objectHexCell =
                        HexGridManager.Instance.HexGrid.GetNearestHexCell(_selectedObject.transform.position);

                    if (objectHexCell == null) return;

                    _previousPosition = objectHexCell.HexCoordinate;

                    ModeStateManager.Instance.SetMode(Mode.Building);
                    break;
                }
                default:
                    throw new Exception("Invalid mode state");
            }
        }

        // Called on UI event when placing a new building.
        private void HandleCreateBuilding(string identifier)
        {
            ModeStateManager.Instance.SetMode(Mode.Idle);

            var building = BuildingDatabase.GetBuildingByID(identifier);

            if (!building.Prefab)
            {
                return;
            }

            var position = MouseUtils.MouseToWorldPosition(Vector3.up, CameraController.Camera);
            var newBuilding = Instantiate(building.Prefab, position, Quaternion.identity);

            // "Simulate" click on building to enter build mode
            EventSystem.Instance.InvokeBuildingClick(newBuilding);
        }

        private void HandleBuildingPlaced(GameObject obj)
        {
            // Free occupied cells
            HexGridManager.Instance.HexGrid.hexCells
                .Where(x => x.OccupiedBy == obj)
                .ToList()
                .ForEach(x => x.OccupiedBy = null);

            // Occupy new cells
            var newTissue =
                HexGridManager.Instance.HexGrid.GetTissue(_selectedBuilding.Origin, _selectedBuilding.Footprint);
            foreach (var cell in newTissue)
            {
                cell.OccupiedBy = obj;
            }

            ResetSelection();
        }

        private void HandleCancel()
        {
            switch (ModeStateManager.Instance.ModeState)
            {
                case Mode.Building:
                    if (_selectedBuilding == null) return;

                    // TODO: Refactor this
                    if (!HexGridManager.Instance.HexGrid.hexCells.Exists(cell => cell.OccupiedBy == _selectedObject))
                    {
                        Destroy(_selectedObject);
                    }

                    _selectedBuilding.Origin = _previousPosition;

                    var cell = HexGridManager.Instance.HexGrid.GetCell(_selectedBuilding.Origin);
                    _selectedObject.transform.position = cell.transform.position;

                    // We need this here since it won't be called in update(), because we set _selectedObject to null
                    var isValidPlacement = IsPlacementValid(_selectedBuilding);
                    ColorBasedOnValidity(isValidPlacement, _selectedBuilding);
                    break;
                case Mode.Idle:
                case Mode.Bulldozing:
                default:
                    // Do nothing
                    break;
            }

            ResetSelection();
        }

        private void HandleBuildingRotate(GameObject obj)
        {
            if (ModeStateManager.Instance.ModeState != Mode.Building) return;

            _selectedBuilding.RotateBuilding();
            _selectedBuilding.RotateFootprint();
        }

        #endregion

        #region Utility functions

        private void ResetSelection()
        {
            _selectedObject = null;
            _selectedBuilding = null;
            ModeStateManager.Instance.SetMode(Mode.Idle);
        }

        private static void ColorBasedOnValidity(bool isValid, Building building)
        {
            building.SetColor(isValid ? BaseOverlay : InvalidOverlay);
        }

        private static bool IsPlacementValid(Building building)
        {
            var cell = HexGridManager.Instance.HexGrid.GetNearestHexCell(building.transform.position);
            if (cell is null) return false;

            var adjacentHexCells =
                HexGridManager.Instance.HexGrid.GetTissue(cell.HexCoordinate, building.Footprint);

            var isInvalidPlacement = adjacentHexCells.Any(adjacentCell =>
                !adjacentCell || (adjacentCell.Occupied && adjacentCell.OccupiedBy != building.gameObject));

            return !isInvalidPlacement;
        }

        #endregion
    }
}