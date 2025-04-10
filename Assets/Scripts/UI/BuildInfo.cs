using Events;
using State;
using UnityEngine;
using UnityEngine.UIElements;

namespace UI
{
    public class BuildInfo : MonoBehaviour
    {
        private VisualElement _root;

        private void Start()
        {
            _root = GetComponent<UIDocument>().rootVisualElement;

            _root.style.display = DisplayStyle.None;

            EventSystem.Instance.OnBuildingClick += ToggleShowBuildingInfo;
            EventSystem.Instance.OnModeChanged += HandleModeChange;
            EventSystem.Instance.OnCancel += HandleCancel;
        }

        private void OnDestroy()
        {
            EventSystem.Instance.OnBuildingClick -= ToggleShowBuildingInfo;
            EventSystem.Instance.OnModeChanged -= HandleModeChange;
            EventSystem.Instance.OnCancel -= HandleCancel;
        }

        private void ToggleShowBuildingInfo(GameObject obj)
        {
            if (ModeStateManager.Instance.ModeState == Mode.Idle)
            {
                _root.style.display = DisplayStyle.Flex;
            }
        }

        private void HandleModeChange(Mode newMode)
        {
            _root.style.display = DisplayStyle.None;
        }

        private void HandleCancel()
        {
            _root.style.display = DisplayStyle.None;
        }
    }
}