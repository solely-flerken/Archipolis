using System.Collections.Generic;
using Events;
using Save;
using Terrain;
using UnityEngine;

namespace Hex
{
    public class HexMapManager : MonoBehaviour
    {
        public static HexMapManager Instance { get; private set; }

        public Dictionary<HexCoordinate, HexCellData> HexMap { get; private set; } = new();

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void Start()
        {
            HexMap = MapGenerator.Instance.GenerateTerrainMesh();

            EventSystem.Instance.OnLoadGame += HandleLoadGame;
        }

        private void OnDestroy()
        {
            EventSystem.Instance.OnLoadGame += HandleLoadGame;
        }

        private void HandleLoadGame(BaseSaveData saveData)
        {
            MapGenerator.Instance.mapParameters = saveData.mapGenerationParameters;
            HexMap = MapGenerator.Instance.GenerateTerrainMesh();
        }
    }
}