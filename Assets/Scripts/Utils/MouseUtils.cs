using Input;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;

namespace Utils
{
    public static class MouseUtils
    {
        public static Vector3 MouseToWorldPosition(Camera camera, Vector3 planeNormal, LayerMask? layerMask = null)
        {
            var ray = camera.ScreenPointToRay(InputManager.Instance.MousePosition);
            var maskToUse = layerMask ?? Physics.DefaultRaycastLayers;

            // Perform raycast to check if something is hit
            if (Physics.Raycast(ray, out var hit, Mathf.Infinity, maskToUse))
            {
                return hit.point; // Return the hit point if the ray hits something
            }

            // If no hit, calculate where the ray intersects with the defined plane
            var rayToPlaneDistance = -Vector3.Dot(ray.origin, planeNormal) / Vector3.Dot(ray.direction, planeNormal);
            
            // Get the world position of the intersection with the plane
            return ray.origin + ray.direction * rayToPlaneDistance;
        }

        public static GameObject GetObjectUnderMouse(Camera camera, LayerMask? layerMask = null)
        {
            var ray = camera.ScreenPointToRay(InputManager.Instance.MousePosition);
            var maskToUse = layerMask ?? Physics.DefaultRaycastLayers;

            return Physics.Raycast(ray, out var hit, Mathf.Infinity, maskToUse) ? hit.collider.gameObject : null;
        }
    }
}