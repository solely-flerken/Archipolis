using Buildings;
using Events;
using State;
using UnityEngine;
using UnityEngine.UIElements;

namespace UI
{
    public class BuildInfo : MonoBehaviour
    {
        private VisualElement _root;
        private Label _name;
        private Label _population;
        private Toggle _isActive;

        private void Start()
        {
            _root = GetComponent<UIDocument>().rootVisualElement;
            _name = _root.Q<Label>("name");
            _population = _root.Q<Label>("population");
            _isActive = _root.Q<Toggle>("isActive");

            _root.style.display = DisplayStyle.None;

            EventSystem.Instance.OnBuildingClick += ShowBuildingInfo;
            EventSystem.Instance.OnModeChanged += HideUIOnModeChange;
            EventSystem.Instance.OnCancel += HideUIOnCancel;
        }

        private void OnDestroy()
        {
            EventSystem.Instance.OnBuildingClick -= ShowBuildingInfo;
            EventSystem.Instance.OnModeChanged -= HideUIOnModeChange;
            EventSystem.Instance.OnCancel -= HideUIOnCancel;
        }

        private void ShowBuildingInfo(GameObject clickedObject)
        {
            if (ModeStateManager.Instance.ModeState != Mode.Idle) return;

            _root.style.display = DisplayStyle.Flex;

            var building = clickedObject.GetComponent<Building>();
            var viewModel = new ViewModelBuildingData(building.buildingData);

            _name.Bind(viewModel.Identifier, nameof(Label.text));
            _population.Bind(viewModel.CurrentPopulation, nameof(Label.text));
            _isActive.Bind(viewModel.IsActive, nameof(Toggle.value), BindingMode.TwoWay);
        }

        private void HideUIOnModeChange(Mode currentMode, Mode newMode)
        {
            _root.style.display = DisplayStyle.None;
        }

        private void HideUIOnCancel()
        {
            _root.style.display = DisplayStyle.None;
        }
    }
}