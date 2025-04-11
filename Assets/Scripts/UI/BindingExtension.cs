using Unity.Properties;
using UnityEngine.UIElements;

namespace UI
{
    public static class BindingExtension
    {
        public static void Bind<TElement, TValue>(this TElement element, BindableProperty<TValue> property,
            string targetPropertyName, BindingMode bindingMode = BindingMode.ToTarget)
            where TElement : VisualElement
        {
            element.dataSource = property;
            element.SetBinding(targetPropertyName, new DataBinding
            {
                dataSourcePath = new PropertyPath(nameof(BindableProperty<TValue>.Value)),
                bindingMode = bindingMode
            });
        }
    }
}