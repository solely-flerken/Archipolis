using System.Collections.Generic;
using Events;
using UnityEngine;
using UnityEngine.UIElements;

namespace UI
{
    public class StartMenu : MonoBehaviour
    {
        private VisualElement _root;
        private VisualElement _buttonContainer;
        private Dictionary<string, Button> _buttons = new();

        private void OnEnable()
        {
            _root = GetComponent<UIDocument>().rootVisualElement;
            _buttonContainer = _root.Q<VisualElement>("buttonContainer");

            var buttons = _buttonContainer.Query<Button>().ToList();
            foreach (var button in buttons)
            {
                button.clicked += () => HandleButtonClick(button.name);

                _buttons.Add(button.name, button);
            }
        }

        private static void HandleButtonClick(string buttonName)
        {
            EventSystem.Instance.InvokeButtonClick(buttonName);
        }
    }
}