using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

// TODO: Fix issue where confirm button sometimes requires multiple clicks to register - possibly related to event handling

namespace UI
{
    public class ConfirmationDialog : UserInterfaceBase
    {
        public static ConfirmationDialog Instance;

        private static Label _label;
        private static Button _confirm;
        private static Button _deny;

        private Action _onConfirm;
        private bool _canReceiveInput;
        
        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void Start()
        {
            IsVisibleInitially = false;
            
            Root = GetComponent<UIDocument>().rootVisualElement;
            _label = Root.Q<Label>("label");
            _confirm = Root.Q<Button>("confirm");
            _deny = Root.Q<Button>("deny");

            Root.style.display = DisplayStyle.None;

            _confirm.clicked += Confirm;
            _deny.clicked += Deny;
        }

        private void OnDestroy()
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
            
            Root.SendToBack();
            Root.style.display = DisplayStyle.None;
        }

        public void Show(string message, Action confirmAction)
        {
            Instance._onConfirm = confirmAction;
            _label.text = message;
            
            Root.BringToFront();
            Root.style.display = DisplayStyle.Flex;
            
            Instance._canReceiveInput = false;
            Instance.StartCoroutine(Instance.EnableInputAfterDelay());
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