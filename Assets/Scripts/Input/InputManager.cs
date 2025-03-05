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

        void InputActions.IGeneralActions.OnClick(InputAction.CallbackContext context)
        {
            if (!context.performed) return;
            
            EventSystem.Instance.InvokeClick();
        }
    }
}