using System.Collections;
using System.Collections.Generic;
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
        }

        // TODO: Display accurate loading information (in GenerateTerrainMesh which we need to make async)
        public IEnumerator Initialize(BaseSaveData saveData)
        {
            MapGenerator.Instance.mapParameters = saveData.mapGenerationParameters;
            HexMap = MapGenerator.Instance.GenerateTerrainMesh();
            yield return null;
        }
    }
}