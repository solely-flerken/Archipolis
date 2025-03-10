using Events;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Input
{
    public class InputManager : MonoBehaviour, InputActions.IGeneralActions
    {
        public static InputManager Instance { get; private set; }

        private InputActions _inputMap;
        private InputActions.GeneralActions _general;

        public Vector2 MousePosition { get; private set; }

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

        public void OnR(InputAction.CallbackContext context)
        {
            if (!context.performed) return;

            EventSystem.Instance.InvokeKeyR();
        }

        public void OnCancel(InputAction.CallbackContext context)
        {
            if (!context.performed) return;

            EventSystem.Instance.InvokeCancel();
        }

        public void OnClickRight(InputAction.CallbackContext context)
        {
            if (context.started)
            {
                EventSystem.Instance.InvokeClickRight();
            }
            else if (context.performed)
            {
                EventSystem.Instance.InvokeClickRightHold(true);
            }
            else if (context.canceled)
            {
                EventSystem.Instance.InvokeClickRightHold(false);
            }
        }

        public void OnMouseScroll(InputAction.CallbackContext context)
        {
            if (!context.performed) return;

            var scroll = context.ReadValue<Vector2>();
            // MouseScrollDelta is stored in a Vector2.y property. (The Vector2.x value is ignored.)
            EventSystem.Instance.InvokeMouseScroll(scroll.y);
        }

        public void OnMouseWheelClick(InputAction.CallbackContext context)
        {
            if (context.started)
            {
                EventSystem.Instance.InvokeMouseWheelClick();
            }
            else if (context.performed)
            {
                EventSystem.Instance.InvokeMouseWheelHold(true);
            }
            else if (context.canceled)
            {
                EventSystem.Instance.InvokeMouseWheelHold(false);
            }
        }

        void InputActions.IGeneralActions.OnClick(InputAction.CallbackContext context)
        {
            if (!context.performed) return;
            EventSystem.Instance.InvokeClick();
        }
    }
}