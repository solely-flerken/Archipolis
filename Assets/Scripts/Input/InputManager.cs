using System;
using Building;
using UnityEngine;
using UnityEngine.InputSystem;
using Utils;

namespace Input
{
    public class InputManager : MonoBehaviour, InputActions.IGeneralActions
    {
        public static InputManager Instance { get; private set; }

        private InputActions _inputMap;
        private InputActions.GeneralActions _general;

        public Vector2 MousePosition { get; private set; }

        public event Action OnClick;
        public event Action<GameObject> OnClickableClick;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
                return;
            }

            _inputMap = new InputActions();
            _general = _inputMap.General;
            _general.AddCallbacks(this);
        }

        private void OnEnable()
        {
            _general.Enable();
        }

        private void OnDisable()
        {
            _general.Disable();
        }

        public void OnMousePosition(InputAction.CallbackContext context)
        {
            MousePosition = context.ReadValue<Vector2>();
        }

        void InputActions.IGeneralActions.OnClick(InputAction.CallbackContext context)
        {
            if (!context.performed) return;
            
            OnClick?.Invoke();

            var clickableGameObject = MouseUtils.GetObjectUnderMouse(Camera.main);
                
            if (!clickableGameObject) return;
            if (!clickableGameObject.TryGetComponent<IClickable>(out var component)) return;

            component.OnClick(clickableGameObject);
            
            // Worse performance since every component listening to OnClickableClick does checks.
            // Only use for UI etc.
            OnClickableClick?.Invoke(clickableGameObject);
        }
    }
}