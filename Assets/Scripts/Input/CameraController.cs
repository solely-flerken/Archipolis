using Events;
using Unity.Mathematics;
using UnityEngine;
using Utils;

namespace Input
{
    public class CameraController : MonoBehaviour
    {
        [Header("Camera Settings")]
        public Vector3 initialPosition;

        [Header("Zoom Settings")] 
        public float zoomSpeed = 10f;
        public float maxZoom = 40f;
        public float zoomSmoothTime = 0.1f;
        
        [Header("Free View Settings")] 
        public float rotationSpeed = 1f;
        public float rotationThreshold = 0.5f;

        public static Camera Camera;

        // Zoom state
        private float _currentZoom;
        private float _currentZoomVelocity;
        private float _targetZoom;
        
        // Free view state
        private bool _isFreeView;
        private Vector2 _lastMousePositionScreen;
        private Vector3 _target;
        private float _distance;
        private float _theta;
        private float _phi;

        // Panning state
        private bool _isPanning;
        private Vector3 _lastMousePositionWorld;
        
        #region Lifecycle Methods
        
        private void Start()
        {
            Camera = Camera.main;

            transform.position = initialPosition;

            EventSystem.Instance.OnMouseScroll += HandleZoom;
            EventSystem.Instance.OnClickRightHold += HandleMouseRightHold;
            EventSystem.Instance.OnMouseWheelHold += HandleMouseWheelHold;
        }
        
        private void Update()
        {
            // If panning is active, move the camera
            if (_isPanning)
            {
                PerformPan();
                initialPosition = transform.position;
            }

            if (_isFreeView)
            {
                PerformFreeView();
                initialPosition = transform.position;
            }

            // Zoom in combination with free view or panning leads to weird behaviour. So we make zooming exclusive for now
            if (_isFreeView || _isPanning) return;
            
            ApplyZoom();
        }
        
        private void OnDestroy()
        {
            EventSystem.Instance.OnMouseScroll -= HandleZoom;
            EventSystem.Instance.OnClickRightHold -= HandleMouseRightHold;
            EventSystem.Instance.OnMouseWheelHold -= HandleMouseWheelHold;
        }
        
        #endregion
        
        #region Free View
        
        private void HandleMouseWheelHold(bool isHeld)
        {
            if (isHeld && !_isFreeView)
            {
                _isFreeView = true;

                _lastMousePositionScreen = InputManager.Instance.MousePosition;

                _target = MouseUtils.ScreenPointToWorldPosition(MouseUtils.ScreenCenter, Vector3.up, Camera);
                
                _distance = Vector3.Distance(_target, transform.position);
                
                var direction = (transform.position - _target).normalized;
                _phi = Mathf.Acos(direction.y);
                _theta = Mathf.Atan2(direction.z, direction.x);
            }   
            else if(!isHeld && _isFreeView)
            {
                _isFreeView = false;
                initialPosition = transform.position;
            }
        }

        private void PerformFreeView()
        {
            var currentMousePosition = InputManager.Instance.MousePosition;
            var delta = currentMousePosition - _lastMousePositionScreen;
            
            if (math.abs(delta.x) > rotationThreshold)
            {
                _theta -= delta.x * rotationSpeed * 0.01f;
            }
            if (math.abs(delta.y) > rotationThreshold)
            {
                _phi = Mathf.Clamp(_phi + delta.y * rotationSpeed * 0.01f, 0.1f, Mathf.PI - 0.1f);
            }
            
            var pointOnSphere = SphereUtils.GetPointOnSphere(_distance, _theta, _phi);
            transform.position = _target + pointOnSphere;
            transform.LookAt(_target);
            _lastMousePositionScreen = currentMousePosition;
        }

        #endregion
        
        #region Panning
        
        private void HandleMouseRightHold(bool isHeld)
        {
            _isPanning = isHeld;

            if (_isPanning)
            {
                // Record initial mouse position when panning starts
                _lastMousePositionWorld = MouseUtils.MouseToWorldPosition(Vector3.up, Camera);
            }
        }

        private void PerformPan()
        {
            var currentMousePosition = MouseUtils.MouseToWorldPosition(Vector3.up, Camera);
            var mouseDelta = _lastMousePositionWorld - currentMousePosition;

            var move = new Vector3(mouseDelta.x, 0, mouseDelta.z);
            transform.position += move;
        }
        
        #endregion
        
        #region Zooming

        private void ApplyZoom()
        {
            if (Mathf.Approximately(_currentZoom, _targetZoom))
                return;
                
            _currentZoom = Mathf.SmoothDamp(_currentZoom, _targetZoom, ref _currentZoomVelocity, zoomSmoothTime);
            transform.position = initialPosition + Camera.transform.forward * _currentZoom;
        }
        
        private void HandleZoom(float zoomAmount)
        {
            _targetZoom += zoomAmount * zoomSpeed;
            _targetZoom = math.clamp(_targetZoom, 0, maxZoom);
        }
        
        #endregion
    }
}