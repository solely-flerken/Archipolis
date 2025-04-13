using System;
using Hex;
using UI;

namespace Buildings
{
    [Serializable]
    public class BuildingState
    {
        public string identifier = Guid.NewGuid().ToString();
        public string blueprintIdentifier;
        public int currentPopulation;
        public bool isActive;
        
        public HexCoordinate origin;
        /// <summary>
        /// Defines the footprint of the building.
        /// Footprint is build by calculating adjacent hex cells with the offsets.
        /// </summary>
        public HexCoordinate[] footprint;
        
        // Rotation in 60-degree increments
        public int yaw; 
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