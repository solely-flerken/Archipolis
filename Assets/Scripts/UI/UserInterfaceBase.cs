using UnityEngine;
using UnityEngine.UIElements;
using Utils;

namespace UI
{
    public abstract class UserInterfaceBase : MonoBehaviour, IUserInterface
    {
        public VisualElement Root { get; set; }
        public bool IsVisibleInitially { get; set; }
        public bool IsVisible => Root?.style.display == DisplayStyle.Flex;

        public void Show()
        {
            Root.style.display = DisplayStyle.Flex;
        }

        public void Hide()
        {
            Root.style.display = DisplayStyle.None;
        }
    }
}