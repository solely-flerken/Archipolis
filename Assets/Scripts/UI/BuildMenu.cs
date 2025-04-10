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
                var button = new Button(() => { EventSystem.Instance.InvokeOnPlaceBuildingUI(building.identifier); })
                {
                    text = building.buildingName,
                    name = "build-" + building.identifier
                };

                button.AddToClassList("button");
                buttonContainer.Add(button);
            }

            var deleteButton = root.Q<Button>("delete");
            deleteButton.clicked += () =>
            {
                if (ModeStateManager.Instance.ModeState == Mode.Bulldozing)
                {
                    ModeStateManager.Instance.SetMode(Mode.Idle);
                }
                else
                {
                    ModeStateManager.Instance.SetMode(Mode.Bulldozing);
                }
            };
        }
    }
}