using Buildings;
using Events;
using State;
using UnityEngine.UIElements;

namespace UI
{
    public class BuildMenu : UserInterfaceBase
    {
        private void Start()
        {
            IsVisibleInitially = true;
            
            Root = GetComponent<UIDocument>().rootVisualElement;
            var buttonContainer = Root.Q<VisualElement>("primaryTools");

            Root.style.display = DisplayStyle.None;
            
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

            var deleteButton = Root.Q<Button>("delete");
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

            var editButton = Root.Q<Button>("edit");
            editButton.clicked += () =>
            {
                if (ModeStateManager.Instance.ModeState == Mode.Building)
                {
                    ModeStateManager.Instance.SetMode(Mode.Idle);
                }
                else
                {
                    ModeStateManager.Instance.SetMode(Mode.Building);
                }
            };
        }
    }
}