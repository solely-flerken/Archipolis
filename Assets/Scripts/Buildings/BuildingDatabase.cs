using System.Collections.Generic;
using UnityEngine;

namespace Buildings
{
    public static class BuildingDatabase
    {
        private static readonly Dictionary<string, BuildingBlueprint> BuildingMap = new();

        static BuildingDatabase()
        {
            Initialize();
        }

        private static void Initialize()
        {
            var buildings = Resources.LoadAll<BuildingBlueprint>("Buildings");
            foreach (var data in buildings)
            {
                if (data == null || data.prefab == null)
                {
                    Debug.LogError($"Invalid blueprint data or missing prefab for {data?.identifier}");
                    continue;
                }

                BuildingMap[data.identifier] = data;
            }
        }

        public static BuildingBlueprint GetBuildingByID(string id)
        {
            return BuildingMap.GetValueOrDefault(id);
        }

        public static List<BuildingBlueprint> GetAllBuildings()
        {
            return new List<BuildingBlueprint>(BuildingMap.Values);
        }
    }
}