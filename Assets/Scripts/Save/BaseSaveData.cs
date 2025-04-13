using System;
using System.Collections.Generic;
using Buildings;

namespace Save
{
    [Serializable]
    public class BaseSaveData
    {
        public List<BuildingState> buildings = new();
    }
}