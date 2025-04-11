using System;
using UI;

namespace Buildings
{
    [Serializable]
    public class BuildingState
    {
        public string identifier = Guid.NewGuid().ToString();
        public int currentPopulation;
        public bool isActive;
    }

    public class ViewModelBuildingState
    {
        public readonly BindableProperty<string> Identifier;
        public readonly BindableProperty<int> CurrentPopulation;
        public readonly BindableProperty<bool> IsActive;

        public ViewModelBuildingState(BuildingState buildingState)
        {
            Identifier = BindableProperty<string>.Bind(() => buildingState.identifier);
            CurrentPopulation = BindableProperty<int>.Bind(() => buildingState.currentPopulation);
            IsActive = BindableProperty<bool>.Bind(() => buildingState.isActive, (x) => buildingState.isActive = x);
        }
    }
}