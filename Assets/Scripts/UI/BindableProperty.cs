using System;
using Unity.Properties;

namespace UI
{
    public class BindableProperty<T>
    {
        private readonly Func<T> _getter;
        private readonly Action<T> _setter;

        private BindableProperty(Func<T> getter, Action<T> setter)
        {
            _getter = getter;
            _setter = setter;
        }

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

        public static BindableProperty<T> Bind(Func<T> getter, Action<T> setter = null) => new(getter, setter);
    }
}