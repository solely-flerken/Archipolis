using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

// TODO: Fix issue where confirm button sometimes requires multiple clicks to register - possibly related to event handling

namespace UI
{
    public class ConfirmationDialog : MonoBehaviour
    {
        private static ConfirmationDialog _instance;

        private static VisualElement _root;
        private static Label _label;
        private static Button _confirm;
        private static Button _deny;

        private Action _onConfirm;
        private bool _canReceiveInput;
        
        private void Awake()
        {
            if (_instance == null)
            {
                _instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void OnEnable()
        {
            _root = GetComponent<UIDocument>().rootVisualElement;
            _label = _root.Q<Label>("label");
            _confirm = _root.Q<Button>("confirm");
            _deny = _root.Q<Button>("deny");

            _root.style.display = DisplayStyle.None;

            _confirm.clicked += Confirm;
            _deny.clicked += Deny;
        }

        private void OnDisable()
        {
            _confirm.clicked -= Confirm;
            _deny.clicked -= Deny;
        }

        private void Confirm()
        {
            if (!_canReceiveInput) return;
            
            _onConfirm?.Invoke();
            Deny();
        }

        private void Deny()
        {
            if (!_canReceiveInput) return;
            
            _root.SendToBack();
            _root.style.display = DisplayStyle.None;
        }

        public static void Show(string message, Action confirmAction)
        {
            _instance._onConfirm = confirmAction;
            _label.text = message;
            
            _root.BringToFront();
            _root.style.display = DisplayStyle.Flex;
            
            _instance._canReceiveInput = false;
            _instance.StartCoroutine(_instance.EnableInputAfterDelay());
        }
        
        private IEnumerator EnableInputAfterDelay()
        {
            // Wait to ensure the current click doesn't register on the dialog
            // TODO: Maybe change this. I don't like having a fixed waiting time.
            yield return new WaitForSeconds(0.1f);
            _canReceiveInput = true;
        }
    }
}