using UnityEngine.UIElements;

namespace Utils
{
    public interface IUserInterface
    {
        VisualElement Root { get; set; }
        bool IsVisible => Root.style.display == DisplayStyle.Flex;
    }
}