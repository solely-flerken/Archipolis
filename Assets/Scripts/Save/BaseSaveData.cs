using System;
using System.Collections.Generic;
using Buildings;
using GameResources;
using Terrain;

namespace Save
{
    [Serializable]
    public class BaseSaveData
    {
        public List<BuildingData> buildings = new();
        public List<ResourceAmountDto> resources = new();
        public List<ResourceFlowTimerEntry> resourceFlowTimers = new();
        public MapGenerationParameters mapGenerationParameters = MapGenerationParameters.Default();
    }
}