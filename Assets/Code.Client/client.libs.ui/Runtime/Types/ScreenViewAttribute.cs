using System;
using Client.Libs.UI.Internal;

namespace Client.Libs.UI.Types
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public sealed class ScreenViewAttribute : Attribute
    {
        public Type ViewType { get; }

        public ScreenViewAttribute(Type viewType)
        {
            if (viewType == null)
                throw new ArgumentNullException(nameof(viewType));

            if (!typeof(UIScreenView).IsAssignableFrom(viewType))
                throw new ArgumentException($"Screen view type must inherit from {nameof(UIScreenView)}.", nameof(viewType));

            ViewType = viewType;
        }
    }
}
