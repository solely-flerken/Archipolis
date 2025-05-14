using System.Collections.Generic;
using Events;
using UnityEngine;
using UnityEngine.UIElements;

namespace UI
{
    public class StartMenu : UserInterfaceBase
    {
        private VisualElement _container;
        private VisualElement _buttonContainer;
        private Dictionary<string, Button> _buttons = new();
        private SaveFileView _saveFileView;

        private void Start()
        {
            IsVisibleInitially = true;

            Root = GetComponent<UIDocument>().rootVisualElement;
            _container = Root.Q<VisualElement>("container");
            _buttonContainer = Root.Q<VisualElement>("buttonContainer");

            var buttons = _buttonContainer.Query<Button>().ToList();
            foreach (var button in buttons)
            {
                button.clicked += () => HandleButtonClick(button.name);

                _buttons.Add(button.name, button);
            }
        }

        private void HandleButtonClick(string buttonName)
        {
            if (buttonName == "continue")
            {
                EventSystem.Instance.InvokeLoadGame(string.Empty);
            }
            else if (buttonName == "load")
            {
                if (_saveFileView == null)
                {
                    _saveFileView = new SaveFileView();
                    _saveFileView.OnSaveSelection = (save) =>
                    {
                        _saveFileView.RemoveFromHierarchy();
                        _saveFileView = null;
                        EventSystem.Instance.InvokeLoadGame(save);
                    };
                    _container.Add(_saveFileView);
                }
            }
            else if (buttonName == "quit")
            {
                ConfirmationDialog.Instance.Show("Are you sure you want to quit?", Quit);
            }

            EventSystem.Instance.InvokeButtonClick(buttonName);
        }

        private static void Quit()
        {
            Application.Quit();

#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#endif
        }
    }
}