using Buildings;
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
            var buttonContainer = root.Q<VisualElement>("ButtonContainer");

            // TODO: Create like a static building storage which stores all unique available buildings in the game,
            // with a unique identifier and prefab
            // Get the count of the list and create buttons according to the number. Name each button after that identifier or a readable name

            var allBuildings = BuildingDatabase.GetAllBuildings();

            foreach (var building in allBuildings)
            {
                var button = new Button(() => { EventSystem.Instance.InvokeOnPlaceBuildingUI(building.ID); })
                {
                    text = building.Name,
                    name = "Build" + building.ID
                };

                button.AddToClassList("button");
                buttonContainer.Add(button);
            }
        }
    }
}