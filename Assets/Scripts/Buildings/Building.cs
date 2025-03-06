using HexGrid;
using UnityEngine;

namespace Buildings
{
    [RequireComponent(typeof(MeshRenderer))]
    public class Building : MonoBehaviour, IClickable
    {
        public int initialYaw;
        
        /// <summary>
        /// Defines the footprint of the building.
        /// Footprint is build by calculating adjacent hex cells with the offsets.
        /// </summary>
        public readonly HexCoordinate[] Footprint = { new(0, 0), new(-1, 0) };
        
        private static readonly int OverlayColor = Shader.PropertyToID("_OverlayColor");

        private int _yaw; // Rotation in 60-degree increments
        private MeshRenderer _meshRenderer;
        private MaterialPropertyBlock _propertyBlock;
        
        private void Start()
        {
            _meshRenderer = GetComponent<MeshRenderer>();
            _propertyBlock = new MaterialPropertyBlock();
            
            transform.rotation = Quaternion.Euler(0, initialYaw, 0);
        }

        public void RotateBuilding()
        {
            _yaw = (_yaw + 1) % 6; // 6 rotations: 0, 60, 120, 180, 240, 300 degrees
            transform.rotation = Quaternion.Euler(0, initialYaw + _yaw * 60, 0);
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