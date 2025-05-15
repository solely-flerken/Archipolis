using GameResources;
using UnityEngine;
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

            Root.style.display = DisplayStyle.None;

            var resources = ResourceManager.Resources;
            foreach (var (resourceType, resourceAmount) in resources)
            {
                // Container for resource image and amount label
                var resourceEntry = new VisualElement
                {
                    style =
                    {
                        flexDirection = FlexDirection.Row,
                        alignItems = Align.Center,
                        justifyContent = Justify.Center
                    }
                };

                // Define label
                var resourceLabel = new Label();
                var viewModel = new ViewModelResourceAmount(resourceAmount);

                if (resourceType.icon != null)
                {
                    resourceLabel.Bind(viewModel.Amount, nameof(Label.text));

                    var imageElement = new Image
                    {
                        image = resourceType.icon.texture,
                        scaleMode = ScaleMode.ScaleToFit,
                        style =
                        {
                            width = 40,
                            height = 40
                        }
                    };
                    resourceEntry.Add(imageElement);
                }
                else
                {
                    // If no icon is present, we display "resourceName: amount"
                    resourceLabel.Bind(viewModel.FormattedAmount, nameof(Label.text));
                }

                resourceEntry.Add(resourceLabel);
                _container.Add(resourceEntry);
            }
        }
    }
}