using GameResources;
using Hex;
using UnityEngine;

namespace Buildings
{
    [RequireComponent(typeof(MeshRenderer))]
    public class Building : MonoBehaviour, IClickable
    {
        public BuildingData buildingData;

        /// <summary>
        /// ResourceFlow is also part of the `BuildingBlueprint` to define static consumption and production rates at the time of creation.
        /// However, since inputs and outputs can change during runtime (due to upgrades, environmental factors, etc.), we store these values separately in here for dynamic modification
        /// during gameplay. So the lists in `BuildingBlueprint` are only used during initialization.
        /// </summary>
        public ResourceFlow ResourceFlow { get; private set; }

        private MeshRenderer _meshRenderer;
        private MaterialPropertyBlock _propertyBlock;

        private static readonly int OverlayColor = Shader.PropertyToID("_OverlayColor");

        public void Initialize(BuildingBlueprint blueprint, BuildingData data = null)
        {
            if (data != null)
            {
                // Initialize from save data
                buildingData = data;
                transform.rotation = Quaternion.Euler(0, buildingData.yaw * 60, 0);
            }
            else
            {
                // Initialize from blueprint
                buildingData = new BuildingData
                {
                    blueprintIdentifier = blueprint.identifier,
                    footprint = blueprint.footprint
                };
            }

            ResourceFlow = blueprint.resourceFlow.Clone();

            _meshRenderer = GetComponent<MeshRenderer>();
            _propertyBlock = new MaterialPropertyBlock();
        }

        public void RotateBuilding()
        {
            buildingData.yaw = (buildingData.yaw + 1) % 6; // 6 rotations: 0, 60, 120, 180, 240, 300 degrees
            transform.rotation = Quaternion.Euler(0, buildingData.yaw * 60, 0);
        }

        public void RotateFootprint()
        {
            buildingData.footprint = HexGrid.RotateHexesClockwise(new HexCoordinate(0, 0), buildingData.footprint);
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