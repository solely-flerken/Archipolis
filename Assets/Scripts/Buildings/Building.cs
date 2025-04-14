using Hex;
using UnityEngine;

namespace Buildings
{
    [RequireComponent(typeof(MeshRenderer))]
    public class Building : MonoBehaviour, IClickable
    {
        public BuildingState buildingState;
        private BuildingData BuildingData => BuildingDatabase.GetBuildingByID(buildingState.blueprintIdentifier);

        private MeshRenderer _meshRenderer;
        private MaterialPropertyBlock _propertyBlock;

        private static readonly int OverlayColor = Shader.PropertyToID("_OverlayColor");

        public void Initialize(BuildingData blueprint, BuildingState state = null)
        {
            // TODO: Refactor
            buildingState = state ?? new BuildingState();
            if (state == null)
            {
                buildingState.blueprintIdentifier = blueprint.identifier;
                buildingState.footprint = blueprint.footprint;
                buildingState.yaw = blueprint.initialYaw;
            }

            if (BuildingData != null)
            {
                transform.rotation = Quaternion.Euler(0, BuildingData.initialYaw + buildingState.yaw * 60, 0);
                buildingState.footprint = BuildingData.footprint;
                for (var i = 0; i < buildingState.yaw; i++)
                {
                    RotateFootprint();
                }
            }

            _meshRenderer = GetComponent<MeshRenderer>();
            _propertyBlock = new MaterialPropertyBlock();
        }

        public void RotateBuilding()
        {
            buildingState.yaw = (buildingState.yaw + 1) % 6; // 6 rotations: 0, 60, 120, 180, 240, 300 degrees
            transform.rotation = Quaternion.Euler(0, BuildingData.initialYaw + buildingState.yaw * 60, 0);
        }

        public void RotateFootprint()
        {
            buildingState.footprint = HexGrid.RotateHexesClockwise(new HexCoordinate(0, 0), buildingState.footprint);
        }

        public void SetColor(Color color)
        {
            _meshRenderer.GetPropertyBlock(_propertyBlock);
            _propertyBlock.SetColor(OverlayColor, color);
            _meshRenderer.SetPropertyBlock(_propertyBlock);
        }

        public void OnClick(GameObject obj)
        {
        }
    }
}