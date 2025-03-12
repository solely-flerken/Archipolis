using Events;
using UnityEngine;
using UnityEngine.UIElements;

namespace UI
{
    public class UI : MonoBehaviour
    {
        private void OnEnable()
        {
            var root = GetComponent<UIDocument>().rootVisualElement;
        
            // TODO: Create like a static building storage which stores all unique available buildings in the game,
            // with a unique identifier and prefab
            // Get the count of the list and create buttons according to the number. Name each button after that identifier or a readable name
            
            var buttonBuild1 = root.Q<Button>("Build1");
            var buttonBuild2 = root.Q<Button>("Build2");
            var buttonBuild3 = root.Q<Button>("Build3");

            buttonBuild1.clicked += () =>
            {
                EventSystem.Instance.InvokeOnPlaceBuildingUI("");
            };
            buttonBuild2.clicked += () =>
            {
                EventSystem.Instance.InvokeOnPlaceBuildingUI("");
            };
            buttonBuild3.clicked += () =>
            {
                EventSystem.Instance.InvokeOnPlaceBuildingUI("");
            };
        }
    }
}
