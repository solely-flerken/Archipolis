using Events;
using UnityEngine;
using Utils;

namespace Input
{
    public class CameraController : MonoBehaviour
    {
        [Header("Camera Settings")] public Vector3 initialPosition;

        [Header("Zoom Settings")] 
        public float zoomSpeed = 10f;
        public float minZoom = 10f;
        public float maxZoom = 40f;
        public float zoomSmoothTime = 0.1f;  

        private float _currentZoom;
        private float _currentZoomVelocity;
        private float _targetZoom;
        
        private bool _isPanning;
        private Vector3 _lastMousePosition;

        public static Camera Camera;

        private void Start()
        {
            Camera = Camera.main;

            transform.position = initialPosition;

            EventSystem.Instance.OnMouseScroll += HandleZoom;
            EventSystem.Instance.OnClickRightHold += HandleMouseRightHold;
        }

        private void Update()
        {
            // If panning is active, move the camera
            if (_isPanning)
            {
                PerformPan();
            }
            
            // Update the zoom if it has changed
            if (!Mathf.Approximately(_currentZoom, _targetZoom))
            {
                _currentZoom = Mathf.SmoothDamp(_currentZoom, _targetZoom, ref _currentZoomVelocity, zoomSmoothTime);
                transform.position = new Vector3(transform.position.x, initialPosition.y + _currentZoom, transform.position.z);
            }
        }

        private void OnDestroy()
        {
            EventSystem.Instance.OnMouseScroll -= HandleZoom;
            EventSystem.Instance.OnClickRightHold -= HandleMouseRightHold;
        }

        private void HandleMouseRightHold(bool isHeld)
        {
            _isPanning = isHeld;

            if (_isPanning)
            {
                // Record initial mouse position when panning starts
                _lastMousePosition = MouseUtils.MouseToWorldPosition(Camera, Vector3.up);
            }
        }
        
        private void HandleZoom(float zoomAmount)
        {
            _targetZoom = _currentZoom - zoomAmount * zoomSpeed;

            // Clamp the target zoom value between minZoom and maxZoom
            _targetZoom = Mathf.Clamp(_targetZoom, minZoom, maxZoom);
        }

        private void PerformPan()
        {
            var currentMousePosition = MouseUtils.MouseToWorldPosition(Camera, Vector3.up);
            var mouseDelta = _lastMousePosition - currentMousePosition;

            var move = new Vector3(mouseDelta.x, 0, mouseDelta.z);
            transform.position += move;
        }
    }
}