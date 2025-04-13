using System.Linq;
using Buildings;
using Events;
using UnityEngine;

namespace Save
{
    public class SaveManager : MonoBehaviour
    {
        public static SaveManager Instance { get; private set; }

        private ISaveSystem _saveSystem;
        
        [SerializeField]
        private BaseSaveData currentSaveData;

        private void Start()
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

        public void SaveGame()
        {
            currentSaveData = new BaseSaveData
            {
                buildings = BuildingManager.Buildings.Select(x => x.buildingState).ToList()
            };

            _saveSystem.Save(currentSaveData);
            EventSystem.Instance.InvokeSaveGame(currentSaveData);
        }

        public void LoadGame()
        {
            currentSaveData = _saveSystem.Load();
            EventSystem.Instance.InvokeLoadGame(currentSaveData);
        }
    }
}