using System;
using Hex;
using UnityEngine;

namespace Buildings
{
    [CreateAssetMenu(fileName = "BuildingBlueprint", menuName = "Building/Blueprint", order = 0)]
    public class BuildingBlueprint : ScriptableObject
    {
        public string identifier = Guid.NewGuid().ToString();
        public string buildingName;
        public GameObject prefab;
        public HexCoordinate[] footprint = { new(0, 0) };
    }
}