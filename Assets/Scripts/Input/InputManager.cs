using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Manager
{
    public class InputManager : MonoBehaviour, InputActions.IGeneralActions
    {
        public static InputManager Instance { get; private set; }

        private InputActions _inputMap;
        private InputActions.GeneralActions _general;

        public event Action OnClick;

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

        void InputActions.IGeneralActions.OnClick(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                Debug.Log($"OnClick: {context.phase}");
                OnClick?.Invoke();
            }
        }
    }
}