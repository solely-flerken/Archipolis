using Input;
using UnityEngine;

namespace Utils
{
    public static class MouseUtils
    {
        public static Vector3? MouseToWorldPosition(Camera camera, LayerMask? layerMask = null)
        {
            var ray = camera.ScreenPointToRay(InputManager.Instance.MousePosition);
            var maskToUse = layerMask ?? Physics.DefaultRaycastLayers;
            
            return Physics.Raycast(ray, out var hit, Mathf.Infinity, maskToUse) ? hit.point : null;
        }
        
        public static GameObject GetObjectUnderMouse(Camera camera, LayerMask? layerMask = null)
        {
            var ray = camera.ScreenPointToRay(InputManager.Instance.MousePosition);
            var maskToUse = layerMask ?? Physics.DefaultRaycastLayers;
            
            return Physics.Raycast(ray, out var hit, Mathf.Infinity, maskToUse) ? hit.collider.gameObject : null;
        }
    }
}