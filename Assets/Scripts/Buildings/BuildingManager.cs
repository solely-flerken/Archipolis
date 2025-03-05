using Events;
using HexGrid;
using UnityEngine;
using Utils;

namespace Buildings
{
    public class BuildingManager : MonoBehaviour
    {
        private GameObject _selectedBuilding;

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
    }
}