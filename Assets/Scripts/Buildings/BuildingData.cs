using System;
using Hex;
using UnityEngine;
using UnityEngine.Serialization;

namespace Buildings
{
    [CreateAssetMenu(fileName = "BuildingData", menuName = "Buildings/BuildingData", order = 0)]
    public class BuildingData : ScriptableObject
    {
        public string identifier = Guid.NewGuid().ToString();
        public string buildingName;
        public GameObject prefab;
        public int initialYaw;
        public HexCoordinate[] footprint = { new(0, 0) };
    }
}