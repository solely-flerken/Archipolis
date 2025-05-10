using System.Linq;
using Buildings;
using Events;
using GameResources;
using Terrain;
using UnityEngine;

namespace Save
{
    public class SaveManager : MonoBehaviour
    {
        public static SaveManager Instance { get; private set; }

        private ISaveSystem _saveSystem;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);

                // TODO: We want to use a database later
                _saveSystem = new LocalJsonSaveSystem();
            }
            else
            {
                Destroy(gameObject);
            }
        }

        public string SaveGame(string fileName = null)
        {
            var currentSaveData = new BaseSaveData
            {
                buildings = BuildingManager.Buildings.Select(x => x.buildingData).ToList(),
                resources = ResourceManager.Resources.Select(x => x.Value.ToDto()).ToList(),
                resourceFlowTimers = ResourceManager.ResourceFlowTimers.ToSerializableList(),
                mapGenerationParameters = MapGenerator.Instance.mapParameters
            };

            var filePath = _saveSystem.Save(currentSaveData, fileName);
            EventSystem.Instance.InvokeSaveGame(currentSaveData);
            return filePath;
        }

        public BaseSaveData LoadGame(string fileName)
        {
            var currentSaveData = _saveSystem.Load(fileName);
            EventSystem.Instance.InvokeLoadGame(currentSaveData);
            return currentSaveData;
        }

        public BaseSaveData LoadLatestGame()
        {
            var currentSaveData = _saveSystem.LoadLatest();
            EventSystem.Instance.InvokeLoadGame(currentSaveData);
            return currentSaveData;
        }

        public void DeleteGame(string parameter)
        {
            _saveSystem.Delete(parameter);
        }
    }
}