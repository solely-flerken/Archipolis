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
            var buildings = Resources.LoadAll<BuildingData>("Buildings");
            foreach (var data in buildings)
            {
                if (data == null || data.prefab == null)
                {
                    Debug.LogError($"Invalid BuildingData or missing prefab for {data?.identifier}");
                    continue;
                }

                BuildingMap[data.identifier] = data;
            }
        }

        public static BuildingData GetBuildingByID(string id)
        {
            return BuildingMap.GetValueOrDefault(id);
        }

        public static List<BuildingData> GetAllBuildings()
        {
            return new List<BuildingData>(BuildingMap.Values);
        }
    }
}