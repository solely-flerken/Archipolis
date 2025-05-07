using GameResources;
using UnityEngine.UIElements;

namespace UI
{
    public class ResourceInfo : UserInterfaceBase
    {
        private VisualElement _container;

        private void Start()
        {
            IsVisibleInitially = true;
            
            Root = GetComponent<UIDocument>().rootVisualElement;
            _container = Root.Q<VisualElement>("container");

            var resources = ResourceManager.Resources;
            foreach (var resourceAmount in resources.Values)
            {
                var resourceLabel = new Label();
                var viewModel = new ViewModelResourceAmount(resourceAmount);
                resourceLabel.Bind(viewModel.FormattedAmount, nameof(Label.text));
                _container.Add(resourceLabel);
            }
        }
    }
}