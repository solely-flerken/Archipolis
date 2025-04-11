using Unity.Properties;
using UnityEngine.UIElements;

namespace UI
{
    public static class BindingExtension
    {
        /// <summary>
        /// Binds a <see cref="BindableProperty{T}"/> to a UI element's property.
        /// </summary>
        /// <typeparam name="TElement">The type of UI element to bind to (extending <see cref="VisualElement"/>).</typeparam>
        /// <typeparam name="TValue">The type of the value in the bindable property.</typeparam>
        /// <param name="element">The UI element to bind to.</param>
        /// <param name="property">The bindable property to bind.</param>
        /// <param name="targetPropertyName">The name of the target property on the UI element to bind to.</param>
        /// <param name="bindingMode">The binding mode (default is <see cref="BindingMode.ToTarget"/>).</param>
        /// <example>
        /// The following example demonstrates how to use a <see cref="BindableProperty{T}"/>.
        /// <code>
        /// var nameProperty = BindableProperty&lt;string&gt;.Bind(() => anyObject.name, (x) => anyObject.name = x);
        /// _nameLabel.Bind(nameProperty, nameof(Label.text), BindingMode.TwoWay);
        /// anyObject.name = "The label text will automatically update!"
        /// // When a setter exists in the BindableProperty. You can even set its value.
        /// nameProperty.Value = "The label text will automatically update even here!"
        /// </code>
        /// </example>
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