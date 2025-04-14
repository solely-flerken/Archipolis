using GameResources;
using UnityEngine;
using UnityEngine.UIElements;

namespace UI
{
    public class ResourceInfo : MonoBehaviour
    {
        private VisualElement _root;
        private VisualElement _container;

        private void Start()
        {
            _root = GetComponent<UIDocument>().rootVisualElement;
            _container = _root.Q<VisualElement>("container");

            var resources = ResourceManager.Instance.Resources;
            foreach (var resourceAmount in resources)
            {
                var resourceLabel = new Label();
                var viewModel = new ViewModelResourceAmount(resourceAmount);
                resourceLabel.Bind(viewModel.FormattedAmount, nameof(Label.text));
                _container.Add(resourceLabel);
            }
        }
    }
}