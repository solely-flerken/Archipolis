using Buildings;
using Events;
using State;
using UnityEngine;
using UnityEngine.UIElements;

namespace UI
{
    public class DevConsole : MonoBehaviour
    {
        private static DevConsole _instance;
        private static bool _isEnabled;
        
        private static VisualElement _root;
        private static ScrollView _scrollView;
        private static TextField _cmdInput;
        private static Button _cmdConfirm;

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

        private void Start()
        {
            _root = GetComponent<UIDocument>().rootVisualElement;
            _scrollView = _root.Q<ScrollView>("cmdView");
            _cmdInput = _root.Q<TextField>("cmdInput");
            _cmdConfirm = _root.Q<Button>("cmdConfirm");

            _root.style.display = DisplayStyle.None;
            
            _cmdInput.RegisterCallback<KeyDownEvent>(OnCmdInputKeyDown, TrickleDown.TrickleDown);
            _cmdConfirm.clicked += OnCommandSubmit;
            
            EventSystem.Instance.OnKeyF3 += ToggleIsEnabled;
        }

        private void OnDestroy()
        {
            _cmdInput.UnregisterCallback<KeyDownEvent>(OnCmdInputKeyDown, TrickleDown.TrickleDown);
            _cmdConfirm.clicked -= OnCommandSubmit;
            
            EventSystem.Instance.OnKeyF3 -= ToggleIsEnabled;
        }

        private void OnCmdInputKeyDown(KeyDownEvent evt)
        {
            if (evt.keyCode is (KeyCode.Return or KeyCode.KeypadEnter))
            {
                OnCommandSubmit();
                evt.StopPropagation();
            }
        }
        
        private void OnCommandSubmit()
        {
            var command = _cmdInput.value;
            
            if (string.IsNullOrEmpty(command)) return;
            
            ExecuteCommand(command);
            _cmdInput.value = "";
        }

        private void ToggleIsEnabled()
        {
            _isEnabled = !_isEnabled;
            _root.style.display = _isEnabled ? DisplayStyle.Flex : DisplayStyle.None;
        }
        
        private void ExecuteCommand(string command)
        {
            switch (command.ToLower())
            {
                case "/clear":
                    ClearConsole();
                    break;
                case "/setmode bulldoze":
                    LogMessage("Switched to Bulldoze mode");
                    ModeStateManager.Instance.SetMode(Mode.Bulldozing);
                    break;
                case "/setmode idle":
                    LogMessage("Switched to Idle mode");
                    ModeStateManager.Instance.SetMode(Mode.Idle);
                    break;
                case "/mode":
                    LogMessage($"Current mode: {ModeStateManager.Instance.ModeState}");
                    break;
                default:
                    LogMessage("Unknown command: " + command);
                    break;
            }
        }

        private void LogMessage(string message)
        {
            var newMessageLabel = new Label(message);
            _scrollView.contentContainer.Add(newMessageLabel);

            ScrollToBottom();
        }

        private void ClearConsole()
        {
            _scrollView.contentContainer.Clear();
        }

        private void ScrollToBottom()
        {
            _scrollView.scrollOffset = new Vector2(0, _scrollView.contentContainer.resolvedStyle.height);
        }
    }
}