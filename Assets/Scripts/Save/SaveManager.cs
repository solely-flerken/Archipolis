using System.Linq;
using Buildings;
using Events;
using GameResources;
using Terrain;

namespace Save
{
    public static class SaveManager
    {
        public static readonly ISaveSystem SaveSystem = new LocalJsonSaveSystem();

        public static string SaveGame(string fileName = null)
        {
            var currentSaveData = new BaseSaveData
            {
                buildings = BuildingManager.Buildings.Select(x => x.buildingData).ToList(),
                resources = ResourceManager.Resources.Select(x => x.Value.ToDto()).ToList(),
                resourceFlowTimers = ResourceManager.ResourceFlowTimers.ToSerializableList(),
                mapGenerationParameters = MapGenerator.Instance.mapParameters
            };

            var filePath = SaveSystem.Save(currentSaveData, fileName);
            EventSystem.Instance.InvokeSaveGame(currentSaveData);
            return filePath;
        }

        public static BaseSaveData LoadGame(string fileName)
        {
            var currentSaveData = SaveSystem.Load(fileName);
            return currentSaveData;
        }

        public static BaseSaveData LoadLatestGame()
        {
            var latestSaveFile = SaveSystem.GetLatestSaveFile();
            var currentSaveData = SaveSystem.Load(latestSaveFile);
            return currentSaveData;
        }

        public static void DeleteGame(string parameter)
        {
            SaveSystem.Delete(parameter);
        }
    }
}