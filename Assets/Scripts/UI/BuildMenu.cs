using Buildings;
using Events;
using State;
using UnityEngine;
using UnityEngine.UIElements;

namespace UI
{
    public class BuildMenu : MonoBehaviour
    {
        private void OnEnable()
        {
            var root = GetComponent<UIDocument>().rootVisualElement;
            var buttonContainer = root.Q<VisualElement>("primaryTools");

            var allBuildings = BuildingDatabase.GetAllBuildings();

            foreach (var building in allBuildings)
            {
                var button = new Button(() => { EventSystem.Instance.InvokeOnPlaceBuildingUI(building.ID); })
                {
                    text = building.Name,
                    name = "build-" + building.ID
                };

                button.AddToClassList("button");
                buttonContainer.Add(button);
            }

            var deleteButton = root.Q<Button>("delete");
            deleteButton.clicked += () =>
            {
                var newState = !StateManager.Instance.IsBulldoze;
                StateManager.Instance.SetBulldoze(newState);
            };
        }
    }
}