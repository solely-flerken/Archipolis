using System.IO;
using UnityEngine;

namespace Save
{
    public class LocalJsonSaveSystem : ISaveSystem
    {
        private readonly string _savePath = Path.Combine(Application.persistentDataPath, "saves", "save.json");

        public void Save(BaseSaveData saveData)
        {
            var folderPath = Path.GetDirectoryName(_savePath);
            if (!Directory.Exists(folderPath) && !string.IsNullOrEmpty(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }

            var json = JsonUtility.ToJson(saveData, true);
            File.WriteAllText(_savePath, json);
        }

        public BaseSaveData Load()
        {
            var saveData = new BaseSaveData();

            if (File.Exists(_savePath))
            {
                var json = File.ReadAllText(_savePath);
                saveData = JsonUtility.FromJson<BaseSaveData>(json);
            }
            
            return saveData;
        }
    }
}