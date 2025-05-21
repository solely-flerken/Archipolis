using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Events;
using GameInitialization;
using GameResources;
using Hex;
using Input;
using Preview;
using Save;
using State;
using UI;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using Utils;

namespace Buildings
{
    public class BuildingManager : MonoBehaviour
    {
        public static BuildingManager Instance { get; private set; }

        public static List<Building> Buildings { get; } = new();
        private Dictionary<HexCoordinate, Building> OccupiedTiles { get; } = new();
        private Dictionary<HexCoordinate, Building> PreviewTiles { get; } = new();

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
                // TODO: Refactor. Check all managers if they have to be DontDestroyOnLoad.
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
            EventSystem.Instance.OnModeChanged += HandleModeChange;
        }

        private void Update()
        {
            if (!_selectedObject || !_selectedBuilding ||
                ModeStateManager.Instance.ModeState != Mode.Placing) return;

            // Snap to grid
            var cell = HexMapManager.Instance.HexMap.GetNearestHexCoordinateToMousePosition();
            if (!cell.HasValue) return;
            _selectedObject.transform.position = HexMapManager.Instance.HexMap[cell.Value].WorldPosition;

            // Check placement validity
            var isValidPlacement = IsPlacementValid(_selectedBuilding);

            // Overlay color based on placement validity
            ColorBasedOnValidity(isValidPlacement, _selectedBuilding);

            // Set preview of occupied cells
            PreviewTiles.Clear();
            PreviewManager.Instance.ClearPreview();
            var tissue = HexMapUtil.GetTissue(cell.Value, _selectedBuilding.buildingData.footprint);
            foreach (var hexCoordinate in tissue)
            {
                PreviewTiles[hexCoordinate] = _selectedBuilding;
            }

            PreviewManager.Instance.SetPreview(tissue);
        }

        private void OnDestroy()
        {
            EventSystem.Instance.OnBuildingClick -= HandleBuildingClick;
            EventSystem.Instance.OnPlaceBuildingUI -= HandleCreateBuilding;
            EventSystem.Instance.OnBuildingPlaced -= HandleBuildingPlaced;
            EventSystem.Instance.OnCancel -= HandleCancel;
            EventSystem.Instance.OnKeyR -= HandleBuildingRotate;
            EventSystem.Instance.OnModeChanged -= HandleModeChange;
        }

        // TODO: Display accurate loading information
        public IEnumerator Initialize(BaseSaveData saveData)
        {
            Buildings.ToList().Where(x => x).ToList().ForEach(building => DeleteBuilding(building.gameObject));
            Buildings.Clear();
            OccupiedTiles.Clear();
            OccupancyPreviewManager.Instance.ClearPreviewHexes();

            var totalBuildings = saveData.buildings.Count;
            
            // TODO: Refactor this
            for (var index = 0; index < totalBuildings; index++)
            {
                var buildingData = saveData.buildings[index];
        
                // Create and initialize building
                var createdBuilding = CreateBuilding(null, buildingData);
                var buildingComponent = createdBuilding.GetComponent<Building>();

                var newTissue = HexMapUtil.GetTissue(buildingData.origin, buildingData.footprint);
                OccupyHexes(newTissue, buildingComponent);

                Buildings.Add(buildingComponent);

                LoadingProgressManager.LoadingMessage = $"Loading Buildings {index + 1}/{totalBuildings}";
                yield return null;
            }
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
                    ConfirmationDialog.Instance.Show("Are you sure to delete this building?",
                        () => { DeleteBuilding(obj); });
                    break;
                // Pick building up
                case Mode.Building:
                {
                    var coordinate =
                        HexMapManager.Instance.HexMap.GetNearestHexCoordinate(_selectedObject.transform.position);

                    if (!coordinate.HasValue) return;

                    _previousPosition = coordinate.Value;

                    ModeStateManager.Instance.SetMode(Mode.Placing);
                    break;
                }
                // Place building
                case Mode.Placing:
                {
                    var coordinate =
                        HexMapManager.Instance.HexMap.GetNearestHexCoordinate(_selectedObject.transform.position);

                    if (!coordinate.HasValue) return;

                    _selectedBuilding.buildingData.origin = coordinate.Value;

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


            PreviewTiles.Clear();
            PreviewManager.Instance.ClearPreview();
            FreeOccupiedHexes(buildingComponent);

            var newTissue = HexMapUtil.GetTissue(_selectedBuilding.buildingData.origin,
                _selectedBuilding.buildingData.footprint);
            OccupyHexes(newTissue, buildingComponent);

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

        private void HandleModeChange(Mode currentMode, Mode newMode)
        {
            if (currentMode == Mode.Placing)
            {
                CancelAllActions(currentMode);
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
                    if (!OccupiedTiles.ContainsKey(_selectedBuilding.buildingData.origin))
                    {
                        DeleteBuilding(_selectedObject);
                        return;
                    }

                    PreviewTiles.Clear();
                    PreviewManager.Instance.ClearPreview();

                    _selectedBuilding.buildingData.origin = _previousPosition;
                    var coordinate = HexMapManager.Instance.HexMap[_selectedBuilding.buildingData.origin];
                    _selectedObject.transform.position = coordinate.WorldPosition;

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

        private bool IsPlacementValid(Building building)
        {
            var coordinate = HexMapManager.Instance.HexMap.GetNearestHexCoordinate(building.transform.position);
            if (!coordinate.HasValue)
            {
                return false;
            }

            var adjacentHexCells = HexMapUtil.GetTissue(coordinate.Value, building.buildingData.footprint);

            var isInvalidPlacement = adjacentHexCells.Any(cell => IsHexInvalid(cell, building));

            return !isInvalidPlacement;
        }

        private bool IsHexInvalid(HexCoordinate coordinate, Building building)
        {
            var hexExists = !HexMapManager.Instance.HexMap.ContainsKey(coordinate);
            var hexIsOccupiedNotBySelf = OccupiedTiles.TryGetValue(coordinate, out var occupyingBuilding) &&
                                         occupyingBuilding != building;
            return hexExists || hexIsOccupiedNotBySelf;
        }

        private static GameObject CreateBuilding(BuildingBlueprint blueprint, BuildingData buildingData = null)
        {
            Vector3 position;
            var buildingBlueprint = blueprint;

            if (buildingData != null)
            {
                position = HexMapManager.Instance.HexMap[buildingData.origin].WorldPosition;
                buildingBlueprint = BuildingDatabase.GetBuildingByID(buildingData.blueprintIdentifier);
            }
            else
            {
                position = MouseUtils.MouseToWorldPosition(Vector3.up, CameraController.Camera);
            }

            var newBuilding = Instantiate(buildingBlueprint.prefab, position, Quaternion.identity);

            // TODO: Refactor this
            SceneManager.MoveGameObjectToScene(newBuilding, SceneManager.GetSceneByName("MainScene"));

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

            PreviewTiles.Clear();
            PreviewManager.Instance.ClearPreview();
            FreeOccupiedHexes(buildingComponent);

            Destroy(building);
        }

        private void FreeOccupiedHexes(Building building)
        {
            var hexesToClear = OccupiedTiles
                .Where(pair => pair.Value == building)
                .Select(pair => pair.Key)
                .ToList();

            foreach (var coordinate in hexesToClear)
            {
                OccupiedTiles.Remove(coordinate);
                OccupancyPreviewManager.Instance.RemovePreviewHex(coordinate);
            }
        }

        private void OccupyHexes(List<HexCoordinate> coordinates, Building building)
        {
            foreach (var cell in coordinates)
            {
                OccupiedTiles[cell] = building;
            }

            OccupancyPreviewManager.Instance.AddPreviewHexes(coordinates);
        }

        #endregion
    }
}