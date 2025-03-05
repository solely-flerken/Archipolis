using UnityEngine;

namespace Buildings
{
    public class Building : MonoBehaviour, IClickable
    {
        public int initialYaw = 0;
        
        private int _yaw; // Rotation in 60-degree increments

        private void Start()
        {
            transform.rotation = Quaternion.Euler(0, initialYaw, 0);
        }

        public void RotateBuilding()
        {
            _yaw = (_yaw + 1) % 6;  // 6 rotations: 0, 60, 120, 180, 240, 300 degrees
            transform.rotation = Quaternion.Euler(0, initialYaw + _yaw * 60, 0);
        }
        
        public void OnClick(GameObject obj)
        {
            
        }
    }
}