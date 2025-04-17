using System;
using System.Collections.Generic;
using System.Linq;
using Events;
using GameResources;
using Hex;
using Input;
using Save;
using State;
using UI;
using Unity.VisualScripting;
using UnityEngine;
using Utils;

namespace Buildings
{
    public class BuildingManager : MonoBehaviour
    {
        public static BuildingManager Instance { get; private set; }

        public static List<Building> Buildings { get; private set; } = new();

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
            EventSystem.Instance.OnLoadGame += HandleLoadGame;
            EventSystem.Instance.OnModeChanged += HandleModeChange;
        }

        private void HandleModeChange(Mode currentMode, Mode newMode)
        {
            if (currentMode == Mode.Placing)
            {
                CancelAllActions(currentMode);
            }
        }

        private void Update()
        {
            if (!_selectedObject || !_selectedBuilding ||
                ModeStateManager.Instance.ModeState != Mode.Placing) return;

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
                HexGridManager.Instance.HexGrid.GetTissue(cell.HexCoordinate,
                    _selectedBuilding.buildingData.footprint);
            adjacentHexCells.Where(x => x is not null).ToList().ForEach(x => x.Preview = true);
        }

        private void OnDestroy()
        {
            EventSystem.Instance.OnBuildingClick -= HandleBuildingClick;
            EventSystem.Instance.OnPlaceBuildingUI -= HandleCreateBuilding;
            EventSystem.Instance.OnBuildingPlaced -= HandleBuildingPlaced;
            EventSystem.Instance.OnCancel -= HandleCancel;
            EventSystem.Instance.OnKeyR -= HandleBuildingRotate;
            EventSystem.Instance.OnLoadGame -= HandleLoadGame;
            EventSystem.Instance.OnModeChanged -= HandleModeChange;
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
                    ConfirmationDialog.Show("Are you sure to delete this building?", () => { DeleteBuilding(obj); });
                    break;
                // Pick building up or place building
                case Mode.Building:
                {
                    var objectHexCell =
                        HexGridManager.Instance.HexGrid.GetNearestHexCell(_selectedObject.transform.position);

                    if (objectHexCell == null) return;

                    _previousPosition = objectHexCell.HexCoordinate;

                    ModeStateManager.Instance.SetMode(Mode.Placing);
                    break;
                }
                case Mode.Placing:
                {
                    var nearestHexCell =
                        HexGridManager.Instance.HexGrid.GetNearestHexCell(_selectedObject.transform.position);

                    _selectedBuilding.buildingData.origin = nearestHexCell.HexCoordinate;

                    if (!IsPlacementValid(_selectedBuilding)) return;

                    EventSystem.Instance.InvokeBuildingPlaced(_selectedObject);
                    break;
                }
                // Show building info
                case Mode.Idle:
                {
                    break;
                }
                default:
                    throw new Exception("Invalid mode state");
            }
        }

        // Called on UI event when placing a new building.
        private void HandleCreateBuilding(string identifier)
        {
            var buildingBlueprint = BuildingDatabase.GetBuildingByID(identifier);

            // TODO: Refactor
            var newBuilding = CreateBuilding(buildingBlueprint);

            ModeStateManager.Instance.SetMode(Mode.Building);
            EventSystem.Instance.InvokeBuildingClick(newBuilding);
        }

        private void HandleBuildingPlaced(GameObject obj)
        {
            var buildingComponent = obj.GetComponent<Building>();
            if (!Buildings.Contains(buildingComponent))
            {
                // TODO: Refactor (I don't like this here that we access ResourceManager, maybe it's ok)
                var blueprint = BuildingDatabase.GetBuildingByID(buildingComponent.buildingData.blueprintIdentifier);
                if (!ResourceManager.Instance.HasEnoughResources(blueprint.buildingCosts))
                {
                    // TODO: Definitely need to notify the player about this somehow.
                    // Can't place building because of insufficient buildings materials.
                    return;
                }
                ResourceManager.Instance.Consume(blueprint.buildingCosts);
                
                Buildings.Add(buildingComponent);
            }
            
            // Free occupied cells
            HexGridManager.Instance.HexGrid.hexCells
                .Where(x => x.OccupiedBy == obj)
                .ToList()
                .ForEach(x => x.OccupiedBy = null);

            // Occupy new cells
            var newTissue =
                HexGridManager.Instance.HexGrid.GetTissue(_selectedBuilding.buildingData.origin,
                    _selectedBuilding.buildingData.footprint);
            foreach (var cell in newTissue)
            {
                cell.OccupiedBy = obj;
            }

            ResetSelection();
            // TODO: Only change to building again if the building placed wasn't a new one
            ModeStateManager.Instance.SetMode(Mode.Building);
        }

        private void HandleCancel()
        {
            CancelAllActions(ModeStateManager.Instance.ModeState);
            ModeStateManager.Instance.SetMode(Mode.Idle);
        }

        private void HandleBuildingRotate(GameObject obj)
        {
            if (ModeStateManager.Instance.ModeState != Mode.Placing) return;

            _selectedBuilding.RotateBuilding();
            _selectedBuilding.RotateFootprint();
        }

        private void HandleLoadGame(BaseSaveData saveData)
        {
            Buildings.ToList().ForEach(building => DeleteBuilding(building.gameObject));
            Buildings.Clear();

            // TODO: Refactor this
            foreach (var buildingData in saveData.buildings)
            {
                var createdBuilding = CreateBuilding(null, buildingData);

                var newTissue =
                    HexGridManager.Instance.HexGrid.GetTissue(buildingData.origin, buildingData.footprint);
                foreach (var cell in newTissue)
                {
                    cell.OccupiedBy = createdBuilding;
                }

                Buildings.Add(createdBuilding.GetComponent<Building>());
            }
        }

        #endregion

        #region Utility functions

        private void ResetSelection()
        {
            _selectedObject = null;
            _selectedBuilding = null;
        }

        private static void ColorBasedOnValidity(bool isValid, Building building)
        {
            building.SetColor(isValid ? BaseOverlay : InvalidOverlay);
        }

        private void CancelAllActions(Mode mode)
        {
            switch (mode)
            {
                case Mode.Placing:
                    if (_selectedBuilding == null) return;

                    // Checks if the building is newly created and wasn't placed yet. If this is the case we delete the building on cancel.
                    if (!HexGridManager.Instance.HexGrid.hexCells.Exists(cell => cell.OccupiedBy == _selectedObject))
                    {
                        DeleteBuilding(_selectedObject);
                        return;
                    }

                    HexGridManager.Instance.HexGrid.hexCells.ForEach(x => x.Preview = false);
                    
                    _selectedBuilding.buildingData.origin = _previousPosition;
                    var cell = HexGridManager.Instance.HexGrid.GetCell(_selectedBuilding.buildingData.origin);
                    _selectedObject.transform.position = cell.transform.position;

                    // We need this here since it won't be called in update(), because we set _selectedObject to null
                    var isValidPlacement = IsPlacementValid(_selectedBuilding);
                    ColorBasedOnValidity(isValidPlacement, _selectedBuilding);
                    break;
                case Mode.Building:
                case Mode.Idle:
                case Mode.Bulldozing:
                default:
                    // Do nothing
                    break;
            }

            ResetSelection();
        }

        private static bool IsPlacementValid(Building building)
        {
            var cell = HexGridManager.Instance.HexGrid.GetNearestHexCell(building.transform.position);
            if (cell is null) return false;

            var adjacentHexCells =
                HexGridManager.Instance.HexGrid.GetTissue(cell.HexCoordinate, building.buildingData.footprint);

            var isInvalidPlacement = adjacentHexCells.Any(adjacentCell =>
                !adjacentCell || (adjacentCell.Occupied && adjacentCell.OccupiedBy != building.gameObject));

            return !isInvalidPlacement;
        }

        private static GameObject CreateBuilding(BuildingBlueprint blueprint, BuildingData buildingData = null)
        {
            Vector3 position;
            var buildingBlueprint = blueprint;

            if (buildingData != null)
            {
                position = HexGridManager.Instance.HexGrid.HexToWorld(buildingData.origin);
                buildingBlueprint = BuildingDatabase.GetBuildingByID(buildingData.blueprintIdentifier);
            }
            else
            {
                position = MouseUtils.MouseToWorldPosition(Vector3.up, CameraController.Camera);
            }

            var newBuilding = Instantiate(buildingBlueprint.prefab, position, Quaternion.identity);
            var buildingComponent = newBuilding.GetOrAddComponent<Building>();
            buildingComponent.Initialize(blueprint, buildingData);

            return newBuilding;
        }

        private void DeleteBuilding(GameObject building)
        {
            var buildingComponent = building.GetComponent<Building>();
            if (Buildings.Contains(buildingComponent))
            {
                Buildings.Remove(buildingComponent);
            }

            foreach (var cell in HexGridManager.Instance.HexGrid.hexCells.Where(cell => cell.Preview))
            {
                cell.Preview = false;
            }

            foreach (var cell in HexGridManager.Instance.HexGrid.hexCells.Where(cell =>
                         cell.OccupiedBy == building))
            {
                cell.OccupiedBy = null;
            }

            Destroy(building);
        }

        #endregion
    }
}