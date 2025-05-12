using System;
using System.IO;
using System.Linq;
using UnityEngine;

namespace Save
{
    public class LocalJsonSaveSystem : ISaveSystem
    {
        private static readonly string SaveDirectory = Path.Combine(Application.persistentDataPath, "saves");

        public string Save(BaseSaveData saveData, string fileName = null)
        {
            // Generate timestamp-based filename if none provided
            if (string.IsNullOrEmpty(fileName))
            {
                fileName = $"save_{DateTime.Now:yyyyMMdd_HHmmss}.json";
            }

            var savePath = ToSavePath(fileName);

            Directory.CreateDirectory(SaveDirectory);

            var json = JsonUtility.ToJson(saveData, true);
            File.WriteAllText(savePath, json);
            return savePath;
        }

        public BaseSaveData Load(string fileName)
        {
            if (string.IsNullOrEmpty(fileName))
            {
                return new BaseSaveData();
            }

            var loadPath = ToSavePath(fileName);

            if (!File.Exists(loadPath))
            {
                return new BaseSaveData();
            }

            var json = File.ReadAllText(loadPath);
            return JsonUtility.FromJson<BaseSaveData>(json);
        }

        public string GetLatestSaveFile()
        {
            var saveFiles = GetSaveFiles();

            if (saveFiles.Length == 0)
            {
                return null;
            }

            var latestSaveFile = saveFiles
                .OrderByDescending(File.GetLastWriteTime)
                .First();

            return latestSaveFile;
        }

        public bool Delete(string fileName)
        {
            if (string.IsNullOrEmpty(fileName))
            {
                return false;
            }

            var savePath = ToSavePath(fileName);

            if (!File.Exists(savePath))
            {
                return false;
            }

            File.Delete(savePath);
            return true;
        }

        public string[] GetSaveFiles()
        {
            if (!Directory.Exists(SaveDirectory))
            {
                return Array.Empty<string>();
            }

            return Directory.GetFiles(SaveDirectory, "*.json");
        }

        public bool HasAnySave()
        {
            return GetSaveFiles().Length > 0;
        }

        public bool SaveExists(string fileName)
        {
            return File.Exists(ToSavePath(fileName));
        }

        private static string ToSavePath(string fileName)
        {
            // Add .json extension if not present
            if (!fileName.EndsWith(".json", StringComparison.OrdinalIgnoreCase))
            {
                fileName += ".json";
            }

            return Path.Combine(SaveDirectory, fileName);
        }
    }
}