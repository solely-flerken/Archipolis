using System.Collections.Generic;
using UnityEngine;

namespace Buildings
{
    public static class BuildingDatabase
    {
        private static readonly Dictionary<string, BuildingData> BuildingMap = new();

        static BuildingDatabase()
        {
            Initialize();
        }

        private static void Initialize()
        {
            AddBuilding("SampleBuilding", Resources.Load<GameObject>("Prefabs/Buildings/SampleBuilding"));
        }

        private static void AddBuilding(string name, GameObject prefab)
        {
            if (prefab == null)
            {
                Debug.LogError($"Prefab for {name} is missing!");
                return;
            }

            var newBuilding = new BuildingData(name, prefab);
            BuildingMap[newBuilding.ID] = newBuilding;
        }

        public static BuildingData GetBuildingByID(string identifier)
        {
            return BuildingMap.GetValueOrDefault(identifier);
        }

        public static List<BuildingData> GetAllBuildings()
        {
            return new List<BuildingData>(BuildingMap.Values);
        }
    }
}