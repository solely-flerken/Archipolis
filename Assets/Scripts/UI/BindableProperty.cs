using System;
using Unity.Properties;

namespace UI
{
    /// <summary>
    /// A bindable property that allows for easy binding between a property and a UI element.
    /// Provides getter and setter functionality for synchronization between data and UI.
    /// </summary>
    /// <typeparam name="T">The type of the property value.</typeparam>
    public class BindableProperty<T>
    {
        private readonly Func<T> _getter;
        private readonly Action<T> _setter;

        private BindableProperty(Func<T> getter, Action<T> setter)
        {
            _getter = getter;
            _setter = setter;
        }

        /// <summary>
        /// The CreateProperty attribute marks the property to be recognized by Unity's internal property system, enabling it to be used in data binding with Unity's UI Toolkit
        /// </summary>
        [CreateProperty]
        public T Value
        {
            get => _getter();
            set
            {
                if (_setter == null)
                    throw new InvalidOperationException("Setter not defined for this BindableProperty.");
                _setter(value);
            }
        }

        /// <summary>
        /// Creates a new instance of <see cref="BindableProperty{T}"/>.
        /// </summary>
        /// <param name="getter">The function to get the value of the property.</param>
        /// <param name="setter">The action to set the value of the property.</param>
        /// <returns>A new instance of the <see cref="BindableProperty{T}"/> class.</returns>
        public static BindableProperty<T> Bind(Func<T> getter, Action<T> setter = null) => new(getter, setter);
    }
}