using System;
using System.Collections.Generic;
using GameResources;
using Hex;
using UnityEngine;

namespace Buildings
{
    [CreateAssetMenu(fileName = "BuildingBlueprint", menuName = "Building/Blueprint", order = 0)]
    public class BuildingBlueprint : ScriptableObject
    {
        public string identifier = Guid.NewGuid().ToString();
        public string buildingName;
        public Sprite icon;
        public GameObject prefab;
        public HexCoordinate[] footprint = { new(0, 0) };
        public List<ResourceAmount> buildingCosts;
        public ResourceFlow resourceFlow;
    }
}