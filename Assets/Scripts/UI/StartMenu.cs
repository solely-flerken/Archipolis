using System.Collections.Generic;
using Events;
using UnityEngine.UIElements;

namespace UI
{
    public class StartMenu : UserInterfaceBase
    {
        private VisualElement _buttonContainer;
        private Dictionary<string, Button> _buttons = new();

        private void Start()
        {
            IsVisibleInitially = true;
            
            Root = GetComponent<UIDocument>().rootVisualElement;
            _buttonContainer = Root.Q<VisualElement>("buttonContainer");

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