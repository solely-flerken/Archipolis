using UnityEngine;

namespace Utils
{
    public static class MouseUtils
    {
        public static Vector3? MouseToWorldPosition(Camera camera, LayerMask? layerMask = null)
        {
            var ray = camera.ScreenPointToRay(Input.mousePosition);
            
            var maskToUse = layerMask ?? Physics.DefaultRaycastLayers;
            
            return Physics.Raycast(ray, out var hit, Mathf.Infinity, maskToUse) ? hit.point : null;
        }
    }
}